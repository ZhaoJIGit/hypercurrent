using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JieNor.Megi.Common.Utility
{
	public static class ConvertUtility
	{
		public static byte ToByte(this string input)
		{
			return input.ToByte(0);
		}

		public static byte ToByte(this string input, byte defaultValue)
		{
			if (byte.TryParse(input, out byte iReturn))
			{
				return iReturn;
			}
			return defaultValue;
		}

		public static short ToInt16(this string input)
		{
			return input.ToInt16(0);
		}

		public static short ToInt16(this string input, short defaultValue)
		{
			if (short.TryParse(input, out short iReturn))
			{
				return iReturn;
			}
			return defaultValue;
		}

		public static int ToInt32(this string input)
		{
			return input.ToInt32(0);
		}

		public static int ToInt32(this string input, int defaultValue)
		{
			if (int.TryParse(input, out int iReturn))
			{
				return iReturn;
			}
			return defaultValue;
		}

		public static long ToInt64(this string input)
		{
			return input.ToInt64(0L);
		}

		public static long ToInt64(this string input, long defaultValue)
		{
			if (long.TryParse(input, out long iReturn))
			{
				return iReturn;
			}
			return defaultValue;
		}

		public static decimal ToDecimal(this string input)
		{
			return input.ToDecimal(decimal.Zero);
		}

		public static decimal ToDecimal(this string input, int decimals)
		{
			return decimal.Round(input.ToDecimal(decimal.Zero), decimals);
		}

		public static decimal ToDecimal(this string input, decimal defaultValue)
		{
			if (decimal.TryParse(input, out decimal iReturn))
			{
				return iReturn;
			}
			if (RegExp.IsScientificCountFormat(input))
			{
				return decimal.Parse(input, NumberStyles.Float);
			}
			return defaultValue;
		}

		public static string ToMoneyFormat(this object obj)
		{
			if (decimal.TryParse(Convert.ToString(obj), out decimal d))
			{
				return d.ToString("N");
			}
			return Convert.ToString(obj);
		}

		public static float ToSingle(this string input)
		{
			return input.ToSingle(0f);
		}

		public static float ToSingle(this string input, float defaultValue)
		{
			if (float.TryParse(input, out float iReturn))
			{
				return iReturn;
			}
			return defaultValue;
		}

		public static bool ToBoolean(this string input)
		{
			return input.ToBoolean(false);
		}

		public static bool ToBoolean(this string input, bool defaultValue)
		{
			if (bool.TryParse(input, out bool iReturn))
			{
				return iReturn;
			}
			return false;
		}

		public static DateTime ToDateTime(this string input)
		{
			return input.ToDateTime(new DateTime(1900, 1, 1));
		}

		public static DateTime ToDateTime(this string input, DateTime defaultValue)
		{
			if (DateTime.TryParse(input, out DateTime iReturn))
			{
				return iReturn;
			}
			return defaultValue;
		}

		public static string ToDateString(this DateTime? dt)
		{
			if (!dt.HasValue)
			{
				return string.Empty;
			}
			return dt.Value.ToString("yyyy-MM-dd");
		}

		public static int[] StrArrayConvertIntArray(string[] arrstr)
		{
			if (arrstr == null || arrstr.Length == 0)
			{
				return new int[0];
			}
			int[] ia = new int[arrstr.Length];
			for (int i = 0; i < arrstr.Length; i++)
			{
				ia[i] = int.Parse(arrstr[i]);
			}
			return ia;
		}

		public static string ToUrl(string Name, string Url)
		{
			return "<a href='" + Url + "'>" + Name + "</a>";
		}

		public static DateTime ToUTCTime(this DateTime dt)
		{
			//bool flag = false;
			if (dt.Kind == DateTimeKind.Utc)
			{
				return dt;
			}
			return dt.ToUniversalTime();
		}

		public static DateTime ToLocalTime(this DateTime utcDateTime, string formatString = "yyyy-MM-dd HH:mm:ss", string cultureString = "zh-CN")
		{
			//bool flag = false;
			return DateTime.ParseExact(utcDateTime.ToString(), formatString, new CultureInfo(cultureString), DateTimeStyles.AssumeUniversal);
		}

		public static string ToDateString(this string str)
		{
			string result = string.Empty;
			if (DateTime.TryParse(str, out DateTime dt))
			{
				result = dt.ToShortDateString();
			}
			else
			{
				string[] arrStr = str.Split(' ');
				if ((arrStr.Length == 2 && arrStr[0].Length == 8) || (str.Length == 8 && str.IndexOf(':') == -1))
				{
					result = $"{str.Substring(0, 4)}/{str.Substring(4, 2)}/{str.Substring(6, 2)}";
				}
			}
			return result;
		}

		public static string ToTimeString(this string str)
		{
			string result2 = string.Empty;
			if (string.IsNullOrWhiteSpace(str))
			{
				return result2;
			}
			if (DateTime.TryParse(str, out DateTime dt))
			{
				result2 = dt.ToString("HH:mm:ss");
			}
			else
			{
				string[] arrStr = str.Split(' ');
				if (arrStr.Length == 2)
				{
					str = arrStr[1];
				}
				str = str.FilterNonNumericChar().PadLeft(6, '0');
				result2 = $"{str.Substring(0, 2)}:{str.Substring(2, 2)}:{str.Substring(4, 2)}";
			}
			return result2;
		}

		public static string ShowCountUnit(this int count, string unit)
		{
			return $"{count} {((count > 1) ? unit : unit.TrimEnd('s'))}";
		}

		public static string ZeroToEmpty(this object obj)
		{
			string result = string.Empty;
			string str = Convert.ToString(obj);
			if (!string.IsNullOrWhiteSpace(str) && Convert.ToDecimal(str.Replace(",", "")) != decimal.Zero)
			{
				result = str;
			}
			return result;
		}

		public static decimal? ZeroToNull(this object obj)
		{
			decimal? result = null;
			decimal tmp = default(decimal);
			string str = Convert.ToString(obj);
			if (!string.IsNullOrWhiteSpace(str) && decimal.TryParse(str.Replace(",", ""), out tmp) && tmp != decimal.Zero)
			{
				result = tmp;
			}
			return result;
		}

		public static string MoneyToChinese(this object obj)
		{
			decimal decVal = Convert.ToDecimal(obj);
			string s = decVal.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
			string d = Regex.Replace(s, "((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\\.]|$))))", "${b}${z}");
			string result = Regex.Replace(d, ".", (Match m) => "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万亿兆京垓秭穰"[m.Value[0] - 45].ToString());
			string decAvailableVal = decVal.ToString("G29");
			if (!decAvailableVal.Contains("."))
			{
				result += "整";
			}
			return result;
		}

		public static string AppendColon(this object obj)
		{
			string str = Convert.ToString(obj);
			if (!string.IsNullOrWhiteSpace(str))
			{
				return $"{str}：";
			}
			return string.Empty;
		}

		public static string ToStandardNo(this object obj, int digit = 3)
		{
			string str = Convert.ToString(obj);
			if (!string.IsNullOrWhiteSpace(str) && str.Length != digit)
			{
				return str.TrimStart('0').PadLeft(3, '0');
			}
			return str;
		}

		public static string ToStandardAcctNumber(this string number, bool appendDot = false, int childrenStepLen = 2)
		{
			if (string.IsNullOrWhiteSpace(number))
			{
				return string.Empty;
			}
			number = number.Trim();
			if (appendDot)
			{
				int acctNoLen = number.Length;
				if (number.IndexOf('.') == -1 && acctNoLen > 4)
				{
					number = number.FilterNonNumericChar();
					if (number.Length < 4)
					{
						return string.Empty;
					}
					List<string> acctSecList = new List<string>();
					acctSecList.Add(number.Substring(0, 4));
					for (int i = 4; i < acctNoLen; i += childrenStepLen)
					{
						int curSubNo = acctNoLen - i;
						if (curSubNo < childrenStepLen && curSubNo > 0)
						{
							acctSecList.Add(number.Substring(4, curSubNo).PadLeft(childrenStepLen, '0'));
						}
						else if (i <= acctNoLen - childrenStepLen)
						{
							acctSecList.Add(number.Substring(i, childrenStepLen));
						}
					}
					return string.Join(".", acctSecList);
				}
			}
			return number;
		}

		public static string ToStandardPrice(this object str, int minDecimalPlace = 2)
		{
			string result = Convert.ToDecimal(str).ToString("G29");
			string[] arr = result.Split('.');
			int totalPlace = minDecimalPlace;
			if (arr.Length == 2 && arr[1].Length > minDecimalPlace)
			{
				totalPlace = arr[1].Length;
			}
			return Convert.ToDecimal(result).ToString($"n{totalPlace}");
		}

		public static string ToPercentFormat(this object str)
		{
			decimal amt = Convert.ToDecimal(str);
			if (amt == decimal.Zero)
			{
				return string.Empty;
			}
			return amt.ToString("G29") + "%";
		}

		public static string GetValidAccountNo(this string str)
		{
			if (str == null)
			{
				return string.Empty;
			}
			return Regex.Match(str.Replace(".", "").Trim(), "^[\\d]+").Value;
		}

		public static string ToEllipsisString(this string str, int limitLength, bool showTitle = true)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}
			int bytesCount = Encoding.GetEncoding("gb2312").GetByteCount(str);
			if (bytesCount > limitLength)
			{
				int readyLength = 0;
				for (int i = 0; i < str.Length; i++)
				{
					int byteLength = Encoding.GetEncoding("gb2312").GetByteCount(new char[1]
					{
						str[i]
					});
					readyLength += byteLength;
					if (readyLength > limitLength)
					{
						if (showTitle)
						{
							return string.Format("<label title='{0}'>{1}</label>", str, str.Substring(0, i) + "...");
						}
						return str.Substring(0, i) + "...";
					}
				}
			}
			return str;
		}

		public static string FilterInvalidFileName(this string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return string.Empty;
			}
			char[] involidChars = Path.GetInvalidFileNameChars();
			return string.Join("", fileName.Split(involidChars));
		}

		public static string FilterInvalidSheetName(this string str, int maxLength = 28)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}
			char[] sheetLimitChars = new char[10]
			{
				'：',
				'？',
				'[',
				']',
				'\'',
				'＇',
				'／',
				'［',
				'］',
				'＼'
			};
			char[] involidChars = Path.GetInvalidFileNameChars().Concat(sheetLimitChars).ToArray();
			return string.Join("", str.Split(involidChars)).ToEllipsisString(maxLength, false);
		}

		public static string ReplaceMultiSpaceToSingle(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}
			RegexOptions options = RegexOptions.None;
			Regex regex = new Regex("[ ]{2,}", options);
			return regex.Replace(str.Trim(), " ");
		}

		public static string ToDateString(this DateTime dateTime, string formatter = null)
		{
			formatter = (string.IsNullOrWhiteSpace(formatter) ? "yyyy-MM-dd" : formatter);
			return dateTime.ToString(formatter);
		}

		public static string ToDateTimeString(this DateTime dateTime, string formatter = null)
		{
			formatter = (string.IsNullOrWhiteSpace(formatter) ? "yyyy-MM-dd HH:mm;ss" : formatter);
			return dateTime.ToString(formatter);
		}

		public static string FilterNonNumericChar(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}
			return Regex.Replace(str, "[^\\d]", "");
		}
	}
}
