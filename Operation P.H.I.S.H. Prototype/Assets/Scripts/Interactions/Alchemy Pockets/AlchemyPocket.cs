/*************************************************
Author Names : 		    Toby Schamberger
Date Created : 		    8/16/2026
Date Last Modified : 	8/16/2026
Brief Description : 	Swaps material while looked at, stores interactables that the player is holding.
External Resources :    	
***************************************************/

using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class AlchemyPocket : MonoBehaviour, IAlchemyInteractable
{
    #region Materials
    private Material standardMat;

    [SerializeField, BoxGroup("Materials"), Required] private Material hoverMat;
    [SerializeField, BoxGroup("Materials"), Required] private Material interactMat;

    private MeshRenderer mr;

    #endregion

    [Header("Pocket Behaviour")]
    [SerializeField, Required] private Transform heldItemPosition;
    [SerializeField] private Vector3 maxHeldItemScale;

    [Header("Debug")]
    [SerializeField, ReadOnly] private AlchemyPocketedItem pocketedItem;

    private Transform anchorPoint;

    #region Functions
    /// <summary>
    /// Start is called on the first frame update
    /// Grabs a reference to the mesh renderer and sets the base material
    /// </summary>
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        standardMat = mr.material;

        CreateAnchorPoint();
    }

    #region Anchor Points

    /// <summary>
    /// Create an anchor point for this pocket to follow, since you cant raycast something thats childed to you (grrrrr)
    /// </summary>
    void CreateAnchorPoint()
    {
        anchorPoint = new GameObject().transform;
        anchorPoint.position = transform.position;
        //anchorPoint.localScale = transform.localScale;
        anchorPoint.rotation = transform.rotation;

        // its like that one parent swap movie
        anchorPoint.parent = transform.parent;
        this.transform.parent = null;

        StartCoroutine(FollowAnchorPoint());
    }

    /// <summary>
    /// Moves this transform to match the anchor point's.
    /// Runs forever
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowAnchorPoint()
    {
        while (true)
        {
            transform.position = anchorPoint.position;
            transform.rotation = anchorPoint.rotation;

            yield return null;
        }
    }

    #endregion


    /// <summary>
    /// Implemented function stub from IInteractable
    /// Changes the object's material when hovered over
    /// </summary>
    public void EnterHover()
    {
        mr.material = hoverMat;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Handles swapping the players currently held item with the item in the pocket
    /// </summary>
    public void EnterInteract(AlchemyPlayerController pc, bool standardInteraction = true)
    {
        mr.material = interactMat;
        Debug.Log($"Pocket: {gameObject.name} is starting its interaction");

        AlchemyPocketedItem oldPocketedItem = pocketedItem;
        AlchemyPocketedItem newPocketedItem = pc.heldInteractable == null ? null : new AlchemyPocketedItem(pc.heldInteractable);

        if (newPocketedItem != null)
        {
            // disables physics and the item now considers its holder to be none
            newPocketedItem.pickup.DropItem();
            DisableItemToGoInPocket(newPocketedItem.pickup);
        }

        // if there was an item was in pocket, it is now in the players hand
        if(oldPocketedItem != null && oldPocketedItem.pickup != null)
        {
            oldPocketedItem.pickup.PickupItem(pc);
            oldPocketedItem.pickup.ToggleInteractable(true);
        }

        pocketedItem = newPocketedItem;
    }

    /// <summary>
    /// Disables physics and rescales object to go pocket mode.
    /// </summary>
    private void DisableItemToGoInPocket(AlchemyPickupInteractable item)
    {
        Debug.Log($"Putting {item.gameObject.name} in pocket");
        item.TogglePhysics(false);
        item.ToggleInteractable(false);
        item.transform.parent = heldItemPosition;
        item.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Resets the object's material when hover ends
    /// </summary>
    public void ExitHover()
    {
        mr.material = standardMat;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Resets the object's material when interaction ends
    /// </summary>
    public void ExitInteract()
    {
        mr.material = standardMat;
        Debug.Log($"{gameObject.name} has ended its interaction");
    }

    public void ThrowItem(Vector3 throwVec)
    {
        // why?
    }

    public void DropItem()
    {
        // why?
    }

    #endregion


}
