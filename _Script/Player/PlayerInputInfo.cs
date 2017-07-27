using UnityEngine;
using System.Collections;
using VrNet.UICommon;
using TNet;

public class PlayerInputInfo : TNBehaviour {

	public TextMesh mTextMesh;
	private GameObject mChat;

	void Start () {
		mTextMesh = mTextMesh.GetComponent<TextMesh>();
		mTextMesh.text = string.Empty;
		mChat = GameObject.FindGameObjectWithTag ("PlayerChat");

	}


	void Update () {
		if (mChat != null) {
			var chat = mChat.GetComponent<UIGameChat> ().inputInfo;
			mTextMesh.text = chat;
			if (chat == "clear")
				mTextMesh.text = string.Empty;

			tno.SendQuickly ("SetPlayerInfo", Target.AllSaved, mTextMesh.text);
		}
	}

	[RFC]
	void SetPlayerInfo(string text){
		mTextMesh.text = text;
	}
}
