<?php

if(isset($_POST['Action'])) { $ACTION = $_POST['Action']; }
if($ACTION != "CheckDatabase") { echo "The action is not set to 'CheckDatabase'."; }

// Check if information has been received correctly
if(!isset($_POST['Host']) || strlen($_POST['Host'])==0) { echo "ERROR : Host is missing !"; exit(0); }
if(!isset($_POST['Name']) || strlen($_POST['Name'])==0) { echo "ERROR : Name is missing !"; exit(0); }
if(!isset($_POST['User']) || strlen($_POST['User'])==0) { echo "ERROR : User is missing !"; exit(0); }
if(!isset($_POST['Password']) || strlen($_POST['Password'])==0) { echo "ERROR : Password is missing !"; exit(0); }

$host = $_POST['Host'];
$name = $_POST['Name'];
$user = $_POST['User'];
$password = $_POST['Password'];

$databaseConnection = new mysqli($host, $user, $password, $name);

// Is the database exists
$request = "SELECT table_name FROM information_schema.tables";
$results = mysqli_query($databaseConnection, $request) or die("ERROR : The database doesn't exists (or not reachable), or the user you used doesn't have privileges to execute requests.");

// Is the Account table exists
$request = "SELECT * FROM Account";
$results = mysqli_query($databaseConnection, $request) or die("ERROR : Account table is missing.");

// Is the Attempts table exists
$request = "SELECT * FROM Attempts";
$results = mysqli_query($databaseConnection, $request) or die("ERROR : Attempts table is missing.");

// Is the IP table exists
$request = "SELECT * FROM IP";
$results = mysqli_query($databaseConnection, $request) or die("ERROR : IP table is missing.");

// Is the SaveGame table exists
$request = "SELECT * FROM SaveGame";
$results = mysqli_query($databaseConnection, $request) or die("ERROR : SaveGame table is missing.");

// Close connection and leave
mysqli_close($databaseConnection);
echo 'SUCCESS';

?>