using Marus.Networking;
using NWH.Common.Utility;
using RosMessageTypes.CommonMsgSrv;
using Actionlib = RosMessageTypes.Actionlib;
using RosMessageTypes.Std;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using RosMessageTypes.Actionlib;

public class Pixhawk : MonoBehaviour
{
    public float force = 8f;
    public float torque = 1f;
    [SerializeField] float maxLinearVelocity = 1f;
    [SerializeField] float maxAngularVelocity = 1f;

    float dt;
    new Rigidbody rigidbody;
    Vector3 forceVector = Vector3.zero;
    Vector3 torqueVector = Vector3.zero;

    ROSConnection ros;
    [SerializeField] string mainTopic = "/pixhawk/control/manual_control_normalized";
    [SerializeField] string headingTopic = "/pixhawk/control/set_target_heading";
    [SerializeField] string deltaHeadingTopic = "/pixhawk/control/set_target_heading_delta";
    [SerializeField] string depthTopic = "/pixhawk/control/set_target_depth";
    [SerializeField] string deltaDepthTopic = "/pixhawk/control/set_target_depth_delta";

    string headingTopicResult;
    string depthTopicResult;

    [SerializeField] PID headingPID = new PID(3f, 0f, 1.5f, -1f, 1f);
    [SerializeField] PID depthPID = new PID(3f, 0f, 1.5f, -1f, 1f);

    public bool headingHoldMode = true;
    public bool depthHoldMode = true;
    [HideInInspector] public bool pauseDepthHold = false;
    [HideInInspector] public bool pauseHeadingHold = false;

    public bool isArm = true; // Arm or disarm mode

    bool isHeadingSet = false;
    float headingSetpoint;
    float lastSmallHeadingControlEffortTime = 0f;

    bool isDepthSet = false;
    float depthSetpoint;
    float lastSmallDepthControlEffortTime = 0f;

    [SerializeField] float verticalDragCoefficient = 10f;
    [SerializeField] float yawDragCoefficient = 1.6f;

    void Start()
    {
        dt = Time.fixedDeltaTime;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.maxAngularVelocity = maxAngularVelocity;
        rigidbody.maxLinearVelocity = maxLinearVelocity;

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<Float32MultiArrayMsg>(mainTopic, MainControlCallback);

        RegisterActionServers();
    }

    void RegisterActionServers()
    {
        ros.Subscribe<Float32Msg>(headingTopic, HeadingControlCallback);
        ros.RegisterPublisher<BoolMsg>(headingTopic + "/result");
        
        ros.Subscribe<Float32Msg>(deltaHeadingTopic, DeltaHeadingControlCallback);
        ros.RegisterPublisher<BoolMsg>(deltaHeadingTopic + "/result");

        ros.Subscribe<Float32Msg>(depthTopic, DepthControlCallback);
        ros.RegisterPublisher<BoolMsg>(depthTopic + "/result");

        ros.Subscribe<Float32Msg>(deltaDepthTopic, DeltaDepthControlCallback);
        ros.RegisterPublisher<BoolMsg>(deltaDepthTopic + "/result");
    }

    void FixedUpdate()
    {
        if (!isArm) return;

        if (isHeadingSet)
        {
            float currentHeading = rigidbody.rotation.eulerAngles.y;
            
            // This block of code is to handle the case when the heading setpoint is near 0 or 360 degrees
            if (headingSetpoint - currentHeading > 180f)
            {
                currentHeading += 360f;
            }
            else if (headingSetpoint - currentHeading < -180f)
            {
                currentHeading -= 360f;
            }

            float controlOutput = headingPID.Update(headingSetpoint, currentHeading, dt);
            Debug.Log("Current heading: " + currentHeading + " Control output: " + controlOutput + " Heading setpoint: " + headingSetpoint);

            rigidbody.AddTorque(controlOutput * transform.up * torque);

            if (Mathf.Abs(controlOutput) < 0.2f)
            {
                // If the pid output is small enough for 2 seconds, then consider the heading is reached
                if (lastSmallHeadingControlEffortTime == 0f)
                {
                    lastSmallHeadingControlEffortTime = Time.time;
                }
                else if (Time.time - lastSmallHeadingControlEffortTime > 2f)
                {
                    isHeadingSet = false;
                    lastSmallHeadingControlEffortTime = 0f;
                    ros.Publish(headingTopicResult, new BoolMsg(true));     // Send a message back to inform that it's done
                }
            }
            else
            {
                // If the pid output is not small enough, then reset the timer
                lastSmallHeadingControlEffortTime = 0f;
            }
        }

        if (isDepthSet) {             
            float currentDepth = rigidbody.position.y;

            float controlOutput = depthPID.Update(depthSetpoint, currentDepth, dt);
            Debug.Log("Current depth: " + currentDepth + " Control output: " + controlOutput + " Depth setpoint: " + depthSetpoint);

            rigidbody.AddForce(controlOutput * transform.up * force);

            if (Mathf.Abs(controlOutput) < 0.2f)
            {
                if (lastSmallDepthControlEffortTime == 0f)
                {
                    lastSmallDepthControlEffortTime = Time.time;
                }
                else if (Time.time - lastSmallDepthControlEffortTime > 2f)
                {
                    isDepthSet = false;
                    lastSmallDepthControlEffortTime = 0f;
                    ros.Publish(depthTopicResult, new BoolMsg(true));
                }
            }
            else
            {
                lastSmallDepthControlEffortTime = 0f;
            }
        }

        rigidbody.AddRelativeForce(forceVector * force);
        rigidbody.AddRelativeTorque(torqueVector * torque);

        if (depthHoldMode && !pauseDepthHold && !isDepthSet)
        {
            // Simulate depth hold
            MotionControlUtils.ApplyVerticalDrag(rigidbody, verticalDragCoefficient, 0.1f);
        }
        if (headingHoldMode && !pauseHeadingHold && !isHeadingSet)
        {
            // Simulate yaw hold
            MotionControlUtils.ApplyYawDrag(rigidbody, yawDragCoefficient, 0.1f);
        }
    }

    // Update is called once per frame
    void MainControlCallback(Float32MultiArrayMsg msg)
    {
        float forward = msg.data[0];
        float sideway = msg.data[1];
        float up = msg.data[2];
        float yaw = msg.data[3];
        Debug.Log("Forward: " + forward + " Sideway: " + sideway + " Up: " + up + " Yaw: " + yaw);
        forceVector = new Vector3(sideway, up, forward);
        torqueVector = new Vector3(0, yaw, 0);

        if (up != 0f) pauseDepthHold = true;
        else pauseDepthHold = false;

        if (yaw != 0f) pauseHeadingHold = true;
        else pauseHeadingHold = false;
    }

    void HeadingControlCallback(Float32Msg msg)
    {
        isHeadingSet = true;
        headingSetpoint = msg.data;
        Debug.Log("Received command to set heading to " + headingSetpoint + " degree");
        headingTopicResult = headingTopic + "/result";
    }

    void DeltaHeadingControlCallback(Float32Msg msg)
    {
        isHeadingSet = true;
        headingSetpoint = rigidbody.rotation.eulerAngles.y + msg.data;
        Debug.Log("Received command to turn heading by " + msg.data + " degree");
        headingTopicResult = deltaHeadingTopic + "/result";
    }

    void DepthControlCallback(Float32Msg msg)
    {
        isDepthSet = true;
        depthSetpoint = msg.data;
        Debug.Log("Received command to set depth to " + depthSetpoint + " meter");
        depthTopicResult = depthTopic + "/result";
    }

    void DeltaDepthControlCallback(Float32Msg msg)
    {
        isDepthSet = true;
        depthSetpoint = rigidbody.position.y + msg.data;
        Debug.Log("Received command to change depth by " + msg.data + " meter");
        depthTopicResult = deltaDepthTopic + "/result";
    }
}
