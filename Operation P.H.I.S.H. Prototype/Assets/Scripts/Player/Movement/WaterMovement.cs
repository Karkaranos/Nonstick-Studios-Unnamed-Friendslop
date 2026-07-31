/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    07/30/2026
Date Last Modified : 	07/30/2026
Brief Description : 	Actually defines and handles water movement

External Resources :    	
***************************************************/
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerController))]
public class WaterMovement : Movement
{
    private PlayerController pc;
    [SerializeField] private float minCameraYClamp = 120f;
    [SerializeField] private float maxCameraYClamp = 60f;

    [SerializeField] private float baseLandMovementSpeed = 1f;
    [SerializeField] private float tapJumpHeightPercent = .5f;
    [SerializeField] private float fullJumpHeight;
    [SerializeField] private float timeToMaxAcceleration = 5f;
    [SerializeField] private float maxAccelerationMultiplier = 3f;
    [SerializeField] private float landGravity = -10f;
    private float accelleration = 1f;
    private Coroutine shiftHold;

    bool ascending;
    bool descending;

    Vector2 rotation;
    Rigidbody rb;

    /// <summary>
    /// Grabs initial references and sets initial variables
    /// </summary>
    private void Start()
    {
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        rotation = new Vector2(pc.CameraRotationParent.eulerAngles.y, pc.CameraRotationParent.eulerAngles.x);
    }


    /// <summary>
    /// Override from movement base class to add extra functionality
    /// Disables the player's gravity when in swim mode
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        rb.useGravity = false;
    }

    /// <summary>
    /// Override from movement base class to add extra functionality
    /// Enables the player's gravity when exiting swim mode
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        rb.useGravity = false;
    }

    /// <summary>
    /// Override from Movement base class
    /// Adjusts the delta using sensitivity and fixed timee
    /// Rotates the camera between clamps
    /// </summary>
    /// <param name="cameraVector">Vector2 containing the mouse's delta </param>
    protected override void OnMouseMove(Vector2 cameraVector)
    {
        Vector2 adjustedDelta = cameraVector * pc.CameraSensitivity * Time.fixedDeltaTime;

        rotation.x -= adjustedDelta.y;
        rotation.x = Mathf.Clamp(rotation.x, minCameraYClamp, maxCameraYClamp);
        rotation.y += adjustedDelta.x;

        pc.CameraRotationParent.localEulerAngles = rotation;
        Debug.Log("LOOK");
    }

    /// <summary>
    /// Override from Movement base class
    /// Reads a value from the OnMove event and adjust velocity accordingly
    /// </summary>
    /// <param name="moveVector"></param>
    protected override void OnMove(Vector2 moveVector)
    {
        Vector3 newValue = ((pc.CameraRotationParent.forward * moveVector.y) + (pc.CameraRotationParent.right * moveVector.x)) * baseLandMovementSpeed * 100f * accelleration * Time.fixedDeltaTime;

        if (ascending || descending)
        {
            newValue.y = rb.linearVelocity.y;
        }
        else
        {
            newValue.y = 0;
        }

        rb.linearVelocity = newValue;

        Debug.Log("MOVE");
    }

    /// <summary>
    /// Override from Movement base class
    /// </summary>
    protected override void OnEClicked()
    {
        Debug.Log("E");
    }

    /// <summary>
    /// Override from movement base class
    /// Should make the player ascend in the water
    /// </summary>
    protected override void OnSpaceStarted(bool fullyPerformed)
    {
        // TODO: Make the player ascend

        Debug.Log("Space Started");
    }

    /// <summary>
    /// Override from movement base class
    /// </summary>
    protected override void OnSpaceFinished()
    {
        // TODO: Stop the player's ascension

        Debug.Log("Space Finished");
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
    protected override void OnShiftStarted()
    {
        if (shiftHold == null)
        {
            shiftHold = StartCoroutine(Accelerate());
        }
        Debug.Log("Shift Start");
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
    protected override void OnShiftFinished()
    {
        if (shiftHold != null)
        {
            StopCoroutine(shiftHold);
            shiftHold = null;
            accelleration = 1;
        }
        Debug.Log("Shift Finished");
    }

    /// <summary>
    /// Override from movement base class
    /// Should make the player descend
    /// </summary>
    protected override void OnControlStarted()
    {
        Debug.Log("Control Started");
    }

    /// <summary>
    /// override from movement base class
    /// Should make the player descend
    /// </summary>
    protected override void OnControlFinished()
    {
        Debug.Log("Control Finished");
    }

    /// <summary>
    /// Accellerates the player while shift is held
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Accelerate()
    {
        float timer = 0f;
        while (timer < timeToMaxAcceleration)
        {
            accelleration = 1f + (timer / timeToMaxAcceleration) * maxAccelerationMultiplier;
            Mathf.Clamp(accelleration, 1f, maxAccelerationMultiplier);

            timer += Time.fixedDeltaTime;

            Debug.Log(accelleration);
            yield return null;
        }
    }
}