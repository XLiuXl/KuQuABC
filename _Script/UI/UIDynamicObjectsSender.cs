using UnityEngine;
using System.Collections;

namespace VrNet.UILogic
{
    public class UIDynamicObjectsSender : MonoBehaviour
    {
        public UILabel mObjectName;
        private bool mState = false;
        public GameObject PosButton = null;
      
        void Start()
        {
           UIEventListener.Get(gameObject).onClick = OnToggle;
            //  lwl  4.13
            UIEventListener.Get(PosButton).onDoubleClick = DoubleClick;
            Messenger.AddListener<bool>("BroadcastDynamicState",OnSet);
		 
		   Debug.Log("OnToggle=>......");      
        }
        void DoubleClick(GameObject go)
        {

            Messenger.Broadcast<string>("MoToPosSendToPlayController", mObjectName.text);
        }
        void OnSet(bool state){
			gameObject.GetComponent<UIToggle> ().value = state;
			mState = state;
		}

        void OnToggle(GameObject go)
        {
            mState = !mState;
            Messenger.Broadcast<string, bool>("DynamicObjSender", mObjectName.text, mState);
            Debug.Log("Broadcast =>" + mObjectName.text + "---" + mState);

        }
    }
}
