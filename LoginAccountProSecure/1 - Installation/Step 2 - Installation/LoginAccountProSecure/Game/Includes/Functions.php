<?php

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : All the utility functions used in all PHP scripts
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

include_once 'Includes/mailFunc.php';

define("SEPARATOR", "<DATA_SEPARATOR>", TRUE);
define("DELIMITOR", "<ENCRYPTED_DATA_DELIMITOR>", TRUE);



//																						UNIVERSAL
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Transform the specified data to ensure security
function protectAgainstInjection($sql, $formUse = true)
{
	// Replace key word by nothing
	$sql = preg_replace("/(from|select|insert|delete|where|drop table|show tables|--|\\\\)/i","",$sql);
	//$sql = preg_replace("/(from|select|insert|delete|where|drop table|show tables|--)/i","",$sql);
	$sql = trim($sql);
	//$sql = strip_tags($sql);
	if(!$formUse || !get_magic_quotes_gpc()) { $sql = addslashes($sql); }
	return $sql;
}

// This function is very important -> USE IT TO DISPLAY ERROR MESSAGES (not encrypted)
function end_script($message)
{
	echo "ERROR: ".$message;
	// Close connection to database properly
	if(isset($_SESSION['databaseConnection'])) { mysqli_close($_SESSION['databaseConnection']); }
	// Ensure the end of the current script
	die();
	exit(0);
}

// Handle messages between pages and transfer POST information
function end_script_redirect_post($redirection_page, $message)
{
	// Same as the "end_script" function but post data are transfer to the redirection page
	// This mechanism  allow us to perform another action via the redirection system (with all the post data needed)
	// Save the redirection page name and the message (if any) in order to get it once the "message.php" page is reached
	$_SESSION['REDIRECTION'] = $redirection_page;
	$_SESSION['MESSAGE'] = $message.'<br />';
	$_SESSION['POSTDATA'] = $_POST;
	// Close the database correctly
	mysqli_close($_SESSION['databaseConnection']);
	// Redirect to the message page
	header('Location:Message.php');
	// Ensure the end of the current script
	die();
	exit(0);
}

// Random string generation
function generateRandomString($length = 30)
{
    $characters = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    $charactersLength = strlen($characters);
    $randomString = '';
	// We can leave this function only if randomString is [$length] characters long AND the string return by protectAgainstInjection is [$length] characters long too
	while(strlen($randomString)<$length || strlen(protectAgainstInjection($randomString))<$length)
	{
		$randomString = '';
		for ($i=0; $i<$length; $i++)
		{
			$randomString .= $characters[rand(0, $charactersLength - 1)];
		}
	}
    return $randomString;
}




//																						AUTHENTIFICATION & REGISTRATION
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Authentification granted if the username exists, the password is correct AND the IP is not blocked for Connection attempts
function checkAuthentification($username, $password, $IP, $session_token)
{
	$id = getUsernameID($username);														// Check account exists
	if($id == "") { end_script('This username is not linked to any account.'); }		// Authentification denied -> Redirect to authentification page
	
	if(SCAN_IP_ACTIVATED)																// If the IP scan is activated (in the installation process)
	{
		checkAttempts($id, $IP);														// Check if the maximum connection attempts is not reached
		checkIPIsValidated($id, $IP);													// Check if IP is validated for this account
		increaseAttempts($id, $IP, 'Connection');										// Increase connection attempts counter (fir this specified IP only and this action only)
	}
	
	$passwordIsValid = passwordVerification($username, $password);						// Check password
	if(!$passwordIsValid) { end_script('Incorrect password.'); }						// Authentification denied -> Redirect to authentification page
	
	if(isBanned($id)) { end_script('You have been banned by an administrator.'); }		// Check if account is banned or not
	
	removeAttempts($id, $IP, 'Connection');												// Authentification granted : remove any unsuccessful previous attempts (for this IP and this account only)
	
	$accountIsActivated = checkAccountIsValidated($id);									// Check account activation
	if(!$accountIsActivated)
	{
		// If the account is not validated yet : send the account activation email
		sendAccountActivationEmail($id);
		end_script('Your account is not activated yet, please follow the link we sent you on your email address to activate it.');
	}
	
	// Success: let's save the session token in the account (to make sure only this user is connected with this account)
	$keys = array('username' => "'".$username."'");				// Where condition
	$infos = array();											// Information to update
	$infos['session_token'] = "'".$session_token."'";
	update_table($_SESSION['AccountTable'], $keys, $infos);		// Update session token for this user
}
function checkSessionToken($username, $session_token)
{
	$SQLrequest = "SELECT session_token FROM ".$_SESSION['AccountTable']." WHERE username = '".$username."'";
	$SQLresult = mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die("CHECK SESSION TOKEN: ".$SQLrequest);
	if(mysqli_num_rows($SQLresult))
	{
		$datas = mysqli_fetch_array($SQLresult);
		return ($datas['session_token'] == $session_token);
	}
	return false;
}

// Check if information received are correct, check if mail and username are not already in use, then register the new user (+ create connection attempt : optional) finally : send the activation email
function register($mail, $username, $password, $IP)
{
	//---------------------------------: Are information valid ? :---------------------------------------
	if(!filter_var($mail, FILTER_VALIDATE_EMAIL)) { end_script('Registration failed, your email address is not valid.'); }
	if(strlen($username)<3) { end_script('Registration failed, your username is not valid.'); }
	if(strlen($password)<3) { end_script('Registration failed, your password is not valid.'); }
	
	//---------------------------------: Is account available ? :----------------------------------------
	if(checkMailExists($mail)) { end_script('Registration failed, this mail address is already in use.'); }
	if(checkUsernameExists($username)) { end_script('Registration failed, this username already exists.'); }
	
	// Add salt to the password and hash it
	$salt = generateRandomString(50);
	$password = hashPassword($password, $salt);
	
	//--------------------------------: Perform registration :-------------------------------------------
	// Generate validation_code
	$validation_code = generateRandomString(30);
	// Create a new account NOT activated
	$requestSQL = "INSERT INTO ".$_SESSION['AccountTable']." (id,mail,username,password,salt,validation_code,validated,creation_date,last_activity,last_connection_date) VALUES (NULL,'$mail','$username','$password','$salt','$validation_code',0,NOW(),NOW(),NOW())";
	mysqli_query($_SESSION['databaseConnection'], $requestSQL) or die("PERFORMING REGISTRATION ERROR! The request you are trying to execute : ".$requestSQL);
	
	//---------------------: Activate IP and send account activation email :-----------------------------
	$id = getUsernameID($username);			// Get the account ID
	createIPValidation($id, $IP, 1);		// Validate IP from where the registration has been completed
	sendAccountActivationEmail($id);		// Send account activation email
	return true;
}

function modify($mail, $username, $password)
{
	// The WHERE SQL condition
	$keys = array('id' => $_SESSION['user_id']);
	
	// Information to update
	$infos = array();
	if($mail!="")
	{
		if(checkMailExists($mail)) { end_script('This email address is already in use.'); } // Check if email address doesn't already exist
		$infos['mail'] = "'".$mail."'";
	}
	if($username!="")
	{
		if(checkUsernameExists($username)) { end_script('This username is already in use.'); } // Check if username doesn't already exist
		$infos['username'] = "'".$username."'";
	}
	if($password!="")
	{
		$salt = generateRandomString(50);
		$passwordHash = hashPassword($password,$salt);
		
		$infos['password'] = "'".$passwordHash."'";
		$infos['salt'] = "'".$salt."'";
	}
	return update_table($_SESSION['AccountTable'], $keys, $infos);
}

// Does the account with this username has administrator rights ?
function isAdministrator($username)
{
	$account = getAccount($username);
	if(count($account)==0) { end_script('Your account does not exists, please contact an administrator.'); }
	return $account['admin']==1;
}




//																						ACCOUNT
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Get the user account
function getAccount($username)
{
	$SQL_userAccount = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE username COLLATE utf8_bin = '".$username."'";
	$Result_SQL_userAccount = mysqli_query($_SESSION['databaseConnection'], $SQL_userAccount) or die("GET USERNAME ID FUNCTION DATABASE ERROR! The request you are trying to execute : ".$SQL_userAccount);
	$userAccountFound = mysqli_num_rows($Result_SQL_userAccount);
	if($userAccountFound)
	{
		return mysqli_fetch_array($Result_SQL_userAccount);
	}
	return null;
}
// Get the user account
function getAccountById($id)
{
	$SQL_userAccount = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE id = '".$id."'";
	$Result_SQL_userAccount = mysqli_query($_SESSION['databaseConnection'], $SQL_userAccount) or die("GET ACCOUNT FUNCTION DATABASE ERROR! The request you are trying to execute : ".$SQL_userAccount);
	$userAccountFound = mysqli_num_rows($Result_SQL_userAccount);
	if($userAccountFound)
	{
		return mysqli_fetch_array($Result_SQL_userAccount);
	}
	return null;
}

// Set the "validated" field of the account table to 1 (+ remove Validation attempts)
function activateAccount($id, $IP)
{
	$SQLupdate = "UPDATE ".$_SESSION['AccountTable']." SET validated = 1 WHERE id = '".$id."'";
	$result_update = mysqli_query($_SESSION['databaseConnection'], $SQLupdate) or die("DATABASE ERROR ACCOUNT ACTIVATION IMPOSSIBLE. The request you are trying to execute : ".$SQLupdate);
	removeAttempts($id, $IP, 'Validation');
	removeAttempts($id, $IP, 'Resend');
	return true;
}

// Set the "validated" field of the account table to 1 (+ remove Validation attempts)
function checkAccountIsValidated($id)
{
	$datas = getAccountById($id);
	if(count($datas)==0) { end_script('Your account does not exists, please contact an administrator.'); }
	return $datas['validated']==1;
}

// Check if the account is banned or not
function isBanned($id)
{
	$datas = getAccountById($id);
	if(count($datas)==0) { end_script('Your account does not exists, please contact an administrator.'); }
	return $datas['banned']==1;
}

// Generate a new validation code for this account (useful for Account activation)
function generateNewAccountValidationCode($id)
{
	$validation_code = generateRandomString(30);
	$keys = array( 'id' => "'".$id."'" );
	$infos = array( 'validation_code' => "'".$validation_code."'" );
	update_table($_SESSION['AccountTable'], $keys, $infos);
	return $validation_code;
}

// Generate a new validation code for this IP (useful for IP activation)
function generateNewIPValidationCode($id, $IP)
{
	$validation_code = generateRandomString(30);
	$keys = array( 'account_id' => "'".$id."'", 'ip' => "'".$IP."'" );
	$infos = array( 'validation_code' => "'".$validation_code."'" );
	update_table($_SESSION['IPTable'], $keys, $infos);
	return $validation_code;
}
function updateConnectionDate($username)
{
	$SQLrequest = "UPDATE ".$_SESSION['AccountTable']." SET last_connection_date = NOW() WHERE username = '".$username."'";
	$SQLresult = mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die("UPDATE CONNECTION DATE ERROR: ".$SQLrequest);
}
function updateActivity($username)
{
	$SQLrequest = "UPDATE ".$_SESSION['AccountTable']." SET last_activity = NOW() WHERE username = '".$username."'";
	$SQLresult = mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die("UPDATE ACTIVITY ERROR: ".$SQLrequest);
}
function timeSinceLastActivity($username)
{
	$SQLrequest = "SELECT last_activity, NOW() as now FROM ".$_SESSION['AccountTable']." WHERE username = '".$username."'";
	$SQLresult = mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die("LAST ACTIVITY ERROR: ".$SQLrequest);
	if(mysqli_num_rows($SQLresult))
	{
		$data = mysqli_fetch_array($SQLresult);
		return strtotime($data['now']) - strtotime($data['last_activity']);
	}
	end_script("isConnected: last_activity not found.");
}
function isConnected($username)
{
	return timeSinceLastActivity($username) < TIMEOUT;
}



//																						PASSWORD
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Replace current VALIDATION_CODE (not the password !!) with a new one
// We don't replace the password because anybody with only the mail address can launch the action "Reinitialize password"
// The current password must be UNCHANGED (because the "real" owner of the account don't want his password changed all the time by usurpers)
// Then the validation code is sent via email to the specified email address
// If the link (with the validation code) is clicked in the email -> the action "sendPassword" is launch (which actually generate a new password and send it via email)
function reinitPassword($mail, $IP)
{
	//---------------------------------: Are information valid ? :---------------------------------------
	if(!filter_var($mail, FILTER_VALIDATE_EMAIL)) { end_script('Password reinitialization failed, your email address is not valid.'); }
	
	//-----------------------------------: Is account valid ? :------------------------------------------
	if(!checkMailExists($mail)) { end_script('Password reinitialization failed, this mail address is not linked to any account.'); }
	
	//--------------------------------------: Is IP valid ? :--------------------------------------------
	$id = getIdFromMail($mail);
	checkForgotAttempts($id, $IP);
	increaseAttempts($id, $IP, 'Forgot');
	
	//----------------------------------------: Update :-------------------------------------------------
	$generatedValidationCode = generateRandomString(30);
	$SQLupdate = "UPDATE ".$_SESSION['AccountTable']." SET validation_code = '".$generatedValidationCode."' WHERE mail = '".$mail."'";
	$result_update = mysqli_query($_SESSION['databaseConnection'], $SQLupdate) or die("REINIT PASSWORD DATABASE ERROR. The request you are trying to execute : ".$SQLupdate);
	
	//----------------------------: Send email to confirm email address :--------------------------------
	$usersubject = "Your information to your ".$_SESSION['GameName']." account";
	$userheaders = "From: ".$_SESSION['SERVER_email']."\n";
	$usermessage = "If you want to change your password by a new generated one :\nPlease click the link below to change your password of your ".$_SESSION['GameName']." account \n\n  http://".$_SESSION['Domain']."/".$_SESSION['SecureLoginFolder']."/ReceiveNewPassword.php?mail=".$mail."&code=".$generatedValidationCode;
	//using smtp instead php mail function ----- fingerx/2016/9/27
	//mail($mail,$usersubject,$usermessage,$userheaders);
	smtp_mail($mail,$usersubject,$usermessage,$userheaders);
	return true;
}

// Generate a new password and send it via email to the user
function sendPassword($mail, $code, $IP)
{
	//---------------------------------: Are information valid ? :---------------------------------------
	if(!filter_var($mail, FILTER_VALIDATE_EMAIL)) { end_script('Password reinitialization failed, your email address is not valid.'); }
	if(strlen($code) < 10) { end_script('Password reinitialization failed, your code is not valid.'); }
	
	//-----------------------------------: Is account valid ? :------------------------------------------
	if(!checkMailExists($mail)) { end_script('Password reinitialization failed, this mail address is not linked to any account.'); }
	
	//--------------------------------------: Is IP valid ? :--------------------------------------------
	$id = getIdFromMail($mail);
	checkForgotAttempts($id, $IP);
	
	$generatedPassword = generateRandomString(30);
	$salt = generateRandomString(50);
	$generatedPasswordHash = hashPassword(hash('sha256', $generatedPassword), $salt);
	
	// Get code
	$SQL_userAccount = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE mail = '".$mail."'";
	$Result_SQL_userAccount = mysqli_query($_SESSION['databaseConnection'], $SQL_userAccount) or die("GET USERNAME ID FUNCTION DATABASE ERROR! The request you are trying to execute : ".$SQL_userAccount);
	$userAccountFound = mysqli_num_rows($Result_SQL_userAccount);
	if(!$userAccountFound){ end_script('This email address is not linked to any account.'); }
	$data = mysqli_fetch_array($Result_SQL_userAccount);
	if($data['validation_code'] != $code) { end_script('Password reinitialization failed, your code is incorrect.'); }
	
	// Notice here : we have to hash the generated password with sha256 to match future sessions passwords received
	$SQLupdate = "UPDATE ".$_SESSION['AccountTable']." SET password = '".$generatedPasswordHash."', salt = '".$salt."' WHERE id = ".$id;
	$result_update = mysqli_query($_SESSION['databaseConnection'], $SQLupdate) or die("SEND PASSWORD DATABASE ERROR. The request you are trying to execute : ".$SQLupdate);
	
	// Send email with generated password
	$usersubject = "Your information to your ".$_SESSION['GameName']." account";
	$userheaders = "From: ".$_SESSION['SERVER_email']."\n";
	$usermessage = "Your information to your ".$_SESSION['GameName']." account :\n - Username : ".$data['username']."\n - Password : ".$generatedPassword."\n\nYou can connect to your account with these information.\nCaution, even if nobody can connect to your account from an IP you didn't validate, keep your information in safe place.";
	//using smtp instead php mail function ----- fingerx/2016/9/27
	//mail($mail,$usersubject,$usermessage,$userheaders);
	smtp_mail($mail,$usersubject,$usermessage,$userheaders);
	removeAttempts($id, $IP, 'Forgot');
	return true;
}

// Verify if password is valid for the specified username
function passwordVerification($CLIENT_username, $CLIENT_password)
{
	// Get the user account
	$SQL_userAccount = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE username COLLATE utf8_bin = '".$CLIENT_username."'";
	$Result_SQL_userAccount = mysqli_query($_SESSION['databaseConnection'], $SQL_userAccount) or die("PASSWORD VERIFICATION DATABASE ERROR! The request you are trying to execute : ".$SQL_userAccount);
	$userAccountFound = mysqli_num_rows($Result_SQL_userAccount);
	if($userAccountFound)
	{
		$datas = mysqli_fetch_array($Result_SQL_userAccount);
		
		$passwordReceived = hashPassword($CLIENT_password, $datas["salt"]);
		return $passwordReceived == $datas["password"];
	}
	return false;
}





//																						IP
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create IP validation (already activated or not?)
function createIPValidation($id, $IP, $activated = 0)
{
	$validation_code = generateRandomString(30);
	$requestSQL = "INSERT INTO ".$_SESSION['IPTable']." (account_id,ip,validation_code,validated,creation_date) VALUES ('".$id."','".$IP."','".$validation_code."',".$activated.",NOW())";
	mysqli_query($_SESSION['databaseConnection'], $requestSQL) or die("CREATE IP VALIDATION ERROR! The request you are trying to execute : ".$requestSQL);
}

// Get the IP information
function getIPInformation($id, $IP)
{
	$SQL_userIP = "SELECT * FROM ".$_SESSION['IPTable']." WHERE account_id = '".$id."' AND ip = '".$IP."'";
	$Result_SQL_userIP = mysqli_query($_SESSION['databaseConnection'], $SQL_userIP) or die("GET IP INFOS FUNCTION DATABASE ERROR! The request you are trying to execute : ".$SQL_userIP);
	$userIPFound = mysqli_num_rows($Result_SQL_userIP);
	if($userIPFound)
	{
		return mysqli_fetch_array($Result_SQL_userIP);
	}
	return null;
}

// Check if the specified IP is validated for this account (if not redirect to the IP validation page)
function checkIPIsValidated($id, $IP)
{
	$SQL_userIP = "SELECT * FROM ".$_SESSION['IPTable']." WHERE account_id = '".$id."' AND ip = '".$IP."'";
	$Result_SQL_userIP = mysqli_query($_SESSION['databaseConnection'], $SQL_userIP) or die("CHECK VALIDATED IP DATABASE ERROR! The request you are trying to execute : ".$SQL_userIP);
	$userIPFound = mysqli_num_rows($Result_SQL_userIP);
	if($userIPFound)
	{
		$datas = mysqli_fetch_array($Result_SQL_userIP);
		// This IP is validated for this account -> connection with this IP granted
		if($datas['validated']==1) { return true; }
	}
	else
	{
		// No IP connection found for this account : create one (NOT validated yet)
		createIPValidation($id, $IP, 0);
	}
	// We are here if :
	// 		- IP connection was found but not activated yet
	// 		- IP connection was NOT found (but we just created one above)
	
	// In the 2 cases : Redirect to the IP validation page
	// Send account activation email
	sendIPActivationEmail($id);
	end_script('Your IP is not activated for this account, please enter your IP password or follow the link we sent you on your email address.');
	return false;
}

// Set the "validated" field of the account table to 1 (+ remove Validation attempts)
function activateIP($id, $IP)
{
	$SQLupdate = "UPDATE ".$_SESSION['IPTable']." SET validated = 1 WHERE account_id = '".$id."' AND ip = '".$IP."'";
	$result_update = mysqli_query($_SESSION['databaseConnection'], $SQLupdate) or die("DATABASE ERROR IP ACTIVATION IMPOSSIBLE. The request you are trying to execute : ".$SQLupdate);
	removeAttempts($id, $IP, 'IP Validation');
	removeAttempts($id, $IP, 'Resend IP');
	return true;
}


//																						ATTEMPTS
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create an attempt for a specified action (for only one pair of (id,IP))
function createAttempt($id, $IP, $action)
{
	$SQLinsert = "INSERT INTO ".$_SESSION['AttemptsTable']." (account_id,ip,action,attempts) VALUES ('".$id."','".$IP."','".$action."',1)";
	$result_insert = mysqli_query($_SESSION['databaseConnection'], $SQLinsert) or die("CREATE ATTEMPT ERROR! The request you are trying to execute : ".$SQLinsert);
}

// Get an attempt for a specified action (for only one pair of (id,IP))
function getAttempts($id, $IP, $action)
{
	$SQL_userIPAttempts = "SELECT * FROM ".$_SESSION['AttemptsTable']." WHERE account_id = '".$id."' AND ip = '".$IP."' AND action = '".$action."'";
	$Result_SQL_userIPAttempts = mysqli_query($_SESSION['databaseConnection'], $SQL_userIPAttempts) or die("GET ATTEMPTS DATABASE ERROR! The request you are trying to execute : ".$SQL_userIPAttempts);
	$userIPAttemptsFound = mysqli_num_rows($Result_SQL_userIPAttempts);
	if($userIPAttemptsFound)
	{
		$datas = mysqli_fetch_array($Result_SQL_userIPAttempts);
		return $datas['attempts'];
	}
	return 0;
}

// Set an attempt for a specified action (for only one pair of (id,IP))
function setAttempts($id, $IP, $action, $attempts)
{
	$SQLupdate = "UPDATE ".$_SESSION['AttemptsTable']." SET attempts = '".$attempts."' WHERE account_id = '".$id."' AND ip = '".$IP."' AND action = '".$action."'";
	$result_update = mysqli_query($_SESSION['databaseConnection'], $SQLupdate) or die("DATABASE ERROR ATTEMPTS MODIFICATION IMPOSSIBLE. The request you are trying to execute : ".$SQLupdate);
}

// Remove all attempts for a specified action (for only one pair of (id,IP))
function removeAttempts($id, $IP, $action)
{
	$SQLdelete = "DELETE FROM ".$_SESSION['AttemptsTable']." WHERE account_id = '".$id."' AND ip = '".$IP."' AND action = '".$action."'";
	$result_delete = mysqli_query($_SESSION['databaseConnection'], $SQLdelete) or die("DATABASE ERROR ATTEMPTS SUPPRESSION IMPOSSIBLE. The request you are trying to execute : ".$SQLdelete);
}

// Increment an attempt for a specified action (for only one pair of (id,IP)) -> if no attempt exists, create one
function increaseAttempts($id, $IP, $action)
{
	$attempts = getAttempts($id, $IP, $action) + 1;
	if($attempts == 1) { createAttempt($id, $IP, $action); }
	else { setAttempts($id, $IP, $action, $attempts); }
}

// Check if the IP hasn't reach the maximum connection attempts yet (for the specified account)
function checkAttempts($id, $IP)
{
	$attempts = getAttempts($id, $IP, 'Connection');
	if($attempts >= $_SESSION['AvailableAttemptsBeforeBlocking']) { end_script('Your IP is blocked for this account, please contact an administrator.'); }
}

// Check if the IP hasn't reach the maximum "Resend" attempts yet (for the specified account)
function checkResendAttempts($id, $IP)
{
	$attempts = getAttempts($id, $IP, 'Resend');
	if($attempts >= $_SESSION['AvailableAttemptsBeforeBlocking']) { end_script('You cannot send account validation email anymore, please contact an administrator.'); }
}

// Check if the IP hasn't reach the maximum "Resend" attempts yet (for the specified account)
function checkResendIPAttempts($id, $IP)
{
	$attempts = getAttempts($id, $IP, 'Resend IP');
	if($attempts >= $_SESSION['AvailableAttemptsBeforeBlocking']) { end_script('You cannot send IP validation email anymore, please contact an administrator.'); }
}

// Check if the IP hasn't reach the maximum "Validation" attempts yet (for the specified account)
function checkAccountValidationAttempts($id, $IP)
{
	$attempts = getAttempts($id, $IP, 'Validation');
	if($attempts >= $_SESSION['AvailableAttemptsBeforeBlocking']) { end_script('You cannot activate your account from your IP address anymore, please contact an administrator.'); }
}

// Check if the IP hasn't reach the maximum "Validation" attempts yet (for the specified account)
function checkIPValidationAttempts($id, $IP)
{
	$attempts = getAttempts($id, $IP, 'IP Validation');
	if($attempts >= $_SESSION['AvailableAttemptsBeforeBlocking']) { end_script('You cannot activate your IP for this account anymore, please contact an administrator.'); }
}

// Check if the IP hasn't reach the maximum "Forgot" attempts yet (for the specified account)
function checkForgotAttempts($id, $IP)
{
	$attempts = getAttempts($id, $IP, 'Forgot');
	if($attempts >= $_SESSION['AvailableAttemptsBeforeBlocking']) { end_script('You cannot get your information back from this IP anymore, please contact an administrator.'); }
}





//																						EMAILS
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Send (or resend) the account activation email for an account
function sendAccountActivationEmail($id)
{
	//-------------------------------: Send email to confirm email address :------------------------------
	$SQL_userAccount = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE id = '".$id."'";
	$Result_SQL_userAccount = mysqli_query($_SESSION['databaseConnection'], $SQL_userAccount) or die("SEND ACTIVATION EMAIL : GET ACCOUNT DATABASE ERROR! The request you are trying to execute : ".$SQL_userAccount);
	$userAccountFound = mysqli_num_rows($Result_SQL_userAccount);
	if(!$userAccountFound) { end_script('Email activation not sent, the ID is not linked to any account.'); }
	$datas = mysqli_fetch_array($Result_SQL_userAccount);
	
	//-------------------------------------: Check resend attempts :--------------------------------------
	$IP = $_SERVER['REMOTE_ADDR'];
	checkResendAttempts($id, $IP);
	increaseAttempts($id, $IP, 'Resend');
	
	//----------------------------------------: Get needed data :-----------------------------------------
	$mail = $datas['mail'];
	$username = $datas['username'];
	
	//--------------------------------: Generated a new validation code :---------------------------------
	$validation_code = generateNewAccountValidationCode($id);
	
	//----------------------------------: Send the email validation :-------------------------------------
	$usermessage = "Please click the link below to confirm you email address in order to activate your ".$_SESSION['GameName']." account \n http://".$_SESSION['Domain']."/".$_SESSION['SecureLoginFolder']."/ValidateAccountByURL.php?username=".$username."&code=".$validation_code;
	$usersubject = "Confirm your email address (".$mail.") to activate your ".$_SESSION['GameName']." account (".$username.")";
	$userheaders = "From: ".$_SESSION['SERVER_email']."\n";
	//using smtp instead php mail function ---fingerx/2016/9/27
	//mail($mail,$usersubject,$usermessage,$userheaders);
	smtp_mail($mail,$usersubject,$usermessage,$userheaders);
	return true;
}

// Send (or resend) the IP activation email
function sendIPActivationEmail($id)
{
	//-------------------------------: Send email to confirm email address :------------------------------
	$SQL_userAccount = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE id = '".$id."'";
	$Result_SQL_userAccount = mysqli_query($_SESSION['databaseConnection'], $SQL_userAccount) or die("SEND ACTIVATION EMAIL : GET ACCOUNT DATABASE ERROR! The request you are trying to execute : ".$SQL_userAccount);
	$userAccountFound = mysqli_num_rows($Result_SQL_userAccount);
	if(!$userAccountFound) { end_script('Email activation not sent, the ID is not linked to any account.'); }
	$datas = mysqli_fetch_array($Result_SQL_userAccount);
	
	//-------------------------------------: Check resend attempts :--------------------------------------
	$IP = $_SERVER['REMOTE_ADDR'];
	checkResendIPAttempts($id, $IP);
	increaseAttempts($id, $IP, 'Resend IP');
	
	//----------------------------------------: Get needed data :-----------------------------------------
	$mail = $datas['mail'];
	$username = $datas['username'];
	
	//--------------------------------: Generated a new validation code :---------------------------------
	$validation_code = generateNewIPValidationCode($id, $IP);
	
	//----------------------------------: Send the email validation :-------------------------------------
	$usermessage = "Please click the link below to confirm you email address in order to activate your ".$_SESSION['GameName']." IP for your account \n http://".$_SESSION['Domain']."/".$_SESSION['SecureLoginFolder']."/ValidateIPByURL.php?username=".$username."&code=".$validation_code;
	$usersubject = "Confirm your IP via your email address (".$mail.") to activate your ".$_SESSION['GameName']." account (".$username.") from this IP";
	$userheaders = "From: ".$_SESSION['SERVER_email']."\n";
	//using smtp instead php mail function ---fingerx/2016/9/27
	//mail($mail,$usersubject,$usermessage,$userheaders);
	smtp_mail($mail,$usersubject,$usermessage,$userheaders);
	return true;
}




//																						EXISTS ?
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Check if the email address is already in use
function checkMailExists($mail)
{
	$SQL_userAccount = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE mail = '".$mail."'";
	$Result_SQL_userAccount = mysqli_query($_SESSION['databaseConnection'], $SQL_userAccount) or die($_SESSION['SERVER_host'].", ".$_SESSION['SERVER_user'].", ".$_SESSION['SERVER_password'].", ".$_SESSION['DB_name'].", CHECK MAIL EXISTS FUNCTION DATABASE ERROR! The request you are trying to execute : ".$SQL_userAccount);
	return mysqli_num_rows($Result_SQL_userAccount);
}

// Check if the username is already in use
function checkUsernameExists($username)
{
	$SQL_userAccount = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE username COLLATE utf8_bin = '".$username."'";
	$Result_SQL_userAccount = mysqli_query($_SESSION['databaseConnection'], $SQL_userAccount) or die("CHECK USERNAME EXISTS FUNCTION DATABASE ERROR! The request you are trying to execute : ".$SQL_userAccount);
	return mysqli_num_rows($Result_SQL_userAccount);
}

// Check if the savegame already exists for this user
function checkSavegameExists($account_id, $name)
{
	$SQLrequest = "SELECT name FROM ".$_SESSION['SaveGame']." WHERE account_id = '".$account_id."' AND name = '".$name."'";
	$SQLresults = mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die("CHECK SAVEGAME: ".$SQLrequest);
	return mysqli_num_rows($SQLresults);
}





//																						UNIVERSAL DATABASE
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Useful to update a table with the table name, the keys array, the infos array
// Example : update_table("Account", $keys, $infos);
// CAUTION with string you have to use : "'".'myString'."'" <- (notice "'" is add to specify to SQL it's a string)
// With :
// - $keys = array( 'mail' => "'".'mail@mail.com'."'", 'username' => 'myName', 'something' => 'something', etc...);
// - $infos = array( 'username' => 'myNewName', 'password' => '65sd4s651zec98sd5fc4q6s5c', 'something' => 'something', etc...);
// The keys array will be used for the "WHERE" conditions
// The infos array will be used for the "SET" clauses
function update_table($table, $keys, $infos)
{
	$end_infos = count($infos);
	$end_keys = count($keys);
	if($end_infos==0 || $end_keys==0) { return false; }
	
	$request = "UPDATE ".$table." SET ";
	// Values to update
	$i=0;
	foreach($infos as $key => $value)
	{
		if($i == $end_infos-1) { $request .= $key." = ".$value." "; }
		else { $request .= $key." = ".$value.", "; }
		$i++;
	}
	
	$request .= "WHERE ";
	
	// Keys
	$i=0;
	foreach($keys as $key => $value)
	{
		if($i == $end_keys-1) { $request .= $key." = ".$value;}
		else { $request .= $key." = ".$value." AND "; }
		$i++;
	}
	
	// Update values
	$result_update = mysqli_query($_SESSION['databaseConnection'], $request) or die("DATABASE ERROR UPDATE IMPOSSIBLE: ".$request);
	return true;
}





//																						GET UNIQUE DATA FROM ANOTHER
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Get the account ID
function getUsernameID($username)
{
	// Get the user account
	$SQL_userAccount = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE username COLLATE utf8_bin = '".$username."'";
	$Result_SQL_userAccount = mysqli_query($_SESSION['databaseConnection'], $SQL_userAccount) or die("GET USERNAME ID FUNCTION DATABASE ERROR! The request you are trying to execute : ".$SQL_userAccount);
	$userAccountFound = mysqli_num_rows($Result_SQL_userAccount);
	if($userAccountFound)
	{
		$datas = mysqli_fetch_array($Result_SQL_userAccount);
		return $datas['id'];
	}
	return "";
}

// Get the username from the email address
function getUsernameFromMail($mail)
{
	// Get the user account
	$SQL_userAccount = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE mail = '".$mail."'";
	$Result_SQL_userAccount = mysqli_query($_SESSION['databaseConnection'], $SQL_userAccount) or die("GET USERNAME ID FUNCTION DATABASE ERROR! The request you are trying to execute : ".$SQL_userAccount);
	$userAccountFound = mysqli_num_rows($Result_SQL_userAccount);
	if($userAccountFound)
	{
		$datas = mysqli_fetch_array($Result_SQL_userAccount);
		return $datas['username'];
	}
	return "";
}

// Get the account ID from the email address
function getIdFromMail($mail)
{
	// Get the user account
	$SQL_userAccount = "SELECT * FROM ".$_SESSION['AccountTable']." WHERE mail = '".$mail."'";
	$Result_SQL_userAccount = mysqli_query($_SESSION['databaseConnection'], $SQL_userAccount) or die("GET USERNAME ID FUNCTION DATABASE ERROR! The request you are trying to execute : ".$SQL_userAccount);
	$userAccountFound = mysqli_num_rows($Result_SQL_userAccount);
	if($userAccountFound)
	{
		$datas = mysqli_fetch_array($Result_SQL_userAccount);
		return $datas['id'];
	}
	return "";
}




//																						AES DECRYPTION
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Useful to get data from POST (since everything is AES encrypted we can use it every time we access POST array
function read($data)
{
	return aes_decrypt(base64_decode($data));
}
// Here to prevent SQL injection, we don't replace characters, we encode the string in Base64
// Use it for the data with special characters and "forbidden" strings (like select, insert, all special characters and everything)
function read_SpecialData($data)
{
	return aes_decrypt(base64_decode($data), true);
}

// AES encryption
function aes_encrypt($string)
{
	$keyString = $_SESSION['aes_key'];
	$ivString = $_SESSION['aes_iv'];
	
	$toBeEncrypted = $string.'<ENC_END>';
	return base64_encode(mcrypt_encrypt(MCRYPT_RIJNDAEL_256, $keyString, addpadding($toBeEncrypted), MCRYPT_MODE_CBC, $ivString));
}
function addpadding($string, $blocksize = 32)
{
    $len = strlen($string);
    $pad = $blocksize - ($len % $blocksize);
    $string .= str_repeat(chr($pad), $pad);
    return $string;
}

// AES decryption
function aes_decrypt($EncryptedData, $fileDecryption=false)
{
	$keyString = $_SESSION['aes_key'];
	$ivString = $_SESSION['aes_iv'];
	
	$decrypted_value = strippadding(mcrypt_decrypt(MCRYPT_RIJNDAEL_256, $keyString, $EncryptedData, MCRYPT_MODE_CBC, $ivString));
	//$decrypted_value = mcrypt_decrypt(MCRYPT_RIJNDAEL_256, $keyString, $EncryptedData, MCRYPT_MODE_CBC, $ivString);
	$splits = explode('<ENC_END>', $decrypted_value);
	// If there isn't exactly 2 members in the AES_KEYS string -> leave
	if(count($splits)!=2) { echo('aes_decrypt function : decrypted_value malformed.'); exit(0); }
	
	if($fileDecryption)
	{
		// If we are decrypting a file : Here to prevent SQL injection, we don't replace characters, we encode the string in Base64
		return base64_encode($splits[0]);
	}
	return protectAgainstInjection($splits[0]);
}
function strippadding($string)
{
    $slast = ord(substr($string, -1));
    $slastc = chr($slast);
    $pcheck = substr($string, -$slast);
    if(preg_match("/$slastc{".$slast."}/", $string))
	{
        $string = substr($string, 0, strlen($string)-$slast);
        return $string;
    }
	return false;
}

// Send information to the game WITH AES encryption
function sendAndFinish($message)
{
	// Send message AES encrypted with session token at the end
	echo DELIMITOR.aes_encrypt($message.SEPARATOR.$_SESSION['session_token']).DELIMITOR;
	// Close connection to database properly (if exists)
	if(isset($_SESSION['databaseConnection'])) { mysqli_close($_SESSION['databaseConnection']); }
	// Ensure the end of the current script
	die();
	exit(0);
}
// Send information to the game WITH AES encryption
function sendArrayAndFinish($datas)
{
	// Ensure $datas is not empty
	if(count($datas)<=0) { end_script("sendArrayAndFinish: $datas array is empty"); }
	
	$message = $datas[0];
	for($x=1; $x<count($datas); $x++)
	{
		$message = $message.SEPARATOR.$datas[$x];
	}
	sendAndFinish($message);
}
// Send information to the game WITH AES encryption
function send_SpecialData($message)
{
	echo DELIMITOR.aes_encrypt(base64_decode($message)).DELIMITOR;
}
// Send information to the game WITHOUT AES encryption
function sendDatas_free($message)
{
	echo DELIMITOR.$message.DELIMITOR;
}

// Add salt and hash
function hashPassword($password, $salt)
{
	return hash('sha256', $password.$salt);
}



?>