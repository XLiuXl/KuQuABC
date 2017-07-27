using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



/// <summary>
/// This class allows us to check if the scenes are presents in the build settings
/// </summary>
using System.IO;


public class RenameHtaccess : MonoBehaviour
{
	public Object nextSceneToLoad;
	
	private Text alertField;
	private RectTransform nextStepButton;
	private RectTransform validIcon;
	
	private RectTransform generateButton;
	private RectTransform errorIcon;
	private SpriteRenderer loader;
	
	
	private float timeBeforeStart = 1f;
	
	void Start ()
	{
		// Get the UI components
		alertField = GameObject.Find("AlertMessage").GetComponent<Text>();
		nextStepButton = GameObject.Find("NextStep").GetComponent<RectTransform>();
		validIcon = GameObject.Find("ValidIcon").GetComponent<RectTransform>();
		generateButton = GameObject.Find("Generate").GetComponent<RectTransform>();
		errorIcon = GameObject.Find("ErrorIcon").GetComponent<RectTransform>();
		loader = GameObject.Find("Loader").GetComponent<SpriteRenderer>();
		
		// Start the verification
		StartCoroutine(RenameHtaccessFiles());
	}
	
	public void NextInstallationStep()
	{
		UtilsProSecure.Load(nextSceneToLoad.name);
	}
	public void GenerateHtaccessFiles()
	{
		#if UNITY_WEBPLAYER
		alertField.text = "No possible file generation while in webplayer plateform, please switch to another plateform in the build settings.";
		#else
		GenerateHtaccessFile(ConfigurationPaths.htaccessFile_game + ".htaccess");
		GenerateHtaccessFile(ConfigurationPaths.htaccessFile_website + ".htaccess");
		alertField.text = "Htaccess files created, you can continue the installation.";
		#endif
	}
	public void GenerateHtaccessFile(string path)
	{
		#if UNITY_WEBPLAYER
		noError = false;
		#else
		// Create and write in the file only if it doesn't already exists
		if (!File.Exists(path))
		{
			try
			{
				File.AppendAllText(path, "Deny from all");
			}
			catch(IOException e)
			{
				Debug.LogError(e);
			}
		}
		#endif
	}
	
	
	private IEnumerator RenameHtaccessFiles()
	{
		// Wait for the customer to know what we are testing here
		yield return new WaitForSeconds(timeBeforeStart);
		loader.color = new Color (255, 255, 255, 0);
		
		// Then rename the htaccess files for BOTH THE GAME AND WEBSITE
		RenameHtaccessFile("GAME", ConfigurationPaths.htaccessFile_game);
		RenameHtaccessFile("WEBSITE", ConfigurationPaths.htaccessFile_website);
		
		// Go to next step
		NextStep();
	}
	private void NextStep()
	{
		nextStepButton.localScale = Vector3.one;
		validIcon.localScale = Vector3.one;
		generateButton.localScale = Vector3.zero;
		errorIcon.localScale = Vector3.zero;
		alertField.text = "Htaccess files renamed, you can continue the installation.";
	}
	private void RenameHtaccessFile(string gameOrWebsite, string path)
	{
		#if UNITY_WEBPLAYER
		Debug.LogError("No possible file generation while in webplayer plateform, please switch to another plateform in the build settings.");
		#else
		// If the .htaccess already exists, no need to execute the renaming
		if(File.Exists(path + ".htaccess"))
		{
			Debug.Log("Success. Htaccess file for the " + gameOrWebsite + " has already been renamed -> no error found, you can continue.");
		}
		else
		{
			if(File.Exists(path + "htaccess"))
			{
				File.Move(path + "htaccess", path + ".htaccess");
			}
			else
			{
				GenerateHtaccessFiles();
			}
		}
		// Remove the htaccess.meta file
		if(File.Exists(path + "htaccess.meta"))
		{
			File.Delete(path + "htaccess.meta");
		}
		#endif
	}
}
