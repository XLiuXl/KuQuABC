using UnityEngine;
using TNet;

namespace VrNet.UICommon
{

    /// <summary>
    /// Networked chat logic. Takes care of sending and receiving of chat messages.
    /// </summary>

    [RequireComponent(typeof(TNObject))]
    public class UIGameChat : UIChat
    {
        TNObject tno;

        /// <summary>
        /// Sound to play when a new message arrives.
        /// </summary>

        public AudioClip notificationSound;

        /// <summary>
        /// If you want the chat window to only be shown in multiplayer games, set this to 'true'.
        /// </summary>

        public bool destroyIfOffline = false;
		[HideInInspector]
		/// <summary>
		/// The User input info.
		/// </summary>
		public string inputInfo;

        void OnEnable()
        {
            TNManager.onPlayerJoin += OnNetworkPlayerJoin;
            TNManager.onPlayerLeave += OnNetworkPlayerLeave;
        }

        void OnDisable()
        {
            TNManager.onPlayerJoin -= OnNetworkPlayerJoin;
            TNManager.onPlayerLeave -= OnNetworkPlayerLeave;
        }

        /// <summary>
        /// We want to listen to input field's events.
        /// </summary>

        void Start()
        {
            if (destroyIfOffline && !TNManager.isInChannel)
            {
                Destroy(gameObject);
            }
            else tno = GetComponent<TNObject>();
        }

        /// <summary>
        /// Send the chat message to everyone else.
        /// </summary>

        protected override void OnSubmit(string text)
        {
            tno.Send("OnChat", Target.All, TNManager.playerID, text);

			//Assgin text to inputInfo
			if(TNManager.isHosting)
				inputInfo = text;

            // Clear the input focus
            UIInput.current.isSelected = false;
        }

        [RFC]
        void OnChat(int playerID, string text)
        {
            Color color = Color.white;
            Player sender = TNManager.GetPlayer(playerID);

            if (sender != null)
            {
                // If the message was not sent by the player, color it differently and play a sound
                if (playerID != TNManager.playerID)
                    color = new Color(0.6f, 1.0f, 0f);

                // Embed the player's name into the message
                text = string.Format("[{0}]: {1}", sender.name, text);
            }
            Add(text, color);

            if (notificationSound != null)
                NGUITools.PlaySound(notificationSound);
        }

        void OnNetworkPlayerJoin(int channelID, Player p) { Add(p.name + " has joined the game."); }
        void OnNetworkPlayerLeave(int channelID, Player p) { Add(p.name + " has left the game."); }
    }
}
