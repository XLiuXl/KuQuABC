<?php
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	This script is used to allow players to report abuse in game
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

$message = $datas[0];
$screenshot = $datas[1];

// If the savegame exists -> update
if(isset($message) && isset($screenshot))
{
	$SQLrequest = "INSERT INTO ".$_SESSION['Report']." (creation_date,reporter_id,message,screenshot) VALUES (NOW(),".USER_ID.",'".$message."','".$screenshot."')";
	mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die("ADD REPORT ERROR: ".$SQLrequest);
	
	$serverDatas = array(
			"Abuse reported, an administrator will study the case."
	);
	sendArrayAndFinish($serverDatas);
}

end_script("Cannot get savegame back.");

?>