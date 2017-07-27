using UnityEngine;
using System.Collections;
using TNet;

public class ServerAutoCreate : MonoBehaviour {

	/// <summary>
	/// ID of the channel where the prefab should be created. '0' means "last used channel".
	/// </summary>

	public int channelID = 0;

	/// <summary>
	/// Prefab to instantiate.
	/// </summary>

	public string prefabPath;

	/// <summary>
	/// Whether the instantiated object will remain in the game when the player that created it leaves.
	/// Set this to 'false' for the player's avatar.
	/// </summary>

	public bool persistent = false;

	IEnumerator Start ()
	{
		while (TNManager.isJoiningChannel) yield return null;
		if (channelID < 1) channelID = TNManager.lastChannelID;

		if(TNManager.isHosting)
			TNManager.Instantiate(channelID, "CreateAtPosition", prefabPath, persistent, transform.position, transform.rotation);

		Destroy(gameObject);
	}

	[RCC]
	static GameObject CreateAtPosition (GameObject prefab, Vector3 pos, Quaternion rot)
	{
		// Instantiate the prefab
		GameObject go = prefab.Instantiate();

		// Set the position and rotation based on the passed values
		Transform t = go.transform;
		t.position = pos;
		t.rotation = rot;
		return go;
	}
}