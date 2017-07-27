using UnityEngine;
using TNet;
using System;
namespace VrNet.UILogic
{
    /// <summary>
    /// Create Dynamic Ui Item（Show/Hide）
    /// </summary>
    public class UIRecvDynamicObjects : TNBehaviour
    {
        [HideInInspector]
        public TweenPosition tween;
        //ScrollView
        [HideInInspector]
        public UIPanel target;   
        public GameObject prefab;
        public float rowHeight = 48f;
        private bool show = false;
        List<GameObject> currentDynamicObjs = new List<GameObject>();

        [HideInInspector]
        public UIButton ButtonInOut;
        //Controller Button show or hide
        [HideInInspector]
        public UIButton showOrHideAll;

		private bool bAllState = false;

        protected override void Awake()
        {
            InitUi();
        }

        void InitUi()
        {
           
            if (TNManager.isHosting)
            {
                GameObject obj = Instantiate(Resources.Load("UI_Scene")) as GameObject;
                TagEnum[] lists = obj.GetComponentsInChildren<TagEnum>();
                Debug.Log("TagEnumLength:" + lists.Length);
                for (int i = 0; i < lists.Length; i++)
                {

                    switch (lists[i].currentType)
                    {
                        case TagEnum.TagType.target:
                            target = lists[i].gameObject.GetComponent<UIPanel>();
                            break;
                        case TagEnum.TagType.Button:
                            showOrHideAll = lists[i].gameObject.GetComponent<UIButton>();

                            break;
                        case TagEnum.TagType.Tween:
                            tween = lists[i].gameObject.GetComponent<TweenPosition>();

                            break;
                        case TagEnum.TagType.TwwenButton:
                            ButtonInOut = lists[i].gameObject.GetComponent<UIButton>();
                            EventDelegate.Add(ButtonInOut.onClick, ShowHideContext);

                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                //Debug.Log("学生端 -----加载控制器");
                GameObject obj = Instantiate(Resources.Load("StudentController")) as GameObject;
            }
        }


        protected override void Start()
        {
            Messenger.AddListener<List<GameObject>,string>("RecvDynamicObjects", OnRecvObjects);
            Messenger.AddListener<string,bool>("DynamicObjSender", OnRecvToggle);

            Messenger.AddListener<string>("MoToPosSendToPlayController", MoToPosSendToPlayController);

            if (showOrHideAll!=null)
            {
                EventDelegate.Add(showOrHideAll.onClick, OnSetAllState);
            }
            
        }

		void OnSetAllState(){
			bAllState = !bAllState;

			Messenger.Broadcast<bool> ("BroadcastDynamicState", bAllState);
			tno.SendQuickly ("SwtichAll",Target.AllSaved,bAllState);
		}

        /// <summary>
        /// Recv Dynamic
        /// </summary>
        /// <param name="t"></param>
        /// <param name="s"></param>
        void OnRecvObjects(List<GameObject> t,string s)
        {      
            
            Debug.Log("DynamicRecv=>OnRecvObjects=>" + s+"=>t=>"+t.Count);
            if (TNManager.isHosting)
            {
                if (t.Count >= 1)
                {
                    showOrHideAll.gameObject.SetActive(true);

                }
                else
                {
                    if (showOrHideAll != null)
                    {
                        showOrHideAll.gameObject.SetActive(false);
                    }
                }
            }

            for (int i =0;i<t.Count;i++){
                if (TNManager.isHosting)
                {
                    GameObject go = NGUITools.AddChild(target.gameObject, prefab);
                    go.gameObject.GetComponentInChildren<UILabel>().text = t[i].gameObject.name;
                    go.gameObject.transform.localPosition = new Vector3(0f, 60f - (i-1) * rowHeight, 0f);
                    Debug.Log("go=>" + go.name);
                }   
			//Caching and Set Default false!
            currentDynamicObjs.Add(t[i].gameObject);
            t[i].gameObject.SetActive(false);
            }
        }

        void OnRecvToggle(string s, bool state)
        {
            for (int i = 0; i < currentDynamicObjs.Count; i++)
            {
                if (s == currentDynamicObjs[i].name)
                {
                    currentDynamicObjs[i].SetActive(state);
                    tno.SendQuickly("OnDynamicObjectSwtich", Target.AllSaved, s, state);
                }
            }
        }

        void MoToPosSendToPlayController(string posObj)
        {

            for (int i = 0; i < currentDynamicObjs.Count; i++)
            {
                if (posObj == currentDynamicObjs[i].name && currentDynamicObjs[i].activeSelf)
                {
                    
                    Messenger.Broadcast<Vector3>("MoveToPos", currentDynamicObjs[i].transform.position);
                    Debug.Log("MoToPosSendToPlayController");
                }
            }

        }

        [RFC]
        void OnDynamicObjectSwtich(string name, bool state)
        {
            for (int i = 0; i < currentDynamicObjs.Count; i++)
            {
                if (name == currentDynamicObjs[i].name)
                {
                    currentDynamicObjs[i].SetActive(state);
                    Debug.Log("RFC=>"+ currentDynamicObjs[i].name+"=="+state);
                }
            }
        }

		[RFC]
		void SwtichAll(bool state)
		{
			for (int i = 0; i < currentDynamicObjs.Count; i++)
			{
				currentDynamicObjs[i].SetActive(state);
				Debug.Log("RFC=>"+ currentDynamicObjs[i].name+"=="+state);
			}
		}
        
        public void ShowHideContext()
        {
            show = !show;

            if (show)
                tween.PlayForward();
            else
                tween.PlayReverse();
        }

    }
}
