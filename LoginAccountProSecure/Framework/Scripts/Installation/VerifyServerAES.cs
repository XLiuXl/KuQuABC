using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;



/// <summary>
/// This class check if the AES process is correctly computing on the server
/// </summary>
using System;


public class VerifyServerAES : MonoBehaviour
{
	public UnityEngine.Object nextSceneToLoad;
	
	private bool processExecutedCorrectly = false;
	private string URLToServer;
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
		URLToServer = readURLFromConfigurationFile();

        Debug.Log("URL=>"+URLToServer);
		
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
		StartCoroutine(EstablishAESSecurity());
	}
	
	protected IEnumerator EstablishAESSecurity()
	{
		string URLtoServer = URLToServer + "/LoginAccountProSecure/Installation/CheckAES.php";
		
		// Generate AES key
		UtilsProSecure.generateAESkeyAndIV(ref UserSession.AES_Key, ref UserSession.AES_IV);
		
		// Preparing the web form
		WWWForm form = new WWWForm();
		form.AddField("Action", "CheckAES");
		form.AddField("AES_KEYS", UserSession.AES_Key+"<AES_KEYS_SEPARATOR>"+UserSession.AES_IV);
		
		// Send the form to the server
		WWW w = new WWW(URLtoServer, form);
		loader.color = new Color (255, 255, 255, 1);
		alertField.text = "Encrypted information sent, please wait...";
		// Wait for the result
		yield return w;
		
		// Result arrived
		loader.color = new Color (255, 255, 255, 0);
		if(w.error != null) //ERROR
		{
			processExecutedCorrectly = false;
			alertField.text = "The server returned an error during the AES process.";
			Debug.LogError("Server answer : "+w.error);
			// Clear the form
			w.Dispose();
		}
		else
		{
			Debug.Log(w.text);
			if(w.text.Contains("ERROR"))
			{
				alertField.text = w.text;
				processExecutedCorrectly = false;
			}
			else
			{
				string separator = "<ENCRYPTED_DATA_DELIMITOR>";
				if(w.text.Contains(separator)) // SUCCESS
				{
					processExecutedCorrectly = true;
					string encryptedDatasReceived = w.text.Split (new string[] { "<ENCRYPTED_DATA_DELIMITOR>" }, StringSplitOptions.None)[1];
					// Split and return data once it's decrypted
					string[] datas = UtilsProSecure.AES_decrypt(encryptedDatasReceived).Split (new string[] { "<DATA_SEPARATOR>" }, StringSplitOptions.None);
					string aesKey = datas[0];
					string aesIV = datas[1];
					// If everything worked well
					alertField.text = "AES process correctly executed, you can continue the installation." + "\n\nAES_KEY : "+ aesKey + "\n\nAES_IV : "+ aesIV;
				}
			}
		}
		// Clear the form
		w.Dispose();
		
		// Show messages
		if (processExecutedCorrectly)
		{
			showSuccess();
		}
		else
		{
			showError();
		}
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


