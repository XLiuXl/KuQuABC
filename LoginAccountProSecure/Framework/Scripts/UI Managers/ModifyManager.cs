using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 
/// Modify manager.
/// 
/// First login and password are asked (it's fill automatically to speed up the process)
/// Second all empty fields will be ignored, all specified information will be changed, only if their are correct
/// (example : an email or a login can be already used, we inform the user if an error occurs)
/// 
/// </summary>
public class ModifyManager : ServerManager
{
	// UI components
	private InputField mailField = null;
	private InputField usernameField = null;
	private InputField currentPasswordField = null;
	private InputField passwordField = null;
	private InputField confirmPasswordField = null;
	
	// Menus
	public string loginMenu = "Login";
	public string Example1Menu = "Example1";
	
	// Variables
	private string mail = "";
	private string username = "";
	private string currentPassword = "";
	private string password = "";
	
	
	// Awake
	void Awake()
	{
		InitManager(); // Initialize the serverManager (the class extended)

		// Verify we did logged in
		if(!UserSession.loggedIn)
		{
			// The connection is not established, go back to login menu
			UtilsProSecure.Load(loginMenu);
		}

		// Get all the component fields
		mailField = GameObject.Find("MailField").GetComponent<InputField>();
		usernameField = GameObject.Find("UsernameField").GetComponent<InputField>();
		currentPasswordField = GameObject.Find("CurrentPasswordField").GetComponent<InputField>();
		passwordField = GameObject.Find("PasswordField").GetComponent<InputField>();
		confirmPasswordField = GameObject.Find("ConfirmPasswordField").GetComponent<InputField>();
	}
	
	
	///////////////////////////////// - Events - ///////////////////////////////////////////
	public void backLaunched()
	{
		UtilsProSecure.Load(Example1Menu);
	}
	
	public void modfiyLaunched()
	{
		mail = mailField.text;
		username = usernameField.text;
		currentPassword = currentPasswordField.text;
		password = passwordField.text;

		// Launch register coroutine only if fields are corrects
		if(currentPassword != UserSession.password) { alertField.text = "Your current password is not correct."; return; }
		if(password != confirmPasswordField.text) { alertField.text = "Your password confirmation do not match."; return; }
		if(mail.Length>0 && !mail.Contains("@")) { alertField.text = "Your email address is not valid."; return; }
		if(password.Length>0 && password.Length<3) { alertField.text = "Your password must be at least 3 characters long."; return; }
		
		// Don't send request if no information to update
		if(String.IsNullOrEmpty(mail) && String.IsNullOrEmpty(username) && String.IsNullOrEmpty(password))
		{
			alertField.text = "No information to update.";
		}
		else
		{
			Modify();
		}
	}


	
	///////////////////////////////// - MODIFY - ///////////////////////////////////////////
	private void Modify()
	{
		// Information to send to the server (encrypted with AES)
		string[] datas = new string[3];
		datas[0] = mail;
		datas[1] = username;
		datas[2] = UtilsProSecure.hash(password);
		
		// Send datas on the server, then call 'handleModifyError' if the message of the server contains "ERROR". 'handleModifySuccess' if not
		Send("Modify", handleModifySuccess, handleModifyError, datas);
	}
	private void handleModifySuccess(string[] serverDatas)
	{
		alertField.text = serverDatas[0];
		// Save information
		if(!String.IsNullOrEmpty(username)) { PlayerPrefs.SetString("username",username); }
		if(!String.IsNullOrEmpty(password)) { PlayerPrefs.SetString("password",password); }
	}
	private void handleModifyError(string errorMessage)
	{
		if(errorMessage.Contains("Failed to connect to mailserver"))
		{
			Debug.LogError("Modification completed, but you didn't configure a mailserver to send emails. Try on your server online.");
			alertField.text = "Can't send emails. Please contact an administrator.";
		}
		else
		{
			alertField.text = errorMessage; // Show the server's message
		}
	}
}

