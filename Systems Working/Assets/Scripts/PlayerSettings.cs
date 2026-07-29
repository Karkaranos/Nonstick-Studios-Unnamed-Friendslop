/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    7/29/2026
Date Last Modified : 	7/29/2026
Brief Description : 	Controls player visuals
External Resources : 	
	***************************************************/
using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private TMP_Text nameText;
    private NetworkVariable<FixedString32Bytes> networkPlayerName = new NetworkVariable<FixedString32Bytes>("Player 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        networkPlayerName.Value =  FindFirstObjectByType<NetcodePasswordManager>(findObjectsInactive:FindObjectsInactive.Include).ReturnPlayerName();
        nameText.text = networkPlayerName.Value.ToString();
    }

}
