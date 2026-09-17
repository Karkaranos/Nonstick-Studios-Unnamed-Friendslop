/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    09/03/2026
Brief Description : 	Handles guest interactions/missions/systems
External Resources :    	
***************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [HideInInspector] public GuestEvent AssignedEvent = null;

    bool isInteractingWith = false;
    bool moving = false;

    NavMeshAgent agent;

    [Header("General")]
    [SerializeField] string guestName;
    //[SerializeField] List<GuestDialogue> listOfDialogue;
    [SerializeField] GuestDialogue[] listOfDialogue;

    Dictionary<DialogueContext, string> guestDialogue = new Dictionary<DialogueContext, string>();

    [Space(8)]

    [Header("UI")]
    [Tooltip("The icon that should appear when the guest has a request.")]
    public GameObject RequestPing;
    [Tooltip("The customer's canvas goes here!")]
    [SerializeField] GameObject dialogueCanvas;
    [Tooltip("The customer's dialogue goes here!")]
    [SerializeField] TMP_Text dialogueText;
    [Tooltip("The sprite that appears on the map"), ShowAssetPreview(32, 32)]
    public Sprite MapSprite;
    [SerializeField] TMP_Text SatisfactionDisplay;
    [HideInInspector] public GameObject RequestPingMap;

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

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentSatisfactionLevel = satisfactionLevel; 
        if (SatisfactionDisplay != null)
        {
            SatisfactionDisplay.text = $"Satisfaction: {currentSatisfactionLevel}";
        }

        StartCoroutine(MoveNavMesh(GuestAndEventManager.Instance.GuestLineLocation));
        
        foreach (GuestDialogue dialogue in listOfDialogue)
        {
            guestDialogue.Add(dialogue.Context, dialogue.Dialogue);
        }
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
        //bandaid fix to an obnoxious bug
        if (isInteractingWith)
        {
            return;
        }

        MoontelPickupInteractable pickup = null;

        if(pc.heldInteractable != null)
        {
            pickup = pc.heldInteractable;
        }

        if(pickup != null && pickup.GetComponent<KeyPickupInteractable>() != null && AssignedRoom == null)
        {
            KeyPickupInteractable keyPickupInteractable = pickup.GetComponent<KeyPickupInteractable>();

            foreach(RoomBehavior room in RoomManager.Instance.Rooms)
            {
                if(room.RoomID == keyPickupInteractable.KeyID && room.OccupyingGuest == null)
                {
                    AssignedRoom = room;
                    room.AssignGuest(this);
                    agent.enabled = false;
                    GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                    GuestAndEventManager.Instance.CheckLine(gameObject, gameObject.transform.position);
                    gameObject.transform.position = room.TeleportPoint.transform.position;
                    gameObject.transform.LookAt(room.RotateTowards);

                    Debug.Log($"{gameObject.name} CHECKED IN.");

                    break;
                }
            }
        }
        //tjis is messy mb
        else if(pickup != null && pickup.GetComponent<EventPickupInteractable>() && AssignedEvent != null && 
        AssignedEvent.RequestedItem == pickup.GetComponent<EventPickupInteractable>().TypeOfItem)
        {
            EventPickupInteractable eventPickupInteractable = 
            pc.heldInteractable.GetComponent<EventPickupInteractable>();

            transform.LookAt(pc.gameObject.transform.position);

            if (guestDialogue.ContainsKey(DialogueContext.FetchedTP) && 
            eventPickupInteractable.TypeOfItem == ItemType.ToiletPaper)
            {
                DisplayDialogue(guestDialogue[DialogueContext.FetchedTP]);
                Invoke("DisableDialogue", GuestAndEventManager.Instance.DialogueDisplayTime);
            }
            else if (guestDialogue.ContainsKey(DialogueContext.FetchedTowel) &&
            eventPickupInteractable.TypeOfItem == ItemType.Towel)
            {
                DisplayDialogue(guestDialogue[DialogueContext.FetchedTowel]);
                Invoke("DisableDialogue", GuestAndEventManager.Instance.DialogueDisplayTime);
            }

            ChangeSatisfaction(GuestAndEventManager.Instance.SatisfactionGainedPerEvent);
            GuestAndEventManager.Instance.ActiveEvents.Remove(this);
            AssignedEvent = null;

            if (RequestPing != null)
            {
                RequestPing.SetActive(false);
            }

            Destroy(pc.heldInteractable.gameObject);

            isInteractingWith = true;
        }
        else
        {
            transform.LookAt(pc.gameObject.transform.position);

            if(AssignedEvent != null)
            {
                if(AssignedEvent.EventType == TypeOfEvent.Fetch)
                {
                    if(AssignedEvent.RequestedItem == ItemType.ToiletPaper)
                    {
                        DisplayDialogue(guestDialogue[DialogueContext.FetchTP]);
                        Invoke("DisableDialogue", GuestAndEventManager.Instance.DialogueDisplayTime);
                    }
                    else if(AssignedEvent.RequestedItem == ItemType.Towel)
                    {
                        DisplayDialogue(guestDialogue[DialogueContext.FetchTowel]);
                        Invoke("DisableDialogue", GuestAndEventManager.Instance.DialogueDisplayTime);
                    }
                }
                else if(AssignedEvent.EventType == TypeOfEvent.Interact)
                {
                    if (AssignedEvent.RequiredTool == MinigameObjectType.Broom)
                    {
                        DisplayDialogue(guestDialogue[DialogueContext.Clean]);
                        Invoke("DisableDialogue", GuestAndEventManager.Instance.DialogueDisplayTime);
                    }
                    else if (AssignedEvent.RequiredTool == MinigameObjectType.Toolbox)
                    {
                        DisplayDialogue(guestDialogue[DialogueContext.Fix]);
                        Invoke("DisableDialogue", GuestAndEventManager.Instance.DialogueDisplayTime);
                    }
                }
            }
            else
            {
                if (guestDialogue.ContainsKey(DialogueContext.CheckingIn) && AssignedRoom == null)
                {
                    DisplayDialogue(guestDialogue[DialogueContext.CheckingIn]);
                    Invoke("DisableDialogue", GuestAndEventManager.Instance.DialogueDisplayTime);
                }
                else if (guestDialogue.ContainsKey(DialogueContext.CheckedIn) && AssignedRoom != null)
                {
                    DisplayDialogue(guestDialogue[DialogueContext.CheckedIn]);
                    Invoke("DisableDialogue", GuestAndEventManager.Instance.DialogueDisplayTime);
                }
            }

            isInteractingWith = true;
        }
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
        currentSatisfactionLevel += changeInSatisfaction;

        if(currentSatisfactionLevel < 0)
        {
            currentSatisfactionLevel = 0;
        }

        if(SatisfactionDisplay != null)
        {
            SatisfactionDisplay.text = $"Satisfaction: {currentSatisfactionLevel}";
        }
    }

    public int GetSatisfaction()
    {
        return currentSatisfactionLevel;
    }

    public int GetMoney()
    {
        return Mathf.CeilToInt(((float)currentSatisfactionLevel / 100f) * payAmount);
    }

    [Button]
    public void ForceCallQuotaUpdate()
    {
        PublicEvents.UpdateQuotaSatisfaction(GetSatisfaction());
        PublicEvents.UpdateQuotaMoney(GetMoney());
    }

    #endregion SATISFACTION
}
