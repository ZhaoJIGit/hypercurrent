using System.Web;

namespace JieNor.Megi.Common.Utility
{
	public class HttpHelper
	{
		public static string RemoteIP
		{
			get
			{
				HttpContext context = HttpContext.Current;
				if (context.Request.ServerVariables["HTTP_VIA"] != null)
				{
					return context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
				}
				return context.Request.ServerVariables["REMOTE_ADDR"].ToString();
			}
		}
	}
}
