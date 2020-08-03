using JieNor.Megi.Common.Utility;
using System;
using System.Web;

namespace JieNor.Megi.Go.Web
{
	public class SetupHeaderModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
		}

		public void Dispose()
		{
		}

		private void ContextPreSendRequestHeaders(object sender, EventArgs e)
		{
			string[] myServerSetupPages = ServerHelper.MyServerSetupPages;
			if (HttpContext.Current != null)
			{
				string text = HttpContext.Current.Request.RawUrl.ToLower();
				if (!text.Contains("/bd/"))
				{
					bool flag = false;
					string[] array = myServerSetupPages;
					foreach (string value in array)
					{
						if (text.IndexOf(value) > -1)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						HttpContext.Current.Response.Headers.Add("X-Frame-Options", ServerHelper.MyServer);
					}
					else
					{
						HttpContext.Current.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
					}
				}
			}
		}
	}
}
