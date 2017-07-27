using UnityEngine;
using TNet;
using VrNet.UICommon;

namespace  VrNet.LoginLogic
{
    public class UIWinsSelector : MonoBehaviour
    {
        static bool mFirstTime = true;

        public UIPanel mainMenu;
        public UIPanel gameType;
        public UIPanel roomSelection;
        public UIPanel newRoom;

        void OnEnable()
        {
            TNManager.onConnect += OnNetworkConnect;
        }
        void OnDisable()
        {
            TNManager.onConnect -= OnNetworkConnect;
        }      
        void Start()
        {
            if (!mFirstTime)
            {
                if (TNManager.isConnected)
                {
                    UIWindow.Show(gameType);
                    UIWindow.Show(roomSelection);
                }
                else
                {
                    UIWindow.Show(gameType);
                    UIWindow.Show(newRoom);
                }
            }
            mFirstTime = false;
        }
        void OnNetworkConnect(bool success, string msg)
        {
            if (success && roomSelection != null)
                UIWindow.Show(roomSelection);
        }
    }
}
