using JieNor.Megi.Common.ImportAndExport.DataModel;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.IO.Export;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class ReportDesignerController : PrintBaseController
	{
		public ActionResult Index(BizReportType reportType, string jsonParam, string printSettingID, CreateReportModelSource source)
		{
			ReportModel model = base.CreateReportModel(reportType, jsonParam, source, printSettingID, null);
			return base.View(model);
		}

		public ActionResult Save(string reportDesigner, string layoutId, BizReportType reportType, string printSettingID)
		{
			string text = "/BD/Print/Preview";
			ReportStorageHelper.SaveReportLayout(layoutId, reportType, reportDesigner, printSettingID);
			text = text.TrimEnd('?');
			text += string.Format("{0}source={1}", (text.LastIndexOf('?') != -1) ? "&" : "?", CreateReportModelSource.SaveLayout);
			return base.Json(new
			{
				redirectUrl = text
			});
		}
	}
}
