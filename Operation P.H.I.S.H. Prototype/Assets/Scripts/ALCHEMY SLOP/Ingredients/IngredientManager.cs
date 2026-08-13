/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/13/2026
Brief Description : 	Will spawn in ingredients and maybe even do some other stuff
External Resources :    	
***************************************************/

using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class IngredientManager : MonoBehaviour
{
    //waaaait should spawning intervals should be here??
    //hmm.....

    [SerializeField] List<Ingredients> ingredients = new List<Ingredients>();

    //TODO: spawn based on timer intervals

    [Button]
    void TestSpawning()
    {
        SpawnIngredient(null, new Vector3(0, 0, 0));
    }

    void SpawnIngredient(GameObject ingredient, Vector3 pos)
    {
        if(ingredient == null)
        {
            int index = Random.Range(0, ingredients.Count);

            ingredient = ingredients[index].SpawnableIngredients
            [Random.Range(0, ingredients[index].SpawnableIngredients.Count)];

            pos = ingredients[index].SpawnLocation;
        }

        Instantiate(ingredient, pos, Quaternion.identity);

        Debug.Log($"{ingredient.name} spawned at {pos}!");
    }
}
