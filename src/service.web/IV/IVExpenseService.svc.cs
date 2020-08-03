using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.IV
{
	public class IVExpenseService : ServiceT<IVExpenseModel>, IIVExpense
	{
		private readonly IIVExpenseBusiness biz = new IVExpenseBusiness();

		public MActionResult<IVExpenseModel> GetExpenseEditModel(string pkID, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.GetExpenseEditModel, pkID, accessToken);
		}

		public MActionResult<OperationResult> AddExpenseNoteLog(IVExpenseModel model, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.AddExpenseNoteLog, model, accessToken);
		}

		public MActionResult<IVExpenseModel> GetExpenseCopyModel(string pkID, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.GetExpenseCopyModel, pkID, accessToken);
		}

		public MActionResult<DataGridJson<IVExpenseListModel>> GetExpenseList(IVExpenseListFilterModel filter, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.GetExpenseList, filter, accessToken);
		}

		public MActionResult<List<IVExpenseModel>> GetExpenseListForPrint(IVExpenseListFilterModel filter, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.GetExpenseListForPrint, filter, accessToken);
		}

		public MActionResult<List<IVExpenseModel>> GetExpenseListForExport(IVExpenseListFilterModel filter, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.GetExpenseListForExport, filter, accessToken);
		}

		public MActionResult<OperationResult> UpdateExpense(IVExpenseModel model, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.UpdateExpense, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateExpenseStatus(ParamBase param, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.UpdateExpenseStatus, param, accessToken);
		}

		public MActionResult<OperationResult> UnApproveExpense(string expenseId, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.UnApproveExpense, expenseId, accessToken);
		}

		public MActionResult<OperationResult> ApproveExpense(ParamBase param, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.ApproveExpense, param, accessToken);
		}

		public MActionResult<OperationResult> DeleteExpenseList(ParamBase param, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.DeleteExpenseList, param, accessToken);
		}

		public MActionResult<OperationResult> AddExpensePayment(List<IVMakePaymentModel> makePaymentList, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.AddExpensePayment, makePaymentList, accessToken);
		}

		public MActionResult<List<IVExpenseModel>> GetModelList(ParamBase param, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.GetModelList, param, accessToken);
		}

		public MActionResult<IVExpenseSummaryModel> GetExpenseSummaryModel(DateTime startDate, DateTime endDate, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.GetExpenseSummaryModel, startDate, endDate, accessToken);
		}

		public MActionResult<string> GetChartStackedDictionary(string Type, DateTime startDate, DateTime endDate, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.GetChartStackedDictionary, Type, startDate, endDate, accessToken);
		}

		public MActionResult<List<IVExpenseModel>> GetExpenseListIncludeEntry(IVExpenseListFilterModel filter, string accessToken = null)
		{
			IIVExpenseBusiness iIVExpenseBusiness = biz;
			return base.RunFunc(iIVExpenseBusiness.GetExpenseListIncludeEntry, filter, accessToken);
		}
	}
}
