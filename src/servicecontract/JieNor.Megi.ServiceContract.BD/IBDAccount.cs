using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IO.Import.Account;
using JieNor.Megi.EntityModel.BD.AccountItem;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDAccount
	{
		[OperationContract]
		MActionResult<List<BDAccountGroupEditModel>> GetBDAccountGroupList(string filterString, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAccountListModel>> GetCurrentAccountInfo(string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAccountModel>> GetCurrentAccountBaseData(string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAccountTypeListModel>> GetBDAccountTypeList(string filterString, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAccountListModel>> GetBDAccountList(string filterString, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAccountModel>> GetBDAccountListByCode(BDAccountListFilterModel filter, bool includeParent = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAccountModel>> GetBaseBDAccountList(SqlWhere filter, bool includeParent = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAccountListModel>> GetAccountListIncludeBalance(SqlWhere filter, bool includeParent = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAccountListModel>> GetAccountList(SqlWhere filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateAccount(BDAccountEditModel model, string accessToken = null);

		[OperationContract]
		MActionResult<BDAccountEditModel> GetBDAccountEditModel(string pkID, string parentId, string accessToken = null);

		[OperationContract]
		MActionResult<JieNor.Megi.DataModel.BD.BDAccountViewModel> GetBDAccountViewModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<BDAccountTypeEditModel> GetBDAccountTypeEditModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteAccount(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteAccountByPkID(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ArchiveAccount(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UnArchiveAccount(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<bool> IsCodeExists(string id, BDAccountTypeEditModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLBalanceModel>> GetBalance(string itemid = "", string accessToken = null);

		[OperationContract]
		MActionResult<List<GLInitBalanceModel>> GetInitBalanceList(SqlWhere filter = null, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<GLInitBalanceModel>> GetInitBalanceListByPage(GLBalanceListFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateInitBalance(List<GLInitBalanceModel> modelList, string contactId, string accessToken = null);

		[OperationContract]
		MActionResult<List<AccountItemTreeModel>> GetAccountTreeList(BDAccountListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAccountListModel>> GetAccountListForExport(BDAccountListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<AccountInitBalanceTreeModel>> GetInitBalanceTreeList(string cyId, SqlWhere filter, string accessToken = null);

		[OperationContract]
		MActionResult<TrialInitBalanceModel> TrialInitBalance(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> AddAccountingProject(GLInitBalanceModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InitBalanceFinish(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ReInitBalance(string accessToken = null);

		[OperationContract]
		MActionResult<ImportTemplateModel> GetImportTemplateModel(IOImportAccountFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportAccountList(List<BDAccountEditModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportCustomAccountList(List<BDAccountEditModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> AddAccountCheck(BDAccountEditModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> CheckCustomAccountIsFinish(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> CheckCustomAccountIsMatch(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> CustomAccountFinishMatch(List<BDAccountModel> accountList, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InsertDefaultAccount(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> CheckBalanceEqualWithBill(string accessToken = null);

		[OperationContract]
		MActionResult<IOAccountMatchModel> GetImportAccountMatchResult(List<BDAccountEditModel> acctList, string accessToken = null);

		[OperationContract]
		MActionResult<List<IOAccountModel>> PreviewAccountMatch(List<BDAccountEditModel> acctList, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDAccountModel>> GetAccountListWithCheckType(string itemID = null, bool includeParent = false, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> CheckCheckTypeIsUsed(string accountId, int checkTypeEnum, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteCheckType(string accountId, int checkTypeEnum, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SetAccountCreateInitBill(string accountId, bool createInitBill, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ValidataData(List<BDAccountEditModel> acctList, string accessToken = null);
	}
}
