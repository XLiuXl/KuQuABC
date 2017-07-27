using UnityEngine;
using System.Collections;

public class Turn : MonoBehaviour {
	void Start()
	{
		time = Time.unscaledTime;
	}
	float  time=0;
	bool addForce=true;
	// Update is called once per frame
	void Update () {
		if (Time.unscaledTime-time>=3f) {
			time = Time.unscaledTime;
			addForce = !addForce;
		}

		if (!addForce) {
			GetComponent<Rigidbody> ().AddForce (0, -2, 1);
			return;
		}

		GetComponent<Rigidbody> ().AddForce (0, 2, -1);
	}
}
