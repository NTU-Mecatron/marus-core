using Marus.Networking;
using NWH.Common.Utility;
using RosMessageTypes.Pixhawk;
using RosMessageTypes.Std;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;


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
    Vector3 targetForceVector = Vector3.zero;
    Vector3 targetTorqueVector = Vector3.zero;

    ROSConnection ros;
    [SerializeField] string mainTopic = "/pixhawk/control/manual_control_normalized";
    [SerializeField] string headingTopic = "/pixhawk/control/set_target_heading";
    [SerializeField] string deltaHeadingTopic = "/pixhawk/control/set_target_heading_delta";
    [SerializeField] string depthTopic = "/pixhawk/control/set_target_depth";
    [SerializeField] string deltaDepthTopic = "/pixhawk/control/set_target_depth_delta";
    [SerializeField] string armService = "/pixhawk/cmd/arming";
    [SerializeField] string setModeService = "/pixhawk/cmd/set_mode";
    string mainFilteredTopic = "/pixhawk/control/manual_control_filtered_unity";
    string xControlTopic = "/pixhawk/control/manual_control_x";
    string yControlTopic = "/pixhawk/control/manual_control_y";
    string zControlTopic = "/pixhawk/control/manual_control_z";
    string rControlTopic = "/pixhawk/control/manual_control_r";

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
    [SerializeField] float minControlThreshold = 0.1f;

    void Start()
    {
        dt = Time.fixedDeltaTime;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.maxAngularVelocity = maxAngularVelocity;
        rigidbody.maxLinearVelocity = maxLinearVelocity;

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<Float32MultiArrayMsg>(mainTopic, MainControlCallback);
        ros.Subscribe<Float32Msg>(xControlTopic, msg => targetForceVector.z = msg.data);
        ros.Subscribe<Float32Msg>(yControlTopic, msg => targetForceVector.x = msg.data);
        ros.Subscribe<Float32Msg>(zControlTopic, msg =>
        {
            targetForceVector.y = msg.data;
            pauseDepthHold = Mathf.Abs(msg.data) > 0.1f;
        });
        ros.Subscribe<Float32Msg>(rControlTopic, msg =>
        {
            targetTorqueVector.y = msg.data;
            pauseHeadingHold = Mathf.Abs(msg.data) > 0.1f;
        });
        ros.RegisterPublisher<ManualControlMsg>(mainFilteredTopic);
        ros.ImplementService<EnableArmDisarmRequest, EnableArmDisarmResponse>(armService, ArmDisarmCallback);
        ros.ImplementService<SetModeRequest, SetModeResponse>(setModeService, SetModeCallback);

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

        // Calculate filtered control
        forceVector = IncrementVector3(forceVector, targetForceVector, 0.007f);
        torqueVector = IncrementVector3(torqueVector, targetTorqueVector, 0.007f);
        ManualControlMsg manualControlMsg = new ManualControlMsg
        {
            x = forceVector.z,
            y = torqueVector.x,
            z = forceVector.y,
            r = torqueVector.y
        };
        ros.Publish(mainFilteredTopic, manualControlMsg);
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
        forward = (Mathf.Abs(forward) < minControlThreshold) ? 0f : forward;
        sideway = (Mathf.Abs(sideway) < minControlThreshold) ? 0f : sideway;
        up = (Mathf.Abs(up) < minControlThreshold) ? 0f : up;
        yaw = (Mathf.Abs(yaw) < minControlThreshold) ? 0f : yaw;
        Debug.Log("Forward: " + forward + " Sideway: " + sideway + " Up: " + up + " Yaw: " + yaw);
        targetForceVector = new Vector3(sideway, up, forward);
        targetTorqueVector = new Vector3(0, yaw, 0);

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

    EnableArmDisarmResponse ArmDisarmCallback(EnableArmDisarmRequest msg)
    {
        isArm = msg.is_enable;
        Debug.Log("Received command to " + (isArm ? "arm" : "disarm"));

        return new EnableArmDisarmResponse { is_success = true };
    }

    SetModeResponse SetModeCallback(SetModeRequest msg)
    {
        string mode = msg.mode;
        Debug.Log("Received command to set mode to " + msg.mode);
        SetModeResponse response = new SetModeResponse { is_success = true };
        switch (mode)
        {
            case "MANUAL":
                headingHoldMode = false;
                depthHoldMode = false;
                return response;
            case "STABILIZE":
                headingHoldMode = true;
                depthHoldMode = false;
                return response;
            case "ALT_HOLD":
                headingHoldMode = true;
                depthHoldMode = true;
                return response;
            default:
                Debug.Log("Mode not support in Unity");
                return new SetModeResponse { is_success = false };
        }
    }

    Vector3 IncrementVector3(Vector3 currentVector, Vector3 targetVector, float increment)
    {
        currentVector.x = Mathf.MoveTowards(currentVector.x, targetVector.x, increment);
        currentVector.y = Mathf.MoveTowards(currentVector.y, targetVector.y, increment);
        currentVector.z = Mathf.MoveTowards(currentVector.z, targetVector.z, increment);
        return currentVector;
    }
}
