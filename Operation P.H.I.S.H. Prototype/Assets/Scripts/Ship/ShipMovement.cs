/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/24/2026
Date Last Modified : 	7/27/2026
Brief Description : 	Moves the P.H.I.S.H.
External Resources : 	
***************************************************/

using System.Collections;
using UnityEngine;

public class ShipMovement : Singleton<ShipMovement>
{
    #region VARIABLES

    [HideInInspector] public bool Moving;

    [Tooltip("The speed of the P.H.I.S.H.")]
    [SerializeField] float forwardBackwardMovementSpeed;

    [Tooltip("The ascension speed of the P.H.I.S.H.")]
    [SerializeField] float upDownMovementSpeed;

    [Tooltip("The rotation speed of the P.H.I.S.H.")]
    [SerializeField] float leftRightMovementSpeed;

    [HideInInspector] public float FBAdjustment;
    [HideInInspector] public float ADAdjustment;
    [HideInInspector] public float WheelAdjustment;

    #endregion VARIABLES

    /// <summary>
    /// starts one of the coroutines that allows the ship to move
    /// </summary>
    /// <param name="controllerType">
    /// which movement type the player has interacted with
    /// </param>
    public void StartMoving(ControllerType controllerType)
    {
        switch (controllerType)
        {
            case ControllerType.FBLever:
                StartCoroutine(MovingForwardAndBackward());
                break;

            case ControllerType.ADLever:
                StartCoroutine(MovingUpAndDown());
                break;

            case ControllerType.Wheel:
                StartCoroutine(MovingLeftOrRight());
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// moves the ship forward or backward
    /// </summary>
    /// <returns></returns>
    IEnumerator MovingForwardAndBackward()
    {
        while(FBAdjustment != 0 || Moving)
        {
            transform.Translate(Vector3.forward * forwardBackwardMovementSpeed * FBAdjustment * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// moves the ship up or down
    /// </summary>
    /// <returns></returns>
    IEnumerator MovingUpAndDown()
    {
        while (ADAdjustment != 0 || Moving)
        {
            transform.Translate(Vector3.up * upDownMovementSpeed * ADAdjustment * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// rotates the ship
    /// </summary>
    /// <returns></returns>
    IEnumerator MovingLeftOrRight()
    {
        while (WheelAdjustment != 0 || Moving)
        {
            //i could maybe change how this is calculated based on what design says
            //it might feel wrong but maybe that's just me
            transform.Rotate(Vector3.up * leftRightMovementSpeed * WheelAdjustment * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
}
