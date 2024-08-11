using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MotionControlUtils
{
    public static void ApplyVerticalDrag(Rigidbody rigidbody, float dragCoefficient, float verticalVelocityThreshold)
    {
        // Calculate the vertical component of the velocity
        Vector3 verticalVelocity = Vector3.Project(rigidbody.velocity, rigidbody.transform.up);

        // Check if the vertical velocity is below the threshold
        if (verticalVelocity.magnitude < verticalVelocityThreshold)
        {
            // Stop the vertical movement
            Vector3 v = rigidbody.velocity;
            rigidbody.velocity = new Vector3(v.x, 0, v.z);
        }
        else
        {
            // Apply drag force proportional to the vertical velocity
            Vector3 dragForce = -dragCoefficient * verticalVelocity;
            rigidbody.AddForce(dragForce);
        }
    }

    public static void ApplyYawDrag(Rigidbody rigidbody, float dragCoefficient, float yawVelocityThreshold)
    {
        // Calculate the yaw (rotational) component of the angular velocity
        Vector3 yawVelocity = Vector3.Project(rigidbody.angularVelocity, rigidbody.transform.up);

        // Check if the yaw velocity is below the threshold
        if (yawVelocity.magnitude < yawVelocityThreshold)
        {
            // Stop the yaw movement
            Vector3 v = rigidbody.angularVelocity;
            rigidbody.angularVelocity = new Vector3(v.x, 0, v.z);
        }
        else
        {
            // Apply drag torque proportional to the yaw velocity
            Vector3 dragTorque = -dragCoefficient * yawVelocity;
            rigidbody.AddTorque(dragTorque);
        }
    }
}
