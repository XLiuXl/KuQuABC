using UnityEngine;

namespace VrNet.Logic
{

    /// <summary>
    /// Checkbox controller for the "Upgrade" state in options.
    /// </summary>

    [RequireComponent(typeof(UIToggle))]
    public class UIOptionUpgrade : MonoBehaviour
    {
        public UILabel info;
        void OnClick() { info.text = Localization.Get("Upgrade Info"); }

        UIToggle mCheck;

        void Awake()
        {
            mCheck = GetComponent<UIToggle>();
            EventDelegate.Add(mCheck.onChange, SaveState);
        }

        void OnEnable() { mCheck.value = PlayerProfile.fullAccess; }
        void SaveState() { PlayerProfile.fullAccess = UIToggle.current.value; }
    }
}
