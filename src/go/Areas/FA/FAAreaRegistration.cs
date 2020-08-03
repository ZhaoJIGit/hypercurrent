using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FA
{
	public class FAAreaRegistration : AreaRegistration
	{
		public override string AreaName => "FA";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("FA_default", "FA/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
