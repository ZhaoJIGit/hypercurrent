using System.Web.Mvc;

namespace JieNor.Megi.My.Web.Areas.Org
{
	public class OrgAreaRegistration : AreaRegistration
	{
		public override string AreaName => "Org";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("Org_default", "Org/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
