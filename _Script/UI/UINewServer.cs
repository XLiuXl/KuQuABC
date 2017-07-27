using UnityEngine;
using System.Collections;
using TNet;

namespace  NetVr.UICommon
{
    public class UINewServer : MonoBehaviour
    {
        public UIButton button;
        public UILabel buttonText;
        public UIInput serverName;

        void Start()
        {
            UIEventListener.Get(button.gameObject).onClick = OnButtonClick;
            EventDelegate.Set(serverName.onSubmit, OnNameChange);
        }

        void OnEnable()
        {
            UpdateButtonText();
        }

        void OnButtonClick(GameObject go)
        {
            if (!TNServerInstance.isActive)
            {
                TNServerInstance.serverName = string.IsNullOrEmpty(serverName.value) ? "LAN Server" : serverName.value;
                TNServerInstance.Start(5127, 0, 5128, null);
                UpdateButtonText();
            }
            else
            {
                TNServerInstance.Stop();
                UpdateButtonText();
            }
        }

        void OnNameChange()
        {
            TNServerInstance.serverName = string.IsNullOrEmpty(UIInput.current.value) ? "LAN Server" : UIInput.current.value;
        }

        void UpdateButtonText()
        {
            buttonText.text = Localization.Get(TNServerInstance.isActive ? "Stop" : "Start");
        }
    }
}
