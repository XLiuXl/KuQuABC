<?php

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : 	Redirect to the specified page with the message
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


session_start();

// Declare template
include_once('./Includes/Templates/RedEagleTemplate.php');
// Set the path to the Models folder
$RET = new Template('./Models');

// Declare and compute all PHP variables
$CURRENT_PAGE = 'Message';

// Declare pages and assign variables
$RET->set_filenames(array($CURRENT_PAGE => $CURRENT_PAGE.'.html'));
// Models variables declaration
if(isset($_POST['MESSAGE'])) { $RET->assign_vars(array('MESSAGE' => $_POST['MESSAGE'])); }
$RET->assign_vars(array('REDIRECTION' => $_SESSION['REDIRECTION']));
$RET->assign_vars(array('MESSAGE' => $_SESSION['MESSAGE']));

// Foreach data in post array saved : add a new input in the redirection form
foreach($_SESSION['POSTDATA'] as $key => $value)
{
	if($key!='ACTION' && $key!='AES_KEYS')
	{
		$RET->assign_block_vars
		(
			'postdata',
			array('id' => $key, 'name' => $key, 'value'  => $value)
		);
	}
}
$_SESSION['POSTDATA'] = '';

// Display page
$RET->pparse($CURRENT_PAGE);

?>
