using System.Text.RegularExpressions;

namespace JieNor.Megi.Common.Utility
{
	public class RegExp
	{
		public static bool IsEmail(string s)
		{
			string text = "^[\\w-]+(\\.[\\w-]+)*@[\\w-]+(\\.[\\w-]+)+$";
			return Regex.IsMatch(s, text);
		}

		public static bool IsIp(string s)
		{
			string text = "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$";
			return Regex.IsMatch(s, text);
		}

		public static bool IsNumeric(string s)
		{
			string text = "^\\-?[0-9]+$";
			return Regex.IsMatch(s, text);
		}

		public static bool IsPhysicalPath(string s)
		{
			string text = "^\\s*[a-zA-Z]:.*$";
			return Regex.IsMatch(s, text);
		}

		public static bool IsRelativePath(string s)
		{
			if (s == null || s == "")
			{
				return false;
			}
			if (s.StartsWith("/") || s.StartsWith("?"))
			{
				return false;
			}
			if (Regex.IsMatch(s, "^\\s*[a-zA-Z]{1,10}:.*$"))
			{
				return false;
			}
			return true;
		}

		public static bool IsSafety(string s)
		{
			string text4 = s.Replace("%20", " ");
			text4 = Regex.Replace(text4, "\\s", " ");
			string text2 = "select |insert |delete from |count\\(|drop table|update |truncate |asc\\(|mid\\(|char\\(|xp_cmdshell|exec master|net localgroup administrators|:|net user|\"|\\'| or ";
			return !Regex.IsMatch(text4, text2, RegexOptions.IgnoreCase);
		}

		public static bool IsUnicode(string s)
		{
			string text = "^[\\u4E00-\\u9FA5\\uE815-\\uFA29]+$";
			return Regex.IsMatch(s, text);
		}

		public static bool IsUrl(string s)
		{
			string text = "^(http|https|ftp|rtsp|mms):(\\/\\/|\\\\\\\\)[A-Za-z0-9%\\-_@]+\\.[A-Za-z0-9%\\-_@]+[A-Za-z0-9\\.\\/=\\?%\\-&_~`@:\\+!;]*$";
			return Regex.IsMatch(s, text, RegexOptions.IgnoreCase);
		}

		public static bool IsInt(string s)
		{
			return Regex.IsMatch(s, "^\\d+$");
		}

		public static bool IsScientificCountFormat(string s)
		{
			string text = "^[+-]?[\\d]+([.][\\d]*)?([Ee][+-]?[\\d]+)?$";
			return Regex.IsMatch(s, text);
		}
	}
}
