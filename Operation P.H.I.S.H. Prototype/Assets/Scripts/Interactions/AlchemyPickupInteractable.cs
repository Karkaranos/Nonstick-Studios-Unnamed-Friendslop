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
    private Collider c;

    private AlchemyPlayerController heldBy;
    private Rigidbody rb;

    protected bool disablePlayerCollision = false;

    [HideInInspector] public Vector3 OriginalPos;

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
        c = GetComponent<Collider>();
        standardMat = mr.material;

        OriginalPos = gameObject.transform.position;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Changes the object's material when hovered over
    /// </summary>
    public virtual void EnterHover()
    {
        mr.material = hoverMat;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Changes the object's material when interacted with
    /// </summary>
    public virtual void EnterInteract(AlchemyPlayerController pc)
    {
        mr.material = interactMat;

        heldBy = pc;
        rb.isKinematic = true;
        c.enabled = false;
        transform.parent = pc.PickupPoint;
        transform.localPosition = Vector3.zero;

        Debug.Log($"{gameObject.name} is starting its interaction");
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Resets the object's material when hover ends
    /// </summary>
    public virtual void ExitHover()
    {
        mr.material = standardMat;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Resets the object's material when interaction ends
    /// </summary>
    public virtual void ExitInteract()
    {
        mr.material = standardMat;

        Debug.Log($"{gameObject.name} has ended its interaction");
    }

    /// <summary>
    /// Drops an item without adding force
    /// </summary>
    public virtual void DropItem()
    {
        ExitInteract();

        transform.parent = null;
        c.enabled = true;
        rb.isKinematic = false;
    }

    /// <summary>
    /// Adds force when dropping an item to allow it to be thrown
    /// </summary>
    public virtual void ThrowItem(Vector3 throwVec)
    {
        rb.excludeLayers = layerToIgnore;
        transform.parent = null;
        c.enabled = true;
        rb.isKinematic = false;

        rb.AddForce(throwVec, ForceMode.Impulse);

        ExitInteract();


    }

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
