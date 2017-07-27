<?php
// Here we check if we are called from 'Server.php' script : in any other cases WE LEAVE !
if(!isset($ServerScriptCalled)) { exit(0); }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : Compute every homepage information needed
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//----------------------------------: AUTHENTIFICATION :--------------------------------------------//
//Check username, password and IP automatically														//
include_once './Includes/Actions/CheckAuthentification.php';										//
//--------------------------------------------------------------------------------------------------//


// Execute request
$SQLrequest = "SELECT * FROM Account WHERE username = '".$username."'";
$SQLresult = mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die("GET DATA1 DATABASE ERROR! ".$SQLrequest);

// Get data
$data1 = "";
if(mysqli_num_rows($SQLresult))
{
	$results = @mysqli_fetch_array($SQLresult);
	$data1 = $results["Data1"];
}

?>