/*************************************************
Author Names : 		    Jay Embry, Toby Schamberger
Date Created : 		    09/03/2026
Date Last Modified : 	09/09/2026
Brief Description : 	Stores a list of (active) guests and spawns them
                        Handles events
External Resources :    	
***************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GuestAndEventManager : Singleton<GuestAndEventManager>
{
    [Header("Lists")]

    [SerializeField] List<GuestInteractable> guestPrefabs;
    [HideInInspector] public List<GuestInteractable> ActiveGuestsInScene = new List<GuestInteractable>();

    List<GuestInteractable> guestQueue = new List<GuestInteractable>(); // why not just use a queue lol //i didnt know what that was man

    [Space(5)]

    [SerializeField] MoontelGuestMapUI map;

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

    [Tooltip("How much satisfaction that the guests lose per unfulfilled event. Cannot be a positive value.")]
    [SerializeField, MaxValue(0)] int satisfactionDropPerEvent;
    [Tooltip("How much satisfaction that the guests lose per disliked neighbors. Cannot be a positive value.")]
    [MaxValue(0)] public int SatisfactionDropPerRoom;

    //TODO: add list of events
    //add list of active events

    //TODO: remove debug
    private void Start()
    {
        ActiveGuestsInScene = FindObjectsByType<GuestInteractable>(FindObjectsSortMode.None)
            .Select(g=>g.GetComponent<GuestInteractable>())
            .ToList();
    }

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
                GuestInteractable selectedGuest = guestPrefabs[Random.Range(0, guestPrefabs.Count)];
                guestQueue.Add(selectedGuest);

                //lemme see if this changes anything
                selectedGuest = null;
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
        foreach(GuestInteractable guest in guestQueue)
        {
            GuestInteractable newGuest = Instantiate(guest, GuestSpawnLocation, Quaternion.identity);
            newGuest.CheckInDay = day;

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
        List<GuestInteractable> guestsToCheckOut = new List<GuestInteractable>();

        foreach (GuestInteractable guest in ActiveGuestsInScene)
        {
            if (guest.GetComponent<GuestInteractable>().AssignedRoom != null &&
               guest.GetComponent<GuestInteractable>().CheckInDay != day)
            {
                guest.DaysSpent++;

                if (guest.DaysSpent >= guest.StayTime)
                {
                    guestsToCheckOut.Add(guest);
                }
            }
        }

        foreach(GuestInteractable oldGuest in guestsToCheckOut)
        {
            oldGuest.GetComponent<GuestInteractable>().AssignedRoom.RemoveGuest();

            map.RemoveGuest(oldGuest);
            ActiveGuestsInScene.Remove(oldGuest);

            Debug.Log($"{oldGuest.gameObject} CHECKED OUT.");

            Destroy(oldGuest.gameObject);
        }
    }

    /// <summary>
    /// checks if the line needs to move up
    /// </summary>
    /// <param name="guest"></param>
    public void CheckLine(GameObject guest, Vector3 pos)
    {
        if (ActiveGuestsInScene.Count - 1 > ActiveGuestsInScene.IndexOf(guest.GetComponent<GuestInteractable>()) &&
           ActiveGuestsInScene[ActiveGuestsInScene.IndexOf(guest.GetComponent<GuestInteractable>()) + 1].GetComponent
           <GuestInteractable>().AssignedRoom == null)
        {
            StartCoroutine(RearrangeLine
            (pos, ActiveGuestsInScene[ActiveGuestsInScene.IndexOf(guest.GetComponent<GuestInteractable>()) + 1].gameObject));
        }
    }

    //hey so i'm probably not using this anymore
    //least of my worries tbh
    IEnumerator RearrangeLine(Vector3 newPos, GameObject guest)
    {
        Vector3 oldPos = guest.transform.position;

        while(guest.transform.position != newPos)
        {
            guest.transform.position = Vector3.MoveTowards
            (guest.transform.position, newPos, Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        if(ActiveGuestsInScene.Count - 1 > ActiveGuestsInScene.IndexOf(guest.GetComponent<GuestInteractable>()) &&
           ActiveGuestsInScene[ActiveGuestsInScene.IndexOf(guest.GetComponent<GuestInteractable>()) + 1].GetComponent
           <GuestInteractable>().AssignedRoom == null)
        {
            StartCoroutine(RearrangeLine(oldPos,
            ActiveGuestsInScene[ActiveGuestsInScene.IndexOf(guest.GetComponent<GuestInteractable>()) + 1].gameObject));
        }
    }

    /// <summary>
    /// will choose a random number of guests for each task
    /// will pull that many guests from the list of active guests to initiate event
    /// </summary>
    public void PullGuestsAndEvents()
    {
        List<GuestInteractable> availableGuests = ActiveGuestsInScene;

        //TODO: create events
        //place the following inside of a loop going through each event
        //return to later!!!!

        int numberOfRequests = Random.Range(0, availableGuests.Count + 1);

        if(numberOfRequests > 0)
        {
            for(int i = 0; i < numberOfRequests; i++)
            {
                var selectedGuest = availableGuests
                [Random.Range(0, availableGuests.Count)];

                //TODO: initiate event

                availableGuests.Remove(selectedGuest);
            }
        }
    }    
}
