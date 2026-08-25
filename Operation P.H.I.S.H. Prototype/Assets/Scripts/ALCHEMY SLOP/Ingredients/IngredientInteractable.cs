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

public class IngredientInteractable : AlchemyPickupInteractable
{
    bool isPickedUp = false;

    [Tooltip ("Ingredient Name")]
    public string IngredientID = "Default";
    
    //i maybe should have accounted for this before it became a problem but whatever
    private GameObject originalParent;

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

    public override void Start()
    {
        if(isMoving)
        {
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            StartCoroutine(MoveNavMesh());
        }

        base.Start();
    }

    /// <summary>
    /// Checks if it's on a prep surface and removes it from there
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="standardInteraction"></param>
    public new void EnterInteract(AlchemyPlayerController pc, bool standardInteraction = true)
    {
        if(!standardInteraction)
        {
            return;
        }

        base.EnterInteract(pc, true);

        RaycastHit checkForPrep;

        if (Physics.Raycast(transform.position, Vector3.down, out checkForPrep, 2f, prepLayer))
        {
            checkForPrep.transform.gameObject.GetComponent<IngredientPrepInteractable>().RemoveItemFromSurface(this);
        }

        transform.parent = pc.PickupPoint;
        isPickedUp = true;

        if (isMoving)
        {
            //rb.isKinematic = true;
            navMeshAgent.enabled = false;
        }

        transformIndex = 0;
        base.EnterInteract(pc);

        //"what if the ingredient uses navmesh?" idk.......
        if(IngredientManager.Instance != null && 
        IngredientManager.Instance.ActiveIngredients.ContainsKey(this.gameObject))
        {
            IngredientManager.Instance.ActiveIngredients.Remove(this.gameObject);
        }

        Debug.Log($"GRABBED {this.name}.");
    }

    /// <summary>
    /// Adds it to a prep surface if it's on there
    /// </summary>
    public override void ExitInteract()
    {
        transform.parent = null;
        isPickedUp = false;

        if (isMoving)
        {
            //rb.isKinematic = false;
            navMeshAgent.enabled = true;
            StartCoroutine(MoveNavMesh());
        }

        RaycastHit checkForPrep;

        if(Physics.Raycast(transform.position, Vector3.down, out checkForPrep, 5f))
        {
            checkForPrep.transform.gameObject.GetComponent<IngredientPrepInteractable>()?.AddItemToSurface(this);
        }

        base.ExitInteract();
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
    public override void OnCollisionEnter(Collision collision)
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

        base.OnCollisionEnter(collision);
    }

    public override void ThrowItem(Vector3 throwVec)
    {
        disablePlayerCollision = true;

        base.ThrowItem(throwVec);
    }
}
