<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Resend the email account activation
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//----------------------------------: DECRYPT AES KEYS :--------------------------------------------//
// Decrypt AES keys received with RSA private key and save them in session array					//
include_once './Includes/Actions/DecryptAESKeys.php';												//
//--------------------------------------------------------------------------------------------------//


//----------------------------: CHECK RECEIVED INFORMATION :-----------------------------------------
// Verify encrypted data presence
if(!isset($_POST['MAIL'])) { end_script('Resend', 'Resend failed, MAIL not received.'); }


//------------------------------: GET USER INFORMATION :---------------------------------------------
$mail = read($_POST['MAIL']);
$id = getIdFromMail($mail);

//-------------------------------: SEND LINK TO USER :-----------------------------------------------
$sendingInformation_completed = sendAccountActivationEmail($id);
if($sendingInformation_completed) { end_script('Authentification', 'A link to activate your account has been sent to your email address.'); }
else { end_script('Forgot', 'Something went wrong with your sending process, please contact an administrator.'); }

?>