<?php
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Verify received data from the game (hash code, version...)
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


include_once 'Includes/AccountServerSettings.php';									// Include server settings to get the configuration in order to connect to database
include_once 'Includes/InitServerConnection.php';							// Initialize the connection to the server
include_once 'Includes/Functions.php';										// Include protection functions

// Verify information are well received, in any other cases -> show error + stop Session script (connection to the database stopped properly)
if(!isset($_POST["PHP_hash"])) { echo "Error : DataVerifications.php - PHP_hash not received."; if(isset($_SESSION['databaseConnection'])) { mysql_close($_SESSION['databaseConnection']); } exit(0); }
if(!isset($_POST["PHP_gameVersion"])) { echo "Error : DataVerifications.php - PHP_gameVersion not received."; if(isset($_SESSION['databaseConnection'])) { mysql_close($_SESSION['databaseConnection']); } exit(0); }
if(!isset($_POST["PHP_action"])) { echo "Error : DataVerifications.php - PHP_action not received."; if(isset($_SESSION['databaseConnection'])) { mysql_close($_SESSION['databaseConnection']); } exit(0); }

// Get post information received from the game
$gameHash = protectAgainstInjection($_POST["PHP_hash"]); 					// Hash code received from the game
$receivedGameVersion = protectAgainstInjection($_POST["PHP_gameVersion"]); 	// The game version received
$formAction = protectAgainstInjection($_POST["PHP_action"]); 				// The action the game wants to do

// Verify hash code
if($gameHash != protectAgainstInjection($_SESSION['ServerHash']))
{
	echo "Your hash code game is not valid.";
	if(isset($_SESSION['databaseConnection'])) { mysql_close($_SESSION['databaseConnection']); }
	exit(0);
}
// Verify game version
if($receivedGameVersion != $_SESSION['GameVersion'])
{
	echo "Your game version is not valid.";
	if(isset($_SESSION['databaseConnection'])) { mysql_close($_SESSION['databaseConnection']); }
	exit(0);
}

?>