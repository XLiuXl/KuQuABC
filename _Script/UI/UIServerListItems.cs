using UnityEngine;
using System.Collections;
using TNet;
using VrNet.NetLogic;

namespace VrNet.UICommon
{
    public class UIServerListItems : MonoBehaviour
    {
        public UILabel nameLabel;
        public UILabel infoLabel;
        public UISprite background;

        ServerList.Entry mEntry;
        bool mIsDirty = true;
        UIButton mButton;
        
        public bool isValid { get; set; }

        public ServerList.Entry entry
        {
            get
            {
                return mEntry;
            }
            set
            {
                isValid = true;
                mEntry = value;
                Update();
            }
        }

        void OnEnable()
        {
            TNManager.onConnect += OnNetworkConnect;
        }

        void OnDisable()
        {
            TNManager.onConnect -= OnNetworkConnect;
        }

        void Awake() {
            mButton = GetComponent<UIButton>();
            EventDelegate.Add(mButton.onClick, ButtonOnClick);
        }

        public void MarkAsDirty() { mIsDirty = true; }
        
        void Update()
        {
            if (mIsDirty)
            {
                mIsDirty = false;
                if (mEntry == null) return;

                nameLabel.text = mEntry.name;
                infoLabel.text = Localization.Get("Players") + ": " + mEntry.playerCount;
            }
            mButton.isEnabled = !TNManager.isTryingToConnect;
        }

        /// <summary>
        /// Change the selection on click.
        /// </summary>

        void ButtonOnClick()
        {
            Debug.Log("UIServerListItems");
            if (!TNManager.isTryingToConnect)
            {
                if (TNManager.isConnected)
                {
                    TNManager.Disconnect();
                    Debug.Log("ServerItems=>Disconnect");
                }
                else
                {
                    TNManager.playerName = PlayerPrefs.GetString("vr_username");
                    TNManager.Connect(entry.externalAddress, entry.internalAddress);
                    Debug.Log("ServerItems=>connect....");
                    Debug.Log("externalAddress=>"+ entry.externalAddress);
                    Debug.Log("internalAddress=>" + entry.internalAddress);
                }
            }
        }

        void OnNetworkConnect(bool result, string msg)
        {
            if (result) GameManager.server = mEntry;
            //打开房间列表
            if (result)
            {
                Debug.Log("Login Lobby Server Sucess... ");
            }
        }
    }
}
