<?php
$ServerScriptCalled = 1;
$debug = true;
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Handle communication with the game and execute commands depending to action specified
//	This script handle every single communication the game wants to make
//	This is the only accessible link from the outside (except ValidateAccountByURL.php and ValidateIPByURL.php -> accessed by link)
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

include_once 'Includes/Functions.php'; 				// Include protection functions and tools functions
include_once 'Includes/Session.php'; 				// Begin client session (or return to index if not session ID received)
include_once 'Includes/AccountServerSettings.php'; 		// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 	// Initialize the connection with the database


// Handle the action the page wants to make
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	CAUTION !
//
//	All actions will be done in other scripts (not in 'Server.php', but included in it)
//	This means that EVERYTIME an "action script" is called we HAVE TO VERIFY if the script has been call from 'Server.php' script
//	In all other cases : WE LEAVE ! (Without doing any action)
//	This is done by checking if $ServerScriptCalled is defined in every single script of the Actions folder.
//	
//	It's an additional protection to the '.htaccess' file forbidding all access to any file in the 'Includes' folder
//	(Technically it's very safe, but we are protected from a incorrectly placed file too, 'never know...)
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
if(isset($_POST['ACTION'])) { $ACTION = $_POST['ACTION']; }

if($ACTION == "OpenSSLconnection") { include_once 'Includes/Actions/OpenSSLconnection.php'; }
else if($ACTION == "EstablishAESSecurity") { include_once 'Includes/Actions/EstablishAESSecurity.php'; }
else if($ACTION == "ShowHomepage") { include_once 'Includes/Actions/ShowHomepage.php'; }
else if($ACTION == "Modify") { include_once 'Includes/Actions/Modify.php'; }
else if($ACTION == "Register") { include_once 'Includes/Actions/Register.php'; }
else if($ACTION == "Forgot") { include_once 'Includes/Actions/Forgot.php'; }
else if($ACTION == "ReceiveNewPassword") { include_once 'Includes/Actions/ReceiveNewPassword.php'; }
else if($ACTION == "IPValidation") { include_once 'Includes/Actions/IPValidation.php'; }
else if($ACTION == "Resend") { include_once 'Includes/Actions/Resend.php'; }
else if($ACTION == "ValidateIP") { include_once 'Includes/Actions/ValidateIP.php'; }

/***************************************************************************/
/************************** ACTION ZONE START ******************************/
// Add all your custom actions below :
// Here we have an example of the action "ActionToDo".
// To implement YOUR custom actions, just copy paste the line below with the name of your action and copy paste the "ActionToDo.php" script you will find in "Includes/Actions" folder
else if($ACTION == "ActionToDo") { include_once 'Includes/Actions/ActionToDo.php'; }
/*************************** ACTION ZONE END ******************************/
/**************************************************************************/

else if($debug) { echo('Warning : action ->'.$ACTION.'<- not set in Server.php script'); }

?>