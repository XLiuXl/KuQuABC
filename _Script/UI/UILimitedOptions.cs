using UnityEngine;
using System.Collections;
using VrNet.UICommon;

namespace VrNet.UICommon
{
    public enum LimitedOptionType
    {
        None,
        Player,
        Subject,
        Level,
        Subject_Level,
    }
    [RequireComponent(typeof(UIPopupList))]
    public class UILimitedOptions : MonoBehaviour
    {
        /// <summary>
        /// Register the selection change listener.
        /// </summary>
        public string LevelName = "";
        /// <summary>
        /// Key in PlayerPrefs where the last selection will be saved so that it can be automatically restored the next time you run the game.
        /// Make it unique and something simple, such as "Difficulty".
        /// </summary>

        public string keyName = "Key Name";
        public LimitedOptionType currentOptionType = LimitedOptionType.None;
        /// <summary>
        /// List of valid choices that will be valid regardless of whether the game is full or not.
        /// </summary>

        public string[] validChoices;

        UIPopupList mList;

        /// <summary>
        /// Check to see if the specified choice is present in the list of valid choices.
        /// </summary>

        bool IsValid(string choice)
        {
            for (int i = 0; i < validChoices.Length; ++i)
                if (validChoices[i] == choice)
                    return true;
            return false;
        }
        void Awake()
        {

            mList = GetComponent<UIPopupList>();
        }
      

        /// <summary>
        /// Load the last selection.
        /// </summary>

        void OnEnable()
        {
            string s = PlayerPrefs.GetString(keyName);
            if (!string.IsNullOrEmpty(s))
                mList.value = s;

        }

        public void SaveTitle()
        {
           
            GetComponent<UIPopupList>().value= GetComponent<UILimitedOptions>().validChoices[0];
        }
        /// <summary>
        /// Validate the selection.
        /// </summary>

        void OnSelection()
        {
            if (PlayerProfile.fullAccess || IsValid(UIPopupList.current.value))
            {
                // The selection is valid -- save it
                PlayerPrefs.SetString(keyName, UIPopupList.current.value);
                Debug.Log("Saved=>" + keyName + "==>" + UIPopupList.current.value);
            }
            else
            {
                // The selection is not valid. Change the selection to a valid one and show the upgrade window.
                mList.value = validChoices[validChoices.Length - 1];
                UIUpgradeWindow.Show();
            }
        }
    }
}
