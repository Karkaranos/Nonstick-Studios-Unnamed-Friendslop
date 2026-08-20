/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    08/16/2026
Brief Description : 	Stores data for the translating orb
                        Did I make this too complicated for what it needs to be? Probably
External Resources :    	
***************************************************/
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class OrbManager : Singleton<OrbManager>
{
    [SerializeField, Tooltip("In a non-prototype we could probably get this to autopopulate")] public OrbPageData[] storedInformation;
    [SerializeField] private Translation[] translations;

    public static OrbManager InstancePub;

    public static Dictionary<string, string> RosettaStone = new Dictionary<string, string>();

    [SerializeField] private int testUnlockByIndex;
    [SerializeField] private string testUnlockByTitle;

    protected override void Awake()
    {
        if (InstancePub == null)
        {
            InstancePub = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        // idk if this'll be useful in the prototype but eh
        foreach(var entry in translations)
        {
            RosettaStone.Add(entry.Term, entry.Definition);
        }
    }


    /// <summary>
    /// Unlocks page based on the page index
    /// </summary>
    /// <param name="i"></param>
    public void UnlockPage(int i)
    {
        if(i < storedInformation.Length)
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
            if(storedInformation[i].Title == s)
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
public struct OrbPageData
{
    public bool Known;
    public string Title;
    public string BodyText;

}

[System.Serializable]
public struct Translation
{
    public string Term;
    public PartOfWord part;
    public string Definition;
}

public enum PartOfWord
{
    Prefix, Postfix, Word
}
