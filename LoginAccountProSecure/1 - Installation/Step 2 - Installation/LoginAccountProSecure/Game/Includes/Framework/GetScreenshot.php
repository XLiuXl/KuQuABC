<?php
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	The script send all reports contained in the report table to be treated by administrators
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Verify administrator
if(!isset($_SESSION['isAdmin']) || !$_SESSION['isAdmin'])
	end_script("GetScreenshot: Only administrators can have access to administration");

// Verify we received the reportId
$reportId = $datas[0];
if(!isset($reportId) || $reportId == "")
	end_script("GetScreenshot: reportId not received.");

$SQLrequest = "SELECT screenshot FROM Report WHERE id = ".$reportId;
$SQLresult = mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die("GET SCREENSHOT DATABASE ERROR! ".$SQLrequest);

// SUCCESS
$datasToSend = array();
if(mysqli_num_rows($SQLresult))
{
	$datasToSend[] = "Screenshot received.";
	if($results = mysqli_fetch_array($SQLresult))
	{
		$datasToSend[] = $results["screenshot"];
	}
	else
	{
		end_script("GetScreenshot: no report found for the id = ".$reportId);
	}
}
else
{
	end_script("GetScreenshot: no report found for the id = ".$reportId);
}
sendArrayAndFinish($datasToSend);

?>