using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using MathNet.Numerics.LinearAlgebra;

public class RoboticArmControllerROS : MonoBehaviour
{
    [SerializeField] GameObject arm1;
    [SerializeField] GameObject arm2;
    [SerializeField] float arm1Increment = 2f;
    [SerializeField] float arm2Increment = 2f;

    HingeJoint hinge1;
    HingeJoint hinge2;
    float hinge1Min; float hinge1Max;
    float hinge2Min; float hinge2Max;

    float armAngle1 = 0;
    float armAngle2 = 0;

    ROSConnection ros;
    [SerializeField] string armTopic = "pixhawk/control/robotic_arm";
    
    void Start()
    {
        hinge1 = arm1.GetComponent<HingeJoint>();
        hinge2 = arm2.GetComponent<HingeJoint>();
        hinge1Min = hinge1.limits.min;
        hinge1Max = hinge1.limits.max;
        hinge2Min = hinge2.limits.min;
        hinge2Max = hinge2.limits.max;

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<Float32MultiArrayMsg>(armTopic, ArmControlCallback); 

        SetArmPosition(armAngle1, armAngle2);
    }

    void ArmControlCallback(Float32MultiArrayMsg msg)
    {
        float angle1 = msg.data[0];
        float angle2 = msg.data[1];

        armAngle1 = Mathf.Clamp(armAngle1, hinge1Min, hinge1Max);
        armAngle2 = Mathf.Clamp(armAngle2, hinge2Min, hinge2Max);

        SetArmPosition(angle1, angle2);
    }

    void SetArmPosition(float angle1, float angle2)
    {
        JointSpring spring1 = hinge1.spring;
        spring1.targetPosition = angle1;
        hinge1.spring = spring1;

        JointSpring spring2 = hinge2.spring;
        spring2.targetPosition = angle2;
        hinge2.spring = spring2;
    }

    public void IncrementArmPosition(float increment1, float increment2)
    {
        // 1 for extend, 0 for stationary, -1 for retract
        armAngle1 += increment1 * arm1Increment;
        armAngle2 += increment2 * arm2Increment;

        armAngle1 = Mathf.Clamp(armAngle1, hinge1Min, hinge1Max);
        armAngle2 = Mathf.Clamp(armAngle2, hinge2Min, hinge2Max);

        SetArmPosition(armAngle1, armAngle2);
    }
}
