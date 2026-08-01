/*************************************************
Author Names : 		    Clare Grady, Cade Naylor
Date Created : 		    07/22/2026
Date Last Modified : 	07/28/202
Brief Description : 	Base Class that defines all functions movement scripts should have

External Resources :    	
***************************************************/
using Unity.VisualScripting;
using UnityEngine;

public abstract class Movement: MonoBehaviour 
{
    /// <summary>
    /// Subscribes to all the Public events for controls
    /// </summary>
    protected virtual void OnEnable()
    {
        PublicEvents.MoveDirection += OnMove;
        PublicEvents.MoveStopped += OnMoveEnd;
        PublicEvents.MousePosition += OnMouseMove;
        PublicEvents.EClicked += OnEClicked;
        PublicEvents.SpaceStarted += OnSpaceStarted;
        PublicEvents.SpaceFinished += OnSpaceFinished;
        PublicEvents.ShiftStarted += OnShiftStarted;
        PublicEvents.ShiftFinished += OnShiftFinished;
        PublicEvents.ControlStarted += OnControlStarted;
        PublicEvents.ControlFinished += OnControlFinished;
    }

    /// <summary>
    /// Unsubscribes from all public events for controls
    /// </summary>
    protected virtual void OnDisable()
    {
        PublicEvents.MoveDirection -= OnMove;
        PublicEvents.MoveStopped -= OnMoveEnd;
        PublicEvents.MousePosition -= OnMouseMove;
        PublicEvents.EClicked -= OnEClicked;
        PublicEvents.SpaceStarted -= OnSpaceStarted;
        PublicEvents.SpaceFinished -= OnSpaceFinished;
        PublicEvents.ShiftStarted -= OnShiftStarted;
        PublicEvents.ShiftFinished -= OnShiftFinished;
        PublicEvents.ControlStarted -= OnControlStarted;
        PublicEvents.ControlFinished -= OnControlFinished;
    }


    protected abstract void OnMove(Vector2 moveVector);
    protected abstract void OnMoveEnd();
    protected abstract void OnMouseMove(Vector2 cameraVector);
    protected abstract void OnEClicked();
    protected abstract void OnSpaceStarted(bool fullyPerformed);
    protected abstract void OnSpaceFinished();
    protected abstract void OnShiftStarted();
    protected abstract void OnShiftFinished();
    protected abstract void OnControlStarted();
    protected abstract void OnControlFinished();

    protected abstract bool LookingAtObject();

    public abstract Vector3 LastCameraAngle();

    public abstract void SetCameraAngle(Vector3 angle);

}
