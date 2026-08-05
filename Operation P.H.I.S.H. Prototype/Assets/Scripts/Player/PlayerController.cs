/*************************************************
Author Names : 		    Clare Grady, Cade Naylor
Date Created : 		    07/22/2026
Date Last Modified : 	07/28/202
Brief Description : 	Central script with player info for design
                        Controls movement system management

External Resources :    	
***************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class PlayerController : MonoBehaviour
{
    #region VARS

    public enum MovementType
    {
        Land,
        Water,
        Ship
    };

    [Tooltip("Default movement setting player is set to when the game starts")]
    [SerializeField] private MovementType defaultMovement;

    [SerializeField, Layer, Tooltip("The layer the water is on")] private int waterLayer;
    [SerializeField, Layer, Tooltip("The layer the phish is on")] private int shipLayer;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private Transform cameraRotationParent;
    public Camera PlayerCam { get { return playerCamera; } }
    public float CameraSensitivity { get { return cameraSensitivity; } }
    public Transform CameraRotationParent { get { return cameraRotationParent; } }

    [ReadOnly, SerializeField] private MovementType currentMovement;
    
    private Dictionary<MovementType, Movement> movementScripts;

    // in a non-prototype, these should be stored in a different script
    [SerializeField] private Image playerCrosshair;
    public Image CrosshairImage {  get { return playerCrosshair; } }
    [SerializeField] private Sprite standard;
    public Sprite StandardSprite { get { return standard; } }
    [SerializeField] private Sprite interactable;
    public Sprite InteractableSprite { get { return interactable; } }

    public Transform PickupPoint;

    private Vector3 lastCamPosition;

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
        movementScripts = new Dictionary<MovementType, Movement>() 
            {
                { MovementType.Land, GetComponent<LandMovement>() },
                { MovementType.Water, GetComponent<WaterMovement>() }
            };

        ToggleMovement(defaultMovement);
    }

    /// <summary>
    /// Subscribes to Toggle Movement Event
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.ToggleMovement += ToggleMovement;
    }

    /// <summary>
    /// Unsubscribes from toggle movement event
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.ToggleMovement -= ToggleMovement;
    }

    #region MOVEMENT

    /// <summary>
    /// Toggles movement based on event call
    /// checks for all of dictionary and turns off all other 
    /// movment scripts and activates the toggled one.
    /// </summary>
    /// <param name="type"></param>
    private void ToggleMovement(MovementType type)
    {
        currentMovement = type;
        Movement newMovement = movementScripts[0];
        foreach (var movement in movementScripts.Values)
        {
            if (movementScripts[type].Equals(movement))
            {
                movement.enabled = true;
                newMovement = movement;
                
            }
            else
            {
                if(movement.enabled)
                {
                    lastCamPosition = movement.LastCameraAngle();
                }
                movement.enabled = false;
            }
                
        }

        //newMovement.SetCameraAngle(lastCamPosition);


    }

    /// <summary>
    /// Swaps between water and land movement when entering water
    /// </summary>
    /// <param name="other"></param>

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == waterLayer)
        {
            ToggleMovement(MovementType.Water);
        }
        else
        {
            ToggleMovement(MovementType.Land);

            if(other.gameObject.layer == shipLayer)
            {
                GameObject newParent = other.gameObject;

                // theres so many better ways to do this but this works for now
                while (!newParent.name.Contains("Ship") && newParent.transform.parent != null)
                {
                    newParent = newParent.transform.parent.gameObject;
                }

                transform.parent = newParent.transform;
            }
        }
    }


    /// <summary>
    /// Swaps between water and land movement when exiting water
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == waterLayer)
        {
            ToggleMovement(MovementType.Land);
        }
        else if (other.gameObject.layer == shipLayer)
        {
            ToggleMovement(MovementType.Land);
            transform.parent = null;
        }
    }


    #endregion
    #endregion
}
