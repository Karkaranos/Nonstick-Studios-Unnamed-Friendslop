/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/19/2026
Date Last Modified : 	7/19/2026
Brief Description : 	Stores and sets the values of the ship's resources
External Resources : 	list, including a link to the resource used
***************************************************/

using System.Collections;
using UnityEngine;

public class ShipResourceManager : Singleton<ShipResourceManager>
{
    //tracks the ship's current oxygen level
    float shipOxygen;

    //might need to make a separate variable from the inspector variable should we go forward with updating this #
    [Tooltip("The highest value that the oxygen can be.")]
    [SerializeField] float shipOxygenMax;

    [Tooltip("The amount of seconds in between when the ship's oxygen depletes.")]
    [SerializeField] float shipOxygenDepletionTime;

    [Tooltip("The amount of oxygen depleted per increment.")]
    [SerializeField] float shipOxygenStepAmount;

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
            shipOxygen -= shipOxygenStepAmount;

            Debug.Log("OXYGEN LEFT: " + shipOxygen);

            if(shipOxygen <= 0)
            {
                oxygenDepleting = false;

                //call the function that ends the dive here
                Debug.Log("DIVE OVER!");
            }
        }
    }

    //this probably won't have anything in it for now but i'm basing this off of the flowchart
    public void AddToMaxOxygen()
    {

    }
}
