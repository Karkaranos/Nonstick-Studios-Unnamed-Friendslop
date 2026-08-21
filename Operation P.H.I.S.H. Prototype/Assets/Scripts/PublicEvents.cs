/*************************************************
Author Names : 		    Clare Grady, Cade Naylor
Date Created : 		    07/21/2026
Date Last Modified : 	07/28/202
Brief Description : 	Contains all the events for the game

External Resources :    Wayward Woods 	
	***************************************************/
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public static class PublicEvents
{
    #region INPUT EVENTS    

    public static Action<Vector2> MoveDirection;
    public static Action MoveStarted;
    public static Action MoveStopped;
    public static Action<Vector2> MousePosition;
    public static Action EClicked;
    public static Action<InputAction.CallbackContext> ECanceled;
    public static Action ReelTetherStarted;
    public static Action<float> ReelTetherHeld; // float: deltaTime
    public static Action ReelTetherFinished;
    public static Action<bool> SpaceStarted;
    public static Action SpaceFinished;
    public static Action ShiftStarted;
    public static Action ShiftFinished;
    public static Action ControlStarted;
    public static Action ControlFinished;
    public static Action ResetInteractable;

    public static Action PrepPerformed;

    #endregion

    #region MOVEMENT EVENTS

    public static Action<PlayerController.MovementType> ToggleMovement;

    #endregion

    #region OTHER EVENTS

    public static Action HaltShipMovement;
    public static Action ResetPlayerInteractions;

    public static Action ForceUpdatePage;
    public static Action<GameObject> ForceCloseCanvas;

    #endregion OTHER EVENTS
}
