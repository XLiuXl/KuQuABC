<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Verify if the email address is linked to an account, if so send an email with a link to reinitialize password account.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//------------------------------: GET USER INFORMATION :---------------------------------------------
$mail = $datas[0];
$IP = $_SERVER['REMOTE_ADDR'];

//-------------------------------: SEND LINK TO USER :-----------------------------------------------
$reinitPassword_completed = reinitPassword($mail, $IP);

//-----------------------------: SUCCESS/ERROR MESSAGE :--------------------------------------------
if($reinitPassword_completed)
{
	// SUCCESS
	sendAndFinish("A link to reinitialize your password has been sent to your email address.");
}

end_script("Something went wrong with your reinitialization, please contact an administrator.");

?>