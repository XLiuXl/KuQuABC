using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagEnum : MonoBehaviour {
    public enum TagType
    {
        target,
        Button,
        Tween,
        TwwenButton,
    }
    public TagType currentType;

}
