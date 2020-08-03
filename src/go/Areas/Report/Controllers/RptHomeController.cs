using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.RPT;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptHomeController : GoControllerBase
	{
		private IRPTReport _report = null;

		public RptHomeController(IRPTReport report)
		{
			_report = report;
		}

		[Permission("Report", "View", "")]
		public ActionResult Index(int? id)
		{
			if (!id.HasValue)
			{
				id = 0;
			}
			base.SetModule("report");
			base.SetTitle(LangHelper.GetText(LangModule.Report, "AllReports", "All Reports"));
			base.ViewData["TabType"] = id;
			base.ViewData["AccountTableID"] = ContextHelper.MContext.MAccountTableID;
			return base.View();
		}

		[Permission("Report", "View", "")]
		public ActionResult Report2(int reportTypeId, string reportId, int? pReportTypeId, string pReportId)
		{
			if (!pReportTypeId.HasValue)
			{
				pReportTypeId = 0;
			}
			if (pReportId == null)
			{
				pReportId = string.Empty;
			}
			base.ViewData["IsView"] = false;
			string text = base.Request["filter"];
			if (string.IsNullOrEmpty(reportId) || reportId == "0")
			{
				reportId = "0";
				reportId = _report.AddEmptyReport(pReportId, null).ResultData;
				string str = string.Format("/Report/Report2/{0}/{1}{2}{3}{4}", reportTypeId, reportId, (pReportTypeId == 0) ? "" : $"/{pReportTypeId}", (pReportTypeId == 0) ? "" : $"/{pReportId}", string.IsNullOrEmpty(text) ? "" : $"?filter={text}");
				string str2 = base.HttpContext.Request.QueryString["bti"];
				string str3 = base.HttpContext.Request.QueryString["lang"];
				str += (string.IsNullOrEmpty(text) ? ("?bti=" + str2 + "&lang=" + str3) : ("&bti=" + str2 + "&lang=" + str3));
				return Redirect(str);
			}
			RPTReportModel resultData = _report.GetMainReportModel(reportId, null).ResultData;
			if (reportTypeId <= 0 || resultData == null)
			{
				return Redirect("/Report");
			}
			base.ViewData["ReportModel"] = resultData;
			base.SetModule("report");
			base.ViewData["ReportTypeID"] = reportTypeId;
			base.ViewData["ReportID"] = reportId;
			base.ViewData["ParentReportID"] = pReportId;
			base.ViewData["ParentReportTypeID"] = pReportTypeId;
			base.ViewData["ReportFilter"] = text;
			BizReportType bizReportType = (BizReportType)reportTypeId;
			base.ViewData["ReportType"] = bizReportType;
			base.ViewData["ReportSheet"] = _report.GetReportSheetList(reportId, null).ResultData;
			base.ViewData["ReportTitle"] = HtmlLang.GetText(LangModule.Report, bizReportType.ToString());
			base.ViewData["Report"] = $"../RptUC/{bizReportType.ToString()}";
			return base.View();
		}

		[Permission("Report", "View", "")]
		public ActionResult View(int reportTypeId, string reportId)
		{
			base.SetModule("report");
			base.ViewData["ReportTypeID"] = reportTypeId;
			base.ViewData["ReportID"] = reportId;
			base.ViewData["ParentReportID"] = "";
			base.ViewData["ParentReportTypeID"] = "";
			base.ViewData["IsView"] = true;
			BizReportType bizReportType = (BizReportType)reportTypeId;
			base.ViewData["ReportType"] = bizReportType;
			RPTReportModel resultData = _report.GetMainReportModel(reportId, null).ResultData;
			if (reportTypeId <= 0 || resultData == null)
			{
				return Redirect("/Report");
			}
			base.ViewData["ReportModel"] = resultData;
			base.ViewData["ReportSheet"] = _report.GetReportSheetList(reportId, null).ResultData;
			string text = HtmlLang.GetText(LangModule.Report, bizReportType.ToString());
			base.ViewData["ReportTitle"] = text;
			base.SetTitleAndCrumb(text, "<a href='/Report'>" + LangHelper.GetText(LangModule.Report, "Report", "Report") + " > </a><a href='/Report/2'>" + LangHelper.GetText(LangModule.Report, "Published", "Published") + " > </a>");
			return base.View();
		}

		public ActionResult Publish(int reportTypeId, string reportId)
		{
			base.SetModule("report");
			RPTReportModel resultData = _report.GetMainReportModel(reportId, null).ResultData;
			if (resultData == null)
			{
				return Redirect("/Report");
			}
			if (resultData.MID != reportId)
			{
				return Redirect($"/Report/Publish/{resultData.MType}/{resultData.MID}");
			}
			base.ViewData["ReportID"] = resultData.MID;
			base.ViewData["ReportTypeID"] = resultData.MType;
			BizReportType mType = (BizReportType)resultData.MType;
			string title = GetTitle(mType);
			base.SetTitleAndCrumb(title, "<a href='/Report'>" + LangHelper.GetText(LangModule.Report, "Report", "Report") + " > </a><a href='/Report/0'>" + LangHelper.GetText(LangModule.Report, "Draft", "Draft") + " > </a>");
			return base.View();
		}

		private string GetTitle(BizReportType type)
		{
			string text = type.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] >= 'A' && text[i] <= 'Z')
				{
					stringBuilder.AppendFormat(" {0}", text[i]);
				}
				else
				{
					stringBuilder.Append(text[i]);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
