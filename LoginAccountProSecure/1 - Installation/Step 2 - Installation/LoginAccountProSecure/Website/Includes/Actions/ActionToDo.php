<?php
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	This script is an example of how to use, get and/or send information to the game
//	It's very simple :
//	Add the name of your action in the "Server.php" script in the "Action zone"
//	Copy paste this script and do whatever you want with the database and data, then send it to the game
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Change this if you want to limit the number of attempts for this action
// (or put an empty string it if you want unlimited attempts)
$VerifyBlockedIP_Action = "ActionToDo";

/*************************************************************************************************/
/********************************** THE IMPORTANT SCRIPT *****************************************/
// Always include this script, it checks everything for you, password, IP, account activation, ...
include_once "Includes/Actions/ClientInformationVerification.php";
/********************************** THE IMPORTANT SCRIPT *****************************************/
/*************************************************************************************************/

// Get parameters
if(!isset($_POST["PHP_playerNumber"])) { echo "Error : ActionToDo.php - PHP_playerNumber not received."; if(isset($databaseConnection)) { mysql_close($databaseConnection); } exit(0); }
if(!isset($_POST["PHP_score"])) { echo "Error : ActionToDo.php - PHP_score not received."; if(isset($databaseConnection)) { mysql_close($databaseConnection); } exit(0); }
if(!isset($_POST["PHP_country"])) { echo "Error : ActionToDo.php - PHP_country not received."; if(isset($databaseConnection)) { mysql_close($databaseConnection); } exit(0); }
// Protect server from data
$playerNumber = protectAgainstInjection(aes_decrypt($_POST["PHP_playerNumber"],$_SESSION['aesKey'], $_SESSION['aesIV']));		// The playerNumber
$score = protectAgainstInjection(aes_decrypt($_POST["PHP_score"],$_SESSION['aesKey'], $_SESSION['aesIV']));						// The score
$country = protectAgainstInjection(aes_decrypt($_POST["PHP_country"],$_SESSION['aesKey'], $_SESSION['aesIV']));					// The country

// Do something... Whatever you like with database or anything.
$yourSecretKeyForAnything = "YourMagicKey";
$yourSecretKeyForAnything2 = "YourMagicKey2, number = ".$playerNumber." score = ".$score;
$yourSecretKeyForAnything3 = "YourMagicKey3";

// The "send" function (in Function.php) encrypt with AES key and send the message to the client (for no encryption just use the "echo" PHP function)
send($_SESSION['aesKey'], $_SESSION['aesIV'], $yourSecretKeyForAnything);
send($_SESSION['aesKey'], $_SESSION['aesIV'], $yourSecretKeyForAnything2);
send($_SESSION['aesKey'], $_SESSION['aesIV'], $yourSecretKeyForAnything3);

?>