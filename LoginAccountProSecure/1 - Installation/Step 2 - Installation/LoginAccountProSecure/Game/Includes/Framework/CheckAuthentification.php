<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Read private certificate to decrypt RSA information
//			Check if account is not blocked (momentanely)
//			Check if username and password are correct
//			-> If not block and information correct :
//				Check if username not already in a session
//			 	-> If no session for the past 10 minutes :
//			 		Open session
//				-> If already in a session
//					Ask for more information before starting a new session
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
if(!isset($_POST['EncryptedInfo'])) { end_script('CheckAuthentification : EncryptedInfo not received.'); }		// Verify encrypted data presence


$connection_granted = 0;	// Initially the connection is not granted


//----------------------------------: DECRYPT AES KEYS (if connection not established yet) :----------------------------------------------
// If the session is not established yet, we have to generate session ID and decrypt AES keys, then check if information are correct
$notConnectYet = isset($_POST["NotConnectedYet"]) && $_POST["NotConnectedYet"];
if($notConnectYet)
{
	// Verify encrypted data presence
	if(!isset($_POST['AESKeys'])) { end_script('CheckAuthentification : AESKeys not received.'); }
	$AESKeys = $_POST['AESKeys'];
	
	// Read private certificate
	include_once './Includes/Crypt/RSA.php';
	$privateCertificatePath = "./Includes/PrivateCertificate.crt";
	$myfile = fopen($privateCertificatePath, "r") or die("Unable to open private certificate file.");
	$privateKey = fread($myfile,filesize($privateCertificatePath));
	fclose($myfile);											// We won't use the certificate file anymore, close it nicely
	
	// RSA decrypt
	$rsa = new Crypt_RSA();										// Create RSA object
	$rsa->setEncryptionMode(CRYPT_RSA_ENCRYPTION_OAEP);			// RSA settings
	$rsa->loadKey($privateKey);									// Load the private key
	$AESData = $rsa->decrypt(base64_decode($AESKeys));			// Decrypt AES keys received
	
	// Save AES keys in session array
	$DecryptedAESKeys = explode(SEPARATOR, $AESData);			// Get information separately
	$_SESSION['aes_key'] = base64_decode($DecryptedAESKeys[0]);	// Decode keys from base64 string
	$_SESSION['aes_iv'] = base64_decode($DecryptedAESKeys[1]);	// Decode keys from base64 string
}



//----------------------------------: CHECK AES KEYS :----------------------------------------------
if(!isset($_SESSION['aes_key']) || !isset($_SESSION['aes_iv']) || $_SESSION['aes_key']=="" || $_SESSION['aes_iv']=="")
{ end_script('CheckAuthentification : AES keys not in session array.'); }



//----------------------------------: READ DATAS :----------------------------------------------
$decryptedData = read($_POST["EncryptedInfo"]);		// Read encrypted data (decrypted with AES keys of the session)
$datas = explode(SEPARATOR, $decryptedData);



//----------------------------------: GRANT/REFUSE CONNECTION :----------------------------------------------
if($notConnectYet)
{
	if($ACTION=='Register' || $ACTION=='Resend'  || $ACTION=='Forgot')
	{
		// Connection granted for registration and resend
		// ONLY FOR THIS CASES !
		// It's the only cases where not connection is made, and we don't check connection here
		$_SESSION['session_token'] = $datas[count($datas)-1];
		$connection_granted = 1;
	}
	else if($ACTION=='Login' || $ACTION=='AdminLogin')
	{
		if(count($datas)!=3)	// If there is not exactly 3 members -> leave
		{
			end_script('CheckAuthentification : encrypted data malformed.');
		}
		// Get user information
		$username = $datas[0];
		$password = $datas[1];
		$session_token = $datas[2];
		$IP = $_SERVER['REMOTE_ADDR'];
		
		// Check login information
		checkAuthentification($username, $password, $IP, $session_token);
		
		// Save session token ONLY once login information are correct
		$_SESSION['session_token'] = $session_token;
		
		// Update activity date
		updateConnectionDate($username);
		updateActivity($username);
		
		// Connection granted
		$connection_granted = 1;
	}
}
else
{
	if(count($datas)<=0)	// If there is nothing -> leave
	{ end_script('CheckAuthentification: encrypted data malformed.'); }
	
	// Read the token in last position
	$tokenReceived = $datas[count($datas)-1];
	
	// Check if the token is set in session array and if the received session token is correct
	if(!isset($_SESSION['session_token']) || $tokenReceived=="" || $tokenReceived != $_SESSION['session_token'])
	{ end_script('CheckAuthentification: session_token incorrect.'); }
	
	if(!checkSessionToken($_SESSION['username'], $tokenReceived))
	{ end_script('CheckAuthentification: you have been deconnected.'); }
	
	if(!isset($_SESSION['connected']) || !$_SESSION['connected'])
	{ end_script('CheckAuthentification: you are not connected, please login.'); }
	
	// Add useful information variable (not case sensitive)
	define("USERNAME", $_SESSION['username'], TRUE);
	define("USER_ID", $_SESSION['user_id'], TRUE);
	define("USER_IP", $_SESSION['user_ip'], TRUE);
	
	// Update activity date
	updateActivity($_SESSION['username']);
	
	// Connection granted
	$connection_granted = 1;
}



//----------------------------------: STOP SCRIPT IF NOT GRANTED :----------------------------------------------
if($connection_granted != 1) { end_script('CheckAuthentification : authentification refused.'); }
//----------------------------------: STOP SCRIPT IF NOT GRANTED :----------------------------------------------

?>