
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
	var $input_mail = $("#input_mail");
	var $input_username = $("#input_username");
	var $input_password = $("#input_password");
	var $input_password_confirmation = $("#input_password_confirmation");
	
	$SID.val(sessvars.SecuritySession.SID);
	$input_mail.val(sessvars.SecuritySession.mail);
	$input_username.val(sessvars.SecuritySession.username);
	$input_password.val(sessvars.SecuritySession.password);
	$input_password_confirmation.val(sessvars.SecuritySession.password);
}

/*
 *	Function : checkSSLSession
 *	Utility : Read session information, if the SSL session is not initialized : redirect to authentification for call back
 *	Parameters : None
 *	Return : Nothing
 */
function register()
{
	// Get inputs to be filled in the AuthentificationForm to send
	var $aes_keys = $("#AES_KEYS");								// The AES_KEYS input to send AES keys encrypted with RSA public key received from the server
	var $mail = $("#MAIL");										// The email address sent encrypted
	var $username = $("#USERNAME");								// The username sent encrypted
	var $password = $("#PASSWORD");								// The password sent encrypted
	
	var $input_mail = $("#input_mail");												// The input field where the user actually set his email address
	var $input_username = $("#input_username");										// The input field where the user actually set his username
	var $input_password = $("#input_password");										// The input field where the user actually set his password
	var $input_password_confirmation = $("#input_password_confirmation");			// The input field where the user actually set his password confirmation
	
	var error = $("#ERROR");
	error.html('');
	var noError = true;
	// Perform verifications
	if($input_mail.val().indexOf("@") == -1 || $input_mail.val().indexOf(".") == -1) { error.html(error.html()+'Incorrect email address.'+'<br />'); noError = false; }
	if($input_username.val().length < 3) { error.html(error.html()+'Username must be at least 3 characters long.'+'<br />'); noError = false; }
	if($input_password.val().length < 3) { error.html(error.html()+'Password must be at least 3 characters long.'+'<br />'); noError = false; }
	if($input_password.val() != $input_password_confirmation.val()) { error.html(error.html()+'Incorrect password confirmation.'+'<br />'); noError = false; }
	if(!noError) { return false; }
	
	// Save username and password in local session
	sessvars.SecuritySession.mail = $input_mail.val();
	sessvars.SecuritySession.username = $input_username.val();
	sessvars.SecuritySession.password = $input_password.val();
	
	// Password in MD5
	var md5_password = md5($input_password.val());			// Encrypt password with MD5 before sending it to the server (Thus nobody, even administrators, have access to passwords)
	
	// Set values
	var aes_keys = sessvars.SecuritySession.aes_key+'<AES_KEYS_SEPARATOR>'+sessvars.SecuritySession.aes_iv;
	aes_keys = rsa_encrypt(aes_keys);						// Encrypt aes_keys with RSA public key saved
	$aes_keys.val(aes_keys);								// Set value
	
	$mail.val(aes_encrypt($input_mail.val()));				// Set the mail value (AES encrypted but no MD5 here)
	$username.val(aes_encrypt($input_username.val()));		// Set the username value (AES encrypted but no MD5 here)
	
	$password.val(aes_encrypt(md5_password));				// Encrypt the password value with aes keys and set the password value (encrypted) in the AuthentificationForm
	
	// The register form is ready -> send it
	$('#RegisterForm').submit();
	return true;
}

/*
 *	Execute as soon as the document is ready
 */
jQuery(document).ready
(
	function($)
	{
		SSLSession();					// Read session information, if the SSL session is not initialized : redirect to authentification for call back
		$('#InputForm').submit			// On submit : execute the register function
		(
			function()
			{
				register();
				return false;			// Caution! We have to stop the current form (with not encrypted password) from leaving : "return false;" stop the sending process.
			}
		);
	}
);
