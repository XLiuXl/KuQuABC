using UnityEngine;
using System.Collections;
using VRTK;
using TNet;
using VrNet.SubFunc;

[RequireComponent(typeof(TNObject))]

public class CSyncClock : VRTK_InteractableObject {

	public ClockFunc myClockFunc;
	private TNObject myTNObject;
	public string myTagetName;

	float zH =0f,zM =0f,zS =0f;

	void Start ()
	{
		myTNObject = GetComponent<TNObject> ();
	}


	public override void OnInteractableObjectTouched (InteractableObjectEventArgs e)
	{
		base.OnInteractableObjectTouched (e);

		switch (myTagetName) {
		case"ClockStartButton":
			myClockFunc.isRunning = true;
			zH = myClockFunc.myHours.localRotation.z;
			zM = myClockFunc.myMins.localRotation.z;
			zS = myClockFunc.mySecs.localRotation.z;

			myTNObject.SendQuickly ("SyncClock",Target.AllSaved,myClockFunc.isRunning,zH,zM,zS);
			break;
			case"ClockStopButton":
			myClockFunc.isRunning = false;
			myTNObject.SendQuickly ("SyncClock",Target.AllSaved,myClockFunc.isRunning,zH,zM,zS);
			break;
		}
	}

	[RFC]
	void SyncClock(bool state,float zh,float zm,float zs){
		
		myClockFunc.isRunning = state;

		myClockFunc.myHours.localRotation = Quaternion.Euler (0f,0f,zh);
		myClockFunc.myMins.localRotation = Quaternion.Euler (0f,0f,zm);
		myClockFunc.mySecs.localRotation = Quaternion.Euler (0f,0f,zs);

		Debug.Log ("RemoteClock=>State="+state);
	}
}
