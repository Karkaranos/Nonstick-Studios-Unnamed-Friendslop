/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    7/19/2026
Date Last Modified : 	7/19/2026
Brief Description : 	Handles a networked main menu with passwords for Netcode
External Resources : 	https://www.youtube.com/watch?v=Pe2LVZGTK20
                        https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.7/manual/basics/connection-approval.html
	***************************************************/
using System;
using System.Text;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Multiplayer;

public class NetcodePasswordManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] public  Button hostButton;
    [SerializeField] public Button joinButton;
    [SerializeField] public TMP_InputField setName;
    [SerializeField] public Button quit;
    [SerializeField] private GameObject passwordInputScreen;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField usernameField;

    [SerializeField] private string password;
    private string username;

    public void CreateLobbyCode()
    {
        string allowedChar = "ABCDEFGHIJKLMNOPQRSTUBWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        password = "";
        for (int i = 0; i < 6; i++)
        {
            password += allowedChar[UnityEngine.Random.Range(0, allowedChar.Length)];
        }
    }

    public void HostLobby()
    {
        CreateLobbyCode();
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(password);
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        CadePublicEvents.LobbyCreated?.Invoke();
        CadePublicEvents.PlayerCountChanged?.Invoke();
        transform.gameObject.SetActive(false);

    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientID = request.ClientNetworkId;
        string extractedPassword = Encoding.UTF8.GetString(request.Payload);


        Debug.Log("Extracted: " + extractedPassword + ". Provided: " + passwordField.text + ".");

        // If the passwords match, the player is approved to join
        response.Approved = true; //(extractedPassword == passwordField.text);
        response.CreatePlayerObject = true;

        response.PlayerPrefabHash = null;

        response.Position = new Vector3(0f,1.7f,0f);
        response.Rotation = Quaternion.identity;

        response.Reason = "Invalid password";

        response.Pending = false;
    }

    public void UpdateUsername(string s)
    {
        username = s;
    }

    /// <summary>
    /// Open the password entry screen
    /// </summary>
    public void TryToJoinLobby()
    {
        passwordField.ActivateInputField();

        passwordInputScreen.SetActive(true);
        mainMenuButtons.SetActive(false);
    }

    /// <summary>
    /// Return to the main menu from the password entry
    /// </summary>
    public void ReturnToMenu()
    {
        passwordField.text = "Enter Password";
        passwordField.DeactivateInputField();

        passwordInputScreen.SetActive(false);
        mainMenuButtons.SetActive(true);
    }

    public void TryPassword()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(passwordField.text);
        if(NetworkManager.Singleton.StartClient())
        {
            gameObject.SetActive(false);
        }


    }

    /// <summary>
    /// Returns the saved player name
    /// </summary>
    /// <returns></returns>
    public string ReturnPlayerName()
    {
        return username;
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }

}
