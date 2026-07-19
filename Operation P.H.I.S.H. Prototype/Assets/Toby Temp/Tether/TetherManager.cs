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

using UnityEngine;

public class TetherManager : Singleton<TetherManager>   
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
