using UnityEngine;
using System.Collections;
using VrNet.NetLogic;

namespace NetVr.UICommon
{
    public class UISwtichLobby : MonoBehaviour
    {
        public enum GameType
        {
            Single,
            Internet,
            LAN,
        }
        void Awake()
       {
            EventDelegate.Add(GetComponent<UIButton>().onClick, LobbyManagerClick);
        }
        public GameType type = GameType.Single;

     public   void LobbyManagerClick()
        {
            if (type == GameType.LAN)
            {
                LobbyManager.EnableLAN();
                Debug.Log("LAN=>"+"Enable Lan......");
            }
            else if (type == GameType.Internet)
            {
                LobbyManager.EnableInternet();
                //Debug.Log("Internet=>" + "Enable Internet......");
            }
            else
            {
                LobbyManager.DisableAll();
            }
        }
    }

}