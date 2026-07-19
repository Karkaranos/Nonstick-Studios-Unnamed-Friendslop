/*****************************************************************************
* File Name :         TetherManager.cs
* Author :            Toby Schamberger
* Creation Date :     7/19/26
*
* Brief Description : 
*****************************************************************************/

using NaughtyAttributes;
using UnityEngine;

public class TetherManager : Singleton<TetherManager>   
{
    [BoxGroup("Tether Settings")]
    public float MaxLengthToCreateNewTetherSegment = 15;
}
