using Mirror;
using UnityEngine;
using Steamworks;

namespace SteamTesting
{
    public class SteamLobby : NetworkBehaviour
    {
        public static SteamLobby Instance;
        public GameObject HostButton = null;
        public ulong LobbyID;
        public NetworkManager NetworkManagerObj;
        public PanelSwapper PanelSwapperObj;
        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;
        protected Callback<LobbyChatUpdate_t> lobbyChatUpdate;

        private const string HostAddressKey = "HostAddress";

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else if(Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            NetworkManagerObj = GetComponent<NetworkManager>();

            if(!SteamManager.Initialized)
            {
                Debug.LogError("Steam is not initialized. Run game in Steam environment.");
                return;
            }

            PanelSwapperObj.gameObject.SetActive(true);
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        }

        public void HostLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, NetworkManagerObj.maxConnections);
        }

        void OnLobbyCreated(LobbyCreated_t callback)
        {
            if(callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError($"Failed to create a lobby: {callback.m_eResult}");
                return;
            }

            Debug.Log($"Lobby created. Lobby ID: {callback.m_ulSteamIDLobby}");
            NetworkManagerObj.StartHost();

            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
            LobbyID = callback.m_ulSteamIDLobby;
        }

        void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            Debug.Log($"Join request received for lobby: {callback.m_steamIDLobby}");

            if(NetworkClient.isConnected || NetworkClient.active)
            {
                Debug.Log("NetworkClient is active or connected. Disconnecting before joining new lobby");
                NetworkManager.singleton.StopClient();
                NetworkClient.Shutdown();
            }

            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        void OnLobbyEntered(LobbyEnter_t callback)
        {
            if(NetworkServer.active)
            {
                Debug.Log("Already in a lobby as a host. Ignoring join request.");
                return;
            }

            LobbyID = callback.m_ulSteamIDLobby;
            string _hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
            NetworkManagerObj.networkAddress = _hostAddress;
            Debug.Log($"Entered lobby: {callback.m_ulSteamIDLobby}");
            NetworkManagerObj.StartClient();
            PanelSwapperObj.SwapPanel("LobbyPanel");
        }

        void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {

        }
    }
}
