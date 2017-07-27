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
if(!isset($_POST['USERNAME'])) { end_script('IPValidation', 'IP Validation failed, USERNAME not received.'); }
if(!isset($_POST['PASSWORD'])) { end_script('IPValidation', 'IP Validation failed, PASSWORD not received.'); }
if(!isset($_POST['IPPASSWORD'])) { end_script('IPValidation', 'IP Validation failed, IPPASSWORD not received.'); }

//------------------------------: GET USER INFORMATION :---------------------------------------------
$username = read($_POST['USERNAME']);
$password = read($_POST['PASSWORD']);
$IPpassword = read($_POST['IPPASSWORD']);
$IP = $_SERVER['REMOTE_ADDR'];

//------------------------------: GET USER INFORMATION :---------------------------------------------
$id = getUsernameID($username);
checkIPValidationAttempts($id, $IP);
increaseAttempts($id, $IP, 'IP Validation');

//-----------------------------: IDENTITY VERIFICATION :---------------------------------------------
$correctPassword = passwordVerification($username, $password);
if(!$correctPassword) { end_script('IPValidation', 'Your account does not exists or your password is incorrect.'); }
$correctIPPassword = IPpasswordVerification($username, $IPpassword);
if(!$correctIPPassword) { end_script('IPValidation', 'Incorrect IP password.'); }

//----------------------------------: ACTIVATE IP :--------------------------------------------------
$IPActivation_completed = activateIP($id, $IP);
if(!$IPActivation_completed) { end_script('IPValidation', 'Your IP cannot be activated, please contact an administrator.'); }

// IP activated -> connect the user
end_script_redirect_post('Homepage', 'Your IP has been successfully activated.');

?>