/*************************************************
Author Names : 		    Jacob Bateman, Clare Grady, Cade Naylor, Sky Beal
Date Created : 		    08/12/2026
Date Last Modified : 	08/12/2026
Brief Description : 	Actually defines and handles land movement.
Jacob Note:             Took this script from PHISH and edited it to fit AS.

External Resources :    	
***************************************************/
using NaughtyAttributes;
using System.Collections;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(AlchemyPlayerController))]
public class AlchemyMovement : Movement
{
    private AlchemyPlayerController pc;
    [SerializeField] private float minCameraYClamp = 120f;
    [SerializeField] private float maxCameraYClamp = 60f;
    [SerializeField] private Transform armsParent;

    [SerializeField] private float baseMovementSpeed = 1f;
    [Tooltip("This penalty is multiplied with the speed and should be 0 - 0.9")]
    [MinValue(0f), MaxValue(0.99f)]
    [SerializeField] private float crouchMovementPenalty = 0.5f;
    [SerializeField] private float tapJumpHeightPercent = .5f;
    [SerializeField] private float fullJumpHeight;
    [SerializeField] private float timeToMaxAcceleration = 5f;
    [SerializeField] private float maxAccelerationMultiplier = 3f;
    [SerializeField] private float gravity = -10f;
    private float accelleration = 1f;
    private Coroutine shiftHold;

    private IAlchemyInteractable lookingAt;
    private IAlchemyInteractable interactingWith;
    private bool interacting;
    [SerializeField] private float sightDistance = 2f;

    private bool jumpThisFrame;
    private bool isCrouching = false;

    Vector2 rotation;
    Rigidbody rb;
    bool paused;

    private Vector3 lookingDirection(Vector2 moveVector) => (pc.CameraRotationParent.forward * moveVector.y) + (pc.CameraRotationParent.right * moveVector.x);

    /// <summary>
    /// runs when loaded into a scene
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();

        PublicEvents.ResetInteractable += ResetInteractions;
    }


    protected override void OnDisable()
    {
        base.OnDisable();

        PublicEvents.ResetInteractable -= ResetInteractions;
    }
    /// <summary>
    /// Grabs initial references and sets initial variables
    /// </summary>
    private void Start()
    {
        pc = GetComponent<AlchemyPlayerController>();
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
        if(paused)
        {
            return;
        }
        Vector2 adjustedDelta = cameraVector * pc.CameraSensitivity * Time.fixedDeltaTime;

        rotation.x -= adjustedDelta.y;
        rotation.x = Mathf.Clamp(rotation.x, minCameraYClamp, maxCameraYClamp);
        rotation.y += adjustedDelta.x;

        pc.CameraRotationParent.localEulerAngles = rotation;

        if (armsParent != null)
        {
            Vector3 armsRotation = armsParent.transform.localEulerAngles;
            armsRotation.y = rotation.y;
            armsRotation.x = Mathf.Clamp(rotation.x, -40, 40);
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

        //Debug.Log("LOOK");
    }

    /// <summary>
    /// Return the jump height
    /// </summary>
    /// <returns></returns>
    public float GetJumpHeight()
    {
        return Mathf.Sqrt(fullJumpHeight * gravity * -2f) + 1f;
    }

    /// <summary>
    /// Override from Movement base class
    /// Reads a value from the OnMove event and adjust velocity accordingly
    /// </summary>
    /// <param name="moveVector"></param>
    protected override void OnMove(Vector2 moveVector)
    {
        if (paused)
        {
            return;
        }
        Vector3 newValue = ((pc.CameraRotationParent.forward * moveVector.y) + (pc.CameraRotationParent.right * moveVector.x)) 
            * baseMovementSpeed * 100f * accelleration * Time.fixedDeltaTime;

        if (isCrouching)
            newValue *= crouchMovementPenalty;

        if (!Grounded() || jumpThisFrame)
        {
            newValue.y = rb.linearVelocity.y;
        }
        else
        {
            newValue.y = 0;
        }

        rb.linearVelocity = newValue;

        //Debug.Log("MOVE");
    }

    /// <summary>
    /// Override from Movement base class
    /// </summary>
    protected override void OnEClicked()
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

        //Debug.Log("E");
    }

    /// <summary>
    /// Override from movement base class
    /// </summary>
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
        //Debug.Log("Space Started");
    }

    /// <summary>
    /// Override from movement base class
    /// </summary>
    protected override void OnSpaceFinished()
    {
        if (paused)
        {
            return;
        }
        jumpThisFrame = false;
        //Debug.Log("Space Finished");
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
    protected override void OnShiftStarted()
    {
        if (paused)
        {
            return;
        }
        if (shiftHold == null && !isCrouching)
        {
            shiftHold = StartCoroutine(Accelerate());
        }
        //Debug.Log("Shift Start");
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
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
            accelleration = 1;
        }
        //Debug.Log("Shift Finished");
    }

    /// <summary>
    /// Override from movement base class
    /// </summary>
    protected override void OnControlStarted()
    {
        if (paused)
        {
            return;
        }
        isCrouching = true;
        Crouch();

        //Debug.Log("Control Started");
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
    protected override void OnControlFinished()
    {
        if (paused)
        {
            return;
        }
        isCrouching = false;
        Crouch();

        //Debug.Log("Control Finished");
    }

    /// <summary>
    /// Sets player height depending on whether or not they are crouching.
    /// In a non-prototype this should be controlled by animations.
    /// </summary>
    private void Crouch()
    {
        Vector3 newScale = gameObject.transform.localScale;

        if (isCrouching)
            newScale.y /= 2f;
        else
            newScale.y *= 2f;

        gameObject.transform.localScale = newScale;
    }

    /// <summary>
    /// Checks if the player is on the ground
    /// </summary>
    /// <returns></returns>
    protected bool Grounded()
    {
        RaycastHit hit;
        float groundCheckDistance = transform.localScale.y + .1f;
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

        if (Physics.Raycast(pc.CameraRotationParent.transform.position, direction, out hit, sightDistance))
        {
            if (hit.transform.GetComponent<IAlchemyInteractable>() != null)
            {
                if (interactingWith != null && hit.transform.GetComponent<IAlchemyInteractable>() == interactingWith)
                {
                    return true;
                }

                if (lookingAt != null && hit.transform.GetComponent<IAlchemyInteractable>() != lookingAt)
                {
                    lookingAt.ExitHover();
                }

                lookingAt = hit.transform.GetComponent<IAlchemyInteractable>();
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
        while (timer < timeToMaxAcceleration)
        {
            accelleration = 1f + (timer / timeToMaxAcceleration) * maxAccelerationMultiplier;
            Mathf.Clamp(accelleration, 1f, maxAccelerationMultiplier);

            timer += Time.fixedDeltaTime;

            //Debug.Log(accelleration);
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
        //pc.CameraRotationParent.transform.localEulerAngles = angle;
        Debug.LogWarning("Function SetCameraAngle should not be called in AS");
    }

    /// <summary>
    /// Override from movement class
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    protected override void OnMoveEnd()
    {
        // does nothing lol
    }

    /// <summary>
    /// override from movement class
    /// sets interaction variables to null
    /// </summary>
    public override void ResetInteractions()
    {
        lookingAt = null;
        interactingWith = null;
    }

    public void SetPauseState(bool state)
    {
        paused = state;
    }

    /// <summary>
    /// It's a wonky way of doing it but it works
    /// Lets you interact with prep atations while holding items
    /// the false makes it only work on prep stations
    /// </summary>
    protected override void OnPrepPerformed()
    {
        if (interactingWith != null)
        {
            interactingWith.ExitInteract();
            if (interactingWith == lookingAt)
            {
                interactingWith.EnterHover();
            }
            interactingWith = null;
        }

        if (lookingAt != null)
        {
            lookingAt.EnterInteract(pc, false);
        }



    }

    /// <summary>
    /// This section contains functions that are necessary to prevent compilation errors but should not be used for AS.
    /// </summary>
    #region Empty Overrides

    protected override void ReelTetherStarted()
    {
        Debug.LogWarning("Function ReelTetherStarted should not be called in AS");
    }

    protected override void WhileReelTetherHeld(float deltaTime)
    {
        Debug.LogWarning("Function WhileReelTetherHeld should not be called in AS");
    }

    protected override void ReelTetherFinished()
    {
        Debug.LogWarning("Function ReelTetherFinished should not be called in AS");
    }

    #endregion
}
