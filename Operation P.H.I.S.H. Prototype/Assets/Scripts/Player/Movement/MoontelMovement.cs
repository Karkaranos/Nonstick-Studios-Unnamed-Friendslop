/*************************************************
Author Names : 		    Jacob Bateman, Clare Grady, Cade Naylor, Sky Beal, Toby Schamberger, Jay Embry
Date Created : 		    08/25/2026
Brief Description : 	Handles Moontel movement
External Resources :    	
***************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoontelMovement : Movement
{

    private MoontelPlayerController pc;

    [Header("Modifiers")]
    [SerializeField] private float baseMovementSpeed = 1f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float tapJumpHeightPercent = .5f;
    [SerializeField] private float fullJumpHeight;
    [SerializeField] private float timeToMaxAcceleration = 5f;
    [SerializeField] private float maxAccelerationMultiplier = 3f;
    [SerializeField] private float gravity = -10f;

    [Space(3)]

    private Coroutine shiftHold;

    [Space(5)]

    [Header("Camera")]
    [SerializeField] private float minCameraYClamp = 120f;
    [SerializeField] private float maxCameraYClamp = 60f;
    [SerializeField] private Transform armsParent;

    Vector2 cameraRotation;

    private IMoontelInteractable lookingAt;
    private IMoontelInteractable interactingWith;


    [Space(5)]

    [Header("Raycasting")]
    [SerializeField] private float sightDistance = 2f;
    [SerializeField] private LayerMask interactionLayerMask;

    Rigidbody rb;

    bool jumpThisFrame;
    bool paused;

    private void Start()
    {
        pc = GetComponent<MoontelPlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    #region MOVEMENT
    protected override void OnMove(Vector2 moveVector)
    {
        if (paused)
        {
            return;
        }
        Vector3 newValue = ((pc.CameraRotationParent.forward * moveVector.y) + (pc.CameraRotationParent.right * moveVector.x))
            * baseMovementSpeed * 100f * acceleration * Time.fixedDeltaTime;

        if (!Grounded() || jumpThisFrame)
        {
            newValue.y = rb.linearVelocity.y;
        }
        else
        {
            newValue.y = 0;
        }

        rb.linearVelocity = newValue;
    }

    protected override void OnMoveEnd()
    {
        
    }

    #endregion MOVEMENT

    #region CAMERA

    protected override void OnMouseMove(Vector2 cameraVector)
    {
        if (paused)
        {
            return;
        }
        Vector2 adjustedDelta = cameraVector * pc.CameraSensitivity * Time.fixedDeltaTime;

        cameraRotation.x -= adjustedDelta.y;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, minCameraYClamp, maxCameraYClamp);
        cameraRotation.y += adjustedDelta.x;

        pc.CameraRotationParent.localEulerAngles = cameraRotation;

        if (armsParent != null)
        {
            Vector3 armsRotation = armsParent.transform.localEulerAngles;
            armsRotation.y = cameraRotation.y;
            armsRotation.x = Mathf.Clamp(cameraRotation.x, -40, 40);
            armsParent.transform.localEulerAngles = armsRotation;
        }

        if (LookingAtObject())
        {
            pc.CrosshairImage.sprite = pc.InteractableSprite;
        }
        else
        {
            pc.CrosshairImage.sprite = pc.StandardSprite;
        }
    }

    protected override bool LookingAtObject()
    {
        RaycastHit hit;
        Vector3 direction = pc.CameraRotationParent.forward;

        if (Physics.Raycast(pc.CameraRotationParent.transform.position, direction, out hit, sightDistance))
        {
            IMoontelInteractable interactable = hit.transform.GetComponentInParent<IMoontelInteractable>();
            if (interactable != null)
            {
                if (interactingWith != null && interactable == interactingWith)
                {
                    return true;
                }

                if (lookingAt != null && interactable != lookingAt)
                {
                    lookingAt.ExitHover();
                }

                lookingAt = interactable;
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

    public override void SetCameraAngle(Vector3 angle)
    {
        return;
    }

    public override Vector3 LastCameraAngle()
    {
        return pc.CameraRotationParent.transform.localEulerAngles;
    }

    #endregion CAMERA

    #region OTHER INPUTS

    protected override void OnEClicked(InputAction.CallbackContext obj)
    {
        if (lookingAt != null)
        {
            if (interactingWith == null)
            {
                interactingWith = lookingAt;
                interactingWith.EnterInteract(pc);
            }
            else
            {
                if (lookingAt != interactingWith)
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
            if (interactingWith == lookingAt)
            {
                interactingWith.EnterHover();
            }
            interactingWith = null;
        }

        Debug.Log("E CLICKED.");
    }

    //i don't think that we're doing varying jump heights yet
    protected override void OnSpaceStarted(bool fullyPerformed)
    {
        if (paused)
        {
            return;
        }
        if (Grounded())
        {
            jumpThisFrame = true;
            float jumpForce = Mathf.Sqrt(fullJumpHeight * gravity * -2f);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    protected override void OnSpaceFinished()
    {
        if (paused)
        {
            return;
        }
        jumpThisFrame = false;
    }

    protected override void OnShiftStarted()
    {
        if (paused)
        {
            return;
        }
        if (shiftHold == null)
        {
            shiftHold = StartCoroutine(Accelerate());
        }
    }

    protected IEnumerator Accelerate()
    {
        float timer = 0f;
        while (timer < timeToMaxAcceleration)
        {
            acceleration = 1f + (timer / timeToMaxAcceleration) * maxAccelerationMultiplier;
            Mathf.Clamp(acceleration, 1f, maxAccelerationMultiplier);

            timer += Time.fixedDeltaTime;

            yield return null;
        }
    }

    protected override void OnShiftFinished()
    {
        if (paused)
        {
            return;
        }
        if (shiftHold != null)
        {
            StopCoroutine(shiftHold);
            shiftHold = null;
            acceleration = 1;
        }
    }

    protected override void OnLeftClick()
    {
        //TODO: add funtion once there are "consumables"
        Debug.Log("LEFT MOUSE BUTTON CLICKED.");
    }

    #endregion OTHER INPUTS

    protected bool Grounded()
    {
        RaycastHit hit;
        float groundCheckDistance = transform.localScale.y + .1f;
        return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);
    }

    public override void ResetInteractions()
    {
        lookingAt = null;
        interactingWith = null;
    }

    public void SetPauseState(bool state)
    {
        paused = state;
    }

    #region EMPTY FUNCTIONS

    protected override void OnECanceled(InputAction.CallbackContext obj)
    {
        
    }

    protected override void ReelTetherStarted()
    {
        
    }

    protected override void WhileReelTetherHeld(float deltaTime)
    {

    }

    protected override void ReelTetherFinished()
    {
        
    }

    protected override void OnControlStarted()
    {
        
    }

    protected override void OnControlFinished()
    {
        
    }

    protected override void OnPrepPerformed()
    {
        throw new System.NotImplementedException();
    }

    #endregion EMPTY FUNCTIONS
}
