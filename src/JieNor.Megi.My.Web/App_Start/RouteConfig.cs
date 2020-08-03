using System.Web.Mvc;
using System.Web.Routing;

namespace JieNor.Megi.My.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute("Default", "{controller}/{action}/{id}", new
			{
				controller = "FWHome",
				action = "Index",
				id = UrlParameter.Optional
			}, new string[1]
			{
				"JieNor.Megi.My.Web.Areas.FW.Controllers"
			}).DataTokens.Add("Area", "FW");
			routes.MapRoute("Default1", "{controller}/{action}/{id}", new
			{
				controller = "Home",
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
