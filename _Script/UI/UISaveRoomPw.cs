using UnityEngine;
using System.Collections;

public class UISaveRoomPw : MonoBehaviour {

    public UIInput pw;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SavePw()
    {
        PlayerPrefs.SetString("Vr_Mulit_RoomPw", pw.value);
    }
}
