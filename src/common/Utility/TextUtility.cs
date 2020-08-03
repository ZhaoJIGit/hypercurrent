using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JieNor.Megi.Common.Utility
{
	public static class TextUtility
	{
		public static bool IsSimilar(this string a, string b)
		{
			if (a == null)
			{
				a = string.Empty;
			}
			if (b == null)
			{
				b = string.Empty;
			}
			a = a.ToLower();
			b = b.ToLower();
			if (a.Equals(b))
			{
				return true;
			}
			StringCompute sc = new StringCompute();
			sc.SpeedyCompute(a, b);
			decimal rate = sc.ComputeResult.Rate;
			return rate > new decimal(0.7);
		}

		public static bool EqualsIgnoreCase(this string a, string b)
		{
			if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
			{
				return true;
			}
			if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
			{
				return false;
			}
			string str = a.Trim();
			string str2 = b.Trim();
			return string.Equals(str, str2, StringComparison.OrdinalIgnoreCase);
		}

		public static bool EqualsIgnoreCase(this string a, params string[] b)
		{
			if (b == null || b.Length == 0)
			{
				return false;
			}
			foreach (string s in b)
			{
				if (string.Equals(a, s, StringComparison.CurrentCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public static string CreateRandomCode(int codeCount = 4)
		{
			string randomCode = string.Empty;
			Random random = new Random();
			for (int i = 0; i < codeCount; i++)
			{
				int number = random.Next(100);
				char c;
				switch (number % 3)
				{
				case 0:
				{
					string str3 = randomCode;
					c = (char)(48 + (ushort)(number % 10));
					randomCode = str3 + c.ToString();
					break;
				}
				case 1:
				{
					string str2 = randomCode;
					c = (char)(97 + (ushort)(number % 26));
					randomCode = str2 + c.ToString();
					break;
				}
				case 2:
				{
					string str = randomCode;
					c = (char)(65 + (ushort)(number % 26));
					randomCode = str + c.ToString();
					break;
				}
				}
			}
			return randomCode;
		}

		public static int GetBitLength(this string str)
		{
			if (str.Equals(string.Empty))
			{
				return 0;
			}
			int strlen = 0;
			ASCIIEncoding strData = new ASCIIEncoding();
			byte[] strBytes = strData.GetBytes(str);
			for (int i = 0; i <= strBytes.Length - 1; i++)
			{
				if (strBytes[i] == 63)
				{
					strlen += 2;
				}
				strlen++;
			}
			return strlen;
		}

		public static bool IsEmpty(this string source)
		{
			return source == null || source.Trim().Length < 1;
		}

		public static string ToEncrypt(this string pToEncrypt, string sKey)
		{
			DESCryptoServiceProvider Des = new DESCryptoServiceProvider();
			Des.Key = Encoding.ASCII.GetBytes(sKey);
			Des.IV = Encoding.ASCII.GetBytes(sKey);
			byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, Des.CreateEncryptor(), CryptoStreamMode.Write);
			cs.Write(inputByteArray, 0, inputByteArray.Length);
			cs.FlushFinalBlock();
			StringBuilder ret = new StringBuilder();
			byte[] array = ms.ToArray();
			foreach (byte b in array)
			{
				ret.AppendFormat("{0:X2}", b);
			}
			ret.ToString();
			return ret.ToString();
		}

		public static string ToDecrypt(this string pToDecrypt, string sKey)
		{
			DESCryptoServiceProvider Des = new DESCryptoServiceProvider();
			byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
			for (int x = 0; x < pToDecrypt.Length / 2; x++)
			{
				int i = Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16);
				inputByteArray[x] = (byte)i;
			}
			Des.Key = Encoding.ASCII.GetBytes(sKey);
			Des.IV = Encoding.ASCII.GetBytes(sKey);
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, Des.CreateDecryptor(), CryptoStreamMode.Write);
			cs.Write(inputByteArray, 0, inputByteArray.Length);
			cs.FlushFinalBlock();
			return Encoding.Default.GetString(ms.ToArray());
		}

		public static string SubStr(this string source, int len, bool isSuffix = false)
		{
			if (source.IsEmpty())
			{
				return string.Empty;
			}
			if (source.Length < len)
			{
				return source;
			}
			return source.Substring(0, len) + (isSuffix ? "..." : "");
		}

		public static int ToOrSum(this string str)
		{
			if (str.IsEmpty())
			{
				return 0;
			}
			IList<int> list = (from p in str.Split(',')
			select p.ToInt32()).ToList();
			int count = 0;
			foreach (int item in list)
			{
				count |= item;
			}
			return count;
		}

		public static string EmptyToNone(this string str, string valueFormat = null)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return "<span class='m-empty-data'>None</span>";
			}
			if (valueFormat != null)
			{
				return string.Format(valueFormat, str);
			}
			return str;
		}

		public static string ToFixLengthString(this string str, int count, string p)
		{
			for (int i = str.Length; i < count; i++)
			{
				str = p + str;
			}
			return str;
		}

		public static int GetYearByYearPeriod(this int yearPeriod)
		{
			if (yearPeriod < 190000)
			{
				return 0;
			}
			return yearPeriod / 100;
		}

		public static int GetPeriodByYearPeriod(this int yearPeriod)
		{
			if (yearPeriod < 190000)
			{
				return 0;
			}
			return yearPeriod % 100;
		}
	}
}
