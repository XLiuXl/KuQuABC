using UnityEngine;
using System.Collections;
using VRTK;

public class QueuePlayerMoveableController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Messenger.AddListener<string,bool> ("QueuePlayerMoveable", OnSetPlayerMoveable);
	}

	void OnSetPlayerMoveable(string name,bool state){

		Debug.Log ("RevRemote=>"+name+"=="+state);

		GameObject go = null;

		if (name != string.Empty && name != null) {

		go = GameObject.Find (name);

		if (go != null) {
				Debug.Log ("RemotePlayer=>"+go.name+"=>"+state);

				Transform left = go.transform.Find ("Controller(left)");
				Transform right = go.transform.Find ("Controller(right)");

				Debug.Log (left.name);
				Debug.Log (right.name);

				if (left != null) {
					var ms = left.GetComponent<VRTK_BezierPointer> ();
					ms.enableTeleport = state;

					Debug.Log ("RemoteLeftController=>"+state);
				}

				if (right != null) {
					var ms = right.GetComponent<VRTK_BezierPointer> ();
					ms.enableTeleport = state;

					Debug.Log ("RemoteLeftController=>"+state);
				}

				/*
				VRTK_BezierPointer[] target = go.GetComponentsInChildren<VRTK_BezierPointer> ();
			
				Debug.Log ("RemoteController=>"+target.Length);

				if (target != null && target.Length >= 1) {
					for (int i = 0; i < target.Length; i++) {
						target [i].enableTeleport = state;
						Debug.Log ("RemoteController=Name=>"+target[i].gameObject.name);
					}
				}*/
			}
		}

	}

}
