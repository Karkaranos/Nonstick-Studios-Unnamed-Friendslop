/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    09/03/2026
Brief Description : 	Stores a list of (active) guests and spawns them
                        Handles events
External Resources :    	
***************************************************/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GuestAndEventManager : Singleton<GuestAndEventManager>
{
    [Header("Lists")]

    [SerializeField] List<GameObject> guests;
    public List<GameObject> ActiveGuestsInScene = new List<GameObject>();

    List<GameObject> guestQueue = new List<GameObject>();

    [Space(8)]

    [Header("Checking In")]

    [Tooltip("How long a guest's dialogue box will appear upon an interaction.")]
    public int DialogueDisplayTime;

    [Space(5)]

    [Tooltip("Where the guest will spawn before moving in line.")]
    public Vector3 GuestSpawnLocation;
    [Tooltip("Where the guest should head after spawning.")]
    public Vector3 GuestLineLocation;

    [Space(5)]

    [Tooltip("What's the earliest in the AMs that guests can check in?")]
    public int EarliestCheckInTime;
    [Tooltip("What's the latest in the PMs that guests can check in?")]
    public int LatestCheckInTime;

    [Space(5)]

    [Tooltip("Least amount of hours that will pass until more guests check in.")]
    public int CheckInIntervalMin;
    [Tooltip("Most amount of hours that will pass until more guests check in.")]
    public int CheckInIntervalMax;

    [Space(5)]

    [Tooltip("Least amount of guests that will check in per interval.")]
    [SerializeField] int GuestsPerCheckInMin;
    [Tooltip("Most amount of guests that will check in per ")]
    [SerializeField] int GuestsPerCheckInMax;

    [Space(5)]

    public int CheckOutTime;

    [Space(5)]

    [Tooltip("How many guests can be checked in at a time.")]
    [SerializeField] int maxAmountOfGuests;

    [HideInInspector] public int NextInterval;

    [Space(8)]

    [Header("Events and Requests")]

    [Tooltip("How much satisfaction that the guests lose per unfulfilled event.")]
    [SerializeField] int satisfactionDrop;

    //TODO: add list of events
    //add list of active events

    /// <summary>
    /// checks in guests per interval
    /// </summary>
    public void ChooseGuests(int day)
    {
        //if this was called specifically at 3pm
        NextInterval = 0;

        int numberOfGuests = Random.Range(GuestsPerCheckInMin, GuestsPerCheckInMax + 1);

        if(ActiveGuestsInScene.Count + numberOfGuests > maxAmountOfGuests)
        {
            numberOfGuests = maxAmountOfGuests - ActiveGuestsInScene.Count;
        }

        if (numberOfGuests > 0)
        {
            for(int i = 0; i < numberOfGuests; i++)
            {
                GameObject selectedGuest = guests[Random.Range(0, guests.Count)];
                guestQueue.Add(selectedGuest);
            }

            StartCoroutine(SpawnGuests(day));
        }

        NextInterval = Random.Range(CheckInIntervalMin, CheckInIntervalMax + 1);

        Debug.Log($"NEXT INTERVAL: {NextInterval}");
    }


    /// <summary>
    /// spawns guests at a stagger to prevent them from stacking on top of each other
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnGuests(int day)
    {
        foreach(GameObject guest in guestQueue)
        {
            GameObject newGuest = Instantiate(guest, GuestSpawnLocation, Quaternion.identity);
            guest.GetComponent<GuestInteractable>().CheckInDay = day;

            ActiveGuestsInScene.Add(newGuest);

            Debug.Log($"GUEST SPAWNED: {guest.name}");

            yield return new WaitForSeconds(3);
        }

        guestQueue.Clear();
    }

    /// <summary>
    /// runs upon the start of a new day
    /// increases the amount of days each guest has stayed if checked in
    /// </summary>
    public void UpdateGuestCheckIn(int day)
    {
        List<GameObject> guestsToCheckOut = new List<GameObject>();

        foreach (GameObject guest in ActiveGuestsInScene)
        {
            if (guest.GetComponent<GuestInteractable>().CheckedIn &&
               guest.GetComponent<GuestInteractable>().CheckInDay != day)
            {
                guest.GetComponent<GuestInteractable>().DaysSpent++;

                if (guest.GetComponent<GuestInteractable>().DaysSpent >=
                    guest.GetComponent<GuestInteractable>().StayTime)
                {
                    guestsToCheckOut.Add(guest);
                }
            }
        }

        //annoying but whatever
        foreach(GameObject newGuest in guestsToCheckOut)
        {
            if(ActiveGuestsInScene.Count - 1 > ActiveGuestsInScene.IndexOf(newGuest) &&
               !ActiveGuestsInScene[ActiveGuestsInScene.IndexOf(newGuest) + 1].GetComponent
               <GuestInteractable>().CheckedIn)
            {
                StartCoroutine(RearrangeLine(newGuest.transform.position, 
                ActiveGuestsInScene[ActiveGuestsInScene.IndexOf(newGuest) + 1]));
            }

            ActiveGuestsInScene.Remove(newGuest);
            Destroy(newGuest);
        }
    }

    IEnumerator RearrangeLine(Vector3 newPos, GameObject guest)
    {
        Vector3 oldPos = guest.transform.position;

        while(guest.transform.position != newPos)
        {
            guest.transform.position = Vector3.MoveTowards
            (guest.transform.position, newPos, Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        if(ActiveGuestsInScene.Count - 1 > ActiveGuestsInScene.IndexOf(guest) &&
           !ActiveGuestsInScene[ActiveGuestsInScene.IndexOf(guest) + 1].GetComponent
           <GuestInteractable>().CheckedIn)
        {
            StartCoroutine(RearrangeLine(oldPos,
            ActiveGuestsInScene[ActiveGuestsInScene.IndexOf(guest) + 1]));
        }
    }

    /// <summary>
    /// will choose a random number of guests for each task
    /// will pull that many guests from the list of active guests to initiate event
    /// </summary>
    public void PullGuestsAndEvents()
    {
        List<GameObject> availableGuests = ActiveGuestsInScene;

        //TODO: create events
        //place the following inside of a loop going through each event
        //return to later!!!!

        int numberOfRequests = Random.Range(0, availableGuests.Count + 1);

        if(numberOfRequests > 0)
        {
            for(int i = 0; i < numberOfRequests; i++)
            {
                GameObject selectedGuest = availableGuests
                [Random.Range(0, availableGuests.Count)];

                //TODO: initiate event

                availableGuests.Remove(selectedGuest);
            }
        }
    }    
}
