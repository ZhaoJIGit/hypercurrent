using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IO.Import.Account;
using JieNor.Megi.EntityModel.BD.AccountItem;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDAccountBusiness : IDataContract<BDAccountModel>
	{
		List<BDAccountGroupEditModel> GetBDAccountGroupList(MContext ctx, string filterString);

		List<BDAccountListModel> GetCurrentAccountInfo(MContext ctx);

		List<BDAccountModel> GetCurrentAccountBaseData(MContext ctx);

		List<BDAccountTypeListModel> GetBDAccountTypeList(MContext ctx, string filterString);

		List<BDAccountListModel> GetBDAccountList(MContext ctx, string filterString);

		List<BDAccountModel> GetBDAccountList(MContext ctx, SqlWhere filter, bool includeParent = false);

		List<BDAccountModel> GetBDAccountList(MContext ctx, BDAccountListFilterModel filter, bool includeParent = false, bool isParentName = false);

		List<BDAccountListModel> GetAccountListIncludeBalance(MContext ctx, SqlWhere filter, bool includeParent = false);

		List<BDAccountListModel> GetAccountList(MContext ctx, SqlWhere filter);

		OperationResult UpdateAccount(MContext ctx, BDAccountEditModel model);

		OperationResult UpdateAccountType(MContext ctx, BDAccountTypeEditModel model);

		BDAccountEditModel GetBDAccountEditModel(MContext ctx, string pkID, string parentId);

		JieNor.Megi.DataModel.BD.BDAccountViewModel GetBDAccountViewModel(MContext ctx, string pkID);

		BDAccountTypeEditModel GetBDAccountTypeEditModel(MContext ctx, string pkID);

		BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param);

		OperationResult DeleteAccount(MContext ctx, ParamBase param);

		OperationResult DeleteAccount(MContext ctx, string pkID);

		OperationResult ArchiveAccount(MContext ctx, ParamBase param);

		OperationResult UnArchiveAccount(MContext ctx, ParamBase param);

		bool IsCodeExists(MContext ctx, string id, BDAccountTypeEditModel model);

		List<GLBalanceModel> GetBalance(MContext ctx, string itemid = "");

		List<GLInitBalanceModel> GetInitBalanceList(MContext ctx, SqlWhere filter = null);

		DataGridJson<GLInitBalanceModel> GetInitBalanceListByPage(MContext ctx, GLBalanceListFilterModel param);

		OperationResult UpdateInitBalance(MContext ctx, List<GLInitBalanceModel> modelList, string contactId);

		List<AccountItemTreeModel> GetAccountItemTreeList(MContext ctx, BDAccountListFilterModel filter = null);

		List<BDAccountListModel> GetAccountListForExport(MContext ctx, BDAccountListFilterModel filter);

		List<AccountInitBalanceTreeModel> GetInitBalanceTreeList(MContext ctx, string cyId, SqlWhere filter);

		TrialInitBalanceModel TrialInitBalance(MContext ctx);

		OperationResult AddAccountingProject(MContext ctx, GLInitBalanceModel model);

		OperationResult InitBalanceFinish(MContext ctx);

		OperationResult ReInitBalance(MContext ctx);

		ImportTemplateModel GetImportTemplateModel(MContext ctx, IOImportAccountFilterModel filter);

		OperationResult ImportAccountList(MContext ctx, List<BDAccountEditModel> list);

		OperationResult ImportCustomAccountList(MContext ctx, List<BDAccountEditModel> list);

		OperationResult AddAccountCheck(MContext ctx, BDAccountEditModel model);

		OperationResult CheckBalanceEqualWithBill(MContext ctx);

		OperationResult CheckCustomAccountIsFinish(MContext ctx);

		OperationResult CheckCustomAccountIsMatch(MContext ctx);

		OperationResult CustomAccountFinishMatch(MContext ctx, List<BDAccountModel> accountList);

		OperationResult InsertDefaultAccount(MContext ctx);

		IOAccountMatchModel GetImportAccountMatchResult(MContext ctx, List<BDAccountEditModel> acctList);

		List<IOAccountModel> PreviewAccountMatch(MContext ctx, List<BDAccountEditModel> acctList);

		List<BDAccountModel> GetAccountListWithCheckType(MContext ctx, string itemID = null, bool includeParent = false);

		OperationResult CheckCheckTypeIsUsed(MContext ctx, string accountId, int checkTypeEnum);

		OperationResult DeleteCheckType(MContext ctx, string acccountId, int checkTypeEnum);

		OperationResult SetAccountCreateInitBill(MContext ctx, string accountId, bool createInitBill);

		OperationResult ValidataData(MContext ctx, List<BDAccountEditModel> acctList);
	}
}
