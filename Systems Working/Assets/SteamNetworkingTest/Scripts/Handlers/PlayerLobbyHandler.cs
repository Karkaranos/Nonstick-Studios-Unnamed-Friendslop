using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

namespace SteamTesting
{
    public class PlayerLobbyHandler : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnReadyStatusChanged))]
        public bool IsReady = false;
        public Button ReadyButton;
        public TextMeshProUGUI NameText;

        private void Start()
        {
            ReadyButton.interactable = isLocalPlayer;
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            ReadyButton.interactable = true;
            IsReady = false;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            LobbyUIManager.Instance.RegisterPlayer(this);
        }

        [Command]
        void CmdSetReady()
        {
            IsReady = !IsReady;
            OnReadyStatusChanged(!IsReady, IsReady);
        }

        public void OnReadyButtonClicked()
        {
            CmdSetReady();
        }

        void SetSelectedButtonColor(Color color)
        {
            ColorBlock cb = ReadyButton.colors;
            cb.normalColor = color;
            cb.selectedColor = color;
            cb.disabledColor = color;
            ReadyButton.colors = cb;
        }

        void OnReadyStatusChanged(bool oldValue, bool newValue)
        {
            if(NetworkServer.active)
            {
                LobbyUIManager.Instance.CheckAllPlayersReady();
            }

            if(IsReady)
            {
                SetSelectedButtonColor(Color.green);
            }
            else
            {
                SetSelectedButtonColor(Color.white);
            }
        }
    }
}