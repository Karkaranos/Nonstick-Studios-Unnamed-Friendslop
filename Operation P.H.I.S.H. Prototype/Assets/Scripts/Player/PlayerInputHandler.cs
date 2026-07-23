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

    private void OnEnable()
    {
        move.performed += MoveCalled;
        e.performed += EPressed;
        space.performed += SpacePressed;
        shift.started += ShiftPressed;
        shift.canceled += ShiftCancled;
        control.started += ControlPressed;
        control.canceled += ControlCancled;
        
    }

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

    private void MoveCalled(InputAction.CallbackContext obj)
    {
        Debug.Log("MOVED");
        Vector2 moveDirection = move.ReadValue<Vector2>();
        //PublicEvents.MoveDirection(moveDirection); 
    }

    private void LookCalled(InputAction.CallbackContext obj)
    {
        Debug.Log("LOOK");
        Vector2 lookDirection = look.ReadValue<Vector2>();
    }
    private void EPressed(InputAction.CallbackContext obj)
    {
        Debug.Log("INTERACT");
    }

    private void SpacePressed(InputAction.CallbackContext obj)
    {
        Debug.Log("SPACE");
    }

    private void ShiftPressed(InputAction.CallbackContext obj)
    {
        Debug.Log("SHIFT");
    }

    private void ShiftCancled(InputAction.CallbackContext obj)
    {
        Debug.Log("END SHIFT");
    }

    private void ControlPressed(InputAction.CallbackContext obj)
    {
        Debug.Log("CONTROL");
    }

    private void ControlCancled(InputAction.CallbackContext obj)
    {
        Debug.Log("END CONTROL");
    }    

    #endregion
}
