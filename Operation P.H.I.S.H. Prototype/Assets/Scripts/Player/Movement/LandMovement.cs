/*************************************************
Author Names : 		    Clare Grady, Cade Naylor
Date Created : 		    07/22/2026
Date Last Modified : 	07/28/202
Brief Description : 	Actually defines and handles land movement

External Resources :    	
***************************************************/
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerController))]
public class LandMovement : Movement
{
    private PlayerController pc;
    [SerializeField] private float minCameraYClamp = 120f;
    [SerializeField] private float maxCameraYClamp = 60f;
    [SerializeField] private Transform armsParent;

    [SerializeField] private float baseLandMovementSpeed = 1f;
    [SerializeField] private float tapJumpHeightPercent = .5f;
    [SerializeField] private float fullJumpHeight;
    [SerializeField] private float timeToMaxAcceleration = 5f;
    [SerializeField] private float maxAccelerationMultiplier = 3f;
    [SerializeField] private float landGravity = -10f;
    private float accelleration = 1f;
    private Coroutine shiftHold;

    private IInteractable lookingAt;
    private IInteractable interactingWith;
    private bool interacting;
    [SerializeField] private float sightDistance = 2f;

    bool jumpThisFrame;

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
    /// also lowkey this wont work as well for multiplayer but for now i'm throwing rayxast look logic in here
    /// </summary>
    /// <param name="cameraVector">Vector2 containing the mouse's delta </param>
    protected override void OnMouseMove(Vector2 cameraVector)
    {
        if (interactingWith != null && interactingWith.ToString().Contains("ShipMovementControllers"))
        {
            return;
        }

        Vector2 adjustedDelta = cameraVector * pc.CameraSensitivity * Time.fixedDeltaTime;

        rotation.x -= adjustedDelta.y;
        rotation.x = Mathf.Clamp(rotation.x, minCameraYClamp, maxCameraYClamp);
        rotation.y += adjustedDelta.x;

        pc.CameraRotationParent.localEulerAngles = rotation;

        Vector3 armsRotation = armsParent.transform.localEulerAngles;
        armsRotation.y = rotation.y;
        armsRotation.x = Mathf.Clamp(rotation.x, -40, 40);
        armsParent.transform.localEulerAngles = armsRotation;

        if(LookingAtObject())
        {
            pc.CrosshairImage.sprite = pc.InteractableSprite;
        }
        else
        {
            pc.CrosshairImage.sprite = pc.StandardSprite;
        }
        
        Debug.Log("LOOK");
    }

    /// <summary>
    /// Override from Movement base class
    /// Reads a value from the OnMove event and adjust velocity accordingly
    /// </summary>
    /// <param name="moveVector"></param>
    protected override void OnMove(Vector2 moveVector)
    {
        if(interactingWith != null && interactingWith.ToString().Contains("ShipMovementControllers"))
        {
            return;
        }

        Vector3 newValue = ((pc.CameraRotationParent.forward * moveVector.y) + (pc.CameraRotationParent.right * moveVector.x)) * baseLandMovementSpeed * 100f * accelleration *  Time.fixedDeltaTime;

        if(!Grounded() || jumpThisFrame)
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
        if(lookingAt != null)
        {
            if(interactingWith == null)
            {
                interactingWith = lookingAt;
                interactingWith.EnterInteract(pc);
            }
            else
            {
                if(lookingAt!= interactingWith)
                {
                    interactingWith.ExitInteract();
                    interactingWith = lookingAt;
                    interactingWith.EnterInteract(pc);
                }
                else
                {
                    interactingWith.ExitInteract();
                    interactingWith = null;
                }
            }
        }
        else if (interactingWith != null)
        {
            interactingWith.ExitInteract();
            if(interactingWith == lookingAt)
            {
                interactingWith.EnterHover();
            }
            interactingWith = null;
        }

            Debug.Log("E");
    }

    /// <summary>
    /// Override from movement base class
    /// </summary>
    protected override void OnSpaceStarted(bool fullyPerformed)
    {
        if (Grounded())
        {
            jumpThisFrame = true;
            float jumpForce = Mathf.Sqrt(fullJumpHeight * landGravity * -2f);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
        else
        {

        }
            Debug.Log("Space Started");
    }

    /// <summary>
    /// Override from movement base class
    /// </summary>
    protected override void OnSpaceFinished()
    {
        jumpThisFrame = false;
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

    /// <summary>
    /// Override from movement base class
    /// </summary>
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
        float groundCheckDistance = transform.localScale.y+ .1f;
        return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);
    }

    /// <summary>
    /// Checks if the player is looking at an object that can be interacted with
    /// </summary>
    /// <returns>Returns true if they are</returns>
    protected override bool LookingAtObject()
    {
        RaycastHit hit;
        Vector3 direction = pc.CameraRotationParent.forward;

        if(Physics.Raycast(pc.CameraRotationParent.transform.position, direction, out hit, sightDistance))
        {
            if(hit.transform.GetComponent<IInteractable>()!= null)
            {
                if(hit.transform.GetComponent<ShipMovementControllers>() != null &&
                !ShipDiveController.Instance.Diving)
                {
                    return false;
                }

                if(interactingWith != null && hit.transform.GetComponent<IInteractable>() == interactingWith)
                {
                    return true;
                }

                if(lookingAt != null && hit.transform.GetComponent<IInteractable>() != lookingAt)
                {
                    lookingAt.ExitHover();
                }

                lookingAt = hit.transform.GetComponent<IInteractable>();
                lookingAt.EnterHover();

                pc.CrosshairImage.sprite = pc.InteractableSprite;

                return true;
            }
        }

        if (lookingAt != null)
        {
            if (lookingAt != interactingWith)
            {
                lookingAt.ExitHover();
            }
            lookingAt = null;
        }

        pc.CrosshairImage.sprite = pc.StandardSprite;
        return false;
    }

    /// <summary>
    /// Accellerates the player while shift is held
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Accelerate()
    {
        float timer = 0f;
        while(timer < timeToMaxAcceleration)
        {
            accelleration = 1f + (timer / timeToMaxAcceleration) * maxAccelerationMultiplier;
            Mathf.Clamp(accelleration, 1f, maxAccelerationMultiplier);

            timer += Time.fixedDeltaTime;

            Debug.Log(accelleration);
            yield return null;
        }
    }


    /// <summary>
    /// Override from Movement class
    /// Sends the last camera angle when disabled
    /// </summary>
    /// <returns></returns>
    public override Vector3 LastCameraAngle()
    {
        return pc.CameraRotationParent.transform.localEulerAngles;
    }

    /// <summary>
    /// Override from movement class
    /// Sets the first camera angle when enabled
    /// </summary>
    /// <param name="angle"></param>
    public override void SetCameraAngle(Vector3 angle)
    {
        pc.CameraRotationParent.transform.localEulerAngles = angle;
    }

    /// <summary>
    /// Override from movement class
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    protected override void OnMoveEnd()
    {
        // does nothinbg lol
    }

    public override void ResetInteractions()
    {
        lookingAt = null;
        interactingWith = null;
    }
}
