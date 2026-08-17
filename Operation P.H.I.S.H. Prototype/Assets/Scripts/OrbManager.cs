using NaughtyAttributes;
using UnityEngine;

public class OrbManager : Singleton<OrbManager>
{
    [SerializeField] private OrbPageData[] storedInformation;

    public static OrbManager instance { get; private set; }

    [SerializeField] private int testUnlockByIndex;
    [SerializeField] private string testUnlockByTitle;

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
