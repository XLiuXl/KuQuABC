using UnityEngine;
using System.Collections;

namespace VrNet.LoginLogic
{
    [RequireComponent(typeof(UILabel))]
    public class UIGameNews : MonoBehaviour
    {

        public string url = "http://360vrbox.com/News.txt";

        UILabel mLabel;
        string mData = null;

        void Awake()
        {
            mLabel = GetComponent<UILabel>();
            mLabel.text = Localization.Get("News");
            CheckNews();
        
        }

        void CheckNews()
        {
            if (string.IsNullOrEmpty(mData))
            {
                if (PlayerProfile.allowedToAccessInternet)
                {
                    GameWebRequest.Create(url, OnFinished);
                }
                else
                {
                    mLabel.text = Localization.Get("NetRequired");
                }
            }
            else
            {
                mData = PlayerPrefs.GetString("Vr_News");
            }
        }

        void OnEnable()
        {
            if (string.IsNullOrEmpty(mData))
            {
                if (PlayerProfile.allowedToAccessInternet)
                {
                    GameWebRequest.Create(url, OnFinished);
                }
                else
                {
                    mLabel.text = Localization.Get("NetRequired");
                }
            }
        }

        public void Update()
        {
            if(!string.IsNullOrEmpty(mData)) mLabel.text = Localization.Get("NetRequired");
        }

        void OnFinished(bool success, object obj, string text)
        {
            if (success)
            {
                mData = text;
                mLabel.text = text;
                PlayerPrefs.SetString("Vr_News", mData);
            }
            else
            {
                mLabel.text = Localization.Get("News Failed");
            }
            Destroy(this);
        }
    }
}
