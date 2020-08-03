using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FC
{
	public class FCAreaRegistration : AreaRegistration
	{
		public override string AreaName => "FC";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("FC_default", "FC/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
