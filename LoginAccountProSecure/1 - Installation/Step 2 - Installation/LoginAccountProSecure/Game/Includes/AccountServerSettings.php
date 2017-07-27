<?php
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Configure your server information
//
//	This file HAS TO BE protected !!
//	IF SOMEONE CAN READ THIS FILE : YOU PROTECTION IS DEAD !
//	.htaccess forbid all access to the entire folder so be caution not to place it elsewhere !
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// The name of your game
$_SESSION['GameName'] = 'ChocVrClassSystem';

// Your game version (in case an update is available, you can refuse the connection and alert the client to update)
// Leave '1.0' if it's not important for you right now
$_SESSION['GameVersion'] = '1.0';

// A GuID (enter a random kind of thing as below, change numbers, characters and symbols as you like)
// Caution : it must be the same in your AccountServerSettings.cs script (in the game)
$_SESSION['ServerHash'] = 'CoLwRMQYyNvLe5tjQSIM4TjVaFRd2KOYrHpYBuzJghFqC95kbq#FKciW'; 


// Your domain
$_SESSION['Domain'] = 'www.360vrbox.com';
// The folder (or path) where you put the LoginAccount+ProSecure folder (initially : LoginAccount+ProSecure, if you didn't change it)
$_SESSION['SecureLoginFolder'] = 'LoginAccountProSecure/Game';

// Your host 
$_SESSION['SERVER_host'] = 'qdm1928083.my3w.com'; // Caution : keep 'localhost' EXCEPT if your database server is not on your server (expert only)
// Your username to connect to the database
$_SESSION['SERVER_user'] = 'qdm1928083';
// Your password to connect to the database 
$_SESSION['SERVER_password'] = 'ywz19880103';
// The name of your database
$_SESSION['DB_name'] = 'qdm1928083_db';
// The table where your accounts are saved
$_SESSION['AccountTable'] = 'Account';
// The table where your IPs are saved
$_SESSION['IPTable'] = 'IP';
// The table where your blocked IPs are saved
$_SESSION['AttemptsTable'] = 'Attempts';
// The table where the saveGame information example are saved
$_SESSION['SaveGame'] = 'SaveGame';
// The table used to report abuses
$_SESSION['Report'] = 'Report';

// Your contact email (in case you want to send email validations), players will receive email from this email address (you could create a contact email address for example)
$_SESSION['SERVER_email'] = 'fingerx@foxmail.com';

// The maximum number of wrong attempts before IP being blocked for an account
$_SESSION['AvailableAttemptsBeforeBlocking'] = 10;

// Scan clients IP
define('SCAN_IP_ACTIVATED', TRUE, TRUE);

?>