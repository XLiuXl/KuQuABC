<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

//////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Accept the client as new user, if the mail and login aren't already taken
//
//////////////////////////////////////////////////////////////////////////////////////////


//------------------------------: GET USER INFORMATION :---------------------------------------------
$mail = $datas[0];
$username = $datas[1];
$password = $datas[2];
$IP = $_SERVER['REMOTE_ADDR'];

//--------------------------------: REGISTER USER :--------------------------------------------------
$registration_completed = register($mail, $username, $password, $IP);

//---------------------------: ACCEPTED/REFUSED MESSAGE :--------------------------------------------
if($registration_completed)
{
	// SUCCESS
	sendAndFinish("Registration accepted, a link to activate your account has been sent to your email address.");
}

end_script("Registration failed, something went wrong with your registration, please contact an administrator.");

?>