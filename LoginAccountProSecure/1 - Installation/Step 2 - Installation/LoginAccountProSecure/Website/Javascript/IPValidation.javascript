
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
	var $input_username = $("#input_username");
	var $input_password = $("#input_password");
	
	$SID.val(sessvars.SecuritySession.SID);
	$input_username.val(sessvars.SecuritySession.username);
	$input_password.val(sessvars.SecuritySession.password);
}


/*
 *	Function : checkSSLSession
 *	Utility : Read session information, if the SSL session is not initialized : redirect to authentification for call back
 *	Parameters : None
 *	Return : Nothing
 */
function ipvalidation()
{
	// Get inputs to be filled in the AuthentificationForm to send
	var $aes_keys = $("#AES_KEYS");								// The AES_KEYS input to send AES keys encrypted with RSA public key received from the server
	var $username = $("#USERNAME");								// The username sent encrypted
	var $password = $("#PASSWORD");								// The password sent encrypted
	var $ippassword = $("#IPPASSWORD");							// The IP password sent encrypted
	
	var $input_username = $("#input_username");					// The input field where the user actually set his username
	var $input_password = $("#input_password");					// The input field where the user actually set his password
	var $input_ippassword = $("#input_ippassword");				// The input field where the user actually set his IP password
	
	var error = $("#ERROR");
	error.html('');
	var noError = true;
	// Perform verifications
	if($input_username.val().length < 3) { error.html(error.html()+'Username must be at least 3 characters long.'+'<br />'); noError = false; }
	if($input_password.val().length < 3) { error.html(error.html()+'Password must be at least 3 characters long.'+'<br />'); noError = false; }
	if($input_ippassword.val().length < 3) { error.html(error.html()+'IP Password must be at least 3 characters long.'+'<br />'); noError = false; }
	if(!noError) { return false; }
	
	sessvars.SecuritySession.username = $input_username.val();
	sessvars.SecuritySession.password = $input_password.val();
	sessvars.SecuritySession.IPPassword = $input_ippassword.val();
	
	// Set values
	var aes_keys = sessvars.SecuritySession.aes_key+'<AES_KEYS_SEPARATOR>'+sessvars.SecuritySession.aes_iv;
	aes_keys = rsa_encrypt(aes_keys);						// Encrypt aes_keys with RSA public key saved
	$aes_keys.val(aes_keys);								// Set value
	
	// Password and IP password in MD5
	var md5_password = md5($input_password.val());			// Encrypt password with MD5 before sending it to the server (Thus nobody, even administrators, have access to passwords)
	var md5_IP_password = md5($input_ippassword.val());		// Encrypt IP password with MD5 before sending it to the server (Thus nobody, even administrators, have access to passwords)
	
	$username.val(aes_encrypt($input_username.val()));		// Set the username value (AES encrypted but no MD5 here)
	$password.val(aes_encrypt(md5_password));				// Encrypt the password value with aes keys and set the password value (encrypted) in the AuthentificationForm
	$ippassword.val(aes_encrypt(md5_IP_password));			// Set the IP password value (notice the MD5 encryption)
	
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
				ipvalidation();
				return false;			// Caution! We have to stop the current form (with not encrypted password) from leaving : "return false;" stop the sending process.
			}
		);
	}
);
