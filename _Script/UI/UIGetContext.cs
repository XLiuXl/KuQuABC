using UnityEngine;
using System.Collections;
using TNet;
/// <summary>
/// 根据唯一Id(课程标题)
/// 从数据库获取Context
/// 此功能只对主机开放
/// </summary>
public class UIGetContext : MonoBehaviour {

    public GameObject UiUserContext;
    public UILabel context;
    public TweenPosition tw;
    private bool show = false;

	void Start () {
        if (TNManager.isHosting)
        {
            UiUserContext.SetActive(true);    
            context.text = PlayerPrefs.GetString("Vr_Mulit_Subject_Context");
        }
        else {
            UiUserContext.SetActive(false);
        }
	}

    public void ShowHideContext()
    {
        show = !show;

        if (show)
            tw.PlayForward();
        else
            tw.PlayReverse();
    }
}
