using UnityEngine;
using System.Collections;
using TNet;

public class UIFollowSender :TNBehaviour  {
	public GameObject target;
	public UILabel mPlayerName;
	private bool mFollow = false;
    public UIPlayerName CurrentPlayer = null;

	void Start () {
		EventDelegate.Add(target.GetComponent<UIButton>().onClick,OnFollowClick);
	}

	void OnFollowClick(){
		mFollow = !mFollow;
		var name = mPlayerName.text;

		Messenger.Broadcast<TNet.Player,bool> ("OnFollow", CurrentPlayer.player, mFollow);
	}
}
