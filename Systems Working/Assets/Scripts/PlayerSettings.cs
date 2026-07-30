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
    private string playerName;
   //private NetworkVariable<FixedString32Bytes> networkPlayerName = new NetworkVariable<FixedString32Bytes>("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {

        //ChangeUsernameServerRpc();
        playerName = FindFirstObjectByType<NetcodePasswordManager>(findObjectsInactive: FindObjectsInactive.Include).ReturnPlayerName();
        nameText.text = playerName;


        //if(IsOwner)
        //{

        //    networkPlayerName.Value = FindFirstObjectByType<NetcodePasswordManager>(findObjectsInactive: FindObjectsInactive.Include).ReturnPlayerName();
        //    //    Debug.Log(OwnerClientId + " name is " + networkPlayerName.Value.ToString() + " and expected is " + FindFirstObjectByType<NetcodePasswordManager>(findObjectsInactive: FindObjectsInactive.Include).ReturnPlayerName());
        //    //    nameText.text = networkPlayerName.Value.ToString();
        //    nameText.text = networkPlayerName.Value.ToString();
        //}
    }


    private void Update()
    {
        //Debug.Log($"{OwnerClientId} is named {networkPlayerName.Value.ToString()}");
        if(!IsOwner)
        {
            return;
        }

        //if(nameText.text != networkPlayerName.Value.ToString())
        //{
        //    nameText.text = networkPlayerName.Value.ToString();
        //}
    }

    //[ServerRpc]
    //private void ChangeUsernameServerRpc()
    //{
        
    //    networkPlayerName.Value = FindFirstObjectByType<NetcodePasswordManager>(findObjectsInactive: FindObjectsInactive.Include).ReturnPlayerName();
    //    Debug.Log(OwnerClientId + " name is " + networkPlayerName.Value.ToString() + " and expected is " + FindFirstObjectByType<NetcodePasswordManager>(findObjectsInactive: FindObjectsInactive.Include).ReturnPlayerName());
    //    nameText.text = networkPlayerName.Value.ToString();
    //}

}
