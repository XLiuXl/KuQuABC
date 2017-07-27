using UnityEngine;
using System.Collections;

namespace VrNet.LoginLogic
{
    [RequireComponent(typeof(UIToggle))]
    public class UISettingLanguage : MonoBehaviour
    {

        void OnClick()
        {
            EventDelegate.Add(mCheck.onChange, SaveState);
        }

        UIToggle mCheck;

        void Awake() { mCheck = GetComponent<UIToggle>(); }

        void OnEnable()
        {
            EventDelegate.Add(mCheck.onChange, SaveState);
            mCheck.value = (Localization.language == "Chinese");
        }

        void OnDestroy()
        {
            EventDelegate.Remove(mCheck.onChange, SaveState);
        }

        void SaveState()
        {
            Localization.language = UIToggle.current.value ? "Chinese" : "English";
        }
    }
}
