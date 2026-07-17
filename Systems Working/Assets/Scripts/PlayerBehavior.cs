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



    [Header("Camera Variables")]
    [SerializeField, Tooltip("The parent transform for the camera and hands")]
    private Transform cameraTransformParent;
    [SerializeField, Tooltip("The player's sight and interact range")]
    private float raycastDistance;

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
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        if(Grounded())
        {
            rb.AddForce(new Vector3(0, playerJumpHeight, 0), ForceMode.Impulse);
        }
    }

    private void Move_canceled(InputAction.CallbackContext obj)
    {
        if(moving)
        {
            moving = false;
            StopCoroutine(moveCour);
        }
    }

    private void Move_started(InputAction.CallbackContext obj)
    {
        if (!moving)
        {
            moving = true;
            moveCour = StartCoroutine(Move());
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private bool Grounded()
    {
        RaycastHit hit;
        float groundCheckDistance = (transform.localScale.y * .5f) + .2f;
        return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);
    }

    private IEnumerator Move()
    {
        Vector2 moveVal;
        while(true)
        {
            moveVal = move.ReadValue<Vector2>();
            float currYVel = rb.linearVelocity.y;
            Vector3 newVel = ((cameraTransformParent.forward * moveVal.y) + (cameraTransformParent.right * moveVal.x)) * playerSpeed * Time.fixedDeltaTime;
            newVel.y = currYVel - GRAVITY * Time.fixedDeltaTime;
            rb.linearVelocity = newVel;
            yield return null;
        }
    }
}
