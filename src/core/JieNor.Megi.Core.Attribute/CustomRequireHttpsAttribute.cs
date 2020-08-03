using System;
using System.Web.Mvc;

namespace JieNor.Megi.Core.Attribute
{
	public class CustomRequireHttpsAttribute : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			bool flag = !filterContext.HttpContext.Request.IsSecureConnection;
			if (flag)
			{
				string path = filterContext.HttpContext.Request.Path;
				string host = filterContext.HttpContext.Request.Url.Host;
				filterContext.HttpContext.Response.Redirect(string.Format("https://{0}{1}", host, path));
			}
			string absoluteUri = filterContext.HttpContext.Request.Url.AbsoluteUri;
		}
	}
}
