using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.CommonMsgSrv;
using UnityEngine.Assertions;

public class GripperControllerROS : MonoBehaviour
{
    bool isGripperOpen = false;
    Rigidbody rbInGripper;
    FixedJoint fixedJoint;
    [SerializeField] List<string> tagsToCheck;

    [SerializeField] string service = "/pixhawk/gripper_service";
    ROSConnection ros;

    public bool IsGripperOpen
    {
        get { return isGripperOpen; }
        set { isGripperOpen = value; }
    }

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.ImplementService<SendBoolRequest, SendBoolResponse>(service, Callback);

        fixedJoint = GetComponent<FixedJoint>();
        rbInGripper = fixedJoint.connectedBody;
        Assert.IsNotNull(rbInGripper, "Nothing in gripper at the start");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tagsToCheck.Contains(other.tag))
        {
            Debug.Log("Object in gripper: " + other.tag);
            rbInGripper = other.attachedRigidbody;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (tagsToCheck.Contains(other.tag))
        {
            Debug.Log("Object left gripper: " + other.tag);
            rbInGripper = null;
        }
    }

    SendBoolResponse Callback(SendBoolRequest request)
    {
        bool msg = request.request;
        if (msg == true)
            CloseGripper();
        else
            OpenGripper();
        
        return new SendBoolResponse
        {
            status = true
        };
    }

    public void CloseGripper()
    {
        Debug.Log("Received request to close gripper");
        if (isGripperOpen == false)
        {
            Debug.Log("Gripper is already closed");
        }
        else
        {
            isGripperOpen = false;
            StartCoroutine(PickupObject(0.3f));
        }
    }

    public void OpenGripper()
    {
        Debug.Log("Received request to open gripper");
        if (isGripperOpen == true)
        {
            Debug.Log("Gripper is already open");
        }
        else
        {
            isGripperOpen = true;
            StartCoroutine(DropObject(0.3f));
        }
    }

    IEnumerator DropObject(float delay)
    {
        if (fixedJoint == null || rbInGripper == null)
        {
            Debug.Log("Nothing in gripper to drop");
        }
        else
        {
            fixedJoint.breakForce = 0.00f;
            fixedJoint.breakTorque = 0.00f;
            Debug.Log("Object dropped");
            rbInGripper.transform.parent = null;
            rbInGripper = null;
        }

        yield return new WaitForSeconds(delay);
    }

    IEnumerator PickupObject(float delay)
    {
        if (rbInGripper == null)
        {
            Debug.Log("Nothing to pick up");
            yield break;
        }

        // If there is something in the gripper, proceed to create a fixed joint
        if (fixedJoint == null)
        {
            fixedJoint = gameObject.AddComponent<FixedJoint>();
        }

        fixedJoint.connectedBody = rbInGripper;
        fixedJoint.breakForce = Mathf.Infinity;
        fixedJoint.breakTorque = Mathf.Infinity;
        rbInGripper.transform.parent = transform;
        Debug.Log("Object picked up");

        yield return new WaitForSeconds(delay);
    }
}
