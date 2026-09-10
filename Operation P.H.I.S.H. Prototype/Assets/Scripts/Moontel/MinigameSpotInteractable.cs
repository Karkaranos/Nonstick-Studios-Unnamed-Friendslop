/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    09/09/2026
Brief Description : 	Controls functionality for the fix and clean minigame "messes"
External Resources :    	
***************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MinigameSpotInteractable : MoontelPickupInteractable
{
    [Header("Design Variabless")]
    public MinigameObjectType objectTypeNeeded;
    [SerializeField] private float secondsToCleanOrFix;

    //lol
    //not implemented yet but might be nice for integration later
    private float satisfactionUponCompletion;

    [Header("Required References")]
    [SerializeField] private Slider fixedOrCleanedSlider;

    private float currentSecondsComplete;

    private Coroutine cleanOrFixCoroutine;
    private bool shouldRunCoroutine = false;

    /// <summary>
    /// Starts timer count up until complete
    /// </summary>
    public void StartCleanOrFixTimer()
    {
        if (cleanOrFixCoroutine == null)
        {
            shouldRunCoroutine = true;
            cleanOrFixCoroutine = StartCoroutine(CleanOrFixTimer());
        }
    }

    /// <summary>
    /// Stops timer count up
    /// </summary>
    public void StopCleanOrFixTimer()
    {
        if (cleanOrFixCoroutine != null)
        {
            shouldRunCoroutine = false;
            cleanOrFixCoroutine = null;
            StopCoroutine(CleanOrFixTimer());
        }
    }

    /// <summary>
    /// Runs cleaning/fixing timer until completed, updates UI accordingly
    /// </summary>
    /// <returns></returns>
    private IEnumerator CleanOrFixTimer()
    {
        while (shouldRunCoroutine)
        {
            if (currentSecondsComplete > secondsToCleanOrFix)
            {
                //spot is cleaned or fixed
                gameObject.SetActive(false);
                cleanOrFixCoroutine = null;
                yield break; 
            }
            else
            {
                currentSecondsComplete += Time.deltaTime;
                fixedOrCleanedSlider.value = currentSecondsComplete / secondsToCleanOrFix;

                yield return null;
            }
        }

    }
}
