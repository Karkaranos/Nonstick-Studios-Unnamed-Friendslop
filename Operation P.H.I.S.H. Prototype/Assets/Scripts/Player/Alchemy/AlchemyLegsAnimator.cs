/*************************************************
Author Names : 		    Toby Schamberger
Date Created : 		    8/15/2026
Date Last Modified : 	8/15/2026
Brief Description : 	Animates the players legs while theyre moving
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

    private float leftLeg_t;
    private float rightLeg_t;

    private bool leftLegMovingForwards = true;
    private bool rightLegMovingForwards = false;

    private Vector3 leftLegRotation, rightLegRotation;

    private float restingLegRotation_percent;
    private Coroutine legAnimationCoroutine;

    private void Start()
    {
        // Get players resting leg percent as a percent
        float restingLegRotation = leftLegPivot.rotation.x;
        restingLegRotation_percent = Mathf.InverseLerp(legRotationRange.x, legRotationRange.y, restingLegRotation);
        leftLeg_t = restingLegRotation_percent;
        rightLeg_t = restingLegRotation_percent;

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
            Debug.Log(leftLeg_t);

            // theres definitely a cleaner way to do this that isnt a million if statements and itll haunt me every night but idc

            if(leftLegMovingForwards)
                leftLeg_t -= Time.deltaTime * legRotationSpeed;
            else
                leftLeg_t += Time.deltaTime * legRotationSpeed;

            if (rightLegMovingForwards)
                rightLeg_t -= Time.deltaTime * legRotationSpeed;
            else
                rightLeg_t += Time.deltaTime * legRotationSpeed;

            leftLeg_t = Mathf.Clamp01(leftLeg_t);
            rightLeg_t = Mathf.Clamp01(rightLeg_t);

            if (leftLeg_t <= 0) leftLegMovingForwards = false;
            if (leftLeg_t >= 1) leftLegMovingForwards = true;

            if (rightLeg_t <= 0) rightLegMovingForwards = false;
            if (rightLeg_t >= 1) rightLegMovingForwards = true;

            float left_x = Mathf.Lerp(legRotationRange.x, legRotationRange.y, leftLeg_t);
            float right_x = Mathf.Lerp(legRotationRange.x, legRotationRange.y, rightLeg_t);

            leftLegRotation.x = left_x;
            rightLegRotation.x = right_x;
            leftLegPivot.transform.localEulerAngles = leftLegRotation;
            rightLegPivot.transform.localEulerAngles = rightLegRotation;

            yield return null;
        }
    }

    IEnumerator ReturnLegsToRestingPosition()
    {
        Debug.Log("Resting Position");
        leftLegMovingForwards = true;
        rightLegMovingForwards = false;
        yield return null;  
    }
}
