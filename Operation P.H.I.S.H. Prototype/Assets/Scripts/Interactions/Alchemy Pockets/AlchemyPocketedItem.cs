/*************************************************
Author Names : 		    Toby Schamberger
Date Created : 		    8/16/2026
Date Last Modified : 	8/16/2026
Brief Description : 	Data holder for the items in the players pockets. 
                        I thought it was going to be more useful. I'm keeping it because i have a hunch. But yeah sorry.
External Resources : 	
***************************************************/

using UnityEngine;

[System.Serializable]
public class AlchemyPocketedItem
{
    public AlchemyPickupInteractable pickup;

    public AlchemyPocketedItem(AlchemyPickupInteractable pickup)
    {
        this.pickup = pickup;
    }
}
