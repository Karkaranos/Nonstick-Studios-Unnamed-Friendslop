/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    8/6/2026
Date Last Modified : 	8/6/2026
Brief Description : 	Starts and ends a dive
External Resources : 	
***************************************************/
using UnityEngine;

public class ShipDiveController : MonoBehaviour, IInteractable
{
    /// <summary>
    /// called when this is interacted with
    /// </summary>
    public void EnterInteract(PlayerController pc)
    {
        ShipResourceManager.Instance.BeginDive();
    }

    /// <summary>
    /// called when this is interacted with again
    /// </summary>
    public void ExitInteract()
    {
        ShipResourceManager.Instance.EndDive();
    }

    /// <summary>
    /// called when the mouse hovers over this object
    /// </summary>
    public void EnterHover()
    {
        //don't worry about this
    }

    /// <summary>
    /// called when the mouse is no longer hovering over this object
    /// </summary>
    public void ExitHover()
    {
        //or this
    }

}
