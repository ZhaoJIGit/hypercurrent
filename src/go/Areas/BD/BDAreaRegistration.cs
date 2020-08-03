using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD
{
	public class BDAreaRegistration : AreaRegistration
	{
		public override string AreaName => "BD";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("BD_default", "BD/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
			context.MapRoute("BD_IDNumberTitle", "BD/{controller}/{action}/{id}/{number}/{title}", new
			{
				action = "Index",
				id = UrlParameter.Optional,
				number = UrlParameter.Optional
			});
		}
	}
}
