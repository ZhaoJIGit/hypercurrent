using System;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Core.Attribute
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class JQueryFileDownloadAttribute : ActionFilterAttribute
	{
		public JQueryFileDownloadAttribute(string cookieName = "fileDownload", string cookiePath = "/")
		{
			this.CookieName = cookieName;
			this.CookiePath = cookiePath;
		}

		public string CookieName { get; set; }

		public string CookiePath { get; set; }

		private void CheckAndHandleFileResult(ActionExecutedContext filterContext)
		{
			HttpContextBase httpContext = filterContext.HttpContext;
			HttpResponseBase response = httpContext.Response;
			bool flag = filterContext.Result is FileResult;
			if (flag)
			{
				response.AppendCookie(new HttpCookie(this.CookieName, "true")
				{
					Path = this.CookiePath
				});
			}
			else
			{
				bool flag2 = httpContext.Request.Cookies[this.CookieName] != null;
				if (flag2)
				{
					response.AppendCookie(new HttpCookie(this.CookieName, "true")
					{
						Expires = DateTime.Now.AddYears(-1),
						Path = this.CookiePath
					});
				}
			}
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			this.CheckAndHandleFileResult(filterContext);
			base.OnActionExecuted(filterContext);
		}
	}
}
