/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/19/2026
Date Last Modified : 	8/6/2026
Brief Description : 	Stores and sets the values of the ship's resources
External Resources : 	
***************************************************/

using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class ShipResourceManager : Singleton<ShipResourceManager>
{

    #region VARIABLES

    //tracks the ship's current oxygen level
    float shipOxygen;

    //whether or not the ship's oxygen is going down
    [HideInInspector] bool oxygenDepleting;

    [Header("Oxygen Variables")]

    //might need to make a separate variable from the inspector variable should we go forward with updating this #
    [Tooltip("The highest value that the oxygen can be.")]
    [SerializeField] float shipOxygenMax;

    [Tooltip("How much the maximum oxygen should increase per treasure collected.")]
    [SerializeField] float shipOxygenPerTreasure;

    [Tooltip("The highest value that the oxygen can be upgraded to.")]
    [SerializeField] float shipOxygenUpgradedMax;

    [Tooltip("The amount of seconds in between when the ship's oxygen depletes.")]
    [SerializeField] float shipOxygenDepletionTime;

    [Tooltip("The amount of seconds that must pass before the UI is updated.")]
    [SerializeField] float shipOxygenStepAmount;

    [Tooltip("The amount of oxygen depleted per increment.")]
    [SerializeField] float shipOxygenDepletionAmount;


    [Space(10)]

    [Header("Scene Variables")]

    [Tooltip("Where the ship will be at the start of a dive.")]
    [SerializeField] Vector3 startingLocation;

    [HideInInspector] public List<Treasure> CollectedTreasures = new List<Treasure>();

    #endregion VARIABLES

    #region FUNCTIONS

    [Button]
    ///<summary>
    ///starts a new dive
    ///</summary>
    public void BeginDive()
    {
        shipOxygen = shipOxygenMax;
        StartCoroutine(DepleteOxygen());
        OxygenUIManager.Instance.DisplayUI();
    }

    /// <summary>
    /// ends a dive
    /// </summary>
    public void EndDive()
    {
        PublicEvents.HaltShipMovement();
        PublicEvents.ResetPlayerInteractions();

        oxygenDepleting = false;
        shipOxygen = shipOxygenMax;

        gameObject.transform.position = startingLocation;

        OxygenUIManager.Instance.UpdateShipOxygenUI(shipOxygenMax, shipOxygen);
        OxygenUIManager.Instance.StopDisplayingUI();

        OxygenUIManager.Instance.DeadPlayersInGame.Clear();

        AddToMaxOxygen();

        Debug.Log("DIVE OVER.");
    }

    /// <summary>
    /// makes the ship's oxygen tick down
    /// only displays lowered oxygen levels at certain intervals
    /// </summary>
    /// <returns>
    /// time until the oxygen depletes again
    /// </returns>
    IEnumerator DepleteOxygen()
    {
        oxygenDepleting = true;

        while (oxygenDepleting)
        {
            yield return new WaitForSeconds(shipOxygenDepletionTime);
            shipOxygen -= shipOxygenDepletionAmount;

            if (shipOxygen % shipOxygenStepAmount == 0)
            {
                OxygenUIManager.Instance.UpdateShipOxygenUI(shipOxygenMax, shipOxygen);
                Debug.Log($"OXYGEN LEFT: {shipOxygen}");
            }

            if (shipOxygen <= 0)
            {
                CollectedTreasures.Clear();
                EndDive();

                Debug.Log("YOU LOSE!");
            }
        }
    }

    /// <summary>
    /// depletes oxygen based on external forces
    /// </summary>
    /// <param name="oxygenLost"> 
    /// the amount of oxygen lost
    /// </param>
    public void LoseOxygen(float oxygenLost)
    {
        shipOxygen -= oxygenLost;

        if (shipOxygen % shipOxygenStepAmount == 0)
        {
            OxygenUIManager.Instance.UpdateShipOxygenUI(shipOxygenMax, shipOxygen);
            Debug.Log($"OXYGEN LEFT: {shipOxygen}");
        }
    }

    [Button]
    /// <summary>
    /// adds to the max amount of the ship's oxygen
    /// </summary>
    public void AddToMaxOxygen()
    {
        foreach (Treasure treasure in CollectedTreasures)
        {
            shipOxygenMax += shipOxygenPerTreasure;
        }

        if (shipOxygenMax > shipOxygenUpgradedMax)
        {
            shipOxygenMax = shipOxygenUpgradedMax;
        }

        CollectedTreasures.Clear();

        Debug.Log($"MAX OXYGEN: {shipOxygenMax}");
    }

    #endregion FUNCTIONS
}
