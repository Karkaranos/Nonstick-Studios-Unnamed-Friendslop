/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    09/08/2026
Brief Description : 	Holds the enum for correct minigame item to use, allows pickup + drop
External Resources :    	
***************************************************/
using UnityEngine;

public class CleanFixMinigameInteractable : MoontelPickupInteractable
{
    private enum MinigameObjectType { Broom, Toolbox };
    [Header("Minigame")]
    [SerializeField] private MinigameObjectType objectType;
}
