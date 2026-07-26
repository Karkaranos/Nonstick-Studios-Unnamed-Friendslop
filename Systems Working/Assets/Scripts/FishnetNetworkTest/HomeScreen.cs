using NaughtyAttributes;
using UnityEngine;
using System.Net.Sockets;
using System;
using FishNet.Managing;


public class HomeScreen : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject hostScreen;
    [SerializeField] private GameObject clientScreen;

    #endregion

    public void HostGame()
    {
        hostScreen.SetActive(true);
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
