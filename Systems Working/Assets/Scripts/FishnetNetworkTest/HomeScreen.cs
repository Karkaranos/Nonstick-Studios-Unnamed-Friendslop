using NaughtyAttributes;
using UnityEngine;
using System.Net.Sockets;
using System;
using FishNet.Managing;


public class HomeScreen : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private GameObject clientScreen;

    [Header("Networking Values")]
    [SerializeField] private bool showManualNetworkSettings;
    [SerializeField, ShowIf("showManualNetworkSettings")] private string ipv4;
    [SerializeField, ShowIf("showManualNetworkSettings")] private ushort port;

    #endregion

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
        catch(Exception)
        {
            Debug.Log($"Port {port} at address {ipv4} is closed");
            udpClient.Close();
            udpClient.Dispose();
            return;
        }

        //Starts a hosted game
        NetworkManager.Instances[0].ServerManager.StartConnection(port);
        Debug.Log($"Host Server Started on {ipv4}:{port}");

        lobbyScreen.SetActive(true);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Opens the client screen
    /// </summary>
    public void JoinGame()
    {
        clientScreen.SetActive(true);
        gameObject.SetActive(false);
    }
}
