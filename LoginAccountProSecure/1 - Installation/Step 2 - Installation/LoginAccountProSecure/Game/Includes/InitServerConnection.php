<?php

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : 	Establish connection to your server and your database
//			The connection to the server and the database is established (an error is displayed if any error occurs)
//			All variables are declared in the 'AccountServerSettings.php' (that's why we ensure to include it)
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Include server settings to get the configuration in order to connect to the database
include_once 'Includes/AccountServerSettings.php';

// Connect to the database
$_SESSION['databaseConnection'] = new mysqli($_SESSION['SERVER_host'], $_SESSION['SERVER_user'], $_SESSION['SERVER_password'], $_SESSION['DB_name']);

?>