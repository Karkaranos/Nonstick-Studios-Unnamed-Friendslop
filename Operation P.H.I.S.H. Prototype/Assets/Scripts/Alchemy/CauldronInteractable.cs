using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CauldronInteractable : Singleton<CauldronInteractable>, IInteractable
{
    //replace type with ingredient ID
    private List<string> currentStoredIngredients;

    #region DesignEditsHere
    //we're hardcoding

    private List<string> potion1; //= { ingredient.ID };
    private List<string> potion2;
    private List<string> potion3;
    private List<string> potion4;
    private List<string> potion5;
    private List<string> potion6;
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision == ingredient)
        {
            //currentStoredIngredients.Add(ingredient.ID)
        }
    }

    /// <summary>
    /// horribly inefficient way of doing it but i'm hardcoding for this prototype
    /// </summary>
    private void CompareCurrentIngredientsToPossiblePotions()
    {
        if (currentStoredIngredients == null)
        {
            Debug.Log($" No ingredients in the pot.");
            return;
        }

        //potion 1
        if (currentStoredIngredients.SequenceEqual(potion1))
        {
            PotionManager.Instance.GivePotion(1);
            ClearCurrentIngredientsList();
        }
        //potion 2
        else if (currentStoredIngredients.SequenceEqual(potion2))
        {
            PotionManager.Instance.GivePotion(2);
            ClearCurrentIngredientsList();
        }
        //potion 3
        else if (currentStoredIngredients.SequenceEqual(potion3))
        {
            PotionManager.Instance.GivePotion(3);
            ClearCurrentIngredientsList();
        }
        //potion 4
        else if (currentStoredIngredients.SequenceEqual(potion4))
        {
            PotionManager.Instance.GivePotion(4);
            ClearCurrentIngredientsList();
        }
        //potion 5
        else if (currentStoredIngredients.SequenceEqual(potion5))
        {
            PotionManager.Instance.GivePotion(5);
            ClearCurrentIngredientsList();
        }
        //potion 6
        else if (currentStoredIngredients.SequenceEqual(potion6))
        {
            PotionManager.Instance.GivePotion(6);
            ClearCurrentIngredientsList();
        }
        //nasty null (potion 7)
        else
        {
            PotionManager.Instance.GivePotion(7);
            ClearCurrentIngredientsList();
        }
    }

    private void ClearCurrentIngredientsList()
    {
        currentStoredIngredients.Clear();
    }

    public void EnterHover()
    {
        throw new System.NotImplementedException();
    }

    public void EnterInteract(PlayerController pc)
    {
        CompareCurrentIngredientsToPossiblePotions();
        Debug.Log("$ Interacted with the cauldron.");
    }

    public void ExitHover()
    {
        throw new System.NotImplementedException();
    }

    public void ExitInteract()
    {
        throw new System.NotImplementedException();
    }
}
