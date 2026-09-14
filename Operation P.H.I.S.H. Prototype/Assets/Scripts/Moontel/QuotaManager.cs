/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    09/10/2026
Brief Description : 	Handles quota fulfillment and end conditions
External Resources :    	
***************************************************/
using NaughtyAttributes;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class QuotaManager : Singleton<QuotaManager>
{
    [SerializeField] private int daysUntilQuotaCheck;
    [SerializeField] private QuotaType quotaType;

    [SerializeField, ShowIf(nameof(quotaType), QuotaType.Money)] private int targetMoney;
    [SerializeField, ShowIf(nameof(quotaType), QuotaType.Satisfaction)] private float targetSatisfaction;
    private int currentMoney;
    private int currentSatisfaction = 100;
    private int counter;
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
    }

    public void GuestLeaveMoney(int money)
    {
        if (quotaType != QuotaType.Money)
        {
            return;
        }

        currentMoney += money;
        Debug.Log($"Current money level: {currentSatisfaction}");

        if (currentMoney >= targetMoney)
        {
            Debug.Log($"Target money met");
        }


    }

    public void GuestLeaveSatisfaction(int satisfaction)
    {
        if (quotaType != QuotaType.Satisfaction)
        {
            return;
        }

        counter++;
        //New average = old average * (n - 1) / n + new value / n
        currentSatisfaction = Mathf.RoundToInt(currentSatisfaction * (counter - 1) / (float)counter + satisfaction / (float)counter);
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
}
