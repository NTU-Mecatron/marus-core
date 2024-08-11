using Marus.Sensors.Primitive;
using Marus.Core;
using Marus.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;
using System;
using RosMessageTypes.Geometry;

[RequireComponent(typeof(DvlSensor))]
public class DvlROS_modified : MonoBehaviour
{
    DvlSensor sensor;

    ROSConnection ros;
    [SerializeField] string topic = "/sensor/dvl";
    [SerializeField] float publishFrequency = 20f;
    double _lastPublishTime = 0;
    float publishPeriodSeconds => 1 / publishFrequency;
    bool shouldPublishSensor => (Time.time - _lastPublishTime) >= publishPeriodSeconds;

    void Start()
    {
        sensor = GetComponent<DvlSensor>();
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistStampedMsg>(topic);
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
        TwistStampedMsg msg = new TwistStampedMsg()
        {
            header = new HeaderMsg()
            {
                stamp = new TimeMsg()
                {
                    sec = (uint)publishTime,
                    nanosec = (uint)((publishTime - Math.Floor(publishTime)) * Clock.k_NanoSecondsInSeconds)
                }
            },
            twist = new TwistMsg()
            {
                linear = sensor.groundVelocity.Unity2Body().AsMsgRos()
            }
        };
        ros.Publish(topic, msg);
    }
}
