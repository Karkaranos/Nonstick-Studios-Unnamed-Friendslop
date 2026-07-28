/*************************************************
Author Names : 		    Clare Grady
Date Created : 		    07/22/2026
Date Last Modified : 	07/22/202
Brief Description : 	Actually defines and handles land movement

External Resources :    	
***************************************************/
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerController))]
public class LandMovement : Movement
{
    private PlayerController pc;
    [SerializeField] private float minCameraYClamp = 120f;
    [SerializeField] private float maxCameraYClamp = 60f;

    [SerializeField] private float baseLandMovementSpeed = 1f;
    [SerializeField] private float tapJumpHeightPercent = .5f;
    [SerializeField] private float fullJumpHeight;
    [SerializeField] private float timeToMaxAcceleration = 5f;
    [SerializeField] private float maxAccelerationMultiplier = 3f;
    private float accelleration = 1f;
    private Coroutine shiftHold;

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
        Vector3 newValue = ((pc.CameraRotationParent.forward * moveVector.y) + (pc.CameraRotationParent.right * moveVector.x)) * baseLandMovementSpeed * 100f * accelleration *  Time.fixedDeltaTime;

        if(!Grounded())
        {
            newValue.y = rb.linearVelocity.y;
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
    /// </summary>
    protected override void OnSpaceStarted(bool fullyPerformed)
    {
        if(Grounded())
        {
            // If the key was tapped and not held, do a smaller jump
            if(fullyPerformed)
            {
                rb.AddForce(pc.CameraRotationParent.up * fullJumpHeight * tapJumpHeightPercent, ForceMode.Impulse);
            }
            // Otherwise, do the full height
            else
            {
                rb.AddForce(pc.CameraRotationParent.up * fullJumpHeight, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.Log("Not Grounded");
        }
            Debug.Log("Space Started");
    }

    /// <summary>
    /// Override from movement base class
    /// </summary>
    protected override void OnSpaceFinished()
    {
        Debug.Log("Space Finished");
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
    protected override void OnShiftStarted()
    {
        if(shiftHold == null)
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
        if(shiftHold != null)
        {
            StopCoroutine(shiftHold);
            shiftHold = null;
            accelleration = 1;
        }
        Debug.Log("Shift Finished");
    }
    protected override void OnControlStarted()
    {
        Debug.Log("Control Started");
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
    protected override void OnControlFinished()
    {
        Debug.Log("Control Finished");
    }

    /// <summary>
    /// Checks if the player is on the ground
    /// </summary>
    /// <returns></returns>
    protected bool Grounded()
    {
        RaycastHit hit;
        float groundCheckDistance = transform.localScale.y + .2f;
        return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);
    }

    /// <summary>
    /// Accellerates the player while shift is held
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Accelerate()
    {
        float timer = 0f;
        while(true)
        {
            accelleration = 1f + (timer / timeToMaxAcceleration) * maxAccelerationMultiplier;
            Mathf.Clamp(accelleration, 1f, maxAccelerationMultiplier);

            timer += Time.fixedDeltaTime;

            yield return null;
        }
    }
}
