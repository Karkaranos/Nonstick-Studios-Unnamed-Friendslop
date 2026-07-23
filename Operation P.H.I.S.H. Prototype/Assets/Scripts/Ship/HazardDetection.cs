/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    7/23/2026
Date Last Modified : 	7/23/2026
Brief Description : 	Detects if ship crashes into a hazard and takes 
                        the appropriate amount of damage/destroys ship
                        Doing this on a separate script so I don't interfere with Jay tasks
External Resources : 	
***************************************************/
using UnityEngine;

public class HazardDetection : MonoBehaviour
{
    /// <summary>
    /// Detects ship collision with hazard
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<HazardStats>() != null)
        {
            //hazard destroy
            if (collision.gameObject.GetComponent<HazardStats>().DestroyOnCollision)
            {
                //would be fancier later (probably an animation + many other systems involved)
                Destroy(gameObject);
                return;
            }

            //hazard damage
            float decreaseAmount = collision.gameObject.GetComponent<HazardStats>().DamageNumber;
            ShipResourceManager.Instance.LoseOxygen(decreaseAmount);
        }
    }
}
