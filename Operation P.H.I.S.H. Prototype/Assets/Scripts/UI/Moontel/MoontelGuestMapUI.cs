/*************************************************
Author Names : 		    Toby Schamberger
Date Created : 		    9/9/2026
Date Last Modified : 	9/9/2026

Brief Description : 	Displays customer locations on the map.
                        Does not need any input from other scripts. (You can do whatever you want to the guests, and this script should adapt pretty good)

External Resources :    	
***************************************************/

using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MoontelGuestMapUI : MonoBehaviour
{
    [Header("Map Calibration")]
    [SerializeField] private Vector2 MapSize = Vector2.one;
    [SerializeField] private Vector2 MapCenter = Vector2.one;
    [SerializeField] private float mapFloorLevel = 0;

    [Header("Floors")]
    [SerializeField] private List<MoontelMapFloor> floorMaps = new List<MoontelMapFloor>();
    [SerializeField, ReadOnly] private int currentFloorIndex;
    private MoontelMapFloor currentFloorMap => floorMaps[currentFloorIndex];

    [Foldout("Animation"), SerializeField] private float animationSpeed = 0.5f;
    [Foldout("Animation"), SerializeField] private float animationMaxScale= 1.25f;

    [Foldout("Advanced"), SerializeField] private Image mapDisplayImage;
    [Foldout("Advanced"), SerializeField] private RectTransform topRightAnchor;
    [Foldout("Advanced"), SerializeField] private RectTransform bottomLeftAnchor;
    [Foldout("Advanced"), SerializeField] private Image guestDisplayIconPrefab;

    private Dictionary<GuestInteractable, Image> guestIconInstances = new();

    // topRight / bottomLeft instead of topLeft / bottomRight because it matches how rectTransform.anchorMin is calculated
    Vector3 topRightPosition3D => new Vector3(MapCenter.x + (MapSize.x / 2), 0, MapCenter.y + (MapSize.y / 2));
    Vector3 bottomLeftPosition3D => new Vector3(MapCenter.x - (MapSize.x / 2), 0, MapCenter.y - (MapSize.y / 2));

    #region Floors

    /// <summary>
    /// Return true if object is on the floor
    /// </summary>
    private bool IsObjectWithinFloor(Transform worldTransform, MoontelMapFloor floorMap)
    {
        float heightTotal = mapFloorLevel;
        for (int i = 0; i < floorMaps.Count; i++)
        {
            MoontelMapFloor floor = floorMaps[i];

            if (floor == floorMap)
            {
                float y = worldTransform.position.y;
                bool withinBounds = (y >= heightTotal && y <= heightTotal + floor.Height);
                return withinBounds;
            }

            // add height for the next iteration
            heightTotal += floor.Height;
        }

        Debug.LogWarning($"{worldTransform.name} is out of the vertical bounds of the motel!");
        return false;
    }

    /// <summary>
    /// Increments current floor index, so a different motel floor is displayed
    /// </summary>
    [Button]
    public void CycleCurrentFloor()
    {
        currentFloorIndex = (currentFloorIndex+1)% floorMaps.Count;
        mapDisplayImage.sprite = currentFloorMap.MapSprite;
    }

    #endregion

    /// <summary>
    /// Update the location of each customer.
    /// Running every frame because of how much customers will be moving. 
    /// This could technically be optimized because not EVERY guest needs to be updated EVERY frame, but come on, its 2026 its okay.
    /// </summary>
    void Update()
    {
        //mapDisplayImage.sprite = currentFloorMap.MapSprite;

        // Update Guests:
        ClearUnusedGuests();

        foreach (var guest in GuestAndEventManager.Instance.ActiveGuestsInScene)
        {
            UpdateIconOnMapDisplay(guest);
            AnimateGuest(guest);
        }
    }

    /// <summary>
    /// Updates the UI for one guest
    /// </summary>
    private void UpdateIconOnMapDisplay(GuestInteractable guest)
    {
        if ( ! guestIconInstances.ContainsKey(guest))
            AddGuestToDisplay(guest);

        var icon = guestIconInstances[guest];

        // Make guest invisible if they arent on current floor
        bool guestOnCurrentFloor = IsObjectWithinFloor(guest.transform, currentFloorMap);
        icon.color = guestOnCurrentFloor ? icon.color.WithAlpha(1) : icon.color.WithAlpha(0);

        if (!guestOnCurrentFloor) return;

        Vector2 scaledPosition = GetPositionScalar(guest.transform);
        Vector2 canvasPosition = GetCanvasPositionFromScaledGuestPosition(scaledPosition);

        icon.rectTransform.localPosition = canvasPosition;
    }

    /// <summary>
    /// Adds one guest to the display
    /// </summary>
    /// <param name="guestInteractable"></param>
    private void AddGuestToDisplay(GuestInteractable guestInteractable)
    {
        Image iconInstance = Instantiate(guestDisplayIconPrefab, parent:mapDisplayImage.transform);
        guestIconInstances.Add(guestInteractable, iconInstance);

        iconInstance.name = $"{guestInteractable.name} map icon";
        iconInstance.sprite = guestInteractable.MapSprite;
    }

    /// <summary>
    /// Deletes the icons of guests that no longer exist
    /// </summary>
    private void ClearUnusedGuests()
    {
        foreach(var guest_icon in guestIconInstances)
        {
            GuestInteractable guest = guest_icon.Key;
            if (guest == null || guest.gameObject == null)
            {
                guestIconInstances.RemoveAllInstancesWithValue(guest_icon.Value);
            }
        }
    }

    /// <summary>
    /// Does a lil scaling animation
    /// </summary>
    /// <param name="guest"></param>
    void AnimateGuest(GuestInteractable guest)
    {
        var icon = guestIconInstances[guest];
        float scale = StaticUtilities.SinRange(Time.time * animationSpeed, 1, animationMaxScale);
        icon.transform.localScale = Vector2.one * scale;
    }

    /// <summary>
    /// Returns guests position as a scalar vector (x and y are between 0-1)
    /// (0,0) is top right of map. (1,1) is bottom left of map.
    /// Position may be out of the 0-1 range if the guest is off of the map
    /// </summary>
    private Vector2 GetPositionScalar(Transform worldObject)
    {
        float x = StaticUtilities.InverseLerpUnclamped(topRightPosition3D.x, bottomLeftPosition3D.x, worldObject.position.x);
        float y = StaticUtilities.InverseLerpUnclamped(topRightPosition3D.z, bottomLeftPosition3D.z, worldObject.position.z);
        return new Vector2(x,y);
    }

    /// <summary>
    /// Given the guests position AS A SCALAR.
    /// Performs calculations to get where the guest would be on the canvas.
    /// Does not move the guest icon.
    /// </summary>
    private Vector2 GetCanvasPositionFromScaledGuestPosition(Vector2 scaledGuestPosition)
    {
        // guests can technically go off the map because i think its funny (also what else would happen if they went OOB, think about it)
        float x = Mathf.LerpUnclamped(topRightAnchor.localPosition.x, bottomLeftAnchor.localPosition.x, scaledGuestPosition.x);
        float y = Mathf.LerpUnclamped(topRightAnchor.localPosition.y, bottomLeftAnchor.localPosition.y, scaledGuestPosition.y);
        return new Vector2(x,y);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 transformedScale = new Vector3(MapSize.x, -1, MapSize.y);
        Vector3 transformedCenter = new Vector3(MapCenter.x, 0, MapCenter.y);
        Gizmos.color = Color.yellow;

        float heightTotal = mapFloorLevel;
        for (int i = 0; i < floorMaps.Count; i++)
        {
            MoontelMapFloor floor = floorMaps[i];
            float y = heightTotal + (floor.Height / 2);
            Gizmos.DrawWireCube(transformedCenter.WithY(y), transformedScale.WithY(floor.Height));

            // add height for the next iteration
            heightTotal += floor.Height;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(topRightPosition3D.WithY(-10), topRightPosition3D.WithY(20));
        Gizmos.DrawWireSphere(topRightAnchor.transform.position,0.05f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(bottomLeftPosition3D.WithY(-10), bottomLeftPosition3D.WithY(20));
        Gizmos.DrawWireSphere(bottomLeftAnchor.transform.position, 0.05f);
    }
}
