using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class OutSideController : MonoBehaviour {
    private Vector3 playerPos = Vector3.zero;
    void Awake()
    {
      
    }
    void InitOutSide(Vector3 playerPos)
    {
        this.playerPos = playerPos;
        transform.position = playerPos - new Vector3(0, 50, 0);
    }

    void OnTriggerEnter(Collider  other)
    {
        StartCoroutine(WaitSomeTime(2.0f, other.name));
    }
    IEnumerator  WaitSomeTime(float time,string  name)
    {
        yield return new WaitForSeconds(time);
        Messenger.Broadcast<string>("OnHandObjClick", name);
    }
}
