/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/19/2026
Date Last Modified : 	7/19/2026
Brief Description : 	Stores and sets the values of the ship's resources
External Resources : 	
***************************************************/

using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class ShipResourceManager : Singleton<ShipResourceManager>
{
    //tracks the ship's current oxygen level
    float shipOxygen;

    [Header("Oxygen Variables")]

    //might need to make a separate variable from the inspector variable should we go forward with updating this #
    [Tooltip("The highest value that the oxygen can be.")]
    [SerializeField] float shipOxygenMax;

    [Tooltip("The amount of seconds in between when the ship's oxygen depletes.")]
    [SerializeField] float shipOxygenDepletionTime;

    [Tooltip("The amount of seconds that must pass before the UI is updated.")]
    [SerializeField] float shipOxygenStepAmount;

    [Tooltip("The amount of oxygen depleted per increment.")]
    [SerializeField] float shipOxygenDepletionAmount;

    [Button]
    ///<summary>
    ///starts depleting the ship's oxygen
    ///</summary>
    public void BeginNewDive()
    {
        shipOxygen = shipOxygenMax;
        StartCoroutine(DepleteOxygen());
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
        bool oxygenDepleting = true;

        while(oxygenDepleting)
        {
            yield return new WaitForSeconds(shipOxygenDepletionTime);
            shipOxygen -= shipOxygenDepletionAmount;

            if(shipOxygen % shipOxygenStepAmount == 0)
            {
                //INSERT FUNCTION FOR UPDATING UI HERE
                Debug.Log($"OXYGEN LEFT: {shipOxygen}");
            }

            if(shipOxygen <= 0)
            {
                oxygenDepleting = false;

                //call the function that ends the dive here
                Debug.Log("DIVE OVER!");
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

        if(shipOxygen % shipOxygenStepAmount == 0)
        {
            //INSERT FUNCTION FOR UPDATING UI HERE
            Debug.Log($"OXYGEN LEFT: { shipOxygen}");
        }
    }

    /// <summary>
    /// adds to the max amount of the ship's oxygen
    /// does nothing for now, but is based off of design's flowchart
    /// </summary>
    public void AddToMaxOxygen()
    {

    }
}
