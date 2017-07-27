using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
#if NETFX_CORE
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
#endif

// Here we will use the new SceneManager if we are not currently using an older Unity version than the 5.3
#if !(UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
using UnityEngine.SceneManagement;
#endif

/// <summary>
/// Utils
/// Gather every generic methods to simplify scripts and ensure security
/// </summary>
public static class UtilsProSecure
{
	// Separators (for communication)
	public static readonly string separator = "<DATA_SEPARATOR>";
	public static readonly string delimitor = "<ENCRYPTED_DATA_DELIMITOR>";
	
	// Don't change it, it must be synchronized with the PHP code on your server (expert only)
	private static readonly int keySize = 256; // DON'T CHANGE IT
	private static readonly int blockSize = 256; // DON'T CHANGE IT
	private static int tokenSize = 128;	// Security
	private static string stringCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890?#";	// Util for random strings
	
	
	
	/// <summary>
	/// Method used to Load a scene
	/// Since Unity 5.3 integrate a new system to load scenes
	/// We have a method to deal with Unity version and loading process
	/// </summary>
	public static void Load(string sceneName)
	{
		#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			Application.LoadLevel(sceneName);
		#else
			SceneManager.LoadScene(sceneName);
		#endif
	}
	
	/// <summary>
	/// This method generate AES keys.
	/// Generate a new session token (just a random string) used as a session secret key.
	/// Read the public certificate to fill modulus and exponent of the UserSession.
	/// </summary>
	public static void PrepareSecurityInformation()
	{
		// Generate AES keys (and save aes_key and aes_iv in UserSession)
		generateAESkeyAndIV(ref UserSession.AES_Key, ref UserSession.AES_IV);
		
		// Generate random token
		// This is useful to check if the server did correctly decrypt our information.
		// This way we are sure that the server is authentic, because it's the only one to know the private key (and the only one able to decrypt our message)
		// So if a man-in-the-middle was receiving our information (without decrypting anything at all, because he wouldn't have the RSA private key)
		// He would just send us "Hey, yeah you are authenfied, don't worry I'm the server"
		// We would send him our information, encrypted with the AES key (that he wouldn't have), here is why we do that :
		// -> if the connection is established, the token we received (AES encrypted) is the correct one : no problem
		//    We know the entity we are talking to has the private key, we know only the server has it : so we are sure it's the server
		// -> However, if the token is not correct, the decryption hasn't been made by the entity we are talking to
		//    So the connection is refused, RSA information hasn't been correctly decrypted, we don't send any information (even AES encrypted)
		//    Otherwise we wouldn't check the server identity and possibly send information AES encrypted (but this way we don't send anything, which is better)
		generateToken(ref UserSession.session_token);
		
		// Read certificate to get the RSA public key (of the server) (and save publicModulus and publicExponent in UserSession)
		readPublicCertificate(ref UserSession.publicModulus, ref UserSession.publicExponent);
	}
	
	/// <summary>
	/// Read the public certificate to fill modulus and exponent of the UserSession.
	/// </summary>
	private static void readPublicCertificate(ref string modulus, ref string exponent)
	{
		TextAsset bindata = Resources.Load("PublicCertificate") as TextAsset; // CAUTION your resource must have a .txt extension
		//string publicCertificate = readTextFile(ConfigurationPaths.publicCertificateFile);
		string publicCertificate = bindata.text;
		string[] publicCertificateInfos = publicCertificate.Split (new string[] { "<CERTIFICATE_SEPARATOR>" }, StringSplitOptions.None);
		modulus = publicCertificateInfos[0];
		exponent = publicCertificateInfos[1];
		if(modulus.Equals("") || exponent.Equals(""))
		{
			Debug.LogError("Security information problem:\n" + "Modulus = [" + modulus + "]" + "\n\nExponent = [" + exponent + "]");
		}
	}
	
	/// <summary>
	/// Encrypt the parameter (string) with RSA public key.
	/// After that only the owner of the private certificate can read it (the server)
	/// </summary>
	public static string RSA_encrypt(string toBeEncrypted)
	{
		string modulus = UserSession.publicModulus;
		string exponent = UserSession.publicExponent;
		
		if(modulus.Equals("") || exponent.Equals(""))
		{
			Debug.LogError("EncryptWithPublicKey : Modulus or Exponent not received !");
			return "";
		}
		try
		{
			#if NETFX_CORE
			byte[] mod = Convert.FromBase64String(modulus);
			byte[] exp = Convert.FromBase64String(exponent);
			
			AsnKeyBuilder.AsnMessage asn1key = AsnKeyBuilder.PublicKeyToX509(mod, exp);
			
			IBuffer keyBuffer = CryptographicBuffer.CreateFromByteArray(asn1key.GetBytes());
			
			AsymmetricKeyAlgorithmProvider asym = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaOaepSha1);
			CryptographicKey key = asym.ImportPublicKey(keyBuffer, CryptographicPublicKeyBlobType.X509SubjectPublicKeyInfo);
			
			IBuffer plainBuffer = CryptographicBuffer.ConvertStringToBinary(toBeEncrypted, BinaryStringEncoding.Utf8);
			IBuffer encryptedBuffer = CryptographicEngine.Encrypt(key, plainBuffer, null);
			
			byte[] encryptedData;
			CryptographicBuffer.CopyToByteArray(encryptedBuffer, out encryptedData);
			#else
			// Create a new RSA parameter to configure RSA provider
			RSAParameters RSAParams = new RSAParameters();
			// Set information to fill the modulus and exponent received from the server at the OpenSSLSessionToGetPublicKey state
			RSAParams.Modulus = Convert.FromBase64String(modulus);
			RSAParams.Exponent = Convert.FromBase64String(exponent);
			
			// Create our provider and configure it
			RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
			RSA.ImportParameters(RSAParams);
			
			// Encrypt data
			// First the data must be convert in byte[] -> calculation of encryption is made on numbers
			// You can read RSA documentation to know how, no need for me to explain it here, it's transparent
			// Here the encryption is made on the bytes values
			// We use OAEP to ensure maximum security (the 'true' parameter in encrypt method)
			byte[] encryptedData = RSA.Encrypt(Encoding.UTF8.GetBytes(toBeEncrypted),true);
			#endif           
			// Last thing to do is to send the encrypted data to PHP server
			// But problem is : How PHP will be able to read our byte array ?
			// We can solve this problem easily with the Base64 strings since they are handled the same way on C# and PHP
			// We just need to convert our byte[] into ToBase64String, you can see it as a serialization technique : we format data to send it
			// (In PHP, to get the original data we just use : 'base64_decode($dataReceived)' --->>> THIS IS ABSOLUTELY NECESSARY !!)
			string toSendToServer = Convert.ToBase64String(encryptedData);
			return toSendToServer;
		}
		//Catch and display a CryptographicException
		catch(CryptographicException e)
		{
			Debug.Log(e.Message);
		}
		return "";
	}
	
	/// <summary>
	/// This function is useful to generate AES keys everytime the client wants new pair of keys
	/// It uses the blockSize (member of the Utils class
	/// </summary>
	public static void generateAESkeyAndIV(ref string AES_Key, ref string AES_IV)
	{
		// We simply create a Rijndael component and use it (after setting the IV size -> 256)
		// We get the key pair generated
		using (RijndaelManaged myRijndael = new RijndaelManaged())
		{
			myRijndael.BlockSize = blockSize;
			AES_Key = Convert.ToBase64String(myRijndael.Key);
			AES_IV = Convert.ToBase64String(myRijndael.IV);
		}
		if(AES_Key.Equals("") || AES_IV.Equals(""))
		{
			Debug.LogError("Security information problem:\n" + "AES_Key = [" + AES_Key + "]" + "\n\nAES_IV = [" + AES_IV + "]");
		}
	}
	
	/// <summary>
	/// Encrypt the parameter (string) with AES keys.
	/// The option can be either None or Hash:
	/// - None: the parameter is encrypted as is, in plain text.
	/// - Hash: the parameter is hashed with SHA256
	/// This method uses keySize and blockSize: member of the Utils class
	/// </summary>
	public static string AES_encrypt(String input, EncryptOptions option = EncryptOptions.None)
	{
		// Encrypt with sha256 if the option is selected
		if (option == EncryptOptions.Hash) { input = hash(input); }
		
		// It's generical code, we won't comment it (not quite useful to change it really)
		var aes = new RijndaelManaged();
		aes.KeySize = keySize;
		aes.BlockSize = blockSize;
		aes.Padding = PaddingMode.PKCS7;
		aes.Key = Convert.FromBase64String(UserSession.AES_Key);
		aes.IV = Convert.FromBase64String(UserSession.AES_IV);
		
		var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
		byte[] xBuff = null;
		using (var ms = new MemoryStream())
		{
			using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
			{
				byte[] xXml = Encoding.UTF8.GetBytes(input+"<ENC_END>");
				cs.Write(xXml, 0, xXml.Length);
			}
			xBuff = ms.ToArray();
		}
		String Output = Convert.ToBase64String(xBuff);
		return Output;
	}
	
	/// <summary>
	/// The decrypt function uses the AES key pair of the session.
	/// This method uses keySize and blockSize: member of the Utils class
	/// </summary>
	public static String AES_decrypt(String input)
	{
		if(input.Length == 0)
		{
			Debug.LogError("The string to decrypt in AES is empty");
			return "";
		}
		
		// It's generical code, we won't comment it (not quite useful to change it really)
		RijndaelManaged aes = new RijndaelManaged();
		aes.KeySize = keySize;
		aes.BlockSize = blockSize;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		aes.Key = Convert.FromBase64String(UserSession.AES_Key);
		aes.IV = Convert.FromBase64String(UserSession.AES_IV);
		
		var decrypt = aes.CreateDecryptor();
		byte[] xBuff = null;
		using (var ms = new MemoryStream())
		{
			using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
			{
				//Debug.Log ("You are trying to decrypt the string  ->"+input+"<- (if it's empty or white space it won't work)");
				byte[] xXml = Convert.FromBase64String(input);
				cs.Write(xXml, 0, xXml.Length);
			}
			xBuff = ms.ToArray();
		}
		String Output = Encoding.UTF8.GetString(xBuff, 0, xBuff.Length);
		return Output.Replace("<ENC_END>", "");
	}
	
	/// <summary>
	/// Hash the parameter with SHA256 and return it
	/// </summary>
	public static string hash(string input)
	{
		if(input == "") { return ""; } // If the string we have to hash is empty -> return an empty string
		
		string hash = "";
		using (SHA256 shaHash = SHA256Managed.Create())
		{
			hash = GetShaHash(shaHash, input);
		}
		return hash;
	}
	private static string GetShaHash(SHA256 shaHash, string input)
	{
		// Convert the input string to a byte array and compute the hash.
		byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));
		
		// Create a new Stringbuilder to collect the bytes
		// and create a string.
		StringBuilder sBuilder = new StringBuilder();
		
		// Loop through each byte of the hashed data 
		// and format each one as a hexadecimal string.
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}
		
		// Return the hexadecimal string.
		return sBuilder.ToString();
	}
	
	/// <summary>
	/// Generation of a random string (with characters took in the member 'stringCharacters' of the Utils class
	/// The size is the number of character the resulting string will be
	/// </summary>
	public static string randomString(int size)
	{
		int stringCharactersLength = stringCharacters.Length;
		string s = string.Empty;
		for(int i=0; i<size; ++i)
		{
			s += stringCharacters[UnityEngine.Random.Range(0, stringCharactersLength)];
		}
		return s;
	}
	/// <summary>
	/// Generates and returns a random session 'secret key' using the member 'tokenSize' of the Utils class
	/// </summary>
	public static void generateToken(ref string token)
	{
		token = UtilsProSecure.randomString(tokenSize);
	}
	
	
	
	
	///////////////////////////////////////////////////////////////////////////////////////////
	/// FILES and CONVERSION
	///////////////////////////////////////////////////////////////////////////////////////////
	
	/// <summary>
	/// Read a file at 'completeFilePath' and return a byte array out of it
	/// </summary>
	public static byte[] readFile(string completeFilePath)
	{
		#if UNITY_WEBPLAYER
		Debug.LogError("No possible file generation while in webplayer plateform, please switch to another plateform in the build settings.");
		return null;
		#else
		return File.ReadAllBytes(completeFilePath);
		#endif
	}
	
	/// <summary>
	/// Write all bytes specified as parameter in the file at 'completeFilePath'
	/// </summary>
	public static void writeBytesInFile(string completeFilePath, byte[] bytes)
	{
		#if UNITY_WEBPLAYER
		Debug.LogError("No possible file generation while in webplayer plateform, please switch to another plateform in the build settings.");
		#else
		File.WriteAllBytes(completeFilePath, bytes);
		#endif
	}
	
	/// <summary>
	/// Read a file at 'completeFilePath' and return the full string out of it
	/// </summary>
	public static string readTextFile(string completeFilePath)
	{
		#if UNITY_WEBPLAYER
		Debug.LogError("No possible file generation while in webplayer plateform, please switch to another plateform in the build settings.");
		return "";
		#else
		return convertBytesToString(File.ReadAllBytes(completeFilePath));
		#endif
	}
	
	/// <summary>
	/// Write the whole string specified as parameter in the file at 'completeFilePath'
	/// </summary>
	public static void writeTextInFile(string completeFilePath, string text)
	{
		#if UNITY_WEBPLAYER
		Debug.LogError("No possible file generation while in webplayer plateform, please switch to another plateform in the build settings.");
		#else
		File.WriteAllBytes(completeFilePath, convertStringToBytes(text));
		#endif
	}
	
	/// <summary>
	/// Convert a byte array into a string and return the full string
	/// </summary>
	public static string convertBytesToString(byte[] bytes)
	{
		return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
	}
	
	/// <summary>
	/// Convert a string into a byte array and return the byte array
	/// </summary>
	public static byte[] convertStringToBytes(string s)
	{
		return Encoding.UTF8.GetBytes(s);
	}
	
	/// <summary>
	/// Decode a Base64 string into a string and return the string
	/// </summary>
	public static string From64String(string s)
	{
		return convertBytesToString(Convert.FromBase64String(s));
	}
	
	/// <summary>
	/// Encode a string into a Base64 string and return the Base64 string
	/// </summary>
	public static string To64String(string s)
	{
		return Convert.ToBase64String(convertStringToBytes(s));
	}
}
