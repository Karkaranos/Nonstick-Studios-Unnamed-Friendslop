/*************************************************
Author Names : 		    Clare Grady
Date Created : 		    07/22/2026
Date Last Modified : 	07/22/202
Brief Description : 	Throws out events for whenever and input happens

External Resources :    Wayward Woods 	
***************************************************/
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
        move.performed += MoveCalled;
        look.performed += LookCalled;   
        e.performed += EPressed;
        space.performed += SpacePressed;
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
        Vector2 moveDirection = move.ReadValue<Vector2>();
        PublicEvents.MoveDirection(moveDirection); 
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
    /// Input function for Space pressed
    /// </summary>
    /// <param name="obj"></param>
    private void SpacePressed(InputAction.CallbackContext obj)
    {
        PublicEvents.SpaceClicked();
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
