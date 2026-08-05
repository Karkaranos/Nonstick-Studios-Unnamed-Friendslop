/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    07/30/2026
Date Last Modified : 	07/30/2026
Brief Description : 	Actually defines and handles water movement

External Resources :    	
***************************************************/
using System.Collections;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody), typeof(PlayerController))]
public class WaterMovement : Movement
{
    private PlayerController pc;
    [SerializeField] private float minCameraYClamp = 120f;
    [SerializeField] private float maxCameraYClamp = 60f;
    [SerializeField] private Transform armsParent;

    [SerializeField] private float baseWaterMovementSpeed = 1f;
    [SerializeField] private float timeToMaxAcceleration = 5f;
    [SerializeField] private float maxAccelerationMultiplier = 3f;
    [SerializeField] private float ascentWaterMovementSpeed = .5f;
    [SerializeField] private float descentWaterMovementSpeed = 1.2f;
    [SerializeField] private float waterGravity = -4f;
    private float accelleration = 1f;
    private Coroutine shiftHold;
    [SerializeField] private GameObject waterTint;

    [ReadOnly, SerializeField] bool ascending;
    [ReadOnly, SerializeField] bool descending;
    [ReadOnly, SerializeField] bool moving;

    Vector2 rotation;
    Rigidbody rb;

    private IInteractable lookingAt;
    private IInteractable interactingWith;
    private bool interacting;
    private Coroutine waterGravityCour;
    [SerializeField] private float sightDistance = 2f;

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
        rb = GetComponent<Rigidbody>();
        waterTint.SetActive(true);
        rb.useGravity = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * .5f, rb.linearVelocity.z);
        waterGravityCour = StartCoroutine(WaterGravity());
    }

    /// <summary>
    /// Override from movement base class to add extra functionality
    /// Enables the player's gravity when exiting swim mode
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        waterTint.SetActive(false);
        rb.useGravity = true;
        StopCoroutine(waterGravityCour);
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

        Vector3 armsRotation = armsParent.transform.localEulerAngles;
        armsRotation.y = rotation.y;
        armsRotation.x = Mathf.Clamp(rotation.x, -40, 40);
        armsParent.transform.localEulerAngles = armsRotation;

        if (LookingAtObject())
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
        moving = true;
        Vector3 newValue = ((pc.CameraRotationParent.forward * moveVector.y) + (pc.CameraRotationParent.right * moveVector.x)) * baseWaterMovementSpeed * 100f * accelleration * Time.fixedDeltaTime;

        if (ascending || descending)
        {
            newValue.y = rb.linearVelocity.y;
        }
        else
        {
            newValue.y = waterGravity;
        }

        rb.linearVelocity = newValue;

        Debug.Log("MOVE");
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

        Debug.Log("E");
    }

    /// <summary>
    /// Override from movement base class
    /// Should make the player ascend in the water
    /// </summary>
    protected override void OnSpaceStarted(bool fullyPerformed)
    {
        // TODO: Make the player ascend
        ascending = true;

        float ascendForce = ascentWaterMovementSpeed * accelleration * 100f * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, ascendForce, rb.linearVelocity.z);
        Debug.Log("Space Started");
    }

    /// <summary>
    /// Override from movement base class
    /// </summary>
    protected override void OnSpaceFinished()
    {
        // TODO: Stop the player's ascension
        ascending = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
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
        descending = true;

        float ascendForce = -1 * descentWaterMovementSpeed * accelleration * 100f * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, ascendForce, rb.linearVelocity.z);
        Debug.Log("Control Started");
    }

    /// <summary>
    /// override from movement base class
    /// Should make the player descend
    /// </summary>
    protected override void OnControlFinished()
    {
        descending = false;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Debug.Log("Control Finished");
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
            if (hit.transform.GetComponent<IInteractable>() != null)
            {

                if (interactingWith != null && hit.transform.GetComponent<IInteractable>() == interactingWith)
                {
                    return true;
                }

                if (lookingAt != null && hit.transform.GetComponent<IInteractable>() != lookingAt)
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
        while (timer < timeToMaxAcceleration)
        {
            accelleration = 1f + (timer / timeToMaxAcceleration) * maxAccelerationMultiplier;
            Mathf.Clamp(accelleration, 1f, maxAccelerationMultiplier);

            timer += Time.fixedDeltaTime;

            Debug.Log(accelleration);
            yield return null;
        }
    }

    /// <summary>
    /// Fakes adding water gravity
    /// </summary>
    /// <returns></returns>

    protected IEnumerator WaterGravity()
    {
        Vector3 newValue;
        while(true)
        {
            newValue = rb.linearVelocity;
            if(!moving && !ascending && !descending)
            {
                newValue.y = waterGravity;
            }
            rb.linearVelocity = newValue;

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
    /// Stops movement
    /// </summary>
    protected override void OnMoveEnd()
    {
        moving = false;
    }

    protected override void ReelTetherStarted()
    {
        throw new System.NotImplementedException();
    }

    protected override void WhileReelTetherHeld(float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    protected override void ReelTetherFinished()
    {
        throw new System.NotImplementedException();
    }
}