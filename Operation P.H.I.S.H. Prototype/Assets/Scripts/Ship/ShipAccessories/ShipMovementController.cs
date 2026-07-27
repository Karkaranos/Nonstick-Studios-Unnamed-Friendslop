/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/27/2026
Date Last Modified : 	7/27/2026
Brief Description : 	Controls the movement of the P.H.I.S.H.
External Resources : 	
***************************************************/
using System.Collections;
using System.Threading;
using NaughtyAttributes;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static ShipMovementControllers;

public enum ControllerType
{
    FBLever,
    ADLever,
    Wheel,
}

public class ShipMovementControllers : MonoBehaviour
{
    #region VARIABLES

    bool moving;

    [Tooltip("Match this variable with the game object!")]
    [SerializeField] ControllerType controllerType;


    [Space(10)]
    [ShowIf(nameof(controllerType), ControllerType.FBLever), SerializeField]
    float FBAdjustmentRate = 0;
    [ShowIf(nameof(controllerType), ControllerType.FBLever), SerializeField]
    float FBAdjustmentCap;


    [Space(10)]
    [ShowIf(nameof(controllerType), ControllerType.ADLever), SerializeField]
    float ADAdjustmentCap = 0;
    [ShowIf(nameof(controllerType), ControllerType.ADLever), SerializeField]
    float ADAdjustmentRate;


    [Space(10)]
    [ShowIf(nameof(controllerType), ControllerType.Wheel), SerializeField]
    float wheelAdjustmentRate = 0;

    #endregion VARIABLES

    /// <summary>
    /// should be called upon interacting with lever
    /// </summary>
    public void InteractWithController()
    {
        ShipMovement.Instance.Moving = !moving;
        moving = !moving;

        if (moving)
        {
            //TODO: disable player movement
        }
        else
        {
            //TODO: enable player movement
        }
    }

    //TODO: put controls here!!!!!
    //tl;dr: when the player uses WASD while moving,
    //it should adjust whichever value is associated with controllerType
    //then, it should gradually adjust the associated variable in ShipMovement using mathf.lerp(?)
    //to keep the coroutine in ShipMovement from running constantly,
    //it should start whenever the variable in this script is less than or greater than 0,
    //and stop whenever the variable in ShipMovement is 0
}