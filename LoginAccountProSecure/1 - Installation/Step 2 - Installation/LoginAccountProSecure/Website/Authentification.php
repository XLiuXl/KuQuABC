<?php

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	GOAL : 	Open a SSL connection between the client and the server, and receive encrypted user information.
//			The Authentification page generate RSA keys (server side), generate AES keys (client side). The user set his login and password (AES encrypted) and send everything back to the server
//			(AES keys generated client side are encrypted with RSA public key sent by the server)
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Declare template
include_once('./Includes/Templates/RedEagleTemplate.php');
// Set the path to the Models folder
$RET = new Template('./Models');

// Declare and compute all PHP variables
$CURRENT_PAGE = 'Authentification';
$GenerateSID = 1;
$ACTION = 'OpenSSLconnection';

// Compute every PHP work through Server.php script via the "$ACTION" variable
include_once './Server.php';



// Declare pages and assign variables
$RET->set_filenames(array($CURRENT_PAGE => $CURRENT_PAGE.'.html'));
// Models variables declaration
if(isset($_POST['MESSAGE'])) { $RET->assign_vars(array('MESSAGE' => $_POST['MESSAGE'])); }
$RET->assign_vars(array('ACTION' => 'EstablishAESSecurity'));
// Send the SID generated, it's useful to send it once, but since the client save it afterwards the other pages won't need to receive the SID
$RET->assign_vars(array('SID' => $SID));
// Send the RSA public key
$RET->assign_vars(array('RSA_MODULUS' => $SSL_Modulus));
$RET->assign_vars(array('RSA_EXPONENT' => $SSL_Exponent));

// Display page
$RET->pparse($CURRENT_PAGE);

?>