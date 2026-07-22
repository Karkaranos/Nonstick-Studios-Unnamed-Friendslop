/*************************************************
Author Names : 		    Clare Grady
Date Created : 		    07/21/2026
Date Last Modified : 	07/21/202
Brief Description : 	Contains all the events for the game

External Resources :    Wayward Woods 	
	***************************************************/
using System;
using UnityEngine;

public static class PublicEvents
{
    #region INPUT EVENTS    

    public static Action<Vector2> MoveDirection;
    public static Action<Vector2> MousePosition;
    public static Action InteractClicekd;

    #endregion
}
