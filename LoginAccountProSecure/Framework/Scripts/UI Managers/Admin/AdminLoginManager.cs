using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 
/// Administration Login manager.
/// 
/// Useful to manage users accounts
/// 
/// </summary>
public class AdminLoginManager : ServerManager
{
	// UI components
	private InputField usernameField = null;
	private InputField passwordField = null;

	// Menus
	public string Administration = "Administration";

	// Variables
	private string username = "";
	private string password = "";



	void Awake()
	{
		InitManager(); // Initialize the serverManager (the class extended)

		// Get UI components
		usernameField = GameObject.Find("LoginField").GetComponent<InputField>();
		passwordField = GameObject.Find("PasswordField").GetComponent<InputField>();

		// Initialize Login
		if(PlayerPrefs.HasKey("username"))
		{
			username = PlayerPrefs.GetString("username");
			usernameField.text = PlayerPrefs.GetString("username");
		}
		// Initialize Password
		if(PlayerPrefs.HasKey("password"))
		{
			password = PlayerPrefs.GetString("password");
			passwordField.text = PlayerPrefs.GetString("password");
		}
	}

	// UI keyboard events
	void Update()
	{
		// Next field
		if(Input.GetKeyDown(KeyCode.Tab))
		{
			if (usernameField.isFocused) focus (passwordField);
			else if (passwordField.isFocused) focus (usernameField);
		}
		// Submit
		if(Input.GetKeyDown(KeyCode.Return))
		{
			connectionLaunched ();
		}
	}
	private void focus(InputField field)
	{
		field.Select();
		field.ActivateInputField();
	}


	///////////////////////////////// - CALLBACKS EVENTS on UI components - ///////////////////////////////////////////
	public void connectionLaunched()
	{
		username = usernameField.text;
		password = passwordField.text;

		// Save the prefs of the player (even if the connection didn't reached the server)
		PlayerPrefs.SetString("username",username);
		PlayerPrefs.SetString("password",password);

		if(username.Length < 3) { alertField.text = "Your username must be at least 3 characters long."; return; }
		if(password.Length < 3) { alertField.text = "Your password must be at least 3 characters long."; return; }
		
		// Login on the server
		AdminLogin();
	}


	
	
	
	
	///////////////////////////////// - ADMIN LOGIN - ///////////////////////////////////////////
	private void AdminLogin()
	{
		// If our fields are empty : no need to send the form
		if(username == "" || password == "") { return; }
		
		// Information to send to the server (encrypted with RSA)
		string[] datas = new string[2];
		datas[0] = username;
		datas[1] = UtilsProSecure.hash(password); // No the password here is NOT salted, only the server can read it (the only one to have the certificate private key). So we let the server take care of the salt (if it's a registration it will generate a new salt and save it besides the salted password, if it's a login it will use the saved salt of the account)
		
		// Send datas on the server, then call 'handleLoginError' if the message of the server contains "ERROR". 'handleLoginSuccess' if not
		Send("AdminLogin", handleLoginSuccess, handleLoginError, datas);
	}
	private void handleLoginSuccess(string[] serverDatas)
	{
		// The connection is granted, move to main menu
		alertField.text = serverDatas[0];
		UserSession.session_id = serverDatas[1];			// Save SID the server gave us
		UserSession.loggedIn = true;						// Set the flag saying the user is logged in correctly
		UserSession.isAdmin = true;							// Set the flag saying the user is an administrator
		UserSession.username = username;					// Save login
		UserSession.password = password;					// Save password
		UtilsProSecure.Load(Administration);				// Log in
	}
	private void handleLoginError(string errorMessage)
	{
		Debug.Log(errorMessage);
		alertField.text = errorMessage; // Show the server's message
	}
}
