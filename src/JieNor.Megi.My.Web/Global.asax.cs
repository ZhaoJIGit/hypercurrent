using JieNor.Megi.Common.Compressor;
using JieNor.Megi.Identity.Common;
using JieNor.Megi.My.Web.App_Start;
using JieNor.Megi.My.Web.AutoManager;
using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace JieNor.Megi.My.Web
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new MyViewEngine());
			XmlConfigurator.Configure();
			AutofacWCF.BuilderWcfService();
			Compressor.Compress(base.Server.MapPath("~/"));
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			string text = "";
			string returnUrl = base.Request.Url.ToString();
			bool flag = CustomerErrorPageHelper.IsRedirectToErrorPage(base.Server, returnUrl, out text);
			if (base.Request != null && new HttpRequestWrapper(base.Request).IsAjaxRequest())
			{
				base.Response.Clear();
				base.Response.ContentType = "application/json; charset=utf-8";
				base.Response.Write(JsonConvert.SerializeObject(new
				{
					RequstExcetion = true,
					ErrorUrl = text
				}));
				base.Response.Flush();
				base.Server.ClearError();
			}
			else if (flag)
			{
				base.Server.ClearError();
				base.Response.Redirect(text);
			}
		}
	}
}
