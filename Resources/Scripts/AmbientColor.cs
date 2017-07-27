using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AmbientColor : MonoBehaviour {
    [SerializeField]
    public Color32 ambient;
	void Start () {
        RenderSettings.ambientLight = ambient;
	}
}
