using UnityEngine;
using System.Collections;
using System;

namespace VrNet.SubFunc{
	public class ClockFunc : MonoBehaviour {

		public Transform myHours;
		public Transform myMins;
		public Transform mySecs;

		private const float hoursToDegrees = 360f / 12f;
		private const float minutesToDegrees = 360f / 6f;
		private const float secondsToDegrees = 360f / 6f;

		public float vSpeed;

		public bool isRunning = false;

		float zH =0f,zM =1f,zS =1f;
		Quaternion hQ = Quaternion.identity,mQ= Quaternion.identity,sQ= Quaternion.identity;

		void Update(){

			if (Input.GetKeyDown (KeyCode.P))
				isRunning = !isRunning;
			

			if (isRunning) {
				zM += (Time.deltaTime*vSpeed);
				zS += (Time.deltaTime*vSpeed*5);

				TimeSpan t = DateTime.Now.TimeOfDay;

				zH = (float)t.TotalHours * hoursToDegrees;
				//zM = (float)t.TotalMinutes * minutesToDegrees;
				//zS = (float)t.TotalSeconds * secondsToDegrees;

				myHours.localRotation = Quaternion.Euler (0f, 0f, zH);
				myMins.localRotation = Quaternion.Euler (0f, 0f, zM);
				mySecs.localRotation = Quaternion.Euler (0f, 0f, zS);

				hQ = myHours.localRotation;
				mQ = myMins.localRotation;
				sQ = mySecs.localRotation;
				
			} else {
				
				myHours.localRotation =hQ;
				myMins.localRotation = mQ;
				mySecs.localRotation = sQ;

				//zM = zS = 0f;
			}
		}
	} 
}
