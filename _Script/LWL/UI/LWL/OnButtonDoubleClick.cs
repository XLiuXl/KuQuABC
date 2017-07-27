using UnityEngine;
using System.Collections;

public class OnButtonDoubleClick : MonoBehaviour {
    private float time = 0;

    private UIButton button = null;
    private int ClickCount = 0;
    private UILabel label = null;
    void Awake()
    {
        label = gameObject.GetComponent<UILabel>();
        button = gameObject.GetComponent<UIButton>();
        EventDelegate.Add(button.onClick, OnCurrentButtonClick);
    }
	void Start () {
	
	}
    void OnCurrentButtonClick()
    {
       
        if (Time.timeSinceLevelLoad - time > 0.2f)
        {
            ClickCount = 0;
            time = Time.timeSinceLevelLoad;
            
        }
        ClickCount += 1;
        if (ClickCount==2)
        {
            if (!gameObject.transform.parent.GetComponent<UIToggle>().value)
                return;
            Messenger.Broadcast<string>("MoveToObjPos", label.text);
        }
    }

}
