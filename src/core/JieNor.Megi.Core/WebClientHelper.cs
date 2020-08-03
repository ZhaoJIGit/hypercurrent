using System.Web;

namespace JieNor.Megi.Core
{
	public class WebClientHelper
	{
		public static string GetIP()
		{
			string text = "127.0.0.1";
			try
			{
				if (HttpContext.Current == null || HttpContext.Current.Request == null || HttpContext.Current.Request.ServerVariables == null)
				{
					return text;
				}
				string value = HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
				if (!string.IsNullOrEmpty(value))
				{
					return text;
				}
				value = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
				if (!string.IsNullOrEmpty(value))
				{
					return text;
				}
				if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
				{
					value = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
					if (string.IsNullOrEmpty(value))
					{
						value = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
					}
				}
				else
				{
					value = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
				}
				if (string.Compare(value, "unknown", true) == 0)
				{
					value = HttpContext.Current.Request.UserHostAddress;
				}
				return string.IsNullOrEmpty(value) ? text : value;
			}
			catch
			{
			}
			return text;
		}
	}
}
