using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 
/// Login manager.
/// 
/// UI inputs and signals are recorded here, we ask for login and password, if it's correct we let the user come in.
/// This screen allow the user to access every fonctionnality we implemented for him.
/// 
/// </summary>
public class LoginManager : ServerManager
{
	// UI components
	private InputField usernameField = null;
	private InputField passwordField = null;
	private CanvasGroup resendButton = null;

	// Menus
	public string Example1 = "Example1";
	public string registerMenu = "Register";
	public string forgotMenu = "Forgot";

	// Variables
	private string username = "";
	private string password = "";


	void Awake()
	{
		InitManager(); // Initialize the serverManager (the class extended)

		// Get UI components
		usernameField = GameObject.Find("LoginField").GetComponent<InputField>();
		passwordField = GameObject.Find("PasswordField").GetComponent<InputField>();
		resendButton = GameObject.Find("Resend").GetComponent<CanvasGroup>();

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

	private void hideResend()
	{
		resendButton.interactable = false;
		resendButton.alpha = 0;
	}
	private void showResend()
	{
		resendButton.interactable = true;
		resendButton.alpha = 1;
	}


	///////////////////////////////// - CALLBACKS EVENTS on UI components - ///////////////////////////////////////////
	public void connectionLaunched()
	{
		hideResend();

		username = usernameField.text;
		password = passwordField.text;

		// Save the prefs of the player (even if the connection didn't reached the server)
		PlayerPrefs.SetString("username",username);
		PlayerPrefs.SetString("password",password);

		if(username.Length < 3) { alertField.text = "Your username must be at least 3 characters long."; return; }
		if(password.Length < 3) { alertField.text = "Your password must be at least 3 characters long."; return; }
		
		// Login on the server
		Login();
	}
	
	public void registerLaunched()
	{
		UtilsProSecure.Load(registerMenu);
	}
	
	public void forgotLaunched()
	{
		UtilsProSecure.Load(forgotMenu);
	}
	
	public void resendLaunched()
	{
		hideResend();

		username = usernameField.text;
		if(username.Length < 3) { alertField.text = "Your username must be 3 characters at least."; return; }

		// Resend the email activation
		Resend();
	}


	
	
	
	
	///////////////////////////////// - LOGIN - ///////////////////////////////////////////
	private void Login()
	{
		// If our fields are empty : no need to send the form
		if(username == "" || password == "") { return; }
		
		// Information to send to the server (encrypted with RSA)
		string[] datas = new string[2];
		datas[0] = username;
		datas[1] = UtilsProSecure.hash(password); // No the password here is NOT salted, only the server can read it (the only one to have the certificate private key). So we let the server take care of the salt (if it's a registration it will generate a new salt and save it besides the salted password, if it's a login it will use the saved salt of the account)
		
		// Send datas on the server, then call 'handleLoginError' if the message of the server contains "ERROR". 'handleLoginSuccess' if not
		Send("Login", handleLoginSuccess, handleLoginError, datas);
	}
	private void handleLoginSuccess(string[] serverDatas)
	{
		// The connection is granted, move to main menu
		alertField.text = serverDatas[0];
		UserSession.session_id = serverDatas[1];			// Save SID the server gave us
		UserSession.loggedIn = true;						// Set the flag saying the user is logged in correctly
		UserSession.username = username;					// Save login
		UserSession.password = password;					// Save password
		UtilsProSecure.Load(Example1);					// Log in
	}
	private void handleLoginError(string errorMessage)
	{
		Debug.Log(errorMessage);
		if(errorMessage.Contains("Your IP is not activated for this account, please enter your IP password or follow the link we sent you on your email address."))
		{
			alertField.text = "Your IP is not activated for this account, please enter your IP password or follow the link we sent you on your email address.";
			hideResend();
		}
		else if(errorMessage.Contains("Your account is not activated yet, please follow the link we sent you on your email address to activate it."))
		{
			// Show the server's message
			alertField.text = "Your account is not activated yet, please follow the link we sent you on your email address to activate it.";
			showResend();
		}
		else
		{
			alertField.text = errorMessage; // Show the server's message
		}
	}


	
	///////////////////////////////// - RESEND - ///////////////////////////////////////////
	private void Resend()
	{
		// If our fields are empty : no need to send the form
		if(username == "") { return; }
		
		// Information to send to the server (encrypted with RSA)
		string[] datas = new string[1];
		datas[0] = username;
		
		// Send datas on the server, then call 'handleResendError' if the message of the server contains "ERROR". 'handleResendSuccess' if not
		Send("Resend", handleResendSuccess, handleResendError, datas);
	}
	private void handleResendSuccess(string[] serverDatas)
	{
		alertField.text = serverDatas[0];
	}
	private void handleResendError(string errorMessage)
	{
		// Show the server's message
		Debug.Log(errorMessage);
		alertField.text = errorMessage;
	}
}
