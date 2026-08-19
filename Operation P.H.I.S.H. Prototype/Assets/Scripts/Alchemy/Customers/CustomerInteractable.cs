/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/18/2026
Brief Description : 	Script for interacting with customers
External Resources :    	
***************************************************/

using UnityEngine;

public class CustomerInteractable : MonoBehaviour, IAlchemyInteractable
{

    bool alreadyInteractedWith = false;
    bool hasPotion = false;

    [SerializeField] Customer customerInfo;

    //will need to be adjusted when we've got an inventory system
    public void EnterInteract(AlchemyPlayerController pc)
    {
        if (!alreadyInteractedWith)
        {
            DialogueUIManager.Instance.ManageDialogueDisplay(true, customerInfo.RequestDialogue);
            alreadyInteractedWith = true;
        }
        else
        {
            //TODO: integrate actual potions/inventory system
            //this is gonna have to be reworked sooo much but this is a decent proof of concept i think
            if(pc.HeldPotionType == customerInfo.CorrectPotion)
            {
                DialogueUIManager.Instance.ManageDialogueDisplay(true, customerInfo.CorrectPotionDialogue);
                CurrencyManager.Instance.AddMoney(customerInfo.CorrectPotionPayment);
            }
            else if(customerInfo.AcceptablePotions.Contains(pc.HeldPotionType))
            {
                DialogueUIManager.Instance.ManageDialogueDisplay(true, customerInfo.AcceptablePotionDialogue);
                CurrencyManager.Instance.AddMoney(customerInfo.AcceptablePotionPayment);
            }
            else if(!customerInfo.AcceptablePotions.Contains(pc.HeldPotionType))
            {
                DialogueUIManager.Instance.ManageDialogueDisplay(true, customerInfo.WrongPotionDialogue);
                CurrencyManager.Instance.AddMoney(customerInfo.WrongPotionPayment);
            }
            else
            {
                //nothing would happen here if the player is not holding a potion
                //ideally, i'd like to make it so that the player cannot interact at all with the customer
                //in this case
            }

            hasPotion = true;
        }
    }

    public void ExitInteract()
    {
        DialogueUIManager.Instance.ManageDialogueDisplay(false);
        
        if(hasPotion)
        {
            //ewww ew ew ewwwwwwwwwwwww
            //i can fix this later but i honestly can't be bothered rn
            CustomerManager.Instance.Invoke("SpawnNewCustomer", CustomerManager.Instance.Cooldown);
            Destroy(this.gameObject.transform.parent.gameObject);

            //temp
            //methinks i can swap this out once i can edit AlchemyMovement
            FindFirstObjectByType<AlchemyMovement>().ResetInteractions();
        }
    }

    public void EnterHover()
    {

    }

    public void ExitHover()
    {

    }
}
