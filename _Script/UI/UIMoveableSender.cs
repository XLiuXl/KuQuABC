using UnityEngine;
using System.Collections;
using TNet;
using VRTK;

public class UIMoveableSender : MonoBehaviour {
	public GameObject target;
    public UILabel mPlayerName;
    private bool mMoveable = false;

    void Awake()
    {
		UIEventListener.Get(target).onClick = OnToggle;
    }

	void Start(){
	}

    void OnToggle(GameObject go)
    {
		mMoveable = !mMoveable;
        var name = mPlayerName.text;

		Messenger.Broadcast<string,bool> ("OnMoveable",name,mMoveable);
    }

}
