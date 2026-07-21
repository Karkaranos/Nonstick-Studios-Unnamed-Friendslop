/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/19/2026
Date Last Modified : 	7/19/2026
Brief Description : 	Stores and sets the values of the player's resources
External Resources : 	
***************************************************/
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class PlayerOxygenManager : MonoBehaviour
{
    //tracks the player's personal oxygen level
    float playerOxygen;

    [Tooltip("The highest value that the oxygen can be.")]
    [SerializeField] float playerOxygenMax;

    [Tooltip("The amount of seconds in between when the player's oxygen depletes.")]
    [SerializeField] float playerOxygenDepletionTime;

    [Tooltip("The amount of oxygen depleted per increment.")]
    [SerializeField] float playerOxygenDepletionAmount;

    //may or may not change how this is detected later on
    //should this depend on a player variable?
    [HideInInspector] public bool IsAttachedToTether;

    [Button]
    ///<summary>
    ///starts depleting the player's oxygen
    ///</summary>
    public void BeginDepletingOxygen()
    {
        playerOxygen = playerOxygenMax;
        StartCoroutine(DepletePlayerOxygen());
    }

    /// <summary>
    /// depletes the player's oxygen per increment
    /// </summary>
    /// <returns> 
    /// time until the oxygen depletes again
    /// </returns>
    IEnumerator DepletePlayerOxygen()
    {
        while (!IsAttachedToTether)
        {
            yield return new WaitForSeconds(playerOxygenDepletionTime);
            playerOxygen -= playerOxygenDepletionAmount;

            //INSERT FUNCTION FOR UPDATING UI HERE
            Debug.Log($"OXYGEN LEFT: {playerOxygen}");

            if (playerOxygen <= 0)
            {
                //call the function that ends the dive here
                Debug.Log("YOU DIED!");

                yield break;
            }
        }
    }

    /// <summary>
    /// resets the player's oxygen to max
    /// </summary>
    public void ResetOxygen()
    {
        playerOxygen = playerOxygenMax;
    }
}
