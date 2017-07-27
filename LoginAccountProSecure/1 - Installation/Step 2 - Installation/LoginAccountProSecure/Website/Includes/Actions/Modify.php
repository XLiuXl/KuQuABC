<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Check authentification, if correct : modify information
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



//----------------------------------: AUTHENTIFICATION :--------------------------------------------//
//Check username, password and IP automatically														//
include_once './Includes/Actions/CheckAuthentification.php';										//
//--------------------------------------------------------------------------------------------------//


//----------------------------: CHECK RECEIVED INFORMATION :-----------------------------------------
// Verify encrypted data presence
if(!isset($_POST['NEWMAIL'])) { end_script('Modify', 'Modification failed, NEWMAIL not received.'); }
if(!isset($_POST['NEWUSERNAME'])) { end_script('Modify', 'Modification failed, NEWUSERNAME not received.'); }
if(!isset($_POST['NEWPASSWORD'])) { end_script('Modify', 'Modification failed, NEWPASSWORD not received.'); }

//------------------------------: GET USER INFORMATION :---------------------------------------------
// $username is defined by CheckAuthentification script
$new_mail = aes_decrypt(base64_decode($_POST['NEWMAIL']));
$new_username = aes_decrypt(base64_decode($_POST['NEWUSERNAME']));
$new_password = aes_decrypt(base64_decode($_POST['NEWPASSWORD']));


//----------------------------------: PREPARATION :--------------------------------------------------
$mailToSend = "";
$usernameToSend = "";

if(strlen($new_mail)<=0 && strlen($new_username)<=0 && strlen($new_password)<=0) { end_script('Modify', 'No information to update.'); }

$keys = array('username'=>"'".$username."'");
$infos = array();
if(strlen($new_mail)>=3)
{
	if(!filter_var($new_mail, FILTER_VALIDATE_EMAIL))
	{
		end_script('Modify', 'Email address invalid.');
	}
	// Update the email address only if it's different from the current one
	$currentMail = getMailFromUsername($username);
	if($currentMail != $new_mail)
	{
		$infos['mail'] = "'".$new_mail."'";
		$mailToSend = $new_mail;
	}
}
if(strlen($new_username)>=3 && $new_username!=$username)
{
	$infos['username'] = "'".$new_username."'";
	$usernameToSend = $new_username;
}
if(strlen($new_password)>=3)
{
	// Add salt to the password and hash it
	$infos['password'] = "'".hashPassword($new_password, $salt)."'";
	$infos['salt'] = "'".$salt."'";
}


//------------------------------: UPDATE INFORMATION :-----------------------------------------------
$update_completed = modify($mailToSend, $usernameToSend, $_SESSION['AccountTable'], $keys, $infos);


if(strlen($new_username)>=3) {$_POST['USERNAME'] = $_POST['NEWUSERNAME'];}
if(strlen($new_password)>=3) {$_POST['PASSWORD'] = $_POST['NEWPASSWORD'];}
if($update_completed) { end_script_redirect_post('Homepage', 'Your information have been updated.'); }

?>