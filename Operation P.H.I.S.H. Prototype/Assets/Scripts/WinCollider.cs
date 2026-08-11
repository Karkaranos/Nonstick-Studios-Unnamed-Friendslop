/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    8/7/2026
Date Last Modified : 	8/7/2026
Brief Description : 	Detects collision for the PHISH and player for win condition
External Resources : 	
***************************************************/
using UnityEngine;

public class WinCollider : MonoBehaviour
{
    [SerializeField, Tooltip("Canvas that displays win text.")]
    private Canvas winCanvas;

    /// <summary>
    /// On trigger enter, activates win canvas
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other != null && (other.gameObject.GetComponent<ShipMovement>() != null || other.gameObject.GetComponent<PlayerController>()))
        {
            winCanvas.gameObject.SetActive(true);
        }
    }
}
