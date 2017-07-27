<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Verify if the email address is linked to an account, if so send an email with a link to reinitialize password account.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//----------------------------------: DECRYPT AES KEYS :--------------------------------------------//
// Decrypt AES keys received with RSA private key and save them in session array					//
include_once './Includes/Actions/DecryptAESKeys.php';												//
//--------------------------------------------------------------------------------------------------//


//----------------------------: CHECK RECEIVED INFORMATION :-----------------------------------------
// Verify encrypted data presence
if(!isset($_POST['MAIL'])) { end_script('Register', 'Forgot failed, MAIL not received.'); }


//------------------------------: GET USER INFORMATION :---------------------------------------------
$mail = read($_POST['MAIL']);
$IP = $_SERVER['REMOTE_ADDR'];

//-------------------------------: SEND LINK TO USER :-----------------------------------------------
$reinitPassword_completed = reinitPassword($mail, $IP);
if($reinitPassword_completed) { end_script('Authentification', 'A link to reinitialize your password has been sent to your email address.'); }
else { end_script('Forgot', 'Something went wrong with your reinitialization, please contact an administrator.'); }

?>