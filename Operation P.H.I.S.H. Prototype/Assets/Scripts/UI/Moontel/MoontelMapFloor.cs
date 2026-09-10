/*************************************************
Author Names : 		    Toby Schamberger
Date Created : 		    9/9/2026
Date Last Modified : 	9/9/2026

Brief Description : 	Data holder for each floor. Utilized by the guest map UI

External Resources :    	
***************************************************/

using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class MoontelMapFloor
{
    public float Height=5;
    [ShowAssetPreview(128,128)]
    public Sprite MapSprite;
}
