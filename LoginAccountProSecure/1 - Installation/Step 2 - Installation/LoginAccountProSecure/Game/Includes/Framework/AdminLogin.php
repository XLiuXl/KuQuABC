<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Check if authentification is correct and validate it if the account has administrator right
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


if($connection_granted == 1)	// If the connection is granted
{
	$isAdmin = isAdministrator($username);
	
	if($isAdmin)
	{
		// SUCCESS
		$_SESSION['connected'] = true;
		$_SESSION['isAdmin'] = true;
		$_SESSION['username'] = $username;
		$_SESSION['user_id'] = getUsernameID($username);
		$_SESSION['user_ip'] = $_SERVER['REMOTE_ADDR'];
		
		$serverDatas = array(
			"Connection granted.",
			$SID
		);
		
		sendArrayAndFinish($serverDatas);
	}
	end_script("Your account doesn't have administrator's rights.");
}

end_script("Authentification failed.");


?>