using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    private bool isDay = true;
    private int currentSecond = 0;
    private int totalSeconds = 0;
    private int currentHour = 3;
    private int totalHours = 3;
    private int currentDay = 1;
    private int totalDays = 1;
    private int currentWeeks;
    private bool AM = false;

    private float secondsBetweenMinuteTicks = 1;

    private TMP_Text clockText;
    private Coroutine clockCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(clockCoroutine == null)
        {
            clockCoroutine = StartCoroutine(RunningClock());
        }
    }

    private IEnumerator RunningClock()
    {
        while(true)
        {
            yield return new WaitForSeconds(secondsBetweenMinuteTicks);

            //seconds tic
            currentSecond++;
            totalSeconds++;

            //check for new hour
            if (currentSecond % 60 == 0)
            {
                currentHour++;
                totalHours++;

                currentSecond = 0;
            }

            //convert to 12 hour clock
            if (currentHour > 12)
            {
                currentHour = 1;
                AM = !AM;
            }

            //check for new day
            if (AM)
            {
                currentDay++;
                totalDays++;
            }

            //check for new week
            if (currentDay > 7)
            {
                currentDay = 1;

                currentWeeks++;
            }
        }
    }
}
