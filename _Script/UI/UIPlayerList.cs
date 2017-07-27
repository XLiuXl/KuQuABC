using UnityEngine;
using TNet;
using VRTK;

namespace VrNet.UICommon
{

    /// <summary>
    /// This script creates a visible list of players that are currently present in the game.
    /// </summary>

	public class UIPlayerList : TNBehaviour
    {


		// Tween triggered on click
        public TweenPosition tween;

        // Player entry prefab
		public GameObject prefab;

        // How far spaced should each prefab be from one another
        public float rowHeight = 44f;

        // Instantiated prefabs
        List<UIPlayerName> mPlayers = new List<UIPlayerName>();
        bool mRebuild = true;
        bool mShown = false;


        bool mMoveable = false;
		bool mFollow = false;

		void Start(){

			Messenger.AddListener<Player,bool> ("OnFollow", OnMoveable);
		}

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
        void OnNetworkPlayerLeave(int channelID, Player p) { 
			mRebuild = true; 
			//RemovePlayerMessage
			//Messenger.Broadcast<string>("RemovePlayer",p.name);
		}

        /// <summary>
        /// Rebuild the list.
        /// </summary>

        void Update()
        {
			if (mRebuild)
            {
                mRebuild = false;
                if (tween==null)
                {
                    return;
                }
                GameObject parent = tween.gameObject;

                // Clear the previous list
                for (int i = 0; i < mPlayers.size; ++i)
                {
                    UIPlayerName p = mPlayers[i];
                    Destroy(p.gameObject);
                }
                mPlayers.Clear();

                // Add the player
                AddPlayer(TNManager.player);

                // Add other players
                for (int i = 0; i < TNManager.players.size; ++i)
                    AddPlayer(TNManager.players[i]);

                // Reposition all children so that they seem to grow from the left side of the screen
                float offset = (mPlayers.size - 1) * 0.5f * rowHeight;

				//fixed all clent controll ui
				Vector3 pos = Vector3.zero;

				if (tno.isMine)
					pos = Vector3.zero;
				else
					pos = new Vector3 (-9999f, 0f, 0f);


                for (int i = 0; i < mPlayers.size; ++i)
                {
                    UIPlayerName pn = mPlayers[i];
					pn.transform.localPosition = new Vector3(pos.x, Mathf.RoundToInt(offset), 0f);
                    offset -= rowHeight;
                }

                // Make sure that the tweened object has a collider
                NGUITools.AddWidgetCollider(parent);
                UIEventListener.Get(parent).onClick = ToggleList;

                // Refresh everything immediately so that there is no visible delay
                parent.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);
                UIPanel pnl = NGUITools.FindInParents<UIPanel>(gameObject);
                if (pnl != null) pnl.Refresh();
            }
        }

        /// <summary>
        /// Add a new player UI entry for the specified network player.
        /// </summary>

        void AddPlayer(Player p)
        {
            UIPlayerName pn = GetPlayerEntry(p);

            if (pn == null)
            {				
				
				GameObject go = NGUITools.AddChild(tween.gameObject, prefab);
                pn = go.GetComponent<UIPlayerName>();

				pn.player = p;
                pn.UpdateInfo(mShown);
                mPlayers.Add(pn);
                InitPlayerName(pn);
            }
        }
        void InitPlayerName(UIPlayerName  pn)
        {
            Debug.Log("MarkFollow::::     " + pn.label.text + "     " + TNManager.GetHost(TNManager.lastChannelID).name);
            if (TNManager.GetHost(TNManager.lastChannelID).name.Equals(pn.label.text)&&PlayerProfile.VUseHmd.Equals("True"))
            {
                pn.MarkFollow.SetActive(false);

            }
            else if (PlayerProfile.VUseHmd.Equals("False"))
                pn.MarkFollow.SetActive(false);
        }

		void OnMoveable(Player name,bool state)
		{
			Debug.Log ("Find=>"+name+"   "+name.name+"=="+state);
			tno.Send ("OnSetMoveable",Target.OthersSaved,name.name,state);
		}
        

		[RFC]
		void OnSetMoveable(string name,bool state)
		{
			GameObject go = null;
			if (name != string.Empty && name != null) {
                Debug.Log("========================================================================="+name );
				go = GameObject.Find (name);    
				if (go != null) {

					Debug.Log("=============================GO NAME======================================="+go.name );

                    var con = go.GetComponentInChildren<VrPlayerController>();

                    if (con == null) return;

                    con.isTeacherNeedFollow = state;
                    
                    //?
					VRTK_Pointer[] v = go.transform.parent.transform.parent.GetComponentsInChildren<VRTK_Pointer> ();

                    if (v.Length == 0) return;

					for (int i = 0; i < v.Length; i++) {

						Debug.LogFormat ("Find Vr Pointer:{0}",v[i].gameObject.name);

						if (v [i] != null)v [i].enableTeleport = !state;
					}

					//GameObject handLeft = GameObject.Find(name + "/" + "Controller (left)");

                    //Debug.Log("Handleft:" + handLeft);

                    //if (handLeft != null)
                    //{
                        //handLeft.GetComponent<VRTK_Pointer>().enableTeleport = !state;
                    //}


                    //GameObject handright = GameObject.Find(name + "/" + "Controller (right)");
                    //if (handright!=null)
                    //{
                        //handright.GetComponent<VRTK_Pointer>().enableTeleport = !state;
                    //}

                    //Debug.Log("Handright:" + handright);
                }
            }
		}


        /// <summary>
        /// Find an existing player entry.
        /// </summary>

        UIPlayerName GetPlayerEntry(Player p)
        {
            for (int i = 0; i < mPlayers.size; ++i)
            {
                UIPlayerName pn = mPlayers[i];
                if (pn.player == p) return pn;
            }
            return null;
        }


        void ToggleList(GameObject go)
        {
            mShown = !mShown;

            if (mShown)
            {
                //Show ControlMoveable
                for (int i = 0; i < mPlayers.size; ++i)
                {
					UIPlayerName pn = mPlayers [i];
                }
            }
            else
            {
                //Hide ControlMovable
                for (int i = 0; i < mPlayers.size; ++i)
                {
                    UIPlayerName pn = mPlayers[i];
                }
            }
            

            tween.Toggle();

            for (int i = 0; i < mPlayers.size; ++i)
            {
                UIPlayerName pn = mPlayers[i];
                pn.UpdateInfo(mShown);
            }
        }
    }
}
