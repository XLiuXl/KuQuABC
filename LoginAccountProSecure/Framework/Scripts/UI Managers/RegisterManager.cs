using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Register manager.
/// This screen allow anybody who open your game to register (as an account) in your database.
/// If information are correct, an account is created, with its associated IP (the IP is already activated)
/// First the account is disabled (user has to activate it with a link the server sent him)
/// (The user can try to connect to his account, and, on the login screen, we ask him if he wants we resend the email again, in case he didn't receive it)
/// The link is clicked, the URL code is checked, the account is activated with its associated email.
/// The user can connect
/// </summary>
public class RegisterManager : ServerManager
{
	// UI components
	private InputField mailField = null;
	private InputField usernameField = null;
	private InputField passwordField = null;
	private InputField confirmPasswordField = null;
	
	// Menus
	public string loginMenu = "Login";
	
	// Variables
	private string mail = "";
	private string username = "";
	private string password = "";
	private string confirmPassword = "";



	void Awake()
	{
		InitManager(); // Initialize the serverManager (the class extended)

		// Get all the component fields
		mailField = GameObject.Find("MailField").GetComponent<InputField>();
		usernameField = GameObject.Find("UsernameField").GetComponent<InputField>();
		passwordField = GameObject.Find("PasswordField").GetComponent<InputField>();
		confirmPasswordField = GameObject.Find("ConfirmPasswordField").GetComponent<InputField>();
	}

	// UI keyboard events
	void Update()
	{
		// Next field
		if(Input.GetKeyDown(KeyCode.Tab))
		{
			if (mailField.isFocused) focus (usernameField);
			else if (usernameField.isFocused) focus (passwordField);
			else if (passwordField.isFocused) focus (confirmPasswordField);
			else if (confirmPasswordField.isFocused) focus (mailField);
		}
		// Submit
		if(Input.GetKeyDown(KeyCode.Return))
		{
			registrationLaunched ();
		}
	}
	private void focus(InputField field)
	{
		field.Select();
		field.ActivateInputField();
	}

	
	
	///////////////////////////////// - Events - ///////////////////////////////////////////
	public void backLaunched()
	{
		UtilsProSecure.Load(loginMenu);
	}

	public void registrationLaunched()
	{
		mail = mailField.text;
		username = usernameField.text;
		password = passwordField.text;
		confirmPassword = confirmPasswordField.text;

		// Launch register coroutine only if fields are corrects
		if(!mail.Contains("@")) { alertField.text = "Your email address is not valid."; return; }
		if(username.Length < 3) { alertField.text = "Your username must be 3 characters at least."; return; }
		if(password.Length < 3) { alertField.text = "Your password must be 3 characters at least."; return; }
		if(!password.Equals(confirmPassword)) { alertField.text = "Your password confirmation do not match."; return; }

		// Login on the server
		Register();
	}


	
	///////////////////////////////// - REGISTER - ///////////////////////////////////////////
	private void Register()
	{
		// Information to send to the server (encrypted with RSA)
		string[] datas = new string[4];
		datas[0] = mail;
		datas[1] = username;
		datas[2] = UtilsProSecure.hash(password);
		
		// Send datas on the server, then call 'handleRegisterError' if the message of the server contains "ERROR". 'handleRegisterSuccess' if not
		Send("Register", handleRegisterSuccess, handleRegisterError, datas);
	}
	private void handleRegisterSuccess(string[] serverDatas)
	{
		// Save the prefs of the player (even if the connection didn't reached the server)
		PlayerPrefs.SetString("username",username);
		PlayerPrefs.SetString("password",password);
		alertField.text = serverDatas[0];
	}
	private void handleRegisterError(string errorMessage)
	{
		Debug.Log(errorMessage);
		alertField.text = errorMessage;
	}
}

