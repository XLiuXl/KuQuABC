using UnityEngine;
using System.Collections;
using TNet;

namespace NetVr.UICommon
{
    public class UICamView : MonoBehaviour
    {

        // Tween triggered on click
        public TweenPosition tween;



        bool mRebuild = true;
        bool mShown = false;

        void OnEnable()
        {
            TNManager.onJoinChannel += OnNetworkJoinChannel;
            TNManager.onPlayerJoin += OnNetworkPlayerJoin;
            TNManager.onPlayerLeave += OnNetworkPlayerLeave;
        }

        void OnDisable()
        {
            TNManager.onJoinChannel -= OnNetworkJoinChannel;
            TNManager.onPlayerJoin -= OnNetworkPlayerJoin;
            TNManager.onPlayerLeave -= OnNetworkPlayerLeave;
        }

        // Rebuild the list on any relevant network message
        void OnNetworkJoinChannel(int channelID, bool success, string message) { mRebuild = true; }
        void OnNetworkPlayerJoin(int channelID, Player p) { mRebuild = true; }
        void OnNetworkPlayerLeave(int channelID, Player p) { mRebuild = true; }

        /// <summary>
        /// Rebuild the list.
        /// </summary>

        void Update()
        {
            if (mRebuild)
            {
                mRebuild = false;
                GameObject parent = tween.gameObject;
              

                // Make sure that the tweened object has a collider
                NGUITools.AddWidgetCollider(parent);
                UIEventListener.Get(parent).onClick = ToggleList;

                // Refresh everything immediately so that there is no visible delay
                parent.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
                UIPanel pnl = NGUITools.FindInParents<UIPanel>(gameObject);
                if (pnl != null) pnl.Refresh();
            }
        }

        void ToggleList(GameObject go)
        {
            mShown = !mShown;
            tween.Toggle();
        }
    
    }
}
