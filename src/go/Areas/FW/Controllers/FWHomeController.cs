using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.FP;
using JieNor.Megi.ServiceContract.IO;
using JieNor.Megi.ServiceContract.IV;
using JieNor.Megi.ServiceContract.SEC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FW.Controllers
{
	public class FWHomeController : GoControllerBase
	{
		private IBDBankAccount _bankAccount;

		private IIVInvoice _invoice;

		private IIVExpense _expenseService;

		private IBASSearch _sea = null;

		private IFPTable FapiaoTableServer;

		private IIOImport _import = null;

		private ISECUser _userService;

		private readonly ISECUserAccount _userAccount;

		private readonly ISECUserLoginLog _loginLogService;

		private IBASMyHome _org = null;

		private IBASOrganisation _orgService = null;

		public FWHomeController(ISECUserAccount uacct, ISECUserLoginLog logService, IBDBankAccount bankAccount, IIVInvoice invoice, IIVExpense expense, IBASSearch sea, IIOImport import, IFPTable fapiaoTableServer, IBASMyHome org, ISECUser user, IBASOrganisation orgServer)
		{
			_bankAccount = bankAccount;
			_invoice = invoice;
			_expenseService = expense;
			_userAccount = uacct;
			_loginLogService = logService;
			_sea = sea;
			_import = import;
			_org = org;
			FapiaoTableServer = fapiaoTableServer;
			_userService = user;
			_orgService = orgServer;
		}

		public ActionResult FWDashboard(int type)
		{
			string empty = string.Empty;
			switch (type)
			{
			case 1:
				empty = "/BD/BDBank/BDBankHome";
				break;
			case 2:
				empty = "/IV/Invoice/InvoiceList";
				break;
			case 3:
				empty = "/IV/Bill/BillList";
				break;
			case 4:
				empty = "/IV/Expense/ExpenseList";
				break;
			case 5:
				empty = "/Report/";
				break;
			default:
				empty = "FWDashboard";
				break;
			}
			return base.View(empty);
		}

		public ActionResult Index()
		{
			MContext mContext = ContextHelper.MContext;
			mContext.MServerType = 1;
			ContextHelper.MContext = mContext;
			base.ViewData["UserCreateDate"] = mContext.MUserCreateDate;
			base.ViewData["MLocaleID"] = mContext.MLCID;
			return base.View();
		}

		public ActionResult GetBankDashbardData(string[] accountList, int chartType)
		{
			MContext mContext = ContextHelper.MContext;
			DateTime dateTime = (mContext.DateNow < mContext.MBeginDate) ? mContext.MBeginDate : mContext.DateNow;
			DateTime dateTime2 = dateTime.AddMonths(-5);
			int year = dateTime2.Year;
			dateTime2 = dateTime.AddMonths(-5);
			DateTime value = new DateTime(year, dateTime2.Month, 1);
			dateTime2 = new DateTime(dateTime.Year, dateTime.Month, 1);
			dateTime2 = dateTime2.AddMonths(1);
			DateTime value2 = dateTime2.AddDays(-1.0).ToDayLastSecond();
			List<BDBankAccountEditModel> list = new List<BDBankAccountEditModel>();
			switch (chartType)
			{
			case 0:
			{
				List<BDBankAccountEditModel> collection = _bankAccount.GetBDBankDashboardData(value, value2, null).ResultData.ToList();
				list.AddRange(collection);
				break;
			}
			}
			return base.Json(list, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetSaleDashboardData(DateTime startDate, DateTime endDate, int chartType, string dataType)
		{
			startDate = DateTime.MinValue;
			endDate = DateTime.MaxValue;
			string chartData = string.Empty;
			IVInvoiceSummaryModel resultData = _invoice.GetInvoiceSummaryModel("Invoice_Sale", startDate, endDate, null).ResultData;
			switch (chartType)
			{
			case 1:
				chartData = _invoice.GetChartStackedDictionary(dataType, startDate, endDate, null, null).ResultData;
				break;
			}
			return base.Json(new
			{
				ChartData = chartData,
				TableData = resultData
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetPurchaseDashboardData(DateTime startDate, DateTime endDate, int chartType, string dataType)
		{
			startDate = DateTime.MinValue;
			endDate = DateTime.MaxValue;
			string chartData = string.Empty;
			IVInvoiceSummaryModel resultData = _invoice.GetInvoiceSummaryModel("Invoice_Purchase", startDate, endDate, null).ResultData;
			switch (chartType)
			{
			case 1:
				chartData = _invoice.GetChartStackedDictionary(dataType, startDate, endDate, null, null).ResultData;
				break;
			}
			return base.Json(new
			{
				ChartData = chartData,
				TableData = resultData
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetExpenseDashboardData(DateTime startDate, DateTime endDate, int chartType)
		{
			startDate = DateTime.MinValue;
			endDate = ContextHelper.MContext.DateNow.ToDayLastSecond();
			string chartData = string.Empty;
			IVExpenseSummaryModel resultData = _expenseService.GetExpenseSummaryModel(startDate, endDate, null).ResultData;
			switch (chartType)
			{
			case 1:
				chartData = _expenseService.GetChartStackedDictionary("2", startDate, endDate, null).ResultData;
				break;
			}
			return base.Json(new
			{
				ChartData = chartData,
				TableData = resultData
			});
		}

		public ActionResult CheckToken(string _eme = null, string _pmp = null, string _tmt = null, string _omo = null, string _lml = null)
		{
			return null;
		}

		public ActionResult LogException(string message)
		{
			return null;
		}

		public ActionResult LoginBoxSignIn(string _eme = null, string _pmp = null, string _tmt = null, string _omo = null, string _lml = null)
		{
			return null;
		}

		public JsonResult GetSearchBizData(BASSearchFilterModel filter)
		{
			return base.Json(_sea.GetSearchResult(filter, null));
		}

		public ActionResult IsBalanceInitOver()
		{
			return base.Json(ContextHelper.MContext.MInitBalanceOver);
		}

		[HttpPost]
		public ActionResult GetOrgList()
		{
			MActionResult<List<BASMyHomeModel>> orgInfoListByUserID = _org.GetOrgInfoListByUserID(null);
			MContext mContext = ContextHelper.MContext;
			ContextHelper.SaveOrgInfoToCookie(mContext?.MOrgID, mContext?.MOrgName, base.HttpContext);
			return base.Json(new
			{
				Data = orgInfoListByUserID.ResultData,
				Context = mContext
			});
		}

		public ActionResult ValidateCreateOrgAuth(int type, bool checkBeta = false)
		{
			MActionResult<OperationResult> data = _userService.ValidateCreateOrgAuth(type, null);
			return base.Json(data);
		}

		public ActionResult ValidateJumpOrg(string id)
		{
			BASOrganisationModel resultData = _orgService.GetModelById(id, null).ResultData;
			MActionResult<OperationResult> data = _userService.ValidateCreateOrgAuth(resultData.MVersionID, null);
			return base.Json(data);
		}

		public ActionResult ValidateBeta(bool isBeta)
		{
			bool flag = Convert.ToBoolean(ConfigurationManager.AppSettings["IsBetaSite"]);
			if ((flag && !isBeta) || (!flag & isBeta))
			{
				return base.Json(true);
			}
			return base.Json(false);
		}

		public ActionResult GetFapiaoDashboardData(DateTime? startDate, DateTime? endDate, int type)
		{
			DateTime endDate2 = (!endDate.HasValue) ? ContextHelper.MContext.DateNow : endDate.Value;
			DateTime startDate2 = (!startDate.HasValue) ? endDate2.AddMonths(-5) : startDate.Value;
			startDate2 = new DateTime(startDate2.Year, startDate2.Month, 1);
			MActionResult<string> chartStackedDictionary = FapiaoTableServer.GetChartStackedDictionary(type, startDate2, endDate2, null);
			return base.Json(chartStackedDictionary);
		}
	}
}
