using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PT
{
	public class PYAreaRegistration : AreaRegistration
	{
		public override string AreaName => "PY";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("PY_default", "PY/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
