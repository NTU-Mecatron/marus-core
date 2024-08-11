using Marus.Sensors.Primitive;
using Marus.Core;
using Marus.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;

[RequireComponent(typeof(ImuSensor))]
public class ImuROS_modified : MonoBehaviour
{
    ROSConnection ros;
    [SerializeField] string topic = "/sensor/imu";
    [SerializeField] float publishFrequency = 20f;
    double _lastPublishTime = 0;
    float publishPeriodSeconds => 1 / publishFrequency;
    bool shouldPublishSensor => (Time.time - _lastPublishTime) >= publishPeriodSeconds;

    ImuSensor sensor;

    void Start()
    {
        sensor = GetComponent<ImuSensor>();
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImuMsg>(topic);
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
        ImuMsg msg = new ImuMsg()
        {
            header = new HeaderMsg()
            {
                frame_id = sensor.frameId,
                stamp = Clock.stamp
            },
            linear_acceleration = sensor.linearAcceleration.Unity2Body().AsMsgRos(),
            angular_velocity = (-sensor.angularVelocity).Unity2Body().AsMsgRos(),
            orientation = sensor.orientation.Unity2Map().AsMsgRos()
        };
        ros.Publish(topic, msg);
    }
}
