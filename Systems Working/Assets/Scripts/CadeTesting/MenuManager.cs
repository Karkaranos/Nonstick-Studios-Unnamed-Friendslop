using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Collections;
using System.Collections.Generic;

public class MenuManager : NetworkBehaviour
{
    [SerializeField] private NetcodePasswordManager npm;

    private Dictionary<int, FixedString32Bytes> connectedPlayers = new Dictionary<int, FixedString32Bytes>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        CadePublicEvents.OnStartHost += StartHost;
        CadePublicEvents.OnStartClient += StartClient;
        CadePublicEvents.OnDisconnect += DisconnectClient;
        CadePublicEvents.OnUsernameChange += UsernameChange;
    }

    private void UsernameChange(FixedString32Bytes bytes)
    {
        throw new NotImplementedException();
    }

    private void DisconnectClient()
    {
        NetworkManager.Shutdown();
    }

    private void StartClient()
    {
        npm.TryToJoinLobby();
    }

    private void StartHost()
    {
        NetworkManager.StartHost();
        npm.HostLobby();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
