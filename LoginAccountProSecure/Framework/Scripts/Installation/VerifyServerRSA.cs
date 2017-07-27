using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;



/// <summary>
/// This class check if the RSA process is correctly computing on the server
/// </summary>
public class VerifyServerRSA : MonoBehaviour
{
	public Object nextSceneToLoad;
	
	private bool processExecutedCorrectly = false;
	//private string URLToServer;
	private Text alertField;
	
	private RectTransform nextStepButton;
	private RectTransform validIcon;
	private RectTransform errorIcon;
	private SpriteRenderer loader;
	
	// File utils
	private List<string> lines;
	
	
	void Start ()
	{
		// Get the UI components
		alertField = GameObject.Find("AlertMessage").GetComponent<Text>();
		nextStepButton = GameObject.Find("NextStep").GetComponent<RectTransform>();
		validIcon = GameObject.Find("ValidIcon").GetComponent<RectTransform>();
		errorIcon = GameObject.Find("ErrorIcon").GetComponent<RectTransform>();
		loader = GameObject.Find("Loader").GetComponent<SpriteRenderer>();
		
		// Fill the URLField with value from step 4
		//URLToServer = readURLFromConfigurationFile();
		
		// Launch RSA process
		launchRSAProcess();
	}
	
	public void NextInstallationStep()
	{
		if(processExecutedCorrectly)
		{
			UtilsProSecure.Load(nextSceneToLoad.name);
		}
		else
		{
			alertField.text = "Please make sure your URL is reachable to go to the next installation step.";
		}
	}
	
	public void launchRSAProcess()
	{
		hideSuccess();
		hideError();
		
		// Start the verification
		StartCoroutine(openSSLSession());
	}
	private IEnumerator openSSLSession()
	{
		// Then launch the checking
		processExecutedCorrectly = true;
		string URLtoServer = "www.360vrbox.com/LoginAccountProSecure/Installation/CheckRSA.php";
		string action = "CheckRSA";
		Debug.Log ("Connection to [" + URLtoServer +"]");
		
		// Preparing the web form
		WWWForm form = new WWWForm();
		form.AddField("Action", action);
		
		// Send the form to the server
		WWW w = new WWW(URLtoServer, form);
		loader.color = new Color (255, 255, 255, 1);
		alertField.text = "Request sent, please wait...";
		// Wait for the result
		yield return w;
		
		// Result arrived
		loader.color = new Color (255, 255, 255, 0);
		if(w.error != null || w.text.Contains("ERROR")) //ERROR
		{
			processExecutedCorrectly = false;
			alertField.text = "The server returned an error during the RSA process.";
			Debug.LogError("Server answer : "+w.error);
			// Clear the form
			w.Dispose();
		}
		else
		{
			Debug.Log(w.text);
			string separator = "<DATA_SEPARATOR>";
			if(w.text.Contains(separator)) // SUCCESS
			{
				string[] keys = w.text.Split(new string[] { separator }, System.StringSplitOptions.None);
				string privateKey = keys[0];
				string modulus = keys[1];
				string exponent = keys[2];
				if(generateCertificateFiles(privateKey, modulus, exponent))
				{
					// If everything worked well
					alertField.text = "RSA Certificate created, you can continue the installation.\n";
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
	}
	private bool generateCertificateFiles(string privateKey, string modulus, string exponent)
	{
		bool noError = true;
		
		#if UNITY_WEBPLAYER
		Debug.LogError("No possible file generation while in webplayer plateform, please switch to another plateform in the build settings.");
		#else
		// PUBLIC CERTIFICATE
		try
		{
			string certificatePublicKey = modulus + "<CERTIFICATE_SEPARATOR>" + exponent;
			File.WriteAllText(ConfigurationPaths.publicCertificateFile, certificatePublicKey);
			Debug.Log ("Public certificate created.");
		}
		catch(IOException e)
		{
			noError = false;
			Debug.LogError(e);
		}
		// PRIVATE CERTIFICATE
		try
		{
			File.WriteAllText(ConfigurationPaths.privateCertificateFile, privateKey);
			Debug.Log ("Private certificate created.");
		}
		catch(IOException e)
		{
			noError = false;
			Debug.LogError(e);
		}
		#endif
		return noError;
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
			File.WriteAllText(path, URL);
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

