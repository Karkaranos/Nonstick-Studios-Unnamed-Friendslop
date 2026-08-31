/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    08/18/2026
Brief Description : 	Defines workspaces that can be used to prepare ingredients
External Resources :    
***************************************************/
using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class IngredientPrepInteractable : MonoBehaviour, IAlchemyInteractable
{
    // Unfortunately dictionaries cannot be serialized anymore
    [SerializeField] private Dictionary<GameObject, GameObject> BaseToPrep = new Dictionary<GameObject, GameObject>();

    // Seems like infoboxes may not work :(
    [InfoBox("Unity no longer allows for dictionary serialization, so a pair of lists is the fix. Make sure ingredients have the same index across lists", EInfoBoxType.Normal)]
    [SerializeField, Tooltip("The standard version of the ingredient")] private List<GameObject> baseIngredients = new List<GameObject>();
    [SerializeField, Tooltip("The prepped version of the ingredient")] private List<GameObject> preppedIngredients = new List<GameObject>();


    [SerializeField] private List<IngredientInteractable> itemsOnSurface = new List<IngredientInteractable>();

    public void EnterHover()
    {
        return;
    }

    /// <summary>
    /// Heheheh let the shennanigans begin!
    /// When interacted with, all ingredients on this object should prep themselves
    /// Whuch means we need to know what's on here, match it with its related prep state, and spawn a new object
    /// standard interaction (E) must be false for this to run
    /// </summary>
    /// <param name="pc"></param>
    public void EnterInteract(AlchemyPlayerController pc, bool standardInteraction)
    {
        if(standardInteraction)
        {
            return;
        }


        Debug.Log("Started ingredient prep interaction");
        List<GameObject> ingredients = new List<GameObject>(BaseToPrep.Keys);

        for(int i= itemsOnSurface.Count-1; i>=0; i--)
        {
            Debug.Log($"Surface has {itemsOnSurface[i].gameObject.name}");
            if (itemsOnSurface[i].GetPrepState())
            {
                continue;
            }

            GameObject key = null;

            foreach (GameObject k in ingredients)
            {
                if (itemsOnSurface[i].gameObject.name.Contains(k.name))
                {
                    key = k;
                }
            }

            GameObject spawnedObject = Instantiate(BaseToPrep[key], itemsOnSurface[i].transform.position, Quaternion.identity);
            spawnedObject.GetComponent<IngredientInteractable>().SetPrepState(true);

            Destroy(itemsOnSurface[i].gameObject);

            // yeah i'm assuming its spawning an ingredient
            itemsOnSurface[i] = spawnedObject.GetComponent<IngredientInteractable>();

        }

        
    }

    public void ExitHover()
    {
        // no
        return;
    }

    public void ExitInteract()
    {
        return;
    }


    public void AddItemToSurface(IngredientInteractable g)
    {
        if(itemsOnSurface.Contains(g)) return;

        itemsOnSurface.Add(g);
    }

    public void RemoveItemFromSurface(IngredientInteractable g)
    {
        if(itemsOnSurface.Contains(g))
        {
            itemsOnSurface.Remove(g);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<IngredientInteractable>())
        {
            AddItemToSurface(collision.gameObject.GetComponent<IngredientInteractable>());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<IngredientInteractable>())
        {
            RemoveItemFromSurface(collision.gameObject.GetComponent<IngredientInteractable>());
        }
    }

    /// <summary>
    /// Creates and populates a dictionary fromt he list of keys and values
    /// </summary>
    void Awake()
    {
        BaseToPrep = new Dictionary<GameObject, GameObject>();
        for (int i = 0; i != Mathf.Min(baseIngredients.Count, preppedIngredients.Count); i++)
        {
            BaseToPrep.Add(baseIngredients[i], preppedIngredients[i]);
        }

        Debug.Log($"There are {BaseToPrep.Count} kvps stored for this prep station");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropItem()
    {
        //throw new System.NotImplementedException();
    }

    public void ThrowItem(Vector3 throwVec)
    {
        //throw new System.NotImplementedException();
    }
}
