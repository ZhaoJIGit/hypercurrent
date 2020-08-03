using JieNor.Megi.Identity.Controllers;

namespace JieNor.Megi.My.Web.Controllers
{
	public class MyControllerBase : FrameworkController
	{
		public MyControllerBase()
		{
			base.ViewData["LoginUserName"] = base.LoginUserName;
			if (base.MContext != null)
			{
				base.ViewData["LastLoginTime"] = base.MContext.MLastAccessTime;
				base.ViewData["LastLoginOrgName"] = base.MContext.MLastLoginOrgName;
			}
		}
	}
}
