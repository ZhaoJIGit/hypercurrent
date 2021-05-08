using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.DataModel;
using JieNor.Megi.Common.ImportAndExport.Export;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.IO.Export;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.FP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FP.Controllers
{
	public class FPHomeController : GoControllerBase
	{
		private IFPTable _table;

		private IFPFapiao _fapiao;

		public FPHomeController(IFPTable table, IFPFapiao fapiao)
		{
			_table = table;
			_fapiao = fapiao;
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult FPHome(int index = 1, int invoiceType = 0)
		{
			base.ViewData["index"] = index;
			base.ViewData["invoiceType"] = invoiceType;
			return base.View();
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult FPEditTable(string tableId, string invoiceIds, int invoiceType = 0)
		{
			base.ViewData["invoiceIds"] = invoiceIds;
			base.ViewData["tableId"] = tableId;
			base.ViewData["invoiceType"] = invoiceType;
			base.ViewData["MCurrencyID"] = base.MContext.MBasCurrencyID;
			return base.View();
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetInvoiceTable(string tableId, string invoiceIds)
		{
			MActionResult<FPTableViewModel> tableViewModel = _table.GetTableViewModel(tableId, invoiceIds, 0, null);
			return base.Json(tableViewModel);
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult FPSaveTable(FPTableViewModel table)
		{
			MActionResult<OperationResult> data = _table.SaveTable(table, null);
			return base.Json(data);
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult FPSaveFapiao(FPFapiaoModel fapiao)
		{
			MActionResult<FPFapiaoModel> data = _fapiao.SaveFapiao(fapiao, null);
			return base.Json(data);
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetTableBaseInfo(string tableId, string invoiceIds, int invoiceType)
		{
			MActionResult<FPTableViewModel> tableViewModel = _table.GetTableViewModel(tableId, invoiceIds, invoiceType, null);
			return base.Json(tableViewModel);
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetTableHomeData(int invoiceType, DateTime? date)
		{
			if (!date.HasValue)
			{
				date = new DateTime(1900, 1, 1);
			}
			MActionResult<List<NameValueModel>> tableHomeData = _table.GetTableHomeData(invoiceType, date.Value, null);
			return base.Json(tableHomeData);
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetFapiaoList(string tableId, string invoiceIds)
		{
			MActionResult<List<FPFapiaoModel>> fapiaoListByTableInvoice = _table.GetFapiaoListByTableInvoice(tableId, invoiceIds, null);
			return base.Json(fapiaoListByTableInvoice);
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetTableViewModelPageList(FPTableViewFilterModel filter)
		{
			MActionResult<DataGridJson<FPTableViewModel>> tableViewModelPageList = _table.GetTableViewModelPageList(filter, null);
			return base.Json(tableViewModelPageList);
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult DeleteFapiaoByFapiaoIds(FPFapiaoFilterModel model)
		{
			MActionResult<OperationResult> data = _fapiao.DeleteFapiaoByFapiaoIds(model, null);
			return base.Json(data);
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult DeleteFPImportByImportIds(FPFapiaoFilterModel model)
		{
			MActionResult<OperationResult> data = _fapiao.DeleteFPImportByIds(model, null);
			return base.Json(data);
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult BatchUpdateFPStatusByIds(FPFapiaoFilterModel model)
		{
			MActionResult<OperationResult> data = _fapiao.BatchUpdateFPStatusByIds(model, null);
			return base.Json(data);
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult BatchUpdateFPVerifyType(List<FPFapiaoModel> model)
		{
			MActionResult<OperationResult> data = _fapiao.BatchUpdateFPVerifyType(model, null);
			return base.Json(data);
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult FPEditFapiao(DateTime? date, string mid = null, string contactId = null, string tableId = null, string invoiceType = null, string explanation = null, string number = null, string tableNumber = null, decimal maxAmount = default(decimal), decimal maxTaxAmount = default(decimal))
		{
			mid = (mid ?? string.Empty);
			contactId = (contactId ?? string.Empty);
			tableId = (tableId ?? string.Empty);
			number = (number ?? string.Empty);
			explanation = (explanation ?? string.Empty);
			tableNumber = (tableNumber ?? string.Empty);
			invoiceType = (invoiceType ?? "0");
			base.ViewData["mid"] = mid;
			base.ViewData["contactId"] = contactId;
			base.ViewData["tableId"] = tableId;
			base.ViewData["explanation"] = explanation;
			base.ViewData["date"] = (date.HasValue ? date.Value : ContextHelper.MContext.DateNow);
			base.ViewData["number"] = number;
			base.ViewData["tableNumber"] = tableNumber;
			base.ViewData["maxAmount"] = maxAmount;
			base.ViewData["maxTaxAmount"] = maxTaxAmount;
			base.ViewData["invoiceType"] = invoiceType;
			return base.View();
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetFapiao(FPFapiaoModel fapiao)
		{
			MActionResult<List<FPFapiaoModel>> fapiaoByIds = _fapiao.GetFapiaoByIds(new List<string>
			{
				fapiao.MID
			}, true, fapiao.MContactID, null);
			FPFapiaoModel fPFapiaoModel = fapiaoByIds.ResultData[0];
			if (string.IsNullOrWhiteSpace(fPFapiaoModel.MID))
			{
				fPFapiaoModel.MNumber = fapiao.MNumber;
				fPFapiaoModel.MExplanation = fapiao.MExplanation;
				fPFapiaoModel.MBizDate = fapiao.MBizDate;
				fPFapiaoModel.MTableID = fapiao.MTableID;
			}
			return base.Json(fPFapiaoModel);
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult FPAddLog(FPFapiaoModel model)
		{
			_table.FPAddLog(model, null);
			return base.Json(new
			{
				Success = true
			}, JsonRequestBehavior.DenyGet);
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult DeleteTableByInvoiceIds(string invoiceIds)
		{
			MActionResult<OperationResult> data = _table.DeleteTableByInvoiceIds(invoiceIds, null);
			return base.Json(data);
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult DeleteTableByTableIds(string tableIds)
		{
			MActionResult<OperationResult> data = _table.DeleteTableByTableIds(tableIds, null);
			return base.Json(data);
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public JsonResult GetTableViewModelByInvoiceID(string invoiceId)
		{
			MActionResult<FPTableViewModel> tableViewModelByInvoiceID = _table.GetTableViewModelByInvoiceID(invoiceId, null);
			return base.Json(tableViewModelByInvoiceID);
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult FPViewFapiao(string fapiaoId)
		{
			base.ViewData["FapiaoId"] = fapiaoId;
			FPFapiaoModel resultData = _fapiao.GetFapiaoModel(new FPFapiaoFilterModel
			{
				MFapiaoIDs = new List<string>
				{
					fapiaoId
				}
			}, null).ResultData;
			base.ViewData["FapiaoModel"] = resultData;
			base.ViewData["FapiaoType"] = resultData.MType;
			return base.View();
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ViewResult FPReconcileHome(int type = 0, int index = 0)
		{
			base.ViewData["Type"] = type;
			base.ViewData["Index"] = index;
			return base.View();
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ViewResult FPEditReconcile(FPFapiaoFilterModel filter)
		{
			base.ViewData["TableID"] = filter.MTableID;
			base.ViewData["MFapiaoCategory"] = filter.MFapiaoCategory;
			return base.View();
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ViewResult FPEditVerifyStatus(string selectIds)
		{
			base.ViewData["selectIds"] = selectIds;
			base.ViewData["lang"] = ((ContextHelper.MContext.MLCID == LangCodeEnum.EN_US) ? "en" : ((ContextHelper.MContext.MLCID == LangCodeEnum.ZH_CN) ? "zh-cn" : "zh-tw"));
			return base.View();
		}

		public ViewResult EditFastCode()
		{
			return base.View();
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ViewResult FPTransactionDetail(FPFapiaoFilterModel filter)
		{
			base.ViewData["ImportID"] = filter.MImportID;
			base.ViewData["MFapiaoCategory"] = filter.MFapiaoCategory;
			base.ViewData["OrgVersion"] = filter.OrgVersion;
			return base.View();
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetReconcileList(FPFapiaoFilterModel filter)
		{
			return base.Json(_fapiao.GetReconcileList(filter, null));
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetStatementList(FPFapiaoFilterModel filter)
		{
			return base.Json(_fapiao.GetStatementList(filter, null));
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetTransactionList(FPFapiaoFilterModel filter)
		{
			return base.Json(_fapiao.GetTransactionList(filter, null));
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetFaPiaoVerifyList(FPFapiaoFilterModel filter)
		{
			List<FPFapiaoModel> resultData = _fapiao.GetFapiaoListByFilter(filter, null).ResultData;
			resultData.Insert(0, new FPFapiaoModel
			{
				MID = ""
			});
			return base.Json(resultData);
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetCodingList(FPFapiaoFilterModel filter)
		{
			return base.Json(null);
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetFapiaoLogList(FPFapiaoFilterModel filter)
		{
			return base.Json(_fapiao.GetFapiaoLogList(filter, null));
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetFapiaoImportDetail(FPFapiaoFilterModel filter)
		{
			return base.Json(_fapiao.GetTransactionList(filter, null));
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult SaveReconcile(FPFapiaoReconcileModel model)
		{
			return base.Json(_fapiao.SaveReconcile(model, null));
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult RemoveReconcile(FPFapiaoReconcileModel model)
		{
			return base.Json(_fapiao.RemoveReconcile(model, null));
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult SetReconcileStatus(FPFapiaoFilterModel filter)
		{
			return base.Json(_fapiao.SetReconcileStatus(filter, null));
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult GetCodingPageList(FPFapiaoFilterModel filter)
		{
			return base.Json(_fapiao.GetCodingPageList(filter, null));
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult SaveCodingStatus(FPFapiaoFilterModel filter)
		{
			return base.Json(_fapiao.SaveCodingStatus(filter, null));
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult SaveCoding(FPFapiaoFilterModel filter)
		{
			return base.Json(_fapiao.SaveCoding(filter, null));
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult ResetCodingData(FPFapiaoFilterModel filter)
		{
			return base.Json(_fapiao.ResetCodingData(filter, null));
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult SaveCodingRow(List<FPCodingModel> data)
		{
			return base.Json(_fapiao.SaveCodingRow(data, null));
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult DeleteCodingRow(FPCodingModel row)
		{
			return base.Json(_fapiao.DeleteCodingRow(row, null));
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult GetCodingSetting()
		{
			return base.Json(_fapiao.GetCodingSetting(null));
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public ActionResult SaveCodingSetting(FPCodingSettingModel model)
		{
			return base.Json(_fapiao.SaveCodingSetting(model, null));
		}

		public ActionResult GetBaseData()
		{
			return base.Json(_fapiao.GetBaseData(null));
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ViewResult FPDashboard()
		{
			return base.View();
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetDashboardData(int type)
		{
			return base.Json(null);
		}

		[Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
		public ActionResult GetDashboardTableData()
		{
			MActionResult<string> dashboardTableData = _table.GetDashboardTableData(null);
			return base.Json(dashboardTableData);
		}

		[Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
		public FileResult Export(string jsonParam)
		{
			FPFapiaoFilterModel fPFapiaoFilterModel = ReportParameterHelper.DeserializeObject<FPFapiaoFilterModel>(jsonParam);
			BizReportType reportType = (fPFapiaoFilterModel.MFapiaoCategory == 0) ? BizReportType.OutputInvoice : BizReportType.InputInvoice;
			string arg = (fPFapiaoFilterModel.MFapiaoCategory == 0) ? HtmlLang.GetText(LangModule.FP, "SalesFapiao", "销项发票") : HtmlLang.GetText(LangModule.FP, "IncomesFaPiao", "进项发票");
			ReportModel reportModel = ReportStorageHelper.CreateReportModel(reportType, jsonParam, CreateReportModelSource.Export, null, 0.ToString(), null);
			Stream stream = ExportHelper.CreateRptExportFile(reportModel, ExportFileType.Xls);
			string exportName = $"{reportModel.OrgName}-{arg}.xls";
			return base.ExportReport(stream, exportName);
		}
	}
}
