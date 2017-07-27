using UnityEngine;
using System.Net;
using TNet;
using VrNet.UICommon;


namespace VrNet.UICommon
{

    /// <summary>
    /// Œ¥≤‚ ‘£°
    /// </summary>

    public class UIDirectConnect : MonoBehaviour
    {
        public UIButton button;
        public UILabel buttonText;
        public UILabel infoLabel;
        public UIInput serverAddress;
        public UILabel internalIP;
        public UILabel externalIP;
        public UIPanel channelListWindow;
        public GameObject clearButton;

        bool mConnecting = false;
        bool mIpResolved = false;
        IPAddress mLocal;
        IPAddress mExternal;

        void Start()
        {
            infoLabel.text = Localization.Get("Direct Note 2");
            internalIP.text = Localization.Get("Searching");
            externalIP.text = Localization.Get("Searching");
            UIEventListener.Get(button.gameObject).onClick = OnButtonClick;
            UIEventListener.Get(serverAddress.gameObject).onSelect = OnSelectInput;
            UIEventListener.Get(clearButton).onClick = OnClear;
            EventDelegate.Set(serverAddress.onSubmit, Connect);
            Tools.ResolveIPs(OnResolvedIPs);
            UpdateButtonText();
        }

        void OnClear(GameObject go)
        {
            serverAddress.value = "";
            UpdateButtonText();
        }

        void OnSelectInput(GameObject go, bool selected)
        {
            UpdateButtonText();
        }

        void OnResolvedIPs(IPAddress local, IPAddress ext)
        {
            mLocal = local;
            mExternal = ext;
            mIpResolved = true;
        }

        void Update()
        {
            if (mIpResolved)
            {
                internalIP.text = mLocal.ToString();
                externalIP.text = mExternal.ToString();
                mIpResolved = false;
            }
        }

        void OnEnable()
        {
            TNManager.onConnect += OnNetworkConnect;
            TNManager.Disconnect();
            UpdateButtonText();
        }

        void OnDisable()
        {
            TNManager.onConnect -= OnNetworkConnect;
        }

        void OnButtonClick(GameObject go)
        {
            if (TNServerInstance.isActive)
            {
                TNServerInstance.Stop();
                UpdateButtonText();
            }
            else if (!string.IsNullOrEmpty(serverAddress.value))
            {
                Connect(serverAddress.value);
            }
#if !UNITY_WEBPLAYER
            else
            {
                TNServerInstance.serverName = "Direct Server";

                if (TNServerInstance.Start(5127))
                {
                    TNManager.Connect("127.0.0.1", 5127);
                }
                else
                {
                    UIMessageBox.Show(Localization.Get("Unable to Start"), Localization.Get("Unable to Start Info"));
                }
                Connect(serverAddress.value);
                UpdateButtonText();
            }
#endif
        }

        void Connect() { Connect(UIInput.current.value); }

        void Connect(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                mConnecting = true;
                TNManager.Connect(val, 5127);
                UpdateButtonText();
            }
        }

        void OnNetworkConnect(bool success, string errmsg)
        {
            if (!success)
            {
                UIMessageBox.Show(Localization.Get("Unable to Connect"), errmsg);
            }

            mConnecting = false;
            UpdateButtonText();

            if (success)
            {
                UIWindow.Show(channelListWindow);
            }
        }

        void UpdateButtonText()
        {
            if (mConnecting)
            {
                button.isEnabled = false;
                buttonText.text = Localization.Get("Connecting");
            }
            else if (TNServerInstance.isActive)
            {
                button.isEnabled = true;
                buttonText.text = Localization.Get("Stop");
            }
#if UNITY_WEBPLAYER
		else if (string.IsNullOrEmpty(serverAddress.value))
		{
			buttonText.text = Localization.Get("Connect");
			button.isEnabled = false;
		}
#else
            else if (string.IsNullOrEmpty(serverAddress.value))
            {
                button.isEnabled = true;
                buttonText.text = Localization.Get("Start");
            }
#endif
            else
            {
                button.isEnabled = true;
                buttonText.text = Localization.Get("Connect");
            }
        }
    }
}
