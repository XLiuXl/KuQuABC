using UnityEngine;
using System.Collections;
using VrNet.NetLogic;

namespace VrNet.UICommon
{
    public class UILAN : MonoBehaviour
    {
        
        public void OpenLan()
        {
            LobbyManager.EnableLAN();
        }

        public void CloseLan()
        {
            LobbyManager.DisableAll();
        }
    }

}