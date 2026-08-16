/*************************************************
Author Names : 		    Cade Naylor, Toby Schamberger
Date Created : 		    07/30/2026
Date Last Modified : 	08/16/2026
Brief Description : 	A test object for pick up interactions
External Resources :    	
***************************************************/
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer), typeof(Collider))]
public class PickupInteractable : MonoBehaviour, IInteractable, IAlchemyInteractable
{
    #region VARS
    private Material standardMat;

    [SerializeField] private Material hoverMat;

    [SerializeField] private Material interactMat;

    private MeshRenderer mr;
    private Collider col;

    [SerializeField, Layer] private int shipLayer;
    private PlayerController heldBy;
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
    void Start()
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
    public void PickupItem(PlayerController pc)
    {
        mr.material = interactMat;

        heldBy = pc;
        TogglePhysics(false);
        transform.parent = pc.PickupPoint;
        transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Sets the item as picked up
    /// </summary>
    public void PickupItem(PlayerController pc)
    {
        mr.material = interactMat;

        heldBy = pc;
        TogglePhysics(false);
        transform.parent = pc.PickupPoint;
        transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Drops the item.
    /// </summary>
    public void DropItem(bool updatePlayer = true)
    {
        mr.material = standardMat;

        heldBy.SetPickupItem(null);

        heldBy = null;
        transform.parent = null;
        TogglePhysics(true);
    }

    #endregion

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
    public void EnterInteract(PlayerController pc)
    {
        if (!IsPickupable()) return;

        PickupItem(pc);   

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
    public void ExitInteract()
    {
        DropItem();

        Debug.Log($"{gameObject.name} has ended its interaction");
    }

    #endregion

    /// <summary>
    /// When this object collides with another object, child it if it's collided with the ship
    /// </summary>
    /// <param name="collision">Information about the collision</param>
    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.layer == shipLayer)
        {
            GameObject newParent = collision.gameObject;

            // theres so many better ways to do this but this works for now
            while(!newParent.name.Contains("Ship") && newParent.transform.parent != null)
            {
                newParent = newParent.transform.parent.gameObject;
            }

            transform.parent = newParent.transform;

            if(!ShipResourceManager.Instance.CollectedTreasures.Contains(gameObject))
            {
                ShipResourceManager.Instance.CollectedTreasures.Add(gameObject);
                Debug.Log("LOOT COLLECTED.");
            }
        }
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

    public void TogglePhysics(bool physicsEnabled)
    {
        col.enabled = physicsEnabled;
        rb.isKinematic = !physicsEnabled;
    }

    public void EnterInteract(AlchemyPlayerController pc)
    {
        if (!IsPickupable()) return;

        PickupItem(pc);

        Debug.Log($"{gameObject.name} is starting its interaction");
    }

    #endregion

}
