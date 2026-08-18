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
using TMPro;

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
    private RecipeManager recipeMan;

    private int pageCount;
    private int currentPage = 0;

    private GameObject leftPage;
    private GameObject rightPage;

    private void Awake()
    {
        PublicEvents.ForceUpdatePage += UpdatePages;
    }

    private void OnDestroy()
    {
        PublicEvents.ForceUpdatePage -= UpdatePages;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(type == CanvasType.Orb)
        {
            orbMan = OrbManager.InstancePub;
            if(orbMan != null)
            {
                locatedManager = true;
                pageCount = (int)Mathf.Ceil(orbMan.storedInformation.Length / 2f);
            }

            leftPage = Instantiate(orbPage, pageLayoutGroup);
            rightPage = Instantiate(orbPage, pageLayoutGroup);
        }
        else
        {
            recipeMan = RecipeManager.InstancePub;
            if(recipeMan != null)
            {
                locatedManager = true;
                pageCount = (int)Mathf.Ceil(recipeMan.storedInformation.Length / 2f);
            }

            leftPage = Instantiate(recipePage, pageLayoutGroup);
            rightPage = Instantiate(recipePage, pageLayoutGroup);
        }



        UpdatePages();
    }

    void UpdatePages()
    {
        if(!locatedManager)
        {
            Debug.Log("Something went wrong");
            return;
        }

        if(type == CanvasType.Orb)
        {
            // is there a better way to do this? oh for sure
            // hpwever this works for now
            leftPage.transform.GetChild(0).GetComponent<TMP_Text>().text = (orbMan.storedInformation[currentPage*2].Known ? orbMan.storedInformation[currentPage*2].Title : "???");
            leftPage.transform.GetChild(1).GetComponent<TMP_Text>().text = (orbMan.storedInformation[currentPage*2].Known ? orbMan.storedInformation[currentPage*2].BodyText : "???");

            if(orbMan.storedInformation.Length > (currentPage*2 + 1))
            {
                rightPage.transform.GetChild(0).GetComponent<TMP_Text>().text = (orbMan.storedInformation[currentPage * 2 + 1].Known ? orbMan.storedInformation[currentPage * 2 + 1].Title : "???");
                rightPage.transform.GetChild(1).GetComponent<TMP_Text>().text = (orbMan.storedInformation[currentPage * 2 + 1].Known ? orbMan.storedInformation[currentPage * 2 + 1].BodyText : "???");
            }
            else
            {
                rightPage.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                rightPage.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
            }
        }
        else
        {
            leftPage.transform.GetChild(0).GetComponent<TMP_Text>().text = (recipeMan.storedInformation[currentPage * 2].Known ? recipeMan.storedInformation[currentPage * 2].Title : "???");
            leftPage.transform.GetChild(1).GetComponent<TMP_Text>().text = (recipeMan.storedInformation[currentPage * 2].Known ? recipeMan.storedInformation[currentPage * 2].BodyText : "???");
            leftPage.transform.GetChild(2).GetComponent<Image>().sprite = (recipeMan.storedInformation[currentPage * 2].Known ? recipeMan.storedInformation[currentPage * 2].Visual : null);

            if (recipeMan.storedInformation.Length > (currentPage * 2 + 1))
            {
                rightPage.transform.GetChild(0).GetComponent<TMP_Text>().text = (recipeMan.storedInformation[currentPage * 2 + 1].Known ? recipeMan.storedInformation[currentPage * 2 + 1].Title : "???");
                rightPage.transform.GetChild(1).GetComponent<TMP_Text>().text = (recipeMan.storedInformation[currentPage * 2 + 1].Known ? recipeMan.storedInformation[currentPage * 2 + 1].BodyText : "???");
                leftPage.transform.GetChild(2).GetComponent<Image>().enabled = true;
                rightPage.transform.GetChild(2).GetComponent<Image>().sprite = (recipeMan.storedInformation[currentPage * 2 + 1].Known ? recipeMan.storedInformation[currentPage * 2 + 1].Visual : null);
            }
            else
            {
                rightPage.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                rightPage.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                leftPage.transform.GetChild(2).GetComponent<Image>().enabled = false;
            }
        }

        nextPage.gameObject.SetActive(currentPage != pageCount-1);
        lastPage.gameObject.SetActive(currentPage != 0);


    }

    public void AdvancePage()
    {
        if(currentPage < pageCount)
        {
            currentPage++;
            UpdatePages();
        }
    }

    public void TurnBackPage()
    {
        if(currentPage > 0)
        {
            currentPage--;
            UpdatePages();
        }
    }

    public void CloseCanvas()
    {
        PublicEvents.ForceCloseCanvas?.Invoke(this.gameObject);
    }

}


