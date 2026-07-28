/*************************************************
Author Names : 		    Clare Grady
Date Created : 		    07/22/2026
Date Last Modified : 	07/22/202
Brief Description : 	Actually defines and handles land movement

External Resources :    	
***************************************************/
using UnityEngine;

public class LandMovement : Movement
{
    /// <summary>
    /// Override from Movement base class
    /// </summary>
    /// <param name="cameraVector"></param>
    protected override void OnMouseMove(Vector2 cameraVector)
    {
        Debug.Log("LOOK");
    }

    /// <summary>
    /// Override from Movement base class
    /// </summary>
    /// <param name="moveVector"></param>
    protected override void OnMove(Vector2 moveVector)
    {
        Debug.Log("MOVE");
    }

    /// <summary>
    /// Override from Movement base class
    /// </summary>
    protected override void OnEClicked()
    {
        Debug.Log("E");
    }

    /// <summary>
    /// Override from movement base class
    /// </summary>
    protected override void OnSpaceClicked()
    {
        Debug.Log("SPACE");
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
    protected override void OnShiftStarted()
    {
        Debug.Log("Shift Start");
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
    protected override void OnShiftFinished()
    {
        Debug.Log("Shift Finished");
    }
    protected override void OnControlStarted()
    {
        Debug.Log("Control Started");
    }

    /// <summary>
    /// override from movement base class
    /// </summary>
    protected override void OnControlFinished()
    {
        Debug.Log("Control Finished");
    }

}
