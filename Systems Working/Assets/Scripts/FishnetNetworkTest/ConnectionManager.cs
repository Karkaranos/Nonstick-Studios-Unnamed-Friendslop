using FishNet;
using FishNet.Connection;
using FishNet.Object;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ConnectionManager : Singleton<ConnectionManager>
{
    [Header("Networking Values")]
    [SerializeField] private bool showManualNetworkSettings;
    [SerializeField, ShowIf("showManualNetworkSettings")] private string ipv4;
    [SerializeField, ShowIf("showManualNetworkSettings")] private ushort port;

    /// <summary>
    /// Hosts a game on the ipv4 and port specified in the variables section
    /// </summary>
    public void HostGame()
    {
        //Checks to see if the set port is open to host the game
        UdpClient udpClient = new UdpClient();

        try
        {
            udpClient.Connect(ipv4, port);
            Debug.Log($"Port {port} at address {ipv4} is open");
            udpClient.Close();
            udpClient.Dispose();
        }
        catch (Exception)
        {
            Debug.Log($"Port {port} at address {ipv4} is closed");
            udpClient.Close();
            udpClient.Dispose();
            return;
        }

        //Starts a hosted game
        InstanceFinder.ServerManager.StartConnection(port);
        InstanceFinder.ClientManager.StartConnection();
        Debug.Log($"Host Server Started on {ipv4}:{port}");
    }

    /// <summary>
    /// Connects to a lobby as a client.
    /// </summary>
    /// <param name="connectionAddress"> The address to connect to, formatted ipv4:port </param>
    public void ConnectToLobby(string connectionAddress)
    {
        string[] str = connectionAddress.Split(':');
        string ipv4 = str[0];
        ushort port = ushort.Parse(str[1]);

        InstanceFinder.ClientManager.StartConnection(ipv4, port);
        Debug.Log($"Connected to lobby at {str[0]}:{str[1]}");
    }

    /// <summary>
    /// Disconnects a given client from the server.
    /// </summary>
    /// <param name="clientId"> The id of the client to be disconnected </param>
    /// <param name="immediately"> Whether the server should wait to disconnect the client until it is finished sending data </param>
    public void DisconnectClient(int clientId, bool immediately)
    {
        if (!InstanceFinder.IsServerStarted)
            return;

        InstanceFinder.ServerManager.Clients[clientId].Disconnect(immediately);
        Debug.Log($"Client {clientId} disconnected; IMMEDIATE = {immediately}");
    }

    #region Player Spawning

    /// <summary>
    /// Spawns a network player prefab into the scene
    /// </summary>
    /// <param name="playerPrefab"> The player prefab </param>
    /// <param name="owner"> The client/server that owns the player </param>
    public void SpawnNetworkPlayer(NetworkObject playerPrefab, NetworkConnection owner)
    {
        Debug.Log("SPAWN PLAYER");

        NetworkObject obj = Instantiate(playerPrefab);
        InstanceFinder.ServerManager.Spawn(obj, owner, gameObject.scene);

        Debug.Log($"Player spawned for {owner.ClientId}");
    }


    /// <summary>
    /// Spawns a network player prefab into the scene
    /// </summary>
    /// <param name="playerPrefab"> The player prefab </param>
    /// <param name="owner"> The client/server that owns the player </param>
    /// <param name="position"> The position to spawn the prefab at </param>
    public void SpawnNetworkPlayer(NetworkObject playerPrefab, NetworkConnection owner, Transform position)
    {
        Debug.Log("SPAWN PLAYER");

        NetworkObject obj = Instantiate(playerPrefab);
        InstanceFinder.ServerManager.Spawn(obj, owner, gameObject.scene);

        Debug.Log($"Player spawned for {owner.ClientId}");
    }

    #endregion
}
