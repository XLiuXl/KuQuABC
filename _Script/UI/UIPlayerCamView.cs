using UnityEngine;
using System.Collections;
using TNet;

public class UIPlayerCamView : MonoBehaviour {

	public TNet.Player player;
	public UILabel label;
	public UITexture sprite;

	public void UpdateInfo ()
	{
		if (player != null)
		{
			label.text = player.name;
			Debug.Log("UIPlayerName =>"+label.text);
		}
	}

	void Update(){
		if (sprite.mainTexture == null&&
		   label.text != null) {
			GameObject go = null;

			if(player.name!=string.Empty&&player.name!=null)
				go = GameObject.Find (player.name);

			if (go != null&&sprite.mainTexture==null) {
				GameObject t = go.GetChild ("CameraRT");
				if (t != null) {
					sprite.mainTexture = t.GetComponent<Camera> ().targetTexture;
					Debug.Log ("UpdateCamRT....");
				}
			}
		}
	}
}
