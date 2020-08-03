using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.IV
{
	[ServiceContract]
	public interface IIVExpense
	{
		[OperationContract]
		MActionResult<IVExpenseModel> GetExpenseEditModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> AddExpenseNoteLog(IVExpenseModel model, string accessToken = null);

		[OperationContract]
		MActionResult<IVExpenseModel> GetExpenseCopyModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<IVExpenseListModel>> GetExpenseList(IVExpenseListFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVExpenseModel>> GetExpenseListForPrint(IVExpenseListFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVExpenseModel>> GetExpenseListForExport(IVExpenseListFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateExpense(IVExpenseModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UnApproveExpense(string expenseId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ApproveExpense(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateExpenseStatus(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteExpenseList(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> AddExpensePayment(List<IVMakePaymentModel> makePaymentList, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVExpenseModel>> GetModelList(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<IVExpenseSummaryModel> GetExpenseSummaryModel(DateTime startDate, DateTime endDate, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetChartStackedDictionary(string type, DateTime startDate, DateTime endDate, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVExpenseModel>> GetExpenseListIncludeEntry(IVExpenseListFilterModel filter, string accessToken = null);
	}
}
