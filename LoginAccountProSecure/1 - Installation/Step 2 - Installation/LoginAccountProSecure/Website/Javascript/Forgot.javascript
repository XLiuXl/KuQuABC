
/*
 *	Function : checkSSLSession
 *	Utility : Read session information, if the SSL session is not initialized : redirect to authentification for call back
 *	Parameters : None
 *	Return : Nothing
 */
function SSLSession()
{
	if(typeof sessvars === 'undefined' || typeof sessvars.SecuritySession === 'undefined' || typeof sessvars.SecuritySession.SID === 'undefined' || typeof sessvars.SecuritySession.rsa_modulus === 'undefined' || typeof sessvars.SecuritySession.rsa_exponent === 'undefined' || typeof sessvars.SecuritySession.aes_key === 'undefined' || typeof sessvars.SecuritySession.aes_iv === 'undefined')
	{
		window.location.href = "./Authentification.php";
	}
	var $SID = $("#SID");
	$SID.val(sessvars.SecuritySession.SID);
}


/*
 *	Function : checkSSLSession
 *	Utility : Read session information, if the SSL session is not initialized : redirect to authentification for call back
 *	Parameters : None
 *	Return : Nothing
 */
function forgot()
{
	// Get inputs to be filled in the AuthentificationForm to send
	var $aes_keys = $("#AES_KEYS");								// The AES_KEYS input to send AES keys encrypted with RSA public key received from the server
	var $mail = $("#MAIL");										// The email address sent encrypted
	var $input_mail = $("#input_mail");							// The input field where the user actually set his email address
	
	var error = $("#ERROR");
	error.html('');
	if($input_mail.val().length <3 || $input_mail.val().indexOf("@") == -1 || $input_mail.val().indexOf(".") == -1) { error.html(error.html()+'Incorrect email address.'+'<br />'); return false; }
	
	// Set values
	var aes_keys = sessvars.SecuritySession.aes_key+'<AES_KEYS_SEPARATOR>'+sessvars.SecuritySession.aes_iv;
	aes_keys = rsa_encrypt(aes_keys);						// Encrypt aes_keys with RSA public key saved
	$aes_keys.val(aes_keys);								// Set value
	$mail.val(aes_encrypt($input_mail.val()));				// Set the mail value (AES encrypted but no MD5 here)
	
	// The form is ready -> send it
	$('#actionForm').submit();
	return true;
}

/*
 *	Execute as soon as the document is ready
 */
jQuery(document).ready
(
	function($)
	{
		SSLSession();
		$('#InputForm').submit			// On submit : execute the transformation and encryption functions
		(
			function()
			{
				forgot();
				return false;			// Caution! We have to stop the current form (with not encrypted password) from leaving : "return false;" stop the sending process.
			}
		);
	}
);
