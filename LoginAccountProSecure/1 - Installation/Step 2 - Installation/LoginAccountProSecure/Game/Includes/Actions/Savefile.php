<?php
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	This script is an example of how to save, get and/or send files to the server
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Save the file
$savegameName = $datas[0];
$savegameFile = $datas[1];

// If the savegame exists -> update
if(checkSavegameExists(USER_ID, $savegameName))
{
	// WHERE condition
	$keys = array();
	$keys['account_id'] = USER_ID;
	$keys['name'] = "'".$savegameName."'";
	
	// Information to update
	$infos = array();
	$infos['file'] = "'".$savegameFile."'";
	
	if(!update_table($_SESSION['SaveGame'], $keys, $infos)) { end_script("Impossible to update savegame information."); }
}
else
{
	$SQLrequest = "INSERT INTO ".$_SESSION['SaveGame']." (account_id,name,file) VALUES (".USER_ID.",'".$savegameName."','".$savegameFile."')";
	mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die("ADD SAVE GAME ERROR: ".$SQLrequest);
}


// Get the file back and send it to the client
$SQLrequest = "SELECT file FROM ".$_SESSION['SaveGame']." WHERE account_id = '".USER_ID."' AND name = '".$savegameName."'";
$SQLresult = mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die("ERROR GET SAVEGAME: ".$SQLrequest);
if(mysqli_num_rows($SQLresult))
{
	// SUCCESS
	$results = mysqli_fetch_array($SQLresult);
	
	$savegameFile = $results["file"];
	$serverDatas = array(
			"Savegame transfered, read the file Savegame1_Received.txt",
			$savegameFile
	);
	sendArrayAndFinish($serverDatas);
}



end_script("Cannot get savegame back.");

?>