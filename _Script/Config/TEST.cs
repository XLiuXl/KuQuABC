using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var csnc = gameObject.AddComponent<CSyncObject>();
        csnc.enabled = true;
        csnc.isGrabbable = true;
        //layer
        gameObject.layer = LayerMask.NameToLayer("CanTake");
       GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
      transform.SetParent(null);
    }

}
