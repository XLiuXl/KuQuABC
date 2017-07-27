
/*
 *	Function : 		getInternetExplorerVersion
 *	Utility : 		Detect IE11 version since there is a known bug in the 11th version : window.name is reset at every request. It unfortunately prevent users from using Javascript session
 *					It's a major bug, admitted by Microsoft but they won't correct the issue (They focus on Spartan now)
 *					Anyway, we inform users that they have to (use a real browser) or just press the key F12 (to open development window) and the bug flies away
 *	Parameters : 	None
 *	Return : 		Nothing
 */
function getInternetExplorerVersion()
{
  var rv = -1;
  if (navigator.appName == 'Microsoft Internet Explorer')
  {
    var ua = navigator.userAgent;
    var re  = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
    if (re.exec(ua) != null)
      rv = parseFloat( RegExp.$1 );
  }
  else if (navigator.appName == 'Netscape')
  {
    var ua = navigator.userAgent;
    var re  = new RegExp("Trident/.*rv:([0-9]{1,}[\.0-9]{0,})");
    if (re.exec(ua) != null)
      rv = parseFloat( RegExp.$1 );
  }
  return rv;
}
if(getInternetExplorerVersion()=='11')
{
	alert("We detect you are using Internet Explorer 11.\nInternet Explorer 11 is bugged, Microsoft admitted the issue but won't do anything, so you won't be able to use Javascript sessions.\n\nYou have 2 solutions:\n- Use a real browser (Firefox, Chrome, Opera, Safari, ...)\n- Press your F12 key (to open the development window so the Internet Explorer bug is fixed, keep it opened but resize it)");
}

/*
 *	Function : 		initializeSession
 *	Utility : 		Create Javascript session to allow persistence memory between pages. CAUTION : Javascript session persistence only work in the SAME browser window.
 *	Parameters : 	None
 *	Return : 		Nothing
 */
function initializeSession()
{
	// You can add any field you want in the SecuritySession
	sessvars.SecuritySession =
	{
		SID: '',
		mail: '',
		username: '',
		password: '',
		IPPassword: '',
		rsa_modulus: '',
		rsa_exponent: '',
		aes_key: '',
		aes_iv: ''
	}
	sessvars.SecuritySession.SID = $("#SID").val();
	
	// You could even add new object in sessvars global object like username and password, or name and birth date... like that :
	/*
	sessvars.TheNameOfYourObject =
	{
		Variable1: 'myInitialValue', 		// Don't forget the comma
		Variable2: 'AnotherInitialValue', 	// Don't forget the comma
		YourVariable3: 351
	}
	*/
}

/*
 *	Function : 		encryptDataAndSendForm
 *	Utility : 		Get HTML objects and fill information
 *	Parameters : 	None
 *	Return : 		Nothing
 */
function authentification()
{
	// Get inputs to be filled in the AuthentificationForm to send
	var $aes_keys = $("#AES_KEYS");							// The AES_KEYS input to send AES keys encrypted with RSA public key received from the server
	var $username = $("#USERNAME");							// The username input
	var $input_username = $("#input_username");				// The input field where the user actually set his username
	var $password = $("#PASSWORD");							// The password input sent encrypted by calling "aes_encrypt" function
	var $input_password = $("#input_password");				// The input field where the user actually set his password
	
	// Save username and password in local session
	sessvars.SecuritySession.username = $input_username.val();
	sessvars.SecuritySession.password = $input_password.val();
	
	// Perform verifications
	var error = $("#ERROR");
	error.html('');
	var noError = true;
	if($input_username.val().length < 3) { error.html(error.html()+'Username must be at least 3 characters long.'+'<br />'); noError = false; }
	if($input_password.val().length < 3) { error.html(error.html()+'Password must be at least 3 characters long.'+'<br />'); noError = false; }
	if(!noError) { return false; }
	
	// Set values
	var aes_keys = sessvars.SecuritySession.aes_key+'<AES_KEYS_SEPARATOR>'+sessvars.SecuritySession.aes_iv;
	aes_keys = rsa_encrypt(aes_keys);						// Encrypt aes_keys with RSA public key received
	$aes_keys.val(aes_keys);								// Set value
	
	$username.val(aes_encrypt($input_username.val()));		// Set the username value (AES encrypted but no MD5 here)
	var md5_password = md5($input_password.val());			// Encrypt password with MD5 before sending it to the server (Thus nobody, even administrators, have access to passwords)
	$password.val(aes_encrypt(md5_password));				// Encrypt the password value with aes keys and set the password value (encrypted) in the AuthentificationForm
	$input_password.val("Password encryption done.");		// Erase the password input value just in case
	
	// The form is ready -> send it
	$('#AuthentificationForm').submit();
	return true;
}

/*
 *	Function : 		encryptDataAndSendForm
 *	Utility : 		Read RSA modulus and exponent received
 *	Parameters : 	None
 *	Return : 		Nothing
 */
function readRsaPublicKey()
{
	sessvars.SecuritySession.rsa_modulus = $("#rsa_modulus").html();
	sessvars.SecuritySession.rsa_exponent = $("#rsa_exponent").html();
}

/*
 *	Execute as soon as the document is ready
 */
jQuery(document).ready
(
	function($)
	{
		initializeSession();			// Initialize Javascript session
		readRsaPublicKey();				// Concatenate modulus and exponent received to get RSA public key
		generateAesKeys();				// Generate AES keys (and save keys in Javascript array session)
		$('#InputForm').submit			// On submit : execute the authentification function
		(
			function()
			{
				authentification();
				return false;			// Caution! We have to stop the current form (with not encrypted password) from leaving : "return false;" stop the sending process.
			}
		);
	}
);
