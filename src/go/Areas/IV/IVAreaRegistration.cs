using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV
{
	public class IVAreaRegistration : AreaRegistration
	{
		public override string AreaName => "IV";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("IV_IDNumber", "IV/{controller}/{action}/{id}/{number}", new
			{
				action = "Index",
				id = UrlParameter.Optional,
				number = UrlParameter.Optional
			});
			context.MapRoute("IV_default", "IV/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
