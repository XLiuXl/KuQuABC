using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemakeTrans : MonoBehaviour {
    private Vector3 originTrans = Vector3.zero;
    private Quaternion originQua = new Quaternion(0,0,0,0);
    void Awake( )
   {
        originTrans = transform.localPosition;
        originQua = transform.localRotation;
    }
    void OnEnable()
    {
        transform.localPosition = originTrans;
        transform.localRotation = originQua;
    }
}
