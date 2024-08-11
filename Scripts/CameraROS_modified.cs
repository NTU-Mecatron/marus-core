using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using RosMessageTypes.Sensor;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;
using System;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

[RequireComponent(typeof(Camera))]
public class CameraROS_modified : MonoBehaviour
{
    Camera sensor;
    Texture2D camText;

    Vector2Int _resolution = new Vector2Int(640, 480);
    RenderTexture rt;

    ROSConnection ros;
    [SerializeField] string frameId = "vehicle/camera_frame";
    [SerializeField] string topic = "/sensor/camera";
    [SerializeField] float publishFrequency = 20f;
    double _lastPublishTime = 0;
    float publishPeriodSeconds => 1 / publishFrequency;
    bool shouldPublishSensor => (Time.time - _lastPublishTime) >= publishPeriodSeconds;

    void Start()
    {
        sensor = GetComponent<Camera>();
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImageMsg>(topic);

        rt = new RenderTexture(_resolution.x, _resolution.y, 32, RenderTextureFormat.ARGBFloat);
        //sensor.targetTexture = rt;

        camText = new Texture2D(sensor.targetTexture.width, sensor.targetTexture.height, TextureFormat.RGBAFloat, false);
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
        var oldRT = RenderTexture.active;
        RenderTexture.active = sensor.targetTexture;
        sensor.Render();

        // Copy the pixels from the GPU into a texture so we can work with them
        // For more efficiency you should reuse this texture, instead of creating and disposing them every time
        camText.ReadPixels(new Rect(0, 0, sensor.targetTexture.width, sensor.targetTexture.height), 0, 0);
        camText.Apply();
        RenderTexture.active = oldRT;

        // Encode the texture as an ImageMsg, and send to ROS
        ImageMsg imageMsg = camText.ToImageMsg(
            new HeaderMsg()
            {
                stamp = Clock.stamp,
                frame_id = frameId // Set the frame ID
            });
        ros.Publish(topic, imageMsg);
    }
}
