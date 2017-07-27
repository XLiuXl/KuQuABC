<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Decrypt and save AES keys in session array, redirect to homepage with information received
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//----------------------------------: DECRYPT AES KEYS :--------------------------------------------//
// Decrypt AES keys received with RSA private key and save them in session array					//
include_once './Includes/Actions/DecryptAESKeys.php';												//
//--------------------------------------------------------------------------------------------------//


// Authentification granted -> Redirect to homepage
end_script_redirect_post('Homepage', 'RSA + AES security established.');

?>