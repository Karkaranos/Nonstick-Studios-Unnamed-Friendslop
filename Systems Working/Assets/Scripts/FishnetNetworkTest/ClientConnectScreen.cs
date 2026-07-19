using UnityEngine;
using TMPro;
using FishNet.Managing;

public class ClientConnectScreen : MonoBehaviour
{
    [SerializeField] private GameObject lobbyScreen;

    [SerializeField] private TMP_InputField addressField;
    [SerializeField] private TMP_InputField playerNameField;
    [SerializeField] private TMP_InputField passwordField;

    public void ConnectToLobby()
    {
        string[] str = addressField.text.Split(':');
        string ipv4 = str[0];
        ushort port = ushort.Parse(str[1]);

        NetworkManager.Instances[0].ClientManager.StartConnection(ipv4, port);
        Debug.Log($"Connected to lobby at {addressField.text}");
        Debug.Log($"Player Name is {playerNameField.text}");

        lobbyScreen.SetActive(true);
        gameObject.SetActive(false);
    }
}
