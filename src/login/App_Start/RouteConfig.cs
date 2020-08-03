using System.Web.Mvc;
using System.Web.Routing;

namespace JieNor.Megi.Login.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute("CheckToken", "FW/FWHome/CheckToken", new
			{
				controller = "Account",
				action = "Index",
				id = UrlParameter.Optional
			});
			routes.MapRoute("IDNumberName", "{controller}/{action}/{id}/{number}/{name}", new
			{
				controller = "Account",
				action = "Index",
				id = UrlParameter.Optional,
				number = UrlParameter.Optional,
				name = UrlParameter.Optional
			});
			routes.MapRoute("IDNumber", "{controller}/{action}/{id}/{number}", new
			{
				controller = "Account",
				action = "Index",
				id = UrlParameter.Optional,
				number = UrlParameter.Optional
			});
			routes.MapRoute("Default", "{controller}/{action}/{id}", new
			{
				controller = "Account",
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
