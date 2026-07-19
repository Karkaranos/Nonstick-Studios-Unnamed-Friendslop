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
    [Tooltip("Tethers will auto adjust themselves so they are as close to this length as possible")]
    [BoxGroup("Tether Settings")] public float DesiredTetherLength = 15;

    [BoxGroup("Tether Settings")] public float MaxLengthToCreateNewTetherSegment = 25;

    [Foldout("Advanced Debug")]
    public float SecondsBetweenTetherCreations = 3;

    [Required]
    public TetherSegment TetherSegmentPrefab;

    public void SplitTetherSegment(TetherSegment segment, float t = 0.5f)
    {
        if (segment.NextSegment == null) {
            Debug.LogError($"Cant split segment \"{segment.gameObject.name}\" if its Next Segment is null");
            return;
        }

        // if the tethers too fresh
        if (Time.time - segment.LastTimeUpdated < SecondsBetweenTetherCreations ||
            Time.time - segment.NextSegment.LastTimeUpdated < SecondsBetweenTetherCreations)
        {
            return;
        }

        Debug.Log("Splitting Tether Segment");

        Vector3 position = segment.Evaluate(t);
        TetherSegment newTetherSegment = Instantiate(TetherSegmentPrefab, position, Quaternion.identity);

        Vector3 forwardDirection = segment.EvaluateForwardDirection(t);
        newTetherSegment.transform.forward = forwardDirection;

        // Update the linked node structure 
        TetherSegment NextSegment = segment.NextSegment;

        segment.NextSegment = newTetherSegment;
        
        newTetherSegment.PreviousSegment = segment;
        newTetherSegment.NextSegment = NextSegment;

        NextSegment.PreviousSegment = newTetherSegment;
    }
}
