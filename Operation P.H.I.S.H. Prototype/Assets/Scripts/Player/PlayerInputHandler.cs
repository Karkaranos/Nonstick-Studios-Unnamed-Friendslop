/*************************************************
Author Names : 		    Clare Grady, Cade Naylor
Date Created : 		    07/22/2026
Date Last Modified : 	07/28/202
Brief Description : 	Throws out events for whenever and input happens

External Resources :    Wayward Woods 	
***************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputHandler : Singleton<PlayerInputHandler>
{
    #region VARIABLES

    [SerializeField] private PlayerInput input;
    
    private InputAction move;
    private InputAction look;
    private InputAction e;
    private InputAction reelTether;
    private InputAction space;
    private InputAction shift;
    private InputAction control;
    private InputAction prep;

    private Coroutine movementUpdates;

    public bool IsReelTetherHeld { get; private set; } = false;
    public bool IsMovementHeld { get; private set; } = false;

    #endregion

    #region INITIALIZATION

    /// <summary>
    /// Enables action map
    /// Sets each action to what it is in the action map
    /// </summary>
    private void Awake()
    {
        base.Awake();

        input.currentActionMap.Enable();
        move = input.currentActionMap.FindAction("Move");
        look = input.currentActionMap.FindAction("Look");
        e = input.currentActionMap.FindAction("E");
        reelTether = input.currentActionMap.FindAction("Reel Tether");
        space = input.currentActionMap.FindAction("Space");
        shift = input.currentActionMap.FindAction("Shift");
        control = input.currentActionMap.FindAction("Control");
        prep = input.currentActionMap.FindAction("Prep");
        
    }

    /// <summary>
    /// subscribes input actions to functions
    /// </summary>
    private void OnEnable()
    {
        move.started += MoveCalled;
        move.canceled += MoveStopped;
        look.performed += LookCalled;   
        e.performed += EPressed;
        reelTether.started += ReelTetherStarted;
        reelTether.canceled += ReelTetherCanceled;
        space.started += SpacePressed;
        space.performed += SpacePressed;
        space.canceled += SpaceCanceled;
        shift.started += ShiftPressed;
        shift.canceled += ShiftCanceled;
        control.started += ControlPressed;
        control.canceled += ControlCanceled;
        prep.performed += Prep_performed;
        
    }

    /// <summary>
    /// unsubscribes input actions from functions
    /// </summary>
    private void OnDisable()
    {
        move.Disable();
        e.Disable();
        reelTether.Disable();
        space.Disable();
        shift.Disable();
        control.Disable();
        prep.Disable();
    }
    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Input function for WASD being pressed
    /// </summary>
    /// <param name="obj"></param>
    private void MoveCalled(InputAction.CallbackContext obj)
    {
        if (movementUpdates == null)
        {
            movementUpdates = StartCoroutine(MovementUpdates());
        }
        IsMovementHeld = true;
        PublicEvents.MoveStarted.Invoke();
    }

    /// <summary>
    /// Input function for WASD stopping
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void MoveStopped(InputAction.CallbackContext context)
    {
        if(movementUpdates != null)
        {
            StopCoroutine(movementUpdates);
            movementUpdates = null;
        }
        Vector2 moveDirection = move.ReadValue<Vector2>();
        PublicEvents.MoveDirection(moveDirection);
        PublicEvents.MoveStopped?.Invoke();
        IsMovementHeld = false;
    }

    /// <summary>
    /// Sends out signals while WASD is held
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovementUpdates()
    {
        while (true)
        {
            Vector2 moveDirection = move.ReadValue<Vector2>();
            PublicEvents.MoveDirection(moveDirection);
            yield return null;
        }
    }

    /// <summary>
    /// Input function for moving mouse
    /// </summary>
    /// <param name="obj"></param>
    private void LookCalled(InputAction.CallbackContext obj)
    {
        Vector2 lookDirection = look.ReadValue<Vector2>();
        PublicEvents.MousePosition(lookDirection);
    }

    /// <summary>
    /// Input function for E pressed
    /// </summary>
    /// <param name="obj"></param>
    private void EPressed(InputAction.CallbackContext obj)
    {
        PublicEvents.EClicked();
    }

    /// <summary>
    /// Input function for Reel Tether started
    /// </summary>
    /// <param name="obj"></param>
    private void ReelTetherStarted(InputAction.CallbackContext obj)
    {
        IsReelTetherHeld = true;
        PublicEvents.ReelTetherStarted();
    }

    /// <summary>
    /// Input function for Reel Tether started
    /// </summary>
    /// <param name="obj"></param>
    private void ReelTetherCanceled(InputAction.CallbackContext obj)
    {
        IsReelTetherHeld = false;
        PublicEvents.ReelTetherFinished();
    }

    /// <summary>
    /// Input function for Space started
    /// </summary>
    /// <param name="obj"></param>
    private void SpacePressed(InputAction.CallbackContext obj)
    {
        PublicEvents.SpaceStarted(obj.performed);
    }

    /// <summary>
    /// Input function for space released
    /// </summary>
    /// <param name="obj"></param>
    private void SpaceCanceled(InputAction.CallbackContext obj)
    {
        PublicEvents.SpaceFinished();
    }

    /// <summary>
    /// Input action for starting to hold shift
    /// </summary>
    /// <param name="obj"></param>
    private void ShiftPressed(InputAction.CallbackContext obj)
    {
        PublicEvents.ShiftStarted();
    }

    /// <summary>
    /// Input action for realsing shift
    /// </summary>
    /// <param name="obj"></param>
    private void ShiftCanceled(InputAction.CallbackContext obj)
    {
        PublicEvents.ShiftFinished();
    }

    /// <summary>
    /// Input action for starting to hold control
    /// </summary>
    /// <param name="obj"></param>
    private void ControlPressed(InputAction.CallbackContext obj)
    {
        PublicEvents.ControlStarted();
    }

    /// <summary>
    /// Input action for realsing control
    /// </summary>
    /// <param name="obj"></param>
    private void ControlCanceled(InputAction.CallbackContext obj)
    {
        PublicEvents.ControlFinished();
    }


    private void Prep_performed(InputAction.CallbackContext obj)
    {
        PublicEvents.PrepPerformed();
    }

    #endregion

    #region Held Events

    /// <summary>
    /// Send events for "held" events
    /// </summary>
    private void FixedUpdate()
    {
        if(IsReelTetherHeld)
            PublicEvents.ReelTetherHeld(Time.fixedDeltaTime);
    }
    #endregion
}
