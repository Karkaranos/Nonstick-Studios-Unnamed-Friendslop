/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    7/17/2026
Date Last Modified : 	7/17/2026
Brief Description : 	Very basic player behavior/character controller
External Resources : 	
	***************************************************/
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehavior : MonoBehaviour
{
    private static float GRAVITY = 9.55f;

    [Header("Movement Variables")]
    [SerializeField, Required, Tooltip("The standard player input map")]
    private PlayerInput controlMap;
    [SerializeField, Tooltip("How fast the player moves")]
    private float playerSpeed;
    [SerializeField, Tooltip("How high the player can jump")]
    private float playerJumpHeight;
    private bool moving = false;
    private InputActionMap inputMap;
    private InputAction move;
    private InputAction jump;
    private InputAction mouseDelta;
    private InputAction mousePosition;
    private InputAction interact;
    private Rigidbody rb;
    private Coroutine moveCour;
    private Coroutine cameraCour;



    [Header("Camera Variables")]
    [SerializeField, Tooltip("The parent transform for the camera and hands")]
    private Transform cameraTransformParent;
    [SerializeField, Tooltip("The player's sight and interact range")]
    private float raycastDistance;
    [SerializeField, Tooltip("How sensitive the mouse is")]
    private float mouseSensitivity;
    [SerializeField]
    private float minCameraY;
    [SerializeField]
    private float maxCameraY;

    /// <summary>
    /// Assigns event listeners to Input Action
    /// </summary>
    private void Awake()
    {
        controlMap.currentActionMap.Enable();
        inputMap = controlMap.currentActionMap;
        rb = GetComponent<Rigidbody>();

        move = inputMap.FindAction("Walk");
        jump = inputMap.FindAction("Jump");
        mouseDelta = inputMap.FindAction("MouseDelta");
        mousePosition = inputMap.FindAction("MousePos");
        interact = inputMap.FindAction("Interact");

        move.started += Move_started;
        move.canceled += Move_canceled;
        jump.performed += Jump_performed;
        interact.performed += Interact_performed;

        cameraCour = StartCoroutine(MoveCamera());
    }

    /// <summary>
    /// Unassigns listeners
    /// </summary>
    private void OnDisable()
    {
        move.started -= Move_started;
        move.canceled -= Move_canceled;
        jump.performed -= Jump_performed;
        interact.performed -= Interact_performed;
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Lets the player jump if they are grounded
    /// </summary>
    /// <param name="obj"></param>
    private void Jump_performed(InputAction.CallbackContext obj)
    {
        if(Grounded())
        {
            rb.AddForce(new Vector3(0, playerJumpHeight, 0), ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Stops moving the player if they are actively moving
    /// </summary>
    /// <param name="obj"></param>
    private void Move_canceled(InputAction.CallbackContext obj)
    {
        if(moving)
        {
            moving = false;
            StopCoroutine(moveCour);
        }
    }

    /// <summary>
    /// Starts moving the player if they are not actively moving
    /// </summary>
    /// <param name="obj"></param>
    private void Move_started(InputAction.CallbackContext obj)
    {
        if (!moving)
        {
            moving = true;
            moveCour = StartCoroutine(Move());
        }
    }


    /// <summary>
    /// Returns true if the player is touching the ground
    /// Returns false if in the air
    /// </summary>
    /// <returns></returns>
    private bool Grounded()
    {
        RaycastHit hit;
        float groundCheckDistance = (transform.localScale.y * .5f) + .2f;
        return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);
    }

    /// <summary>
    /// Moves the player 
    /// Reads a value from the input action
    /// Handles any jumping/falling states
    /// </summary>
    /// <returns></returns>
    private IEnumerator Move()
    {
        Vector2 moveVal;
        while(true)
        {
            moveVal = move.ReadValue<Vector2>();
            Vector3 newMoveVal = ((cameraTransformParent.forward * moveVal.y) + (cameraTransformParent.right * moveVal.x)) * playerSpeed * Time.fixedDeltaTime;
            if (!Grounded())
            {
                newMoveVal.y = rb.linearVelocity.y;
            }
            rb.linearVelocity = newMoveVal;
            yield return null;
        }
    }

    private IEnumerator MoveCamera()
    {
        Vector2 mDelta;
        Vector2 adjustedMouseDelta;
        float xRotation = cameraTransformParent.eulerAngles.y;
        float yRotation = cameraTransformParent.eulerAngles.x;
        while(true)
        {
            mDelta = mouseDelta.ReadValue<Vector2>();

            adjustedMouseDelta = mDelta * mouseSensitivity * Time.fixedDeltaTime;

            xRotation -= adjustedMouseDelta.y;
            xRotation = Mathf.Clamp(xRotation, minCameraY, maxCameraY);
            yRotation += adjustedMouseDelta.x;

            cameraTransformParent.localEulerAngles = new Vector3(xRotation, yRotation, 0);

            yield return null;

        }
    }
}
