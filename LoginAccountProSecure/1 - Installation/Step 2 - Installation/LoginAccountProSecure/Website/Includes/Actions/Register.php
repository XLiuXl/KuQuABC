<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Accept the client as new user, if the mail and login aren't already taken
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//----------------------------------: DECRYPT AES KEYS :--------------------------------------------//
// Decrypt AES keys received with RSA private key and save them in session array					//
include_once './Includes/Actions/DecryptAESKeys.php';												//
//--------------------------------------------------------------------------------------------------//


//----------------------------: CHECK RECEIVED INFORMATION :-----------------------------------------
// Verify encrypted data presence
if(!isset($_POST['MAIL'])) { end_script('Register', 'Registration failed, MAIL not received.'); }
if(!isset($_POST['USERNAME'])) { end_script('Register', 'Registration failed, USERNAME not received.'); }
if(!isset($_POST['PASSWORD'])) { end_script('Register', 'Registration failed, PASSWORD not received.'); }

//------------------------------: GET USER INFORMATION :---------------------------------------------
$mail = read($_POST['MAIL']);
$username = read($_POST['USERNAME']);
$password = read($_POST['PASSWORD']);
$IP = $_SERVER['REMOTE_ADDR'];

//----------------------------------: REGISTER USER :-------------------------------------------------
$registration_completed = register($mail, $username, $password, $IP);
if($registration_completed) { end_script('Authentification', 'Registration completed, you can now log in.'); }
else { end_script('Authentification', 'Something went wrong with your registration, please contact an administrator.'); }

?>