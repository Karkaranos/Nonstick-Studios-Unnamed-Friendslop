/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    09/09/2026
Brief Description : 	Stores information for Moontel events
External Resources :    	
***************************************************/

using NaughtyAttributes;
using NUnit.Framework;
using UnityEngine;

public enum EventType
{
    FetchItem,
    InteractWithItem
}

public class GuestEvent
{
    public EventType TypeOfEvent;
    public GameObject RequiredInteractableItem;
}
