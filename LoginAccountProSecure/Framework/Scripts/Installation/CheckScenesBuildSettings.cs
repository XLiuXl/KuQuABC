using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



/// <summary>
/// This class allows us to check if the scenes are presents in the build settings
/// </summary>
public class CheckScenesBuildSettings : MonoBehaviour
{
	public Object nextSceneToLoad;


	private Text alertField;
	private CanvasGroup nextStepButton;
	private CanvasGroup validIcon;

	private RectTransform indications;
	private CanvasGroup helpButton;
	private CanvasGroup errorIcon;
	private SpriteRenderer loader;
	
	private bool allScenesInTheBuildSettings = false;
	private Dictionary<string, bool> scenesRequired;
	private List<string> scenesRequiredNames = new List<string>
	{
		"1 - Launch me first",
		"2 - Rename htaccess files",
		"3 - Send files on the server",
		"4 - Verify server URL",
		"5 - GameVersion + HashCode",
		"6 - Verify RSA security",
		"7 - Verify AES security",
		"8 - Install database",
		"9 - Verify database",
		"Finally"
	};

	private float timeBeforeStart = 1f;


	void Start ()
	{
		// Get the UI components
		alertField = GameObject.Find("AlertMessage").GetComponent<Text>();
		nextStepButton = GameObject.Find("NextStep").GetComponent<CanvasGroup>();
		validIcon = GameObject.Find("ValidIcon").GetComponent<CanvasGroup>();
		indications = GameObject.Find("Indications").GetComponent<RectTransform>();
		helpButton = GameObject.Find("Help").GetComponent<CanvasGroup>();
		errorIcon = GameObject.Find("ErrorIcon").GetComponent<CanvasGroup>();
		loader = GameObject.Find("Loader").GetComponent<SpriteRenderer>();

		// Start the verification
		StartCoroutine(CheckTheBuildSettings ());
	}

	public void NextInstallationStep()
	{
		if(allScenesInTheBuildSettings)
		{
			UtilsProSecure.Load(nextSceneToLoad.name);
		}
		else
		{
			alertField.text = "Please make sure all the scenes are in the build settings before going any further.";
		}
	}
	
	public void ShowHelp()
	{
		indications.localScale = Vector3.one;
	}
	public void HideHelp()
	{
		indications.gameObject.GetComponent<RectTransform> ().localScale = Vector3.zero;
	}

	private IEnumerator CheckTheBuildSettings()
	{
		// Wait for the customer to know what we are testing here
		yield return new WaitForSeconds(timeBeforeStart);
		loader.color = new Color (255, 255, 255, 0);

		// Then launch the checking
		scenesRequired = new Dictionary<string, bool> ();
		foreach(string s in scenesRequiredNames)
		{
			scenesRequired.Add(s,false);
		}
		
		ReadScenesNamesInBuildSettings();

		string aSceneNotPresent = "";
		bool allScenes = true;
		foreach(string s in scenesRequiredNames)
		{
			if(!scenesRequired[s])
			{
				aSceneNotPresent = s;
				//Debug.LogError("Please make sure the scene '"+s+"' is part of the build settings\n(File -> Build Settings...)");
				allScenes = false;
			}
		}
		allScenesInTheBuildSettings = allScenes;
		if(allScenesInTheBuildSettings)
		{
			nextStepButton.alpha = 1;
			validIcon.alpha = 1;
			helpButton.alpha = 0;
			errorIcon.alpha = 0;
			alertField.text = "Okay, all the scenes are in the build settings, you can continue the installation.";
		}
		else
		{
			nextStepButton.alpha = 0;
			validIcon.alpha = 0;
			helpButton.alpha = 1;
			errorIcon.alpha = 1;
			alertField.text = "Please make sure the scene '"+aSceneNotPresent+"' is part of the build settings (File -> Build Settings...)\n"+
				"You must have those scenes in the build settings :\n"+
				"\t1 - Launch me first\n"+
				"\t2 - Rename htaccess files\n"+
				"\t3 - Send files on the server\n"+
				"\t4 - Verify server URL\n"+
				"\t5 - GameVersion + HashCode\n"+
				"\t6 - Verify RSA security\n"+
				"\t7 - Verify AES security\n"+
				"\t8 - Install database\n"+
				"\t9 - Verify database\n"+
				"\tFinally\n\n"+
				"Please, add them and launch the scene [1 - Launch me first] again.";
		}
	}
	
	private void ReadScenesNamesInBuildSettings()
	{
		#if UNITY_EDITOR
		foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
		{
			if (scene.enabled)
			{
				string sceneName = scene.path.Substring(scene.path.LastIndexOf('/')+1);
				sceneName = sceneName.Substring(0,sceneName.Length-6);
				if(scenesRequiredNames.Contains(sceneName))
				{
					scenesRequired[sceneName] = true;
				}
			}
		}
		#endif
		return;
	}
}
