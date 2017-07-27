<?php
$debug = true;
$ServerScriptCalled = 1;
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Handle communication with the game and execute commands depending to action specified
//	This script handle every single communication the game wants to make
//	This is the only accessible link from the outside (except ValidateAccountByURL.php and ValidateIPByURL.php -> accessed by link)
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


include_once 'Includes/Functions.php'; 					// Include protection functions and tools functions
include_once 'Includes/Session.php'; 					// Begin client session (or return to index if not session ID received)
include_once 'Includes/AccountServerSettings.php'; 		// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/InitServerConnection.php'; 		// Initialize the connection with the database
if(!isset($NotInGame) || !$NotInGame==1)
{ include_once 'Includes/DataVerifications.php'; } 		// Check game information (only if we call this script from the game)

// If Action is ReceiveNewPassword don't check authentification
if(isset($ACTION) && $ACTION=="ReceiveNewPassword") { include_once 'Includes/Framework/ReceiveNewPassword.php'; }


if(isset($_POST['Action'])) { $ACTION = $_POST['Action']; }
else { end_script('Server: No action has been received.'); }

//----------------------------------: AUTHENTIFICATION :--------------------------------------------//
// Check session information automatically for you													//
include_once './Includes/Framework/CheckAuthentification.php';										//
//--------------------------------------------------------------------------------------------------//



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
//	// VERY IMPORTANT :
//	In your new scripts just use the array '$datas' to read information you sent from the game (everything is done for you)
//	You can see this '$datas' array as the exact copy of the array you sent from your C# scripts to the server
//	(You will find 2 good examples on how to use everything very easily in the folder 'Actions')
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//// FRAMEWORK ///////////////////////////////////////////////////////////////////////////////////////
if($ACTION == "Login") { include_once 'Includes/Framework/Login.php'; }
else if($ACTION == "Register") { include_once 'Includes/Framework/Register.php'; }
else if($ACTION == "Modify") { include_once 'Includes/Framework/Modify.php'; }
else if($ACTION == "Forgot") { include_once 'Includes/Framework/Forgot.php'; }
else if($ACTION == "Resend") { include_once 'Includes/Framework/Resend.php'; }
else if($ACTION == "Report") { include_once 'Includes/Framework/Report.php'; }
// ADMIN
else if($ACTION == "AdminLogin") { include_once 'Includes/Framework/AdminLogin.php'; }
else if($ACTION == "Administration") { include_once 'Includes/Framework/Administration.php'; }
else if($ACTION == "GetScreenshot") { include_once 'Includes/Framework/GetScreenshot.php'; }
else if($ACTION == "SaveAdministration") { include_once 'Includes/Framework/SaveAdministration.php'; }
else if($ACTION == "BanUser") { include_once 'Includes/Framework/BanUser.php'; }
//// FRAMEWORK ///////////////////////////////////////////////////////////////////////////////////////



/************************** ACTION ZONE START ******************************/
// Add all your actions below :
// To implement YOUR actions, just copy paste the line below with the name of your action
// Here is 2 examples of how to use everything easily
else if($ACTION == "GetData") { include_once 'Includes/Actions/GetData.php'; }
else if($ACTION == "SendData") { include_once 'Includes/Actions/SendData.php'; }
else if($ACTION == "Savefile") { include_once 'Includes/Actions/Savefile.php'; }

/*************************** ACTION ZONE END ******************************/



// If the action hasn't been declared here we will inform you
else if($debug) { echo('Warning : action ->'.$ACTION.'<- not set in Server.php script'); }
// Close connection to database properly
end_script('');

?>