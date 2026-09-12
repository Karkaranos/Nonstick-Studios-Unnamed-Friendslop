/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    09/09/2026
Brief Description : 	Handles guests' interactions with rooms and stores data
External Resources :    	
***************************************************/

using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{
    public int RoomID;
    public Transform TeleportPoint;

    Animator animator;
    List<RoomBehavior> neighboringRooms = new List<RoomBehavior>();

    //TODO(?): more room data here

    [HideInInspector] public GuestInteractable OccupyingGuest;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// assigns guests to this room
    /// </summary>
    /// <param name="newGuest"> assigned guest </param>
    public void AssignGuest(GuestInteractable newGuest)
    {
        animator.SetBool("IsOpen", true);

        OccupyingGuest = newGuest;

        //this is funny to me for some reason. enjoy the foreach loops, everyone <3
        foreach (RoomBehavior neighbor in neighboringRooms)
        {
            if(neighbor.OccupyingGuest != null)
            {
                foreach(GuestTraits guestTrait in OccupyingGuest.ExhibitedTraits)
                {
                    if(neighbor.OccupyingGuest.DislikedTraits.Contains(guestTrait))
                    {
                        neighbor.OccupyingGuest.ChangeSatisfaction
                        (GuestAndEventManager.Instance.SatisfactionDropPerRoom);
                        Debug.Log($"{neighbor.OccupyingGuest.gameObject.name}'s SATISFACTION {GuestAndEventManager.Instance.SatisfactionDropPerRoom}");
                    }
                }

                foreach(GuestTraits otherGuestTrait in neighbor.OccupyingGuest.ExhibitedTraits)
                {
                    if(OccupyingGuest.DislikedTraits.Contains(otherGuestTrait))
                    {
                        OccupyingGuest.ChangeSatisfaction
                        (GuestAndEventManager.Instance.SatisfactionDropPerRoom);
                        Debug.Log($"{OccupyingGuest.gameObject.name}'s SATISFACTION {GuestAndEventManager.Instance.SatisfactionDropPerRoom}");
                    }
                }
            }
        }
    }


    /// <summary>
    /// unassigns guest after they check out
    /// </summary>
    public void RemoveGuest()
    {
        OccupyingGuest = null;
        animator.SetBool("IsOpen", false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        //the latter two lines prevent a diagonal room from being counted as a neighboring room
        //if we want them to be counted, then they can be removed
        if(collider.GetComponent<RoomBehavior>() != null && 
        !neighboringRooms.Contains(collider.GetComponent<RoomBehavior>()) &&
        (collider.gameObject.transform.position.x == gameObject.transform.position.x || 
        collider.gameObject.transform.position.y == gameObject.transform.position.y))
        {
            neighboringRooms.Add(collider.GetComponent<RoomBehavior>());
        }
    }
}
