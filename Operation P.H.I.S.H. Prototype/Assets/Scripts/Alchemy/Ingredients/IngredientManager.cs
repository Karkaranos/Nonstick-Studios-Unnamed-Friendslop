/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/13/2026
Brief Description : 	Will spawn in ingredients and maybe even do some other stuff
External Resources :    	
***************************************************/

using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using System.Linq;

public class IngredientManager : Singleton<IngredientManager>
{
    public List<Ingredients> AllIngredients = new List<Ingredients>();
    [HideInInspector] public Dictionary<GameObject, Vector3> ActiveIngredients = new Dictionary<GameObject, Vector3>();

    public void SpawnIngredient(Ingredients ingredient)
    {
        Vector3 pos = Vector3.zero;

        //goes through possible spawn points and picks the first available option
        //if there are no available options, then nothing will spawn
        for(int i = 0; i < ingredient.SpawnPoints.Count; i++)
        {
            if (!ActiveIngredients.ContainsValue(ingredient.SpawnPoints[i]))
            {
                pos = ingredient.SpawnPoints[i];
                break;
            }
        }

        if(pos == Vector3.zero)
        {
            return;
        }

        GameObject spawnedIngredient = Instantiate(ingredient.SpawnableIngredient, pos, Quaternion.identity);
        ActiveIngredients.Add(spawnedIngredient, pos);

        Debug.Log($"{ingredient.name} spawned at {pos}!");
    }
}
