<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Decrypt AES keys received with RSA private key and save them in session array
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//----------------------------------: CHECK RECEPTION :----------------------------------------------
// Verify encrypted data presence
if(!isset($_POST['AES_KEYS'])) { end_script('Authentification', 'DecryptAESKeys, AES_KEYS not received.'); }



//-------------------------------------: AES KEYS :--------------------------------------------------
// Decrypt aes_keys encrypted with RSA public key
$decryptedAesKeys = rsa_decrypt($_POST['AES_KEYS']);
// Get information and save them in session array
$keys = explode('<AES_KEYS_SEPARATOR>', $decryptedAesKeys);
// If there isn't exactly 2 members in the AES_KEYS string -> leave 
if(count($keys)!=2) { end_script('Authentification', 'DecryptAESKeys, AES_KEYS malformed.'); }
// Decode keys from base64 string
$aesKey = base64_decode($keys[0]);
$aesIV = base64_decode($keys[1]);
if(isset($_SESSION['aes_key']) || isset($_SESSION['aes_iv']))
{
	// If keys are already received don't change it (in case anyone try to change someone else keys)
	if($_SESSION['aes_key'] != $aesKey || $_SESSION['aes_iv'] != $aesIV) { end_script('Authentification', 'DecryptAESKeys, security keys already exist.'); }
}
// Save AES keys in session array
$_SESSION['aes_key'] = $aesKey;
$_SESSION['aes_iv'] = $aesIV;


?>