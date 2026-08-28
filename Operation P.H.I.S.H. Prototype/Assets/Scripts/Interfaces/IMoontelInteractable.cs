/*************************************************
Author Names : 		    Cade Naylor, Jay Embry
Date Created : 		    07/29/2026
Brief Description : 	Defines basic functions for anything that can be interacted with
                        Thanks, Cade :]
External Resources :    	
***************************************************/

using UnityEngine;

public interface IMoontelInteractable
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
    public abstract void EnterInteract(MoontelPlayerController pc);

    /// <summary>
    /// Abstract function for when an object implementing this stops being interacted with
    /// </summary>
    public abstract void ExitInteract();


}
