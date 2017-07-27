using UnityEngine;
using System.Collections;
using TNet;

namespace VrNet.NetLogic
{
    public class LobbyManager : MonoBehaviour
    {

        static LobbyManager mInst;

        public TNUdpLobbyClient lan;
        public TNUdpLobbyClient internet;

        void Start() { mInst = this; }

        static public void DisableAll()
        {
            if (mInst != null)
            {
                mInst.internet.enabled = false;
                mInst.lan.enabled = false;
            }
        }

        static public void EnableInternet()
        {
            if (mInst != null)
            {
                mInst.internet.enabled = true;
                mInst.lan.enabled = false;
            }
        }

        static public void EnableLAN()
        {
            if (mInst != null)
            {
                mInst.internet.enabled = false;
                mInst.lan.enabled = true;
            }
        }
    }
}
