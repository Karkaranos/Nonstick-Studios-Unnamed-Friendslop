/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/13/2026
Brief Description : 	Script for interacting with ingredients
                        This should be put on each ingredient prefab(?)
                        Thank you for IInteractable, Cade!!
External Resources :    	
***************************************************/

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IngredientInteractable : MonoBehaviour, IInteractable
{
    Rigidbody rb;
    bool isPickedUp = false;

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

    int transformIndex = 0;

    NavMeshAgent navMeshAgent;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if(isMoving)
        {
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            StartCoroutine(MoveNavMesh());
        }
    }

    //wait im blocked
    public void EnterInteract(PlayerController pc)
    {
        transform.parent = pc.PickupPoint;
        isPickedUp = true;

        transformIndex = 0;
    }

    public void ExitInteract()
    {
        isPickedUp = false;

        if(isMoving)
        {
            StartCoroutine(MoveNavMesh());
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
            if(breakableSurfaces == (breakableSurfaces | 1 << collision.gameObject.layer) &&
            rb.linearVelocity.magnitude >= minVelocity)
            {
                //swap for broken version of ingredient later
                Destroy(this);
            }
        }
    }
}
