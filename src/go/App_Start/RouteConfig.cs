using System.Web.Mvc;
using System.Web.Routing;

namespace JieNor.Megi.Go.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute("IDNumberTitle", "{controller}/{action}/{id}/{number}/{title}", new
			{
				controller = "Home",
				action = "Index",
				id = UrlParameter.Optional,
				number = UrlParameter.Optional,
				title = UrlParameter.Optional
			});
			routes.MapRoute("IDNumber", "{controller}/{action}/{id}/{number}", new
			{
				controller = "Home",
				action = "Index",
				id = UrlParameter.Optional,
				number = UrlParameter.Optional
			});
			routes.MapRoute("Default", "{controller}/{action}/{id}", new
			{
				controller = "Home",
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
