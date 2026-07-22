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
        e = input.currentActionMap.FindAction("E");
        space = input.currentActionMap.FindAction("Space");
        shift = input.currentActionMap.FindAction("Shift");
        control = input.currentActionMap.FindAction("Control");
    }

    private void OnEnable()
    {
        move.performed += MoveCalled;
        
    }

    private void OnDisable()
    {
        input.currentActionMap.Disable();
        move.Disable();
    }
    #endregion

    #region FUNCTIONS

    private void MoveCalled(InputAction.CallbackContext obj)
    {
        Debug.Log("MOVED");
        Vector2 moveDirection = move.ReadValue<Vector2>();
        PublicEvents.MoveDirection(moveDirection); 
    }
    #endregion
}
