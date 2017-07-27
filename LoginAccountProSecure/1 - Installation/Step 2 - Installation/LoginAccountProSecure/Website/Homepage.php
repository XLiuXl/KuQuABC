<?php

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : 	Show the main screen
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Declare template
include_once('./Includes/Templates/RedEagleTemplate.php');
// Set the path to the Models folder
$RET = new Template('./Models');

// Declare and compute all PHP variables
$CURRENT_PAGE = 'Homepage';
$ACTION = 'ShowHomepage';					// Important : Here the action "ShowHomepage.php" will be executed (to fill variables we are going to use below, like "$data1")

// Compute every PHP work through Server.php script via the "$ACTION" variable
include_once './Server.php';


// Now that the script "Homepage.php" is executed, we can create the "Homepage.html" and set its variables
$RET->set_filenames(array($CURRENT_PAGE => $CURRENT_PAGE.'.html'));		// Create the page and assign variables

if(isset($_POST['MESSAGE']))
{
	$RET->assign_vars(array('MESSAGE' => $_POST['MESSAGE']));
}
// Never forget to encrypt data you want to display !
$RET->assign_vars(array('USERNAME' => aes_encrypt($username)));
$RET->assign_vars(array('PASSWORD' => aes_encrypt($password)));
$RET->assign_vars(array('DATA1' => aes_encrypt($data1)));				// Assign the variable DATA1 that you will use in the page Homepage.html using {DATA1}

$RET->pparse($CURRENT_PAGE);											// Display page

?>