<?php
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	This script is an example of how to use, get and/or send information to the game
//	It's very simple :
//	Add the name of your action in the "Server.php" script in the "Action zone"
//	Create your script starting from this example and you can do whatever you want
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Use those variables (they are defined in the CheckAuthentification.php script) -> (WITHOUT $ !! DO NOT USE IT LIKE THAT: $USERNAME it won't work, use USERNAME)
// USERNAME
// USER_ID
// USER_IP

// Moreover, the array '$datas' is the same as the one you set in your C# script, use it like that:
$data1 = $datas[0];
$data2 = $datas[1];
$data3 = $datas[2];

// Notice that $_SESSION['AccountTable'] is set in the script 'AccountServerSettings.php', if you want to add other tables: create a new script and include it in 'Server.php'
$SQLrequest = "UPDATE ".$_SESSION['AccountTable']." SET Data1 = '".$data1."', Data2 = '".$data2."', Data3 = '".$data3."' WHERE id = '".USER_ID."'";
$SQLresult = mysqli_query($_SESSION['databaseConnection'], $SQLrequest) or die($SQLrequest." = SAVE DATA DATABASE ERROR!");

// SUCCESS
sendAndFinish("Information saved!");

// That's all ! :)






/* 
LITTLE REMINDER
Use the already implemented functions to communicate with the database (you will find them in the Function.php script) or create yours :

// REMEMBER : when you create a table, don't use its name directly, use the session array, like Account, IP and Attempts (in AccountServerSettings.php)

/////////////////////////////////// GET data from a table ////////////////////////////////////////////////////////
$SQL_requestString = "SELECT * FROM tableName WHERE column1 = '".$something."'";
$SQL_requestResult = mysqli_query($SQL_requestString) or die("DATABASE ERROR");
$oneLineFoundAtLeast = mysqli_num_rows($SQL_requestResult);
if($oneLineFoundAtLeast)
{
	$dataFromTable = mysqli_fetch_array($SQL_requestResult); // First line (call mysqli_fetch_array again for the second one and so on)
	$theInformationIncolumn3 = $dataFromTable["column3"];
	$theInformationIncolumn8 = $dataFromTable["column8"];
	$theInformationIncolumn15 = $dataFromTable["column15"];
	// ...
}

/////////////////////////////////// INSERT data in a table ////////////////////////////////////////////////////////
mysqli_query("INSERT INTO tableInYourDatabase (account_id,ip,validation_code,validated,creation_date) VALUES ('$accountID','$IPaddress','$validation_code',1,NOW())") or die("REGISTRATION ERROR");

/////////////////////////////////// DELETE data from a table ////////////////////////////////////////////////////////
$SQL_deleteString = "DELETE FROM tableInYourDatabase WHERE id = '".$IDtoDelete."'";
$SQL_deleteResult = mysqli_query($SQL_deleteString) or die("DELETE ERROR");

/////////////////////////////////// UPDATE data in a table ////////////////////////////////////////////////////////
$SQL_updateString = "UPDATE tableInYourDatabase SET mail = '".$newMail."', username = '".$newUsername."', password = '".$newPassword."' WHERE id = '".$IDtoUpdate."'";
$SQL_updateResult = mysqli_query($SQL_updateString) or die("UPDATE ERROR");
*/

?>