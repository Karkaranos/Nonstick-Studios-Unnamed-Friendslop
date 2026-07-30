/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    07/30/2026
Date Last Modified : 	07/30/2026
Brief Description : 	A test object for pick up interactions
External Resources :    	
***************************************************/
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
public class PickupInteractable : MonoBehaviour, IInteractable
{
    #region VARS
    private Material standardMat;

    [SerializeField] private Material hoverMat;

    [SerializeField] private Material interactMat;

    private MeshRenderer mr;


    private PlayerController heldBy;
    private Rigidbody rb;
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
    /// Changes the object's material when interacted with
    /// </summary>
    public void EnterInteract(PlayerController pc)
    {
        mr.material = interactMat;

        heldBy = pc;
        rb.isKinematic = true;
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

        transform.parent = null;
        rb.isKinematic = false;

        Debug.Log($"{gameObject.name} has ended its interaction");
    }

    private void OnCollisionStay(Collision collision)
    {
        // ship layer
        if(collision.gameObject.layer == 3)
        {
            GameObject newParent = collision.gameObject;

            // theres so many better ways to do this but this works for now
            while(!newParent.name.Contains("Ship") && newParent.transform.parent != null)
            {
                newParent = newParent.transform.parent.gameObject;
            }

            transform.parent = newParent.transform;
        }
    }
    #endregion

}
