/*************************************************
Author Names : 		    Jacob Bateman, Clare Grady, Cade Naylor
Date Created : 		    08/12/2026
Date Last Modified : 	08/12/202
Brief Description : 	Central script with player info for design
                        Controls movement system management
Jacob Note:             Taken from PHISH and edited for AS.

External Resources :    	
***************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Collections;

public class AlchemyPlayerController : MonoBehaviour
{
    #region VARS

    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private Transform cameraRotationParent;

    public Camera PlayerCam { get { return playerCamera; } }
    public float CameraSensitivity { get { return cameraSensitivity; } }
    public Transform CameraRotationParent { get { return cameraRotationParent; } }


    // in a non-prototype, these should be stored in a different script
    [SerializeField] private Image playerCrosshair;
    public Image CrosshairImage { get { return playerCrosshair; } }
    [SerializeField] private Sprite standard;
    public Sprite StandardSprite { get { return standard; } }
    [SerializeField] private Sprite interactable;
    public Sprite InteractableSprite { get { return interactable; } }

    public Transform PickupPoint;

    public AlchemyPickupInteractable heldInteractable { get; private set; }
    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Start function
    /// Populates MovementType Movement dictionary
    /// Sets default movement script to active
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Sets held item. Rest of pickup logic is handled in the PickupInteractable script.
    /// </summary>
    /// <param name="pickup"></param>
    public void SetPickupItem(AlchemyPickupInteractable pickup, bool dropHeldItem = true)
    {
        Debug.Log($"{gameObject.name} is now holding {(pickup == null ? "nothing" : pickup.gameObject.name)}");

        // if already holding something
        if (heldInteractable != null && dropHeldItem)
        {
            heldInteractable.DropItem();
        }

        heldInteractable = pickup;
    }

    #endregion
}
