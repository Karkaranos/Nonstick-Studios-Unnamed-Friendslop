/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    09/12/2026
Brief Description : 	For items like towels and toilet paper
External Resources :    	
***************************************************/

using Unity.VisualScripting;
using UnityEngine;

public class EventPickupInteractable : MoontelPickupInteractable
{
    [HideInInspector] public bool canBeCopied = true;

    GameObject eventItemCopy;
    Vector3 rotation;

    public override void Start()
    {
        base.Start();
        eventItemCopy = gameObject;
        rotation = gameObject.transform.rotation.eulerAngles;
    }

    public override void PickupItem(MoontelPlayerController pc)
    {
        if (canBeCopied)
        {
            GameObject copy = Instantiate(eventItemCopy, OriginalPosition, Quaternion.Euler(rotation));
            copy.GetComponent<MeshRenderer>().material = StandardMat;
            canBeCopied = false;
        }

        base.PickupItem(pc);
    }
}
