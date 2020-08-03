using JieNor.Megi.Identity.Controllers;
using System.Web.Mvc;

namespace JieNor.Megi.Login.Web.Controllers
{
	public class LinkExpireController : FrameworkController
	{
		public ActionResult Expire()
		{
			return base.View();
		}
	}
}
