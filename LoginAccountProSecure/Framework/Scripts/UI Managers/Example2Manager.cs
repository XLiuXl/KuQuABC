using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Main menu manager.
/// This screen is only implemented to show you how to get sensible information from the server and decrypt it (AES encryption)
/// </summary>
using System.Text;


public class Example2Manager : ServerManager
{
	// Menus
	public string loginMenu = "Login";
	public string Example1 = "Example1";



	void Awake()
	{
		InitManager(); // Initialize the serverManager (the class extended)

		// Verify we did logged in
		if(!UserSession.loggedIn) { UtilsProSecure.Load(loginMenu); }	// The connection is not established, go back to login menu
	}
	
	///////////////////////////////// - Events - ///////////////////////////////////////////
	public void getSomethingFromServerLaunched()
	{
		Savefile();
	}
	public void backLaunched()
	{
		// Back
		UtilsProSecure.Load(Example1);
	}
	

	///////////////////////////////// - SAVEFILE - ///////////////////////////////////////////
	private void Savefile()
	{
		// Read a saveGameFile
		//TextAsset bindata = Resources.Load("Savegames files/Savegame1") as TextAsset; // CAUTION your resource must have a .txt extension
		//string savefile = Application.dataPath+"/LoginAccountProSecure/Framework/Resources/Savegames files/Savegame1.txt";
		//string savegame = UtilsProSecure.readTextFile(savefile);
		//string savegame = bindata.text;
		//savegame = UtilsProSecure.To64String(savegame); 	// Encode it in base64 string to make sure no character will be replaced

		// OR (if you want you can easily switch from one to another

		// Send image file
		Texture2D tex = Resources.Load("Savegames files/Icon") as Texture2D;
		string savegame = Convert.ToBase64String(tex.EncodeToPNG());

		// Information to send to the server
		string[] datas = new string[2];
		datas[0] = "Savegame1";
		datas[1] = savegame;
		
		// Send datas on the server, then call 'handleSavefileError' if the message of the server contains "ERROR". 'handleSavefileSuccess' if not
		Send("Savefile", handleSavefileSuccess, handleSavefileError, datas);
	}
	private void handleSavefileSuccess(string[] serverDatas)
	{
		alertField.text = serverDatas[0];

		// Here, if you build the solution make sure you can reach the folder you specify here (depending on where the build is located)
		string mySaveGamePath = Application.dataPath+"/Savegame1_Received.txt";
		#if UNITY_EDITOR
		mySaveGamePath = Application.dataPath+"/LoginAccountProSecure/Framework/Resources/Savegames files/Savegame1_Received.png";
		#endif

		#if UNITY_WEBPLAYER
		Debug.Log(mySaveGamePath);
		alertField.text = "If you are on webplayer you can't send/receive files, switch plateform in your build settings.";
		#else
		byte[] receivedBytes = Convert.FromBase64String(serverDatas[1]);
		UtilsProSecure.writeBytesInFile(mySaveGamePath, receivedBytes);

		//string fileReceived = UtilsProSecure.From64String(serverDatas[1]); // Decode it from base64 string (we sent it like that)
		//UtilsProSecure.writeTextInFile(mySaveGamePath, fileReceived);
		#endif
	}
	private void handleSavefileError(string errorMessage)
	{
		Debug.Log(errorMessage);
		alertField.text = errorMessage; // Show the server's message
	}
}
