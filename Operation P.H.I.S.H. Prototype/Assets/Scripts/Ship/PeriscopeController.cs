/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    7/23/2026
Date Last Modified : 	8/7/2026
Brief Description : 	Controls switching between periscope camera + player camera
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
    [SerializeField, Tooltip("Player camera in scene, would be replaced by static ref (like GameManager) later."), Required]
    private CinemachineCamera playerCamera;
    [SerializeField, Tooltip("PeriscopeCam camera in scene under PHISH prefab, reference for periscope cinemachine camera."), Required]
    private CinemachineCamera periscopeCamera;
    [SerializeField, Tooltip("Parent of the outside periscope, named RotationParent under the ship periscope."), Required]
    private GameObject periscopeCameraRotationParent;

    [Header("Design")]
    [SerializeField, Tooltip("How quickly the periscope camera rotates.")]
    private float periscopeCameraRotationSpeed = 20;

    private bool isRotating = false;
    private Coroutine rotationCoroutine;

    /// <summary>
    /// when periscope is hovered over
    /// </summary>
    public void EnterHover()
    {
        return;
    }

    /// <summary>
    /// assigns movement and changes to periscope cam on interact
    /// </summary>
    /// <param name="pc"></param>
    public void EnterInteract(PlayerController pc)
    {
        PeriscopeChangeCamera();
        PublicEvents.MoveDirection += PeriscopeCamRotate;
        PublicEvents.MoveStopped += PeriscopeCamStopRotate;
    }

    /// <summary>
    /// when periscope is unhovered over
    /// </summary>
    public void ExitHover()
    {
        return;
    }

    /// <summary>
    /// unassigns actions and changes to player camera on "uninteract"
    /// </summary>
    public void ExitInteract()
    {
        PeriscopeChangeCamera();
        isRotating = false;
        rotationCoroutine = null;
        PublicEvents.MoveDirection -= PeriscopeCamRotate;
        PublicEvents.MoveStopped -= PeriscopeCamStopRotate;
    }

    #region CameraSwitching

    /// <summary>
    /// Switches cameras on interact pressed, will determine which camera based on which is active
    /// Does Player -> Periscope AND Periscope -> Player
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

    /// <summary>
    /// Starts periscope rotation clockwise or counterclockwise
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
    /// Stops periscope rotation on move released
    /// </summary>
    private void PeriscopeCamStopRotate()
    {
        isRotating = false;
        rotationCoroutine = null;
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
