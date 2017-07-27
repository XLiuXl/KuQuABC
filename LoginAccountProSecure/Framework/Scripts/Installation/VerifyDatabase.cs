using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;



/// <summary>
/// This class check if the specified URL is existing and save it in the configuration file
/// </summary>
public class VerifyDatabase : MonoBehaviour
{
	public Object nextSceneToLoad;
	
	private bool processExecutedCorrectly = false;
	private string URLToServer;
	
	// Fields
	private InputField URLField;
	private InputField hostField;
	private InputField nameField;
	private InputField userField;
	private InputField passwordField;
	// Buttons
	private RectTransform checkDatabaseButton;
	private RectTransform nextStepButton;
	// Icons
	private RectTransform validIcon;
	private RectTransform errorIcon;
	private SpriteRenderer loader;
	// Messages
	private Text alertField;
	
	// File utils
	private List<string> lines;
	
	
	void Start ()
	{
		// Get the UI components
		// Fields
		URLField = GameObject.Find("URLField").GetComponent<InputField>();
		hostField = GameObject.Find("DatabaseHost").GetComponent<InputField>();
		nameField = GameObject.Find("DatabaseName").GetComponent<InputField>();
		userField = GameObject.Find("DatabaseUser").GetComponent<InputField>();
		passwordField = GameObject.Find("DatabasePassword").GetComponent<InputField>();
		// Buttons
		checkDatabaseButton = GameObject.Find("CheckDatabase").GetComponent<RectTransform>();
		// Icons
		nextStepButton = GameObject.Find("NextStep").GetComponent<RectTransform>();
		validIcon = GameObject.Find("ValidIcon").GetComponent<RectTransform>();
		errorIcon = GameObject.Find("ErrorIcon").GetComponent<RectTransform>();
		loader = GameObject.Find("Loader").GetComponent<SpriteRenderer>();
		// Messages
		alertField = GameObject.Find("AlertMessage").GetComponent<Text>();
		
		// Fill the URLField with value from step 4
		readConfigurationFile();
		URLToServer = lines[3];
		URLField.text = URLToServer;
	}
	
	public void NextInstallationStep()
	{
		if(processExecutedCorrectly)
		{
			UtilsProSecure.Load(nextSceneToLoad.name);
		}
		else
		{
			alertField.text = "Please make sure your URL is reachable, your Database host set correctly and you are using the good database user to go to the next installation step.";
		}
	}
	
	public void verificationLaunched()
	{
		hideSuccess();
		hideError();
		
		// Start the verification
		StartCoroutine(verifyServerURL());
	}
	private IEnumerator verifyServerURL()
	{
		// Then launch the checking
		processExecutedCorrectly = true;
		string URLtoServer = URLToServer + "/LoginAccountProSecure/Installation/CheckDatabase.php";
		string action = "CheckDatabase";
		Debug.Log ("Connection to [" + URLtoServer +"]");
		
		// Preparing the web form
		WWWForm form = new WWWForm();
		form.AddField("Action", action);
		form.AddField("Host", hostField.text);
		form.AddField("Name", nameField.text);
		form.AddField("User", userField.text);
		form.AddField("Password", passwordField.text);
		
		// Send the form to the server
		WWW w = new WWW(URLtoServer, form);
		checkDatabaseButton.localScale = Vector3.zero;
		loader.color = new Color (255, 255, 255, 1);
		alertField.text = "Request sent, please wait...";
		// Wait for the result
		yield return w;
		
		// Result arrived
		loader.color = new Color (255, 255, 255, 0);
		if(w.error != null) // SERVER ERROR
		{
			if(URLtoServer.Contains("http") || URLtoServer.Contains("HTTP"))
			{
				alertField.text = "Please remove the 'http://' prefix and check the URL again.";
			}
			else
			{
				alertField.text = "Invalid URL. The server can't be reached.\nMake sure the URL is correct and your domain is activated.\nTry to watch the video from step 3 to make sure you did everything correctly.";
			}
			Debug.LogError("Server answer : "+w.error);
			// Clear the form
			w.Dispose();
			processExecutedCorrectly = false;
		}
		else
		{
			Debug.Log(w.text);
			if(w.text.Contains("ERROR")) // VERIFICATION ERROR
			{
				alertField.text = w.text;
				processExecutedCorrectly = false;
			}
			else // SUCCESS
			{
				alertField.text = "Great job, your database is installed correctly, the owner has privileges to execute requests, you can continue the installation.";
				// If everything worked well, and we saved the configuration into our configuration file
				if(saveConfigurationFile(hostField.text, nameField.text, userField.text, passwordField.text))
				{
					alertField.text = "Database information verified and saved, you can continue the installation.";
				}
				else
				{
					alertField.text = "Can't save the configuration, please make sure the file ["+ConfigurationPaths.configurationFile+"] is accessible and not read only.";
				}
			}
			// Clear the form
			w.Dispose();
		}
		
		// Show messages
		if (processExecutedCorrectly)
			showSuccess();
		else
			showError();
		
		// Show the button once again
		checkDatabaseButton.localScale = Vector3.one;
	}
	
	private void hideError()
	{
		errorIcon.localScale = Vector3.zero;
	}
	private void hideSuccess()
	{
		nextStepButton.localScale = Vector3.zero;
		validIcon.localScale = Vector3.zero;
	}
	private void showError()
	{
		nextStepButton.localScale = Vector3.zero;
		validIcon.localScale = Vector3.zero;
		errorIcon.localScale = Vector3.one;
	}
	private void showSuccess()
	{
		nextStepButton.localScale = Vector3.one;
		validIcon.localScale = Vector3.one;
		errorIcon.localScale = Vector3.zero;
	}
	
	private bool saveConfigurationFile(string host, string name, string user, string password)
	{
		// If the configuration file didn't even exist or something is missing, we can't go any further, the user has to complete the 4 and 5 steps before the 9
		if(lines.Count<3)
		{
			Debug.LogError("Please launch the step 4 & 5 before the step 9 because URL, game version or hash code are missing.");
			return false;
		}
		
		// Specify the new datas
		if(lines.Count<7)
		{
			lines.Add(host);
			lines.Add(name);
			lines.Add(user);
			lines.Add(password);
		}
		else
		{
			lines[6] = host;
			lines[7] = user;
			lines[8] = password;
			lines[9] = name;
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
	private void readConfigurationFile()
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
		if(lines.Count >= 10)
		{
			hostField.text = lines[6];
			userField.text = lines[7];
			passwordField.text = lines[8];
			nameField.text = lines[9];
		}
		else if(lines.Count < 3)
		{
			Debug.LogError("Please complete the fourth step before you do this one. No URL has been found in the configuration file.");
		}
	}
}

