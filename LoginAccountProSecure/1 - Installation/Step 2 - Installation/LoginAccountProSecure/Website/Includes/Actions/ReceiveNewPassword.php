<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Verify if the email address is linked to an account, if so send an email with a link to reinitialize password account.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//----------------------------: CHECK RECEIVED INFORMATION :-----------------------------------------
// Verify encrypted data presence
if(!isset($_GET['mail'])) { end_script('Authentification', 'Password reinitialization failed, no email address specified.'); }
if(!isset($_GET['code'])) { end_script('Authentification', 'Password reinitialization failed, no code specified.'); }


//------------------------------: GET USER INFORMATION :---------------------------------------------
$mail = protectAgainstInjection($_GET['mail']);
$code = protectAgainstInjection($_GET['code']);
$IP = $_SERVER['REMOTE_ADDR'];


//-------------------------------: SEND LINK TO USER :-----------------------------------------------
$sendPassword_completed = sendPassword($mail, $code, $IP);
if(!$sendPassword_completed) { end_script('Authentification', 'Something went wrong with your reinitialization, please contact an administrator.'); }

?>