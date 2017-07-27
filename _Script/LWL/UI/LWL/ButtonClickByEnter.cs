using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrNet.UICommon;


public class ButtonClickByEnter : MonoBehaviour {
    public UIButton buttonWhenWithOutWins = null;
    // Use this for initialization
    void Awake()
    {
        buttonWhenWithOutWins =GetComponent<UIButton>();

    }
	void Update () {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("EnterButtonClick");
                EventDelegate.Execute(buttonWhenWithOutWins.onClick);
           
        }
	}
}
