/*****************************************************************************
* File Name :         TetherSegment.cs
* Author :            Toby Schamberger
* Creation Date :     7/18/26
*
* Brief Description : Refers to one piece of the entire tether. 
*                     Recursively references other TetherSegment in a node-like structure.
* 
*                     Specifically this script is to be placed on the node before whatever comes next.
*                     (IE. The hatch of the ship, before the player, or just somewhere random in the middle of the whole thing).
*****************************************************************************/

using NaughtyAttributes;
using UnityEngine;

public class TetherSegment : MonoBehaviour
{
    public float ForwardHandleLength = 10;
    public float BackwardHandleLength = 10;

    [Foldout("Debug")] public TetherSegment PreviousSegment;
    [Foldout("Debug")] public TetherSegment NextSegment;
    [Foldout("Debug")] public Transform followingObject;

    // Forwards and Backwards behave as the "handles" for the spline

    public Vector3 segmentStart => transform.position;
    public Vector3 segmentForwards => transform.forward;
    public Vector3 segmentBackwards => -transform.forward;

    // Handlers referring to the endpoint
    private Vector3 segmentEnd => GetSegmentEnd();
    private Vector3 segmentEndBackward => GetSegmentEndBackward();

    private void Update()
    {
        
    }


    /// <summary>
    /// Get bezier position at t percent.
    /// If "Evaluate Bezier curve at t" doesnt make sense to you then u should do research bc its a pretty visual thing so its hard to explain in a code comment.
    /// </summary>
    public Vector3 Evaluate(float t)
    {
        // segment start -> forward
        Vector3 a = Vector3.Lerp(segmentStart, segmentForwards * ForwardHandleLength, t);

        // segment end backward -> segment end
        Vector3 b = Vector3.Lerp(NextSegment.segmentBackwards * NextSegment.BackwardHandleLength, segmentEnd, t);

        return Vector3.Lerp(a, b, t);
    }

    /// <summary>
    /// Get the position of where the spline ends
    /// </summary>
    private Vector3 GetSegmentEnd()
    {
        if (NextSegment != null)
            return NextSegment.segmentStart;
        else
            return followingObject.position;
    }

    /// <summary>
    /// Get the position of where the spline ends
    /// </summary>
    private Vector3 GetSegmentEndBackward()
    {
        if (NextSegment != null)
            return NextSegment.segmentBackwards;
        else
            return -followingObject.transform.forward;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(segmentStart, 1);

        if (NextSegment == null) return;

        // Draw the handles
        Gizmos.color = Color.white;
        Gizmos.DrawRay(segmentStart, segmentForwards * ForwardHandleLength);
        Gizmos.DrawRay(segmentEnd, segmentEndBackward * NextSegment.BackwardHandleLength);

        // Draw the curve

        bool intersecting = SplineUtilities.SplineSphereCast(this, out RaycastHit idontcare, radius: 1);
        Gizmos.color = intersecting ? Color.red : Color.cyan;

        if (intersecting)
        {
            Gizmos.DrawWireSphere(idontcare.point, 1);
        }

        int SEGMENTS_TO_DRAW = 5;

        for (int i = 0; i < SEGMENTS_TO_DRAW; i++)
        {
            float t_1 = ((float)i)   / SEGMENTS_TO_DRAW;
            float t_2 = ((float)i+1) / SEGMENTS_TO_DRAW;

            // these can be cached but im lazy and this is a debug function
            Vector3 pos_1 = Evaluate(t_1);
            Vector3 pos_2 = Evaluate(t_2);

            Gizmos.DrawLine(pos_1, pos_2);
            //Debug.DrawLine(pos_1, pos_2);
        }
    }
}
