using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PA
{
	public class PAAreaRegistration : AreaRegistration
	{
		public override string AreaName => "PA";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("PA_default", "PA/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
