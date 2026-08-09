using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;
using static UnityEngine.Rendering.HableCurve;

public static class SplineUtilities
{
    private static int _debug_total_segments_spawned = 2;
    private static LayerMask layerMask => TetherManager.Instance == null ? new LayerMask() : TetherManager.Instance.TetherLayerMask;

    /// <summary>
    /// Returns the APPROXIMATE length of the spline segment.
    /// </summary>
    /// <param name="n_slices">Increase this number to increase accuracy but also do more computations</param>
    /// <returns></returns>
    public static float GetSegmentLength(TetherSegment segment, int n_slices = 3)
    {
        float totalLength = 0;

        for (int i = 0; i < n_slices; i++)
        {
            float t_1 = ((float)i) / n_slices;
            float t_2 = ((float)i + 1) / n_slices;

            // these can be cached but im lazy
            Vector3 pos_1 = segment.Evaluate(t_1);
            Vector3 pos_2 = segment.Evaluate(t_2);

            float distance = Vector3.Distance(pos_1, pos_2); // running Vector3.Distance a lot every frame could be a problem *shrug*
            totalLength += distance;
        }
        return totalLength;
    }

    /// <summary>
    /// Recursively calculates the total length of the entire tether rope.
    /// </summary>
    /// <param name="segment">The head node for the rope</param>
    /// <param name="n_slices">Increase this number to increase accuracy but also do more computations</param>
    public static float GetTotalRopeLength(TetherSegment segment, int n_slices = 2)
    {
        if (segment == null) return 0;
        return GetSegmentLength(segment, n_slices) + GetTotalRopeLength(segment.NextSegment, n_slices);
    }

    public static TetherSegment GetEndSegment(TetherSegment segment)
    {
        if (segment == null) return null;
        if (segment.NextSegment == null)
            return segment;

        return GetEndSegment(segment.NextSegment);
    }

    public static List<BezierKnot> CreateBezierKnots(TetherSegment segment)
    {
        List<BezierKnot> splineKnots = new List<BezierKnot>();
        while (segment != null)
        {
            splineKnots.Add(segment.GetBezierKnot());
            segment = segment.NextSegment;
        }
        return splineKnots;
    }

    public static Spline CreateSpline(TetherSegment segment)
    {
        return new Spline(CreateBezierKnots(segment));
    }

    #region Physics

    /// <summary>
    /// Performes a spherecast along the tether segment towards its end point.
    /// Returns the first point that the spline sees but uhh there could be more.
    /// </summary>
    /// <param name="n_slices">How many straight lines to slice the spline into, since computing collisions is expensive</param>
    /// <param name="intersection_t">APPROXIMATE percent along the bezier curve where the collision occured.</param>
    /// <returns></returns>
    public static bool SplineSphereCast(TetherSegment segment, out RaycastHit raycastResult, out float intersection_t, float radius = 3, int n_slices = 3)
    {
        for (int i = 0; i < n_slices; i++)
        {
            float t_1 = ((float)i) / n_slices;
            float t_2 = ((float)i + 1) / n_slices;

            // these can be cached but im lazy
            Vector3 pos_1 = segment.Evaluate(t_1);
            Vector3 pos_2 = segment.Evaluate(t_2);

            Vector3 direction = pos_2 - pos_1;
            float distance = Vector3.Distance(pos_1, pos_2); // running Vector3.Distance a lot every frame could be a problem *shrug*

            Ray ray = new Ray();
            // TODO: layermask
            bool hit = Physics.SphereCast(pos_1, radius, direction, out raycastResult, distance, layerMask: layerMask);

            if (hit)
            {
                // raycastResult was just set so we dont need to do anything with it
                intersection_t = (t_1 + t_2) / 2;
                return true;
            }

            //Debug.DrawLine(pos_1, pos_2);
        }
        raycastResult = default;
        intersection_t = -1;
        return false;

    }

    public static bool CheckNodeCollisionSphere(TetherSegment segment, float radius, out Vector3 closestCollisionPoint)
    {
        // TODO: layermask
        RaycastHit[] hits = Physics.SphereCastAll(
            origin: segment.startPosition,
            radius: radius,
            // not sure what to do with direction / maxDistance since i really just want a sphere rn
            direction: segment.forwardDirection,
            maxDistance: 0.01f,
            layerMask: layerMask
        );

        if (hits.Length <= 0)
        {
            closestCollisionPoint = Vector3.zero;
            return false;
        }

        // using manhattan distance here to avoid calling a lot of sqrt functions AND because we can really get away with it here.
        closestCollisionPoint = hits
            .Select(h => h.point)
            .OrderBy(p => StaticUtilities.ManhattanDistance(segment.startPosition, p)) 
            .First();
        return true;
    }

    #endregion

    #region Node Manipulation

    /// <summary>
    /// Creates a new TetherSegment between "segment" and its next segment at t percent.
    /// </summary>
    /// <returns>The new segment</returns>
    public static TetherSegment InsertTetherSegment(TetherSegment segment, float t = 0.5f)
    {
        if(segment == null)
        {
            Debug.LogError($"Segment is null for like, no reason");
            return null;
        }

        if (segment.NextSegment == null)
        {
            Debug.LogError($"Cant split segment \"{segment.gameObject.name}\" if its Next Segment is null");
            return null;
        }

        // if the tethers too fresh
        if (Time.time - segment.LastTimeUpdated < TetherManager.Instance.SecondsBetweenTetherCreations ||
            Time.time - segment.NextSegment.LastTimeUpdated < TetherManager.Instance.SecondsBetweenTetherCreations)
        {
            return null;
        }

        Vector3 position = segment.Evaluate(t);
        Vector3 forwardDirection = segment.EvaluateForwardDirection(t);
        float length = SplineUtilities.GetSegmentLength(segment);

        TetherSegment newTetherSegment = GameObject.Instantiate(TetherManager.Instance.TetherSegmentPrefab, position, Quaternion.identity);
        newTetherSegment.gameObject.name = $"Tether {++_debug_total_segments_spawned}";

        newTetherSegment.transform.forward = forwardDirection;

        // Update the linked node structure 
        SplineUtilities.InsertNodeReference(segment, newTetherSegment, segment.NextSegment);

        Debug.Log($"Inserted Tether Segment between {segment.gameObject.name} and {segment.NextSegment.gameObject.name}:\nt={t}\tPosition:{position}\tPrevious Length: {length}");

        return newTetherSegment;
    }

    /// <summary>
    /// Deletes segment and inserts two evenly spaced tether nodes.
    /// </summary>
    /// <param name="segment"></param>
    public static void SplitTetherSegment(TetherSegment segment)
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
        if (Time.time - segment.LastTimeUpdated < TetherManager.Instance.SecondsBetweenTetherCreations ||
            Time.time - segment.NextSegment.LastTimeUpdated < TetherManager.Instance.SecondsBetweenTetherCreations)
        {
            return;
        }

        #endregion

        Debug.Log("Splitting Tether Segment");

        var segment_a = segment.PreviousSegment;

        // there may be a more efficient way to redo all of this but the temptation to reuse code was too strong...

        // get rid of middle segment
        DissolveTetherSegment(segment);

        // insert 2 new segments
        var segment_b = InsertTetherSegment(segment_a, 0.33f);
        InsertTetherSegment(segment_b, 0.5f); // if there is 0.66 left from the original length, then frick idk how to explain it just trust me on this one.
    }

    /// <summary>
    /// Inserts a tether node between two other tether nodes by updating references. 
    /// Does not update transformations.
    /// </summary>
    public static void InsertNodeReference(TetherSegment node1, TetherSegment node2, TetherSegment node3)
    {
        node1.NextSegment = node2;

        node2.PreviousSegment = node1;
        node2.NextSegment = node3;

        node3.PreviousSegment = node2;
    }

    /// <summary>
    /// Delete the tether segment and reconnect its neighbor nodes.
    /// </summary>
    /// <param name="segment"></param>
    public static void DissolveTetherSegment(TetherSegment segment)
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


        float segmentLength = SplineUtilities.GetSegmentLength( segment );
        Debug.Log($"Disolving Tether Segment: '{segment.gameObject.name}'\n" +
            $"Length: {segmentLength}");

        var segment_a = segment.PreviousSegment;
        var segment_b = segment.NextSegment;

        segment_a.NextSegment = segment_b;
        segment_b.PreviousSegment = segment_a;

        GameObject.Destroy(segment.gameObject);

        // TODO: adjust tether handle lengths so it looks nicer.
    }

    #endregion
}
