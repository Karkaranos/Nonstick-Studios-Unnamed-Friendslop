/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    7/23/2026
Date Last Modified : 	7/23/2026
Brief Description : 	Controls the switch to the periscope camera
                        and controls periscope movement
External Resources : 	
***************************************************/
using NaughtyAttributes;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PeriscopeController : MonoBehaviour
{
    [Header ("References")]
    [SerializeField, Tooltip("In scene as TempPlayerCam, will be replaced by static ref later."), Required]
    private CinemachineCamera playerCamera;
    [SerializeField, Tooltip("PeriscopeCam prefab, reference for cinemachine camera."), Required]
    private CinemachineCamera periscopeCamera;
    [SerializeField, Tooltip("PeriscopeCam prefab, reference for cinemachine pan tilt."), Required]
    private CinemachinePanTilt periscopePanTilt;

    [Header("Design")]
    [SerializeField, Tooltip("How quickly the periscope camera rotates.")]
    private float periscopeCameraRotationSpeed = 15;

    private Coroutine periscopeRotationCoroutine;
    private bool isRotating = false;

    #region CameraSwitching

    [Button]
    /// <summary>
    /// Switches from Player Camera to Periscope Camera
    /// this would eventually take in player camera from a static ref in playermanager
    /// </summary>
    private void ChangeToPeriscopeCamera()
    {
        playerCamera.gameObject.SetActive(false);
        periscopeCamera.gameObject.SetActive(true);
    }


    [Button]
    /// <summary>
    /// Switches from Periscope Camera to Player Camera
    /// this would eventually take in player camera from a static ref in playermanager
    /// </summary>
    private void ChangeToPlayerCamera()
    {
        periscopeCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        //stop rotation coroutine if still going
        isRotating = false;

        if (periscopeRotationCoroutine != null)
        {
            StopCoroutine(periscopeRotationCoroutine);
            periscopeRotationCoroutine = null;
        }
    }

    #endregion

    #region CameraRotation

    [Button]
    /// <summary>
    /// Starts or stops periscope rotation clockwise
    /// </summary>
    private void PeriscopeCamRotationClockwise()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        //if not rotating, rotate clockwise
        if (!isRotating)
        {
            isRotating = true;

            if (periscopeRotationCoroutine == null)
            {
                periscopeRotationCoroutine = StartCoroutine(RotatePeriscope(1));
            }

        }
        //if rotating, stop rotating
        else
        {
            isRotating = false;

            if (periscopeRotationCoroutine != null)
            {
                StopCoroutine(periscopeRotationCoroutine);
                periscopeRotationCoroutine = null;
            }
        }
    }



    [Button]
    /// <summary>
    /// Starts or stops periscope rotation counterclockwise
    /// </summary>
    private void PeriscopeCamRotationCounterClockwise()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        //if not rotating, rotate counterclockwise
        if (!isRotating)
        {
            isRotating = true;

            if (periscopeRotationCoroutine == null)
            {
                periscopeRotationCoroutine = StartCoroutine(RotatePeriscope(-1));
            }
        }
        //if rotating, stop rotating
        else
        {
            isRotating = false;

            if (periscopeRotationCoroutine != null)
            {
                StopCoroutine(periscopeRotationCoroutine);
                periscopeRotationCoroutine = null;
            }
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
            periscopePanTilt.PanAxis.Value += direction * periscopeCameraRotationSpeed * Time.deltaTime;
            yield return null;
        }
    }

    #endregion 
}
