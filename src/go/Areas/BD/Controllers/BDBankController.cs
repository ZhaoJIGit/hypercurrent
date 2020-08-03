using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.DataModel;
using JieNor.Megi.Common.ImportAndExport.Export;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Export;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Param;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Areas.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.IV;
using JieNor.Megi.ServiceContract.REG;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class BDBankController : BDBankControllerBase
	{
		private IIVBankApi _bankApi;

		private IIVTransactions _transaction;

		private IBDBankType _bdBankType;

		private IBDBankRule _bankRule;

		private IIVBankBill _bankBill;

		private IREGCurrency _currency;

		private IBDTrack _track;

		private IBDBankAccount _bankAccount;

		private new IBDAccount _bdAccount;

		private IGLInitBalance _initBalance;

		private IBDContacts _bdCont = null;

		private List<KeyValuePair<string, string>> SysStatementColumn
		{
			get
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("", LangHelper.GetText(LangModule.Bank, "Unassigned", "Unassigned")));
				list.Add(new KeyValuePair<string, string>("MDate", LangHelper.GetText(LangModule.Bank, "TransDate", "Transaction Date")));
				list.Add(new KeyValuePair<string, string>("MTime", LangHelper.GetText(LangModule.Bank, "TransTime", "Transaction Time")));
				list.Add(new KeyValuePair<string, string>("MTransType", LangHelper.GetText(LangModule.Bank, "TransType", "Transaction Type")));
				list.Add(new KeyValuePair<string, string>("MTransNo", LangHelper.GetText(LangModule.Bank, "TransNo", "Transaction No")));
				list.Add(new KeyValuePair<string, string>("MTransAcctName", LangHelper.GetText(LangModule.Bank, "PayeeAcctName", "Payee Account Name")));
				list.Add(new KeyValuePair<string, string>("MTransAcctNo", LangHelper.GetText(LangModule.Bank, "PayeeAcctNo", "Payee Account No")));
				list.Add(new KeyValuePair<string, string>("MSpentAmt", LangHelper.GetText(LangModule.Bank, "SpentAmt", "Spent Amount")));
				list.Add(new KeyValuePair<string, string>("MReceivedAmt", LangHelper.GetText(LangModule.Bank, "ReceivedAmt", "Received Amount")));
				list.Add(new KeyValuePair<string, string>("MBalance", LangHelper.GetText(LangModule.Bank, "Balance", "Balance")));
				list.Add(new KeyValuePair<string, string>("MDesc", LangHelper.GetText(LangModule.Common, "Reference", "Reference")));
				list.Add(new KeyValuePair<string, string>("MRef", LangHelper.GetText(LangModule.Common, "Description", "Description")));
				return list;
			}
		}

		public BDBankController(IBDAccount bdAccount, IBDBankType bdBankType, IIVBankApi bankApi, IIVTransactions transaction, IBDBankRule rule, IIVBankBill bill, IREGCurrency currency, IBDTrack track, IBDBankAccount bankAccount, IGLInitBalance initBalance, IBDContacts bdContact)
		{
			_transaction = transaction;
			_bdBankType = bdBankType;
			_bankRule = rule;
			_bankApi = bankApi;
			_bankBill = bill;
			_currency = currency;
			_track = track;
			_bankAccount = bankAccount;
			_bdAccount = bdAccount;
			_initBalance = initBalance;
			_bdCont = bdContact;
		}

		[Permission("Bank", "View", "")]
		public ActionResult BDBankHome(string acctId)
		{
			base.ViewData["acctId"] = acctId;
			return base.View();
		}

		[Permission("BankAccount", "Change", "")]
		public ActionResult BDBankAccountEdit(string type, string id)
		{
			ViewBag.Type = type;
			ViewBag.Id = id;
			return View();
		}

		[Permission("Bank", "View", "")]
		public ActionResult GetBDBankAccountViewList()
		{
			MContext mContext = ContextHelper.MContext;
			DateTime dateTime = mContext.DateNow;
			DateTime dateTime2 = dateTime.AddMonths(-5);
			int year = dateTime2.Year;
			dateTime2 = dateTime.AddMonths(-5);
			dateTime = new DateTime(year, dateTime2.Month, 1);
			dateTime = ((dateTime < mContext.MBeginDate) ? mContext.MBeginDate : dateTime);
			dateTime2 = mContext.DateNow;
			int year2 = dateTime2.Year;
			dateTime2 = mContext.DateNow;
			dateTime2 = new DateTime(year2, dateTime2.Month, 1);
			dateTime2 = dateTime2.AddMonths(1);
			DateTime endDate = dateTime2.AddDays(-1.0).ToDayLastSecond();
			string[] accountIds = new string[0];
			MActionResult<List<BDBankAccountEditModel>> bDBankAccountEditList = _bankAccount.GetBDBankAccountEditList(dateTime, endDate, accountIds, false, false, true, null);
			return base.Json(bDBankAccountEditList);
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult GetBDBankAccountEditModel(string MItemID, string parentId)
		{
			MActionResult<BDBankAccountEditModel> bDBankAccountEditModel = _bankAccount.GetBDBankAccountEditModel(MItemID, null);
			base.ViewData["Id"] = MItemID;
			return base.Json(bDBankAccountEditModel);
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult SaveBDBankAccount(BDBankAccountEditModel model)
		{
			MActionResult<OperationResult> data = _bankAccount.UpdateBankAccount(model, null);
			return base.Json(data);
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult BDBankAccountDelete(string MItemID)
		{
			MActionResult<OperationResult> data = _bdAccount.DeleteAccountByPkID(MItemID, null);
			return base.Json(data);
		}

		[Permission("BankAccount", "View", "")]
		public JsonResult GetBankTotalChartModel()
		{
			MActionResult<BDBankChartModel> bankTotalChartModel = _bankAccount.GetBankTotalChartModel(null);
			return base.Json(bankTotalChartModel);
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public ActionResult BDBankStatementView(IVTransactionQueryParamModel param)
		{
			List<IVBankStatementsModel> resultData = _transaction.GetBankStatementsList(param, null).ResultData;
			base.ViewData["statement"] = resultData.FirstOrDefault();
			List<IVBankStatementViewModel> resultData2 = _transaction.GetBankStatementView(param.MID, null).ResultData;
			base.ViewData["statementViews"] = resultData2;
			base.ViewData["MID"] = param.MID;
			base.ViewData["MBankID"] = param.MBankID;
			return base.View();
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public ActionResult DeleteBankbill(string[] mids)
		{
			MActionResult<OperationResult> data = _bankAccount.DeleteBankbill(mids, null);
			return base.Json(data);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public ActionResult BankStatementDelete(string id)
		{
			base.ViewData["selectIds"] = id;
			return base.View();
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public ActionResult BDBankStatementEdit(string MID, string MBankID)
		{
			base.ViewData["MID"] = MID;
			base.ViewData["MBankID"] = MBankID;
			return base.View();
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult UpdateBankBillVoucherStatus(ParamBase param, IVBankBillVoucherStatus status)
		{
			MActionResult<OperationResult> data = _bankBill.UpdateBankBillVoucherStatus(param.KeyIDSWithNoSingleQuote.Split(',').ToList(), status, null);
			return base.Json(data);
		}

		public ActionResult Import(string id, string type, string message)
		{
			List<BankBillImportSolutionModel> resultData = _transaction.GetBankBillImportSolutionList(type, null).ResultData;
			BDBankTypeModel resultData2 = _transaction.GetBankTypeModel(type, null).ResultData;
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			foreach (BankBillImportSolutionModel item in resultData)
			{
				list.Add(new KeyValuePair<string, string>(item.MItemID, item.MName));
			}
			list.Insert(0, new KeyValuePair<string, string>(string.Empty, LangHelper.GetText(LangModule.Bank, "NewImportSolution", "New Import Solution")));
			ViewBag.BankStaImpSolKVList = JsonConvert.SerializeObject(list);
			ViewBag.BankStaImpSolList = list;
			ViewBag.BankId = id;
			ViewBag.BankTypeId = type;
			ViewBag.BankTypeName = resultData2.MName;
			ViewBag.Message = message;
			return View();
		}

		public ActionResult ImportByBankFeeds(string id, string type)
		{
			BDBankTypeModel resultData = _transaction.GetBankTypeModel(type, null).ResultData;
			ViewBag.BankId = id;
			ViewBag.BankTypeId = type;
			ViewBag.BankTypeName = resultData.MName;

			return base.View();
		}

		public ActionResult ImportOptions(string id, string solutionId, string type, string fileName)
		{
			DataTable dataTable = null;
			try
			{
				dataTable = GetImportingData(fileName);
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					throw new Exception(HtmlLang.GetText(LangModule.Common, "NoDataFoundInImportingFile", "No data found in your importing file!"));
				}
			}
			catch (Exception ex)
			{
				return base.RedirectToAction("Import", new
				{
					id = id,
					type = type,
					message = ex.Message
				});
			}
			BDBankTypeModel resultData = _transaction.GetBankTypeModel(type, null).ResultData;
			ViewBag.BankTypeId = type;
			ViewBag.BankID = id;
			ViewBag.FileName = fileName;

			BankBillImportSolutionModel bankBillImportSolutionModel = new BankBillImportSolutionModel();
			if (string.IsNullOrWhiteSpace(solutionId))
			{
				ViewBag.IsNewSolution = true;

				if (type != 3.ToString())
				{
					bankBillImportSolutionModel.MName = resultData.MName;
				}
			}
			else
			{
				bankBillImportSolutionModel = _transaction.GetBankBillImportSolutionModel(solutionId, null).ResultData;
				ViewBag.IsNewSolution = false;

			}
			int num = 20;
			if (bankBillImportSolutionModel.MDataRowStart > 0)
			{
				num += bankBillImportSolutionModel.MDataRowStart - 1;
			}
			ViewBag.PreviewTop = num;

			DataTable dataTable2 = dataTable.AsEnumerable().Take(num).CopyToDataTable();
			ViewBag.ExcelHeader = string.Join(",", ExcelHelper.GetExcelHeader(dataTable2.Columns.Count));
			ViewBag.ExcelStatementData = dataTable2;
			ViewBag.JsonStatementData = JsonConvert.SerializeObject(dataTable2, new DataTableConverter()).Replace("'", "&apos;");
			ViewBag.JsonSysStatementColumn = JsonConvert.SerializeObject(SysStatementColumn);
			ViewBag.JsonSolution = JsonConvert.SerializeObject(bankBillImportSolutionModel);

			return base.View(bankBillImportSolutionModel);
		}

		public ActionResult SelectBank(string id)
		{
			ViewBag.Type = id;
			return base.View();
		}

		public FileResult Export(string jsonParam)
		{
			IVAccountTransactionsListFilterModel iVAccountTransactionsListFilterModel = ReportParameterHelper.DeserializeObject<IVAccountTransactionsListFilterModel>(jsonParam);
			ReportModel reportModel = ReportStorageHelper.CreateReportModel(BizReportType.TransactionList, jsonParam, CreateReportModelSource.Export, null, null, null);
			Stream stream = ExportHelper.CreateRptExportFile(reportModel, ExportFileType.Xls);
			string str = string.Empty;
			if (!string.IsNullOrWhiteSpace(iVAccountTransactionsListFilterModel.MBankID))
			{
				BDBankAccountModel resultData = _bankAccount.GetBDBankAccountEditModel(iVAccountTransactionsListFilterModel.MBankID, null).ResultData;
				MultiLanguageFieldList multiLanguageFieldList = resultData.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
				if (multiLanguageFieldList != null)
				{
					str = "-" + multiLanguageFieldList.MMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == base.MContext.MLCID).MValue;
				}
			}
			string empty = string.Empty;
			string transactionType = iVAccountTransactionsListFilterModel.TransactionType;
			empty = ((transactionType == "Payment") ? HtmlLang.GetText(LangModule.Bank, "SpendMoney", "Spend Money") : ((!(transactionType == "Receive")) ? HtmlLang.GetText(LangModule.Bank, "Transaction", "Transaction") : HtmlLang.GetText(LangModule.Bank, "ReceiveMoney", "Receive Money")));
			return base.ExportReport(stream, $"{reportModel.OrgName + str} - {empty}.xls");
		}

		public JsonResult SaveImportSolution(BankBillImportSolutionModel model)
		{
			MActionResult<OperationResult> data = _transaction.SaveBankBillImportSolution(model, null);
			return base.Json(data);
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult SaveImportBankBill(IVBankBillModel model)
		{
			OperationResult operationResult = new OperationResult();
			model.MFileName = string.Join("\\", base.Server.MapPath("~/App_Data/Temp/Common"), base.MContext.MUserID, model.MFileName);
			operationResult = _transaction.SaveImportBankBill(model, null, null).ResultData;
			return base.Json(operationResult);
		}

		public JsonResult GetCurrentTime()
		{
			return base.Json(base.MContext.DateNow.ToString("yyyyMMdd HH:mm:ss"), JsonRequestBehavior.AllowGet);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult StatementStatusUpdate(string selectIds, int directType)
		{
			MActionResult<int> data = _transaction.StatementStatusUpdate(selectIds, directType, null);
			return base.Json(data);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult StatementUpdate(List<IVBankStatementViewModel> viewModels)
		{
			return base.Json(_transaction.StatementUpdate(viewModels, null));
		}

		[Permission("BankAccount", "View", "")]
		public ActionResult GetBDBankTypeList()
		{
			MActionResult<List<BDBankTypeViewModel>> bDBankTypeList = _bdBankType.GetBDBankTypeList(null);
			return base.Json(bDBankTypeList);
		}

		public ActionResult BDBankTypeEdit()
		{
			return base.View();
		}

		public ActionResult GetBDBankTypeEditModel()
		{
			BDBankTypeEditModel model = new BDBankTypeEditModel();
			MActionResult<BDBankTypeEditModel> bDBankTypeEditModel = _bdBankType.GetBDBankTypeEditModel(model, null);
			return base.Json(bDBankTypeEditModel);
		}

		[Permission("BankAccount", "View", "")]
		public ActionResult SaveBDBankType(BDBankTypeEditModel model)
		{
			return base.Json(_bdBankType.SaveBankType(model, null));
		}

		[Permission("BankAccount", "View", "")]
		public ActionResult DeleteBDBankType(BDBankTypeModel model)
		{
			return base.Json(_bdBankType.DeleteBankType(model, null));
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public ActionResult BDBankRuleHome()
		{
			return base.View();
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public ActionResult BDBankRuleEdit(string id)
		{
			base.ViewData["BankRuleID"] = id;
			return base.View();
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public ActionResult BDBankRuleView(string id)
		{
			base.ViewData["BankRuleID"] = id;
			return base.View();
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public JsonResult GetBDBankRuleEditModel(string MItemID)
		{
			MActionResult<BDBankRuleModel> bDBankRuleModel = _bankRule.GetBDBankRuleModel(MItemID, null);
			return base.Json(bDBankRuleModel);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult UpdateBDBankRule(BDBankRuleModel model)
		{
			MActionResult<OperationResult> data = _bankRule.UpdateBankRule(model, null);
			return base.Json(data);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult DeleteBDBankRule(ParamBase param)
		{
			MActionResult<OperationResult> data = _bankRule.DeleteBankRule(param, null);
			return base.Json(data);
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public JsonResult GetBDBankRuleList(BDBankRuleListFilterModel filter)
		{
			MActionResult<DataGridJson<BDBankRuleListModel>> bankRuleList = _bankRule.GetBankRuleList(filter, null);
			return base.Json(bankRuleList);
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public ActionResult BDBankStatementList(string id)
		{
			base.ViewData["BankID"] = id;
			return base.View();
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public JsonResult GetBDBankStatementsList(IVTransactionQueryParamModel param)
		{
			MActionResult<List<IVBankStatementsModel>> bankStatementsList = _transaction.GetBankStatementsList(param, null);
			return base.Json(bankStatementsList);
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public JsonResult GetBDBankStatementModel(IVBankStatementsModel model)
		{
			MActionResult<IVBankStatementsModel> bankStatementModel = _transaction.GetBankStatementModel(model.MID, null);
			return base.Json(bankStatementModel);
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public JsonResult GetBDStatementDetails(string id)
		{
			MActionResult<List<IVBankStatementViewModel>> bankStatementView = _transaction.GetBankStatementView(id, null);
			return base.Json(bankStatementView);
		}

		[Permission("Bank", "View", "")]
		public ActionResult BDBankReconcileHome(string acctid, string type, string index, string bankType)
		{
			SetVDTracking();
			base.ViewData["id"] = acctid;
			base.ViewData["index"] = index;
			base.ViewData["type"] = type;
			base.ViewData["bankType"] = bankType;
			BDBankAccountEditModel resultData = _bankAccount.GetBDBankAccountEditModel(acctid, null).ResultData;
			if (resultData == null || string.IsNullOrEmpty(resultData.MItemID))
			{
				return Redirect("/");
			}
			base.ViewData["BankModel"] = resultData;
			List<BDContactItem> resultData2 = _bdCont.GetContactItemList(new BDContactsListFilter
			{
				MaxCount = 0
			}, null).ResultData;
			base.ViewData["ContactList"] = resultData2;
			return base.View();
		}

		[Permission("Bank", "View", "")]
		public ActionResult GetBDBankAccountInfoByDate(string acctid, DateTime? endDate)
		{
			MContext mContext = ContextHelper.MContext;
			BDBankAccountEditModel data = _bankAccount.GetBDBankAccountEditList(mContext.MBeginDate, endDate.HasValue ? endDate.Value.ToDayLastSecond() : DateTime.MaxValue, new string[1]
			{
				acctid
			}, false, false, false, null).ResultData.FirstOrDefault();
			return base.Json(data);
		}

		[Permission("BankAccount", "View", "")]
		public ActionResult GetBDBankAccountInfo(string acctid)
		{
			MContext mContext = ContextHelper.MContext;
			BDBankAccountEditModel data = _bankAccount.GetBDBankAccountEditList(mContext.MBeginDate, mContext.DateNow.ToDayLastSecond(), new string[1]
			{
				acctid
			}, false, false, false, null).ResultData.FirstOrDefault();
			return base.Json(data);
		}

		[Permission("BankAccount", "View", "")]
		public ActionResult GetAllBDBankAccountInfo()
		{
			MContext mContext = ContextHelper.MContext;
			MActionResult<List<BDBankAccountEditModel>> bDBankAccountEditList = _bankAccount.GetBDBankAccountEditList(mContext.MBeginDate, mContext.DateNow.ToDayLastSecond(), null, false, false, false, null);
			return base.Json(bDBankAccountEditList);
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public JsonResult GetBDBankReconcileList(IVBankBillRecListFilterModel filter)
		{
			MActionResult<DataGridJson<IVBankBillRecListModel>> bankBillRecList = _bankBill.GetBankBillRecList(filter, null);
			return base.Json(bankBillRecList);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public ActionResult BDBankReconcileEdit(string acctid)
		{
			base.ViewData["BankBillEntryID"] = acctid;
			base.ViewData["BankBillEntry"] = _bankBill.GetBankBillEntryEditModel(acctid, null).ResultData;
			base.ViewData["Amt"] = base.Request["amt"];
			return base.View();
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public ActionResult BDBankReconcileMatch(string id, string bankid, string mathid)
		{
			IVBankBillRecListModel result = _bankBill.GetBankBillRecByID(id, bankid, null).ResultData;
			if (string.IsNullOrEmpty(result.MMatchBillID) && !string.IsNullOrEmpty(mathid))
			{
				result.MMatchBillID = mathid;
			}
			if (result.MMatchList != null && result.MMatchList.Count > 0 && (string.IsNullOrEmpty(result.MMatchBillID) || !result.MMatchList.Any((IVReconcileTranstionListModel t) => t.MBillID == result.MMatchBillID)))
			{
				result.MMatchBillID = result.MMatchList[0].MBillID;
			}

			ViewBag.BankBillEntry = result;
			ViewBag.BankId = bankid;
			return base.View();
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public ActionResult BDBankReconcileSplitEdit(decimal totalAmtFor, decimal splitAmtFor, int rowIndex)
		{
			ViewBag.TotalAmtFor = totalAmtFor.To2Decimal();
			ViewBag.SplitAmtFor = splitAmtFor.To2Decimal();
			ViewBag.TotalAmtMoneyFormat = totalAmtFor.ToMoneyFormat();
			ViewBag.RowIndex = rowIndex;

			return base.View();
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public JsonResult GetBDBankReconcileTranstionList(IVReconcileTranstionListFilterModel filter)
		{
			MActionResult<List<IVReconcileTranstionListModel>> recTranstionList = _bankBill.GetRecTranstionList(filter, null);
			return base.Json(recTranstionList);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult UpdateBDBankBillReconcile(List<IVBankBillReconcileModel> model)
		{
			MActionResult<OperationResult> data = _bankBill.UpdateBankBillRec(model, null);
			return base.Json(data);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult UpdateReconcileMatch(string entryID, string matchBillID)
		{
			MActionResult<OperationResult> data = _bankBill.UpdateReconcileMatch(entryID, matchBillID, null);
			return base.Json(data);
		}

		[Permission("BankAccount", "View", "")]
		public ActionResult BDBankTransaction(string acctid)
		{
			base.ViewData["BankID"] = acctid;
			GLBalanceModel value = _bdAccount.GetBalance(acctid, null).ResultData.FirstOrDefault();
			base.ViewData["BankBalance"] = value;
			return base.View();
		}

		[Permission("Bank", "View", "")]
		public JsonResult GetBDBankTransactionList(IVAccountTransactionsListFilterModel param)
		{
			MActionResult<DataGridJson<IVAccountTransactionsModel>> transactionsList = _transaction.GetTransactionsList(param, null);
			return base.Json(transactionsList);
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public ActionResult BDBankReconcileView(string billType, string billId)
		{
			List<IVBankBillReconcileEntryModel> resultData = _bankBill.GetIVBankBillReconcileEntryModelList(billType, billId, null).ResultData;
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			base.ViewData["IVBankBillReconcileEntryModelList"] = javaScriptSerializer.Serialize(resultData);
			IVBankBillEntryModel value = new IVBankBillEntryModel();
			if (resultData != null && resultData.Count > 0)
			{
				value = _bankBill.GetIVBankBillEntryModelByBankBillReconcileMID(resultData[0].MID, null).ResultData;
			}
			base.ViewData["IVBankBillEntryModel"] = value;
			base.ViewData["BillType"] = billType;
			return base.View();
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult DeleteBankReconcile(string billType, string billId)
		{
			MActionResult<OperationResult> data = _bankBill.DeleteBankBillReconcile(billType, billId, null);
			return base.Json(data);
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult DeleteTransactions(List<IVAccountTransactionsModel> list)
		{
			MActionResult<OperationResult> data = _transaction.DeteleTransactions(list, null);
			return base.Json(data);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult UpdateReconcileStatu(IVTransactionsReconcileParam param)
		{
			MActionResult<OperationResult> data = _transaction.UpdateReconcileStatu(param, null);
			return base.Json(data);
		}

		public JsonResult GetBankFeeds(IVBankFeedsModel feedModel)
		{
			return base.Json(_bankApi.GetBankFeeds(feedModel, null));
		}

		public JsonResult GetBankStatementDetailList(IVBankStatementDetailFilter filter)
		{
			MActionResult<DataGridJson<IVBankStatementDetailModel>> bankStatementDetailList = _transaction.GetBankStatementDetailList(filter, null);
			return base.Json(bankStatementDetailList);
		}

		[Permission("BankAccount", "View", "")]
		public ActionResult BankCashCodingList(string id)
		{
			SetVDTracking();
			base.ViewData["BankID"] = id;
			return base.View();
		}

		[Permission("Bank_Reconciliation", "View", "")]
		public JsonResult GetCashCodingList(IVBankBillEntryListFilterModel filter)
		{
			MActionResult<DataGridJson<IVBankBillEntryModel>> cashCodingList = _bankBill.GetCashCodingList(filter, null);
			return base.Json(cashCodingList);
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult UpdateBankBillEntryList(List<IVBankBillEntryModel> entryList)
		{
			MActionResult<OperationResult> data = _bankBill.UpdateBankBillEntryList(entryList, null);
			return base.Json(data);
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult UpdateCashCodingList(IVCashCodingEditModel model)
		{
			BDBankAccountEditModel resultData = _bankAccount.GetBDBankAccountEditModel(model.MBankID, null).ResultData;
			if (resultData == null)
			{
				return base.Json(new OperationResult());
			}
			MActionResult<OperationResult> data = _bankBill.UpdateCashCoding(model, null);
			return base.Json(data);
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult DeleteCashCoding(ParamBase param)
		{
			MActionResult<OperationResult> data = _bankBill.DeleteBankBillEntry(param, null);
			return base.Json(data);
		}

		protected void SetVDTracking()
		{
			List<NameValueModel> resultData = _track.GetTrackBasicInfo(null, null).ResultData;
			string value = SerializeHelper.JsonSerialize(resultData);
			base.ViewData["Track"] = value;
		}

		private DataTable GetImportingData(string fileName)
		{
			string fullPath = string.Join("\\", base.Server.MapPath("~/App_Data/Temp/Common"), base.MContext.MUserID, fileName);
			Stream fileStream = FileHelper.GetFileStream(fullPath);
			NPOIHelper nPOIHelper = new NPOIHelper();
			return nPOIHelper.ImportData(fileStream, fileName);
		}

		[Permission("Setting", "View", "")]
		public ActionResult BankAccountBalances(string id)
		{
			ViewBag.OrgCode = id;
			return base.View();
		}

		[Permission("Setting", "View", "")]
		public JsonResult GetBankAccountBalancesByPage(string accountId)
		{
			SqlWhere filter = new SqlWhere();
			DataGridJson<BDBankInitBalanceModel> dataGridJson = _bankAccount.GetInitBalanceListByPage(filter, null).ResultData;
			if (dataGridJson == null)
			{
				dataGridJson = new DataGridJson<BDBankInitBalanceModel>();
			}
			return base.Json(dataGridJson);
		}

		public ActionResult BankAccountBalancesEdit(string accountId, string currencyId, string bankItemId)
		{
			BDBankInitBalanceModel resultData = _bankAccount.GetBDBankInitBalance(bankItemId, null).ResultData;
			if (resultData != null)
			{
				base.ViewData["balancefor"] = resultData.MBeginBalanceFor;
				base.ViewData["balance"] = resultData.MBeginBalance;
				base.ViewData["accountId"] = resultData.MAccountID;
				base.ViewData["mcyId"] = resultData.MCyID;
				base.ViewData["name"] = resultData.MBankName;
				base.ViewData["id"] = resultData.MID;
			}
			else
			{
				BDBankAccountEditModel resultData2 = _bankAccount.GetBDBankAccountEditModel(bankItemId, null).ResultData;
				base.ViewData["balance"] = 0;
				base.ViewData["accountId"] = accountId;
				base.ViewData["mcyId"] = resultData2.MCyID;
				base.ViewData["name"] = resultData2.MName;
			}
			base.ViewData["baseCurrency"] = ContextHelper.MContext.MBasCurrencyID;
			return base.View();
		}

		[Permission("Setting", "Change", "")]
		public JsonResult UpdateBankInitBalance(BDBankInitBalanceModel model)
		{
			MActionResult<OperationResult> data = _bankAccount.UpdateBankInitBalance(model, null);
			return base.Json(data);
		}
	}
}
