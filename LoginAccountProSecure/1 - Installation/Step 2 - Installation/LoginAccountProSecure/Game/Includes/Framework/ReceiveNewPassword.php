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
if(!isset($_GET['mail'])) { end_script('Password reinitialization failed, no email address specified.'); }
if(!isset($_GET['code'])) { end_script('Password reinitialization failed, no code specified.'); }


//------------------------------: GET USER INFORMATION :---------------------------------------------
$mail = protectAgainstInjection($_GET['mail']);
$code = protectAgainstInjection($_GET['code']);
$IP = $_SERVER['REMOTE_ADDR'];


//-------------------------------: SEND LINK TO USER :-----------------------------------------------
$sendPassword_completed = sendPassword($mail, $code, $IP);
if($sendPassword_completed)
{
	echo("Password reinitialization succeded, a new generated password has been sent to your email address.");
	// Close connection to database properly
	if(isset($_SESSION['databaseConnection'])) { mysqli_close($_SESSION['databaseConnection']); }
	// Ensure the end of the current script
	die();
	exit(0);
}
else { end_script('Something went wrong with your reinitialization, please contact an administrator.'); }

?>