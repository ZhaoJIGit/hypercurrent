using System.Web.Mvc;
using System.Web.Routing;

namespace JieNor.Megi.Main.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute("CheckToken", "FW/FWHome/CheckToken", new
			{
				controller = "Account",
				action = "Register",
				id = UrlParameter.Optional
			});
			routes.MapRoute("Default", "{controller}/{action}/{id}", new
			{
				controller = "Account",
				action = "Register",
				id = UrlParameter.Optional
			});
		}
	}
}
