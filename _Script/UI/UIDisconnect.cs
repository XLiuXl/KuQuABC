using UnityEngine;
using TNet;

namespace NetVr.UICommon
{
    [RequireComponent(typeof(UIButton))]
    public class UIDisconnect : MonoBehaviour
    {
        void OnClick()
        {
            TNManager.Disconnect();
        }
    }
}
