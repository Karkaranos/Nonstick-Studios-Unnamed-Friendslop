/*************************************************
Author Names : 		    Toby Schamberger
Date Created : 		    9/9/2026
Date Last Modified : 	9/9/2026

Brief Description : 	Cycles the current floor map on MoontelGuestMapUI.cs
                        The longest script name in the project btw??? I'd argue all the words in the name are necessary too.

External Resources :    	
***************************************************/

using NaughtyAttributes;
using UnityEngine;

public class MoontelCycleMapFloorButtonInteractable : MonoBehaviour, IMoontelInteractable
{
    [Required, SerializeField] MoontelGuestMapUI guestMap;
    public void EnterInteract(MoontelPlayerController pc)
    {
        Debug.Log("Cycling map floor");
        guestMap.CycleCurrentFloor();
    }

    #region Unnecessary Events
    public void DropItem()
    {
        //throw new System.NotImplementedException();
    }

    public void EnterHover()
    {
        //throw new System.NotImplementedException();
    }

    public void ExitHover()
    {
        //throw new System.NotImplementedException();
    }

    public void ExitInteract()
    {
        //throw new System.NotImplementedException();
    }

    #endregion
}
