using DevExpress.Web.Mvc;
using DevExpress.XtraReports.UI;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.AutoManager;
using JieNor.Megi.Common.ImportAndExport.DataModel;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.IO.Export;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.HtmlHelper;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class PrintController : PrintBaseController
	{
		public ActionResult Preview(BizReportType reportType, string jsonParam, string printSettingID, CreateReportModelSource source = CreateReportModelSource.Preview, string layoutId = null)
		{
			if (source == CreateReportModelSource.Restore)
			{
				RestoreReport(layoutId);
			}
			ReportModel reportModel = CreateReportModel(reportType, jsonParam, source, printSettingID, null);

			ViewBag.PdfUrl = ExportHelper.CreatePdfAccessUrl(reportModel);

			return View(reportModel);
		}

		public JsonResult PreviewCheck(BizReportType reportType, string jsonParam, string printSettingID)
		{
			return base.Json(ReportStorageHelper.CheckPrintDataModel(reportType, jsonParam, printSettingID));
		}

		public ActionResult PreviewPartial(BizReportType reportType, string jsonParam, string printSettingID)
		{
			return base.PartialView(base.CreateReportModel(reportType, jsonParam, CreateReportModelSource.PreviewPartial, printSettingID, null));
		}

		public ActionResult ExportDocument(BizReportType reportType, string jsonParam, string dxArgument)
		{
			string exportFormat = GetExportFormat(dxArgument);
			XtraReport report = base.CreateReportModel(reportType, jsonParam, CreateReportModelSource.PreviewExport, null, exportFormat).Report;
			FileResult fileResult = null;
			try
			{
				fileResult = DocumentViewerExtension.ExportTo(report);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				MLogger.Log(ex);
				if (message.IndexOf("too big") > -1)
				{
					message = HtmlLang.GetText(LangModule.BD, "ExportFileTooBig", "The output file is too big, please choose another export mode.");
					base.Response.Write("<script>alert('" + message + "');</script>");
					base.Response.End();
				}
			}
			if (fileResult.ContentType == "text/plain")
			{
				base.Response.Charset = "GBK";
			}
			return fileResult;
		}

		public ActionResult ViewPDF(string path)
		{
			ViewBag.PdfPath = path;
			return base.View();
		}

		public JsonResult WriteLog()
		{
			LogHelper.EndPreviewLog();
			LogHelper.EndLog();
			return base.Json(new
			{
				Success = true
			});
		}

		private void RestoreReport(string layoutId)
		{
			ReportManager.RestoreReport(layoutId);
		}

		private string GetExportFormat(string dxArgument)
		{
			string text = string.Empty;
			if (string.IsNullOrWhiteSpace(dxArgument))
			{
				return text;
			}
			dxArgument = HttpUtility.UrlDecode(dxArgument);
			string[] array = dxArgument.Split(new char[1]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string[] array3 = text2.Split(':');
				string text3 = array3[0];
				string a = text3;
				if (a == "saveToDisk=format" || a == "saveToWindow=format")
				{
					text = array3[1];
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					break;
				}
			}
			return text;
		}
	}
}
