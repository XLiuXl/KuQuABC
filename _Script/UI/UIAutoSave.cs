using UnityEngine;
using System.Collections;

namespace VrNet.UICommon
{
    [RequireComponent(typeof(UIPopupList))]
    public class UIAutoSave : MonoBehaviour
    {

        public string keyName;
        
        void Start()
        {
            PlayerPrefs.SetString(keyName,GetComponent<UIPopupList>().value);
            Debug.Log("Save=>"+keyName +"=>value=>"+ GetComponent<UIPopupList>().value);
        }


    }
}
