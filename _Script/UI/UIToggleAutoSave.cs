using UnityEngine;
using System.Collections;

namespace VrNet.UICommon
{
    [RequireComponent(typeof(UIToggle))]
    public class UIToggleAutoSave : MonoBehaviour
    {

        public string keyName;
        

        public void Set()
        {
			PlayerPrefs.SetString(keyName,GetComponent<UIToggle>().value.ToString());
			Debug.Log("Save=>"+keyName +"=>value=>"+ GetComponent<UIToggle>().value.ToString());
        }
    }
}
