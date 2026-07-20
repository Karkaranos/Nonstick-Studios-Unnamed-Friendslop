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
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class TetherSegment : MonoBehaviour
{
    [Foldout("Debug Options"), SerializeField] private bool _drawSplineCalculations;

    [Foldout("Debug"),SerializeField] private TetherSegment _previousSegment;
    [Foldout("Debug"), SerializeField] private TetherSegment _nextSegment;
    [Foldout("Debug")] public Transform followingObject;

    [Foldout("Debug")] public float forwardAnchorOffset;
    [Foldout("Debug")] public float backwardAnchorOffset;


    public float LastTimeUpdated { get; private set; }

    public TetherSegment PreviousSegment
    {
        get { return _previousSegment; }
        set
        {
            LastTimeUpdated = Time.time;
            _previousSegment = value;
        }
    }
    public TetherSegment NextSegment
    {
        get { return _nextSegment; }
        set
        {
            LastTimeUpdated = Time.time;
            _nextSegment = value;
        }
    }

    // Forwards and Backwards behave as the "handles" for the spline

    // Calculations relative to this node
    public Vector3 startPosition => transform.position;
    public Vector3 forwardDirection => transform.forward;
    private Vector3 backwardDirection => -transform.forward;
    public float ForwardHandleLength => ((GetForawrdLength() + GetBackwardLength()) / 2) + forwardAnchorOffset; //GetForawrdLength
    public float BackwardHandleLength => ((GetForawrdLength() + GetBackwardLength()) / 2) + backwardAnchorOffset; //GetBackwardLength
    private Vector3 forwardAnchorPosition => startPosition + (forwardDirection * ForwardHandleLength);
    private Vector3 backwardAnchorPosition => startPosition + (backwardDirection * backwardAnchorOffset);

    // Calculations relative to the next node or following object (the player)
    private bool hasEndPoint => NextSegment != null || followingObject != null;
    private Vector3 endPosition => GetSegmentEnd();
    private Vector3 endBackwardDirection => GetSegmentEndBackward();
    private Vector3 endBackwardAnchorPosition => NextSegment.backwardAnchorPosition;

    private void Start()
    {
        LastTimeUpdated = Time.time;
    }

    /// <summary>
    /// Move the Tether forwards along itself (towards the next tether).
    /// </summary>
    /// <param name="maxStepLength">Is implied to have already accounted for deltatime</param>
    public void AdjustForwards(float stepLength)
    {
        // TODO: CACHE THIS USING LastTimeUpdated
        float tetherLength = SplineUtilities.GetSegmentLength(this);

        // inverse lerp
        float t = stepLength / tetherLength;

        Vector3 targetPosition = Evaluate(t);
        Vector3 targetForwardDirection = EvaluateForwardDirection(t);

        transform.position = targetPosition;
        transform.forward = targetForwardDirection; 
    }

    /// <summary>
    /// Move the Tether forwards along itself (towards the next tether).
    /// </summary>
    /// <param name="maxStepLength">Is implied to have already accounted for deltatime</param>
    public void AdjustBackwards(float stepLength)
    {
        if(PreviousSegment == null)
        {
            Debug.LogError("Can not step backwards because previous segment is null");
            return;
        }

        // TODO: CACHE THIS USING LastTimeUpdated
        float tetherLength = SplineUtilities.GetSegmentLength(PreviousSegment);

        // inverse lerp
        float t = stepLength / tetherLength;

        Vector3 targetPosition = Evaluate(1-t);
        Vector3 targetForwardDirection = EvaluateForwardDirection(1-t);

        transform.position = targetPosition;
        transform.forward = targetForwardDirection;
    }

    /// <summary>
    /// Get bezier position at t percent.
    /// If "Evaluate Bezier curve at t" doesnt make sense to you then u should do research bc its a pretty visual thing so its hard to explain in a code comment.
    /// </summary>
    public Vector3 Evaluate(float t)
    {
        // segment start -> forward
        Vector3 a = Vector3.Lerp(startPosition, forwardAnchorPosition, t);

        // segment end backward -> segment end
        Vector3 b = Vector3.Lerp(endBackwardAnchorPosition, endPosition, t);

        return Vector3.Lerp(a, b, t);
    }

    /// <summary>
    /// Get forward direction at t percent.
    /// If you imagine a lil airplane that is flying and following the beziers path, this is the direction the airplane is pointing at precent t.
    /// </summary>
    public Vector3 EvaluateForwardDirection(float t)
    {
        // Kinda sample the surrounding points, sort of like a derivative.
        // Theres probably a better mathy way to do this but i think this is a good enough solution.

        float t_a = Mathf.Max(t-0.01f, 0);
        float t_b = Mathf.Min(t+0.01f, 1);

        Vector3 a = Evaluate(t_a);
        Vector3 b = Evaluate(t_b);

        Vector3 difference = b - a;

        return difference.normalized;
    }


    #region Physics

    private void Update()
    {
        // I have an idea for a robust solution that doesnt involve update + heavy calculations every frame but this is a prototype so i dont care.

        CheckForCollisions(Time.deltaTime);
    }

    //deltaTimes here bc eventually this shouldnt be getting called every frame yk
    private void CheckForCollisions(float deltaTime)
    {
        if (NextSegment == null && followingObject == null) return;

        if (TryAdjustTetherLength(deltaTime))
            return;

        if (CheckForCollisionAroundNode(Time.deltaTime))
            return; // dont move the node and update the spline at the same frame, thats just rude.

        // Check for collisions along the spline

        CheckForCollisionAlongSpline();
    }

    /// <summary>
    /// Nudges the tether node if its length is funky.
    /// </summary>
    /// <returns>True if something happened</returns>
    private bool TryAdjustTetherLength(float deltaTime)
    {
        // do not resize if this is one of the ends
        if(NextSegment == null || PreviousSegment == null) return false;

        // Lengths of tether ahead and behind of this node
        float forwardLength = SplineUtilities.GetSegmentLength(this);
        float backwardsLength = SplineUtilities.GetSegmentLength(PreviousSegment);

        // if tether is too long on both sides then there needs to be more nodes!
        if(forwardLength > TetherManager.Instance.MaxDesiredTetherLength && backwardsLength > TetherManager.Instance.MaxDesiredTetherLength)
        {
            TetherManager.Instance.SplitTetherSegment(this);
            return true;
        }

        // If tether is too short on both sides then lets just destroy it.
        if(forwardLength < TetherManager.Instance.MinDesiredTetherLength && backwardsLength < TetherManager.Instance.MinDesiredTetherLength)
        {
            TetherManager.Instance.DissolveTetherSegment(this);
            return true;
        }

        // If too much length ahead / not enough length behind
        if(forwardLength > TetherManager.Instance.MaxDesiredTetherLength || backwardsLength < TetherManager.Instance.MinDesiredTetherLength)
        {
            AdjustForwards(deltaTime * TetherManager.Instance.TetherAutoAdjustmentSpeed);
            return true;
        }

        // If too much length behind / not enough length ahead
        if (forwardLength < TetherManager.Instance.MinDesiredTetherLength || backwardsLength > TetherManager.Instance.MaxDesiredTetherLength)
        {
            AdjustBackwards(deltaTime * TetherManager.Instance.TetherAutoAdjustmentSpeed);
            return true;
        }

        // Try to even out tether lengths
        if (TetherManager.Instance.TryEvenTetherLengths)
        {
            float difference = Mathf.Abs(forwardLength - backwardsLength);

            // if the difference is big enough to care
            if (difference < TetherManager.Instance.UnevenTetherSideDifference)
                return false;

            if (forwardLength > backwardsLength)
                AdjustForwards(deltaTime * TetherManager.Instance.TetherAutoEvenLengthSpeed);
            else
                AdjustBackwards(deltaTime * TetherManager.Instance.TetherAutoEvenLengthSpeed);

            return false; // <- false because this is only a small adjustment
        }

        return false;
    }

    private bool CheckForCollisionAroundNode(float deltaTime)
    {
        bool intersecting = SplineUtilities.CheckNodeCollisionSphere(this, radius: TetherManager.Instance.TetherNodeCollisionRadius, out Vector3 hitPoint);

        if (!intersecting) return false;

        // Move away from hitpoint (SMOOTH THIS OUT IN THE FINAL GAME OBVIOUSLY)
        Vector3 direction = transform.position - hitPoint;
        transform.position = transform.position + (direction.normalized * deltaTime);

        LastTimeUpdated = Time.time;

        return true;
    }

    private bool CheckForCollisionAlongSpline()
    {
        bool intersecting = SplineUtilities.SplineSphereCast(this, out RaycastHit hit, out float intersection_t, radius: TetherManager.Instance.TetherSplineCollisionRadius);

        if (!intersecting)
        {
            return false;
        }

        TODO

        // if intersection is in the middle
        if(intersection_t > 0.25f && intersection_t < 0.75f)
        {
            TetherManager.Instance.InsertTetherSegment(this, intersection_t);
            return true;
        }
    }

    #endregion Physics


    #region Getters 

    /// <summary>
    /// The max length of the tether is half of the distance between the two nodes.
    /// TODO: make a more robust system for this.
    /// </summary>
    private float GetForawrdLength()
    {
        if(!hasEndPoint && PreviousSegment == null)
        {
            Debug.LogError("Empty little tether segment node :(");
            return 0;
        }

        if (!hasEndPoint)
            return GetBackwardLength();

        // Specifically not SplineUtilities.distance
        float distance = Vector3.Distance(startPosition, endPosition);
        return distance/2;
    }

    /// <summary>
    /// The max length of the tether is half of the distance between the two nodes.
    /// TODO: make a more robust system for this.
    /// </summary>
    private float GetBackwardLength()
    {
        if(PreviousSegment == null)
            return GetForawrdLength();

        // Specifically not SplineUtilities.distance
        float distance = Vector3.Distance(PreviousSegment.startPosition, startPosition);
        return distance/2;
    }

    /// <summary>
    /// Get the position of where the spline ends
    /// </summary>
    private Vector3 GetSegmentEnd()
    {
        if (NextSegment != null)
            return NextSegment.startPosition;
        else
            return followingObject.position;
    }

    /// <summary>
    /// Get the position of where the spline ends
    /// </summary>
    private Vector3 GetSegmentEndBackward()
    {
        if (NextSegment != null)
            return NextSegment.backwardDirection;
        else
            return -followingObject.transform.forward;
    }

    #endregion Getters

    #region Debug
    [Button]
    private void AddTetherAhead()
    {
        Vector3 position = startPosition + (forwardDirection * (TetherManager.Instance.MaxLengthToCreateNewTetherSegment * 0.8f));
        TetherSegment newTetherSegment = Instantiate(TetherManager.Instance.TetherSegmentPrefab, position, Quaternion.identity);

        newTetherSegment.transform.forward = this.forwardDirection;

        // Update the linked node structure 

        this.NextSegment = newTetherSegment;
        newTetherSegment.PreviousSegment = this;

        Selection.activeGameObject = newTetherSegment.gameObject;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startPosition, 2 /*TetherManager.Instance.TetherNodeCollisionRadius*/);

        if (NextSegment == null) return;

        // Draw the handles
        Gizmos.color = Color.white;
        Gizmos.DrawRay(startPosition, forwardDirection * ForwardHandleLength);
        Gizmos.DrawRay(endPosition, endBackwardDirection * NextSegment.BackwardHandleLength);

        // Draw the curve

        bool intersecting = SplineUtilities.SplineSphereCast(this, out RaycastHit raycastHit, out float idontcare, radius: 1 /*TetherManager.Instance.TetherSplineCollisionRadius*/);
        Gizmos.color = intersecting ? Color.red : Color.cyan;

        if (intersecting)
        {
            Gizmos.DrawWireSphere(raycastHit.point, 1);
        }

        int SEGMENTS_TO_DRAW = 5;

        for (int i = 0; i < SEGMENTS_TO_DRAW; i++)
        {
            float t_1 = ((float)i)   / SEGMENTS_TO_DRAW;
            float t_2 = ((float)i+1) / SEGMENTS_TO_DRAW;

            // these can be cached but im lazy and this is a debug function
            Vector3 pos_1 = Evaluate(t_1);
            Vector3 pos_2 = Evaluate(t_2);

            #region Extra Debug

            if (_drawSplineCalculations)
            {
                // segment start -> forward
                Vector3 a = Vector3.Lerp(startPosition, forwardAnchorPosition, t_1);

                // segment end backward -> segment end
                Vector3 b = Vector3.Lerp(endBackwardAnchorPosition, endPosition, t_1);

                Gizmos.color = Color.Lerp(Color.red, Color.green, t_1);
                Gizmos.DrawLine(a, b);

                // set it back
                Gizmos.color = intersecting ? Color.red : Color.cyan;
            }

            #endregion

            Gizmos.DrawLine(pos_1, pos_2);
            //Debug.DrawLine(pos_1, pos_2);
        }
    }
}
