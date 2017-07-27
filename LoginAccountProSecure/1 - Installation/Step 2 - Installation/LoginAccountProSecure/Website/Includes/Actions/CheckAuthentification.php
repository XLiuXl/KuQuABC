<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Check username, password and IP automatically
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// If AES keys not saved -> Redirect to authentification page
if(!isset($_SESSION['aes_key']) || !isset($_SESSION['aes_iv'])) { end_script('Authentification', 'CheckAuthentification failed, AES security not established.'); }

//------------------------------: GET USER INFORMATION :---------------------------------------------

$username = aes_decrypt(base64_decode($_POST['USERNAME']));
$password = aes_decrypt(base64_decode($_POST['PASSWORD']));

$IP = $_SERVER['REMOTE_ADDR'];

//------------------------------: CHECK USER INFORMATION :---------------------------------------------
checkAuthentification($username, $password, $IP);

?>