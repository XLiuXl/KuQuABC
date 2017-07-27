<?php

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : 	Begin client session (or return to index if not session ID received)
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


if(isset($_POST["NotConnectedYet"]) && $_POST["NotConnectedYet"])
{
	$GenerateSID = 1;
}

if(isset($GenerateSID) && $GenerateSID==1)					// Force the SID generation
{
	session_id(generateRandomString(30));
}
else if(isset($_POST['SID']))								// If a SID has been received
{
	session_id(protectAgainstInjection($_POST['SID']));
}
else														// Otherwise, check if the current page is authentification (the only page where a new session can be created, except Message.php which is a special page)
{
	end_script('Session problem. No SID received (set $GenerateSID=1 if you want to start the session)');
}

// Launch session
$SID = session_id();
session_start();

?>