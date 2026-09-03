/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    9/1/2026
Date Last Modified : 	9/2/2026
Brief Description : 	Controls the timer for the day/night cycle, Updates UI accordingly
External Resources :    	
***************************************************/
using NaughtyAttributes;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DayNightManager : Singleton<DayNightManager>
{
    #region Variables
    [Header("Editable Variables")]
    [SerializeField, Tooltip("The hour the game starts on, decides when new day starts."), MinValue(1), MaxValue(12)] private int startingHour = 3;
    [SerializeField, Tooltip("If the day starts in the AM or PM.")] private bool startingAM = false;
    [SerializeField, Tooltip("How long (in seconds) it takes to go to a new minute.")] private float secondsBetweenMinuteTicks = 1;

    //minutes
    private int currentMinute = 0;
    [HideInInspector] public int totalMinutes;

    //hours
    private int currentHour;
    [HideInInspector] public int totalHours;

    //days
    private int currentDay = 1;
    [HideInInspector] public int totalDays;

    //weeks
    private int currentWeek = 1;

    [Header("Required References")]
    [SerializeField, Tooltip("Text box for days.")] private TMP_Text dayText;
    [SerializeField, Tooltip("Text box for weeks.")] private TMP_Text weekText;
    [SerializeField, Tooltip("Text box for current time (X:XX AM/PM).")] private TMP_Text clockText;

    private bool AM = false;
    private Coroutine clockCoroutine;

    #endregion

    /// <summary>
    /// Setting variables and starting timer
    /// </summary>
    void Start()
    {
        currentHour = startingHour;
        AM = startingAM;

        if (clockCoroutine == null)
        {
            clockCoroutine = StartCoroutine(RunningClock());
        }
        
    }

    /// <summary>
    /// Starts the live clock - manages current minutes, hours, days, and weeks.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RunningClock()
    {
        while(true)
        {
            yield return new WaitForSeconds(secondsBetweenMinuteTicks);

            //seconds tic
            currentMinute++;
            totalMinutes++;

            //check for new hour
            if (currentMinute == 60)
            {
                currentHour++;
                totalHours++;

                currentMinute = 0;
            }

            //convert to 12 hour clock
            if (currentHour > 12)
            {
                currentHour = 1;
            }

            //change AM
            if (currentHour > 11 && currentMinute == 0)
            {
                AM = !AM;
            }

            //check for new day
            if (AM == startingAM && currentMinute == 0 && currentHour == startingHour)
            {
                currentDay++;
                totalDays++;
            }

            //check for new week
            if (currentDay > 7)
            {
                currentDay = 1;
                currentWeek++;
            }

            UpdateClockUI();
            yield return null;
        }
    }

    /// <summary>
    /// Updates the Clock UI with the current time.
    /// </summary>
    private void UpdateClockUI()
    {
        dayText.text = "Day: " + currentDay;
        weekText.text = "Week: " + currentWeek;

        //formatting (I didn't want the leading 0)
        DateTime clockTime = new(1, 1, 1, currentHour, currentMinute, 0);
        clockText.text = clockTime.ToString("h:mm");

        //add AM or PM
        if (AM)
        {
            clockText.text += " AM";
        }
        else if (!AM)
        {
            clockText.text += " PM";
        }
    }
}
