using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.CommonMsgSrv;

public class CameraControllerROS : MonoBehaviour
{
    float xAxisAngle = 0;
    float yAxisAngle = 0;
    [SerializeField] float xAngleIncrement = 2f;
    [SerializeField] float yAngleIncrement = 2f;

    ROSConnection ros;
    [SerializeField] string topic = "pixhawk/control/keyboard_input";
    [SerializeField] string xAngleService = "/camera/x_angle";
    [SerializeField] string yAngleService = "/camera/y_angle";

    public float XAngle
    {
        get { return xAxisAngle; }
        set { xAxisAngle = value; }
    }

    public float YAngle 
    {
        get { return yAxisAngle; }
        set { yAxisAngle = value; }
    }

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<StringMsg>(topic, CameraControlCallback);
        ros.ImplementService<SendFloatRequest, SendFloatResponse>(xAngleService, XAngleCallback);
        ros.ImplementService<SendFloatRequest, SendFloatResponse>(yAngleService, YAngleCallback);
    }

    void CameraControlCallback(StringMsg msg)
    {
        string data = msg.data;

        if (data == "i") xAxisAngle -= xAngleIncrement;
        else if (data == "k") xAxisAngle += xAngleIncrement;
        else if (data == "j") yAxisAngle -= yAngleIncrement;
        else if (data == "l") yAxisAngle += yAngleIncrement;

        transform.localEulerAngles = new Vector3(xAxisAngle, yAxisAngle, 0);
    }

    SendFloatResponse XAngleCallback(SendFloatRequest request)
    {
        xAxisAngle = request.data;
        transform.localEulerAngles = new Vector3(xAxisAngle, yAxisAngle, 0);

        return new SendFloatResponse
        {
            status = true
        };
    }

    SendFloatResponse YAngleCallback(SendFloatRequest request)
    {
        yAxisAngle = request.data;
        transform.localEulerAngles = new Vector3(xAxisAngle, yAxisAngle, 0);

        return new SendFloatResponse
        {
            status = true
        };
    }
}
