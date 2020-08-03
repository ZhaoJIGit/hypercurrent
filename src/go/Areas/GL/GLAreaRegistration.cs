using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.GL
{
	public class GLAreaRegistration : AreaRegistration
	{
		public override string AreaName => "GL";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("GL_default", "GL/{controller}/{action}/{id}", new
			{
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
