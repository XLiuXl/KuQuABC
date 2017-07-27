using UnityEngine;

namespace VrNet.UICommon
{

    /// <summary>
    /// This script is used in Options to give detailed information about the options the player is interacting with.
    /// </summary>

    [RequireComponent(typeof(UILabel))]
    public class UIOptionInfo : MonoBehaviour
    {
        void OnSelectionChange(string item)
        {
            UILabel lbl = GetComponent<UILabel>();
            lbl.text = Localization.Get(item + " Info");
        }
    }
}
