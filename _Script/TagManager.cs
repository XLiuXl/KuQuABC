using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour {

    public enum TAG
    {
        None,
        GroundBig,
        GroundSmall,
        GroundTeacher
    }


    [SerializeField]
    public TAG kTag = TAG.None;

    void Awake()
    {
        this.gameObject.tag = kTag.ToString();
    }
}
