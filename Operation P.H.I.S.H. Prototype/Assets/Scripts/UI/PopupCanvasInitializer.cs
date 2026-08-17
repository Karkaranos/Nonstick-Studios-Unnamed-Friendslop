/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    08/16/2026
Brief Description : 	Sets what is currently visible on each page
                        Did I make this too complicated for what it needs to be? Probably
External Resources :    	
***************************************************/
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PopupCanvasInitializer : MonoBehaviour
{
    private enum CanvasType
    {
        Orb, Recipe_Book
    }

    [SerializeField, Tooltip("What type of canvas this is")] private CanvasType type;

    [Space(10)]
    [SerializeField, Tooltip("The page parent. Should be a Horizontal Layout Group")] private Transform pageLayoutGroup;
    [SerializeField] private Button nextPage;
    [SerializeField] private Button lastPage;
    [SerializeField, ShowIf(nameof(type), CanvasType.Orb)] private GameObject orbPage;
    [SerializeField, ShowIf(nameof(type), CanvasType.Recipe_Book)] private GameObject recipePage;

    private bool locatedManager = false;
    private OrbManager orbMan;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void UpdatePages()
    {
        if(!locatedManager)
        {
            return;
        }

    }



}


