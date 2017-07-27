using UnityEngine;
using System.Collections;
using TNet;

public enum ObjectSnap
{
    Grip,
    Exact
}

public class InteractableObject : TNBehaviour
{

    public ObjectSnap snapType;
    public bool isUsable;
	public bool isGrabbable;
    
	[RFC]
	public void RpcAttachToHand(int handId)
    {
        var hand = GameObject.Find(TNManager.GetPlayer(handId).name);
        if (hand == null)
            return;
        AttachToHand(hand);
    }
      
    public void AttachToHand(GameObject hand)
    {
        var attachpoint = hand.transform.Find("Attachpoint");

        switch (snapType)
        {
            case ObjectSnap.Exact:
                transform.parent = attachpoint.transform;
                break;
            case ObjectSnap.Grip:
                Helper.AttachAtGrip(attachpoint, transform);
                break;
        }

        GetComponent<Rigidbody>().isKinematic = true;
        //GetComponent<TNObject>().enabled = false;
    }
    
    [RFC]
	public void RpcDetachFromHand(Vector3 currentHolderVelocity)
	{
        DetachFromHand(currentHolderVelocity);
    }

    public void DetachFromHand(Vector3 currentHolderVelocity)
    {
        transform.parent = null;
        var rigidbodyOfObject = GetComponent<Rigidbody>();
        rigidbodyOfObject.isKinematic = false;
        rigidbodyOfObject.velocity = currentHolderVelocity*1.5f;
        //GetComponent<TNObject>().enabled = true;
    }
}