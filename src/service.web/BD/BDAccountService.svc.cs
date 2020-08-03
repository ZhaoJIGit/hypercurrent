using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IO.Import.Account;
using JieNor.Megi.EntityModel.BD.AccountItem;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDAccountService : ServiceT<BDAccountModel>, IBDAccount
	{
		private readonly IBDAccountBusiness biz = new BDAccountBusiness();

		public MActionResult<List<BDAccountGroupEditModel>> GetBDAccountGroupList(string filterString, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetBDAccountGroupList, filterString, accessToken);
		}

		public MActionResult<List<BDAccountTypeListModel>> GetBDAccountTypeList(string filterString, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetBDAccountTypeList, filterString, accessToken);
		}

		public MActionResult<List<BDAccountListModel>> GetBDAccountList(string strWhere, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetBDAccountList, strWhere, accessToken);
		}

		public MActionResult<List<BDAccountModel>> GetBDAccountListByCode(BDAccountListFilterModel filter, bool includeParent = false, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetBDAccountList, filter, includeParent, true, accessToken);
		}

		public MActionResult<List<BDAccountModel>> GetBaseBDAccountList(SqlWhere filter, bool includeParent = false, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetBDAccountList, filter, includeParent, accessToken);
		}

		public MActionResult<List<BDAccountListModel>> GetAccountListIncludeBalance(SqlWhere filter, bool includeParent = false, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetAccountListIncludeBalance, filter, includeParent, accessToken);
		}

		public MActionResult<OperationResult> UpdateAccount(BDAccountEditModel model, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.UpdateAccount, model, accessToken);
		}

		public MActionResult<BDAccountEditModel> GetBDAccountEditModel(string pkID, string parentId, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetBDAccountEditModel, pkID, parentId, accessToken);
		}

		public MActionResult<JieNor.Megi.DataModel.BD.BDAccountViewModel> GetBDAccountViewModel(string pkID, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetBDAccountViewModel, pkID, accessToken);
		}

		public MActionResult<BDAccountTypeEditModel> GetBDAccountTypeEditModel(string pkID, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetBDAccountTypeEditModel, pkID, accessToken);
		}

		public MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.IsCanDeleteOrInactive, param, accessToken);
		}

		public MActionResult<OperationResult> DeleteAccount(ParamBase param, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.DeleteAccount, param, accessToken);
		}

		public MActionResult<OperationResult> DeleteAccountByPkID(string pkID, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.DeleteAccount, pkID, accessToken);
		}

		public MActionResult<OperationResult> ArchiveAccount(ParamBase param, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.ArchiveAccount, param, accessToken);
		}

		public MActionResult<OperationResult> UnArchiveAccount(ParamBase param, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.UnArchiveAccount, param, accessToken);
		}

		public MActionResult<bool> IsCodeExists(string id, BDAccountTypeEditModel model, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.IsCodeExists, id, model, accessToken);
		}

		public MActionResult<List<GLBalanceModel>> GetBalance(string itemid = "", string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetBalance, itemid, accessToken);
		}

		public MActionResult<List<GLInitBalanceModel>> GetInitBalanceList(SqlWhere filter = null, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetInitBalanceList, filter, accessToken);
		}

		public MActionResult<List<AccountInitBalanceTreeModel>> GetInitBalanceTreeList(string cyId, SqlWhere filter, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetInitBalanceTreeList, cyId, filter, null);
		}

		public MActionResult<DataGridJson<GLInitBalanceModel>> GetInitBalanceListByPage(GLBalanceListFilterModel param, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetInitBalanceListByPage, param, accessToken);
		}

		public MActionResult<OperationResult> UpdateInitBalance(List<GLInitBalanceModel> modelList, string contactId, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.UpdateInitBalance, modelList, contactId, accessToken);
		}

		public MActionResult<List<AccountItemTreeModel>> GetAccountTreeList(BDAccountListFilterModel filter, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetAccountItemTreeList, filter, accessToken);
		}

		public MActionResult<List<BDAccountListModel>> GetAccountListForExport(BDAccountListFilterModel filter, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetAccountListForExport, filter, accessToken);
		}

		public MActionResult<List<BDAccountListModel>> GetAccountList(SqlWhere filter, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetAccountList, filter, accessToken);
		}

		public MActionResult<TrialInitBalanceModel> TrialInitBalance(string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.TrialInitBalance, accessToken);
		}

		public MActionResult<OperationResult> AddAccountingProject(GLInitBalanceModel model, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.AddAccountingProject, model, accessToken);
		}

		public MActionResult<OperationResult> InitBalanceFinish(string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.InitBalanceFinish, accessToken);
		}

		public MActionResult<OperationResult> ReInitBalance(string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.ReInitBalance, accessToken);
		}

		public MActionResult<ImportTemplateModel> GetImportTemplateModel(IOImportAccountFilterModel filter, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetImportTemplateModel, filter, accessToken);
		}

		public MActionResult<OperationResult> ImportAccountList(List<BDAccountEditModel> list, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.ImportAccountList, list, accessToken);
		}

		public MActionResult<OperationResult> ImportCustomAccountList(List<BDAccountEditModel> list, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.ImportCustomAccountList, list, accessToken);
		}

		public MActionResult<OperationResult> AddAccountCheck(BDAccountEditModel model, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.AddAccountCheck, model, accessToken);
		}

		public MActionResult<OperationResult> CheckBalanceEqualWithBill(string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.CheckBalanceEqualWithBill, accessToken);
		}

		public MActionResult<List<BDAccountListModel>> GetCurrentAccountInfo(string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetCurrentAccountInfo, accessToken);
		}

		public MActionResult<List<BDAccountModel>> GetCurrentAccountBaseData(string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetCurrentAccountBaseData, accessToken);
		}

		public MActionResult<OperationResult> CheckCustomAccountIsFinish(string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.CheckCustomAccountIsFinish, accessToken);
		}

		public MActionResult<OperationResult> CheckCustomAccountIsMatch(string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.CheckCustomAccountIsMatch, accessToken);
		}

		public MActionResult<OperationResult> CustomAccountFinishMatch(List<BDAccountModel> accountList, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.CustomAccountFinishMatch, accountList, accessToken);
		}

		public MActionResult<OperationResult> InsertDefaultAccount(string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.InsertDefaultAccount, accessToken);
		}

		public MActionResult<IOAccountMatchModel> GetImportAccountMatchResult(List<BDAccountEditModel> acctList, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetImportAccountMatchResult, acctList, accessToken);
		}

		public MActionResult<List<IOAccountModel>> PreviewAccountMatch(List<BDAccountEditModel> acctList, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.PreviewAccountMatch, acctList, accessToken);
		}

		public MActionResult<List<BDAccountModel>> GetAccountListWithCheckType(string itemID = null, bool includeParent = false, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.GetAccountListWithCheckType, itemID, includeParent, accessToken);
		}

		public MActionResult<OperationResult> CheckCheckTypeIsUsed(string accountId, int checkTypeEnum, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.CheckCheckTypeIsUsed, accountId, checkTypeEnum, accessToken);
		}

		public MActionResult<OperationResult> DeleteCheckType(string accountId, int checkTypeEnum, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.DeleteCheckType, accountId, checkTypeEnum, accessToken);
		}

		public MActionResult<OperationResult> SetAccountCreateInitBill(string accountId, bool createInitBill, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.SetAccountCreateInitBill, accountId, createInitBill, accessToken);
		}

		public MActionResult<OperationResult> ValidataData(List<BDAccountEditModel> acctList, string accessToken = null)
		{
			IBDAccountBusiness iBDAccountBusiness = biz;
			return base.RunFunc(iBDAccountBusiness.ValidataData, acctList, accessToken);
		}
	}
}
