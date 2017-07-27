using UnityEngine;
using System.Collections;
using VrNet.NetLogic;
using VrNet.UICommon;

namespace VrNet.LoginLogic
{
    public class UILogin : ServerManager
    {
        private UIInput UserName;
        private UIInput Password;
        private UIToggle RemeberMe;

        public UIPanel PlayPanel;

        /// <summary>
        /// 登录界面用户名
        /// </summary>
        private string username = "";
        /// <summary>
        /// 登录界面密码
        /// </summary>
        private string password = "";
        
        void Awake()
        {
            InitManager();
            InitWins();
        }

        void InitWins()
        {
            UserName = GameObject.Find("InputArea_Username").GetComponent<UIInput>();
            Password = GameObject.Find("InputArea_Password").GetComponent<UIInput>();
            RemeberMe = GameObject.Find("Checkbox-RememberMe").GetComponent<UIToggle>();
            RemeberMe.value = true;

            if(PlayPanel != null)UIWindow.Add(PlayPanel);


            // 初始化用户名
            if (PlayerPrefs.HasKey("vr_username"))
            {
                username = PlayerPrefs.GetString("vr_username");
                UserName.value = PlayerPrefs.GetString("vr_username");
            }

            //初始化密码
            if (PlayerPrefs.HasKey("vr_password"))
            {
                password = PlayerPrefs.GetString("vr_password");
                Password.value = PlayerPrefs.GetString("vr_password");
            }
        }

       

        /// <summary>
        /// 执行登录操作
        /// </summary>
        public void CallLogin()
        {
            string tips = "用户名或者密码必须大于6位字符！";
            username = UserName.value;
            password = Password.value;
            
            if (username.Length < 6) { alertField.text = tips; return; }
            if (password.Length < 6) { alertField.text = tips; return; }


            Login();
        }

        /// <summary>
        /// 登录
        /// </summary>
        void Login()
        {

            if (username == "" || password == "") { return; }

            // 发送消息到服务器(RSA加密)
            string[] datas = new string[2];
            datas[0] = username;
            datas[1] = UtilsProSecure.hash(password);
            
            //第一次尝试是否为管理员
            //成功以管理员登录否则以普通用户登录
            Send("AdminLogin", OnLoginRequestAdminSucess, OnLoginRequestAdminError, datas);
        }

        private void OnLoginRequestAdminSucess(string[] serverDatas)
        {
            alertField.text = serverDatas[0];
            UserSession.session_id = serverDatas[1];            // Save SID the server gave us
            UserSession.loggedIn = true;                        // Set the flag saying the user is logged in correctly
            UserSession.isAdmin = true;                         // Set the flag saying the user is an administrator
            UserSession.username = username;                    // Save login
            UserSession.password = password;                    // Save password

            //登录成功且勾选记录密码功能后记住密码
            if (RemeberMe.value)
            {
                PlayerPrefs.SetString("vr_username", username);
                PlayerPrefs.SetString("vr_password", password);
            }

            Debug.Log(UserSession.username+"=>Admin："+UserSession.isAdmin);

            //登录成功打开学习方式列表
            //测试版本跳过学习方式，直接打开局域网设置
            UIWindow.Show(PlayPanel);

        }
        private void OnLoginRequestAdminError(string errorMessage)
        {
            Debug.Log(errorMessage);
            alertField.text = errorMessage; // Show the server's message

            //登录成功打开学习方式列表(非管理员方式登录)
            //测试版本跳过学习方式，直接打开局域网设置
            //发送动作，注册回调函数
            string[] datas = new string[2];
            datas[0] = username;
            datas[1] = UtilsProSecure.hash(password);
            Send("Login", OnLoginSucess, OnLoginError, datas);
        }

        /// <summary>
        /// 登录成功
        /// </summary>
        /// <param name="msg">服务器消息</param>
        void OnLoginSucess(string[] msg)
        {
            alertField.text = msg[0];

            UserSession.session_id = msg[1];            // Save SID the server gave us
            UserSession.loggedIn = true;                        // Set the flag saying the user is logged in correctly
            UserSession.isAdmin = false;                         // Set the flag saying the user is an administrator
            UserSession.username = username;                    // Save login
            UserSession.password = password;                    // Save password

            Debug.Log("OnLoginSucess");
          
            //登录成功且勾选记录密码功能后记住密码
            if (RemeberMe.value)
            {
                PlayerPrefs.SetString("vr_username", username);
                PlayerPrefs.SetString("vr_password", password);
            }

            UIWindow.Show(PlayPanel);
        }

        /// <summary>
        /// 登录失败
        /// </summary>
        /// <param name="msg">服务器消息</param>
        void OnLoginError(string msg)
        {
            Debug.Log(msg);

            string tips = "账号未激活,请核实账号信息并检查您的注册邮箱，点击链接激活账号.";

            if (msg.Contains("Your IP is not activated for this account, please enter your IP password or follow the link we sent you on your email address."))
            {
                alertField.text = tips;
            }
            else if (msg.Contains("Your account is not activated yet, please follow the link we sent you on your email address to activate it."))
            {
                //显示服务器消息
                alertField.text = tips;
            }
            else
            {
                //显示服务器消息
                alertField.text = msg;
            }
        }
    }

}