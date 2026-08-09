/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    07/30/2026
Date Last Modified : 	07/30/2026
Brief Description : 	A test object for pick up interactions
External Resources :    	
***************************************************/
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
public class PickupInteractable : MonoBehaviour, IInteractable
{
    #region VARS
    private Material standardMat;

    [SerializeField] private Material hoverMat;

    [SerializeField] private Material interactMat;

    private MeshRenderer mr;

    [SerializeField, Layer] private int shipLayer;
    private PlayerController heldBy;
    private Rigidbody rb;

    [HideInInspector] public Vector3 OriginalPos;
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

        position = gameObject.transform.position;
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
    #endregion

}
