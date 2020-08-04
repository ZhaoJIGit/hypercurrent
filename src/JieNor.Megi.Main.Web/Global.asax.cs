using JieNor.Megi.Common.Compressor;
using JieNor.Megi.Identity.Common;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace JieNor.Megi.Main.Web
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AutofacWCF.BuilderWcfService();
			Compressor.Compress(base.Server.MapPath("~/"));
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			string customErrorPage = "";
			string returnUrl = base.Request.Url.ToString();
			bool isRedirect = CustomerErrorPageHelper.IsRedirectToErrorPage(base.Server, returnUrl, out customErrorPage);
			if (base.Request != null && new HttpRequestWrapper(base.Request).IsAjaxRequest())
			{
				base.Response.ClearContent();
				base.Response.ContentType = "application/json; charset=utf-8";
				base.Response.Write(JsonConvert.SerializeObject(new
				{
					RequstExcetion = true,
					ErrorUrl = customErrorPage
				}));
				base.Server.ClearError();
			}
			else if (isRedirect)
			{
				base.Server.ClearError();
				base.Response.Redirect(customErrorPage);
			}
		}
	}
}