using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// AutoRegister manager.
/// This screen allow administrators to automatically generate as many accounts as they want by specifying the number of accounts needed
/// </summary>
public class AutoRegisterManager : ServerManager
{
	// UI components
	private InputField mailField = null;
	private InputField usernameField = null;
	private InputField passwordField = null;
	private InputField confirmPasswordField = null;
	
	// Menus
	public string loginMenu = "Login";
	public int NumberOfAccountsToGenerate = 10;
	
	// Variables
	private string mail = "";
	private string username = "";
	private string password = "";
	private string confirmPassword = "";
	private int numberOfAccountsCreated = 0;



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
		if(numberOfAccountsCreated > NumberOfAccountsToGenerate)
		{
			numberOfAccountsCreated = 0;
			alertField.text = "All " + NumberOfAccountsToGenerate + " have been generated! Get their information in the file 'GeneratedAccounts.txt'.";
			return;
		}

		// Generate fields
		mailField.text = UtilsProSecure.randomString(20) + "@mail.com";
		usernameField.text = UtilsProSecure.randomString(20);
		passwordField.text = UtilsProSecure.randomString(20);
		confirmPasswordField.text = passwordField.text;

		mail = mailField.text;
		username = usernameField.text;
		password = passwordField.text;
		confirmPassword = confirmPasswordField.text;

		// Launch register coroutine only if fields are corrects
		if(!mail.Contains("@")) { alertField.text = "Your email address is not valid."; return; }
		if(username.Length < 3) { alertField.text = "Your username must be 3 characters at least."; return; }
		if(password.Length < 3) { alertField.text = "Your password must be 3 characters at least."; return; }
		if(!password.Equals(confirmPassword)) { alertField.text = "Your password confirmation do not match."; return; }

		// Save credentials in the GeneratedAccounts.txt file
		string filePath = Application.dataPath + "/LoginAccountProSecure/Framework/Scripts/UI Managers/Admin/Resources/GeneratedAccounts.txt";
		string lineToAdd = mail + " " + username + " " + password + Environment.NewLine;
		File.AppendAllText(filePath, lineToAdd);

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
		// PlayerPrefs.SetString("username",username);
		// PlayerPrefs.SetString("password",password);
		alertField.text = serverDatas[0];

		// Launch the next registration again
		numberOfAccountsCreated++;
		registrationLaunched();
	}
	private void handleRegisterError(string errorMessage)
	{
		Debug.Log(errorMessage);
		alertField.text = errorMessage;

		// Launch the next registration again
		numberOfAccountsCreated++;
		registrationLaunched();
	}
}

