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

    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private Transform cameraRotationParent;
    public Camera PlayerCam { get { return playerCamera; } }
    public float CameraSensitivity { get { return cameraSensitivity; } }
    public Transform CameraRotationParent { get { return cameraRotationParent; } }

    
    private Dictionary<MovementType, Movement> movementScripts;

    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Start function
    /// Populates MovementType Movement dictionary
    /// Sets default movement script to active
    /// </summary>
    private void Start()
    {
        movementScripts = new Dictionary<MovementType, Movement>() 
            {
                { MovementType.Land, GetComponent<LandMovement>() }
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

        foreach (var movement in movementScripts.Values)
        {
            if (movementScripts[type].Equals(movement))
            {
                movement.enabled = true;
                
            }
            else
            {
                movement.enabled = false;
            }
                
        }
    }

    #endregion
    #endregion
}
