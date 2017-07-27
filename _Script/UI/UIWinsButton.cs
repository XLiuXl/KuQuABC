using UnityEngine;
using System.Collections;

namespace VrNet.UICommon
{
    public class UIWinsButton : MonoBehaviour
    {
        public enum Action
        {
            Show,
            Hide,
            GoBack,
        }

        public UIPanel window;
        public Action action = Action.Hide;
        public bool requiresFullVersion = false;
        public bool eraseHistory = false;

        void Start()
        {
            UIPanel panel = NGUITools.FindInParents<UIPanel>(gameObject);

            if (panel != null)
            {
                UIWindow.Add(panel);
            }
            EventDelegate.Add(GetComponent<UIButton>().onClick, ButtonClick);
        }

        //void OnClick()
        //{
        //    //if (requiresFullVersion && !PlayerProfile.fullAccess)
        //    //{
        //    //    UIUpgradeWindow.Show();
        //    //    return;
        //    //}
        //    ButtonClick();
          
        //}
     public   void ButtonClick()
      {
            switch (action)
            {
                case Action.Show:
                    {
                        if (window != null)
                        {
                            if (eraseHistory) UIWindow.Close();
                            UIWindow.Show(window);
                        }
                    }
                    break;

                case Action.Hide:
                    UIWindow.Close();
                    break;

                case Action.GoBack:
                    UIWindow.GoBack();
                    break;
            }
        }
        

    }
}
