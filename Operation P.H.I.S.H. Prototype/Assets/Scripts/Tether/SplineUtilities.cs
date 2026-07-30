using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;
using static UnityEngine.Rendering.HableCurve;

public static class SplineUtilities
{
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
            bool hit = Physics.SphereCast(pos_1, radius, direction, out raycastResult, distance);

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
            maxDistance: 0.01f
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
}
