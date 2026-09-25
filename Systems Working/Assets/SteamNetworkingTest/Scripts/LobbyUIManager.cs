using UnityEngine;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace SteamTesting
{
    public class LobbyUIManager : NetworkBehaviour
    {
        public static LobbyUIManager Instance;

        public Transform PlayerListParent;
        public List<TextMeshProUGUI> PlayerNameTexts = new List<TextMeshProUGUI>();
        public List<PlayerLobbyHandler> PlayerLobbyHandlers = new List<PlayerLobbyHandler>();
        public Button PlayGameButton;

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

        void Start()
        {
            PlayGameButton.interactable = false;
        }

        public void UpdatePlayerLobbyUI()
        {

        }

        public void OnPlayButtonClicked()
        {

        }

        public void RegisterPlayer(PlayerLobbyHandler player)
        {

        }

        [Server]
        public void CheckAllPlayersReady()
        {

        }

        [ClientRpc]
        void RpcSetPlayerButtonInteractable(bool truthStatus)
        {

        }

        private IEnumerator RetryUpdate()
        {
            yield return new WaitForSeconds(1f);
            UpdatePlayerLobbyUI();
        }
    }
}
