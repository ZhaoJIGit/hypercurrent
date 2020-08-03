using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.Go.HtmlHelper;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.FA;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.IV;
using JieNor.Megi.ServiceContract.RI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.GL.Controllers
{
	public class GLVoucherController : GoControllerBase
	{
		private IBDAccount _account = null;

		private IBDContacts _contact = null;

		private IBDTrack _track = null;

		private IGLVoucher _voucher = null;

		public IGLInitBalance _initBalance = null;

		public IGLVoucherReference _voucherReference = null;

		public IGLPeriodTransfer _periodTransfer = null;

		public IGLSettlement _settlement = null;

		public IGLDocVoucher _docVoucher = null;

		public IIVInvoice _invoice = null;

		public IFADepreciation _depreciation = null;

		public IRIInspect _inspect = null;

		public GLVoucherController(IBDAccount account, IBDTrack track, IBDContacts contact, IGLVoucher voucher, IGLInitBalance initBalance, IGLPeriodTransfer periodTransfer, IGLVoucherReference voucherReference, IGLSettlement settlement, IGLDocVoucher docVoucher, IIVInvoice invoice, IFADepreciation depreciation, IRIInspect inspect)
		{
			_account = account;
			_track = track;
			_contact = contact;
			_voucher = voucher;
			_initBalance = initBalance;
			_voucherReference = voucherReference;
			_settlement = settlement;
			_periodTransfer = periodTransfer;
			_docVoucher = docVoucher;
			_invoice = invoice;
			_depreciation = depreciation;
			_inspect = inspect;
		}

		public ActionResult GLVoucherHome(int Index = 0, int SubIndex = 0)
		{
			base.ViewData["Index"] = Index;
			base.ViewData["SubIndex"] = SubIndex;
			base.ViewData["DashboardInfo"] = _voucher.GetDashboardInfo(null).ResultData;
			return base.View();
		}

		public ActionResult GetGLDashboardData(int year, int period, int type = 0)
		{
			MActionResult<GLDashboardModel> dashboardData = _voucher.GetDashboardData(year, period, type, null);
			return base.Json(dashboardData);
		}

		public ActionResult GLVoucherEdit(string MItemID, string MNumber = null, bool IsCopy = false, bool IsReverse = false, bool FromPeriodTransfer = false, int year = 0, int period = 0, int day = 0, int IsModule = 0, int MDir = 0)
		{
			if (MDir == -1 || MDir == 1)
			{
				GLVoucherModel resultData = _voucher.GetVoucherEditModel(new GLVoucherModel
				{
					MItemID = MItemID,
					MNumber = MNumber,
					MIsCopy = (IsCopy && true),
					MIsReverse = (IsReverse && true),
					MYear = year,
					MPeriod = period,
					MDir = MDir
				}, null).ResultData;
				if (resultData != null)
				{
					MItemID = resultData.MItemID;
					MDir = 0;
					IsCopy = false;
					IsReverse = false;
					MNumber = resultData.MNumber;
					year = resultData.MYear;
					period = resultData.MPeriod;
					FromPeriodTransfer = false;
				}
			}
			base.ViewData["MItemID"] = MItemID;
			base.ViewData["MNumber"] = MNumber;
			base.ViewData["IsCopy"] = (IsCopy ? 1 : 0);
			base.ViewData["IsReverse"] = (IsReverse ? 1 : 0);
			base.ViewData["TrackCategory"] = _track.GetTrackBasicInfo(null, null).ResultData;
			base.ViewData["ShowContact"] = 1;
			base.ViewData["FromPeriodTransfer"] = (FromPeriodTransfer ? 1 : 0);
			base.ViewData["MYear"] = year;
			base.ViewData["MPeriod"] = period;
			base.ViewData["IsModule"] = IsModule;
			base.ViewData["Day"] = day;
			base.ViewData["MDir"] = MDir;
			base.ViewData["DashboardInfo"] = _voucher.GetDashboardInfo(null).ResultData;
			return base.View();
		}

		public ActionResult Exists(string pkID)
		{
			MActionResult<bool> data = _voucher.Exists(pkID, false, null);
			return base.Json(data);
		}

		public ActionResult GLGetNextVoucherNumber(int year, int period)
		{
			MActionResult<string> nextVoucherNumber = _voucher.GetNextVoucherNumber(year, period, null);
			return base.Json(nextVoucherNumber);
		}

		public ActionResult GetDocMType(string docID)
		{
			IVInvoiceModel resultData = _invoice.GetDataModel(docID, false, null).ResultData;
			return base.Json((resultData == null) ? "" : resultData.MType);
		}

		public ActionResult GLGetVoucherPageList(GLVoucherListFilterModel filter)
		{
			MActionResult<DataGridJson<GLVoucherViewModel>> voucherModelPageList = _voucher.GetVoucherModelPageList(filter, null);
			string obj = new HtmlVoucherHelper().GenerateVoucherListHtml(voucherModelPageList.ResultData.rows);
			voucherModelPageList.ResultData.rows = new List<GLVoucherViewModel>();
			voucherModelPageList.ResultData.obj = obj;
			GLSettlementModel model = new GLSettlementModel
			{
				MYear = filter.Year,
				MPeriod = filter.Period
			};
			GLSettlementModel resultData = _settlement.GetSettlementModel(model, null).ResultData;
			return base.Json(new
			{
				Voucher = voucherModelPageList.ResultData,
				Settlement = resultData,
				Success = voucherModelPageList.ResultCode
			});
		}

		public ActionResult GetSettlement(GLSettlementModel model)
		{
			MActionResult<GLSettlementModel> settlementModel = _settlement.GetSettlementModel(model, null);
			return base.Json(settlementModel);
		}

		public ActionResult GetExsitsAndCalculatedPeriodTransfer(GLPeriodTransferModel model)
		{
			MActionResult<List<GLPeriodTransferModel>> exsitsAndCalculatedPeriodTransfer = _periodTransfer.GetExsitsAndCalculatedPeriodTransfer(model, null);
			return base.Json(exsitsAndCalculatedPeriodTransfer);
		}

		public ActionResult GLDeleteVoucher(List<string> ids)
		{
			MActionResult<OperationResult> data = _voucher.DeleteVoucherModels(ids, null);
			return base.Json(data);
		}

		public ActionResult ApproveVoucher(List<string> ids, string status)
		{
			MActionResult<OperationResult> data = _voucher.ApproveVouchers(ids, status, null);
			return base.Json(data);
		}

		public ActionResult GLPreSettlePeriod(DateTime date, bool isCalculate = false)
		{
			MActionResult<OperationResult> data = _settlement.PreSettle(date, isCalculate, null);
			return base.Json(data);
		}

		public ActionResult GLSettlePeriod(GLSettlementModel model)
		{
			MActionResult<OperationResult> data = _settlement.Settle(model, null);
			return base.Json(data);
		}

		public ActionResult GLExplanationEdit()
		{
			return base.View();
		}

		public ActionResult GetVoucherExplanationList(int size)
		{
			MActionResult<List<GLVoucherReferenceModel>> referenceList = _voucherReference.GetReferenceList(size, null);
			return base.Json(referenceList);
		}

		public ActionResult GetRelateDeleteVoucherList(List<string> pkIDS)
		{
			MActionResult<List<GLVoucherModel>> relateDeleteVoucherList = _voucher.GetRelateDeleteVoucherList(pkIDS, null);
			return base.Json(relateDeleteVoucherList);
		}

		public ActionResult GetVoucherExplanationPageList(SqlWhere filter)
		{
			filter.AddFilter(new SqlFilter("MLocaleID", SqlOperators.Equal, ContextHelper.MContext.MLCID));
			MActionResult<DataGridJson<GLVoucherReferenceModel>> modelPageList = _voucherReference.GetModelPageList(filter, false, null);
			return base.Json(modelPageList);
		}

		public ActionResult DeleteVoucherExplanation(string itemID)
		{
			MActionResult<OperationResult> data = _voucherReference.Delete(itemID, null);
			return base.Json(data);
		}

		public ActionResult UpdateVoucherExplanation(string itemID, string content)
		{
			MContext mContext = ContextHelper.MContext;
			GLVoucherReferenceModel model = new GLVoucherReferenceModel
			{
				MItemID = itemID,
				MContent = content,
				MLocaleID = mContext.MLCID,
				MOrgID = mContext.MOrgID
			};
			MActionResult<OperationResult> data = _voucherReference.InsertReference(model, null);
			return base.Json(data);
		}

		public ActionResult GLVoucherNumberReorder(int year, int period)
		{
			MActionResult<bool> data = _voucher.ReorderVoucherNumber(year, period, 1, null);
			return base.Json(data);
		}

		public ActionResult CheckVoucherHasUnapproved(GLSettlementModel model)
		{
			MActionResult<bool> data = _voucher.CheckVoucherHasUnapproved(model, null);
			return base.Json(data);
		}

		public ActionResult GLCalculatePeriodTransfer(GLPeriodTransferModel model)
		{
			MActionResult<GLPeriodTransferModel> data = _periodTransfer.CalculatePeriodTransfer(model, null);
			return base.Json(data);
		}

		public ActionResult GLGetPeriodTransfer(GLPeriodTransferModel model)
		{
			MActionResult<GLPeriodTransferModel> periodTransfer = _periodTransfer.GetPeriodTransfer(model, true, null);
			return base.Json(periodTransfer);
		}

		public ActionResult GLCreateVoucher(GLPeriodTransferModel model)
		{
			base.ViewData["FromPeriodTransfer"] = 1;
			base.ViewData["MItemID"] = "";
			base.ViewData["IsCopy"] = 0;
			base.ViewData["TrackCategory"] = _track.GetTrackBasicInfo(null, null).ResultData;
			base.ViewData["MYear"] = model.MYear;
			base.ViewData["MPeriod"] = model.MPeriod;
			base.ViewData["MAmount"] = model.MAmount;
			base.ViewData["MPercent0"] = model.MPercent0;
			base.ViewData["MPercent1"] = model.MPercent1;
			base.ViewData["MPercent2"] = model.MPercent2;
			base.ViewData["MTransferTypeID"] = model.MTransferTypeID;
			base.ViewData["DashboardInfo"] = _voucher.GetDashboardInfo(null).ResultData;
			return base.View("GLVoucherEdit");
		}

		[Permission("General_Ledger", "Export", "")]
		public FileResult Export(string jsonParam)
		{
			Stream stream = ExportToImportHelper.CreateExportObjStream(BizReportType.VoucherList, jsonParam);
			string exportName = string.Format("{0} - {1}.xls", ContextHelper.MContext.MOrgName, HtmlLang.GetText(LangModule.GL, "Vouchers", "Vouchers"));
			return base.ExportReport(stream, exportName);
		}

		public ActionResult GetDocVoucherModelList(GLDocVoucherFilterModel filter)
		{
			object obj = null;
			obj = ((filter.Type != 6) ? ((object)_docVoucher.GetDocVoucherModelList(filter, null).ResultData) : ((object)_depreciation.GetSummaryDepreciationPageList(new FAFixAssetsFilterModel
			{
				Year = filter.Year,
				Period = filter.Period,
				Keyword = filter.Keyword,
				MKeyword = filter.DecimalKeyword,
				rows = filter.rows,
				page = filter.page,
				Number = filter.Number
			}, null).ResultData));
			GLSettlementModel resultData = _settlement.GetSettlementModel(new GLSettlementModel
			{
				MYear = filter.Year,
				MPeriod = filter.Period
			}, null).ResultData;
			return base.Json(new
			{
				docs = obj,
				settlement = resultData
			});
		}

		public ActionResult GetUpdatedDocTable(DateTime lastQueryDate)
		{
			MActionResult<List<string>> updatedDocTable = _docVoucher.GetUpdatedDocTable(lastQueryDate, null);
			return base.Json(updatedDocTable);
		}

		public ActionResult CreateDocVoucher(List<GLDocEntryVoucherModel> list, bool create = true)
		{
			MActionResult<OperationResult> data = _docVoucher.CreateDocVoucher(list, create, null);
			return base.Json(data);
		}

		public ActionResult ResetDocVoucher(List<string> docIDs, int docType)
		{
			MActionResult<OperationResult> data = _docVoucher.ResetDocVoucher(docIDs, docType, null);
			return base.Json(data);
		}

		public ActionResult IsMNumberUsed(int year, int period, string number)
		{
			return base.Json(_voucher.IsMNumberUsed(year, period, number, null));
		}

		public ActionResult GLDocVoucherRuleEdit(GLDocVoucherFilterModel filter)
		{
			base.ViewData["MType"] = filter.Type;
			filter.MEntryIDs = (filter.MEntryIDs ?? new List<string>());
			base.ViewData["MDocIDs"] = string.Join(",", (filter.MDocIDs ?? new List<string>()).Distinct().ToArray());
			return base.View();
		}

		public ActionResult DeleteDocVoucher(List<GLDocEntryVoucherModel> list)
		{
			MActionResult<OperationResult> data = _docVoucher.DeleteDocVoucher(list, null);
			return base.Json(data);
		}

		public ActionResult GLDocVoucherEdit(GLDocEntryVoucherModel model)
		{
			base.ViewData["MItemID"] = model.MVoucherID;
			base.ViewData["FromDocVoucher"] = 1;
			base.ViewData["MDocVoucherID"] = model.MDocVoucherID;
			base.ViewData["MDocType"] = model.MDocType;
			base.ViewData["MEntryAccountPair"] = model.MDebitEntryID + "-" + model.MDebitAccountID + "," + model.MTaxEntryID + "-" + model.MTaxAccountID + "," + model.MCreditEntryID + "-" + model.MCreditAccountID;
			base.ViewData["DashboardInfo"] = _voucher.GetDashboardInfo(null).ResultData;
			return base.View("GLVoucherEdit");
		}

		[Permission("General_Ledger", "View", "")]
		public ActionResult GetVoucherEditModel(GLVoucherModel model)
		{
			MActionResult<GLVoucherModel> voucherEditModel = _voucher.GetVoucherEditModel(model, null);
			return base.Json(voucherEditModel);
		}

		[Permission("General_Ledger", "Change", "")]
		public ActionResult UpdateVoucher(GLVoucherModel model)
		{
			MActionResult<GLVoucherModel> data = _voucher.UpdateVoucher(model, null);
			return base.Json(data);
		}

		[Permission("General_Ledger", "Change", "")]
		public ActionResult RoutineInpect(int year, int period)
		{
			base.ViewData["year"] = year;
			base.ViewData["period"] = period;
			return base.View();
		}

		[Permission("General_Ledger", "Change", "")]
		public ActionResult GetCategoryList(int year, int period)
		{
			MActionResult<List<RICategoryModel>> categoryList = _inspect.GetCategoryList(false, year, period, null);
			return base.Json(categoryList);
		}

		[Permission("General_Ledger", "Change", "")]
		public ActionResult StartInspect(string settingId, int year, int period)
		{
			MActionResult<RIInspectionResult> data = _inspect.Inspect(settingId, year, period, null);
			return base.Json(data);
		}

		[Permission("General_Ledger", "Change", "")]
		public ActionResult ClearDataPool()
		{
			MActionResult<OperationResult> data = _inspect.ClearDataPool(null);
			return base.Json(data);
		}
	}
}
