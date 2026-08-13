/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/13/2026
Brief Description : 	Script for interacting with ingredients
                        This should be put on each ingredient prefab(?)
                        Thank you for IInteractable, Cade!!
External Resources :    	
***************************************************/

using UnityEngine;

public class IngredientInteractable : MonoBehaviour, IInteractable
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //wait im blocked
    public void EnterInteract(PlayerController pc)
    {
        rb.isKinematic = true;
        transform.parent = pc.PickupPoint;
    }

    public void ExitInteract()
    {

    }

    public void EnterHover()
    {

    }

    public void ExitHover()
    {

    }
}
