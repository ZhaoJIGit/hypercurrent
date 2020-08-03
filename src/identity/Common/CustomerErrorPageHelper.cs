using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using System;
using System.ServiceModel;
using System.Web;

namespace JieNor.Megi.Identity.Common
{
	public class CustomerErrorPageHelper
	{
		public static bool IsRedirectToErrorPage(HttpServerUtility server, string returnUrl, out string customerUri)
		{
			bool result = false;
			string str = "?from=" + HttpUtility.HtmlEncode(returnUrl);
			string text = string.Format(ServerHelper.StaticServer + ServerHelper.NotFoundPage) + str;
			string text2 = string.Format(ServerHelper.StaticServer + ServerHelper.ErrorPage) + str;
			customerUri = "";
			Exception lastError = server.GetLastError();
			if (lastError is HttpException)
			{
				string text3 = ((HttpException)lastError).GetHttpCode().ToString();
				if (text3.IndexOf("4") == 0)
				{
					customerUri = text;
					result = true;
				}
				else if (text3.IndexOf('5') == 0)
				{
					customerUri = text2;
					result = true;
				}
			}
			else if (lastError is EndpointNotFoundException)
			{
				customerUri = text2;
				result = true;
			}
			else if (lastError is ServerTooBusyException)
			{
				customerUri = text;
				result = true;
			}
			else
			{
				customerUri = text2;
				result = true;
			}
			MLogger.Log(returnUrl, lastError, null);
			return result;
		}
	}
}
