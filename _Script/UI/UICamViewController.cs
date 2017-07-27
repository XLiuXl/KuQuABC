using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TNet;

public class UICamViewController : TNBehaviour {

	public GameObject UiCViewController;
	public GameObject ScrollView;
	public GameObject prefab;
	float rowWidth = 200f;

	public TweenPosition tw;
	private bool show = false;

    TNet.List<UIPlayerCamView> mPlayerCamView = new TNet.List<UIPlayerCamView>();
	bool mRebuild = true;


	void Start () {
		if (TNManager.isHosting)
		{	
			UiCViewController.SetActive(true);
		}
		else {
            if (UiCViewController!=null)
                UiCViewController.SetActive(false);
		}
	}

	void OnEnable()
	{
		TNManager.onJoinChannel += OnNetworkJoinChannel;
		TNManager.onPlayerJoin += OnNetworkPlayerJoin;
		TNManager.onPlayerLeave += OnNetworkPlayerLeave;
	}

	void OnDisable()
	{
		TNManager.onJoinChannel -= OnNetworkJoinChannel;
		TNManager.onPlayerJoin -= OnNetworkPlayerJoin;
		TNManager.onPlayerLeave -= OnNetworkPlayerLeave;
	}

	void OnNetworkJoinChannel(int channelID, bool success, string message) { mRebuild = true; }
	void OnNetworkPlayerJoin(int channelID, Player p) { mRebuild = true; }
	void OnNetworkPlayerLeave(int channelID, Player p) {mRebuild = true; }

	
	// Update is called once per frame
	void Update () {
		if (mRebuild&&TNManager.isHosting)
		{
			mRebuild = false;

			// Clear the previous list
			for (int i = 0; i < mPlayerCamView.size; ++i)
			{
				UIPlayerCamView p = mPlayerCamView[i];
				Destroy(p.gameObject);
			}
			mPlayerCamView.Clear();

			// Add the player
			AddPlayer(TNManager.player);

			// Add other players
			for (int i = 0; i < TNManager.players.size; ++i)
				AddPlayer(TNManager.players[i]);
			
			// Reposition all children so that they seem to grow from the Top side of the screen
			float offset = (mPlayerCamView.size - 1) * 0.5f * rowWidth;

			Debug.Log ("Size=>"+mPlayerCamView.size+" Offset=>"+offset);

			for (int i = 0; i < mPlayerCamView.size; ++i)
			{
				UIPlayerCamView pn = mPlayerCamView[i];
				pn.transform.localPosition = new Vector3(Mathf.RoundToInt(offset),0f, 0f);
				offset -= rowWidth;
			}

			UIPanel pnl = NGUITools.FindInParents<UIPanel>(ScrollView);
			if (pnl != null) pnl.Refresh();

		}
	}

	void AddPlayer(Player p)
	{
		UIPlayerCamView pn = GetPlayerEntry(p);

		if (pn == null)
		{
			GameObject go = NGUITools.AddChild(ScrollView.gameObject, prefab);
			pn = go.GetComponent<UIPlayerCamView>();
			pn.player = p;
			pn.UpdateInfo();
		
			//Find Player Camera RT
			GameObject tt = GameObject.Find (p.name);

			if (tt != null) {
				var t = GameObject.Find (p.name).GetChild ("CameraRT");
				if(t!=null)pn.sprite.mainTexture = t.GetComponent<Camera> ().targetTexture;
			}

			mPlayerCamView.Add (pn);

			Debug.Log("CamView_AddPlayer =>"+pn.player.name);
		}
	}

	UIPlayerCamView GetPlayerEntry(Player p)
	{
		for (int i = 0; i < mPlayerCamView.size; ++i)
		{
			UIPlayerCamView pn = mPlayerCamView[i];
			if (pn.player == p) return pn;
		}
		return null;
	}

	public void ShowHideContext()
	{
		show = !show;

		if (show)
			tw.PlayForward();
		else
			tw.PlayReverse();
	}
}
