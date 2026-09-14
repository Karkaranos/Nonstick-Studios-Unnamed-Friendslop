/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    09/11/2026
Brief Description : 	Stores a list of rooms (and keys?)
External Resources :    	
***************************************************/

using UnityEngine;
using System.Collections.Generic;

public class RoomManager : Singleton<RoomManager>
{
    public List<RoomBehavior> Rooms;
}
