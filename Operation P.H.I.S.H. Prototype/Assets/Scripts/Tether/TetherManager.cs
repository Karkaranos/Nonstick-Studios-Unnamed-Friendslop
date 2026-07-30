/*****************************************************************************
* File Name :         TetherManager.cs
* Author :            Toby Schamberger
* Creation Date :     7/19/26
*
* Brief Description : 
*****************************************************************************/

using NaughtyAttributes;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevel;

public class TetherManager : Singleton<TetherManager>   
{
    [BoxGroup("Player Interaction"), SerializeField] public float TotalMaxTetherLength = 100f;

    [Range(0, 1f)]
    [Tooltip("How much toleration the player has if they try to retract the tether while it is at max length")]
    [BoxGroup("Player Interaction"), SerializeField] private float retractionClearance = 0.3f;

    [Tooltip("Minimum space there needs to be around the tether")]
    [BoxGroup("Tether Settings")] public float TetherNodeCollisionRadius = 2;
    [Tooltip("Minimum space there needs to be along the tether's spline")]
    [BoxGroup("Tether Settings")] public float TetherSplineCollisionRadius = 1;

    //TODO: add a way to disable this because it may be laggy

    [Header("Tether Auto Adjustment")]

    [BoxGroup("Tether Settings"), Range(0, 15)] public float MinLengthToDissolveTetherSegment = 5;
    [BoxGroup("Tether Settings"), Range(5, 100)] public float MaxLengthToCreateNewTetherSegment = 25;

    [Tooltip("Tethers will auto adjust themselves so they are as close to this length as possible")]
    [BoxGroup("Tether Settings"), MinMaxSlider(5, 100),SerializeField] 
    private Vector2 DesiredTetherLengthRange = new Vector2(5, 15);

    [Space(20)]

    [BoxGroup("Tether Settings"), Min(0.001f)]
    public float TetherAutoAdjustmentSpeed = 1;

    [BoxGroup("Tether Settings")]
    public bool TryEvenTetherLengths = true; // i have a feeling this is gonna get really expensive
    [BoxGroup("Tether Settings"), Min(0.001f), ShowIf(nameof(TryEvenTetherLengths))]
    [Tooltip ("Minimum difference between two sides of a tether for it to be considered uneven")]
    public float UnevenTetherSideDifference = 0.5f;
    [BoxGroup("Tether Settings"), Min(0.001f), ShowIf(nameof(TryEvenTetherLengths))]
    public float TetherAutoEvenLengthSpeed = 0.25f;

    [Header("Other")]

    [BoxGroup("Tether Settings"), Required] public TetherSegment TetherSegmentPrefab;

    [Foldout("Advanced")]
    public float SecondsBetweenTetherCreations = 3;

    public float MaxDesiredTetherLength => DesiredTetherLengthRange.y;
    public float MinDesiredTetherLength => DesiredTetherLengthRange.x;


    [Foldout("Debug Settings"), SerializeField] public TetherSegment.DebugColorOption debugColorOption;
    [Foldout("Debug Settings"), SerializeField] private TetherSegment startingSegment;

    [Foldout("Debug"), SerializeField] private TetherSegment/*List<TetherSegment>*/ tetherHeadNodes; //TODO: support for multiple tethers lol

    /// <summary>
    /// Returns true if player can move in a specific direction, given the tether.
    /// </summary>
    /// <returns></returns>
    public bool CanPlayerMoveInDirection(Vector3 moveDirection)
    {
        /*float totalDistance = 0;
        foreach (var tether in tetherHeadNodes)
        {
            totalDistance += SplineUtilities.GetTotalRopeLength(tether);
        }*/
        float totalDistance = SplineUtilities.GetTotalRopeLength(tetherHeadNodes);
        TetherSegment playerSegment = SplineUtilities.GetEndSegment(tetherHeadNodes); // TODO: VERY temp patch. add a dictionary or something that maps players to their tethers.

        // player still has so much wiggle room!!
        if (totalDistance < TotalMaxTetherLength)
            return true;

        moveDirection.Normalize();
        Vector3 endSegmentDirection = playerSegment.EvaluateForwardDirection(0.95f).normalized;

        // 1 means perfect alignemt, -1 means player is perfectly moving towards/along the tether
        float dot = Vector3.Dot(moveDirection, endSegmentDirection);
        dot *= -1;
        return (dot > 1 - retractionClearance)
    }
}
