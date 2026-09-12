/*************************************************
Author Names : 		    Cade Naylor, Toby Schamberger, Sky Beal
Date Created : 		    07/30/2026
Date Last Modified : 	09/09/2026
Brief Description : 	A test object for pick up interactions
                        Stolen from PHISH for Moontel
External Resources :    	
***************************************************/
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer), typeof(Collider))]
public class MoontelPickupInteractable : MonoBehaviour, IMoontelInteractable
{
    #region VARS
    private Material standardMat;

    [SerializeField] private Material hoverMat;

    [SerializeField] private Material interactMat;

    private MeshRenderer mr;
    private Collider col;

    [SerializeField, Layer] private int shipLayer;
    private MoontelPlayerController heldBy;
    private Rigidbody rb;

    [HideInInspector] public Vector3 OriginalPosition;
    [HideInInspector] public Vector3 OriginalScale;

    [SerializeField, BoxGroup("Debug")] private bool isToggled = true;
    private bool isHeld => heldBy != null;
    #endregion

    #region Functions
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

    #region Pickup Functions

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
    public void PickupItem(MoontelPlayerController pc)
    {
        mr.material = interactMat;

        heldBy = pc;
        TogglePhysics(false);
        transform.parent = pc.PickupPoint;
        transform.localPosition = Vector3.zero;

        if (pc.heldInteractable != this)
            pc.SetPickupItem(this);
    }

    /// <summary>
    /// Drops the item.
    /// </summary>
    public virtual void DropItem()
    {
        mr.material = standardMat;

        heldBy = null;
        transform.parent = null;
        TogglePhysics(true);
    }

    #endregion

    #region Interaction Implementation
    /// <summary>
    /// Implemented function stub from IMoontelInteractable
    /// Changes the object's material when hovered over
    /// </summary>
    public void EnterHover()
    {
        if (!IsPickupable()) return;

        mr.material = hoverMat;
    }

    /// <summary>
    /// Implemented function stub from IMoontelInteractable
    /// Resets the object's material when hover ends
    /// </summary>
    public void ExitHover()
    {
        mr.material = standardMat;
    }

    /// <summary>
    /// Implemented function stub from IMoontelInteractable
    /// Resets the object's material when interaction ends
    /// </summary>
    public virtual void ExitInteract()
    {
        DropItem();

        Debug.Log($"{gameObject.name} has ended its interaction");
    }

    #endregion

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

    public void TogglePhysics(bool physicsEnabled)
    {
        col.enabled = physicsEnabled;
        rb.isKinematic = !physicsEnabled;
    }

    public void EnterInteract(MoontelPlayerController pc)
    {
        if (!IsPickupable()) return;

        PickupItem(pc);

        Debug.Log($"{gameObject.name} is starting its interaction");
    }

    #endregion

}
