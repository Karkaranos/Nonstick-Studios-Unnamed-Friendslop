/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/27/2026
Date Last Modified : 	7/27/2026
Brief Description : 	Controls the movement of the P.H.I.S.H.
                        Won't need most of the functions from Movement
External Resources : 	
***************************************************/
using NaughtyAttributes;
using UnityEngine;

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
    float ADAdjustmentRate;
    [ShowIf(nameof(controllerType), ControllerType.ADLever), SerializeField]
    float ADAdjustmentCap = 0;


    [Space(10)]
    [ShowIf(nameof(controllerType), ControllerType.Wheel), SerializeField]
    float wheelAdjustmentRate = 0;

    #endregion VARIABLES

    /// <summary>
    /// runs when loaded into a scene
    /// </summary>
    void OnEnable()
    {
        PublicEvents.MoveDirection += MoveController;
        PublicEvents.EClicked += EClicked;
    }

    /// <summary>
    /// runs when this script is destroyed
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.MoveDirection -= MoveController;
        PublicEvents.EClicked -= EClicked;
    }

    [Button]
    /// <summary>
    /// should be called upon interacting with lever
    /// </summary>
    public void InteractWithController()
    {
        moving = !moving;

        if (moving)
        {
            //TODO: disable player movement
        }
        else
        {
            //TODO: enable player movement

            if(controllerType == ControllerType.Wheel)
            {
                wheelAdjustmentRate = 0;
                ShipMovement.Instance.WheelAdjustment =
                Mathf.Lerp(ShipMovement.Instance.WheelAdjustment, 0, 1.5f);

                ShipMovement.Instance.Moving = false;
            }
        }
    }

    /// <summary>
    /// runs when WASD is pressed
    /// </summary>
    /// <param name="moveVector">
    /// which key the player has used
    /// </param>
    void MoveController(Vector2 moveVector)
    {
        if(moving)
        {
            switch(controllerType)
            {
                case ControllerType.FBLever:

                    if(moveVector.y >= 0.5f && FBAdjustmentRate < FBAdjustmentCap)
                    {
                        FBAdjustmentRate += 1;

                        if(FBAdjustmentRate > 0 && !ShipMovement.Instance.Moving)
                        {
                            ShipMovement.Instance.Moving = true;
                            ShipMovement.Instance.StartMoving(controllerType);
                        }
                    }
                    else if(moveVector.y <= -0.5f && FBAdjustmentRate > -FBAdjustmentCap)
                    {
                        FBAdjustmentRate -= 1;

                        if (FBAdjustmentRate < 0 && !ShipMovement.Instance.Moving)
                        {
                            ShipMovement.Instance.Moving = true;
                            ShipMovement.Instance.StartMoving(controllerType);
                        }
                    }

                    ShipMovement.Instance.FBAdjustment =
                    Mathf.Lerp(ShipMovement.Instance.FBAdjustment, FBAdjustmentRate, 1.5f);

                    Debug.Log($"SHIP FB ADJUSTMENT: {ShipMovement.Instance.FBAdjustment}");

                    break;

                case ControllerType.ADLever:

                    if (moveVector.y >= 0.5f && ADAdjustmentRate < ADAdjustmentCap)
                    {
                        ADAdjustmentRate += 1;

                        if (ADAdjustmentRate > 0 && !ShipMovement.Instance.Moving)
                        {
                            ShipMovement.Instance.Moving = true;
                            ShipMovement.Instance.StartMoving(controllerType);
                        }
                    }
                    else if (moveVector.y <= -0.5f && ADAdjustmentRate > -ADAdjustmentCap)
                    {
                        ADAdjustmentRate -= 1;

                        if (ADAdjustmentRate < 0 && !ShipMovement.Instance.Moving)
                        {
                            ShipMovement.Instance.Moving = true;
                            ShipMovement.Instance.StartMoving(controllerType);
                        }
                    }

                    ShipMovement.Instance.ADAdjustment =
                    Mathf.Lerp(ShipMovement.Instance.ADAdjustment, ADAdjustmentRate, 1.5f);

                    Debug.Log($"SHIP AD ADJUSTMENT: {ShipMovement.Instance.ADAdjustment}");

                    break;

                case ControllerType.Wheel:

                    if(moveVector.x >= 0.5f)
                    {
                        wheelAdjustmentRate += 1;

                        if (wheelAdjustmentRate > 0 && !ShipMovement.Instance.Moving)
                        {
                            ShipMovement.Instance.Moving = true;
                            ShipMovement.Instance.StartMoving(controllerType);
                        }
                    }
                    else if(moveVector.x <= -0.5f)
                    {
                        wheelAdjustmentRate -= 1;

                        if (wheelAdjustmentRate < 0 && !ShipMovement.Instance.Moving)
                        {
                            ShipMovement.Instance.Moving = true;
                            ShipMovement.Instance.StartMoving(controllerType);
                        }
                    }

                    ShipMovement.Instance.WheelAdjustment =
                    Mathf.Lerp(ShipMovement.Instance.WheelAdjustment, wheelAdjustmentRate, 1.5f);

                    Debug.Log($"SHIP WHEEL ADJUSTMENT: {ShipMovement.Instance.WheelAdjustment}");

                    break;

                default:
                    break;

            }
        }
    }

    /// <summary>
    /// runs when E is pressed
    /// </summary>
    void EClicked()
    {
        if(moving)
        {
            InteractWithController();
        }
    }

}