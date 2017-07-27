using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;


public class ServerManager : MonoBehaviour
{
	protected UILabel alertField = null;


	protected void InitManager()
	{
		alertField = GameObject.Find ("AlertMessage").GetComponent<UILabel> ();
	}


	/// <summary>
	/// Automatic sending data over the server and handling datas received
	/// Just specify a method to handle result with this signature
	/// void [METHOD_NAME] (string[] serverDatas)
	/// and test if serverDatas[0] contains "ERROR" or not
	/// If it contains "ERROR":
	/// -> it means something went wrong
	/// -> otherwise your get the server answers in 'serverDatas' array !ALREADY DECRYPTED!
	/// </summary>
	public void Send(string action, Action<string[]> methodForResult, Action<string> methodForError, string[] datas = null)
	{
		StartCoroutine(createForm (action, methodForResult, methodForError, datas));
	}


	/// <summary>
	/// This is the main function to communicate with the server
	/// Every script must use it because it tests absolutely everything automatically
	/// (See LoginManager and Example1Manager to see how it works)
	/// - If the parameter 'session' is set as NotConnectedYet -> the encryption will be made with RSA&AES: creation of a session on the server, generation (client side) of AES keys decrypted with RSA private certificate by the server
	/// - If the parameter 'session' is set as Connected -> the session token (generated client side) will be checked (both sides): see this session token as the key of the session
	/// </summary>
	private IEnumerator createForm(string action, Action<string[]> methodForResult, Action<string> methodForError, string[] infoToEncrypt = null)
	{
		if(action == "") { Debug.LogError("createForm: No action has been set."); yield break; }	// If no action has been specified -> stop
		
		// The optionnal information to send (encrypted of course)
		string concatenatedData = "";
		// Only set this field if there is something to send (it could be an action)
		if(infoToEncrypt != null)
		{
			// Transform info from string[] to string (with separators)
			for(int i=0; i<infoToEncrypt.Length; i++)
			{
				concatenatedData +=  infoToEncrypt[i] + UtilsProSecure.separator;
			}
		}
		// Create the web form to send to the server
		WWWForm form = new WWWForm();
		form.AddField("Action", action);
		form.AddField("Hash", AccountServerSettings.gameHashCode);
		form.AddField("GameVersion", AccountServerSettings.gameVersion);
		
		if(UserSession.loggedIn) // SSL connection already established
		{
			// Add the session ID (not encrypted of course, because the server has to get AES keys back if they exist in session array)
			if(UserSession.session_id == "") { Debug.LogError("ERROR, createForm: UserSession.session_id is empty."); }
			form.AddField("SID", UserSession.session_id);
		}
		else // SSL connection not established yet (encrypt information with RSA)
		{
			UtilsProSecure.PrepareSecurityInformation();	// Generate AES keys and encrypt it with RSA public key + Generate random session token + Read RSA public key from certificate
			string aesKeys = UserSession.AES_Key + UtilsProSecure.separator + UserSession.AES_IV;		// Brand new generated AES key and AES IV
			form.AddField("AESKeys", UtilsProSecure.RSA_encrypt(aesKeys));								// Encrypt AES keys to send with RSA public key
			form.AddField("NotConnectedYet", "true");											// Say that the server has to decrypt AES Keys
		}

		concatenatedData += UserSession.session_token;					// Add the session token
		concatenatedData = UtilsProSecure.AES_encrypt(concatenatedData);			// Encrypt the message with AES
		form.AddField("EncryptedInfo", concatenatedData);				// Send encrypted data
		
		// Create and return the form
		WWW w = new WWW("http://"+AccountServerSettings.URLtoServer, form);
		alertField.text = "Encrypted information sent, please wait...";			// Loading message...
		yield return w;															// Wait for the result

		// RESULT ARRIVED:
		if(w.error != null) // ERROR
		{
			alertField.text = "Server can't be reached: "+w.error;
			Debug.LogError(w.error);
			Debug.LogError("Server can't be reached. Did you configure the AccountServerSettings.cs script ?\nMake sure the Server.php script is well placed, and your AccountServerSettings.cs (in game) AND AccountServerSettings.php (on server) scripts are corrects.\n"+w.error);
			
			// Clear the form
			w.Dispose();
			yield break;
		}
		else if(w.text.Contains("ERROR") || w.text.Contains("Error") || w.text.Contains("error")) // ERROR
		{
			string errorMessage = w.text;
			// Clear the form
			w.Dispose();
			// Call the method with error
			methodForError(errorMessage);
			yield break;
		}
		else
		{
			string[] serverDatas = readServerDatas(w.text);
			// Clear the form
			w.Dispose();

			if(serverDatas!=null)
			{
				// Call the method with success
				methodForResult(serverDatas);
				yield break;
			}
			else { alertField.text = "Session tokens don't match!"; }
		}
	}
	
	/// <summary>
	/// The method to call when the server answers.
	/// The AES decryption is made to read the server's answer.
	/// It tests the session_token (to ensure security).
	/// And gives an array with all datas (the exact copy of what you sent in your PHP script).
	/// </summary>
	public static string[] readServerDatas(string encryptedData)
	{
		if(encryptedData=="") { Debug.LogError(""); return null; }
		if(UserSession.AES_Key=="" || UserSession.AES_IV=="") { Debug.LogError(""); return null; }

		// If the answer does not contains the delimitor : there is a problem
		if(!encryptedData.Contains(UtilsProSecure.delimitor))
		{
			Debug.LogError ("The server's answer does not contains any encryption delimitor, the server's answer is [" + encryptedData + "]");
			return null;
		}

		// Get the first string after the delimitor
		string encryptedDatasReceived = encryptedData.Split (new string[] { UtilsProSecure.delimitor }, StringSplitOptions.None)[1];
		// Split and return data once it's decrypted
		string[] datas = UtilsProSecure.AES_decrypt(encryptedDatasReceived).Split (new string[] { UtilsProSecure.separator }, StringSplitOptions.None);
		
		// Check if session token match !
		if(datas.Length <=0 || datas[datas.Length-1] != UserSession.session_token)
		{
			Debug.LogError("Session tokens don't match!");
			return null;
		}
		// If the session token is correct it means we talked to the server
		// Only the authentic server could possibly decrypt our RSA encrypted session token
		// It's here returned AES encrypted (so the server decrypted it with RSA private key)
		// Only the server could do that -> we talk to the server : continue our session
		return datas;
	}
}
