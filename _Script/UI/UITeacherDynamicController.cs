using UnityEngine;
using System.Collections;
using TNet;

public class UITeacherDynamicController : TNBehaviour {

	public TweenPosition tw;
	private bool show = false;

	public UIButton uiSwtichArea;

    public UIScrollBar uiDynamicObjsScrollBar;

	public  GameObject UiDynamicObjsController;
	private bool bDynamicObjsShow = true;

	private bool useHmd = false;
	private bool isSmall =false;
	private bool bSwtich = true;

	/// <summary>
	/// The ground small
	/// </summary>
	private GameObject groundSmall;
	/// <summary>
	/// The ground big.
	/// </summary>
	private GameObject groundBig;
	/// <summary>
	/// The ground tearcher.
	/// </summary>
	private GameObject groundTearcher;

	protected override void Start () {

		if (PlayerProfile.VUseHmd == "True")
			useHmd = true;
		else
			useHmd = false;

		if (PlayerProfile.VSmall == "True")
			isSmall = true;
		else
			isSmall = false;


        Init();
		if(useHmd)StartCoroutine (GetGrounds (3f));
        

	}
    void Init()
    {
        if (TNManager.isHosting)
        {
            tw.gameObject.SetActive(true);
            EventDelegate.Add(uiSwtichArea.onClick, OnSwtichArea);
        }
    }
	IEnumerator GetGrounds(float t)
	{
		yield return new WaitForSeconds(t);
		//Ground
		groundBig = GameObject.FindGameObjectWithTag("GroundBig");
		groundSmall = GameObject.FindGameObjectWithTag("GroundSmall");
		groundTearcher = GameObject.FindGameObjectWithTag ("GroundTeacher");

        Debug.Log("groundBig=>"+groundBig);
        Debug.Log("groundSmall=>"+groundSmall);
		Debug.Log("GroundTeacher=>"+groundTearcher);
		if (useHmd&&groundSmall!=null)
			groundSmall.SetActive (false);
	}

	void OnSwtichArea(){
		bSwtich = !bSwtich;

        if (bSwtich)
        {
            if (groundSmall != null) groundSmall.SetActive(true);
			if(groundTearcher != null)groundTearcher.SetActive(false);
			if (groundBig != null)
				groundBig.SetActive (false);
			tno.SendQuickly("SetGrounds", Target.AllSaved, true, false,false);
        }
        else
        {
			if (groundTearcher != null) groundTearcher.SetActive(true);
            if (groundSmall != null) groundSmall.SetActive(false);
			if (groundBig != null)
				groundBig.SetActive (true);
			tno.SendQuickly("SetGrounds", Target.AllSaved, false, true,true);
        }
    }

	[RFC]
	void SetGrounds(bool bSmallState,bool bTeacherState,bool bBigState){
        if (groundSmall != null) groundSmall.SetActive(bSmallState);
		if (groundTearcher != null) groundTearcher.SetActive(bTeacherState);
		if (groundBig != null)
			groundBig.SetActive (bBigState);
    }
	
	public void ShowHideContext()
	{
		show = !show;

		if (show)
			tw.PlayForward();
		else
			tw.PlayReverse();
	}

	public void ShowOrHideUiPanel(){
		if (tno.isMine) {
			bDynamicObjsShow = !bDynamicObjsShow;
            if (bDynamicObjsShow) {
                UiDynamicObjsController.SetActive(true);
                uiDynamicObjsScrollBar.value = 0f;
            }
			else
				UiDynamicObjsController.SetActive (false);
		}
	}


}
