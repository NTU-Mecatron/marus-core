using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Pixhawk))]
public class ControllerInUnity : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;   
    string actionMap = "Ardusub";

    Vector3 horizontalInput;
    Vector3 verticalInput;
    Vector3 yawInput;
    [SerializeField] bool invertVertical = false;

    Rigidbody rigidbody;
    float force;
    float torque;

    Pixhawk pixhawk;
    TorpedoLauncherROS torpedoLauncher;
    RoboticArmControllerROS roboticArmController;
    CameraControllerROS cameraController;
    MarkerDropperROS markerDropper;
    GripperControllerROS gripperController;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerInput.SwitchCurrentActionMap(actionMap);
        pixhawk = GetComponent<Pixhawk>();
        force = pixhawk.force;
        torque = pixhawk.torque;

        roboticArmController = GetComponentInChildren<RoboticArmControllerROS>();
        Assert.IsNotNull(roboticArmController, "Robotic arm controller not found");

        cameraController = GetComponentInChildren<CameraControllerROS>();
        Assert.IsNotNull(cameraController, "Camera controller not found");

        gripperController = GetComponentInChildren<GripperControllerROS>();
        Assert.IsNotNull(gripperController, "Gripper controller not found");
    }

    public void OnLeftJoystick(InputAction.CallbackContext value)
    {
        // Control forward/backward and left/right translation
        Vector2 input = value.ReadValue<Vector2>();
        horizontalInput = new Vector3(input.x, 0, input.y);
    }

    public void OnRightJoystick(InputAction.CallbackContext value)
    {
        // Control yaw and vertical translation
        Vector2 input = value.ReadValue<Vector2>();
        yawInput = new Vector3(0, input.x, 0);

        if (invertVertical)
            verticalInput = new Vector3(0, input.y, 0);
        else
            verticalInput = new Vector3(0, -input.y, 0);

        if (Mathf.Abs(input.y) >= 0.1)
            pixhawk.pauseDepthHold = true;
        else
            pixhawk.pauseDepthHold = false;

        if (Mathf.Abs(input.x) >= 0.1)
            pixhawk.pauseHeadingHold = true;
        else
            pixhawk.pauseHeadingHold = false;

    }

    public void OnArm(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            pixhawk.isArm = true;
            Debug.Log("Armed");
        }
    }

    public void OnDisarm(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            pixhawk.isArm = false;
            Debug.Log("Disarmed");
        }
    }

    public void OnSetManualMode(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            pixhawk.depthHoldMode = false;
            pixhawk.headingHoldMode = false;
            Debug.Log("Switched to manual mode");
        }
    }

    public void OnSetStabilizeMode(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            pixhawk.depthHoldMode = false;
            pixhawk.headingHoldMode = true;
            Debug.Log("Switched to stabilize mode");
        }
    }

    public void OnSetDepthHoldMode(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            pixhawk.depthHoldMode = true;
            pixhawk.headingHoldMode = true;
            Debug.Log("Switched to depth hold mode");
        }
    }

    public void OnLaunchTorpedo(InputAction.CallbackContext value)
    {
        torpedoLauncher = GetComponentInChildren<TorpedoLauncherROS>();
        if (torpedoLauncher == null)
        {
            Debug.LogError("Torpedo launcher not found");
            return;
        }

        if (value.started)
        {
            StartCoroutine(torpedoLauncher.LaunchTorpedoAfterDelay(0.3f));
        }
    }

    public void OnDropMarker(InputAction.CallbackContext value)
    {
        markerDropper = GetComponentInChildren<MarkerDropperROS>();
        if (markerDropper == null)
        {
            Debug.LogError("Marker dropper not found");
            return;
        }

        if (value.started)
        {
            StartCoroutine(markerDropper.DropBall());
        }
    }

    public void OnExtendArm1(InputAction.CallbackContext value)
    {
        if (value.started) 
            roboticArmController.IncrementArmPosition(1, 0);
    }

    public void OnRetractArm1(InputAction.CallbackContext value)
    {
        if (value.started) roboticArmController.IncrementArmPosition(-1, 0);
    }

    public void OnExtendArm2(InputAction.CallbackContext value)
    {
        if (value.started) roboticArmController.IncrementArmPosition(0, 1);
    }

    public void OnRetractArm2(InputAction.CallbackContext value)
    {
        if (value.started) roboticArmController.IncrementArmPosition(0, -1);
    }

    public void OnRotateCamera(InputAction.CallbackContext value)
    {
        Vector2 input = value.ReadValue<Vector2>();
        cameraController.XAngle += - input.y * 2f;    // turn 2 degree every keystroke
        cameraController.YAngle += input.x * 2f;    // turn 2 degree every keystroke
        cameraController.transform.localEulerAngles = new Vector3(cameraController.XAngle, cameraController.YAngle, 0);
    }

    public void OnOpenGripperOpen(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            if (gripperController == null) gripperController = GetComponentInChildren<GripperControllerROS>();
            gripperController.OpenGripper();
        }
    }

    public void OnCloseGripper(InputAction.CallbackContext value)
    {   
        if (value.started)
        {
            if (gripperController == null) gripperController = GetComponentInChildren<GripperControllerROS>();
            gripperController.CloseGripper();
        }
    }

    private void FixedUpdate()
    {
        if (!pixhawk.isArm) return;
        // Apply forces to the vehicle
        rigidbody.AddRelativeForce(horizontalInput * force);
        rigidbody.AddRelativeForce(verticalInput * force);
        rigidbody.AddRelativeTorque(yawInput * torque);
    }
}
