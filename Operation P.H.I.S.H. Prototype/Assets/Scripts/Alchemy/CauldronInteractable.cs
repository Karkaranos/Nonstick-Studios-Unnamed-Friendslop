using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CauldronInteractable : Singleton<CauldronInteractable>, IInteractable
{
    //replace type with ingredient ID
    private List<string> currentStoredIngredients;

    #region DesignEditsHere
    //we're hardcoding

    //private List<string> potion1 = { ingredient.ID };
    private List<string> potion2;
    private List<string> potion3;
    private List<string> potion4;
    private List<string> potion5;
    private List<string> potion6;
    private List<string> potion7;
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

    }

    public void EnterHover()
    {
        throw new System.NotImplementedException();
    }

    public void EnterInteract(PlayerController pc)
    {
        throw new System.NotImplementedException();
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
