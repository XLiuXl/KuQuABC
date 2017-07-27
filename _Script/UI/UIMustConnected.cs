using UnityEngine;
using System.Collections;
using TNet;

namespace NetVr.UICommon
{
    [RequireComponent(typeof(UIButton))]
    public class UIMustConnected : MonoBehaviour
    {
        UIButton mButton;

        void Awake()
        {
            mButton = GetComponent<UIButton>();
        }

        void Update()
        {
            mButton.isEnabled = TNManager.isConnected;
        }
    }
}
