using UnityEngine;
using System.Collections;
using TNet;
using VrNet.NetLogic;

namespace NetVr.UICommon
{
    public class UIPlayButton : MonoBehaviour
    {

        public enum Type
        {
            None,
            Single,
            Multi,
        }

        static public Type choice = Type.None;

        /// <summary>
        /// 游戏类型（学习类型）
        /// </summary>

        public Type type = Type.Single;
        

        public bool startOnClick = false;
        
        public bool disableInWebPlayer = false;

        /// <summary>
        /// multiplayer 需要网络支持
        /// </summary>

        public bool requireWifi = false;

        public Type chosenType { get { return (type != Type.None) ? type : choice; } }

        UIButton mButton;
        bool mForceDisable = false;

        void Awake()
        {
            mButton = GetComponent<UIButton>();
#if UNITY_WEBPLAYER || UNITY_FLASH
		if (disableInWebPlayer)
		{
			mButton.isEnabled = false;
			enabled = false;
		}
#endif
        }

        void OnEnable()
        {
            mForceDisable = (requireWifi && !PlayerProfile.allowedToAccessInternet);
        }

        void Update()
        {
            if (mForceDisable) mButton.isEnabled = false;
            else mButton.isEnabled = (!startOnClick || ((TNManager.isConnected || (int)chosenType < (int)Type.Multi)));
        }

        void OnClick()
        {
            if (startOnClick)
            {
                switch (chosenType)
                {
                    case Type.Single:
                        GameManager.StartSingleGame();
                        break;

                    case Type.Multi:
                        GameManager.StartMultiGame();
                        break;
#if UNITY_EDITOR
                    default:
                        Debug.Log("No game type selected");
                        break;

#endif
                }

                this.gameObject.GetComponent<UIButton>().isEnabled = false;
                enabled = false;
            }
            else choice = type;
        }
    }
}
