/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    08/16/2026
Brief Description : 	Stores data for the translating orb
                        Did I make this too complicated for what it needs to be? Probably
External Resources :    	
***************************************************/
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class RecipeManager : Singleton<RecipeManager>
{
    [SerializeField] public RecipePageData[] storedInformation;

    public static RecipeManager InstancePub;


    [SerializeField] private int testUnlockByIndex;
    [SerializeField] private string testUnlockByTitle;


    protected override void Awake()
    {
        if(InstancePub == null)
        {
            InstancePub = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    /// <summary>
    /// Unlocks page based on the page index
    /// </summary>
    /// <param name="i"></param>
    public void UnlockPage(int i)
    {
        if (i < storedInformation.Length)
        {
            storedInformation[i].Known = true;
            PublicEvents.ForceUpdatePage?.Invoke();
        }
        else
        {
            Debug.LogWarning($"The orb entry at {i} could not be accessed");
        }
    }

    /// <summary>
    /// Unlocks page based on the page title
    /// </summary>
    /// <param name="s"></param>
    public void UnlockPage(string s)
    {
        bool foundMatch = false;
        // didnt let me do a foreach ebcause it was 'modifying the indexed variable or smth :(
        for (int i = 0; i < storedInformation.Length; i++)
        {
            if (storedInformation[i].Title == s)
            {
                storedInformation[i].Known = true;
                foundMatch = true;
                PublicEvents.ForceUpdatePage?.Invoke();
            }
        }
        if (!foundMatch)
        {
            Debug.LogWarning($"Orb entry titled {s} could not be found");
        }
    }


    [Button]
    public void TestUnlockByIndex()
    {
        UnlockPage(testUnlockByIndex);
    }

    [Button]
    public void TestUnlockByTitle()
    {
        UnlockPage(testUnlockByTitle);
    }
}

[System.Serializable]
public struct RecipePageData
{
    public bool Known;
    public string Title;
    public string BodyText;

}
