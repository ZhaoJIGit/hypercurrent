using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace JieNor.Megi.Common.Utility
{
	public static class URLUtility
	{
		public static bool CheckHostInList(this string url, List<string> targetUrls)
		{
			url = url.RemoveHttpLabel().RemoveHttpsLabel();
			string[] tempDomainString = ServerHelper.IsBetaSite ? ServerHelper.BetaDomainString : ServerHelper.DomainString;
			string domainPrefix = (tempDomainString != null && tempDomainString.Length == 2) ? tempDomainString[0] : "";
			foreach (string targetUrl in targetUrls)
			{
				string tempTargetUrl3 = targetUrl.RemoveHttpLabel().RemoveHttpsLabel();
				tempTargetUrl3 = (string.IsNullOrWhiteSpace(domainPrefix) ? tempTargetUrl3.ToLower() : new Regex(domainPrefix).Replace(tempTargetUrl3.ToLower(), "", 1));
				tempTargetUrl3 = (tempTargetUrl3.Contains(":") ? tempTargetUrl3.Split(':')[0] : tempTargetUrl3);
				if (url.ToLower().Contains(tempTargetUrl3))
				{
					return true;
				}
			}
			return false;
		}

		public static string AddHttpLabel(this string url)
		{
			return "HTTP://" + url.RemoveHttpLabel();
		}

		public static string RemoveHttpLabel(this string url)
		{
			return url.ToUpper().Replace("HTTP://", string.Empty);
		}

		public static string AddHttpsLabel(this string url)
		{
			return "HTTPS://" + url.RemoveHttpLabel();
		}

		public static string RemoveHttpsLabel(this string url)
		{
			return url.ToUpper().Replace("HTTPS://", string.Empty);
		}

		public static string AddUrlParamemter(this string url, string key, string value)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add(key, value);
			return url.AddParameters2Url(dic);
		}

		public static string GetPureUri(this string url)
		{
			return url.Substring(0, (url.IndexOf('?') < 0) ? url.Length : url.IndexOf('?'));
		}

		public static string AddParameters2Url(this string url, Dictionary<string, string> dic)
		{
			Dictionary<string, string> urlDic = url.GetParameters();
			IEnumerable<KeyValuePair<string, string>> union = dic.Union(urlDic);
			if (dic != null && dic.Count > 0)
			{
				string paramStr = string.Empty;
				foreach (KeyValuePair<string, string> item in union)
				{
					paramStr = paramStr + item.Key + "=" + item.Value + "&";
				}
				url = url.GetPureUri() + "?" + paramStr.TrimEnd('&');
			}
			return url;
		}

		public static Dictionary<string, string> GetParameters(this string url)
		{
			string paramStr = url.Substring(url.IndexOf('?') + 1);
			string[] keyValueStrs = paramStr.Split('&');
			Dictionary<string, string> result = new Dictionary<string, string>();
			string[] array = keyValueStrs;
			foreach (string keyValueStr in array)
			{
				string[] keyValue = keyValueStr.Split('=');
				if (keyValue != null && keyValue.Length == 2)
				{
					result.Add(keyValue[0], keyValue[1]);
				}
			}
			return result;
		}

		public static string GetAbsoluteUri(this string url, HttpContext httpContext)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				return url;
			}
			url = $"{httpContext.Request.Url.Scheme}://{url.RemoveHttpLabel()}";
			return url;
		}
	}
}
