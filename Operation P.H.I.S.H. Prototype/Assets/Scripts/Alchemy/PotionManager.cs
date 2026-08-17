using System.Collections.Generic;
using UnityEngine;

public class PotionManager : Singleton<PotionManager>
{
    [SerializeField] private GameObject potion1;
    [SerializeField] private GameObject potion2;
    [SerializeField] private GameObject potion3;
    [SerializeField] private GameObject potion4;
    [SerializeField] private GameObject potion5;
    [SerializeField] private GameObject potion6;
    [SerializeField] private GameObject potion7;

    private List<GameObject> possiblePotions;

    private void Start()
    {
        possiblePotions = new List<GameObject> {potion1, potion2, potion3, potion4, potion5, potion6, potion7 };
    }

    public void GivePotion(int potion)
    {
        if (possiblePotions[potion - 1] == null)
        {
            Debug.Log($" {potion} is null.");
        }

        Instantiate(possiblePotions[potion - 1]);
        Debug.Log($"Give {potion}");
    }
}

