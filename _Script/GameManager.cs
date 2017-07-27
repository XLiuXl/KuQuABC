using System;
using UnityEngine;
using TNet;
using VrNet.Common;
using VrNet.UICommon;

namespace VrNet.NetLogic
{

 
    public class GameManager : TNBehaviour
    {
        static GameManager mInstance;

        public enum GameType
        {
            None,
            SinglePlayer,
            Multiplayer,
        }

        /// <summary>
        /// Random number generator to be used throughout the code.
        /// </summary>

        static public RandomGenerator random = new RandomGenerator();

        /// <summary>
        /// Type of the game being played.
        /// </summary>

        static public GameType gameType = GameType.None;

        /// <summary>
        /// Custom data associated with multiplayer games.
        /// </summary>

        static public string gameData = "";

        /// <summary>
        /// Chosen server, if any.
        /// </summary>

        static public ServerList.Entry server;

        /// <summary>
        /// Whether tooltips are going to be shown.
        /// </summary>

        static public bool enableTooltips = true;

        /// <summary>
        /// 6h limit. 60*60*6
        /// </summary>

        static public float timeLimit = 21600f;

        /// <summary>
        /// Current elapsed game time. This value is synchronized with all connected players.
        /// </summary>

        static public float gameTime = 0f;

        // Number of times the game has been paused
        static int mPause = 0;
        static float mTargetTimeScale = 1f;
        static float mNextChannelUpdate = 0f;

        /// <summary>
        /// PlayerPrefs-saved time limit.
        /// </summary>

        static float savedTimeLimit
        {
            get
            {
                string s = PlayerPrefs.GetString("Vr_Mulit_TimeLimit", "21600");
                float val = 60f;
                float.TryParse(s, out val);
                return val * 60f*6f;//60*60*6 => 6H
            }
        }

        /// <summary>
        /// Pause the game.
        /// </summary>

        static public void Pause()
        {
            ++mPause;
            mTargetTimeScale = 0f;
        }

        /// <summary>
        /// Unpause the game.
        /// </summary>

        static public void Unpause()
        {
            if (--mPause < 1)
            {
                mTargetTimeScale = 1f;
                mPause = 0;
            }
        }

        /// <summary>
        /// Start a new single player game.
        /// </summary>

        static public void StartSingleGame()
        {
            if (mInstance != null)
            {
                gameType = GameType.SinglePlayer;
                timeLimit = savedTimeLimit;
                gameTime = 0f;


                TNManager.Disconnect();
                TNManager.playerName = PlayerProfile.playerName;

                string sceneName = PlayerPrefs.GetString("Vr_Single_Scene");

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			Application.LoadLevel(sceneName);
#else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
#endif
            }
        }

        /// <summary>
        /// Start a new single player game.
        /// </summary>

        static public void StartMultiGame()
        {
            if (mInstance != null && TNManager.isConnected)
            {
                gameType = GameType.Multiplayer;
                timeLimit = savedTimeLimit;
                gameTime = 0f;

                TNManager.playerName = PlayerProfile.playerName;
                
                string sceneName = PlayerProfile.mCommonLoaderScene;
                string players = PlayerProfile.VMaxPlayer;
                string pw = PlayerProfile.VUserPw;
                
                //Set Room Password
                TNManager.CreateChannel(sceneName, false, Convert.ToInt32(players), pw);

                Debug.Log("StartMulitGame=>"+pw);
                Debug.Log("Players=>"+players);
            }
        }

        /// <summary>
        /// End the game in progress.
        /// </summary>

        static public void EndGame()
        {
            if (TNManager.isHosting)
            {
                EndNow();

                if (mInstance != null)
                {
                    mInstance.tno.Send("OnEndGame", Target.Others);
                    TNManager.CloseChannel();
                }
            }
        }

        /// <summary>
        /// Forfeit the current game.
        /// </summary>

        static public void Forfeit()
        {
            if (TNManager.isInChannel) TNManager.LeaveChannel();
            else if (gameType != GameType.None) EndNow();
            else LoadMenu();
        }

        /// <summary>
        /// Stop time when the game ends.
        /// </summary>

        [RFC]
        void OnEndGame() { EndNow(); }

        /// <summary>
        /// Immediately end the game.
        /// </summary>

        static void EndNow()
        {
            if (gameType != GameType.None)
            {
                gameType = GameType.None;
                Time.timeScale = 0f;
                mTargetTimeScale = 0f;
                mPause = 0;

                LoadMenu();
            }
        }

        /// <summary>
        /// Load the main menu, ending the game in progress.
        /// </summary>

        public static void LoadMenu()
        {
            gameType = GameType.None;
            Time.timeScale = 1f;
            mTargetTimeScale = 1f;
            mPause = 0;

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		if (Application.loadedLevelName != "Main")
#else
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Main")
#endif
            {
                if (mInstance != null)
                {
                    Destroy(mInstance);
                    mInstance = null;
                }
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			Application.LoadLevel("Main");
#else
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
#endif
            }
        }

        float mNextSync = 0f;

        /// <summary>
        /// Register various network callback functions.
        /// </summary>

          void OnEnable()
        {
            //base.OnEnable();
            TNManager.onJoinChannel += OnNetworkJoinChannel;
            TNManager.onLeaveChannel += OnNetworkLeaveChannel;
            TNManager.onPlayerJoin += OnNetworkPlayerJoin;
        }

        /// <summary>
        /// Unregister network callback functions
        /// </summary>

        void OnDisable()
        {
            TNManager.onJoinChannel -= OnNetworkJoinChannel;
            TNManager.onLeaveChannel -= OnNetworkLeaveChannel;
            TNManager.onPlayerJoin -= OnNetworkPlayerJoin;
        }

        /// <summary>
        /// Set the instance reference.
        /// </summary>
        protected override void Awake()
        {
            
            if (mInstance == null)
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
                Application.targetFrameRate = PlayerProfile.powerSavingMode ? 120 : 200;
                gameTime = 0f;
                mNextSync = 5f;
                mInstance = this;
            }
            else Destroy(this);
        }

        /// <summary>
        /// Clear the instance reference.
        /// </summary>

        void OnDestroy() { if (mInstance == this) mInstance = null; }

        /// <summary>
        /// Keep track of game time.
        /// </summary>
        void Update()
        {
            OnEscClick();

            if (gameType == GameType.Multiplayer)
            {
                //Debug.Log("RealTime==" + RealTime.time);
                
                if (mNextChannelUpdate < RealTime.time)
                {
                    UpdateChannelData();
                    //Debug.LogFormat("mNextChannelUpdate==" + mNextChannelUpdate);
                }
            }
            else
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, mTargetTimeScale, 8f * RealTime.deltaTime);
            }

            gameTime += Time.deltaTime;

            // Only the host can end the match
            if (TNManager.isHosting && !TNManager.isJoiningChannel)
            {
                // Periodically update everyone's time just to ensure that everyone runs the same
                if (TNManager.isConnected && mNextSync < RealTime.time)
                {
                    mNextSync = RealTime.time + 5f;
                    tno.Send("SetTime", Target.Others, gameTime);
                }

                // Once the timer limit has been reached, end the game
                if (timeLimit > 0f && gameTime > timeLimit) EndGame();
            }
        }



        float time = 0;
        float coolingTime = 0.5f;
        bool isESCClick = false;
        void OnEscClick()
        {
            if (isESCClick)
            {
                time += Time.deltaTime;
                if (time>=coolingTime)
                {
                    time = 0;
                    isESCClick = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                WhenEscButtonDoubleClick();
            }
        }
        void WhenEscButtonDoubleClick()
        {
            if (time<=coolingTime&&isESCClick)
            {
                Debug.Log("quit");
                Application.Quit();
                EndGame();
            }
            isESCClick = true;
        }
        [RFC]
        void SetTime(float val) { gameTime = val; }

        /// <summary>
        /// Send a channel data update. This updates the data visible on the channel list.
        /// When modifying this function, be sure to modify UIRoomListItem.Set function as well.
        /// </summary>

        void UpdateChannelData()
        {
            mNextChannelUpdate = RealTime.time +10f;

            //Debug.LogFormat("mNextChannelUpdate==" + mNextChannelUpdate);

            // Only the host should be doing this
            if (TNManager.isHosting)
            {

                //Game Verison
                TNManager.SetChannelData("Build", UIVersion.buildID);
                //Game name
                string gameName = PlayerPrefs.GetString("GameName", "").Trim();

                //Scene name
                TNManager.SetChannelData("SceneName", PlayerProfile.VScene);
                //Room name
                //string currentGameName = string.IsNullOrEmpty(gameName) ? TNManager.playerName + "'s game" : gameName;
                string currentGameName = string.IsNullOrEmpty(gameName) ? TNManager.playerName + "'s game" : gameName;
                TNManager.SetChannelData("GameName", currentGameName);

                //Load progress
                float progress = Mathf.RoundToInt(Mathf.Clamp01(gameTime / timeLimit) * 100f);
                TNManager.SetChannelData("Progress", progress + "%");

                //Class level
                TNManager.SetChannelData("ClassLevel", PlayerProfile.VLevel);

                //Class title
                TNManager.SetChannelData("ClassTitle",PlayerProfile.VTitle);

                //Player count
                TNManager.SetChannelData("GamePlayers",PlayerProfile.VMaxPlayer);
                
                //Room password
                TNManager.SetChannelData("RoomPw", PlayerProfile.VUserPw);
                
				//hmd controller
				TNManager.SetChannelData("UseHmdHand",PlayerProfile.VUseHmd);

                //Elements
                TNManager.SetChannelData("ClassElements",PlayerProfile.VElements);
                //Pos
                TNManager.SetChannelData("PlayerPos",PlayerProfile.VPos);
                
                Debug.Log("Build=>" + UIVersion.buildID);
                Debug.Log("GameName=>" + currentGameName);
                Debug.Log("SceneName=>" + PlayerProfile.VScene);
				Debug.Log("HmdController=>" + PlayerProfile.VUseHmd);
            }
        }

        /// <summary>
        /// Start the game as soon as we join the channel.
        /// </summary>

        void OnNetworkJoinChannel(int channelID, bool success, string message)
        {

            if (success)
            {
                Debug.Log("OnNetworkJoinChannel Sucess.................");

                gameType = GameType.Multiplayer;
                mNextChannelUpdate = RealTime.time + 10f;

                if (TNManager.isHosting)
                {
                    gameTime = 0f;
                    UpdateChannelData();

                    // This is where you would set the player limit for your game, allowing others to join.
                    TNManager.SetPlayerLimit(Convert.ToInt32(PlayerProfile.VMaxPlayer));
                }
            }
            else
            {
                gameType = GameType.None;
                UIMessageBox.Show(Localization.Get("Unable to Join"), message);
            }
        }

        /// <summary>
        /// Return to the menu when left the channel.
        /// </summary>

        void OnNetworkLeaveChannel(int channelID) { EndNow(); }

        /// <summary>
        /// New player joins -- add him to the list.
        /// </summary>

        void OnNetworkPlayerJoin(int channelID, Player p)
        {
            if (TNManager.isHosting)
            {
                tno.Send("OnWelcome", p, gameTime, timeLimit);
                //Debug.LogFormat("Message:{0}==>Player:{1}==>GameTime:{1}==>TimeLimit:{2}", "OnWelcome", p.name, gameTime, timeLimit);
            }
        }

        [RFC]
        void OnWelcome(float val, float limit)
        {
            timeLimit = limit;
            gameTime = val;
        }
    }
}
