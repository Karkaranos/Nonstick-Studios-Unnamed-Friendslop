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
    [SerializeField]
    private CinemachineCamera playerCamera;
    [SerializeField]
    private CinemachineCamera periscopeCamera;
    [SerializeField]
    private CinemachinePanTilt periscopePanTilt;
    private Coroutine periscopeRotationCoroutine;
    private bool isRotating = false;
    [SerializeField]
    private float periscopeCameraRotationSpeed = 1;

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
    }

    [Button]
    /// <summary>
    /// Starts or stops periscope rotation clockwise
    /// </summary>
    private void PeriscopeCamRotationClockwise()
    {
        //if not rotating, rotate clockwise
        if (!isRotating)
        {
            if (periscopeRotationCoroutine == null)
            {
                periscopeRotationCoroutine = StartCoroutine(RotatePeriscope(1));
            }

            isRotating = true;
        }
        //if rotating, stop rotating
        else
        {
            if (periscopeRotationCoroutine != null)
            {
                StopCoroutine(periscopeRotationCoroutine);
                periscopeRotationCoroutine = null;
            }

            isRotating = false;
        }
    }



    [Button]
    /// <summary>
    /// Starts or stops periscope rotation counterclockwise
    /// </summary>
    private void PeriscopeCamRotationCounterClockwise()
    {
        //if not rotating, rotate counterclockwise
        if (!isRotating)
        {
            if (periscopeRotationCoroutine == null)
            {
                periscopeRotationCoroutine = StartCoroutine(RotatePeriscope(-1));
            }

            isRotating = true;
        }
        //if rotating, stop rotating
        else
        {
            if (periscopeRotationCoroutine != null)
            {
                StopCoroutine(periscopeRotationCoroutine);
                periscopeRotationCoroutine = null;
            }

            isRotating = false;
        }
    }

    /// <summary>
    /// Rotates the periscope either clockwise or counterclockwise
    /// </summary>
    /// <param name="direction"></param>
    /// use 1 for clockwise and -1 for counterclockwise
    /// <returns></returns>
    private IEnumerator RotatePeriscope(int direction)
    {
        while (true)
        {
            periscopePanTilt.PanAxis.Value += direction * periscopeCameraRotationSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
