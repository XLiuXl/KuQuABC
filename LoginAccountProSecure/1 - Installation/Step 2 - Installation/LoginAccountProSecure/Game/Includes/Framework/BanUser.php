<?php
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	This script allow administrators to ban a player by specifying his username
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Verify administrator
if(!isset($_SESSION['isAdmin']) || !$_SESSION['isAdmin'])
	end_script("BanUser: Only administrators can have access to administration.");

// Username to ban
$username = $datas[0];
if(!isset($username))
	end_script("BanUser: username not set.");

// Set flag "banned" of the account as 1
$BanRequest = "UPDATE ".$_SESSION['AccountTable']." SET banned=1 WHERE username='".$username."'";
$SQLresult = mysqli_query($_SESSION['databaseConnection'], $BanRequest) or die("BAN REQUEST DATABASE ERROR : ".$BanRequest);

// SUCCESS
sendAndFinish("User banned.");

?>