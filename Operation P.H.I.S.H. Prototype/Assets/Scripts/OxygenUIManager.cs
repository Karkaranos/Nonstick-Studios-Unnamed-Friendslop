/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/27/2026
Date Last Modified : 	8/6/2026
Brief Description : 	Manages UI related to oxygen
External Resources : 	
***************************************************/
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class OxygenUIManager : Singleton<OxygenUIManager>
{
    [Tooltip("The bar that will display the ship's oxygen.")]
    [SerializeField] Image shipOxygenDisplay;

    //i've got a vision
    //i'd imagine that this is temporary lol
    [HideInInspector] public List<PlayerOxygenManager> PlayersInGame = new List<PlayerOxygenManager>();
    public List<GameObject> PlayerDisplays = new List<GameObject>();

    /// <summary>
    /// displays the ui for each player once a dive begins
    /// </summary>
    public void DisplayUI()
    {
        for(int i = 0; i < PlayersInGame.Count; i++)
        {
            PlayerDisplays[i].SetActive(true);
        }
    }

    /// <summary>
    /// stops displaying the ui for each player once a dive ends
    /// </summary>
    public void StopDisplayingUI()
    {
        for (int i = 0; i < PlayersInGame.Count; i++)
        {
            PlayerDisplays[i].SetActive(false);
        }
    }

    /// <summary>
    /// updates the bar depicting the ship's oxygen
    /// </summary>
    /// <param name="maxOxygen">
    /// the max amount of oxygen that the ship can have
    /// </param>
    /// <param name="currentOxygen">
    /// the amount of oxygen that the ship currently has
    /// </param>
    public void UpdateShipOxygenUI(float maxOxygen, float currentOxygen)
    {
        shipOxygenDisplay.fillAmount = currentOxygen / maxOxygen;
    }
}
