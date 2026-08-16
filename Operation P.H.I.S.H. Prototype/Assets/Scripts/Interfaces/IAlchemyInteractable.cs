/*************************************************
Author Names : 		    Cade Naylor, Jacob Bateman
Date Created : 		    08/12/2026
Date Last Modified : 	08/12/2026
Brief Description : 	Defines basic functions for anything that can be interacted with
Jacob Note:             Edited script from PHISH for AS
External Resources :    	
***************************************************/
using UnityEngine;

public interface IAlchemyInteractable
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
    public abstract void EnterInteract(AlchemyPlayerController pc);

    /// <summary>
    /// Abstract function for when an object implementing this stops being interacted with
    /// </summary>
    public abstract void ExitInteract();

    /// <summary>
    /// Abstract function for when an object should be dropped
    /// </summary>
    public abstract void DropItem();

    /// <summary>
    /// Abstract function for when an object should be thrown
    /// </summary>
    /// <param name="throwVec"> The direction and force with which the object should be thrown </param>
    public abstract void ThrowItem(Vector3 throwVec);


}
