using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Marus.Sensors.Primitive;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using RosMessageTypes.Std;

[RequireComponent(typeof(DepthSensor))]
public class DepthROS_modified : MonoBehaviour
{
    DepthSensor sensor;
    double depth;

    ROSConnection ros;
    [SerializeField] string topic = "/sensor/depth";
    [SerializeField] float publishFrequency = 20f;
    double _lastPublishTime = 0;
    float publishPeriodSeconds => 1 / publishFrequency;
    bool shouldPublishSensor => (Time.time - _lastPublishTime) >= publishPeriodSeconds;

    void Start()
    {
        sensor = GetComponent<DepthSensor>();

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32Msg>(topic);
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldPublishSensor)
        {
            PublishMsg();
            _lastPublishTime = Time.time;
        }
    }

    void PublishMsg()
    {
        double publishTime = Clock.time;
        depth = sensor.Depth;
        Float32Msg depthMsg = new Float32Msg
        {
            data = -(float)depth
        };
        ros.Publish(topic, depthMsg);
    }
}
