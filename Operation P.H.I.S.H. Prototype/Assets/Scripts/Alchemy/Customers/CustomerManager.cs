/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    08/18/2026
Brief Description : 	Spawns customers in
External Resources :    	
***************************************************/

using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : Singleton<CustomerManager>
{
    [Tooltip("What customers can spawn in the scene?")]
    [SerializeField] List<GameObject> customers;

    [Tooltip("Where should the customer spawn?")]
    [SerializeField] Vector3 spawnPoint;

    [Tooltip("Where should the cusomter be facing in the scene?")]
    [SerializeField] float spawnRotation;

    [Space(5)]

    [Tooltip("How long should the customers' dialogue be displayed?")]
    public float DialogueDisplayTime;

    [Tooltip("How long until the next customer spawns in?")]
    public float Cooldown;

    private void Start()
    {
        SpawnNewCustomer();
    }

    public void SpawnNewCustomer()
    {
        GameObject newCustomer = Instantiate(customers[Random.Range(0, customers.Count)], spawnPoint,
        Quaternion.Euler(0, spawnRotation, 0));
    }
}
