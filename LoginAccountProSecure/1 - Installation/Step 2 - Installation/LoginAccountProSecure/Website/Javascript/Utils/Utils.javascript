
function hexToAscii(hexData)
{
	var aInput = hexData.split(" ");
	var strWork = aInput.join("");
	aInput = strWork.split("\n");
	strWork = aInput.join("");
	aInput = strWork.split("%");
	strWork = aInput.join("");
	var l = strWork.length/2;
	var strBuf = "";
	var nBuf;
	var strOut = "";
	for (var i=0; i<l; ++i)
	{
		strBuf = strWork.substr(i*2, 1);
		strBuf += strWork.substr((i*2)+1, 1);
		strBuf = "0x"+strBuf;
		nBuf = Number(strBuf);
		if (nBuf == 0)
		{
			strOut += " ";
		}
		else
		{
			strOut += String.fromCharCode(nBuf);
		}
	}
	return strOut;
}


var Base64 = {
    _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",
    encode: function(input)
	{
        var output = "";
        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
        var i = 0;

        input = Base64._utf8_encode(input);
        while (i < input.length)
		{
            chr1 = input.charCodeAt(i++);
            chr2 = input.charCodeAt(i++);
            chr3 = input.charCodeAt(i++);

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2))
			{
                enc3 = enc4 = 64;
            }
			else if (isNaN(chr3))
			{
                enc4 = 64;
            }
            output = output + this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) + this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);
        }
        return output;
    },

    decode: function(input)
	{
        var output = "";
        var chr1, chr2, chr3;
        var enc1, enc2, enc3, enc4;
        var i = 0;

        input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
        while (i < input.length)
		{
            enc1 = this._keyStr.indexOf(input.charAt(i++));
            enc2 = this._keyStr.indexOf(input.charAt(i++));
            enc3 = this._keyStr.indexOf(input.charAt(i++));
            enc4 = this._keyStr.indexOf(input.charAt(i++));

            chr1 = (enc1 << 2) | (enc2 >> 4);
            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
            chr3 = ((enc3 & 3) << 6) | enc4;

            output = output + String.fromCharCode(chr1);
            if (enc3 != 64)
			{
                output = output + String.fromCharCode(chr2);
            }
            if (enc4 != 64)
			{
                output = output + String.fromCharCode(chr3);
            }
        }
        output = Base64._utf8_decode(output);
        return output;
    },

    _utf8_encode: function(string)
	{
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";

        for (var n = 0; n < string.length; n++)
		{
            var c = string.charCodeAt(n);
            if (c < 128)
			{
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048))
			{
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else
			{
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }
        }
        return utftext;
    },

    _utf8_decode: function(utftext)
	{
        var string = "";
        var i = 0;
        var c = c1 = c2 = 0;

        while (i < utftext.length)
		{
            c = utftext.charCodeAt(i);
            if (c < 128)
			{
                string += String.fromCharCode(c);
                i++;
            }
            else if ((c > 191) && (c < 224))
			{
                c2 = utftext.charCodeAt(i + 1);
                string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                i += 2;
            }
            else
			{
                c2 = utftext.charCodeAt(i + 1);
                c3 = utftext.charCodeAt(i + 2);
                string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                i += 3;
            }
        }
        return string;
    }
}

/*
 *	Function : escapeHtml
 *	Utility : transform any kind of interpreted character into escaped strings
 *	Parameters :
 *		-> text, the string to encode
 *	Return : The escaped string
 */
function escapeHtml(text)
{
  var map = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#039;'
  };
  return text.replace(/[&<>"']/g, function(m) { return map[m]; });
}   

/*
 *	Function : toBase64
 *	Utility : encode a string into a base64 string
 *	Parameters :
 *		-> data the string to encode
 *	Return : The encoded base64 string from the given string
 */
function toBase64(data)
{
	return Base64.encode(data);
}

/*
 *	Function : fromBase64
 *	Utility : decode a base64 string into string
 *	Parameters :
 *		-> data_base64 the base64 string to decode
 *	Return : The decoded string from the base64 string
 */
function fromBase64(data_base64)
{
	return Base64.decode(data_base64);
}

/*
 *	Function : generateAesKeys
 *	Utility : generate AES key and iv from a random salt and passphrase, and save AES keys in Javascript session array
 *	Parameters : none
 *	Return : The encrypted keys (key_base64 + '<AES_KEYS_SEPARATOR>' + iv_base64)
 */
function generateAesKeys()
{
	// Generate keys
	var salt = CryptoJS.lib.WordArray.random(128/8);
	var passphrase = CryptoJS.lib.WordArray.random(128/8);
	var key = CryptoJS.PBKDF2(passphrase, salt, { keySize: 128/32, iterations: 500 });
	var iv = CryptoJS.enc.Hex.parse(CryptoJS.lib.WordArray.random(128/8).toString());
	
	var key_base64 = key.toString(CryptoJS.enc.Base64);
	var iv_base64 = iv.toString(CryptoJS.enc.Base64);
	
	// Save AES keys in Javascript session array
	sessvars.SecuritySession.aes_key = key_base64;
	sessvars.SecuritySession.aes_iv = iv_base64;
	
	return true;
}

/*
 *	Function : md5
 *	Utility : encrypt data with MD5 algorithm
 *	Parameters :
 *		-> textToEncrypt : string
 *	Return : The MD5 encrypted data
 */
function md5(textToEncrypt)
{
	return CryptoJS.SHA256(textToEncrypt);
}

/*
 *	Function : rsa_encrypt
 *	Utility : encrypt data with RSA public key
 *	Parameters :
 *		-> textToEncrypt : string
 *	Return : The encrypted data in Base64 string
 */
function rsa_encrypt(textToEncrypt)
{
	var rsa = new RSAKey();
	var mod = b64tohex(sessvars.SecuritySession.rsa_modulus);
	var exp = b64tohex(sessvars.SecuritySession.rsa_exponent);
	rsa.setPublic(mod, exp);
	var res = rsa.encrypt(textToEncrypt);
	if(res)
	{
		return hex2b64(res);
	}
	alert('rsa_encrypt function failed.');
	return '';
}

/*
 *	Function : aes_encrypt
 *	Utility : encrypt data with AES keys in parameter
 *	Parameters :
 *		-> textToEncrypt : string
 *	Return : The encrypted data in Base64 string
 */
function aes_encrypt(textToEncrypt)
{
	// Split keys into key + IV
	var key = CryptoJS.enc.Base64.parse(sessvars.SecuritySession.aes_key);
	var iv = CryptoJS.enc.Base64.parse(sessvars.SecuritySession.aes_iv);
	
	var toBeEncrypted = textToEncrypt+'<ENC_END>';
	// Caution : A string with 48 characters can't be decrypted so we add a salt.
	if(toBeEncrypted.Length == 48)
	{
		toBeEncrypted += 'salt';
	}
	
	// Encrypt data
	var encrypted = CryptoJS.AES.encrypt(toBeEncrypted, key, { iv: iv });
	return encrypted.ciphertext.toString(CryptoJS.enc.Base64);
}

/*
 *	Function : aes_decrypt
 *	Utility : decrypt a string with keys in parameter
 *	Parameters :
 *		-> textToDecrypt : string
 *		-> keys : (key_base64 + '<AES_KEYS_SEPARATOR>' + iv_base64)
 *	Return : The decrypted data in string (NOT Base64)
 */
function aes_decrypt(textToDecrypt)
{
	// Split keys into key + IV
	var key = CryptoJS.enc.Base64.parse(sessvars.SecuritySession.aes_key);
	var iv = CryptoJS.enc.Base64.parse(sessvars.SecuritySession.aes_iv);
	
	// Decrypt data with AES keys in parameter
	var decrypted = CryptoJS.AES.decrypt(textToDecrypt, key, { iv: iv });
	// Decode result from HEX string to ASCII string, and cut off the additional suffix
	var data_decrypted = hexToAscii(decrypted.toString()).split('<ENC_END>');
	if(data_decrypted.length != 2)
	{
		return "aes_decrypt : textToDecrypt malformed.";
	}
	return data_decrypted[0];
}










