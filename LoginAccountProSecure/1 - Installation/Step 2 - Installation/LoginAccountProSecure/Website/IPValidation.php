<?php

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : 	Check SSL connection between the client and the server, and receive encrypted user information for the registration
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Declare template
include_once('./Includes/Templates/RedEagleTemplate.php');
// Set the path to the Models folder
$RET = new Template('./Models');

// Declare and compute all PHP variables
$CURRENT_PAGE = 'IPValidation';
$ACTION = 'IPValidation';

// Here we DON'T include "Server" script since our page doesn't check anything (and no work has to be done in it)

// Declare pages and assign variables
$RET->set_filenames(array($CURRENT_PAGE => $CURRENT_PAGE.'.html'));
// Models variables declaration
if(isset($_POST['MESSAGE'])) { $RET->assign_vars(array('MESSAGE' => $_POST['MESSAGE'])); }
$RET->assign_vars(array('ACTION' => $ACTION));

// Display page
$RET->pparse($CURRENT_PAGE);

?>