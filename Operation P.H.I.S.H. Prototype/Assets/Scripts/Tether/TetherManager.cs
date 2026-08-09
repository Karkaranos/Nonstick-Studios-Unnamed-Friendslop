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
using UnityEngine.Splines;

public class TetherManager : Singleton<TetherManager>   
{
    [BoxGroup("Temp"), SerializeField] private LineRenderer TEMP_lineRenderer;

    [BoxGroup("Player Interaction"), SerializeField] public float TotalMaxTetherLength = 100f;
    [BoxGroup("Player Interaction"), SerializeField] private SplineContainer splineContainer;
    [BoxGroup("Player Interaction"), SerializeField] private SplineExtrude splineExtrude;

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
    [BoxGroup("Tether Settings"), Range(0, 100)] public float MaxLengthToCreateNewTetherSegment = 25;

    [Tooltip("Tethers will auto adjust themselves so they are as close to this length as possible")]
    [BoxGroup("Tether Settings"), MinMaxSlider(0, 100),SerializeField] 
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

    [Foldout("Advanced")] public float SecondsBetweenTetherCreations = 3;
    [Foldout("Advanced")] public LayerMask TetherLayerMask;

    public float MaxDesiredTetherLength => DesiredTetherLengthRange.y;
    public float MinDesiredTetherLength => DesiredTetherLengthRange.x;


    [Foldout("Debug Settings"), SerializeField] public TetherSegment.DebugColorOption debugColorOption;
    [Foldout("Debug Settings"), SerializeField] private TetherSegment startingSegment; //TODO: support for multiple tethers lol

    #region Player Interaction

    /// <summary>
    /// Forces tethered object to move along the spline, towards its base
    /// </summary>
    public void PullTetheredObject(TetherSegment tether, float deltaTime)
    {   
        // TEMP 
        if (tether == null) tether = startingSegment;

        // temp recursive solution until we do something better
        if (tether.followingObject == null)
        {
            PullTetheredObject(tether.NextSegment, deltaTime);
            return;
        }

        // temp numbers im tired
        tether.AdjustBackwards(0.5f);
    }

    #endregion

    public bool IsPlayerTethered(Movement player)
    {
        // TODO:
        //changed for testing for now
        return false;
    }

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

        // TODO: VERY temp patch. add a dictionary or something that maps players to their tethers.
        float totalDistance = SplineUtilities.GetTotalRopeLength(startingSegment);
        TetherSegment playerSegment = SplineUtilities.GetEndSegment(startingSegment); 

        // player still has so much wiggle room!!
        if (totalDistance < TotalMaxTetherLength)
            return true;

        moveDirection.Normalize();
        Vector3 endSegmentDirection = playerSegment.EvaluateForwardDirection(0.6f).normalized;

        return IsMoveDirectionCleared(moveDirection, endSegmentDirection) 
            || IsMoveDirectionCleared(moveDirection, (playerSegment.PreviousSegment.transform.position - playerSegment.transform.position).normalized);
    }

    /// <summary>
    /// 1 means perfect alignemt, -1 means player is perfectly moving towards/along the tether
    /// </summary>
    private bool IsMoveDirectionCleared(Vector3 moveDirection, Vector3 endDirection)
    {
        // 
        float dot = Vector3.Dot(moveDirection, endDirection);
        //dot *= -1;
        return (dot > 1 - retractionClearance);
    }

    /// <summary>
    /// Get Length of all used tethers
    /// </summary>
    public float GetTotalTetherLength()
    {
        return SplineUtilities.GetTotalRopeLength(startingSegment);
    }

    public bool IsTotalTetherLengthMaxed()
    {
        float totalDistance = GetTotalTetherLength();  // TODO: VERY temp patch. add a dictionary or something that maps players to their tethers.

        // player still has so much wiggle room!!
        return (totalDistance < TotalMaxTetherLength) ;
    }

    private void Update()
    {
        // terribly unperformant but fine for prototype

        /*
        Spline spline = SplineUtilities.CreateSpline(startingSegment);
        splineContainer.Spline = spline;
        splineExtrude.Rebuild();
        */

        //TODO: remove TEMP line renderer (as alternative to spline renderer)
        List<Vector3> tetherPoints = new List<Vector3>();
        TetherSegment segment = startingSegment;
        while (segment != null)
        {
            tetherPoints.Add(segment.Evaluate(0.33f));
            tetherPoints.Add(segment.Evaluate(0.66f));

            // if the next segment is the end
            if (segment.NextSegment != null && segment.NextSegment.NextSegment == null)
                break;

            tetherPoints.Add(segment.startPosition);

            segment = segment.NextSegment;
        }

        TEMP_lineRenderer.positionCount = tetherPoints.Count;
        TEMP_lineRenderer.SetPositions(tetherPoints.ToArray());
    }
}
