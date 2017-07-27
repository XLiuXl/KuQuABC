using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;



/// <summary>
/// This class 
/// </summary>
public class VersionAndHashCode : MonoBehaviour
{
	public Object nextSceneToLoad;
	
	private InputField nameField;
	private InputField versionField;
	private InputField hashField;
	
	private RectTransform nextStepButton;
	
	// Characters available for random strings
	static string stringCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890?#";
	
	// File utils
	private List<string> lines;
	
	
	
	void Start ()
	{
		// Get the UI components
		nameField = GameObject.Find("GameName").GetComponent<InputField>();
		versionField = GameObject.Find("GameVersion").GetComponent<InputField>();
		hashField = GameObject.Find("HashCode").GetComponent<InputField>();
		nextStepButton = GameObject.Find("NextStep").GetComponent<RectTransform>();
		
		// Get already saved hashCode
		readVersionAndHashSaved();
		
		if(hashField.text != "")
		{
			showNextStep();
		}
	}
	
	public void NextInstallationStep()
	{
		if(saveConfigurationFile(nameField.text, versionField.text, hashField.text))
		{
			UtilsProSecure.Load(nextSceneToLoad.name);
		}
		else
		{
			Debug.LogError("Can't save the version and the hash code in the file ["+ConfigurationPaths.configurationFile+"]. Make sure it's accessible and not read only.");
		}
	}
	
	public void GenerateHashCode()
	{
		showNextStep();
		int stringCharactersLength = stringCharacters.Length;
		string s = string.Empty;
		for(int i=0; i<56; ++i)
		{
			s += stringCharacters[UnityEngine.Random.Range(0, stringCharactersLength)];
		}
		hashField.text = s;
		showNextStep();
	}
	
	private void showNextStep()
	{
		nextStepButton.localScale = Vector3.one;
	}
	
	private bool saveConfigurationFile(string name, string version, string hash)
	{
		// If the configuration file didn't even exist or didn't contain the URL, we can't go any further, the user has to complete the fourth step before the fifth
		if(lines.Count<1)
		{
			Debug.LogError("Please launch the step 4 before the step 5 because no URL is configured yet.");
			return false;
		}
		
		// Specify the new game version and hash code
		if(lines.Count<3)
		{
			lines.Add(name);
			lines.Add(version);
			lines.Add(hash);
		}
		else
		{
			lines[0] = name;
			lines[1] = version;
			lines[2] = hash;
		}

		bool noError = true;
		
		#if UNITY_WEBPLAYER
		Debug.LogError("No possible file generation while in webplayer plateform, please switch to another plateform in the build settings.");
		#else
		string path = ConfigurationPaths.configurationFile;
		try
		{
			File.WriteAllLines(path, lines.ToArray());
		}
		catch(IOException e)
		{
			noError = false;
			Debug.LogError(e);
		}
		#endif
		return noError;
	}
	private void readVersionAndHashSaved()
	{
		string path = ConfigurationPaths.configurationFile;
		lines = new List<string>();
		
		// If the file exists, read the URL from it if you can
		if(File.Exists(path))
		{
			try
			{
				lines.AddRange(File.ReadAllLines(path));
			}
			catch(IOException e)
			{
				Debug.LogError(e);
			}
		}
		if(lines.Count >= 3)
		{
			nameField.text = lines[0];
			versionField.text = lines[1];
			hashField.text = lines[2];
		}
		else if(lines.Count <= 0)
		{
			Debug.LogError("Please launch the step 4 before the step 5 because no URL is configured yet.");
		}
	}
}


