/*************************************************
Author Names : 		    Jay Embry
Date Created : 		    7/24/2026
Date Last Modified : 	7/24/2026
Brief Description : 	Moves the P.H.I.S.H.
External Resources : 	
***************************************************/

using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class ShipMovementController: Singleton<ShipMovementController>
{

    bool movingForwardOrBackward = false;

    [Tooltip("The initial speed of the P.H.I.S.H.")]
    [SerializeField] float speed; 

    [Range(-5, 5)]
    [Tooltip("This variable is exposed in the inspector for the sake of testing. It should be hidden later.")]
    [SerializeField] float acceleration = 1;

    /// <summary>
    /// determines whether the player is attempting to move the ship or not
    /// </summary>
    void ToggleForwardOrBackwardMovement()
    {
        movingForwardOrBackward = !movingForwardOrBackward;

        if(movingForwardOrBackward)
        {
            StartCoroutine(MoveShipForwardOrBackward());
        }
    }

    IEnumerator MoveShipForwardOrBackward()
    {
        while(movingForwardOrBackward)
        {
            gameObject.transform.position += 
            gameObject.transform.position + transform.forward * speed * acceleration * Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// changes the acceleration of the ship based on the lever's position
    /// </summary>
    /// <param name="direction">
    /// which direction the lever was pushed
    /// </param>
    void MoveLeverForwardOrBackward(Vector2 direction)
    {
        if(movingForwardOrBackward)
        {
            if (direction.y >= 0.5f)
            {
                acceleration += 1;
            }
            else if(direction.y <= -0.5f)
            {
                acceleration -= 1;
            }
        }

    }
    
}
