/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    09/12/2026
Brief Description : 	Stores guest dialogue
External Resources :    	
***************************************************/

using UnityEngine;

public enum DialogueContext
{
    CheckingIn,
    CheckedIn,
    
    FetchTP,
    FetchTowel,
    FetchedTP,
    FetchedTowel,

    Fix,
    Clean

}

[System.Serializable]
public class GuestDialogue
{
    public DialogueContext Context;
    public string Dialogue;
}
