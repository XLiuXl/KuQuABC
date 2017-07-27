using UnityEngine;
using System.Collections;

public class PlayerCamRTView: MonoBehaviour {
	public TNet.Player player;
	public int TexValue = 256;

	// Use this for initialization
	void Awake () {
		InitRtTex ();
	}

	void InitRtTex(){
		if (this.GetComponent<Camera> ().targetTexture == null) {
			var rt = new RenderTexture (TexValue, TexValue, 24);
			rt.antiAliasing = 2;
			rt.format = RenderTextureFormat.ARGB32;

			this.GetComponent<Camera> ().targetTexture = rt;
		}
	}
}
