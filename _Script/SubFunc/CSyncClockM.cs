using UnityEngine;
using System.Collections;
using VRTK;
using TNet;
using VrNet.SubFunc;

[RequireComponent(typeof(TNObject))]

public class CSyncClockM : VRTK_InteractableObject {

	private TNObject myTNObject;
	float speed =-1f;
	public Transform arrow;
	float currentRot;

	void Start ()
	{
		myTNObject = GetComponent<TNObject> ();
	}

	public override void StartTouching (GameObject currentTouchingObject)
	{

		base.StartTouching (currentTouchingObject);

		currentRot += (arrow.localRotation.x+speed);
		arrow.localRotation = Quaternion.Euler (0f,180f,currentRot);

		myTNObject.SendQuickly ("SyncClockRotM",Target.AllSaved,currentRot);
	}

	[RFC]
	void SyncClockRotM(float v){
		arrow.localRotation = Quaternion.Euler (0f,180f,v);
		Debug.Log ("RemoteClock-->"+"M=>v="+v);
	}
}
