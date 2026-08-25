/*************************************************
Author Names : 		    Jacob Bateman, Clare Grady, Cade Naylor, Sky Beal, Toby Schamberger
Date Created : 		    08/12/2026
Date Last Modified : 	08/16/2026
Brief Description : 	Actually defines and handles land movement.
Jacob Note:             Took this script from PHISH and edited it to fit AS.

External Resources :    	
***************************************************/
using NaughtyAttributes;
using System.Collections;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(AlchemyPlayerController))]
public class AlchemyMovement : Movement
{
    private AlchemyPlayerController pc;
    [SerializeField] private float minCameraYClamp = 120f;
    [SerializeField] private float maxCameraYClamp = 60f;
    [SerializeField] private Transform armsParent;
    [SerializeField] private Transform bodyParent;
    [SerializeField] private Transform hipsParent;

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

    [Header("Interaction")]
    [SerializeField] private float sightDistance = 2f;
    [SerializeField] private LayerMask interactionLayerMask;

    [Header("Throwing")]
    [Tooltip("Multiplier for the force being applied when throwing a pickup item")]
    [SerializeField] private float thrownItemForceMultiplier = 1f;
    [Tooltip("The amount of time E must be held for an object to swap to being thrown")]
    [SerializeField] private float durationWhenThrowStarts = 0.5f;
    [Tooltip("The maximum amount of time E is held down that can be applied to force calculations")]
    [SerializeField] private float maxCalculableDuration = 1f;

    [Header("Camera Interaction")]
    [Tooltip("The minimum angle for the camera to be considered to be \"looking down\"")]
    [SerializeField] private float lookingDownAngle = 50f;
    [Tooltip("If the difference between the players look direction and their legs rotation is greater, then legs start rotating with the camera")]
    [SerializeField] private float lookingAwayFromLegsAngle = 50f;

    private bool jumpThisFrame;
    private bool isCrouching = false;
    private bool itemPickedUpThisActon = false;

    Vector3 baseHipLocalRotation;

    Vector2 cameraRotation;
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
        cameraRotation = new Vector2(pc.CameraRotationParent.eulerAngles.y, pc.CameraRotationParent.eulerAngles.x);

        if (hipsParent != null)
            baseHipLocalRotation = hipsParent.transform.localEulerAngles;
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

        cameraRotation.x -= adjustedDelta.y;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, minCameraYClamp, maxCameraYClamp);
        cameraRotation.y += adjustedDelta.x;

        pc.CameraRotationParent.localEulerAngles = cameraRotation;

        #region Rotate Arms Parent
        if (armsParent != null)
        {
            Vector3 armsRotation = armsParent.transform.localEulerAngles;
            armsRotation.y = cameraRotation.y;
            armsRotation.x = Mathf.Clamp(cameraRotation.x, -40, 40);
            armsParent.transform.localEulerAngles = armsRotation;
        }
        #endregion

        #region Rotate Body Parent
        // Similar rotation to arms, but doesnt move up and down with camera.
        if (bodyParent != null)
        {
            Vector3 bodyRotation = bodyParent.transform.localEulerAngles;
            bodyRotation.y = cameraRotation.y;
            bodyParent.transform.localEulerAngles = bodyRotation;
        }
        #endregion

        #region Rotate hips/legs Parent
        RotateHips();
        #endregion

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
    /// Rotates legs so that pockets can be easily accessed.
    /// </summary>
    private void RotateHips()
    {
        if (hipsParent == null)
            return;

        float hipRotation = hipsParent.transform.localEulerAngles.y;

        // only rotate the hips if the player is looking up, this way they can reach in their pockets
        if (cameraRotation.x < lookingDownAngle)
        {
            hipRotation = cameraRotation.y;
        }

        // if the player is looking too far away from their legs, then rotate em just a lil
        else if (Mathf.Abs(Mathf.DeltaAngle(hipRotation, cameraRotation.y)) > lookingAwayFromLegsAngle)
        {
            hipRotation = Mathf.MoveTowardsAngle(hipRotation, cameraRotation.y, Time.deltaTime * 10); //todo: dont hardcode this l8r
        }

        // if code reaches this point, then player is looking down, so we dont rotate anything.
        hipsParent.transform.localEulerAngles = baseHipLocalRotation.WithY(hipRotation);
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
        if(lookingAt != null)
        {
            if(interactingWith == null)
            {
                itemPickedUpThisActon = true;
                interactingWith = lookingAt;
                interactingWith.EnterInteract(pc);
            }
            else if(lookingAt != interactingWith)
            {
                if(lookingAt is not AlchemyPocket)
                    interactingWith.DropItem();
                itemPickedUpThisActon = true;
                interactingWith = lookingAt;
                interactingWith.EnterInteract(pc);
            }
            else
            {
                Debug.Log("Object will be thrown when E is released");
            }
        }
        else if(interactingWith != null)
        {
            Debug.Log("Object will be thrown when E is released");
        }
        
        
        /*if (lookingAt != null)
        {
            if (interactingWith == null)
            {
                interactingWith = lookingAt;
                interactingWith.EnterInteract(pc);
            }
        }

        if (lookingAt != null)
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
        else if (interactingWith != null)
        {
            interactingWith.ExitInteract();
            if (interactingWith == lookingAt)
            {
                interactingWith.EnterHover();
            }
            interactingWith = null;
        }*/

        if(interactingWith != null && interactingWith.ToString().Contains("Customer"))
        {
            interactingWith = null;
        }

        //Debug.Log("E");
    }

    /// <summary>
    /// Handles deciding between dropping and throwing an item as well as calculating the force with which to throw
    /// </summary>
    protected override void OnECanceled(InputAction.CallbackContext obj)
    {
        if (interactingWith == null)
            return;

        if (itemPickedUpThisActon)
        {
            itemPickedUpThisActon = false;
            return;
        }

        float eDuration = (float)obj.duration;
        eDuration = Mathf.Clamp(Mathf.Abs(eDuration), 0f, maxCalculableDuration);

        if (eDuration < 0.1f)
            return;

        if(eDuration >= durationWhenThrowStarts)
        {
            Vector3 throwDir = pc.CameraRotationParent.transform.forward;
            throwDir.x *= (eDuration * thrownItemForceMultiplier);
            throwDir.y *= 2;
            throwDir.z *= (eDuration * thrownItemForceMultiplier);

            interactingWith.ThrowItem(throwDir);
        }
        else
        {
            interactingWith.DropItem();
        }

        if(interactingWith == lookingAt)
        {
            interactingWith.EnterHover();
        }

        interactingWith = null;
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
            IAlchemyInteractable interactable = hit.transform.GetComponentInParent<IAlchemyInteractable>();
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
