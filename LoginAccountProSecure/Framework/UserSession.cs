using UnityEngine;
using System.Collections;

/// <summary>
/// User session is the class where all user options are saved, it's static so it's saved between Unity scenes.
/// Once the login is completed, you can access any user information by simply use : UserSession.[member]
/// Example : string playerNickname = UserSession.username;
/// </summary>
public static class UserSession
{
	public static bool loggedIn = false;			// Is the user logged in or not ?
	public static string username = "";				// The username used for connection
	public static string password = "";				// The password used for connection
	public static string session_id = "";			// The SID sent to the server to reload session information concerning the user (like session_token, AES keys, ...)
	public static string session_token = "";		// The session 'secret key' checked both sides to make sure the communication has been made between the real user and the real server (generated everytime a session is started)

	// Administration
	public static bool isAdmin = false;				// Is the user an administrator ?

	// Security
	public static string publicModulus = "";		// RSA keys (public only)
	public static string publicExponent = "";		// RSA exponent (read from the public certificate too)
	public static string AES_Key = "";				// The AES key of the session
	public static string AES_IV = "";				// The AES initial vector of the session
	
	// Reinitialization method
	public static void clearSession()
	{
		loggedIn = false;
		username = "";
		password = "";
		session_id = "";
		session_token = "";
		isAdmin = false;
		publicModulus = "";
		publicExponent = "";
		AES_Key = "";
		AES_IV = "";
	}
}
