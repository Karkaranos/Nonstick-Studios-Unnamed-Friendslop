/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    8/6/2026
Date Last Modified : 	8/6/2026
Brief Description : 	Starts and ends a dive
External Resources : 	
***************************************************/
using UnityEngine;

public class ShipDiveController : Singleton<ShipDiveController>, IInteractable
{
    [HideInInspector] public bool Diving = false;

    /// <summary>
    /// called when this is interacted with
    /// </summary>
    public void EnterInteract(PlayerController pc)
    {
        Diving = !Diving;

        if(Diving)
        {
            ShipResourceManager.Instance.BeginDive();
        }
        else
        {
            ShipResourceManager.Instance.EndDive();
        }
    }

    /// <summary>
    /// called when this is interacted with again
    /// </summary>
    public void ExitInteract()
    {
        //i never needed to put anything here at all, actually
        //this button is weird
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
