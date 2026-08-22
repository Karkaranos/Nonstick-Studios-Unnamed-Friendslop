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
using System.Collections;
using System.Runtime.CompilerServices;
using System.Linq;

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
    [SerializeField, Layer, Tooltip("The layer land is on")] private int landLayer;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private Transform cameraRotationParent;

    [SerializeField, Tooltip("How often, in seconds, the game should check for a new movement type")] private float timeBetweenMovementUpdates;
    public Camera PlayerCam { get { return playerCamera; } }
    public float CameraSensitivity { get { return cameraSensitivity; } }
    public Transform CameraRotationParent { get { return cameraRotationParent; } }

    [ReadOnly, SerializeField] private MovementType currentMovement;

    private Dictionary<MovementType, Movement> movementScripts;

    // in a non-prototype, these should be stored in a different script
    [SerializeField] private Image playerCrosshair;
    public Image CrosshairImage { get { return playerCrosshair; } }
    [SerializeField] private Sprite standard;
    public Sprite StandardSprite { get { return standard; } }
    [SerializeField] private Sprite interactable;
    public Sprite InteractableSprite { get { return interactable; } }

    public Transform PickupPoint;

    private Vector3 lastCamPosition;
    private float lineLength;

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

        StartCoroutine(MovementTypeUpdates());
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
                if (movement.enabled)
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

            if (other.gameObject.layer == shipLayer)
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
            transform.parent = null;
        }
    }

    /// <summary>
    /// Periodically check for movement type updates
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovementTypeUpdates()
    {
        MovementType lastMovement = defaultMovement;
        RaycastHit hit;

        float distanceToCheck = transform.localScale.y + .2f;
        lineLength = distanceToCheck;
        float longerDistanceToCheck = transform.localScale.y + .2f + GetComponent<LandMovement>().GetJumpHeight();

        transform.parent = FindFirstObjectByType<ShipMovement>(findObjectsInactive: FindObjectsInactive.Include).transform;

        while (true)
        {
            Physics.Raycast(transform.position, Vector3.down, out hit, distanceToCheck);

            // If there's something just under the player
            if (hit.collider != null)
            {
                lineLength = distanceToCheck;
                // When entering the ship, set the movement type to land and parent the player to the ship
                if (hit.collider.gameObject.layer == shipLayer && gameObject.transform.parent == null)
                {
                    GameObject newParent = hit.collider.gameObject;

                    // theres so many better ways to do this but this works for now
                    while (!newParent.name.Contains("Ship") && newParent.transform.parent != null)
                    {
                        newParent = newParent.transform.parent.gameObject;
                    }

                    transform.parent = newParent.transform;


                    ToggleMovement(MovementType.Land);

                    lastMovement = MovementType.Land;

                }
                //When exiting the ship onto land
                // In theory the last condition should be redundant
                else if (hit.collider.gameObject.layer == landLayer && gameObject.transform.parent != null && lastMovement == MovementType.Land)
                {
                    transform.parent = null;
                }
                else if (hit.collider.gameObject.layer == landLayer && lastMovement == MovementType.Water)
                {
                    if (transform.parent != null)
                    {
                        transform.parent = null;
                    }

                    ToggleMovement(MovementType.Land);
                    lastMovement = MovementType.Land;

                }
                else if (hit.collider.gameObject.layer == waterLayer)
                {
                    // in case the ship was just exited
                    if (transform.parent != null)
                    {
                        transform.parent = null;
                    }
                    ToggleMovement(MovementType.Water);
                    lastMovement = MovementType.Water;
                }
                // If any other layer is hit, they should be in water
                else if (hit.collider.gameObject.layer != landLayer && hit.collider.gameObject.layer != shipLayer && hit.collider.gameObject.layer != waterLayer)
                {
                    // in case the ship was just exited
                    if (transform.parent != null)
                    {
                        transform.parent = null;
                    }
                    ToggleMovement(MovementType.Water);
                    lastMovement = MovementType.Water;
                }

            }
            // no collider detected immediately under the player
            else
            {
                // Not childed to the ship, so in the ocean or on the ground
                if (transform.parent == null)
                {
                    // short raycast up
                    Physics.Raycast(transform.position, Vector3.up, out hit, 3f);
                    lineLength = 3;

                    if (hit.collider != null)
                    {

                        // If they aren't on the ship, had been on land, and there's something above them, they're probably in water
                        if (hit.collider.gameObject != null && lastMovement == MovementType.Land)
                        {
                            ToggleMovement(MovementType.Water);
                            lastMovement = MovementType.Water;
                        }
                    }
                    // idk this works
                    else
                    {
                        ToggleMovement(MovementType.Water);
                        lastMovement = MovementType.Water;
                    }
                }
                // Otherwise, they're on the ship or are leaving the ship
                else
                {
                    // Let's cast down again, but further, in case they're jumping
                    // This shouldn't cause issues if they jump off something while on land
                    Physics.Raycast(transform.position, Vector3.down, out hit, longerDistanceToCheck);
                    lineLength = longerDistanceToCheck;

                    // if it hit something
                    if (hit.collider != null)
                    {
                        // If they're jumping on the ship and they dont currently have land movement (this is a redundancy check)
                        if (hit.collider.gameObject.layer == shipLayer && lastMovement == MovementType.Water)
                        {
                            GameObject newParent = hit.collider.gameObject;

                            // theres so many better ways to do this but this works for now
                            while (!newParent.name.Contains("Ship") && newParent.transform.parent != null)
                            {
                                newParent = newParent.transform.parent.gameObject;
                            }

                            transform.parent = newParent.transform;


                            ToggleMovement(MovementType.Land);

                            lastMovement = MovementType.Land;
                        }
                        // they're jumping on the ship with correct movement
                        else if (hit.collider.gameObject.layer == shipLayer)
                        {
                            // do nothing
                        }
                        // They're leaving the ship or are jumping and on land
                        else if (hit.collider.gameObject.layer == landLayer)
                        {
                            transform.parent = null;

                            ToggleMovement(MovementType.Land);
                            lastMovement = MovementType.Land;
                        }
                        // they're leaving the ship and going into water
                        else
                        {
                            transform.parent = null;
                            ToggleMovement(MovementType.Water);
                            lastMovement = MovementType.Water;
                        }
                    }
                    else
                    {
                        transform.parent = null;
                        ToggleMovement(MovementType.Water);
                        lastMovement = MovementType.Water;
                    }
                }
            }




            yield return new WaitForSecondsRealtime(timeBetweenMovementUpdates);

        }
    }

    #endregion
    #endregion
}
