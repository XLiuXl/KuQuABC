<?php
$ServerScriptCalled = 1;
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : 	Validate user account by clicking a link in a validation email
//			This script is accessible from the outside (as well as Script.php and ValidateIPByURL.php)
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//--------------------------------------: INCLUDE FILES :-------------------------------------------------
$GenerateSID = 1;
include_once 'Includes/Functions.php'; 				// Include protection functions and tools functions
include_once 'Includes/Session.php'; 				// Begin client session (or return to index if not session ID received)
include_once 'Includes/AccountServerSettings.php'; 	// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 	// Initialize the connection with the database


//-----------------------------------: CHECK DATA PRESENCE :----------------------------------------------
if(!isset($_GET['username'])) { end_script('Username not received.'); }
if(!isset($_GET['code'])) { end_script('Code not received.'); }

//--------------------------------: PROTECT AGAINST INJECTION :-------------------------------------------
$username = protectAgainstInjection($_GET['username']);
$code = protectAgainstInjection($_GET['code']);

//------------------------------------: GET USER ACCOUNT :------------------------------------------------
$id = getUsernameID($username);
$IP = $_SERVER['REMOTE_ADDR'];
$IPInformation = getIPInformation($id, $IP);
if(is_null($IPInformation)) { end_script('IP Activation failed, this username is not linked to any account.'); }
if($IPInformation['validated']!=0) { end_script('Your IP is already activated for this account. You can connect to your account.'); }
if(strcmp($IPInformation['validation_code'],$code)) { end_script('Your code is incorrect.'); }

//------------------------------------: CHECK IP BLOCKED :------------------------------------------------
checkIPValidationAttempts($id, $IP);
increaseAttempts($id, $IP, 'IP Validation');

//--------------------------------------: ACTIVATE IP :---------------------------------------------------
$IPactivation_completed = activateIP($id, $IP);
if($IPactivation_completed)
{
	echo("Your IP has been successfully activated. You can now log in.");
	// Close connection to database properly
	if(isset($_SESSION['databaseConnection'])) { mysqli_close($_SESSION['databaseConnection']); }
	// Ensure the end of the current script
	die();
	exit(0);
}
else { end_script('IP activation went wrong.'); }

?>