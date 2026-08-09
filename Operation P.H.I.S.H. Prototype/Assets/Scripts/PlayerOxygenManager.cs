/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    07/19/2026
Date Last Modified : 	08/09/2026
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

    Vector3 startingPosition;

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
    [HideInInspector] public bool LosingOxygen;

    #endregion VARIABLES

    /// <summary>
    /// runs on start
    /// adds itself to a list in OxygenUIManager
    /// </summary>
    void Start()
    {
        OxygenUIManager.Instance.PlayersInGame.Add(this);
        playerID = OxygenUIManager.Instance.PlayersInGame.IndexOf(this) + 1;

        startingPosition = gameObject.transform.position;
    }

    [Button]
    ///<summary>
    ///starts depleting the player's oxygen
    ///</summary>
    public void BeginDepletingOxygen()
    {
        OxygenUIManager.Instance.PlayerDisplays[OxygenUIManager.Instance.PlayersInGame.IndexOf(this)].
        GetComponent<Image>().fillAmount = 1;

        playerOxygen = playerOxygenMax;
        LosingOxygen = true;
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
        while (LosingOxygen && ShipDiveController.Instance.Diving)
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
                OxygenUIManager.Instance.PlayerDisplays[OxygenUIManager.Instance.PlayersInGame.IndexOf(this)].
                SetActive(false);
                OxygenUIManager.Instance.DeadPlayersInGame.Add(this);

                //i wanna disable movement in the future, but there's only one player for now
                //so it would be a redundant for now

                if(OxygenUIManager.Instance.DeadPlayersInGame.Count >=
                OxygenUIManager.Instance.PlayersInGame.Count)
                {
                    ShipResourceManager.Instance.CollectedTreasures.Clear();
                    ShipResourceManager.Instance.EndDive();

                    Debug.Log("YOU LOSE!");
                }

                ResetLocation();

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
        LosingOxygen = false;
        playerOxygen = playerOxygenMax;

        OxygenUIManager.Instance.PlayerDisplays[OxygenUIManager.Instance.PlayersInGame.IndexOf(this)].
        GetComponent<Image>().fillAmount = 1;
    }

    /// <summary>
    /// resets player's location
    /// </summary>
    public void ResetLocation()
    {
        //shitty but works for now
        gameObject.transform.position = startingPosition;
    }
}
