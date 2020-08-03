using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Param;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.BusinessContract.IV
{
	public interface IIVTransactionsBusiness
	{
		DataGridJson<IVAccountTransactionsModel> GetTransactionsList(MContext ctx, IVAccountTransactionsListFilterModel param);

		IVAccountTransactionsExportModel GetTransactionListForExport(MContext ctx, IVAccountTransactionsListFilterModel param);

		OperationResult DeteleTransactions(MContext ctx, List<IVAccountTransactionsModel> list);

		OperationResult UpdateReconcileStatu(MContext ctx, IVTransactionsReconcileParam param);

		List<IVBankStatementsModel> GetBankStatementsList(MContext ctx, IVTransactionQueryParamModel param);

		IVBankStatementsModel GetBankStatementModel(MContext ctx, string MID);

		List<IVBankStatementViewModel> GetBankStatementView(MContext ctx, string statementID);

		DataGridJson<IVBankStatementDetailModel> GetBankStatementDetailList(MContext ctx, IVBankStatementDetailFilter filter);

		int StatementStatusUpdate(MContext ctx, string selectIds, int directType);

		OperationResult StatementUpdate(MContext ctx, List<IVBankStatementViewModel> models);

		BDBankAccountModel GetBankAccountModel(MContext ctx, string bankNo);

		BDBankTypeModel GetBankTypeModel(MContext ctx, string id);

		List<BankBillImportSolutionModel> GetBankBillImportSolutionList(MContext ctx, string bankTypeId);

		BankBillImportSolutionModel GetBankBillImportSolutionModel(MContext ctx, string MItemID);

		OperationResult SaveBankBillImportSolution(MContext ctx, BankBillImportSolutionModel model);

		OperationResult SaveImportBankBill(MContext ctx, IVBankBillModel model, DataTable importData);
	}
}
