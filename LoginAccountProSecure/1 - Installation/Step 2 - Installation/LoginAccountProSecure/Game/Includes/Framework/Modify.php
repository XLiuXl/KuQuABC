<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Check authentification, if correct : change information
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//------------------------------: GET USER INFORMATION :---------------------------------------------
// $username is defined by CheckAuthentification script
$new_mail = $datas[0];
$new_username = $datas[1];
$new_password = $datas[2];

//---------------------------------: VERIFICATION :--------------------------------------------------
if(strlen($new_mail)<=0 && strlen($new_username)<=0 && strlen($new_password)<=0) { end_script('No information to update.'); }
if($new_mail!="" && strlen($new_mail)<3)
{
	end_script('Email address must be at least 3 characters long.');
	if(!filter_var($new_mail, FILTER_VALIDATE_EMAIL)) { end_script('Email address invalid.'); }
}
if($new_username!="" && strlen($new_username)<3) { end_script('Username must be at least 3 characters long.'); }
if($new_password!="" && strlen($new_password)<3) { end_script('Username must be at least 3 characters long.'); }

//------------------------------: UPDATE INFORMATION :-----------------------------------------------
$update_completed = modify($new_mail, $new_username, $new_password);

//-----------------------------: SUCCESS/ERROR MESSAGE :--------------------------------------------
if($update_completed)
{
	if($new_username!="") { $_SESSION['username'] = $new_username; }
	
	// SUCCESS
	sendAndFinish("Your information have been updated.");
}

end_script("Something went wrong with your modification, please contact an administrator.");

?>