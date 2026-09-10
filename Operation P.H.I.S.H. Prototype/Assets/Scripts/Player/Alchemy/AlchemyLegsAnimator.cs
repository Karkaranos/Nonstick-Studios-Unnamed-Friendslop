/*************************************************
Author Names : 		    Toby Schamberger
Date Created : 		    8/15/2026
Date Last Modified : 	8/15/2026
Brief Description : 	Animates the players legs while theyre moving. Hip rotation is handled in LandMovement.
External Resources : 	
***************************************************/

using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class AlchemyLegsAnimator : MonoBehaviour
{
    [SerializeField, Required] private Transform leftLegPivot;
    [SerializeField, Required] private Transform rightLegPivot;
    [SerializeField, MinMaxSlider(0,180)] private Vector2 legRotationRange;
    [SerializeField] private float legRotationSpeed = 3f;

    // t refers to the left leg primarily. RLeg rotation is a 1-t
    private float t;

    private bool leftLegMovingForwards = true;
    private bool rightLegMovingForwards = false;

    private Vector3 leftLegRotation, rightLegRotation;

    private float restingLegRotation_percent;
    private Coroutine legAnimationCoroutine;

    private void Start()
    {
        // Get players resting leg percent as a percent
        float restingLegRotation = leftLegPivot.localEulerAngles.x;
        restingLegRotation_percent = Mathf.InverseLerp(legRotationRange.x, legRotationRange.y, restingLegRotation);
        t = restingLegRotation_percent;

        leftLegRotation = leftLegPivot.localEulerAngles;
        rightLegRotation = rightLegPivot.localEulerAngles;

        PublicEvents.MoveStarted += OnPlayerMoveStart;
    }

    /// <summary>
    /// When player presses move, then start the animation
    /// </summary>
    void OnPlayerMoveStart()
    {
        if(legAnimationCoroutine != null)
            StopCoroutine(legAnimationCoroutine);
        legAnimationCoroutine = StartCoroutine(AnimateLegs()).Then(ReturnLegsToRestingPosition());
    }

    /// <summary>
    /// Swings the legs while the player is moving
    /// </summary>
    /// <returns></returns>
    IEnumerator AnimateLegs()
    {
        while (PlayerInputHandler.Instance.IsMovementHeld)
        {

            // theres definitely a cleaner way to do this that isnt a million if statements and itll haunt me every night but idc

            if(leftLegMovingForwards)
                t -= Time.deltaTime * legRotationSpeed;
            else
                t += Time.deltaTime * legRotationSpeed;

            t = Mathf.Clamp01(t);

            if (t <= 0) leftLegMovingForwards = false;
            if (t >= 1) leftLegMovingForwards = true;

            leftLegRotation.x = Mathf.Lerp(legRotationRange.x, legRotationRange.y, t);
            rightLegRotation.x = Mathf.Lerp(legRotationRange.x, legRotationRange.y, 1-t);
            leftLegPivot.transform.localEulerAngles = leftLegRotation;
            rightLegPivot.transform.localEulerAngles = rightLegRotation;

            yield return null;
        }
    }

    IEnumerator ReturnLegsToRestingPosition()
    {
        float rightLeg_t = Mathf.InverseLerp(legRotationRange.x, legRotationRange.y, rightLegPivot.localEulerAngles.x);

        while (t != restingLegRotation_percent || rightLeg_t != restingLegRotation_percent)
        {
            t = Mathf.MoveTowards(t, restingLegRotation_percent, Time.deltaTime * legRotationSpeed);
            rightLeg_t = Mathf.MoveTowards(rightLeg_t, restingLegRotation_percent, Time.deltaTime * legRotationSpeed);

            leftLegRotation.x = Mathf.Lerp(legRotationRange.x, legRotationRange.y, t);
            rightLegRotation.x = Mathf.Lerp(legRotationRange.x, legRotationRange.y, rightLeg_t);
            leftLegPivot.transform.localEulerAngles = leftLegRotation;
            rightLegPivot.transform.localEulerAngles = rightLegRotation;

            yield return null;
        }

        leftLegMovingForwards = true;
        rightLegMovingForwards = false;
    }
}
