<?php
session_start();

// Include encryption necessary
include_once './../Game/Includes/Crypt/Random.php';
include_once './../Game/Includes/Crypt/Hash.php';
include_once './../Game/Includes/Crypt/BigInteger.php';
include_once './../Game/Includes/Crypt/RSA.php';
include_once './../Game/Includes/Functions.php';

// Create RSA object
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

// Send the certificate information
echo $_SESSION["PRIVATE_KEY"]."<DATA_SEPARATOR>".$_SESSION['Modulus']."<DATA_SEPARATOR>".$_SESSION['Exponent'];

?>