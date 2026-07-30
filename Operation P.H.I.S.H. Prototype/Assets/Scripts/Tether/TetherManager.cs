/*****************************************************************************
* File Name :         TetherManager.cs
* Author :            Toby Schamberger
* Creation Date :     7/19/26
*
* Brief Description : 
*****************************************************************************/

using NaughtyAttributes;
using UnityEngine;

public class TetherManager : Singleton<TetherManager>   
{
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


    [Foldout("Debug Options"), SerializeField] public TetherSegment.DebugColorOption debugColorOption;
}
