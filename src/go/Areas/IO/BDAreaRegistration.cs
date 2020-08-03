using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IO
{
	public class BDAreaRegistration : AreaRegistration
	{
		public override string AreaName => "IO";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("IO_default", "IO/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
