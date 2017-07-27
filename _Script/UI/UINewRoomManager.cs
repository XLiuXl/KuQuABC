using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class UINewRoomManager : MonoBehaviour {

    public GameObject mLvDropdown;
    public GameObject dropdownParent;
    public GameObject dropdownPrefab;
    private int mClassLvCount = 0;
    private Dictionary<int, GameObject> cacheChapterItem = new Dictionary<int, GameObject>();

    public UIToggle cHmdControllerState;
    public UIPopupList cMaxPlayers;
    public UIInput cPassword;

    /// <summary>
    /// [Level......Titles]
    /// </summary>
    NameValueCollection nc = new NameValueCollection();


    private string mTitle = string.Empty;

    void Start () {
        InitDb();
        Init();

        //GetOthersState(mTitle);

        EventDelegate.Add(cMaxPlayers.onChange, OnMaxPlayerChange);
    }
	
	void Update () {
		
	}

    void OnMaxPlayerChange()
    {
        Debug.LogFormat("MaxPlayers:{0}",cMaxPlayers.value);

        PlayerPrefs.SetString(PlayerProfile.mMaxPlayer, cMaxPlayers.value);
        PlayerProfile.VMaxPlayer = cMaxPlayers.value;
    }


    /// <summary>
    /// Init UI from Db
    /// </summary>
    void Init()
    {
     
        mClassLvCount = nc.AllKeys.Length;
        Debug.LogFormat("NC Count:{0}", mClassLvCount);

        //LvDropDown
        var LvDropdown = mLvDropdown.GetComponent<UIPopupList>();
        var LvFirstLabel = mLvDropdown.GetComponentInChildren<UILabel>();

        //Per Level Name  ==> Chapter
        for (int i = 0; i < mClassLvCount; i++)
        {
            string LvData = nc.GetKey(i);

            Debug.LogFormat("Level========{0}=>{1}", i, nc.GetKey(i));

            LvFirstLabel.text = nc.GetKey(0);
            LvDropdown.AddItem(LvData);

            //LvDropdown.onChange.Add(new EventDelegate(OnDropdownLevel));
            EventDelegate.Add(LvDropdown.onChange, OnDropdownLevel);

            //Instantiate Dropdown => Per Lv
            var go = Instantiate(dropdownPrefab, dropdownParent.transform, true);
            //Set Name
            go.name += LvData;
            //Cache chapter dropdwon ui
            cacheChapterItem.Add(i, go);

            Debug.Log("Cache:........................"+ go.name);

            //Set Pos&Scale[Manual] 
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            //====================Per Level Name Per Contexts===============================
            var enumerable = nc.GetValues(i);
            if (enumerable != null)
                for (int index = 0; index < enumerable.Length; index++)
                {
                    var temp = enumerable[index];
                    string chapterData = temp;
                    //Debug.LogFormat("PerContext:index:{0}=>{1}", enumerable.Length, temp);
                    //SetName
                    go.GetComponentInChildren<UILabel>().text = enumerable[0];
                    mTitle = enumerable[0];
                    Debug.LogFormat("value:{0}", enumerable[0]);

                    //Set Options
                    go.GetComponent<UIPopupList>().AddItem(chapterData);
                    //Register Event
                    //go.GetComponent<UIPopupList>().onChange.Add(OnDropdownChapter);
                    EventDelegate.Add(go.GetComponent<UIPopupList>().onChange, OnDropdownChapter);
                }
        }

        //End===> Enable Chapter 1;Disable others
        for (int k = 0; k < cacheChapterItem.Count; k++)
        {
            if (k != 0)
                cacheChapterItem[k].SetActive(false);
        }

    }

     void InitDb()
    {
        var ds = new DbService(PlayerProfile.mDbName, PlayerProfile.mDbPassword);
        var t = ds._connection.Table<VrClass>().GetEnumerator();

        NameValueCollection nvc = new NameValueCollection();

        while (t.MoveNext())
        {
            if (t.Current != null)
            {
                if (!string.IsNullOrEmpty(t.Current.SceneName))
                {
                    nvc.Add(t.Current.Level, t.Current.Title);
                }
            }
        }

        //Sort NVC
        var sortedList = new SortedList(nvc.AllKeys.ToDictionary(k => k, k => nvc[k]));
        for (int i = 0; i < sortedList.Count; i++)
        {
            //Debug.LogFormat("{0}:{1}", sortedList.GetKey(i), sortedList.GetByIndex(i));
            string[] temp = StringSplit(sortedList.GetByIndex(i).ToString(), ",");

            for (int m = 0; m < temp.Length; m++)
            {
                nc.Add(sortedList.GetKey(i).ToString(), temp[m]);
            }
        }

        nvc = null;
        sortedList = null;
        t.Dispose();
    }
  
    void OnDisable()
    {
        EventDelegate.Remove(mLvDropdown.GetComponent<UIPopupList>().onChange, OnDropdownLevel);
        //RemoveChapterListener
        for (int i = 0; i < cacheChapterItem.Count; i++)
            EventDelegate.Remove(cacheChapterItem[i].gameObject.GetComponent<UIPopupList>().onChange, OnDropdownChapter);
        
    }

    public string[] StringSplit(string strSource, string strSplit)
    {
        string[] strtmp = new string[1];
        int index = strSource.IndexOf(strSplit, 0);
        if (index < 0)
        {
            strtmp[0] = strSource;
            return strtmp;
        }
        else
        {
            strtmp[0] = strSource.Substring(0, index);
            return StringSplit(strSource.Substring(index + strSplit.Length), strSplit, strtmp);
        }
    }
    private string[] StringSplit(string strSource, string strSplit, string[] attachArray)
    {
        string[] strtmp = new string[attachArray.Length + 1];
        attachArray.CopyTo(strtmp, 0);

        int index = strSource.IndexOf(strSplit, 0);
        if (index < 0)
        {
            strtmp[attachArray.Length] = strSource;
            return strtmp;
        }
        else
        {
            strtmp[attachArray.Length] = strSource.Substring(0, index);
            return StringSplit(strSource.Substring(index + strSplit.Length), strSplit, strtmp);
        }
    }
    public void OnDropdownLevel()
    {
        var LvDropdown = mLvDropdown.GetComponent<UIPopupList>();
        Debug.Log("OnDropdownLevel=>" + LvDropdown.value);

        PlayerPrefs.SetString(PlayerProfile.mLevel, LvDropdown.value);
        PlayerPrefs.Save();


        //Set Selected Item ==>Chapter Dropdown
        for (int k = 0; k < cacheChapterItem.Count; k++)
        {
            Debug.Log("cacheChapterItem=>" + LvDropdown.value);

            int t = Convert.ToInt16(LvDropdown.value.Substring(5));//Level

            Debug.Log("cacheChapterItemIndex=>" + t + "===" + k);

            cacheChapterItem[k].SetActive(false);

            if (k == t-1)
            {

                cacheChapterItem[k].SetActive(true);

                cacheChapterItem[k].gameObject.GetComponentInChildren<UILabel>().text
                    = cacheChapterItem[k].GetComponent<UIPopupList>().items[0];

                GetOthersState(cacheChapterItem[k].gameObject.GetComponentInChildren<UILabel>().text);
            }
            
        }

       
    }
    void OnDropdownChapter()
    {
        for (int k = 0; k < cacheChapterItem.Count; k++)
        {
            if (cacheChapterItem[k].gameObject.activeSelf)
            {
                PlayerPrefs.SetString(PlayerProfile.mTitle, cacheChapterItem[k].GetComponent<UIPopupList>().value);
                PlayerProfile.VTitle = cacheChapterItem[k].GetComponent<UIPopupList>().value;
                PlayerPrefs.Save();
                Debug.Log("SelectAndSave====================>" + cacheChapterItem[k].GetComponent<UIPopupList>().value);

               GetOthersState(cacheChapterItem[k].GetComponent<UIPopupList>().value);
            }
        }
    }

    void GetOthersState(string title)
    {
        
        //Sql
        string subject = string.Empty;
        string level = string.Empty;
        string sceneName = string.Empty;
        string hmdState = string.Empty;
        string context = string.Empty;
        string adjustHeight = string.Empty;
        string isSmall = string.Empty;
        string pos = string.Empty;
        string elements = string.Empty;

        var ds = new DbService(PlayerProfile.mDbName, PlayerProfile.mDbPassword);

        var t = ds._connection.Table<VrClass>().GetEnumerator();
        while (t.MoveNext())
        {
            if (t.Current != null && !string.IsNullOrEmpty(t.Current.Title))
            {
                if (t.Current.Title.Equals(title)&&t.Current.Level.Equals(mLvDropdown.GetComponentInChildren<UILabel>().text))
                {
                    Debug.Log("/t"+t.Current.Subject + " " + t.Current.Level + " " + t.Current.Position + " " +t.Current.Element);
                    subject = t.Current.Subject;
                    level = t.Current.Level;
                    sceneName = t.Current.SceneName;
                    hmdState = t.Current.UseHmd;
                    context = t.Current.Context;
                    isSmall = t.Current.IsSmall;
                    adjustHeight = t.Current.AdjustHeight;
                    pos = t.Current.Position;
                    elements = t.Current.Element;
                }
            }
        }

        //SUBJECT
        PlayerPrefs.SetString(PlayerProfile.mSubjet,subject);
        PlayerProfile.VSubjet = subject;

        //LEVEL
        PlayerPrefs.SetString(PlayerProfile.mLevel, level);
        PlayerProfile.VLevel = level;
        
        //SCENE NAME
        PlayerPrefs.SetString(PlayerProfile.mScene, sceneName);
        PlayerProfile.VScene = sceneName;

        //CONTEXT
        PlayerPrefs.SetString(PlayerProfile.mContext, context);
        PlayerProfile.VContext = context;

        //HMD
        PlayerPrefs.SetString(PlayerProfile.mUseHmd, hmdState);
        PlayerProfile.VUseHmd = hmdState;

        if (PlayerProfile.VUseHmd == "True")
            SetHmdState(true);
        else
            SetHmdState(false);

        //POSITION
        PlayerPrefs.SetString(PlayerProfile.mPos, pos);
        PlayerProfile.VPos = pos;

        //ELEMENTS
        PlayerPrefs.SetString(PlayerProfile.mElements,elements);
        PlayerProfile.VElements = elements;

        //IsSMALL
        PlayerPrefs.SetString(PlayerProfile.mSmall, isSmall);
        PlayerProfile.VSmall = isSmall;

        //ADJUST HEIGHT
        PlayerPrefs.SetString(PlayerProfile.mAdjustHeight, adjustHeight);
        PlayerProfile.VAdjustHeight = adjustHeight;

        //MAXPLAYERS
        //PlayerPrefs.SetString(PlayerProfile.mMaxPlayer, cMaxPlayers.value);
        //PlayerProfile.VMaxPlayer = cMaxPlayers.value;
        

        //PASSWORD
        PlayerPrefs.SetString(PlayerProfile.mUserPw, cPassword.value);
        PlayerProfile.VUserPw = cPassword.value;

        //SAVE ALL
        PlayerPrefs.Save();

        Debug.LogFormat("Level=>{0},Scene=>{1},HMD=>{2},Password=>{3},AdjustHeight=>{4},IsSmall=>{5},MaxPlayers =>{6},Context={7}",
                        PlayerProfile.VLevel,PlayerProfile.VScene,PlayerProfile.VUseHmd,PlayerProfile.VUserPw,PlayerProfile.VAdjustHeight,PlayerProfile.VSmall,PlayerProfile.VMaxPlayer,PlayerProfile.VContext);

        t.Dispose();
    }

    void SetHmdState(bool state)
    {
        cHmdControllerState.value = state;
    }
}
