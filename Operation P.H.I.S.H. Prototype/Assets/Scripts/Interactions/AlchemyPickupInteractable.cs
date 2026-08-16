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

    private Material standardMat;

    [SerializeField] private Material hoverMat;

    [SerializeField] private Material interactMat;

    private MeshRenderer mr;
    private Collider c;

    private AlchemyPlayerController heldBy;
    private Rigidbody rb;

    [HideInInspector] public Vector3 OriginalPos;

    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Start is called on the first frame update
    /// Grabs a reference to the mesh renderer and sets the base material
    /// </summary>
    void Start()
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
    public void EnterHover()
    {
        mr.material = hoverMat;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Changes the object's material when interacted with
    /// </summary>
    public void EnterInteract(AlchemyPlayerController pc)
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

    /// <summary>
    /// Drops an item without adding force
    /// </summary>
    public void DropItem()
    {
        ExitInteract();

        transform.parent = null;
        c.enabled = true;
        rb.isKinematic = false;
    }

    /// <summary>
    /// Adds force when dropping an item to allow it to be thrown
    /// </summary>
    public void ThrowItem(Vector3 throwVec)
    {
        transform.parent = null;
        c.enabled = true;
        rb.isKinematic = false;

        rb.AddForce(throwVec, ForceMode.Impulse);

        ExitInteract();


    }

    #endregion
}
