using System;
using System.Web;

namespace JieNor.Megi.Common.Utility
{
	public static class HtmlHelper
	{
		public static string Encode(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			return HttpUtility.HtmlEncode(s);
		}

		public static string Decode(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			return HttpUtility.HtmlDecode(s);
		}

		public static string RemoveLineBreaks(this object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			return Convert.ToString(obj).Trim().Replace("\r", "")
				.Replace("\n", "");
		}
	}
}
