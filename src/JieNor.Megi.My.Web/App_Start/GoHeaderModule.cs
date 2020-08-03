using JieNor.Megi.Common.Utility;
using System;
using System.Web;

namespace JieNor.Megi.My.Web
{
	public class GoHeaderModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			context.PreSendRequestHeaders += ContextPreSendRequestHeaders;
		}

		public void Dispose()
		{
		}

		private void ContextPreSendRequestHeaders(object sender, EventArgs e)
		{
			string[] obj = new string[3]
			{
				"/Feed/FeedBack/FeedIndex",
				"/Setting/Account/AccountEdit",
				"/Setting/Profile/ProfileEdit"
			};
			string rawUrl = HttpContext.Current.Request.RawUrl;
			bool flag = false;
			string[] array = obj;
			foreach (string value in array)
			{
				if (rawUrl.IndexOf(value) > -1)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				HttpContext.Current.Response.Headers.Add("X-Frame-Options", ServerHelper.GoServer);
			}
			else
			{
				HttpContext.Current.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
			}
		}
	}
}
