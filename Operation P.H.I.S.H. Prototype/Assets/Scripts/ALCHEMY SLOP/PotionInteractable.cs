/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    08/21/2026
Brief Description : 	Allows potions to be picked up and holds potion data.
External Resources :    	
***************************************************/
using UnityEngine;

public class PotionInteractable : MonoBehaviour, IAlchemyInteractable
{
    public string PotionID = "Default";

    public void EnterHover()
    {
        return;
    }

    /// <summary>
    /// Puts potion in hands
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="standardInteraction"></param>
    public void EnterInteract(AlchemyPlayerController pc, bool standardInteraction = true)
    {
        transform.parent = pc.PickupPoint;
        transform.localPosition = Vector3.zero;
        Debug.Log($"GRABBED {this.PotionID}.");
    }

    public void ExitHover()
    {
        return;
    }

    /// <summary>
    /// Removes potion from hands
    /// </summary>
    public void ExitInteract()
    {
        transform.parent = null;
        Debug.Log($"DROPPED {this.PotionID}.");
    }
}
