using UnityEngine;
using UnityEngine.Splines;

public static class SplineUtilities
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="spline"></param>
    /// <returns></returns>
    public static float GetSegmentLength(Spline spline, int n_slices=3)
    {
        return -1;
    }

    /// <summary>
    /// Performes a spherecast along the tether segment towards its end point.
    /// Returns the first point that the spline sees but uhh there could be more.
    /// </summary>
    /// <param name="n_slices">How many straight lines to slice the spline into, since computing collisions is expensive</param>
    /// <returns></returns>
    public static bool SplineSphereCast(TetherSegment segment, out RaycastHit raycastResult, float radius = 3, int n_slices = 3)
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
            bool hit = Physics.SphereCast(pos_1, radius, direction, out raycastResult, distance);

            // raycastResult was just set so we dont need to do anything with it
            if (hit)
                return true;

            //Debug.DrawLine(pos_1, pos_2);
        }
        raycastResult = default;
        return false;

    }
}
