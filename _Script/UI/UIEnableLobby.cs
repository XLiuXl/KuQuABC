using UnityEngine;

namespace VrNet.NetLogic
{

    /// <summary>
    /// This script makes it possible to enable and disable Lobby Clients as the player navigates through the menu.
    /// It's necessary because only one lobby client should update the list of known servers at a time, which means
    /// that only one lobby client should be active. So if a TCP lobby client is active for Internet play, and
    /// the player goes to the LAN menu, which uses a UDP-based lobby, the TCP lobby must be turned off first.
    /// </summary>

    public class UIEnableLobby : MonoBehaviour
    {
        public enum GameType
        {
            Single,
            Internet,
            LAN,
        }

        public GameType type = GameType.Single;

        void OnClick()
        {
            if (type == GameType.LAN)
            {
                LobbyManager.EnableLAN();
            }
            else if (type == GameType.Internet)
            {
                LobbyManager.EnableInternet();
            }
            else
            {
                LobbyManager.DisableAll();
            }
        }
    }
}
