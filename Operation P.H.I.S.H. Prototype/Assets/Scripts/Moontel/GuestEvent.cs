/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    09/09/2026
Brief Description : 	Stores information for Moontel events
External Resources :    	
***************************************************/

using NaughtyAttributes;
using NUnit.Framework;
using UnityEngine;

//chat does this suck
public enum EventTitle
{
    GrabTowels,
    GrabToiletPaper,
    FixObject,
    CleanUpMess
}

public enum EventType
{
    FetchItem,
    InteractWithItem
}

public enum ObjectType
{
    InGame,
    Prefab
}

[System.Serializable]
public class GuestEvent
{
    public EventTitle Title;

    [Tooltip("Are you bringing an item to a guest or cleaning up a mess?")]
    public EventType TypeOfEvent;

    [AllowNesting]
    [ShowIf(nameof(TypeOfEvent), EventType.InteractWithItem)]
    [Tooltip("Will this object be spawned in, or is it already in the world?")]
    public ObjectType TypeOfInteractableItem;

    [Tooltip("Put an already in-game object here if this is a fetch quest. This should otherwise be a prefab.")]
    public GameObject RequiredInteractableItem;
}
