/*************************************************
Author Names : 		    Clare Grady, Cade Naylor
Date Created : 		    07/21/2026
Date Last Modified : 	07/28/202
Brief Description : 	Contains all the events for the game

External Resources :    Wayward Woods 	
	***************************************************/
using System;
using UnityEngine;

public static class PublicEvents
{
    #region INPUT EVENTS    

    public static Action<Vector2> MoveDirection;
    public static Action MoveStopped;
    public static Action<Vector2> MousePosition;
    public static Action EClicked;
    public static Action<bool> SpaceStarted;
    public static Action SpaceFinished;
    public static Action ShiftStarted;
    public static Action ShiftFinished;
    public static Action ControlStarted;
    public static Action ControlFinished;

    #endregion

    #region MOVEMENT EVENTS

    public static Action<PlayerController.MovementType> ToggleMovement;

    #endregion

    #region OTHER EVENTS

    public static Action HaltShipMovement;
    public static Action ResetPlayerInteractions;

    #endregion OTHER EVENTS
}
