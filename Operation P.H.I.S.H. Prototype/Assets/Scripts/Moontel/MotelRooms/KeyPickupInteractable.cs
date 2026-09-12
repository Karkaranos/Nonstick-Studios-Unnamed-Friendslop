/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    09/11/2026
Brief Description : 	Script for interacting with room keys
External Resources :    	
***************************************************/

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KeyPickupInteractable : MoontelPickupInteractable
{
    public int KeyID;

    Transform parent;
    Vector3 rotation;

    //i agonized for awhile over getting the keys to disappear and reappear but idgaf anymore for now

    public override void Start()
    {
        base.Start();
        parent = gameObject.transform.parent;
        rotation = gameObject.transform.rotation.eulerAngles;
    }

    public override void DropItem()
    {
        base.DropItem();
        gameObject.transform.position = OriginalPosition;
        gameObject.transform.SetParent(parent, true);
        gameObject.transform.rotation = Quaternion.Euler(rotation);
    }
}
