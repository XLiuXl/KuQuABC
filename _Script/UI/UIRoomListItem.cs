using UnityEngine;
using System.Collections;
using TNet;
using VrNet.UICommon;

namespace VrNet.NetLogic
{
    public class UIRoomListItem : MonoBehaviour
    {
        public UILabel titleLabel;
        public UILabel playerLabel;
        public UILabel descriptionLabel;

        [HideInInspector]
        public int id;
        [HideInInspector]
        public bool isValid = false;

        string mSceneName;
        UIButton mButton;
        Color mDefaultColor = Color.white;
        int mServerVersion = 0;
        bool mPass = false;

        void OnEnable()
        {
            TNManager.onSetChannelData += OnNetworkSetChannelData;
        }

        void OnDisable()
        {
            TNManager.onSetChannelData -= OnNetworkSetChannelData;
        }

        void Awake()
        {
            mButton = GetComponent<UIButton>();
            mDefaultColor = descriptionLabel.color;
            EventDelegate.Add(mButton.onClick, ButtonOnClick);
        }

        //void Update()
        //{
        //    if (mPass) GetComponent<BoxCollider>().enabled = true;
        //}

        /// <summary>
        /// 加入房间
        /// </summary>

        void ButtonOnClick()
        {
            if (TNManager.isConnected)
            {
                if (mPass)
                {
                    //if has password show password windows
                    UIPasswordWindow.Show(id, mSceneName);

                    Debug.Log("Show Pw Windows...");
                }
                else
                {
                    //join room
                    mButton.isEnabled = false;
                    GameManager.gameType = GameManager.GameType.Multiplayer;
                    TNManager.JoinChannel(id, mSceneName, false, 1, null);
                }
            }
        }

        /// <summary>
        /// Set the visible channel data.
        /// When modifying this function, be sure to modify GameManager.UpdateChannelData as well.
        /// </summary>

        public void Set(Channel.Info info)
        {
            mPass = info.hasPassword;

            Debug.Log("mPass=>"+mPass);

            Set(info.data);

            mSceneName = info.level;

            Debug.Log("mScene=>"+mSceneName);

            playerLabel.text = info.players + "/" + info.limit;

            if (mServerVersion == UIVersion.buildID)
            {
                mButton.isEnabled = (info.players < info.limit);
            }
            else
            {
                mButton.isEnabled = false;
            }

            isValid = true;
        }

        /// <summary>
        /// Sets any DataNode information for the channel.  Servers may periodically update this information
        /// with the SetChannelData command.
        /// </summary>

        void Set(DataNode node)
        {
            titleLabel.text = node.GetChild<string>("GameName");
            mServerVersion = node.GetChild<int>("Build");

            if (mServerVersion == UIVersion.buildID)
            {
                // Progress percentage comes last
                descriptionLabel.text = string.Format("{0} - {1}", titleLabel.text, node.GetChild<string>("Progress"));
                descriptionLabel.color = mPass ? Color.red : mDefaultColor;
            }
            else
            {
                // Version mismatch
                descriptionLabel.text = string.Format(Localization.Get("Version"), mServerVersion);
                descriptionLabel.color = Color.red;
            }

            string classLevel = node.GetChild<string>("ClassLevel");
            string sceneName = node.GetChild<string>("SceneName");
            string players = node.GetChild<string>("GamePlayers");
            string classTitle = node.GetChild<string>("ClassTitle");
			string hmdHandController = node.GetChild<string> ("UseHmdHand");
            string playerPos = node.GetChild<string>("PlayerPos");
            string elements = node.GetChild<string>("ClassElements");

			Debug.Log ("Node->HmdHandController=>"+hmdHandController);

            //Set Title(UIJoinSelecter base on this search db!)
            PlayerPrefs.SetString(PlayerProfile.mTitle, classTitle);
            PlayerProfile.VTitle = classTitle;
			//Set hmd controller
			PlayerPrefs.SetString(PlayerProfile.mUseHmd, hmdHandController);
            PlayerProfile.VUseHmd = hmdHandController;
            //Set SceneName
            PlayerPrefs.SetString(PlayerProfile.mScene,sceneName);
            PlayerProfile.VScene = sceneName;
            //Set Level
            PlayerPrefs.SetString(PlayerProfile.mLevel,classLevel);
            PlayerProfile.VLevel = classLevel;
            //Set Pos
            PlayerPrefs.SetString(PlayerProfile.mPos,playerPos);
            PlayerProfile.VPos = playerPos;
            //Set Elements
            PlayerPrefs.SetString(PlayerProfile.mElements, elements);
            PlayerProfile.VElements = elements;
            //Set Players
            PlayerPrefs.SetString(PlayerProfile.mMaxPlayer, players);
            PlayerProfile.VMaxPlayer = players;



            Debug.Log("ClassLevel=>"+classLevel);
            Debug.Log("ClassTitle=>" + classTitle);
            Debug.Log("SceneName=>" + sceneName);
            Debug.Log("Players=>" + PlayerProfile.VMaxPlayer);
			Debug.Log("Prefs->HmdHandController=>" + hmdHandController);
        }

        /// <summary>
        /// When a SetChannelData event is triggered, update the listbox with the new information.
        /// </summary>

        void OnNetworkSetChannelData(Channel ch, string path, DataNode node)
        {
            Set(node);
            Debug.Log("SetChannelData Event......");
        }
    }
}
