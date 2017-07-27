using UnityEngine;
using System.Collections;

public interface IUsable {
	void StartUsing(int handId);
	void StopUsing(int handId);
}
