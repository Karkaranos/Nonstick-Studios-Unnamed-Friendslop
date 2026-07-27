/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/27/2026
Date Last Modified : 	7/27/2026
Brief Description : 	Manages UI related to oxygen
External Resources : 	
***************************************************/
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OxygenUIManager : MonoBehaviour
{
    [Tooltip("The bar that will display the ship's oxygen.")]
    [SerializeField] Image shipOxygenDisplay;

    //i've got a vision
    [HideInInspector] public int NumberOfPlayers = 0;

    [SerializeField] List<Image> playerOxygenDisplays = new List<Image>();

    private void Start()
    {
       
    }
}
