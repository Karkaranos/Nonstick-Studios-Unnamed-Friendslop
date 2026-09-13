/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    09/09/2026
Brief Description : 	Stores information for Moontel events
External Resources :    	
***************************************************/

using NaughtyAttributes;
using NUnit.Framework;
using UnityEngine;

public enum TypeOfEvent
{
    Fetch,
    Interact
}

[System.Serializable]
public class GuestEvent
{
    public TypeOfEvent EventType;

    [AllowNesting, ShowIf(nameof(EventType), TypeOfEvent.Fetch)]
    public ItemType RequestedItem;
    [AllowNesting, ShowIf(nameof(EventType), TypeOfEvent.Interact)]
    public MinigameObjectType RequiredTool;
}
