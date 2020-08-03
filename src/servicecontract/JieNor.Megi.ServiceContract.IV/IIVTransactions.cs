using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Param;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.IV
{
	[ServiceContract]
	public interface IIVTransactions
	{
		[OperationContract]
		MActionResult<DataGridJson<IVAccountTransactionsModel>> GetTransactionsList(IVAccountTransactionsListFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<IVAccountTransactionsExportModel> GetTransactionListForExport(IVAccountTransactionsListFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeteleTransactions(List<IVAccountTransactionsModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateReconcileStatu(IVTransactionsReconcileParam param, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVBankStatementsModel>> GetBankStatementsList(IVTransactionQueryParamModel param, string accessToken = null);

		[OperationContract]
		MActionResult<IVBankStatementsModel> GetBankStatementModel(string MID, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVBankStatementViewModel>> GetBankStatementView(string statementID, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<IVBankStatementDetailModel>> GetBankStatementDetailList(IVBankStatementDetailFilter filter, string accessToken = null);

		[OperationContract]
		MActionResult<int> StatementStatusUpdate(string selectIds, int directType, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> StatementUpdate(List<IVBankStatementViewModel> models, string accessToken = null);

		[OperationContract]
		MActionResult<BDBankAccountModel> GetBankAccountModel(string bankNo, string accessToken = null);

		[OperationContract]
		MActionResult<BDBankTypeModel> GetBankTypeModel(string id, string accessToken = null);

		[OperationContract]
		MActionResult<List<BankBillImportSolutionModel>> GetBankBillImportSolutionList(string bankTypeId, string accessToken = null);

		[OperationContract]
		MActionResult<BankBillImportSolutionModel> GetBankBillImportSolutionModel(string MItemID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveBankBillImportSolution(BankBillImportSolutionModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveImportBankBill(IVBankBillModel model, DataTable importData, string accessToken = null);
	}
}
