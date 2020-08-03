using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Chart
{
	public class ChartAreaRegistration : AreaRegistration
	{
		public override string AreaName => "Chart";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("Chart_default", "Chart/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
