using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.HtmlHelper;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Adviser.Controllers
{
	public class OptLogController : GoControllerBase
	{
		public ActionResult LogList()
		{
			base.SetTitle(HtmlLang.Write(LangModule.IV, "HistoryNotesActivity", "History & Notes Activity").ToString());
			base.SetModule("adviser");
			return base.View();
		}
	}
}
