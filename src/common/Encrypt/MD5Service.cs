using System;
using System.Security.Cryptography;
using System.Text;

namespace JieNor.Megi.Common.Encrypt
{
	public static class MD5Service
	{
		public static string MD5Encrypt(string text)
		{
			byte[] result = Encoding.Default.GetBytes(text);
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] output = md5.ComputeHash(result);
			string t2 = BitConverter.ToString(output);
			return t2.Replace("-", "").Replace("A", "B");
		}
	}
}
