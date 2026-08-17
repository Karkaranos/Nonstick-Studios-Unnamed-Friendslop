using System.Collections.Generic;
using UnityEngine;

public class PotionManager : Singleton<PotionManager>
{
    private static GameObject potion1;
    private static GameObject potion2;
    private static GameObject potion3;
    private static GameObject potion4;
    private static GameObject potion5;
    private static GameObject potion6;
    private static GameObject potion7;
    
    private List<GameObject> possiblePotions = new List<GameObject> { potion1, potion2, potion3, potion4, potion5, potion6, potion7 };
    public void GivePotion(int potion)
    {
        Instantiate(possiblePotions[potion - 1]);
        Debug.Log($"Give {potion}");
    }
}
