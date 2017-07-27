using UnityEngine;
using System.Collections;

namespace VrNet.LoginLogic
{
    public class UISettingTitleState : MonoBehaviour
    {
        public UISprite CN;
        public UISprite EN;

        void OnEnable()
        {
            SwtichLans();
        }

        void SwtichLans()
        {
            switch (Localization.language)
            {
                case "Chinese":
                    CN.gameObject.SetActive(true);
                    EN.gameObject.SetActive(false);
                    break;
                case "English":
                    CN.gameObject.SetActive(false);
                    EN.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        public void Update()
        {
            SwtichLans();
        }
    }

}