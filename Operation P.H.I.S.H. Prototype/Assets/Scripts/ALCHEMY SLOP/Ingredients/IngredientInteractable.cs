/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/13/2026
Brief Description : 	Script for interacting with ingredients
                        This should be put on each ingredient prefab(?)
External Resources :    	
***************************************************/

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IngredientInteractable : MonoBehaviour, IAlchemyInteractable
{
    Rigidbody rb;
    bool isPickedUp = false;

    public string IngredientID = "Default";

    [Tooltip("Is this ingredient breakable?")]
    [SerializeField] bool isBreakable;

    [Tooltip("How fast does the ingredient need to be going in order to break?")]
    [ShowIf("isBreakable")] public float minVelocity;

    [Tooltip("Which surfaces can break this ingredient?")]
    [ShowIf("isBreakable")] public LayerMask breakableSurfaces;

    [Space(3)]

    [Tooltip("Does this ingredient move?")]
    [SerializeField] bool isMoving;

    [Tooltip("Where will this ingredient move?")]
    [ShowIf("isMoving")] public List<Transform> Transforms = new List<Transform>();

    [Layer]
    [SerializeField] private int prepLayer = 10;

    int transformIndex = 0;

    NavMeshAgent navMeshAgent;

    private bool prepped;

    public bool GetPrepState()
    {
        return prepped;
    }

    public void SetPrepState(bool b)
    {
        prepped = b;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if(isMoving)
        {
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            StartCoroutine(MoveNavMesh());
        }
    }

    /// <summary>
    /// Checks if it's on a prep surface and removes it from there
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="standardInteraction"></param>
    public void EnterInteract(AlchemyPlayerController pc, bool standardInteraction = true)
    {
        if(!standardInteraction)
        {
            return;
        }


        RaycastHit checkForPrep;

        if (Physics.Raycast(transform.position, Vector3.down, out checkForPrep, 2f, prepLayer))
        {
            checkForPrep.transform.gameObject.GetComponent<IngredientPrepInteractable>().RemoveItemFromSurface(this);
        }

        transform.parent = pc.PickupPoint;
        isPickedUp = true;

        if (isMoving)
        {
            rb.isKinematic = true;
            navMeshAgent.enabled = false;
        }

        transformIndex = 0;

        Debug.Log($"GRABBED {this.name}.");
    }

    /// <summary>
    /// Adds it to a prep surface if it's on there
    /// </summary>
    public void ExitInteract()
    {
        transform.parent = null;
        isPickedUp = false;

        if (isMoving)
        {
            rb.isKinematic = false;
            navMeshAgent.enabled = true;
            StartCoroutine(MoveNavMesh());
        }

        RaycastHit checkForPrep;

        if(Physics.Raycast(transform.position, Vector3.down, out checkForPrep, 5f))
        {
            checkForPrep.transform.gameObject.GetComponent<IngredientPrepInteractable>()?.AddItemToSurface(this);
        }


    }

    public void EnterHover()
    {

    }

    public void ExitHover()
    {
        
    }

    /// <summary>
    /// moves an ingredient between set points
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveNavMesh()
    {
        //int index = 0;

        while (!isPickedUp)
        {

            float distanceFromDestination = Vector3.Distance
            (Transforms[transformIndex].position, transform.position);

            if (distanceFromDestination <= 2)
            {
                transformIndex++;

                if (transformIndex > Transforms.Count - 1)
                {
                    transformIndex = 0;
                }
            }

            navMeshAgent.SetDestination(Transforms[transformIndex].position);

            yield return new WaitForFixedUpdate();
        }
    }

    //something about a glass ingredient??
    //remember midnight museum
    void OnCollisionEnter(Collision collision)
    {
        if (isBreakable)
        {
            //this is driving me a little crazy
            if(breakableSurfaces == (breakableSurfaces | 1 << collision.gameObject.layer))
            {
                if(rb.linearVelocity.magnitude >= minVelocity)
                {
                    //swap for broken version of ingredient later
                    Destroy(this.gameObject);
                }
                else if (collision.gameObject.GetComponent<Rigidbody>() != null && 
                collision.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude >= minVelocity)
                {
                    //swap for broken version of ingredient later
                    Destroy(this.gameObject);
                }
                //this is gross i'm sorry
                else if (transform.parent.name.Contains("Hold") &&
                transform.GetComponentInParent<AlchemyPlayerController>().
                GetComponent<Rigidbody>().linearVelocity.magnitude >= minVelocity)
                {
                    //swap for broken version of ingredient later
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
