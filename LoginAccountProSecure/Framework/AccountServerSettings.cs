
using UnityEngine;
using System.Collections;

public static class AccountServerSettings
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 
	/// Server configuration
	/// 
	//////////////////////////////////////////////////////////////////////////////////////////////////////


	// CAUTION !!
	// All information here MUST match the information you have specified in the 'AccountServerSettings.php' script !


	// The game hash code must be the same here than in your php file 'AccountServerSettings.php' on the server
	// Just put random characters here (be caution not to put forbidden characters though)
	public static string gameHashCode = "CoLwRMQYyNvLe5tjQSIM4TjVaFRd2KOYrHpYBuzJghFqC95kbq#FKciW";
	// The version of the game, the server will compare this number with his own and alert the client if a new version is available
	// Leave 1.0 if it's not important for you right now
	public static string gameVersion = "1.0";
	// The URL to reach the connection script named 'Server.php' (initially the folder name is : LoginAccount+ProSecure, if you didn't change it)
	public static string URLtoServer = "www.360vrbox.com/LoginAccountProSecure/Game/Server.php";			
}