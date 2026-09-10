/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    08/21/2026
Brief Description : 	Allows potions to be picked up and holds potion data.
External Resources :    	
***************************************************/
using UnityEngine;

public enum PotionType
{
    Endurance,
    GentleRepose,
    Health,
    Hydration,
    Luck,
    Strength,
    Null
}


public class PotionInteractable : AlchemyPickupInteractable
{
    public PotionType PotionID;
}
