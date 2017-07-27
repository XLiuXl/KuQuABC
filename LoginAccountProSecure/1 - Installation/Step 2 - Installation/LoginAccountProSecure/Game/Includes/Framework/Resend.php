<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Resend the email account activation
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//------------------------------: GET USER INFORMATION :--------------------------------------------
$username = $datas[0];
$id = getUsernameID($username);

//------------------------------: SEND EMAIL TO USER :----------------------------------------------
$sendingInformation_completed = sendAccountActivationEmail($id);

//-----------------------------: SUCCESS/ERROR MESSAGE :--------------------------------------------
if($sendingInformation_completed)
{
	// SUCCESS
	sendAndFinish("A link to activate your account has been sent to your email address.");
}

end_script("Something went wrong with your sending process, please contact an administrator.");

?>