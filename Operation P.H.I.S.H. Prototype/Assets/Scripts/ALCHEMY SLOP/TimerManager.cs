/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    08/19/2026
Brief Description : 	Controls the timer for alchemy slop.
External Resources :    	
***************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : Singleton<TimerManager>
{
    private Coroutine timerCoroutine = null;
    [SerializeField, Tooltip ("Seconds in between timer tick downs.")] private float timeBetweenIncrements;
    [SerializeField, Tooltip("How many times the timer will tick down.")] private int amountOfIncrements;
    [SerializeField, Tooltip("UI Slider for the timer.")] private Slider timerSlider;
    [SerializeField, Tooltip("UI canvas group for the timer/game over UI.")] private CanvasGroup gameOverScreenCanvasGroup;


    /// <summary>
    /// starts timer
    /// </summary>
    void Start()
    {
        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(RunTimer());
        }
    }

    /// <summary>
    /// Controls the timer countdown, turns on "game over" UI at the end of the countdown
    /// </summary>
    /// <returns></returns>
    private IEnumerator RunTimer()
    {
        float currentValue = timerSlider.value;
        int currentIncrement = 0;

        while (timerSlider.value > 0)
        {
            yield return new WaitForSeconds(timeBetweenIncrements);
            currentValue -= (1 / (float)amountOfIncrements);
            timerSlider.value = currentValue;

            currentIncrement++;
            
            foreach(Ingredients ingredient in IngredientManager.Instance.AllIngredients)
            {
                if(currentIncrement % ingredient.SpawnInterval == 0)
                {
                    IngredientManager.Instance.SpawnIngredient(ingredient);
                }
            }
        }

        //just to see that the timer reached 0
        yield return new WaitForSeconds(0.5f);

        gameOverScreenCanvasGroup.alpha = 1;
        timerCoroutine = null;
    }
}
