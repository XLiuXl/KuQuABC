using UnityEngine;
using System.Collections;

public interface ITouchable {
	void Touch (int handId);
	void Untouch (int handId);
}
