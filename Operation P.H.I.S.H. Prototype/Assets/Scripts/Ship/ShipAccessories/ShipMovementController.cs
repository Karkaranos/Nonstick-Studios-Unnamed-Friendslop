/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/27/2026
Date Last Modified : 	7/27/2026
Brief Description : 	Controls the movement of the P.H.I.S.H.
                        Won't need most of the functions from Movement
                        TODO: With networking, make sure that multiple players cannot interact with one controller
External Resources : 	
***************************************************/
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public enum ControllerType
{
    FBLever,
    ADLever,
    Wheel,
}

public class ShipMovementControllers : MonoBehaviour, IInteractable
{
    #region VARIABLES

    bool moving;
    bool canMove = true;

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
        PublicEvents.HaltShipMovement += ResetSpeed;
    }

    /// <summary>
    /// runs when this script is destroyed
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.MoveDirection -= MoveController;
        PublicEvents.HaltShipMovement -= ResetSpeed;
    }

    [Button]
    /// <summary>
    /// called when the player interacts with a controller
    /// </summary>
    public void EnterInteract(PlayerController pc)
    {
        moving = true;
        Debug.Log($"{controllerType} ENABLED.");
    }

    /// <summary>
    /// called when the player interacts with a controller in use
    /// </summary>
    public void ExitInteract()
    {
        if (controllerType == ControllerType.Wheel)
        {
            wheelAdjustmentRate = 0;
            StartCoroutine(AdjustShipSpeed());
        }

        ShipMovement.Instance.Moving = false;
        Debug.Log($"{controllerType} DISABLED.");
    }

    /// <summary>
    /// runs when WASD is pressed
    /// </summary>
    /// <param name="moveVector">
    /// which key the player has used
    /// </param>
    void MoveController(Vector2 moveVector)
    {
        if(moving && canMove)
        {
            canMove = false;

            switch (controllerType)
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

                    StartCoroutine(AdjustShipSpeed());

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

                    StartCoroutine(AdjustShipSpeed());

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

                    StartCoroutine(AdjustShipSpeed());

                    break;

                default:
                    break;

            }
        }
    }

    /// <summary>
    /// gradually adjusts the ship's speed for real this time
    /// </summary>
    /// <returns></returns>
    IEnumerator AdjustShipSpeed()
    {
        float timer = 0;

        //sorry switch statements are fun for me
        switch(controllerType)
        {
            case (ControllerType.FBLever):

                while(timer < 2)
                {
                    timer += 1;

                    ShipMovement.Instance.FBAdjustment =
                    Mathf.Lerp(ShipMovement.Instance.FBAdjustment, FBAdjustmentRate, Mathf.Clamp01(timer/2));

                    yield return new WaitForSeconds(1);

                    canMove = true;
                }

                Debug.Log($"SHIP FB ADJUSTMENT: {ShipMovement.Instance.FBAdjustment}");
                break;

            case (ControllerType.ADLever):

                while (timer < 2)
                {
                    timer += 1;

                    ShipMovement.Instance.ADAdjustment =
                    Mathf.Lerp(ShipMovement.Instance.ADAdjustment, ADAdjustmentRate, Mathf.Clamp01(timer / 2));

                    yield return new WaitForSeconds(1);

                    canMove = true;
                }

                Debug.Log($"SHIP AD ADJUSTMENT: {ShipMovement.Instance.ADAdjustment}");
                break;

            case (ControllerType.Wheel):

                while (timer < 2)
                {
                    timer += 1;

                    ShipMovement.Instance.WheelAdjustment =
                    Mathf.Lerp(ShipMovement.Instance.WheelAdjustment, wheelAdjustmentRate, Mathf.Clamp01(timer / 2));

                    yield return new WaitForSeconds(1);

                    canMove = true;
                }

                Debug.Log($"SHIP WHEEL ADJUSTMENT: {ShipMovement.Instance.WheelAdjustment}");
                break;

            default:
                break;
        }
    }

    void ResetSpeed()
    {
        moving = false;
        ShipMovement.Instance.Moving = false;

        switch(controllerType)
        {
            case ControllerType.FBLever:

                FBAdjustmentRate = 0;
                ShipMovement.Instance.FBAdjustment = 0;
                break;

            case ControllerType.ADLever:

                ADAdjustmentRate = 0;
                ShipMovement.Instance.ADAdjustment = 0;
                break;

            case ControllerType.Wheel:

                wheelAdjustmentRate = 0;
                ShipMovement.Instance.WheelAdjustment = 0;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// called when the player is looking at a controller
    /// </summary>
    public void EnterHover()
    {
        Debug.Log($"{controllerType} SPOTTED");
    }

    /// <summary>
    /// called when the player stops looking at a controller
    /// </summary>
    public void ExitHover()
    {
        Debug.Log($"{controllerType} LOST");
    }
}