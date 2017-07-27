using UnityEngine;

namespace VrNet.UICommon
{

    /// <summary>
    /// Toggles between full screen and not full screen
    /// </summary>

    public class UIFullScreen : MonoBehaviour
    {
        public UIButton button;
        public UILabel label;

        void Awake()
        {
#if UNITY_IPHONE || UNITY_ANDROID
		if (button != null) button.gameObject.SetActive(false);
		Destroy(this);
#else
            if (button != null) UIEventListener.Get(button.gameObject).onClick = OnClickBtn;
#endif
        }

        void Start()
        {
            if (PlayerPrefs.GetInt("FS", 0) == 1) Set(true);
            else if (label != null) label.text = Localization.Get("FullScreen");
        }

        void OnClick() { Set(!Screen.fullScreen); }

        void OnClickBtn(GameObject go) { Set(!Screen.fullScreen); }

        void Set(bool full)
        {
            if (Screen.fullScreen == full) return;

            if (full)
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                if (label != null) label.text = Localization.Get("Windowed");
                PlayerPrefs.SetInt("FS", 1);
            }
            else
            {
                Screen.SetResolution(1920, 1080, false);
                if (label != null) label.text = Localization.Get("FullScreen");
                PlayerPrefs.SetInt("FS", 0);
            }
        }
    }
}
