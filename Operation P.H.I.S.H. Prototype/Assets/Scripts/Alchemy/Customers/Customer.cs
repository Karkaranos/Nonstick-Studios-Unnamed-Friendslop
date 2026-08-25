/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/18/2026
Brief Description : 	Script for interacting with customers
External Resources :    	
***************************************************/

using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Customer", menuName = "Scriptable Objects/Customer")]

public class Customer : ScriptableObject
{
    [ResizableTextArea] public string RequestDialogue;

    [Space(5)]

    public PotionType CorrectPotion;
    public List<PotionType> AcceptablePotions;

    [Space(5)]

    public float CorrectPotionPayment;
    public float AcceptablePotionPayment;
    public float WrongPotionPayment;

    [Space(5)]

    [ResizableTextArea] public string CorrectPotionDialogue;
    [ResizableTextArea] public string AcceptablePotionDialogue;
    [ResizableTextArea] public string WrongPotionDialogue;

    public Customer(string request, PotionType correctPotion, List<PotionType> acceptablePotions,
    float correctPotionPayment, float acceptablePotionPayment, float wrongPotionPayment,
    string correctPotionDialogue, string acceptablePotionDialogue, string wrongPotionDialogue)
    {
        this.RequestDialogue = request;

        this.CorrectPotion = correctPotion;
        this.AcceptablePotions = acceptablePotions;

        this.CorrectPotionPayment = correctPotionPayment;
        this.AcceptablePotionPayment = acceptablePotionPayment;
        this.WrongPotionPayment = wrongPotionPayment;

        this.CorrectPotionDialogue = correctPotionDialogue;
        this.AcceptablePotionDialogue = wrongPotionDialogue;
        this.WrongPotionDialogue = wrongPotionDialogue;
    }
}
