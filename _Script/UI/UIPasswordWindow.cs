using UnityEngine;
using TNet;
using VrNet.NetLogic;

namespace VrNet.UICommon
{

    /// <summary>
    /// 玩家输入密码加入房间.
    /// </summary>

    public class UIPasswordWindow : MonoBehaviour
    {
        public UIPanel panel;
        public UIInput input;
        public GameObject button;

        static UIPasswordWindow mInst;
        static int mId = 0;
        static string mMap = "";

        void Awake() { mInst = this; }
        void OnDestroy() { if (mInst == this) mInst = null; }

        static public void Show(int id, string mapName)
        {
            if (mInst != null)
            {
                mId = id;
                mMap = mapName;
                UIWindow.Show(mInst.panel);
                EventDelegate.Set(mInst.input.onSubmit, mInst.OnSubmit);
                mInst.input.isSelected = true;
                UIEventListener.Get(mInst.button).onClick = mInst.Click;

                Debug.Log("Ex Show Func....");
            }
        }
        private bool isJoin = false;
        void Click(GameObject go)
        {
      
            
            GameManager.gameType = GameManager.GameType.Multiplayer;
            TNManager.JoinChannel(mId, mMap, false, 1, input.value);
            go.gameObject.GetComponent<UIButton>().enabled = false;
            //Debug.Log(mId + "-"+mMap + "-"+ input.value);
           
        }

        void OnSubmit()
        {
            if (isJoin)
                return;

            isJoin = true;
            GameManager.gameType = GameManager.GameType.Multiplayer;
            TNManager.JoinChannel(mId, mMap, false, 1, UIInput.current.value);
        }
    }

}