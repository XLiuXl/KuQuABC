<?php
session_start();

// Include encryption necessary
include_once './../Game/Includes/Crypt/Random.php';
include_once './../Game/Includes/Crypt/Hash.php';
include_once './../Game/Includes/Crypt/BigInteger.php';
include_once './../Game/Includes/Crypt/RSA.php';
include_once './../Game/Includes/Functions.php';


//-------------------------------------: AES KEYS :--------------------------------------------------
// Decrypt aes_keys encrypted with RSA public key
$aesKeys = $_POST['AES_KEYS'];

// Get information and save them in session array
$keys = explode('<AES_KEYS_SEPARATOR>', $aesKeys);

// If there isn't exactly 2 members in the AES_KEYS string -> leave 
if(count($keys)!=2) { end_script('DecryptAESKeys, AES_KEYS malformed.'); }

// Decode keys from base64 string
$aesKey = base64_decode($keys[0]);
$aesIV = base64_decode($keys[1]);

$_SESSION['aes_key'] = $aesKey;
$_SESSION['aes_iv'] = $aesIV;

sendAndFinish(base64_encode($aesKey)."<DATA_SEPARATOR>".base64_encode($aesIV));


?>