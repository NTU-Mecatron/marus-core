using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using RosMessageTypes.Std;
using Marus.Sensors.Primitive;
using Marus.Core;
using Marus.Networking;
using RosMessageTypes.Geometry;

[RequireComponent(typeof(PoseSensor))]
public class PoseROS_modified : MonoBehaviour
{
    ROSConnection ros;
    [SerializeField] string topic = "/vehicle/true_pose";
    [SerializeField] float publishFrequency = 20f;
    double _lastPublishTime = 0;
    float publishPeriodSeconds => 1 / publishFrequency;
    bool shouldPublishSensor => (Time.time - _lastPublishTime) >= publishPeriodSeconds;

    PoseSensor sensor;
    // Start is called before the first frame update
    void Start()
    {
        sensor = GetComponent<PoseSensor>();
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(topic);
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
        PoseStampedMsg msg = new PoseStampedMsg()
        {
            header = new HeaderMsg()
            {
                frame_id = sensor.frameId,
                stamp = Clock.stamp
            },
            pose = new PoseMsg()
            {
                position = sensor.position.Unity2Map().AsPointMsgRos(),
                orientation = sensor.orientation.Unity2Map().AsMsgRos()
            }
        };
        ros.Publish(topic, msg);
    }
}
