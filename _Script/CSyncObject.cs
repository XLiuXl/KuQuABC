using UnityEngine;
using TNet;
using VRTK;

[RequireComponent(typeof(TNObject))]
[RequireComponent(typeof(Rigidbody))]

public class CSyncObject : VRTK_InteractableObject {

	private TNObject myTNObject;
	public bool useGravity = true;

	 void Start ()
	{
		
		if (this.GetComponent<Rigidbody> () == null)
			this.gameObject.AddComponent<Rigidbody> ();

        //Controller Type
	    //this.grabAttachMechanic=GrabAttachType.Child_Of_Controller;


		myTNObject = GetComponent<TNObject> ();
    }

	public override void OnInteractableObjectGrabbed (InteractableObjectEventArgs e)
	{
		base.OnInteractableObjectGrabbed (e);
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        myTNObject.SendQuickly ("SetRigidbody",Target.AllSaved,this.gameObject.name,false);

		myTNObject.SendQuickly ("UpdateRigidbodyConstraints",Target.AllSaved,this.gameObject.name);

		SetTrans ();

        Debug.LogFormat("OnInteractableObjectGrabbed:{0}", this.gameObject.name);
	}

	public override void OnInteractableObjectUngrabbed (InteractableObjectEventArgs e)
	{
		base.OnInteractableObjectUngrabbed (e);
      
        myTNObject.SendQuickly ("SetRigidbody",Target.AllSaved,this.gameObject.name,true);

		Invoke ("SendUpdatePos", 1f);

		CancelInvoke ("Send");

        Debug.LogFormat("OnInteractableObjectUngrabbed:{0}", this.gameObject.name);
    }

	[RFC]
	void UpdateRigidbodyConstraints(string obj){
		var go = GameObject.Find (obj);
		if(go!=null)
			go.gameObject.GetComponent<Rigidbody>().constraints= RigidbodyConstraints.None;


		Debug.Log ("UpdateRigidbodyConstraints===>"+go.gameObject.name);
		
	}


	public override void OnInteractableObjectUntouched (InteractableObjectEventArgs e)
	{
		base.OnInteractableObjectUntouched (e);

	}


	protected override void Update ()
	{
		base.Update ();
		GrabbingState ();
	}


	void GrabbingState(){
		if (IsGrabbed()) {
			Debug.Log ("isGrabbed....");
			SetTrans ();
		}
	}


	[RFC]
	void SetRigidbody(string name, bool state){
		var go = GameObject.Find (name);
		if(go!=null)
			go.GetComponent<Rigidbody> ().useGravity = state;

		Debug.Log ("LocalPlayer=>name="+name+"=>gravity="+state);
	}

	void SetTrans(){
		//Every sec sync all object
		InvokeRepeating ("Send", 0f, 1f);
	}

	void Send(){
		var pos = transform.position;
		var q = transform.rotation;
		myTNObject.SendQuickly ("SyncAllObject", Target.AllSaved, pos, q);
	}

	void SendUpdatePos(){
		var pos = transform.position;
		var q = transform.rotation;
		myTNObject.SendQuickly ("SyncPos",Target.AllSaved,pos,q);
	}

	[RFC]
	void SyncPos(Vector3 p,Quaternion q){
		transform.position = p;
		transform.rotation = q;
	}

	[RFC]
	void SyncAllObject(Vector3 pos,Quaternion q){
		transform.position = pos;
		transform.rotation = q;
	}
}
