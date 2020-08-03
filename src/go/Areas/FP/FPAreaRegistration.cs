using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FP
{
	public class FPAreaRegistration : AreaRegistration
	{
		public override string AreaName => "FP";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("FP_default", "FP/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
