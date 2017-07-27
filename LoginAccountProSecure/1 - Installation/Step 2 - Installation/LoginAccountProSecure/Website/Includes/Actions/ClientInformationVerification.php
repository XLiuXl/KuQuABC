<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Verification before any action
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Verify IP is not blocked for this account
include_once 'Includes/Actions/VerifyBlockedIP.php';

// Check the specified password matches with the specified username
include_once 'Includes/Actions/IsValidPassword.php';

// Verify if the client's account is validated (in other cases inform the game)
include_once 'Includes/Actions/IsValidatedAccount.php';

// Verify if the client is trying to connect to his account from an IP he already activated (in other cases inform the game AND send the IP address validation email)
include_once 'Includes/Actions/IsValidIP.php';

// We delete attempts made from this account with this IP
$SQL_deleteAttempts = "DELETE FROM ".$SERVER_AttemptsTable." WHERE account_id = '".$accountID."' AND ip = '".$accountIP."' AND action = '".$VerifyBlockedIP_Action."'";
$Result_SQL_deleteAttempts = @mysql_query($SQL_deleteAttempts) or die("DATABASE ERROR CANNOT DELETE ATTEMPTS");

?>