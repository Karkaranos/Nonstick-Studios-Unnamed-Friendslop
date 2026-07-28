/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/19/2026
Date Last Modified : 	7/28/2026
Brief Description : 	Stores and sets the values of the player's resources
External Resources : 	
***************************************************/
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOxygenManager : MonoBehaviour
{

    #region VARIABLES

    //tracks the player's personal oxygen level
    float playerOxygen;

    [Header("ID Display")]

    [Tooltip("DO NOT TOUCH! Reference this to make sure that the display is correct!")]
    [SerializeField] int playerID;

    [Space(10)]

    [Header("Oxygen Values")]

    [Tooltip("The highest value that the oxygen can be.")]
    [SerializeField] float playerOxygenMax;

    [Tooltip("The amount of seconds in between when the player's oxygen depletes.")]
    [SerializeField] float playerOxygenDepletionTime;

    [Tooltip("The amount of oxygen depleted per increment.")]
    [SerializeField] float playerOxygenDepletionAmount;

    [Space(10)]

    //may or may not change how this is detected later on
    //should this depend on a player variable?
    [HideInInspector] public bool IsAttachedToTether;

    #endregion VARIABLES

    /// <summary>
    /// runs on start
    /// adds itself to a list in OxygenUIManager
    /// </summary>
    void Start()
    {
        OxygenUIManager.Instance.PlayersInGame.Add(this);
        playerID = OxygenUIManager.Instance.PlayersInGame.IndexOf(this) + 1;
    }

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

            //putting this here for now only because there might be more than one tether??
            //someone please correct me if i'm wrong
            OxygenUIManager.Instance.PlayerDisplays[OxygenUIManager.Instance.PlayersInGame.IndexOf(this)].
            GetComponent<Image>().fillAmount = playerOxygen / playerOxygenMax;

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
