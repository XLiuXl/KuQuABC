
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
	$("#SID").html(sessvars.SecuritySession.SID);
}

/*
 *	Function : decryptSecret
 *	Utility : Main javascript function for the Homepage
 *	Parameters : None
 *	Return : Nothing
 */
function homepage()
{
	$(".Encrypted").each
	(
		function()
		{
			$(this).html(aes_decrypt($(this).html()));
		}
	);
}

/*
 *	Execute as soon as the document is ready
 */
jQuery(document).ready
(
	function($)
	{
		SSLSession();
		homepage();
	}
);
