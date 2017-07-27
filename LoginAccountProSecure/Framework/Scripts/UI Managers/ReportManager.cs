using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// Report manager.
/// Used to allow players to report abuses they see in game
/// </summary>
public class ReportManager : ServerManager
{
	// Menus
	public string loginMenu = "Login";
	public string SceneToLoadOnBack = "Example1";
	
	// UI components
	private InputField reportMessageField = null;
	private RectTransform reportWindow = null;




	void Awake()
	{
		InitManager(); // Initialize the serverManager (the class extended)

		// Verify we did logged in
		if(!UserSession.loggedIn)
		{
			UtilsProSecure.Load(loginMenu);	// The connection is not established, go back to login menu
		}
		// Get UI fields
		reportMessageField = GameObject.Find("ReportMessageField").GetComponent<InputField>();
		reportWindow = GameObject.Find("ReportWindow").GetComponent<RectTransform>();
	}


	///////////////////////////////// - Events - ///////////////////////////////////////////
	public void reportLaunched()
	{
		StartCoroutine (Report());
	}
	public void backLaunched()
	{
		UtilsProSecure.Load(SceneToLoadOnBack);
	}


	public void ShowReport()
	{
		reportWindow.localScale = Vector3.one;
	}
	public void HideReport()
	{
		reportWindow.localScale = Vector3.zero;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/// 
	/// EXAMPLE to report any abuse a player could see in a scene.
	/// It takes screenshot when the report is made
	/// And the reporter player can add an optionnal message to it
	/// 
	///////////////////////////////////////////////////////////////////////////////////////////

	
	///////////////////////////////// - Report abuse - ///////////////////////////////////////////
	private IEnumerator Report()
	{
		HideReport ();
		// The path of the buffer file
		string screenshotPath = Application.persistentDataPath + "/Screenshot.png";
		// Remove the file if exists
		if(File.Exists (screenshotPath))
			File.Delete (screenshotPath);
		// Capture the screenshot
		Application.CaptureScreenshot (screenshotPath);
		// Wait for the screenshot to be taken (end of the fixed update)
		yield return new WaitForFixedUpdate();

		// The frame is over and the screenshot took
		string screenshot = Convert.ToBase64String(UtilsProSecure.readFile(screenshotPath));

		// Information to send to the server (encrypted with AES) in order to report an abuse
		string[] datas = new string[2];
		datas[0] = reportMessageField.text;
		datas[1] = screenshot;

		// Send datas on the server, then call 'handleSendDataError' if the message of the server contains "ERROR". 'handleSendDataSuccess' if not
		Send("Report", handleSuccess, handleError, datas);
	}
	private void handleSuccess(string[] serverDatas)
	{
		alertField.text = serverDatas[0];
	}
	private void handleError(string errorMessage)
	{
		Debug.Log(errorMessage);
		alertField.text = errorMessage; // Show the server's message
	}
}
