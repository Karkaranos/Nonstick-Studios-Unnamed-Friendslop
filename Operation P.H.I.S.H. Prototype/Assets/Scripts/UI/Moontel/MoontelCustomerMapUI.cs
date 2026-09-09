/*************************************************
Author Names : 		    Toby Schamberger
Date Created : 		    9/9/2026
Date Last Modified : 	9/9/2026

Brief Description : 	Displays customer locations on the map.

External Resources :    	
***************************************************/

using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoontelCustomerMapUI : MonoBehaviour
{
    [Header("Map Calibration")]
    [SerializeField] private Vector2 MapSize = Vector2.one;
    [SerializeField] private Vector2 MapCenter = Vector2.one;

    [Foldout("Advanced"), SerializeField] private Image mapDisplayImage;
    [Foldout("Advanced"), SerializeField] private Image guestDisplayIconPrefab;

    private Dictionary<GuestInteractable, Image> guestCanvasImages;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    /// <summary>
    /// Returns guests position as a scalar vector (x and y are between 0-1)
    /// (0,0) is top left of map. (1,1) is bottom right of map.
    /// Position may be out of the 0-1 range if the guest is off of the map
    /// </summary>
    private Vector2 GetGuestPositionScalar(GuestInteractable guest)
    {

    }

    /// <summary>
    /// Update the location of each customer.
    /// Running every frame because of how much customers will be moving. 
    /// This could technically be optimized because not EVERY guest needs to be updated EVERY frame, but come on, its 2026 its okay.
    /// </summary>
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 transformedScale = new Vector3(MapSize.x, 2, MapSize.y);
        Vector3 transformedCenter = new Vector3(MapCenter.x, 0, MapCenter.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transformedCenter, transformedScale);
    }
}
