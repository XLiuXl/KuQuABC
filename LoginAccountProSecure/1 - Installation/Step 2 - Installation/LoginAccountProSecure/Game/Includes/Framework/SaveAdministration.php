<?php
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	This script allow administrators to make changes on report like remove some of them or check them as "done"
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Verify administrator
if(!isset($_SESSION['isAdmin']) || !$_SESSION['isAdmin'])
	end_script("SaveAdministration: Only administrators can have access to administration.");

// Get all the reports to delete
$toDelete = array();
$toFlagDone = array();
$toFlagNotDone = array();

$datasCount = count($datas);
for($i=0; ($i+2) < $datasCount; $i+=3)
{
	if($datas[$i+2] == "True") 		
		$toDelete[] = $datas[$i];		// The report must be deleted
	else if($datas[$i+1] == "True") 	
		$toFlagDone[] = $datas[$i];		// The report flag "Done" must be True
	else							
		$toFlagNotDone[] = $datas[$i];	// The report flag "Done" must be False
}

// DELETE
$deleteRequest = "DELETE FROM ".$_SESSION['Report']." WHERE id IN (";
$toDeleteCount = count($toDelete);
for($i=0; $i < ($toDeleteCount-1); $i++)
{
	$deleteRequest .= $toDelete[$i].",";
}
if($toDeleteCount > 0)
	$deleteRequest .= $toDelete[$toDeleteCount-1];
else
	$deleteRequest .= "0";
$deleteRequest .= ")";

// DONE = True
$toFlagDoneRequest = "UPDATE ".$_SESSION['Report']." SET done_date=NOW() WHERE done_date IS NULL AND id IN (";
$toFlagDoneCount = count($toFlagDone);
for($i=0; $i < ($toFlagDoneCount-1); $i++)
{
	$toFlagDoneRequest .= $toFlagDone[$i].",";
}
if($toFlagDoneCount > 0)
	$toFlagDoneRequest .= $toFlagDone[$toFlagDoneCount-1];
else
	$toFlagDoneRequest .= "0";
$toFlagDoneRequest .= ")";

// DONE = False
$toFlagNotDoneRequest = "UPDATE ".$_SESSION['Report']." SET done_date=NULL WHERE id IN (";
$toFlagNotDoneCount = count($toFlagNotDone);
for($i=0; $i < ($toFlagNotDoneCount-1); $i++)
{
	$toFlagNotDoneRequest .= $toFlagNotDone[$i].",";
}
if($toFlagNotDoneCount > 0)
	$toFlagNotDoneRequest .= $toFlagNotDone[$toFlagNotDoneCount-1];
else
	$toFlagNotDoneRequest .= "0";
$toFlagNotDoneRequest .= ")";

// Execute requests
$SQLresult = mysqli_query($_SESSION['databaseConnection'], $deleteRequest) or die("DELETE REQUEST DATABASE ERROR : ".$deleteRequest);
$SQLresult = mysqli_query($_SESSION['databaseConnection'], $toFlagDoneRequest) or die("FLAGS DONE REQUEST DATABASE ERROR : ".$toFlagDoneRequest);
$SQLresult = mysqli_query($_SESSION['databaseConnection'], $toFlagNotDoneRequest) or die("FLAGS NOT DONE REQUEST DATABASE ERROR : ".$toFlagNotDoneRequest);

// SUCCESS
sendAndFinish("Report list saved.");

?>