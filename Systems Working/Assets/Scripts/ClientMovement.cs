/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    7/17/2026
Date Last Modified : 	7/17/2026
Brief Description : 	Dictates whether movement is enabled or disabled
External Resources : 	https://www.youtube.com/watch?v=kVt0I6zZsf0
	***************************************************/
using Unity.Netcode;
using UnityEngine;

public class ClientMovement : NetworkBehaviour
{
    [SerializeField] private PlayerBehavior pb;
    private void Awake()
    {
        pb.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            pb.enabled = true;
        }
    }
}
