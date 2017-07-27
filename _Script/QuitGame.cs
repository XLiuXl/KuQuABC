﻿using UnityEngine;
using VrNet.NetLogic;


public class QuitGame : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F4)) {
			GameManager.EndGame ();
			GameManager.LoadMenu ();
			Debug.Log ("F4=>"+"quit......");
		}
	}
}
