using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.CommonMsgSrv;
using System.Security.Authentication.ExtendedProtection;

public class MarkerDropperROS : MonoBehaviour
{
    FixedJoint fixedJoint;
    [SerializeField] Rigidbody connectedBody;
    [SerializeField] Transform markerSpawnPoint;
    [SerializeField] GameObject markerPrefab;

    [SerializeField] string service = "/marker_drop";
    ROSConnection ros;
    void Start()
    {
        fixedJoint = GetComponent<FixedJoint>();
        fixedJoint.breakForce = Mathf.Infinity;
        fixedJoint.breakTorque = Mathf.Infinity;

        ros = ROSConnection.GetOrCreateInstance();
        ros.ImplementService<SendBoolRequest, SendBoolResponse>(service, Callback);
    }

    SendBoolResponse Callback(SendBoolRequest request)
    {
        Debug.Log("Received request to drop marker");
        StartCoroutine(DropBall());
        
        return new SendBoolResponse
        {
            status = true
        };
    }

    public IEnumerator DropBall()
    {
        fixedJoint.breakForce = 0.001f;
        fixedJoint.breakTorque = 0.001f;
        transform.parent = null;

        yield return new WaitForSeconds(0.1f);
        Debug.Log("Marker dropped");

        GameObject marker = Instantiate(markerPrefab, markerSpawnPoint.position, markerSpawnPoint.rotation, connectedBody.transform);
        fixedJoint = marker.GetComponent<FixedJoint>();
        if (fixedJoint == null)
        {
            fixedJoint = marker.AddComponent<FixedJoint>();
        }
        fixedJoint.connectedBody = connectedBody;
        fixedJoint.breakForce = Mathf.Infinity;
        fixedJoint.breakTorque = Mathf.Infinity;
    }
}
