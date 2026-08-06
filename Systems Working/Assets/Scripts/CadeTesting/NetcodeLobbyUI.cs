/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    7/19/2026
Date Last Modified : 	7/19/2026
Brief Description : 	Generates a lobby passcode and disolas the player count
External Resources : 	
	***************************************************/
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System;
using System.Text;

public class NetcodeLobbyUI : NetworkBehaviour
{

    public NetworkVariable<int> playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<FixedString32Bytes> password = new NetworkVariable<FixedString32Bytes>("ABC123", NetworkVariableReadPermission.Everyone);
    [SerializeField] private TMP_Text playerCountDisplay;
    [SerializeField] private TMP_Text passwordDisplay;

    private void OnEnable()
    {
        CadePublicEvents.PlayerCountChanged += UpdatePlayerCount;
        CadePublicEvents.LobbyCreated += CreateLobbyCode;
    }

    private void OnDisable()
    {
        CadePublicEvents.PlayerCountChanged -= UpdatePlayerCount;
        CadePublicEvents.LobbyCreated -= CreateLobbyCode;
    }

    public void CreateLobbyCode()
    {
        password.Value = Encoding.UTF8.GetString(NetworkManager.Singleton.NetworkConfig.ConnectionData);
        passwordDisplay.text = $"Password: {password.Value.ToString()}";
    }

    public void UpdatePlayerCount()
    {
        if(IsServer)
        {
            playerCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
        }
        playerCountDisplay.text = $"Players: {playerCount.Value.ToString()}";
    }
}
