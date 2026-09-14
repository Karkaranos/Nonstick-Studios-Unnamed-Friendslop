/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    09/03/2026
Brief Description : 	Handles guest interactions/missions/systems
External Resources :    	
***************************************************/

using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Rendering.MaterialUpgrader;

//TODO: change to be more accurate later
public enum GuestTraits
{
    Noisy,
    Quiet,
    Fragrant,
    Smelly
}

public class GuestInteractable : MonoBehaviour, IMoontelInteractable
{
    [HideInInspector] public int DaysSpent = 0;
    [HideInInspector] public int CheckInDay;

    [HideInInspector] public RoomBehavior AssignedRoom;

    bool isInteractingWith = false;
    bool moving = false;

    NavMeshAgent agent;

    [Header("General")]
    [SerializeField] string guestName;
    [SerializeField] string dialogue;

    [Space(8)]

    [Header("UI")]
    [Tooltip("The icon that should appear when the guest has a request.")]
    [SerializeField] GameObject requestPing;
    [Tooltip("The customer's canvas goes here!")]
    [SerializeField] GameObject dialogueCanvas;
    [Tooltip("The customer's dialogue goes here!")]
    [SerializeField] TMP_Text dialogueText;
    [Tooltip("The sprite that appears on the map"), ShowAssetPreview(32,32)]
    public Sprite MapSprite;

    [Space(8)]

    [Header("Stats")]
    [Tooltip("How satisfied are they to begin with?")]
    [SerializeField] int satisfactionLevel;

    int currentSatisfactionLevel;

    [Space(5)]

    public int StayTime;
    [SerializeField] float payAmount;

    [Space(5)]

    public List<GuestTraits> ExhibitedTraits;
    public List<GuestTraits> DislikedTraits;

    //[Space(8)]

    //[Header("Missions")]
    //[SerializeField] bool hasMission;
    //[ShowIf("hasMission")] public int DayOfMission;

    //TODO: create missions
    //TODO: add active mission

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(MoveNavMesh(GuestAndEventManager.Instance.GuestLineLocation));
    }

    IEnumerator MoveNavMesh(Vector3 newPos)
    {
        moving = true;

        while(moving && agent.isOnNavMesh && Vector3.Distance(gameObject.transform.position, newPos) >= 0)
        {
            agent.SetDestination(newPos);
            yield return new WaitForFixedUpdate();
        }

        moving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<GuestInteractable>() && AssignedRoom == null)
        {
            moving = false;
            agent.isStopped = true;
        }
    }

    #region IINTERACTABLE

    public void EnterInteract(MoontelPlayerController pc)
    {
        if (isInteractingWith)
        {
            return;
        }

        if(pc.heldInteractable .GetComponent<KeyPickupInteractable>() != null)
        {
            foreach(RoomBehavior room in RoomManager.Instance.Rooms)
            {
                if(room.RoomID == pc.heldInteractable.GetComponent<KeyPickupInteractable>().KeyID &&
                   room.OccupyingGuest == null)
                {
                    AssignedRoom = room;
                    room.AssignGuest(this);

                    //not actually messing with navmesh more rn sorry
                    //StartCoroutine(MoveNavMesh(room.gameObject.transform.position));

                    moving = false;
                    agent.enabled = false;

                    GuestAndEventManager.Instance.CheckLine(gameObject, gameObject.transform.position);
                    gameObject.transform.position = room.TeleportPoint.transform.position;

                    Debug.Log($"{gameObject.name} CHECKED IN.");

                    break;
                }
            }

            return;
        }

            transform.LookAt(pc.gameObject.transform.position);

        DisplayDialogue(dialogue);

        if (this != null)
        {
            Invoke("DisableDialogue", GuestAndEventManager.Instance.DialogueDisplayTime);
        }

        isInteractingWith = true;
    }

    public void ExitInteract()
    {

    }

    public void EnterHover()
    {

    }

    public void ExitHover()
    {

    }

    public void DropItem()
    {
        //needed for minigame items
        throw new System.NotImplementedException();
    }

    #endregion IINTERACTABLE

    #region DIALOGUE

    /// <summary>
    /// displays dialogue box and text
    /// </summary>
    void DisplayDialogue(string activeDialogue)
    {
        if (this != null)
        {
            dialogueCanvas.SetActive(true);
            dialogueText.text = activeDialogue;
        }
    }

    /// <summary>
    /// removes dialogue box and text
    /// </summary>
    void DisableDialogue()
    {
        dialogueCanvas.SetActive(false);
        isInteractingWith = false;
    }

    #endregion DIALOGUE

    #region SATISFACTION

    /// <summary>
    /// handles the guest's satisfaction
    /// for losing satisfaction, changeInSatisfaction should be a negative number
    /// </summary>
    /// <param name="changeInSatisfaction"> how much satisfaction the guest gains or loses</param>
    public void ChangeSatisfaction(int changeInSatisfaction)
    {
        //TODO: UI lol
        currentSatisfactionLevel += changeInSatisfaction;
    }

    #endregion SATISFACTION
}
