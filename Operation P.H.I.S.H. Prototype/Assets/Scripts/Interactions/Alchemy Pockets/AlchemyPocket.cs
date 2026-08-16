/*************************************************
Author Names : 		    Toby Schamberger
Date Created : 		    8/16/2026
Date Last Modified : 	8/16/2026
Brief Description : 	Swaps material while looked at, stores interactables that the player is holding.
External Resources :    	
***************************************************/

using NaughtyAttributes;
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
    [SerializeField, ReadOnly] private PocketedItem pocketedItem;

    #region Functions
    /// <summary>
    /// Start is called on the first frame update
    /// Grabs a reference to the mesh renderer and sets the base material
    /// </summary>
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        standardMat = mr.material;
    }

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
    public void EnterInteract(AlchemyPlayerController pc)
    {
        mr.material = interactMat;
        Debug.Log($"{gameObject.name} is starting its interaction");

        PocketedItem oldPocketedItem = pocketedItem;
        PocketedItem newPocketedItem = pc.heldInteractable == null ? null : new PocketedItem(pc.heldInteractable);

        if (newPocketedItem != null)
        {
            // disables physics and the item now considers its holder to be none
            newPocketedItem.pickup.DropItem(updatePlayer: false);
            DisableItemToGoInPocket(newPocketedItem.pickup);
        }

        // if item was in pocket, it is now in the players hand
        if(oldPocketedItem.pickup != null)
        {
            oldPocketedItem.pickup.PickupItem(pc);
            oldPocketedItem.pickup.ToggleInteractable(true);
        }

        pocketedItem = newPocketedItem;
    }

    /// <summary>
    /// Disables physics and rescales object to go pocket mode.
    /// </summary>
    private void DisableItemToGoInPocket(PickupInteractable item)
    {
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

    #endregion


}
