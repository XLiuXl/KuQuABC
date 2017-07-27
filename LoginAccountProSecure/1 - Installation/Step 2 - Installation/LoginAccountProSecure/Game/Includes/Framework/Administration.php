<?php
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	The script send all reports contained in the report table to be treated by administrators
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Verify administrator
if(!isset($_SESSION['isAdmin']) || !$_SESSION['isAdmin'])
	end_script("Administration: Only administrators can have access to administration");

$SQLrequest = "SELECT ".$_SESSION['Report'].".id as id, ".$_SESSION['Report'].".creation_date as creation_date, ".$_SESSION['Report'].".done_date as done_date, ".$_SESSION['Report'].".message as message, ".$_SESSION['Report'].".screenshot as screenshot, ".$_SESSION['AccountTable'].".username as username FROM Report JOIN ".$_SESSION['AccountTable']." ON reporter_id = ".$_SESSION['AccountTable'].".id";
$SQLresult = mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die("GET REPORTS DATABASE ERROR! ".$SQLrequest);

// SUCCESS
$datasToSend = array();
if(mysqli_num_rows($SQLresult))
{
	$datasToSend[] = "Report list received.";
	while($results = mysqli_fetch_array($SQLresult))
	{
		$datasToSend[] = $results["id"];						// The id of the report (NOT the reporter, the report)
		$datasToSend[] = $results["creation_date"];				// The date of the report
		$datasToSend[] = $results["username"];					// The reporter username
		$datasToSend[] = $results["message"];					// The reporter message
		// We don't send the screenshot so the loading is lazy (loaded only on demand of each screenshot) otherwise it takes too long to get all of them at once
		//$datasToSend[] = $results["screenshot"];				// The screenshot image
		$datasToSend[] = $results["done_date"]==null ? "False" : "True";	// Done if the doneDate is not null
	}
}
else
{
	$datasToSend[] = "No report to display.";
}
sendArrayAndFinish($datasToSend);

?>