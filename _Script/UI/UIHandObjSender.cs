using UnityEngine;
using System.Collections;

public class UIHandObjSender : MonoBehaviour {

	public GameObject lable;
	public UIButton button;
	[HideInInspector]
	public GameObject target;

	void Start () {
		EventDelegate.Add (this.GetComponent<UIToggle>().onChange,OnToggle);
		EventDelegate.Add (button.onClick,OnHandObjClicked);

		Messenger.AddListener<bool> ("BroadcastHandObjsState",OnSet);

     
	}

	void OnSet(bool state){
		gameObject.GetComponent<UIToggle> ().value = state;
	}

	void OnToggle(){
		 //Debug.Log ("Toggle=>"+target+"=>"+this.GetComponent<UIToggle> ().value);
		Messenger.Broadcast<string,bool> ("OnHandObjToggle",target.name, this.GetComponent<UIToggle> ().value);
	}

	void OnHandObjClicked(){
		//Debug.Log ("Clicked=>"+target+"=>"+target.transform.position);
		Messenger.Broadcast<string> ("OnHandObjClick",target.name);
	}

}
