using JieNor.Megi.Core;
using JieNor.Megi.Go.Web.Controllers;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers.GL
{
	public class ReportGLBaseController : GoControllerBase
	{
		public ActionResult CheckTypeDetail(string value)
		{
			List<NameValueModel> obj = new List<NameValueModel>();
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			javaScriptSerializer.MaxJsonLength = 2147483647;
			if (!string.IsNullOrWhiteSpace(value))
			{
				value = HttpUtility.UrlDecode(value);
				obj = javaScriptSerializer.Deserialize<List<NameValueModel>>(value);
			}

			ViewBag.CheckTypeValueList = javaScriptSerializer.Serialize(obj);
			return View("~\\Areas\\Report\\Views\\RptUC\\GL\\CheckTypeDetail.cshtml");
		}
	}
}
