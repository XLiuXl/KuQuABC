using UnityEngine;
using System.Collections;
using VrNet.UICommon;

public class UILocalLogin : MonoBehaviour {

    private UIInput UserName;
    private UIToggle RemeberMe;

    public UIPanel PlayPanel;
    /// <summary>
    /// 登录界面用户名
    /// </summary>
    private string username = "";
    private UILabel alertField;

    void Start () {
        InitWins( );
    }

    void InitWins( ) {
        alertField = GameObject.Find("AlertMessage").GetComponent<UILabel>( );
        UserName = GameObject.Find("InputArea_Username").GetComponent<UIInput>( );   
        RemeberMe = GameObject.Find("Checkbox-RememberMe").GetComponent<UIToggle>( );
        RemeberMe.value = true;

        if (PlayPanel != null) UIWindow.Add(PlayPanel);


        // 初始化用户名
        if (PlayerPrefs.HasKey("vr_username")) {
            username = PlayerPrefs.GetString("vr_username");
            UserName.value = PlayerPrefs.GetString("vr_username");
            PlayerProfile.playerName = username;
        }
    }

    /// <summary>
    /// 执行登录操作
    /// </summary>
    public void CallLogin( ) {
        string tips = "用户名或者密码必须大于6位字符！";
        username = UserName.value;

        if (username.Length < 6) { alertField.text = tips; return; }
        Login( );
    }

    /// <summary>
    /// 登录
    /// </summary>
    void Login( ) {

        if (username == "" ) { return; }

        //登录成功且勾选记录密码功能后记住密码
        if (RemeberMe.value) {
            PlayerPrefs.SetString("vr_username", username);
        }

        //登录成功打开学习方式列表
        //测试版本跳过学习方式，直接打开局域网设置
        UIWindow.Show(PlayPanel);
    }


    // Update is called once per frame
    void Update () {
	
	}
}
