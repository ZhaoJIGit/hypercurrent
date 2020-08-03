using DevExpress.XtraReports.UI;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.DataModel;
using JieNor.Megi.Common.ImportAndExport.Export;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.DataModel.IO.Export;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.RPT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptManagerController : GoControllerBase
	{
		private IRPTReport _report = null;

		private IBASMyHome _myHome = null;

		public RptManagerController()
		{
		}

		public RptManagerController(IRPTReport report, IBASMyHome myHome)
		{
			_report = report;
			_myHome = myHome;
		}

		[HttpPost]
		public JsonResult UpdateReportByBizReport(string reportId, string content)
		{
			MActionResult<OperationResult> mActionResult = _report.UpdateReportByBizReport(reportId, HttpUtility.UrlDecode(content), null);
			base.UploadResult = JsonConvert.SerializeObject(mActionResult);
			return base.GetJsonResult(mActionResult);
		}

		public JsonResult DeleteReportByReportID(string reportId)
		{
			RPTReportModel resultData = _report.GetMainReportModel(reportId, null).ResultData;
			OperationResult resultData2 = _report.DeleteReport(new List<string>
			{
				reportId
			}, null).ResultData;
			if (resultData == null || reportId == resultData.MID)
			{
				resultData2.ObjectID = "";
			}
			else
			{
				resultData2.ObjectID = resultData.MID;
			}
			return base.Json(resultData2);
		}

		public JsonResult SaveReportAsDraft(RPTReportModel model)
		{
			model.MStatus = Convert.ToInt32(RPTReportStatus.Draft);
			MActionResult<OperationResult> data = _report.UpdateReport(model, null);
			return base.Json(data);
		}

		public JsonResult SaveReportAsPublished(RPTReportModel model)
		{
			model.MStatus = Convert.ToInt32(RPTReportStatus.Published);
			MActionResult<OperationResult> data = _report.UpdateReport(model, null);
			return base.Json(data);
		}

		public JsonResult GetDraftReportList(RPTReportQueryParam param)
		{
			param.Status = RPTReportStatus.Draft;
			MActionResult<List<RPTReportModel>> draftReportList = _report.GetDraftReportList(param, null);
			return base.Json(draftReportList);
		}

		public JsonResult GetPublishedReportList(RPTReportQueryParam param)
		{
			param.Status = RPTReportStatus.Published;
			MActionResult<List<RPTReportModel>> draftReportList = _report.GetDraftReportList(param, null);
			return base.Json(draftReportList);
		}

		public JsonResult GetReportModel(string MID)
		{
			MActionResult<RPTReportModel> reportModel = _report.GetReportModel(MID, null);
			return base.Json(reportModel);
		}

		public JsonResult GetReportData(ReportFilterBase filter)
		{
			MActionResult<BizReportModel> bizReportModel = _report.GetBizReportModel(filter.MReportID, null);
			return base.Json(bizReportModel);
		}

		[ValidateInput(false)]
		public FileResult Export(string id, string type, int reportTypeId, string jsonParam)
		{
			ExportFileType fileType = (ExportFileType)Enum.Parse(typeof(ExportFileType), type);
			BizReportType bizReportType = (BizReportType)reportTypeId;
			string arg = string.Empty;
			MContext mContext = base.MContext;
			Stream stream = null;
			ReportModel reportModel = null;
			try
			{
				if (string.IsNullOrWhiteSpace(jsonParam))
				{
					RPTReportModel resultData = _report.GetMainReportModel(id, null).ResultData;
					reportModel = ReportStorageHelper.CreateReportModel(bizReportType, JsonConvert.SerializeObject(new BaseParamModel
					{
						ReportId = id
					}), CreateReportModelSource.Export, null, type, null);
					stream = ExportHelper.CreateRptExportFile(reportModel, fileType);
					arg = (string.IsNullOrWhiteSpace(resultData.MSheetName) ? HtmlLang.GetText(LangModule.Report, bizReportType.ToString()) : resultData.MSheetName);
				}
				else
				{
					int num = 0;
					List<string> list = new List<string>();
					string text = $"{mContext.MUserID}-{mContext.MOrgID}";
					string text2 = string.Join("\\", base.Server.MapPath("~/App_Data/Temp/"), bizReportType, text);
					FileHelper.CreateRandomDirectory(ref text2, RandomDirectoryType.Public);
					List<ReportModel> list2 = ReportStorageHelper.CreateReportModelList(bizReportType, jsonParam, CreateReportModelSource.Export, null, type, mContext);
					foreach (ReportModel item in list2)
					{
						if (item.IsSheetHide)
						{
							num++;
						}
						else
						{
							string text3 = item.SheetName.FilterInvalidSheetName(28);
							item.ReportPathForEmail = string.Join("\\", text2, text3) + ".xls";
							ExportHelper.CreateRptAttachForEmail(item, fileType, text3);
							list.Add(item.ReportPathForEmail);
							num++;
						}
					}
					stream = ExportHelper.CombineExcelSheet(list);
					arg = HtmlLang.GetText(LangModule.Report, bizReportType.ToString());
					if (stream == null)
					{
						MLogger.Log(new Exception("stream为空"));
						ReportModel reportModel2 = new ReportModel();
						reportModel2.Report = new XtraReport();
						stream = ExportHelper.CreateRptExportFile(reportModel2, fileType);
					}
				}
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
			}
			string exportName = $"{mContext.MOrgName}-{arg}.{fileType.ToString().ToLower()}";
			return base.ExportReport(stream, exportName);
		}
	}
}
