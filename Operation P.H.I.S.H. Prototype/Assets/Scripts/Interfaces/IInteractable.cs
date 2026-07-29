/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    07/29/2026
Date Last Modified : 	07/29/2026
Brief Description : 	Defines basic functions for anything that can be interacted with
External Resources :    	
***************************************************/
using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Abstract function for when an object implementing this is hovered
    /// </summary>
    public abstract void EnterHover();

    /// <summary>
    /// Abstract function for when an object implementing this stops being hovered
    /// </summary>
    public abstract void ExitHover();

    /// <summary>
    /// Abstract function for when an object implementing this starts being interacted with
    /// </summary>
    public abstract void EnterInteract();

    /// <summary>
    /// Abstract function for when an object implementing this stops being interacted with
    /// </summary>
    public abstract void ExitInteract();
    

}
