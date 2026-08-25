/*************************************************
Author Names : 		    Jacob Bateman, Cade Naylor
Date Created : 		    08/16/2026
Brief Description : 	Component that all alchemy pickup interactables should have attached.
Jacob Note:             Another refactor of a PHISH script to make it work for AS.

External Resources :    	
***************************************************/
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer), typeof(Collider))]
public class AlchemyPickupInteractable : MonoBehaviour, IAlchemyInteractable
{
    #region VARS

    [SerializeField] private LayerMask layerToIgnore;

    [Space(1)]

    private Material standardMat;

    [SerializeField] private Material hoverMat;

    [SerializeField] private Material interactMat;

    private MeshRenderer mr;
    private Collider col;

    private AlchemyPlayerController heldBy;
    protected Rigidbody rb;

    protected bool disablePlayerCollision = false;

    [HideInInspector] public Vector3 OriginalPosition;
    [HideInInspector] public Vector3 OriginalScale;

    [SerializeField, BoxGroup("Debug")] private bool isToggled = true;
    private bool isHeld => heldBy != null;

    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Start is called on the first frame update
    /// Grabs a reference to the mesh renderer and sets the base material
    /// </summary>
    public virtual void Start()
    {
        mr = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        standardMat = mr.material;

        OriginalPosition = gameObject.transform.position;
        OriginalScale = gameObject.transform.lossyScale;
    }

    /// <summary>
    /// Return true if pickup can be... picked up
    /// </summary>
    public bool IsPickupable()
    {
        return isToggled && !isHeld;
    }

    /// <summary>
    /// Sets the item as picked up
    /// </summary>
    public void PickupItem(AlchemyPlayerController pc)
    {
        mr.material = interactMat;

        heldBy = pc;
        TogglePhysics(false);
        transform.parent = pc.PickupPoint;
        transform.localPosition = Vector3.zero;

        if(pc.heldInteractable != this)
            pc.SetPickupItem(this);
    }

    /// <summary>
    /// Drops an item without adding force
    /// </summary>
    public void DropItem()
    {
        Debug.Log($"Dropping {gameObject.name}");

        if(heldBy != null && heldBy.heldInteractable == this)
        {
            // do this to prevent stack overflow
            var oldHeldBy = heldBy;
            heldBy = null;
            oldHeldBy.SetPickupItem(null);
        }
            

        transform.parent = null;
        col.enabled = true;
        rb.isKinematic = false;
        heldBy = null;  
    }

    /// <summary>
    /// Adds force when dropping an item to allow it to be thrown
    /// </summary>
    public virtual void ThrowItem(Vector3 throwVec)
    {
        rb.excludeLayers = layerToIgnore;
        transform.parent = null;
        col.enabled = true;
        rb.isKinematic = false;

        rb.AddForce(throwVec, ForceMode.Impulse);

        ExitInteract();


    }

    /// <summary>
    /// Toggles if item can be physically interacted with
    /// </summary>
    /// <param name="physicsEnabled"></param>
    public void TogglePhysics(bool physicsEnabled)
    {
        col.enabled = physicsEnabled;
        rb.isKinematic = !physicsEnabled;
    }

    /// <summary>
    /// Toggle if player can pickup this guy
    /// </summary>
    /// <param name="interactable"></param>
    public void ToggleInteractable(bool interactable)
    {
        isToggled = interactable;

        if (!interactable)
        {
            ExitInteract();
            ExitHover();
        }
    }

    #region Interaction Implementation
    /// <summary>
    /// Implemented function stub from IInteractable
    /// Changes the object's material when hovered over
    /// </summary>
    public void EnterHover()
    {
        if (!IsPickupable()) return;

        mr.material = hoverMat;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Changes the object's material when interacted with
    /// </summary>
    public virtual void EnterInteract(AlchemyPlayerController apc, bool standardInteraction = true)
    {
        if (!IsPickupable()) return;

        PickupItem(apc);

        Debug.Log($"{gameObject.name} is starting its interaction");
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
    public virtual void ExitInteract()
    {
        //why would we drop it when E is released?
        //DropItem();

        Debug.Log($"{gameObject.name} has ended its interaction");
    }

    #endregion


    public virtual void OnCollisionEnter(Collision collision)
    {
        if (disablePlayerCollision)
        {
            //enable player collision
            rb.excludeLayers = LayerMask.NameToLayer("Nothing");

            disablePlayerCollision = false;
        }
    }

    #endregion
}
