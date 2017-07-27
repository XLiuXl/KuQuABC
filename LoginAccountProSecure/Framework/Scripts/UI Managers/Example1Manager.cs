using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Main menu manager.
/// This screen is only implemented to show you how to get sensible information from the server and decrypt it (AES encryption)
/// </summary>
public class Example1Manager : ServerManager
{
	// Menus
	public string loginMenu = "Login";
	public string modifyMenu = "Modify";
	public string Report = "ReportUser";
	public string Example2 = "Example2";
	
	// UI components
	private InputField Data1Field = null;
	private InputField Data2Field = null;
	private InputField Data3Field = null;




	void Awake()
	{
		InitManager(); // Initialize the serverManager (the class extended)

		// Verify we did logged in
		if(!UserSession.loggedIn)
		{
			UtilsProSecure.Load(loginMenu);	// The connection is not established, go back to login menu
		}
		// Get UI fields
		Data1Field = GameObject.Find("Data1Field").GetComponent<InputField>();
		Data2Field = GameObject.Find("Data2Field").GetComponent<InputField>();
		Data3Field = GameObject.Find("Data3Field").GetComponent<InputField>();
	}

	// UI keyboard events
	void Update()
	{
		// Next field
		if(Input.GetKeyDown(KeyCode.Tab))
		{
			if (Data1Field.isFocused) focus (Data2Field);
			else if (Data2Field.isFocused) focus (Data3Field);
			else if (Data3Field.isFocused) focus (Data1Field);
		}
		// Submit
		if(Input.GetKeyDown(KeyCode.Return))
		{
			sendDataLaunched ();
		}
	}
	private void focus(InputField field)
	{
		field.Select();
		field.ActivateInputField();
	}


	///////////////////////////////// - Events - ///////////////////////////////////////////
	public void sendDataLaunched()
	{
		SendData();
	}
	public void getDataLaunched()
	{
		GetData();
	}
	
	public void modifyLaunched()
	{
		UtilsProSecure.Load(modifyMenu);
	}

	public void ReportLaunched()
	{
		UtilsProSecure.Load(Report);
	}
	
	public void Example2Launched()
	{
		UtilsProSecure.Load(Example2);
	}
	
	public void disconnectLaunched()
	{
		UserSession.clearSession();			// Clear the session
		UtilsProSecure.Load(loginMenu);	// Disconnection, go back to login menu
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/// 
	/// EXAMPLE to get or send or save something to the server
	/// Very easy to use and understand !
	/// 
	///////////////////////////////////////////////////////////////////////////////////////////

	
	///////////////////////////////// - SEND DATA - ///////////////////////////////////////////
	private void SendData()
	{
		// Information to send to the server (encrypted with AES)
		string[] datas = new string[3];
		datas[0] = Data1Field.text;
		datas[1] = Data2Field.text;
		datas[2] = Data3Field.text;
		// Here you can add as many values as you want
		// IMPORTANT don't forget to set the array size (here it's 3, you MUST set the exact number of values you send)

		// Send datas on the server, then call 'handleSendDataError' if the message of the server contains "ERROR". 'handleSendDataSuccess' if not
		Send("SendData", handleSendDataSuccess, handleSendDataError, datas);
	}
	private void handleSendDataSuccess(string[] serverDatas)
	{
		alertField.text = serverDatas[0];
	}
	private void handleSendDataError(string errorMessage)
	{
		Debug.Log(errorMessage);
		alertField.text = errorMessage; // Show the server's message
	}

	
	///////////////////////////////// - GET DATA - ///////////////////////////////////////////
	private void GetData()
	{
		// Send datas on the server, then call 'handleGetDataError' if the message of the server contains "ERROR". 'handleGetDataSuccess' if not
		Send("GetData", handleGetDataSuccess, handleGetDataError, null);
	}
	private void handleGetDataSuccess(string[] serverDatas)
	{
		alertField.text = "Information read online:";
		alertField.text += "\n" + serverDatas[1];
		alertField.text += "\n" + serverDatas[2];
		alertField.text += "\n" + serverDatas[3];
	}
	private void handleGetDataError(string errorMessage)
	{
		Debug.Log(errorMessage);
		alertField.text = errorMessage; // Show the server's message
	}
}
