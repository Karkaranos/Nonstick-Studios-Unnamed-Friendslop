/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/13/2026
Brief Description : 	Stores data for ingredients/spawning
External Resources :    	
***************************************************/

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Ingredients", menuName = "Scriptable Objects/Ingredients")]
public class Ingredients : ScriptableObject
{
    [Tooltip("Where should this ingredient spawn?")]
    public List<Vector3> SpawnPoints;

    //TODO: add spawning intervals once the timer is finished!!

    [Tooltip("Which ingredient(s) can spawn here?")]
    public GameObject SpawnableIngredient;

    public Ingredients(List<Vector3> spawnPoints, GameObject spawnableIngredient)
    {
        this.SpawnPoints = spawnPoints;
        this.SpawnableIngredient = spawnableIngredient;
    }
}
