/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    09/10/2026
Brief Description : 	Handles quota fulfillment and end conditions
External Resources :    	
***************************************************/
using UnityEngine;

public class QuotaManager : Singleton<QuotaManager>
{
    [SerializeField] private int daysUntilQuotaCheck;

    protected enum QuotaType
    {
        Satisfaction, Money
    }

    public void CheckDayCount(int days)
    {
        if(days != daysUntilQuotaCheck)
        {
            return;
        }
    }

    public void CustomerLeft(int money)
    {

    }

    public void CustomerLeft(float satisfaction)
    {

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
