using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CauldronInteractable : Singleton<CauldronInteractable>, IAlchemyInteractable
{
    //replace type with ingredient ID
    private List<Tuple<string, bool>> currentStoredIngredients = new();

    #region DesignEditsHere
    //we're hardcoding
    
    //health: raw carrot, raw carrot, raw bulbshroom
    private List<Tuple<string, bool>> potion1 = new List<Tuple<string, bool>> {Tuple.Create("Awakened Carrot", false), Tuple.Create("Awakened Carrot", false), Tuple.Create("Bulbshroom", false)};

    //strength: chopped fragilefruit, chopped carrot, chopped carrot
    private List<Tuple<string, bool>> potion2 = new List<Tuple<string, bool>> {Tuple.Create("Glassfruit", true), Tuple.Create("Awakened Carrot", true), Tuple.Create("Awakened Carrot", true)};

    //endurance: raw carrot, raw snoozeberry, chopped snoozeberry
    private List<Tuple<string, bool>> potion3 = new List<Tuple<string, bool>> {Tuple.Create("Awakened Carrot", false), Tuple.Create("Snoozeberry", false), Tuple.Create("Snoozeberry", true)};

    //luck: chopped bulbshroom, chopped snoozeberry, raw glassfruit
    private List<Tuple<string, bool>> potion4 = new List<Tuple<string, bool>> {Tuple.Create("Bulbshroom", true), Tuple.Create("Snoozeberry", true), Tuple.Create("Glassfruit", false)};

    //gentle repose: raw snoozeberry, chopped carrot, chopped snoozeberry
    private List<Tuple<string, bool>> potion5 = new List<Tuple<string, bool>> {Tuple.Create("Snoozeberry", false), Tuple.Create("Awakened Carrot", true), Tuple.Create("Snoozeberry", true)};

    //hydration: raw snoozeberry, chopped carrot, chopped bulbfruit
    private List<Tuple<string, bool>> potion6 = new List<Tuple<string, bool>> { Tuple.Create("Snoozeberry", false), Tuple.Create("Awakened Carrot", true), Tuple.Create("Bulbshroom", true)};

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IngredientInteractable>())
        {
            currentStoredIngredients.Add(Tuple.Create(collision.gameObject.GetComponent<IngredientInteractable>().IngredientID, collision.gameObject.GetComponent<IngredientInteractable>().GetPrepState()));
            Debug.Log($"Added {collision.gameObject} to the list. ID: {collision.gameObject.GetComponent<IngredientInteractable>().IngredientID}, Prepped : {collision.gameObject.GetComponent<IngredientInteractable>().GetPrepState()}");
            collision.gameObject.GetComponent<IngredientInteractable>().ExitInteract();
            collision.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// horribly inefficient way of doing it but i'm hardcoding for this prototype
    /// </summary>
    private void CompareCurrentIngredientsToPossiblePotions()
    {
        if (currentStoredIngredients.Count() <= 0)
        {
            Debug.Log($" No ingredients in the pot.");
            return;
        }

        //potion 1
        if (currentStoredIngredients.SequenceEqual(potion1))
        {
            PotionManager.Instance.GivePotion(1);
            ClearCurrentIngredientsList();
            Debug.Log($"Found potion 1 : {potion1}.");
        }
        //potion 2
        else if (currentStoredIngredients.SequenceEqual(potion2))
        {
            PotionManager.Instance.GivePotion(2);
            ClearCurrentIngredientsList();
            Debug.Log($"Found potion 2 : {potion2}.");
        }
        //potion 3
        else if (currentStoredIngredients.SequenceEqual(potion3))
        {
            PotionManager.Instance.GivePotion(3);
            ClearCurrentIngredientsList();
            Debug.Log($"Found potion 3 : {potion3}.");
        }
        //potion 4
        else if (currentStoredIngredients.SequenceEqual(potion4))
        {
            PotionManager.Instance.GivePotion(4);
            ClearCurrentIngredientsList();
            Debug.Log($"Found potion 4 : {potion4}.");
        }
        //potion 5
        else if (currentStoredIngredients.SequenceEqual(potion5))
        {
            PotionManager.Instance.GivePotion(5);
            ClearCurrentIngredientsList();
            Debug.Log($"Found potion 5 : {potion5}.");
        }
        //potion 6
        else if (currentStoredIngredients.SequenceEqual(potion6))
        {
            PotionManager.Instance.GivePotion(6);
            ClearCurrentIngredientsList();
            Debug.Log($"Found potion 6 : {potion6}.");
        }
        //nasty null (potion 7)
        else
        {
            PotionManager.Instance.GivePotion(7);
            ClearCurrentIngredientsList();
            Debug.Log($"Found potion 7 : Nasty Null.");
        }
    }

    private void ClearCurrentIngredientsList()
    {
        currentStoredIngredients.Clear();
        Debug.Log($"Current ingredients cleared, List check : {currentStoredIngredients}");
    }

    public void EnterHover()
    {
    }

    public void ExitHover()
    {
    }

    public void EnterInteract(AlchemyPlayerController pc, bool standardInteraction = true)
    {
        CompareCurrentIngredientsToPossiblePotions();
        Debug.Log("$ Interacted with the cauldron.");
    }

    public void ExitInteract()
    {
    }
}
