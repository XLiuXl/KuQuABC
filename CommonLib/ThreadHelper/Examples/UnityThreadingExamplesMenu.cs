using UnityEngine;
using System.Collections;

public class UnityThreadingExamplesMenu : MonoBehaviour {

	void Awake()
	{
		UnityThreadHelper.EnsureHelper();
	}
	
    void OnGUI()
    {
        if (GUILayout.Button("Sort Example"))
        {
            this.gameObject.AddComponent<ThreadingSortExample>();
            DestroyImmediate(this);
        }
        if (GUILayout.Button("Unity API Calls"))
        {
            this.gameObject.AddComponent<ThreadingUnityCallsExample>();
            DestroyImmediate(this);
        }
    }
}
