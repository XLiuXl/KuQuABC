using UnityEngine;
using System.Collections;
using System;

public class UITips : MonoBehaviour {
	void OnHover (){
		UITooltip.Hide ();
	}

	void OnClick(){
		UITooltip.Show ("input 'clear' remove all your words!");
	}
}
