using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using RosMessageTypes.Sensor;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;
using System;
using Marus.Core;
using Marus.Sensors;
using Marus.NoiseDistributions;

[RequireComponent(typeof(Sonar2D),typeof(TfStreamer_modified))]
public class Sonar2DROS_modified : MonoBehaviour
{
    Sonar2D sensor;

    ROSConnection ros;
    [SerializeField] string topic = "/sensor/multibeam_sonar";
    [SerializeField] float publishFrequency = 20f;
    double _lastPublishTime = 0;
    float publishPeriodSeconds => 1 / publishFrequency;
    bool shouldPublishSensor => (Time.time - _lastPublishTime) >= publishPeriodSeconds;

    [Header("Sonar Noise")]
    [SerializeField] NoiseParameters noise;

    [Tooltip("Default false which produce a planar point cloud. If true, it will add noise to the vertical axis, generating a 3D point cloud.")]
    [SerializeField] bool addNoiseForVerticalAxis = false;
    // Start is called before the first frame update
    void Start()
    {
        sensor = GetComponent<Sonar2D>();
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PointCloud2Msg>(topic);
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
        List<byte> data = new List<byte>();
        int pointCount = sensor.pointsCopy.Length;

        foreach (var point in sensor.pointsCopy)
        {
            var tmp = TfExtensions.Unity2Map(point);
            //Debug.Log($"x={tmp.x}, y={tmp.y}, z={tmp.z}");
            data.AddRange(BitConverter.GetBytes(tmp.x + Noise.Sample(noise)));
            data.AddRange(BitConverter.GetBytes(tmp.y + Noise.Sample(noise)));

            if (addNoiseForVerticalAxis)
                tmp.z += Noise.Sample(noise);
            data.AddRange(BitConverter.GetBytes(tmp.z));
        }

        PointCloud2Msg msg = new PointCloud2Msg
        {
            header = new HeaderMsg
            {
                stamp = Clock.stamp,
                frame_id = sensor.frameId
            },
            height = 1,
            width = (uint)pointCount,
            fields = new PointFieldMsg[]
            {
                new PointFieldMsg("x", 0, 7, 1),
                new PointFieldMsg("y", 4, 7, 1),
                new PointFieldMsg("z", 8, 7, 1)
            },
            is_bigendian = false,
            point_step = 12,
            row_step = (uint)(12 * pointCount),
            data = data.ToArray(),
            is_dense = true
        };

        ros.Publish(topic, msg);
    }
}
