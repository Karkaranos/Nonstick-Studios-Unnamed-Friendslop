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
    public void BeginNewDive()
    {
        shipOxygen = shipOxygenMax;
        StartCoroutine(DepleteOxygen());
    }

    //makes the ship's oxygen tick down
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
                Debug.Log("OXYGEN LEFT: " + shipOxygen);
            }

            if(shipOxygen <= 0)
            {
                oxygenDepleting = false;

                //call the function that ends the dive here
                Debug.Log("DIVE OVER!");
            }
        }
    }

    //for losing oxygen to obstacles
    public void LoseOxygen(float oxygenLost)
    {
        shipOxygen -= oxygenLost;

        if(shipOxygen % shipOxygenStepAmount == 0)
        {
            //INSERT FUNCTION FOR UPDATING UI HERE
            Debug.Log("OXYGEN LEFT: " + shipOxygen);
        }
    }

    //this probably won't have anything in it for now but i'm basing this off of the flowchart
    public void AddToMaxOxygen()
    {

    }
}
