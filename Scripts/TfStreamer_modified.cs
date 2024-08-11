using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Geometry;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;
using RosMessageTypes.Tf2;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;
using Marus.CustomInspector;
using TreeEditor;
using Marus.Core;
using System;
using Marus.Networking;

public class TfStreamer_modified : MonoBehaviour
{
    /// <summary>
    /// Transform of the parent object.
    /// Orientation and translation are calculated in relationship to this object.
    /// </summary>
    [SerializeField] Transform ParentTransform;
    [SerializeField] string ParentFrameId;
    [SerializeField] string FrameId;
    [SerializeField] bool AddOffset = false;

    [ConditionalHideInInspector("AddOffset", false)]
    [SerializeField] Vector3 TranslationOffset;

    [ConditionalHideInInspector("AddOffset", false)]
    [SerializeField] Vector3 RotationOffset;

    Quaternion _rotation;
    Vector3 _translation;

    ROSConnection ros;
    const string topic = "/tf";
    [SerializeField] float publishFrequency = 10f;
    double _lastPublishTime = 0;
    float publishPeriodSeconds => 1 / publishFrequency;
    bool shouldPublishTf => (Time.time - _lastPublishTime) >= publishPeriodSeconds;

    #if UNITY_EDITOR
    protected void Reset()
    {
        // Return the parent transform if it exists, otherwise return the current transform
        var veh = (transform.parent != null) ? transform.parent : transform;
        FrameId = veh.name;
        ParentFrameId = veh.name;
        // if not same object, assume sensor is attached to vehicle
        if (veh != transform)
        {
            FrameId = FrameId + $"/{gameObject.name}_frame";
            ParentFrameId = ParentFrameId + "/base_link";
            ParentTransform = veh.transform;
        }
        else
        {
            // if same object assume it's vehicle, and assign base_link and map parent
            FrameId = FrameId + "/base_link";
            ParentFrameId = "map";
        }
    }
    #endif


    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        //ros.RegisterPublisher<TFMessageMsg>(topic);    
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldPublishTf)
        {
            UpdateTransform();
            PublishTf();
            _lastPublishTime = Time.time;
        }
    }

    void UpdateTransform()
    {
        if (ParentTransform != null)
        {
            _translation = ParentTransform.InverseTransformPoint(transform.position);
            _rotation = (Quaternion.Inverse(ParentTransform.transform.rotation) * transform.rotation);
            if (AddOffset)
            {
                _translation += TranslationOffset;
                _rotation *= Quaternion.Euler(RotationOffset);
            }
            // if parent is assigned, assume it is local position (body frame) and transform to (forward-left-up) FLU
            _translation = _translation.Unity2Body();
            _rotation = _rotation.Unity2Body();
        }
        else
        {
            // if no parent is assigned, assume it is global position and transform to ENU frame
            _rotation = transform.rotation.Unity2Map(); //
            _translation = transform.position.Unity2Map();

        }
    }

    void PublishTf()
    {
        TFMessageMsg msg = new TFMessageMsg
        {
            transforms = new TransformStampedMsg[]
            {
                new TransformStampedMsg
                {
                    header = new HeaderMsg
                    {
                        stamp = Clock.stamp,
                        frame_id = ParentFrameId,
                    },
                    child_frame_id = FrameId,
                    transform = new TransformMsg
                    {
                        translation = _translation.AsMsgRos(),
                        rotation = _rotation.AsMsgRos()
                    }
                }
            }
        };
        ros.Publish(topic, msg);
    }
}
