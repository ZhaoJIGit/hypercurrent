using System.Web.Mvc;

namespace JieNor.Megi.My.Web.Areas.Setting
{
	public class SettingAreaRegistration : AreaRegistration
	{
		public override string AreaName => "Setting";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("Setting_default", "Setting/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
