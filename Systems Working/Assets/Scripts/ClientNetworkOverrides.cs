/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    7/18/2026
Date Last Modified : 	7/18/2026
Brief Description : 	Overrides for Client Transform
External Resources : 	https://www.youtube.com/watch?v=kVt0I6zZsf0
	***************************************************/
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkOverrides : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
