using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;



/// <summary>
/// This class check if the specified URL is existing and save it in the configuration file
/// </summary>
public class VerifyServerURL : MonoBehaviour
{
	public Object nextSceneToLoad;
	
	private bool URLVerified = false;
	
	private InputField URLField;
	private Text alertField;
	
	private RectTransform checkURLButton;
	private RectTransform nextStepButton;
	private RectTransform validIcon;
	private RectTransform errorIcon;
	private SpriteRenderer loader;
	
	// File utils
	private List<string> lines;
	
	
	void Start ()
	{
		// Get the UI components
		URLField = GameObject.Find("URLField").GetComponent<InputField>();
		alertField = GameObject.Find("AlertMessage").GetComponent<Text>();
		checkURLButton = GameObject.Find("CheckServerURL").GetComponent<RectTransform>();
		nextStepButton = GameObject.Find("NextStep").GetComponent<RectTransform>();
		validIcon = GameObject.Find("ValidIcon").GetComponent<RectTransform>();
		errorIcon = GameObject.Find("ErrorIcon").GetComponent<RectTransform>();
		loader = GameObject.Find("Loader").GetComponent<SpriteRenderer>();
		
		// Fill the URLField with previous value
		URLField.text = readURLFromConfigurationFile();
	}
	
	public void NextInstallationStep()
	{
		if(URLVerified)
		{
			UtilsProSecure.Load(nextSceneToLoad.name);
		}
		else
		{
			alertField.text = "Please make sure your URL is reachable to go to the next installation step.";
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
		URLVerified = true;
		string URLtoServer = URLField.text + "/LoginAccountProSecure/Installation/CheckURL.php";
		string action = "CheckURL";
		Debug.Log ("Connection to [" + URLtoServer +"]");
		
		// Preparing the web form
		WWWForm form = new WWWForm();
		form.AddField("Action", action);
		
		// Send the form to the server
		WWW w = new WWW(URLtoServer, form);
		checkURLButton.localScale = Vector3.zero;
		loader.color = new Color (255, 255, 255, 1);
		alertField.text = "Request sent, please wait...";
		// Wait for the result
		yield return w;
		
		// Result arrived
		loader.color = new Color (255, 255, 255, 0);
		if(w.error != null) //ERROR
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
			URLVerified = false;
		}
		else
		{
			Debug.Log(w.text);
			if(w.text.Contains("SUCCESS")) // SUCCESS
			{
				// If the URL does not contain "www." put a warning
				if(!URLField.text.Contains("www."))
				{
					saveConfigurationFile(URLField.text);
					alertField.text = "IMPORTANT WARNING! You should add a 'www.' prefix in front of your domain because redirections won't execute correctly. (You can do that in your CPanel).\n(You can continue the installation, ONLY if you know what you are doing.)";
				}
				else
				{
					// If everything worked well, and we save the configuration into our configuration file
					if(saveConfigurationFile(URLField.text))
					{
						alertField.text = "URL verified and saved, you can continue the installation.";
					}
					else
					{
						alertField.text = "Can't save the configuration, please make sure the file ["+ConfigurationPaths.configurationFile+"] is accessible and not read only.";
					}
				}
			}
			// Clear the form
			w.Dispose();
		}
		
		// Show messages
		if (URLVerified)
			showSuccess();
		else
			showError();
		
		// Show the button once again
		checkURLButton.localScale = Vector3.one;
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
	
	private bool saveConfigurationFile(string URL)
	{
		bool noError = true;
		
		#if UNITY_WEBPLAYER
		Debug.LogError("No possible file generation while in webplayer plateform, please switch to another plateform in the build settings.");
		#else
		string path = ConfigurationPaths.configurationFile;
		try
		{
			lines[3] = URL;
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
	private string readURLFromConfigurationFile()
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
		if(lines.Count >= 4)
		{
			return lines[3];
		}
		return "";
	}
}

