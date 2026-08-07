/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    7/23/2026
Date Last Modified : 	8/7/2026
Brief Description : 	Controls the switch to the periscope camera
                        and controls periscope movement
External Resources : 	
***************************************************/
using NaughtyAttributes;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PeriscopeController : MonoBehaviour, IInteractable
{
    [Header ("References")]
    [SerializeField, Tooltip("In scene as TempPlayerCam, will be replaced by static ref later."), Required]
    private CinemachineCamera playerCamera;
    [SerializeField, Tooltip("PeriscopeCam prefab, reference for cinemachine camera."), Required]
    private CinemachineCamera periscopeCamera;
    [SerializeField, Tooltip("Parent of the outside periscope, named RotationParent under the ship periscope."), Required]
    private GameObject periscopeCameraRotationParent;


    [Header("Design")]
    [SerializeField, Tooltip("How quickly the periscope camera rotates.")]
    private float periscopeCameraRotationSpeed = 15;
    private bool isRotating = false;
    private Coroutine rotationCoroutine;

    public void OnEnable()
    {
        PublicEvents.MoveDirection += PeriscopeCamRotate;
    }
    public void OnDisable()
    {
        PublicEvents.MoveDirection -= PeriscopeCamRotate;
    }


    public void EnterHover()
    {
        return;
    }

    public void EnterInteract(PlayerController pc)
    {
        PeriscopeChangeCamera();
    }

    public void ExitHover()
    {
        return;
    }

    public void ExitInteract()
    {
        PeriscopeChangeCamera();
        isRotating = false;
        rotationCoroutine = null;
    }

    #region CameraSwitching
    /// <summary>
    /// Switches cameras on interact pressed, will determine which camera based on which is active
    /// Does Player -> Periscope and Periscope -> Player
    /// </summary>
    private void PeriscopeChangeCamera()
    {
        //change to periscope
        if (!periscopeCamera.gameObject.activeSelf)
        {
            playerCamera.gameObject.SetActive(false);
            periscopeCamera.gameObject.SetActive(true);
        }
        //change to player
        else
        {
            periscopeCamera.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
        }
    }

    #endregion

    #region CameraRotation

    [Button]
    /// <summary>
    /// Starts or stops periscope rotation clockwise
    /// </summary>
    private void PeriscopeCamRotate(Vector2 moveVector)
    {
        if (!gameObject.activeSelf || rotationCoroutine != null)
        {
            return;
        }

        isRotating = true;

        if (moveVector.x < 0)
        {
            rotationCoroutine = StartCoroutine(RotatePeriscope(-1));
        }
        else if (moveVector.x > 0)
        {
            rotationCoroutine = StartCoroutine(RotatePeriscope(1));
        }
    }

    /// <summary>
    /// Rotates the periscope either clockwise or counterclockwise
    /// </summary>
    /// <param name="direction"></param>
    /// determines rotation direction, use 1 for clockwise and -1 for counterclockwise
    /// <returns></returns>
    private IEnumerator RotatePeriscope(int direction)
    {
        while (isRotating)
        {
            periscopeCameraRotationParent.transform.Rotate(new Vector3(0, direction * periscopeCameraRotationSpeed, 0) * Time.deltaTime);
            yield return null;
        }

        rotationCoroutine = null;
    }

    #endregion 
}
