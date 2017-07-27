using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
public class SyncHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
        TNAutoSync sync = this.gameObject.AddComponent<TNAutoSync>();
        TNAutoSync.SavedEntry ent1 = new TNAutoSync.SavedEntry();
        ent1.target = sync.GetComponent<Transform>();
        ent1.propertyName = "position";
        sync.entries.Add(ent1);

        TNAutoSync.SavedEntry ent2 = new TNAutoSync.SavedEntry();
        ent2.target = sync.GetComponent<Transform>();
        ent2.propertyName = "rotation";
        sync.entries.Add(ent2);

        sync.isSavedOnServer = false;
        sync.onlyOwnerCanSync = false;

        sync.enabled = true;

        var t = GetComponent<TNObject>();
        t.Register();
        t.channelID = TNManager.lastChannelID;

        Debug.LogFormat("..............................This {0} gameobject can sync [{1}]",this.gameObject.name,t.hasBeenRegistered);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
