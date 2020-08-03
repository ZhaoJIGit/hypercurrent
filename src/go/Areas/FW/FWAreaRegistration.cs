using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FW
{
	public class FWAreaRegistration : AreaRegistration
	{
		public override string AreaName => "FW";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("FW_default", "", new
			{
				controller = "FWHome",
				action = "Index",
				id = UrlParameter.Optional
			});
			context.MapRoute("FW", "FW/{controller}/{action}", new
			{
				controller = "FWHome",
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
