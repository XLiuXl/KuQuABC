using UnityEngine;
using System.Collections;

public static class ConfigurationPaths
{
	// Path to the htaccess file for the game folder
	public static string htaccessFile_game
	{ get { return Application.dataPath + "/LoginAccountProSecure/1 - Installation/Step 2 - Installation/LoginAccountProSecure/Game/Includes/"; } }
	
	// Path to the htaccess file for the website folder
	public static string htaccessFile_website
	{ get { return Application.dataPath + "/LoginAccountProSecure/1 - Installation/Step 2 - Installation/LoginAccountProSecure/Website/Includes/"; } }
	
	
	// Path to the htaccess file for the website folder
	public static string configurationFile
	{ get { return Application.dataPath + "/LoginAccountProSecure/Framework/ProjectConfiguration.cfg"; } }
	
	
	// Path to the certificate file
	public static string privateCertificateFile
	{ get { return Application.dataPath + "/LoginAccountProSecure/1 - Installation/Step 2 - Installation/LoginAccountProSecure/Game/Includes/PrivateCertificate.crt"; } }

	// Path to the certificate file
	public static string publicCertificateFile
	{ get { return Application.dataPath + "/LoginAccountProSecure/Framework/Resources/PublicCertificate.txt"; } }
	

}
