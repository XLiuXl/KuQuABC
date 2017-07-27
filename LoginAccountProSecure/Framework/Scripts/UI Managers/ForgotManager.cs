using System;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 
/// Forgot manager.
/// 
/// The forgot screen allow user to get an email with a link to reload their password with a generated one and get it in an email too.
/// 
/// </summary>
public class ForgotManager : ServerManager
{
	// UI components
	private InputField mailField = null;
	
	// Menus
	public string loginMenu = "Login";
	
	// Variables
	private string mail = "";



	void Awake()
	{
		InitManager(); // Initialize the serverManager (the class extended)

		// Get all the component fields
		mailField = GameObject.Find("MailField").GetComponent<InputField>();
	}
	
	
	///////////////////////////////// - Events - ///////////////////////////////////////////
	public void backLaunched()
	{
		UtilsProSecure.Load(loginMenu);
	}

	public void sendLaunched()
	{
		mail = mailField.text;

		// Launch register coroutine only if fields are corrects
		if(!mail.Contains("@")) { alertField.text = "Your email address is not valid."; return; }

		// Login on the server
		Forgot();
	}

	
	///////////////////////////////// - FORGOT - ///////////////////////////////////////////
	private void Forgot()
	{
		// If our fields are empty : no need to send the form
		if(mail == "") { return; }

		// Information to send to the server
		string[] datas = new string[1];
		datas[0] = mail;
		
		// Send datas on the server, then call 'handleForgotError' if the message of the server contains "ERROR". 'handleForgotSuccess' if not
		Send("Forgot", handleForgotSuccess, handleForgotError, datas);
	}
	private void handleForgotSuccess(string[] serverDatas)
	{
		alertField.text = serverDatas[0];
	}
	private void handleForgotError(string errorMessage)
	{
		Debug.Log(errorMessage);
		if(errorMessage.Contains("Failed to connect to mailserver"))
		{
			alertField.text = "Error: Email not sent, you didn't configure a mailserver to send emails.\nTry this process on your server, not on your machine.\nTry on your server online.";
		}
		else
		{
			alertField.text = errorMessage; // Show the server's message
		}
	}
}

