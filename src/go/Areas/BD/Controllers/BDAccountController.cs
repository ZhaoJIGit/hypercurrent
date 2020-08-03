using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import.Account;
using JieNor.Megi.EntityModel.BD.AccountItem;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.Go.HtmlHelper;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.GL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class BDAccountController : GoControllerBase
	{
		private IBDAccount _bdAccount;

		private IGLInitBalance GLInitBalanceServie;

		private IBDAccountMatchLog _matchLog;

		private IGLCheckType _checkType;

		public BDAccountController(IBDAccount bdAccount, IGLInitBalance glInitBalanceService, IBDAccountMatchLog matchLog, IGLCheckType checkType)
		{
			_bdAccount = bdAccount;
			GLInitBalanceServie = glInitBalanceService;
			_matchLog = matchLog;
			_checkType = checkType;
		}

		[Permission("Setting", "View", "")]
		public ActionResult AccountList()
		{
			List<BDAccountGroupEditModel> resultData = _bdAccount.GetBDAccountGroupList("", null).ResultData;
			ViewData["AccountGroup"] = resultData;
			ViewData["IsGLPermission"] = HtmlSECMenu.HavePermission("General_Ledger", "Change", "");
			ViewData["AccountStandard"] = base.MContext.MAccountTableID;
			ViewData["IsCreate"] = true;
			if (MContext.MAccountTableID == "3")
			{
				OperationResult resultData2 = _bdAccount.CheckCustomAccountIsMatch(null).ResultData;
				ViewData["IsMatch"] = resultData2.Success;
			}
			else
			{
				ViewData["IsMatch"] = true;
			}
			ViewBag.ExistMatchLog = _matchLog.ExistsByFilter(new SqlWhere(), null).ResultData;

			return View();
		}

		public ActionResult GetAccountGroupList()
		{
			MActionResult<List<BDAccountGroupEditModel>> bDAccountGroupList = _bdAccount.GetBDAccountGroupList("", null);
			return base.Json(bDAccountGroupList);
		}

		public ActionResult AccountEdit(string id, string parentId, string parentName, int mdc = 1, bool isLeaf = false, bool isCombox = false)
		{
			if (!isCombox)
			{
				BDAccountEditModel resultData = _bdAccount.GetBDAccountEditModel(id, parentId, null).ResultData;
				base.ViewData["AccountID"] = id;
				base.ViewData["ParentID"] = parentId;
				base.ViewData["IsCanRelateContact"] = resultData.IsCanRelateContact;
				base.ViewData["MDC"] = mdc;
				base.ViewData["ParentName"] = parentName;
				base.ViewData["AllowEdit"] = resultData.MIsAllowEdit;
			}
			else
			{
				base.ViewData["isCombox"] = isCombox;
			}
			base.ViewData["IsLeaf"] = isLeaf;
			base.ViewData["AccountStandard"] = ContextHelper.MContext.MAccountTableID;
			base.ViewData["LangeID"] = ContextHelper.MContext.MLCID;
			return base.View();
		}

		public ActionResult GetAccountInfo(string id)
		{
			MActionResult<BDAccountEditModel> bDAccountEditModel = _bdAccount.GetBDAccountEditModel(id, null, null);
			return base.Json(bDAccountEditModel);
		}

		[Permission("Setting", "Change", "")]
		public ActionResult CheckBalanceEqualWithBill()
		{
			MActionResult<OperationResult> data = _bdAccount.CheckBalanceEqualWithBill(null);
			return base.Json(data);
		}

		[Permission("Setting", "View", "")]
		public JsonResult GetAccountBalancesByPage(string accountId, GLBalanceListFilterModel param)
		{
			DataGridJson<GLInitBalanceModel> dataGridJson = _bdAccount.GetInitBalanceListByPage(param, null).ResultData;
			if (dataGridJson == null)
			{
				dataGridJson = new DataGridJson<GLInitBalanceModel>();
			}
			return base.Json(dataGridJson);
		}

		public JsonResult GetAccountInitBalanceTreeList(string cyId, bool includeContact = false)
		{
			SqlWhere sqlWhere = new SqlWhere();
			if (!includeContact)
			{
				sqlWhere.Equal("MContactID", "0");
			}
			if (!string.IsNullOrEmpty(cyId) && cyId != "0")
			{
				sqlWhere.Like("MCurrencyID", cyId);
			}
			MActionResult<List<AccountInitBalanceTreeModel>> initBalanceTreeList = _bdAccount.GetInitBalanceTreeList(cyId, sqlWhere, null);
			return base.Json(initBalanceTreeList);
		}

		public JsonResult GetAccountList(BDAccountListFilterModel filter)
		{
			MActionResult<List<AccountItemTreeModel>> accountTreeList = _bdAccount.GetAccountTreeList(filter, null);
			return base.Json(accountTreeList);
		}

		public JsonResult AddAcountCheck(BDAccountEditModel model)
		{
			MActionResult<OperationResult> data = _bdAccount.AddAccountCheck(model, null);
			return base.Json(data);
		}

		public JsonResult SaveAccount(BDAccountEditModel model)
		{
			MActionResult<OperationResult> data = _bdAccount.UpdateAccount(model, null);
			return base.Json(data);
		}

		public JsonResult GetAccount(string MItemID, string MParentID)
		{
			MActionResult<BDAccountEditModel> bDAccountEditModel = _bdAccount.GetBDAccountEditModel(MItemID, MParentID, null);
			return base.Json(bDAccountEditModel);
		}

		public JsonResult GetBaseAccountList(bool isIncludeParent = false)
		{
			SqlWhere filter = new SqlWhere();
			MActionResult<List<BDAccountModel>> baseBDAccountList = _bdAccount.GetBaseBDAccountList(filter, isIncludeParent, null);
			return base.Json(baseBDAccountList);
		}

		public JsonResult GetBDAccountListByCode(BDAccountListFilterModel filter)
		{
			MActionResult<List<BDAccountModel>> bDAccountListByCode = _bdAccount.GetBDAccountListByCode(filter, false, null);
			return base.Json(bDAccountListByCode);
		}

		public JsonResult GetCurrentAccountInfo()
		{
			MActionResult<List<BDAccountListModel>> currentAccountInfo = _bdAccount.GetCurrentAccountInfo(null);
			return base.Json(currentAccountInfo);
		}

		public JsonResult GetCurrentAccountBaseData()
		{
			MActionResult<List<BDAccountModel>> currentAccountBaseData = _bdAccount.GetCurrentAccountBaseData(null);
			return base.Json(currentAccountBaseData);
		}

		public JsonResult GetAccountListIncludeBalance(bool isIncludeParent = false)
		{
			MActionResult<List<BDAccountListModel>> accountListIncludeBalance = _bdAccount.GetAccountListIncludeBalance(new SqlWhere(), isIncludeParent, null);
			return base.Json(accountListIncludeBalance);
		}

		public JsonResult IsCanDeleteOrInactive(ParamBase param)
		{
			MActionResult<BDIsCanDeleteModel> data = _bdAccount.IsCanDeleteOrInactive(param, null);
			return base.Json(data);
		}

		public JsonResult DeleteAccount(ParamBase param)
		{
			param.OrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _bdAccount.DeleteAccount(param, null);
			return base.Json(data);
		}

		public JsonResult ArchiveAccount(ParamBase param)
		{
			param.OrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _bdAccount.ArchiveAccount(param, null);
			return base.Json(data);
		}

		public JsonResult RestoreAccount(ParamBase param)
		{
			param.OrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _bdAccount.UnArchiveAccount(param, null);
			return base.Json(data);
		}

		public JsonResult IsCodeExists(string id, BDAccountTypeEditModel model)
		{
			MActionResult<bool> data = _bdAccount.IsCodeExists(id, model, null);
			return base.Json(data);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult UpdateInitBalance(List<GLInitBalanceModel> modelList, string contactId)
		{
			return base.Json(_bdAccount.UpdateInitBalance(modelList, contactId, null));
		}

		public ActionResult AccountBalancesEdit(string accountId, string accoutName, string cyId, string exchange)
		{
			base.ViewData["name"] = accoutName;
			base.ViewData["accountId"] = accountId;
			base.ViewData["cyId"] = cyId;
			base.ViewData["exchange"] = exchange;
			cyId = (string.IsNullOrEmpty(cyId) ? "0" : cyId);
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MCurrencyID", cyId);
			List<GLInitBalanceModel> resultData = _bdAccount.GetInitBalanceList(sqlWhere, null).ResultData;
			GLInitBalanceModel gLInitBalanceModel = (from x in resultData
			where x.MAccountID == accountId
			select x).FirstOrDefault();
			if (gLInitBalanceModel != null)
			{
				base.ViewData["balanceId"] = gLInitBalanceModel.MItemID;
				base.ViewData["initBalance"] = gLInitBalanceModel.MInitBalance;
				base.ViewData["initBalanceFor"] = gLInitBalanceModel.MInitBalanceFor;
				base.ViewData["accumulatedDebit"] = gLInitBalanceModel.MYtdDebit;
				base.ViewData["accumulatedCredit"] = gLInitBalanceModel.MYtdCredit;
				base.ViewData["accountId"] = gLInitBalanceModel.MAccountID;
				base.ViewData["mcyId"] = gLInitBalanceModel.MCurrencyID;
			}
			return base.View();
		}

		public ActionResult InitBalancesEdit(string accountId, string balanceId, string cyId, string exchange)
		{
			return base.View("AccountBalancesEdit");
		}

		public ActionResult TrialInitBalance()
		{
			int month = ContextHelper.MContext.MGLBeginDate.Month;
			base.ViewData["GLMonth"] = month;
			return base.View();
		}

		public JsonResult CheckTrialInitBalance()
		{
			return base.Json(_bdAccount.TrialInitBalance(null));
		}

		public ActionResult CheckInitBalance()
		{
			MActionResult<TrialInitBalanceModel> data = _bdAccount.TrialInitBalance(null);
			return base.Json(data);
		}

		public ActionResult AddAccountingProject(string cyId, string accountId, string accountName)
		{
			base.ViewData["CyID"] = cyId;
			base.ViewData["AccountId"] = accountId;
			base.ViewData["AccountName"] = accountName;
			return base.View();
		}

		public ActionResult UpdateAccountingProject(GLInitBalanceModel model)
		{
			return base.Json(_bdAccount.AddAccountingProject(model, null));
		}

		public ActionResult InitBalanceFinish()
		{
			return base.Json(_bdAccount.InitBalanceFinish(null));
		}

		public ActionResult ReInitBalance()
		{
			return base.Json(_bdAccount.ReInitBalance(null));
		}

		public FileResult Export(string jsonParam)
		{
			Stream stream = ExportToImportHelper.CreateExportObjStream(BizReportType.Accounts, jsonParam);
			string exportName = string.Format("{0} - {1}.xls", ContextHelper.MContext.MOrgName, HtmlLang.GetText(LangModule.BD, "AccountItem", "Account"));
			return base.ExportReport(stream, exportName);
		}

		public FileResult ExportOpeningBalances()
		{
			Stream stream = ExportToImportHelper.CreateExportObjStream(BizReportType.OpeningBalances, null);
			string exportName = string.Format("{0} - {1}.xls", ContextHelper.MContext.MOrgName, HtmlLang.GetText(LangModule.BD, "AccountBalancesFinancial", "科目初始余额"));
			return base.ExportReport(stream, exportName);
		}

		public ActionResult GetAccountListWithCheckType(string itemID = null)
		{
			MActionResult<List<BDAccountModel>> accountListWithCheckType = _bdAccount.GetAccountListWithCheckType(itemID, false, null);
			return base.Json(accountListWithCheckType);
		}

		public ActionResult CheckCustomAccountIsFinish()
		{
			MActionResult<OperationResult> data = _bdAccount.CheckCustomAccountIsFinish(null);
			return base.Json(data);
		}

		public ActionResult CheckCustomAccountIsMatch()
		{
			MActionResult<OperationResult> data = _bdAccount.CheckCustomAccountIsMatch(null);
			return base.Json(data);
		}

		public ActionResult CustomAccountMatch()
		{
			return base.View();
		}

		public ActionResult CustomAccountFinishMatch(List<BDAccountModel> list)
		{
			MActionResult<OperationResult> data = _bdAccount.CustomAccountFinishMatch(list, null);
			return base.Json(data);
		}

		public ActionResult InsertDefaultAccount()
		{
			MActionResult<OperationResult> data = _bdAccount.InsertDefaultAccount(null);
			return base.Json(data);
		}

		public JsonResult SaveAccountMatchResult(List<BDAccountEditModel> list)
		{
			MActionResult<OperationResult> data = _bdAccount.ImportAccountList(list, null);
			return base.Json(data);
		}

		public JsonResult PreviewAccountMatch(string json)
		{
			List<BDAccountEditModel> acctList = JsonConvert.DeserializeObject<List<BDAccountEditModel>>(HttpUtility.UrlDecode(json));
			MActionResult<List<IOAccountModel>> data = _bdAccount.PreviewAccountMatch(acctList, null);
			return base.Json(data);
		}

		public ActionResult AccountMatchLog()
		{
			return base.View();
		}

		public JsonResult GetAccountMatchLog()
		{
			return base.Json(_matchLog.GetMatchLogList(null));
		}

		public JsonResult SaveAccountMatchLog(List<BDAccountEditModel> acctList)
		{
			return base.Json(_matchLog.SaveMatchLog(acctList, null));
		}

		[Permission("Setting", "View", "")]
		public ActionResult GetCheckTypeList()
		{
			MActionResult<List<GLCheckTypeModel>> modelList = _checkType.GetModelList(null, false, null);
			return base.Json(modelList);
		}

		[Permission("Setting", "View", "")]
		public ActionResult CheckTypeIsUsed(string accountId, int checkType)
		{
			MActionResult<OperationResult> data = _bdAccount.CheckCheckTypeIsUsed(accountId, checkType, null);
			return base.Json(data);
		}

		[Permission("Setting", "View", "")]
		public ActionResult DeleteCheckType(string accountId, int checkType)
		{
			MActionResult<OperationResult> data = _bdAccount.DeleteCheckType(accountId, checkType, null);
			return base.Json(data);
		}

		[Permission("Setting", "View", "")]
		public ActionResult SetAccountCreateInitBill(string accountId, bool createInitBill)
		{
			MActionResult<OperationResult> data = _bdAccount.SetAccountCreateInitBill(accountId, createInitBill, null);
			return base.Json(data);
		}
	}
}
