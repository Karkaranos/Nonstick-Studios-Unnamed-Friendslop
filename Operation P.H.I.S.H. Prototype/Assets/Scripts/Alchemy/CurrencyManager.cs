/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/18/2026
Brief Description : 	Tracks the players' currency
External Resources :    	
***************************************************/

using UnityEngine;

public class CurrencyManager : Singleton<CurrencyManager>
{
    float amountOfMoney;

    [Tooltip("How much do the players start out with?")]
    [SerializeField] float startingAmountOfMoney;

    private void Start()
    {
        amountOfMoney = startingAmountOfMoney;
    }

    public void AddMoney(float amountPaid)
    {
        amountOfMoney += amountPaid;
        Debug.Log($"$$$: {amountOfMoney}");
    }
}
