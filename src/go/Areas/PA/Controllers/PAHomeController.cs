using JieNor.Megi.Go.Web.Controllers;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PA.Controllers
{
	public class PAHomeController : GoControllerBase
	{
		public ActionResult Index()
		{
			return base.View();
		}
	}
}
