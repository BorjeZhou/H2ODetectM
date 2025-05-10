using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GCProject.Main.Lib;

public class EncryptHelper
{
	public static string Get32MD5One(string source)
	{
		using MD5 md5Hash = MD5.Create();
		byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
		StringBuilder sBuilder = new StringBuilder();
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}
		return sBuilder.ToString().ToUpper();
	}

	public static string Get16MD5One(string source)
	{
		using MD5 md5Hash = MD5.Create();
		return BitConverter.ToString(md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source)), 4, 8).Replace("-", "").ToString()
			.ToUpper();
	}

	public static string Get32MD5Two(string source)
	{
		return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(source))).Replace("-", "").ToUpper();
	}

	public static string Get16MD5Two(string source)
	{
		return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToUpper();
	}

	public static string SHA1Encrypt(string normalTxt)
	{
		byte[] bytes = Encoding.Default.GetBytes(normalTxt);
		return Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(bytes));
	}

	public static string SHA256Encrypt(string normalTxt)
	{
		byte[] bytes = Encoding.Default.GetBytes(normalTxt);
		return Convert.ToBase64String(new SHA256CryptoServiceProvider().ComputeHash(bytes));
	}

	public static string SHA384Encrypt(string normalTxt)
	{
		byte[] bytes = Encoding.Default.GetBytes(normalTxt);
		return Convert.ToBase64String(new SHA384CryptoServiceProvider().ComputeHash(bytes));
	}

	public string SHA512Encrypt(string normalTxt)
	{
		byte[] bytes = Encoding.Default.GetBytes(normalTxt);
		return Convert.ToBase64String(new SHA512CryptoServiceProvider().ComputeHash(bytes));
	}

	public static string Base64Decode(string content)
	{
		byte[] bytes = Convert.FromBase64String(content);
		return Encoding.UTF8.GetString(bytes);
	}

	public static string DESEncryption(string Text, string sKey = null)
	{
		sKey = sKey ?? "zhiqiang";
		try
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			byte[] inputByteArray = Encoding.Default.GetBytes(Text);
			string md5SKey = Get32MD5One(sKey).Substring(0, 8);
			des.Key = Encoding.ASCII.GetBytes(md5SKey);
			des.IV = Encoding.ASCII.GetBytes(md5SKey);
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
			cryptoStream.FlushFinalBlock();
			StringBuilder ret = new StringBuilder();
			byte[] array = memoryStream.ToArray();
			foreach (byte b in array)
			{
				ret.AppendFormat("{0:X2}", b);
			}
			return ret.ToString();
		}
		catch
		{
			return "error";
		}
	}

	public static string DESDecrypt(string Text, string sKey = null)
	{
		sKey = sKey ?? "zhiqiang";
		try
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			int len = Text.Length / 2;
			byte[] inputByteArray = new byte[len];
			for (int x = 0; x < len; x++)
			{
				int i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
				inputByteArray[x] = (byte)i;
			}
			string md5SKey = Get32MD5One(sKey).Substring(0, 8);
			des.Key = Encoding.ASCII.GetBytes(md5SKey);
			des.IV = Encoding.ASCII.GetBytes(md5SKey);
			MemoryStream ms = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
			cryptoStream.FlushFinalBlock();
			return Encoding.Default.GetString(ms.ToArray());
		}
		catch
		{
			return "error";
		}
	}

	public static string RSAEncryption(string express, string KeyContainerName = null)
	{
		using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(new CspParameters
		{
			KeyContainerName = (KeyContainerName ?? "zhiqiang")
		});
		byte[] plaindata = Encoding.Default.GetBytes(express);
		return Convert.ToBase64String(rsa.Encrypt(plaindata, fOAEP: false));
	}

	public static string RSADecrypt(string ciphertext, string KeyContainerName = null)
	{
		using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(new CspParameters
		{
			KeyContainerName = (KeyContainerName ?? "zhiqiang")
		});
		byte[] encryptdata = Convert.FromBase64String(ciphertext);
		byte[] decryptdata = rsa.Decrypt(encryptdata, fOAEP: false);
		return Encoding.Default.GetString(decryptdata);
	}
}
