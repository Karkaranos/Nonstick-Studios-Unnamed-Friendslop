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

    [BoxGroup("Tether Settings"), Range(5, 100)] public float MaxLengthToCreateNewTetherSegment = 25;

    [Header("Tether Auto Adjustment")]

    [Tooltip("Tethers will auto adjust themselves so they are as close to this length as possible")]
    [BoxGroup("Tether Settings"), MinMaxSlider(5, 100),SerializeField] 
    private Vector2 DesiredTetherLengthRange = new Vector2(5, 15);

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

    [Foldout("Advanced Debug")]
    public float SecondsBetweenTetherCreations = 3;

    public float MaxDesiredTetherLength => DesiredTetherLengthRange.y;
    public float MinDesiredTetherLength => DesiredTetherLengthRange.x;

    private int _debug_total_segments_spawned = 2;

    /// <summary>
    /// Creates a new TetherSegment between "segment" and its next segment at t percent.
    /// </summary>
    /// <returns>The new segment</returns>
    public TetherSegment InsertTetherSegment(TetherSegment segment, float t = 0.5f)
    {
        if (segment.NextSegment == null) {
            Debug.LogError($"Cant split segment \"{segment.gameObject.name}\" if its Next Segment is null");
            return null;
        }

        // if the tethers too fresh
        if (Time.time - segment.LastTimeUpdated < SecondsBetweenTetherCreations ||
            Time.time - segment.NextSegment.LastTimeUpdated < SecondsBetweenTetherCreations)
        {
            return null;
        }

        Debug.Log($"Inserting Tether Segment between {segment.gameObject.name} and {segment.NextSegment.gameObject.name} with t={t}");

        Vector3 position = segment.Evaluate(t);
        TetherSegment newTetherSegment = Instantiate(TetherSegmentPrefab, position, Quaternion.identity);
        newTetherSegment.gameObject.name = $"Tether {++_debug_total_segments_spawned}";

        Vector3 forwardDirection = segment.EvaluateForwardDirection(t);
        newTetherSegment.transform.forward = forwardDirection;

        // Update the linked node structure 
        TetherSegment NextSegment = segment.NextSegment;

        segment.NextSegment = newTetherSegment;
        
        newTetherSegment.PreviousSegment = segment;
        newTetherSegment.NextSegment = NextSegment;

        NextSegment.PreviousSegment = newTetherSegment;

        return newTetherSegment;
    }

    /// <summary>
    /// Delete the tether segment and reconnect its neighbor nodes.
    /// </summary>
    /// <param name="segment"></param>
    public void DissolveTetherSegment(TetherSegment segment)
    {

        #region whatever
        if (segment.PreviousSegment == null)
        {
            Debug.LogError($"Cant split segment \"{segment.gameObject.name}\" if its Previous Segment is null");
            return;
        }

        if (segment.NextSegment == null)
        {
            Debug.LogError($"Cant split segment \"{segment.gameObject.name}\" if its Next Segment is null");
            return;
        }

        #endregion

        Debug.Log("Disolving Tether Segment");

        var segment_a = segment.PreviousSegment;
        var segment_b = segment.NextSegment;

        segment_a.NextSegment = segment_b;
        segment_b.PreviousSegment = segment_a;

        Destroy(segment.gameObject);

        // TODO: adjust tether handle lengths so it looks nicer.
    }

    /// <summary>
    /// Deletes segment and inserts two evenly spaced tether nodes.
    /// </summary>
    /// <param name="segment"></param>
    public void SplitTetherSegment(TetherSegment segment)
    {
        #region whatever
        if (segment.PreviousSegment == null)
        {
            Debug.LogError($"Cant split segment \"{segment.gameObject.name}\" if its Previous Segment is null");
            return;
        }

        if (segment.NextSegment == null)
        {
            Debug.LogError($"Cant split segment \"{segment.gameObject.name}\" if its Next Segment is null");
            return;
        }

        // if the tethers too fresh
        if (Time.time - segment.LastTimeUpdated < SecondsBetweenTetherCreations ||
            Time.time - segment.NextSegment.LastTimeUpdated < SecondsBetweenTetherCreations)
        {
            return;
        }

        #endregion

        Debug.Log("Splitting Tether Segment");

        var segment_a = segment.PreviousSegment;

        // there may be a more efficient way to redo all of this but the temptation to reuse code was too strong...
        DissolveTetherSegment(segment);
        var segment_b = InsertTetherSegment(segment_a, 0.33f);
        InsertTetherSegment(segment_b, 0.5f); // if there is 0.66 left from the original length, then frick idk how to explain it just trust me on this one.
    }
}
