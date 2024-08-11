using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.CommonMsgSrv;

public class TorpedoLauncherROS : MonoBehaviour
{
    Rigidbody rigidbody;
    FixedJoint fixedJoint;
    [SerializeField] Rigidbody connectedBody;
    [SerializeField] Transform torpedoSpawnPoint;

    [Tooltip("Speed at which the torpedo is launched in m/s")]
    [SerializeField] float launchSpeed = 5f;
    [SerializeField] string service = "/torpedo_launch";
    [SerializeField] GameObject torpedoPrefab;
    ROSConnection ros;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.maxLinearVelocity = launchSpeed;

        fixedJoint = GetComponent<FixedJoint>();
        fixedJoint.breakForce = Mathf.Infinity;
        fixedJoint.breakTorque = Mathf.Infinity;

        ros = ROSConnection.GetOrCreateInstance();
        ros.ImplementService<SendBoolRequest, SendBoolResponse>(service, Callback);
    }

    SendBoolResponse Callback(SendBoolRequest request)
    {
        Debug.Log("Received request to launch torpedo");
        StartCoroutine(LaunchTorpedoAfterDelay(0.3f));

        return new SendBoolResponse
        {
            status = true
        };
    }

    public IEnumerator LaunchTorpedoAfterDelay(float delay)
    {
        // Set break force and torque to 0
        fixedJoint.breakForce = 0.00f;
        fixedJoint.breakTorque = 0.00f;

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Launch the torpedo
        rigidbody.AddForce(transform.forward * launchSpeed, ForceMode.VelocityChange);
        transform.parent = null;
        Debug.Log("Torpedo launched");

        // Wait for 0.1 seconds before instantiating a new torpedo
        yield return new WaitForSeconds(0.1f);

        GameObject newTorpedo = Instantiate(torpedoPrefab, torpedoSpawnPoint.position, torpedoSpawnPoint.rotation, connectedBody.transform);
        fixedJoint = newTorpedo.GetComponent<FixedJoint>();
        if (fixedJoint == null)
        {
            fixedJoint = newTorpedo.AddComponent<FixedJoint>();
        }
        fixedJoint.connectedBody = connectedBody;
        fixedJoint.breakForce = Mathf.Infinity;
        fixedJoint.breakTorque = Mathf.Infinity;
    }
}
