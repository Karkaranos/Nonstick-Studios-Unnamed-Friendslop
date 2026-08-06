/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    7/19/2026
Date Last Modified : 	7/19/2026
Brief Description : 	GPublic events for netcode
External Resources : 	
	***************************************************/
using System;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public static class CadePublicEvents
{
    public static Action PlayerCountChanged;
    public static Action LobbyCreated;
    public static Action CallPlayerName;
    public static Action<FixedString32Bytes> ReturnPlayerName;

    public static Action OnStartHost;
    public static Action OnStartClient;
    public static Action OnDisconnect;
    public static Action<FixedString32Bytes> OnUsernameChange;
}
