using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Demo
{
	public class DemoAreaRegistration : AreaRegistration
	{
		public override string AreaName => "Demo";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("Demo_default", "Demo/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
