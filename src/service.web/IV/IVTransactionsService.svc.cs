using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Param;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.IV;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.Service.Web.IV
{
	public class IVTransactionsService : ServiceT<IVAccountTransactionsModel>, IIVTransactions
	{
		private readonly IIVTransactionsBusiness biz = new IVTransactionsBusiness();

		public MActionResult<DataGridJson<IVAccountTransactionsModel>> GetTransactionsList(IVAccountTransactionsListFilterModel param, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.GetTransactionsList, param, accessToken);
		}

		public MActionResult<IVAccountTransactionsExportModel> GetTransactionListForExport(IVAccountTransactionsListFilterModel filter, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.GetTransactionListForExport, filter, accessToken);
		}

		public MActionResult<OperationResult> DeteleTransactions(List<IVAccountTransactionsModel> list, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.DeteleTransactions, list, accessToken);
		}

		public MActionResult<OperationResult> UpdateReconcileStatu(IVTransactionsReconcileParam param, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.UpdateReconcileStatu, param, accessToken);
		}

		public MActionResult<List<IVBankStatementsModel>> GetBankStatementsList(IVTransactionQueryParamModel param, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.GetBankStatementsList, param, accessToken);
		}

		public MActionResult<IVBankStatementsModel> GetBankStatementModel(string MID, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.GetBankStatementModel, MID, accessToken);
		}

		public MActionResult<List<IVBankStatementViewModel>> GetBankStatementView(string statementID, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.GetBankStatementView, statementID, accessToken);
		}

		public MActionResult<DataGridJson<IVBankStatementDetailModel>> GetBankStatementDetailList(IVBankStatementDetailFilter filter, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.GetBankStatementDetailList, filter, null);
		}

		public MActionResult<int> StatementStatusUpdate(string selectIds, int directType, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.StatementStatusUpdate, selectIds, directType, accessToken);
		}

		public MActionResult<OperationResult> StatementUpdate(List<IVBankStatementViewModel> models, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.StatementUpdate, models, accessToken);
		}

		public MActionResult<BDBankAccountModel> GetBankAccountModel(string bankNo, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.GetBankAccountModel, bankNo, accessToken);
		}

		public MActionResult<BDBankTypeModel> GetBankTypeModel(string id, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.GetBankTypeModel, id, accessToken);
		}

		public MActionResult<List<BankBillImportSolutionModel>> GetBankBillImportSolutionList(string bankTypeId, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.GetBankBillImportSolutionList, bankTypeId, accessToken);
		}

		public MActionResult<BankBillImportSolutionModel> GetBankBillImportSolutionModel(string MItemID, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.GetBankBillImportSolutionModel, MItemID, accessToken);
		}

		public MActionResult<OperationResult> SaveBankBillImportSolution(BankBillImportSolutionModel model, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.SaveBankBillImportSolution, model, accessToken);
		}

		public MActionResult<OperationResult> SaveImportBankBill(IVBankBillModel model, DataTable importData, string accessToken = null)
		{
			IIVTransactionsBusiness iIVTransactionsBusiness = biz;
			return base.RunFunc(iIVTransactionsBusiness.SaveImportBankBill, model, importData, accessToken);
		}
	}
}
