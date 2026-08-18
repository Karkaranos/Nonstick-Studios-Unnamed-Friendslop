/*************************************************
Author Names : 		    Clare Grady, Cade Naylor, Sky Beal
Date Created : 		    07/22/2026
Date Last Modified : 	08/7/2026
Brief Description : 	Actually defines and handles land movement

External Resources :    	
***************************************************/
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private Vector3 lookingDirection(Vector2 moveVector) => (pc.CameraRotationParent.forward * moveVector.y) + (pc.CameraRotationParent.right * moveVector.x);

    /// <summary>
    /// runs when loaded into a scene
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();

        if(ShipDiveController.Instance.Diving)
        {
            this.GetComponent<PlayerOxygenManager>().ResetOxygen();
        }

    }

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

        if (armsParent != null)
        {
            Vector3 armsRotation = armsParent.transform.localEulerAngles;
            armsRotation.y = rotation.y;
            armsRotation.x = Mathf.Clamp(rotation.x, -40, 40);
            armsParent.transform.localEulerAngles = armsRotation;
        }

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
    /// Return the jump height
    /// </summary>
    /// <returns></returns>
    public float GetJumpHeight()
    {
        return Mathf.Sqrt(fullJumpHeight * landGravity * -2f) + 1f;
    }

    /// <summary>
    /// Override from Movement base class
    /// Reads a value from the OnMove event and adjust velocity accordingly
    /// </summary>
    /// <param name="moveVector"></param>
    protected override void OnMove(Vector2 moveVector)
    {
        /*Vector3 newValue;
        if (TetherManager.Instance.CanPlayerMoveInDirection(moveVector))
            newValue = lookingDirection(moveVector) * baseLandMovementSpeed * 100f * accelleration * Time.fixedDeltaTime;
        else
            newValue = Vector3.zero;*/
        if(interactingWith != null && (interactingWith.ToString().Contains("ShipMovementControllers") || interactingWith.ToString().Contains("PeriscopeController")))
        {
            return;
        }

        Vector3 newValue = ((pc.CameraRotationParent.forward * moveVector.y) + (pc.CameraRotationParent.right * moveVector.x)) * baseLandMovementSpeed * 100f * accelleration *  Time.fixedDeltaTime;

        if (!Grounded() || jumpThisFrame)
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
    /// Reel in tether while reel button is held (in theory)
    /// </summary>
    /// <param name="deltaTime"></param>
    protected override void WhileReelTetherHeld(float deltaTime)
    {
        Debug.Log("REELIN IN");
        //TetherManager.Instance.PullTetheredObject(null, deltaTime);
    }

    protected override void ReelTetherStarted()
    {
        Debug.Log("Reel Tether");
    }

    protected override void ReelTetherFinished()
    {
        Debug.Log("Reel Tether Finished");

        // this feels terrible for the player but its temp and i need to get this done fast
        //rb.angularVelocity = Vector3.zero;
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
    /// Adjust movement for tether
    /// </summary>
    private void FixedUpdate()
    {
        if (TetherManager.Instance == null) return;

        if (TetherManager.Instance.IsPlayerTethered(this) == false)
            return;

        // dont care if player isnt moving
        Vector3 velocity = rb.linearVelocity;
        if (Mathf.Approximately(velocity.magnitude, 0))
            return;

        /*
        bool canMove = TetherManager.Instance.CanPlayerMoveInDirection(velocity);
        if (!canMove && PlayerInputHandler.Instance.IsReelTetherHeld)
        {
            rb.linearVelocity = Vector3.zero;
        }*/
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

    /// <summary>
    /// override from movement class
    /// sets interaction variables to null
    /// </summary>
    public override void ResetInteractions()
    {
        lookingAt = null;
        interactingWith = null;
    }

    protected override void OnECanceled(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }
}
