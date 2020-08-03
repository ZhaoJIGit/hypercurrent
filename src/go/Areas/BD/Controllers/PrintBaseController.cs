using JieNor.Megi.Go.Web.Controllers;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class PrintBaseController : GoControllerBase
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			string name = "en-US";
			if (base.MContext != null && base.MContext.MLCID == "0x7804")
			{
				name = "zh-CN";
			}
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(name);
			Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
			base.OnActionExecuting(filterContext);
		}
	}
}
