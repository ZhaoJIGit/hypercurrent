using System.Collections.Generic;
using System.Web;

namespace JieNor.Megi.Common.Utility
{
	public class Fetch
	{
		public static string CurrentUrl => HttpContext.Current.Request.Url.ToString();

		public static string ServerDomain
		{
			get
			{
				string urlHost = HttpContext.Current.Request.Url.Host.ToLower();
				string[] urlHostArray = urlHost.Split('.');
				if (urlHostArray.Length < 3 || RegExp.IsIp(urlHost))
				{
					return urlHost;
				}
				string urlHost2 = urlHost.Remove(0, urlHost.IndexOf(".") + 1);
				if (urlHost2.StartsWith("com.") || urlHost2.StartsWith("net.") || urlHost2.StartsWith("org.") || urlHost2.StartsWith("gov."))
				{
					return urlHost;
				}
				return urlHost2;
			}
		}

		public static string UserIp
		{
			get
			{
				string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
				string text = result;
				if (text == null || (text != null && text.Length == 0))
				{
					result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
				}
				if (!RegExp.IsIp(result))
				{
					return "Unknown";
				}
				return result;
			}
		}

		public static string Get(string name)
		{
			string text = HttpContext.Current.Request.QueryString[name];
			return (text == null) ? "" : text.Trim();
		}

		public static string Post(string name)
		{
			string text = HttpContext.Current.Request.Form[name];
			return (text == null) ? "" : text.Trim();
		}

		public static int GetQueryId(string name)
		{
			int id = 0;
			int.TryParse(Get(name), out id);
			return id;
		}

		public static int[] GetIds(string name)
		{
			string ids = Post(name);
			List<int> result = new List<int>();
			int id = 0;
			string[] array = ids.Split(',');
			string[] array2 = array;
			foreach (string a in array2)
			{
				if (int.TryParse(a.Trim(), out id))
				{
					result.Add(id);
				}
			}
			return result.ToArray();
		}

		public static int[] GetQueryIds(string name)
		{
			string ids = Get(name);
			List<int> result = new List<int>();
			int id = 0;
			string[] array = ids.Split(',');
			string[] array2 = array;
			foreach (string a in array2)
			{
				if (int.TryParse(a.Trim(), out id))
				{
					result.Add(id);
				}
			}
			return result.ToArray();
		}
	}
}
