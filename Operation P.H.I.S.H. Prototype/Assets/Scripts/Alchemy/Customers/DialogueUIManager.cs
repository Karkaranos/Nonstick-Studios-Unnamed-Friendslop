/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/18/2026
Brief Description : 	Handles displaying dialogue
External Resources :    	
***************************************************/

using TMPro;
using UnityEngine;

public class DialogueUIManager : Singleton<DialogueUIManager>
{
    //this is stuff that i'm assuming could be reworked were we to implement actual ui

    [SerializeField] GameObject dialogueCanvas;
    [SerializeField] TMP_Text dialogue;

    /// <summary>
    /// displays dialogue box and text
    /// </summary>
    public void ManageDialogueDisplay(bool isDisplayed, string displayedDialogue = null)
    {
        if(isDisplayed)
        {
            dialogueCanvas.SetActive(true);
            dialogue.text = displayedDialogue;
        }
        else
        {
            dialogueCanvas.SetActive(false);
        }
    }
}
