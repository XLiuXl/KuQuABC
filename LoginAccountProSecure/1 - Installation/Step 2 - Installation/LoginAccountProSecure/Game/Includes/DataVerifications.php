<?php
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Verify received data from the game (hash code, version...)
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


include_once 'Includes/Functions.php';										// Include protection functions
include_once 'Includes/AccountServerSettings.php';							// Include server settings to get the configuration in order to connect to database
include_once 'Includes/InitServerConnection.php';							// Initialize the connection to the server

// Verify information are well received, in any other cases -> show error + stop Session script (connection to the database stopped properly)
if(!isset($_POST["Hash"])) { end_script("Error : DataVerifications.php - Hash not received."); }
if(!isset($_POST["GameVersion"])) { end_script("Error : DataVerifications.php - GameVersion not received."); }
if(!isset($_POST["Action"])) { end_script("Error : DataVerifications.php - Action not received."); }

// Get post information received from the game
$gameHash = protectAgainstInjection($_POST["Hash"]); 						// Hash code received from the game
$receivedGameVersion = protectAgainstInjection($_POST["GameVersion"]); 		// The game version received
$formAction = protectAgainstInjection($_POST["Action"]); 					// The action the game wants to do

// Check hash code
if($gameHash != protectAgainstInjection($_SESSION['ServerHash'])) { end_script("Error : Your hash code game is not valid : ".$gameHash." != ".protectAgainstInjection($_SESSION['ServerHash'])); }

// Check game version
if($receivedGameVersion != $_SESSION['GameVersion']) { end_script("Error : Your game version is not valid."); }

?>