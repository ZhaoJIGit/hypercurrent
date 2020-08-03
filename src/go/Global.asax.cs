using DevExpress.Web.Mvc;
using JieNor.Megi.Common.Compressor;
using JieNor.Megi.Common.Framework;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Go.Web.AutoManager;
using JieNor.Megi.Identity.Common;
using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JieNor.Megi.Go.Web
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			new MvcApplicationStart().Start();
			XmlConfigurator.Configure();
			ModelBinders.Binders.DefaultBinder = new MModelBinder();
			AutofacWCF.BuilderWcfService();
			Compressor.Compress(base.Server.MapPath("~/Scripts/"));
		}

		protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
		{
			DevExpressHelper.Theme = "DevEx";
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			MLogger.Log(base.Server.GetLastError());
			string text = "";
			string returnUrl = base.Request.Url.ToString();
			bool flag = CustomerErrorPageHelper.IsRedirectToErrorPage(base.Server, returnUrl, out text);
			if (base.Request != null && new HttpRequestWrapper(base.Request).IsAjaxRequest())
			{
				base.Response.ClearContent();
				base.Response.ContentType = "application/json; charset=utf-8";
				base.Response.Write(JsonConvert.SerializeObject(new
				{
					RequstExcetion = true,
					ErrorUrl = text
				}));
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

