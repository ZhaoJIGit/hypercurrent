using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PT
{
	public class PAAreaRegistration : AreaRegistration
	{
		public override string AreaName => "PT";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("PT_default", "PT/{controller}/{action}/{id}", new
			{
				action = "PTIndex",
				id = UrlParameter.Optional
			});
		}
	}
}
