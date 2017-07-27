<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Initiate encryption by generating public and private RSA keys, and we send the public key to the client
//
//	RSA brief explanation :
//	We send the public key generated to the client
//	(Both public and private key are saved in session array)
//	The client encrypt his AES keys with the public key
//	We receive the AES keys encrypted, the private key will allow us to decrypt it : the communication is now made through the AES encryption on both sides
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// If our session information are empty or incomplete : load a new encryption information
if(!isset($_SESSION['Modulus']) || empty($_SESSION['Modulus']) || !isset($_SESSION['Exponent']) || empty($_SESSION['Exponent']) || !isset($_SESSION['PUBLIC_KEY']) || empty($_SESSION['PUBLIC_KEY']) || !isset($_SESSION['PRIVATE_KEY']) || empty($_SESSION['PRIVATE_KEY']))
{
	// Include encryption necessary
	include_once './Includes/Crypt/RSA.php';
	$rsa = new Crypt_RSA();
	$PublicAndPrivateKeysGenerated = extract($rsa->createKey(2048)); // Return an array with $privatekey and $publickey (but publickey and privatekey variables already exist in RSA.php so we use them)
	
	// Clean public and private keys from headers and footers
	$publickey = str_ireplace("-----BEGIN PUBLIC KEY-----", "", $publickey);
	$publickey = str_ireplace("-----END PUBLIC KEY-----", "", $publickey);
	$privatekey = str_ireplace("-----BEGIN RSA PRIVATE KEY-----", "", $privatekey);
	$privatekey = str_ireplace("-----END RSA PRIVATE KEY-----", "", $privatekey);
	
	// Save public and private key in our session array
	$_SESSION["PUBLIC_KEY"] = $publickey;
	$_SESSION["PRIVATE_KEY"] = $privatekey;
	
	// The next line is IMPORTANT
	// We force the RSA script to load the public key, this way we generate the modulus and the exponent, and we save them in our session
	// Here we need an explanation :
	// Depending on the public key the RSA script will generate a modulus and an exponent
	// With this information our C# script will be able to encrypt data that the PHP script can decrypt using the private key
	//
	// Caution though, We advise you to proceed to only one encryption/decryption cycle in one request with the same key/modulus/exponent because the process is really complex and cost time.
	// You can send multiple data though by simply concatenate all strings and add separators between them.
	$rsa->loadKey($_SESSION["PUBLIC_KEY"]);
}


$SSL_Modulus = $_SESSION['Modulus'];
$SSL_Exponent = $_SESSION['Exponent'];

?>