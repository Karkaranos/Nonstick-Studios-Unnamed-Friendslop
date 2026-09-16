/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    09/10/2026
Brief Description : 	Handles quota fulfillment and end conditions
External Resources :    	
***************************************************/
using NaughtyAttributes;
using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class QuotaManager : Singleton<QuotaManager>
{
    [SerializeField] private int daysUntilQuotaCheck;
    [SerializeField] private QuotaType quotaType;

    [SerializeField, ShowIf(nameof(quotaType), QuotaType.Money)] private int targetMoney;
    [SerializeField, ShowIf(nameof(quotaType), QuotaType.Satisfaction)] private float targetSatisfaction;
    private int currentMoney;
    private int currentSatisfaction = 0;
    private int counter = 0;
    private int sumSatisfaction = 0;

    [Foldout("Canvas")][SerializeField] private GameObject quotaCanvas;
    [Foldout("Canvas")]
    [SerializeField] Color metColor;
    [Foldout("Canvas")][SerializeField] Color failColor;
    [Foldout("Canvas")][SerializeField] string metText;
    [Foldout("Canvas")][SerializeField] string failText;
    [Foldout("Canvas")][SerializeField] float displayTime;

    bool met = false;
    bool displaying = false;
    protected enum QuotaType
    {
        Satisfaction, Money
    }

    protected override void Awake()
    {
        base.Awake();
        PublicEvents.UpdateQuotaSatisfaction += GuestLeaveSatisfaction;
        PublicEvents.UpdateQuotaMoney += GuestLeaveMoney;
    }

    public void CheckDayCount(int days)
    {
        if(days != daysUntilQuotaCheck)
        {
            return;
        }

        if(quotaType == QuotaType.Satisfaction)
        {
            if(currentSatisfaction >= targetSatisfaction)
            {
                Debug.Log($"Target satisfaction met");

                if (!met)
                {
                    met = true;
                    if (!displaying)
                    {
                        StartCoroutine(DisplayBox());
                    }
                }
            }
            else
            {
                if (!displaying)
                {
                    StartCoroutine(DisplayBox());
                }
            }
        }
        else if (quotaType == QuotaType.Money)
        {
            if (currentMoney >= targetMoney)
            {
                Debug.Log($"Target money met");

                if(!met)
                {
                    met = true;
                }

                if (!displaying)
                {
                    StartCoroutine(DisplayBox());
                }
            }
            else
            {
                if(!displaying)
                {
                    StartCoroutine(DisplayBox());
                }
            }
        }
    }

    public void GuestLeaveMoney(int money)
    {
        if (quotaType != QuotaType.Money)
        {
            return;
        }


        currentMoney += money;
        Debug.Log($"Current money level: {currentMoney} after adding {money}");

        if (currentMoney >= targetMoney)
        {
            Debug.Log($"Target money met");

            if(!met)
            {
                met = true;
                if(!displaying)
                {
                    StartCoroutine(DisplayBox());
                }
            }
        }


    }

    public void GuestLeaveSatisfaction(int satisfaction)
    {
        if (quotaType != QuotaType.Satisfaction)
        {
            return;
        }

        counter++;
        sumSatisfaction += satisfaction;

        currentSatisfaction = sumSatisfaction / counter;
        Debug.Log($"Current satisfaction level: {currentSatisfaction}");

        if(currentSatisfaction >= targetSatisfaction)
        {
            Debug.Log($"Target satisfaction maintained");
        }
        else
        {
            Debug.Log("$Target satisfaction dropped");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator DisplayBox()
    {
        displaying = true;
        GameObject temp = Instantiate(quotaCanvas, DayNightManager.Instance.gameObject.GetComponent<Canvas>().transform);
        if(met)
        {
            temp.GetComponent<Image>().color = metColor;
            temp.GetComponentInChildren<TMP_Text>().text = metText;
        }
        else
        {
            temp.GetComponent<Image>().color = failColor;
            temp.GetComponentInChildren<TMP_Text>().text = failText;
        }
        yield return new WaitForSeconds(displayTime);
        Destroy(temp);
        displaying = false;

    }
}
