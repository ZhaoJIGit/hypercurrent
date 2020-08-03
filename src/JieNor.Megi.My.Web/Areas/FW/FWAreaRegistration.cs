using System.Web.Mvc;

namespace JieNor.Megi.My.Web.Areas.FW
{
	public class FWAreaRegistration : AreaRegistration
	{
		public override string AreaName => "FW";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("FW_default", "FW/{controller}/{action}/{id}", new
			{
				controller = "FWHome",
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
