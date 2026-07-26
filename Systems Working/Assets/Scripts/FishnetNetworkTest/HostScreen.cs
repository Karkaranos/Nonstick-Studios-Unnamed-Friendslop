using TMPro;
using UnityEngine;

public class HostScreen : MonoBehaviour
{
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private PasswordAuth passwordAuth;

    /// <summary>
    /// Starts a server with the password input in the password field.
    /// </summary>
    public void StartServer()
    {
        passwordAuth.SetPassword(passwordField.text);
        ConnectionManager.Instance.HostGame();

        lobbyScreen.SetActive(true);
        gameObject.SetActive(false);
    }
}
