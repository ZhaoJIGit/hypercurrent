using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace PasswordDecrypt
{
    class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine(DESEncrypt.Decrypt("86A41E1E1BF2DBC61E6FEFD9C386FD8487CBF66F8A46A4E5BFA764F260B392C12D262B54E4E28DBA3F7B51C6FF7A638E40580AA605FC403486C271601B0BD316384B1A2103C6B835F196A2E64D4B24F65DF52B446CDCDA3043B3042C94DC35B2F68CF8D8B3A66D54F9EAC611AEBD6A9B5CD2B789BC6CCC5B180220A45D58B98EF627943AD0BE739608FF2A140A180B747E93DE5ADEA0AEAF"));
			Console.ReadKey();

		}
		public class DESEncrypt
		{
			public static string Encrypt(string Text)
			{
				return Encrypt(Text, "JieNor-001");
			}

			public static string Encrypt(string Text, string sKey)
			{
				if (string.IsNullOrEmpty(sKey))
				{
					return string.Empty;
				}
				DESCryptoServiceProvider des = new DESCryptoServiceProvider();
				byte[] inputByteArray = Encoding.Default.GetBytes(Text);
				des.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
				des.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
				cs.Write(inputByteArray, 0, inputByteArray.Length);
				cs.FlushFinalBlock();
				StringBuilder ret = new StringBuilder();
				byte[] array = ms.ToArray();
				foreach (byte b in array)
				{
					ret.AppendFormat("{0:X2}", b);
				}
				return ret.ToString();
			}

			public static string Decrypt(string Text)
			{
				return Decrypt(Text, "JieNor-001");
			}

			public static string Decrypt(string Text, string sKey)
			{
				if (string.IsNullOrEmpty(sKey))
				{
					return string.Empty;
				}
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
					des.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
					des.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
					MemoryStream ms = new MemoryStream();
					CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
					cs.Write(inputByteArray, 0, inputByteArray.Length);
					cs.FlushFinalBlock();
					return Encoding.Default.GetString(ms.ToArray());
				}
				catch
				{
					return Text;
				}
			}
		}
	}
}
