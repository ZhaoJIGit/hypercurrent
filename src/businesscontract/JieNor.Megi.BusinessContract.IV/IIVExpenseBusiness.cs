using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.IV
{
	public interface IIVExpenseBusiness
	{
		IVExpenseModel GetExpenseEditModel(MContext ctx, string pkID);

		IVExpenseModel GetExpenseCopyModel(MContext ctx, string pkID);

		OperationResult AddExpenseNoteLog(MContext ctx, IVExpenseModel model);

		DataGridJson<IVExpenseListModel> GetExpenseList(MContext ctx, IVExpenseListFilterModel param);

		List<IVExpenseModel> GetExpenseListForPrint(MContext ctx, IVExpenseListFilterModel param);

		List<IVExpenseModel> GetExpenseListForExport(MContext ctx, IVExpenseListFilterModel param);

		OperationResult UpdateExpense(MContext ctx, IVExpenseModel model);

		OperationResult UnApproveExpense(MContext ctx, string expenseId);

		OperationResult ApproveExpense(MContext ctx, ParamBase param);

		OperationResult UpdateExpenseStatus(MContext ctx, ParamBase param);

		OperationResult DeleteExpenseList(MContext ctx, ParamBase param);

		OperationResult AddExpensePayment(MContext ctx, List<IVMakePaymentModel> makePaymentList);

		List<IVExpenseModel> GetModelList(MContext ctx, ParamBase param);

		IVExpenseSummaryModel GetExpenseSummaryModel(MContext ctx, DateTime startDate, DateTime endDate);

		string GetChartStackedDictionary(MContext ctx, string statisticType, DateTime startDate, DateTime endDate);

		List<IVExpenseModel> GetExpenseListIncludeEntry(MContext ctx, IVExpenseListFilterModel filter);
	}
}
