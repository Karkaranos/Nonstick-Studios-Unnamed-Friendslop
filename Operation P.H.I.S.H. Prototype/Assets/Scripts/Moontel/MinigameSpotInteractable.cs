using System.Collections;
using UnityEngine;

public class MinigameSpotInteractable : MoontelPickupInteractable
{
    [Header("Minigame")]
     public MinigameObjectType objectTypeNeeded;

    [SerializeField] private float secondsToCleanOrFix;
    private float currentSecondsComplete;

    private Coroutine cleanOrFixCoroutine;
    private bool shouldRunCoroutine = false;

    public void StartCleanOrFixTimer()
    {
        if (cleanOrFixCoroutine == null)
        {
            shouldRunCoroutine = true;
            cleanOrFixCoroutine = StartCoroutine(CleanOrFixTimer());
        }
    }
    public void StopCleanOrFixTimer()
    {
        if (cleanOrFixCoroutine != null)
        {
            shouldRunCoroutine = false;
            cleanOrFixCoroutine = null;
            StopCoroutine(CleanOrFixTimer());
        }
    }

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

                Debug.Log("Counting...");
                yield return null;
            }
        }

    }
}
