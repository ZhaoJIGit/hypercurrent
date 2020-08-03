using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Sys
{
	public class SysAreaRegistration : AreaRegistration
	{
		public override string AreaName => "Sys";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("Sys_default", "Sys/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
