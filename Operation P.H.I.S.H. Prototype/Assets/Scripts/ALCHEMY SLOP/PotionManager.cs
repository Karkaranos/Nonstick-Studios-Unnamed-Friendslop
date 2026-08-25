/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    08/17/2026
Brief Description : 	Manages potion creation, should act as the reference for potions
External Resources :    	
***************************************************/
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : Singleton<PotionManager>
{
    [Header ("Potions")]
    [SerializeField, Tooltip("Health Potion")] private GameObject potion1;
    [SerializeField, Tooltip("Strength Potion")] private GameObject potion2;
    [SerializeField, Tooltip("Endurance Potion")] private GameObject potion3;
    [SerializeField, Tooltip("Luck Potion")] private GameObject potion4;
    [SerializeField, Tooltip("Gentle Repose Potion")] private GameObject potion5;
    [SerializeField, Tooltip("Hydration Potion")] private GameObject potion6;
    [SerializeField, Tooltip("Nasty Null Potion")] private GameObject potion7;

    [Header("Potion Spawn Point")]
    [SerializeField, Tooltip("Where the potion should spawn on cauldron stir.")] private Transform potionSpawnPoint;

    private List<GameObject> possiblePotions;

    private void Start()
    {
        possiblePotions = new List<GameObject> {potion1, potion2, potion3, potion4, potion5, potion6, potion7};
    }

    /// <summary>
    /// Spawns in potion based on current ingredients in the cauldron
    /// </summary>
    /// <param name="potion"></param>
    /// number corresponding to which potion should be given (look at potion tooltips)
    public void GivePotion(int potion)
    {
        if (possiblePotions[potion - 1] == null)
        {
            Debug.Log($" Potion {potion} is null.");
        }

        Instantiate(possiblePotions[potion - 1], potionSpawnPoint);
        Debug.Log($"Give potion {potion}");
    }
}

