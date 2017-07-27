using UnityEngine;
using System.Collections;
using TNet;
using System;
using System.Collections.Generic;
using VRTK.Highlighters;

public class UITeacherController : TNBehaviour {
    int channelID = 0;
	public GameObject UiTeacherController;

	public GameObject UiCharacterRotate;
	public UIToggle UiCharacterSH;

	public TweenPosition tw;
	private bool show = false;

	private UIScrollBar TeacherCharacterRotScrollBar;

	private GameObject MainPlayer;
	private GameObject MainPlayrBody;

    

	public  UILabel UiDynamicObjsInfo;
	public  GameObject UiDynamicObjsController;
	public  GameObject UiDynamicScrollView;
	public GameObject UiDynamicItemPrefab;
	public float rowHeight = 48f;

	string mDynamicPath = string.Empty;
    Dictionary<GameObject,Vector3> currentDynamicObjsPos = new Dictionary<GameObject,Vector3>();

	static int index =-1;

	//Items All Controller

	//public UIButton resetAll;
	public UIButton showOrHideAll;

	private bool bAllState = true;

	void Start () {
        //  lwl  4.13
        //highlighterOptions = new Dictionary<string, object>();
        //highlighterOptions.Add("resetMainTexture", true);

        StartCoroutine(GetPlayer (2f));

		Messenger.AddListener<string,bool> ("OnHandObjToggle",RecvHandObjAction);
		Messenger.AddListener<string> ("OnHandObjClick",RecvHandObjClick);
		Messenger.AddListener<string,int> ("OnRccCreatHandObj",RecvRccCreatHandObj);
        //lwl  4.13
        Messenger.AddListener<string>("MoveToObjPos", MoveToObjPos);
        Messenger.AddListener("LoadOnCompleted",OnLoadCompleted);

		if (TNManager.isHosting)
		{
			UiTeacherController.SetActive(true); 
			UiDynamicObjsController.SetActive (true);

			Invoke ("TryInit", 2f);

			//event		
			TeacherCharacterRotScrollBar = UiCharacterRotate.GetComponent<UIScrollBar>();
			EventDelegate.Add (TeacherCharacterRotScrollBar.onChange, TeacherCharacterRotate);
			EventDelegate.Add (UiCharacterSH.onChange, TeacherCharacterState);

			//EventDelegate.Add (resetAll.onClick, OnResetAll);
			EventDelegate.Add (showOrHideAll.onClick, OnSetAllState);
		}
        
    }

    /// <summary>
    /// Runtime Load all objects completed
    /// </summary>
    void OnLoadCompleted()
    {
        //Only Create On Hosting Player
        if (PlayerProfile.VUseHmd == "True"&&TNManager.isHosting)
        {
            //StartCoroutine(InitHmdControllerObjects());

            showOrHideAll.gameObject.SetActive(true);
            UiDynamicObjsController.SetActive(true);

            Debug.Log("UsingHmd=======OnLoadCompleted===>InitHmdControllerObjects......");
        }
    }

    void OnEnable()
    {
        TNManager.onPlayerJoin += OnNetworkPlayerJoin;
    }
    
    void OnDisable()
    {
        TNManager.onPlayerJoin -= OnNetworkPlayerJoin;
      
    }

    void OnNetworkPlayerJoin(int channelID, Player p)
    {
        if (TNManager.isHosting)
        {
            //OnResetAll();
            Debug.Log("PlayerJoin======================>jump reset........");
        }
    }


    public void OnResetAll(){
        GameObject t = null;
        Vector3 p = Vector3.zero;

        var e = currentDynamicObjsPos.GetEnumerator ();
        while (e.MoveNext()) {
            t = e.Current.Key;
            p = e.Current.Value;
            t.transform.position = p;
            t.GetComponent<Rigidbody>().useGravity = true;
        }
        
        e.Dispose();

        Debug.Log("Reset All HMD Controller Objects...");

        //RFC
        //if (TNManager.isHosting&&t!=null)
           // tno.SendQuickly ("OnResetHandObjAll",Target.AllSaved);
    }


    //public  void OnResetHandObjAll()
    //  {
    //      GameObject t = null;
    //      Vector3 p = Vector3.zero;

    //      var e = currentDynamicObjsPos.GetEnumerator();
    //      while (e.MoveNext())
    //      {
    //          t = e.Current.Key;
    //          p = e.Current.Value;
    //          t.transform.position = p;
    //          t.GetComponent<Rigidbody>().useGravity = true;
    //      }

    //      e.Dispose();

    //      Debug.Log("OnResetHandObjAll==>"+tno.ownerID);
    //  }


    /// <summary>
    /// 发送寻路信息 以及使物体高亮
    /// </summary>
    /// <param name="name"></param>
    void MoveToObjPos(string name)
    {
        GameObject t = null;
        var e = currentDynamicObjsPos.GetEnumerator();
        while (e.MoveNext())
        {
            if (name.Equals(e.Current.Key.name))
            {
                t = e.Current.Key;
            }
        }
        e.Dispose();
        Messenger.Broadcast<Vector3>("MoveToPos", t.transform.position);
        //u取消高亮
         // SerOutLineModle(name);
      //  if (TNManager.isHosting && t != null)
          //  tno.SendQuickly("SerOutLineModle", Target.AllSaved, name);
    }
    private Dictionary<string, object> highlighterOptions;
    private VRTK_BaseHighlighter currentOutLine = null;
    private float outlineTime = 0;
   // [RFC]
    void SetOutLineModle(string name)
    {
        GameObject obj = null;
        var e = currentDynamicObjsPos.GetEnumerator();
        while (e.MoveNext())
        {
            if (name.Equals(e.Current.Key.name))
            {
                obj = e.Current.Key;
            }
        }
        e.Dispose();
      SetObjOutLineH.GetInstance().SetHighlitghter(obj);
    }
    void OnSetAllState(){
		bAllState = !bAllState;

		GameObject t = null;
		var e = currentDynamicObjsPos.GetEnumerator ();
		while (e.MoveNext()) {
			t = e.Current.Key;
		}

		t.SetActive (bAllState);

		Messenger.Broadcast<bool> ("BroadcastHandObjsState", bAllState);

		//RFC
		if (TNManager.isHosting&&t!=null)
			tno.SendQuickly ("SetAllHandObjsState", Target.AllSaved,bAllState);
	}
    
	[RFC]
	void SetAllHandObjsState(bool state){

        GameObject t = null;
        var e = currentDynamicObjsPos.GetEnumerator();
        while (e.MoveNext())
        {
            t = e.Current.Key;
        }

        e.Dispose();
        if (t==null)
        
            return;
        
        t.SetActive(state);
    }

    void TryInit(){
		//if select using hmd hand controller call init hands objects
		if (PlayerProfile.VUseHmd == "True") {
            //Modeify 2017/3/3
            //StartCoroutine (InitHmdControllerObjects ());
            //resetAll.gameObject.SetActive (true);
            //showOrHideAll.gameObject.SetActive (true);
            //UiDynamicObjsController.SetActive (true);
            Debug.Log("%%%%%####%#%%%#%%#%%#%%#%%#%   useHand");
		} else {
			//resetAll.gameObject.SetActive (false);
			showOrHideAll.gameObject.SetActive (false);
			UiDynamicObjsController.SetActive (false);
		}

		StartCoroutine (InitHandUi (2f));
	}

	IEnumerator  InitHandUi(float t){
		yield return new WaitForSeconds (t);
		if (index <= 0) {
			//showOrHideAll.gameObject.SetActive (false);
			//resetAll.gameObject.SetActive (false);
			UiDynamicObjsController.SetActive (false);
		}
	}

	void RecvHandObjAction(string go,bool state){	
		Debug.Log ("Recv-=>" + go + "=>" + state);
		GameObject t = null;
		var e = currentDynamicObjsPos.GetEnumerator ();
		while (e.MoveNext()) {
			if (e.Current.Key.name == go)
				t = e.Current.Key;
		}

		t.SetActive (state);

		if (TNManager.isHosting&&t!=null)
			tno.SendQuickly ("SetDynamicObjState", Target.AllSaved, t.name, state);
	}

	///// <summary>
	///// Inits the hmd controller objects.
	///// Search Db DynamicPath and load path objects .
	///// </summary>
	///// <returns>The hmd controller objects.</returns>
	//IEnumerator InitHmdControllerObjects()
 //   {
        
 //       if (mDynamicPath == null || mDynamicPath == string.Empty || mDynamicPath == "Null")
	//		yield return null;
		
	//	var t = Resources.LoadAll (mDynamicPath);

	//	while (TNManager.isJoiningChannel) yield return null;
	//	if (channelID < 1) channelID = TNManager.lastChannelID;

	//	for (int i = 0; i < t.Length; i++) {

	//		var path = mDynamicPath + "/" + t [i].name ;

	//		//create and call rcc
	//		TNManager.Instantiate(channelID, "CreateDynamicObjAtPosition", path, false, Vector3.zero, Quaternion.identity);

	//	}
	//}

	void RecvHandObjClick(string go){
		GameObject t = null;
		var e = currentDynamicObjsPos.GetEnumerator ();
		while (e.MoveNext()) {
			if (e.Current.Key.name == go)
				t = e.Current.Key;
		}

		t.transform.position = currentDynamicObjsPos[t];
		t.GetComponent<Rigidbody> ().useGravity = true;
		if (TNManager.isHosting&&t!=null)
			tno.SendQuickly ("OnResetHandObj",Target.AllSaved,t.name,t.transform.position);
	}

    void OnApplicationQuit()
    {
        if (TNManager.isHosting)
            tno.SendQuickly("OnTeacherQuit", Target.All);
    }
    [RFC]
    void OnTeacherQuit()
    {
        Debug.Log("主机退出   学生端也退出");
        Application.Quit();
    }
	[RFC]
	void OnResetHandObj(string go,Vector3 p){
		GameObject t = null;
		var e = currentDynamicObjsPos.GetEnumerator ();
		while (e.MoveNext()) {
			if (e.Current.Key.name == go)
				t = e.Current.Key;
		}

		t.transform.position = p;
		t.GetComponent<Rigidbody> ().useGravity = true;
	}


	///// <summary>
	///// Creates the dynamic object at position.[rcc function name must be diff]
	///// </summary>
	///// <returns>The dynamic object at position.</returns>
	///// <param name="prefab">Prefab.</param>
	///// <param name="pos">Position.</param>
	///// <param name="q">Q.</param>
	//[RCC]
	//static GameObject CreateDynamicObjAtPosition (GameObject prefab,Vector3 pos,Quaternion q)
	//{
	//	// Instantiate the prefab
	//	GameObject go = prefab.Instantiate();

	//    go.layer = LayerMask.NameToLayer("CanTake");

	//    Debug.Log("Server?" + TNManager.isHosting);
         
 //       Debug.Log("Layer=======================>"+go.layer);

	//	go.GetComponent<TNObject> ().channelID = TNManager.lastChannelID;

	//	Debug.Log (TNManager.player.name+"=>"+go.name+"=>id-"+go.GetComponent<TNObject> ().channelID);

 //       //Send Message
 //       //Check GameObject Type
        
	//    if (go.GetComponent<CSyncObject>() != null)
	//    {
 //           index += 1;
 //           Messenger.Broadcast<string, int>("OnRccCreatHandObj", go.name, index);
 //       }
	 

 //       return go;
	//}

	void RecvRccCreatHandObj(string go,int i){
        var t = GameObject.Find(go);
        Debug.Log("如果是学生端则不需要添加界面物体");
        if (TNManager.isHosting)
        {
            //add hand obj to scrollview
            GameObject tt = NGUITools.AddChild(UiDynamicScrollView, UiDynamicItemPrefab);
            tt.gameObject.transform.localPosition = new Vector3(0f,  i * rowHeight, 0f);
           
            tt.gameObject.GetComponentInChildren<UILabel>().text = t.name;
            tt.gameObject.GetComponent<UIHandObjSender>().target = t;
        }
        Debug.Log("RecvRcc=>" + t + "---" + i + "--" + t.activeSelf);
        currentDynamicObjsPos.Add(t,t.transform.position);
	}


	[RFC]
	void SetDynamicObjState(string go,bool state){
		GameObject t = null;
		var e = currentDynamicObjsPos.GetEnumerator ();
		while (e.MoveNext()) {
			if (e.Current.Key.name == go)
				t = e.Current.Key;
		}
        if (t == null)
            return;
        Debug.Log("*********************SetDynamicObjState" + t.name);
        
        t.SetActive (state);
	}


	void UpdateDynamicInfo(){
        if (!TNManager.isHosting)
        {
            return;

        }
		if (PlayerPrefs.GetString ("Vr_Mulit_HmdHand") == "True")
			UiDynamicObjsInfo.text = string.Empty;
		else {
			UiDynamicObjsInfo.text = "\tNo Element!";
			//resetAll.gameObject.SetActive (false);
			showOrHideAll.gameObject.SetActive (false);
		}
	}

	void Update(){
		UpdateDynamicInfo ();
	}


	IEnumerator GetPlayer(float t)
	{
		yield return new WaitForSeconds(t);
		//GetHostPlayer
		MainPlayer = GameObject.FindGameObjectWithTag("Player");
		MainPlayrBody= GameObject.FindGameObjectWithTag("PlayerBody");
	}

	public void ShowHideContext()
	{
		show = !show;

		if (show)
			tw.PlayForward();
		else
			tw.PlayReverse();
    }


	void TeacherCharacterRotate(){	

		if (MainPlayer != null) {			
			MainPlayer.transform.rotation = Quaternion.Euler (new Vector3 (
				MainPlayer.transform.eulerAngles.x,
				MainPlayer.transform.eulerAngles.y+TeacherCharacterRotScrollBar.value*18f,
				MainPlayer.transform.eulerAngles.z));

			var angle = MainPlayer.transform.eulerAngles.y + TeacherCharacterRotScrollBar.value * 18f;

			tno.SendQuickly ("SetTeacherCharacterRot", Target.AllSaved, angle);
		}
	}

	[RFC]
	void SetTeacherCharacterRot(float f){
		if (MainPlayer != null) {
			MainPlayer.transform.rotation = Quaternion.Euler (new Vector3 (
				MainPlayer.transform.eulerAngles.x,
				f,
				MainPlayer.transform.eulerAngles.z));
		}
	}


	void TeacherCharacterState()
	{
		if (MainPlayrBody != null) {
			MainPlayrBody.SetActive (UiCharacterSH.value);
			tno.SendQuickly ("SetTeachrCharacterState",Target.AllSaved,UiCharacterSH.value);
		}
	}

	[RFC]
	void SetTeachrCharacterState(bool b){
		if (MainPlayrBody != null) {
			MainPlayrBody.SetActive (b);
		}
	}
}
