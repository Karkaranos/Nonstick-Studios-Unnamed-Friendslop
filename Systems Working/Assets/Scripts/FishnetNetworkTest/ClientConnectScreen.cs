using UnityEngine;
using TMPro;
using FishNet.Managing;

public class ClientConnectScreen : MonoBehaviour
{
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private PasswordAuth authenticationObj;

    [SerializeField] private TMP_InputField addressField;
    [SerializeField] private TMP_InputField playerNameField;
    [SerializeField] private TMP_InputField passwordField;

    public void Connect()
    {
        authenticationObj.InputPassword = passwordField.text;
        ConnectionManager.Instance.ConnectToLobby(addressField.text);

        lobbyScreen.SetActive(true);
        gameObject.SetActive(false);
    }
}
