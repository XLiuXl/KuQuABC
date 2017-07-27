
/*
 *	Function : Authentify
 *	Utility : 	Read session information, if the SSL session is not initialized : redirect to authentification for call back
 *				Otherwise add 3 inputs to the "actionForm" with the SID the USERNAME and the PASSWORD information saved in session array
 *	Parameters : None
 *	Return : Nothing
 */
function Authentify()
{
	if(typeof sessvars === 'undefined' || typeof sessvars.SecuritySession === 'undefined' || typeof sessvars.SecuritySession.SID === 'undefined' || typeof sessvars.SecuritySession.rsa_modulus === 'undefined' || typeof sessvars.SecuritySession.rsa_exponent === 'undefined' || typeof sessvars.SecuritySession.aes_key === 'undefined' || typeof sessvars.SecuritySession.aes_iv === 'undefined')
	{
		window.location.href = "./Authentification.php";
	}
	
	$("#actionForm").html($("#actionForm").html()+'<input id="SID" name="SID" type="hidden" value=""/><input id="USERNAME" name="USERNAME" type="hidden" value=""/><input id="PASSWORD" name="PASSWORD" type="hidden" value=""/>');
	
	var $SID = $("#SID");
	var $input_username = $("#USERNAME");
	var $input_password = $("#PASSWORD");
	
	$SID.val(sessvars.SecuritySession.SID);
	$input_username.val(aes_encrypt(sessvars.SecuritySession.username));
	$input_password.val(aes_encrypt(md5(sessvars.SecuritySession.password)));
	
	associateRLinks();
}

/*
 *	Function : associateRLinks
 *	Utility : 	Get all the RLinks (RedirectionLinks) (all the <a> link with class="RLink")
 *				Prepare all the RLinks "onclick" event to transform the actionForm to the desired page
 *				Launch the actionForm with :
 *					action="[href]"
 *
 *				Everything is automatic just specified the class="RLink" and fill the <a> attribute : "href"
 *				=> href must be page where you want to go (use "Server" for actions only <- you would better set here the name of a page which is including "Server", Server is for actions and PHP variables only)
 *	Parameters : None
 *	Return : Nothing
 */
function associateRLinks()
{
	$(".RLink").each
	(
		function()
		{
			$(this).click
			(
				function()
				{
					// Set the destination page
					$("#actionForm").attr("action", "./"+$(this).attr("href")+".php");
					
					// Clean the form
					$("#ACTION").remove();
					$("#NEWMAIL").remove();
					$("#NEWUSERNAME").remove();
					$("#NEWPASSWORD").remove();
					
					// Submit
					$("#actionForm").submit();
					return false;
				}
			);
		}
	);
}


/*
 *	Execute as soon as the document is ready
 */
jQuery(document).ready ( function($) { Authentify(); } );
