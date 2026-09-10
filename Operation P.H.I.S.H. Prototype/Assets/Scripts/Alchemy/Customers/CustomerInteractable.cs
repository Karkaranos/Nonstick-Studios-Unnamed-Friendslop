/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/18/2026
Brief Description : 	Script for interacting with customers
External Resources :    	
***************************************************/

using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerInteractable : MonoBehaviour, IAlchemyInteractable
{

    bool alreadyInteractedWith = false;
    bool isInteractingWith = false;
    bool hasPotion = false;

    [SerializeField] Customer customerInfo;

    [Space(5)]

    [Tooltip("The customer's canvas goes here!")]
    [SerializeField] GameObject dialogueCanvas;

    [Tooltip("The customer's dialogue goes here!")]
    [SerializeField] TMP_Text dialogueText;

    //will need to be adjusted when we've got an inventory system
    public void EnterInteract(AlchemyPlayerController pc, bool standardInteraction)
    {
        if(isInteractingWith)
        {
            return;
        }

        if (!alreadyInteractedWith || pc.heldInteractable == null || 
            pc.heldInteractable.GetComponent<PotionInteractable>() == null)
        {
            DisplayDialogue(customerInfo.RequestDialogue);
            alreadyInteractedWith = true;
        }
        else if (alreadyInteractedWith && pc.heldInteractable != null && 
                 pc.heldInteractable.GetComponent<PotionInteractable>() != null)
        {
            if (pc.heldInteractable.GetComponent<PotionInteractable>().PotionID ==
                customerInfo.CorrectPotion)
            {
                DisplayDialogue(customerInfo.CorrectPotionDialogue);
                CurrencyManager.Instance.AddMoney(customerInfo.CorrectPotionPayment);
            }
            else if (customerInfo.AcceptablePotions.Contains
                    (pc.heldInteractable.GetComponent<PotionInteractable>().PotionID))
            {
                DisplayDialogue(customerInfo.AcceptablePotionDialogue);
                CurrencyManager.Instance.AddMoney(customerInfo.AcceptablePotionPayment);
            }
            else if (!customerInfo.AcceptablePotions.Contains
                    (pc.heldInteractable.GetComponent<PotionInteractable>().PotionID))
            {
                DisplayDialogue(customerInfo.WrongPotionDialogue);
                CurrencyManager.Instance.AddMoney(customerInfo.WrongPotionPayment);
            }

            Destroy(pc.heldInteractable.gameObject);

            hasPotion = true;
        }

        isInteractingWith = true;

        if(this != null)
        {
            Invoke("DisableDialogue", CustomerManager.Instance.DialogueDisplayTime);
        }    
    }

    /// <summary>
    /// displays dialogue box and text
    /// </summary>
    void DisplayDialogue(string displayedDialogue)
    {
        if(this != null)
        {
            dialogueCanvas.SetActive(true);
            dialogueText.text = displayedDialogue;
        }
    }

    /// <summary>
    /// removes dialogue box and text
    /// </summary>
    void DisableDialogue()
    {
        dialogueCanvas.SetActive(false);
        isInteractingWith = false;

        if (hasPotion)
        {
            //ewww ew ew ewwwwwwwwwwwww
            //i can fix this later but i honestly can't be bothered rn
            CustomerManager.Instance.Invoke("SpawnNewCustomer", CustomerManager.Instance.Cooldown);
            Destroy(this.gameObject.transform.parent.gameObject);
        }
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

    public void DropItem()
    {

    }

    public void ThrowItem(Vector3 throwVec)
    {

    }
}
