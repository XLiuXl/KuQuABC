using UnityEngine;
using System.Collections;
using TNet;

public class UIDisableCpt : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!TNManager.isHosting)
			gameObject.SetActive (false);
	}
}
