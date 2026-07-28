/*************************************************
Author Names : 		    Clare Grady
Date Created : 		    07/22/2026
Date Last Modified : 	07/22/202
Brief Description : 	Throws out events for whenever and input happens

External Resources :    Wayward Woods 	
***************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputHandler : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] private PlayerInput input;
    
    private InputAction move;
    private InputAction look;
    private InputAction e;
    private InputAction space;
    private InputAction shift;
    private InputAction control;

    private Coroutine movementUpdates;

    #endregion

    #region INITIALIZATION

    /// <summary>
    /// Enables action map
    /// Sets each action to what it is in the action map
    /// </summary>
    private void Awake()
    {
        input.currentActionMap.Enable();
        move = input.currentActionMap.FindAction("Move");
        look = input.currentActionMap.FindAction("Look");
        e = input.currentActionMap.FindAction("E");
        space = input.currentActionMap.FindAction("Space");
        shift = input.currentActionMap.FindAction("Shift");
        control = input.currentActionMap.FindAction("Control");
        
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
        space.started += SpacePressed;
        space.canceled += SpaceCancled;
        shift.started += ShiftPressed;
        shift.canceled += ShiftCancled;
        control.started += ControlPressed;
        control.canceled += ControlCancled;
        
    }

    /// <summary>
    /// unsubscribes input actions from functions
    /// </summary>
    private void OnDisable()
    {
        move.Disable();
        e.Disable();
        space.Disable();
        shift.Disable();
        control.Disable();
    }
    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Input function for WASD being pressed
    /// </summary>
    /// <param name="obj"></param>
    private void MoveCalled(InputAction.CallbackContext obj)
    {
        if(movementUpdates == null)
        {
            movementUpdates = StartCoroutine(MovementUpdates());
        }
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
    /// Input function for Space started
    /// </summary>
    /// <param name="obj"></param>
    private void SpacePressed(InputAction.CallbackContext obj)
    {
        PublicEvents.SpaceStarted();
    }

    /// <summary>
    /// Input function for space released
    /// </summary>
    /// <param name="obj"></param>
    private void SpaceCancled(InputAction.CallbackContext obj)
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
    private void ShiftCancled(InputAction.CallbackContext obj)
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
    private void ControlCancled(InputAction.CallbackContext obj)
    {
        PublicEvents.ControlFinished();
    }    

    #endregion
}
