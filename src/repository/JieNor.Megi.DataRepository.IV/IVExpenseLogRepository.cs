using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVExpenseLogRepository
	{
		public static void AddExpenseEditLog(MContext ctx, IVExpenseModel model)
		{
			OptLogTemplate optLogTemplate = OptLogTemplate.None;
			if (model.IsNew)
			{
				optLogTemplate = GetExpenseLogAddTemplate(model.MType);
				AddExpenseEditLog(ctx, optLogTemplate, model);
				if (model.MStatus == Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment))
				{
					optLogTemplate = GetExpenseLogApproveTemplate(model.MType);
					AddExpenseEditLog(ctx, optLogTemplate, model);
				}
			}
			else
			{
				optLogTemplate = ((model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment)) ? GetExpenseLogEditTemplate(model.MType) : GetExpenseLogApproveTemplate(model.MType));
				AddExpenseEditLog(ctx, optLogTemplate, model);
			}
		}

		public static void AddExpenseDeleteLog(MContext ctx, ParamBase param)
		{
			string[] values = param.KeyIDs.Split(',');
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter("MID", SqlOperators.In, values);
			List<IVExpenseModel> dataModelList = ModelInfoManager.GetDataModelList<IVExpenseModel>(ctx, sqlWhere, false, false);
			if (dataModelList.Count != 0)
			{
				foreach (IVExpenseModel item in dataModelList)
				{
					OptLogTemplate expenseLogDeleteTemplate = GetExpenseLogDeleteTemplate(item.MType);
					OptLog.AddLog(expenseLogDeleteTemplate, ctx, item.MID, item.MReference);
				}
			}
		}

		public static void AddExpenseApprovalLog(MContext ctx, ParamBase param)
		{
			if (param.MOperationID.ToMInt32() == Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment))
			{
				string[] values = param.KeyIDs.Split(',');
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.AddFilter("MID", SqlOperators.In, values);
				List<IVExpenseModel> dataModelList = ModelInfoManager.GetDataModelList<IVExpenseModel>(ctx, sqlWhere, false, false);
				if (dataModelList.Count != 0)
				{
					foreach (IVExpenseModel item in dataModelList)
					{
						OptLogTemplate expenseLogApproveTemplate = GetExpenseLogApproveTemplate(item.MType);
						AddExpenseEditLog(ctx, expenseLogApproveTemplate, item);
					}
				}
			}
		}

		public static void AddExpenseExpectedInfoLog(MContext ctx, IVInvoiceModel model)
		{
			OptLogTemplate template = OptLogTemplate.None;
			string mType = model.MType;
			if (mType == "Expense_Claims")
			{
				template = OptLogTemplate.ExpenseClaims_NoteExpectedDate;
			}
			OptLog.AddLog(template, ctx, model.MID, model.MDesc, model.MExpectedDate);
		}

		public static void AddExpensePaidLog(MContext ctx, string ExpenseId, decimal veriAccount)
		{
			IVExpenseModel dataEditModel = ModelInfoManager.GetDataEditModel<IVExpenseModel>(ctx, ExpenseId, false, true);
			if (dataEditModel != null)
			{
				decimal num = dataEditModel.MTaxTotalAmtFor - dataEditModel.MVerificationAmt;
				if (dataEditModel.MType == "Expense_Claims")
				{
					OptLog.AddLog(OptLogTemplate.ExpenseClaims_Paid, ctx, dataEditModel.MID, dataEditModel.MEmployee, ctx.DateNow, Math.Abs(veriAccount).To2Decimal());
				}
			}
		}

		public static CommandInfo GetAddExpensePaidLogCmd(MContext ctx, string ExpenseId, DateTime paidDate, decimal veriAccount)
		{
			IVExpenseModel dataEditModel = ModelInfoManager.GetDataEditModel<IVExpenseModel>(ctx, ExpenseId, false, true);
			if (dataEditModel == null)
			{
				return null;
			}
			if (dataEditModel.MType == "Expense_Claims")
			{
				return OptLog.GetAddLogCommand(OptLogTemplate.ExpenseClaims_Paid, ctx, dataEditModel.MID, dataEditModel.MEmployee, paidDate, Math.Abs(veriAccount).To2Decimal());
			}
			return null;
		}

		public static void AddExpenseNoteLog(MContext ctx, IVExpenseModel model)
		{
			OptLogTemplate expenseLogNoteTemplate = GetExpenseLogNoteTemplate(model.MType);
			OptLog.AddLog(expenseLogNoteTemplate, ctx, model.MID, model.MDesc);
		}

		private static void AddExpenseEditLog(MContext ctx, OptLogTemplate logTemplate, IVExpenseModel model)
		{
			OptLog.AddLog(logTemplate, ctx, model.MID, model.MReference, model.MEmployee, Math.Abs(model.MTaxTotalAmtFor).To2Decimal());
		}

		private static OptLogTemplate GetExpenseLogAddTemplate(string bizType)
		{
			if (bizType == "Expense_Claims")
			{
				return OptLogTemplate.ExpenseClaims_Created;
			}
			return OptLogTemplate.None;
		}

		private static OptLogTemplate GetExpenseLogApproveTemplate(string bizType)
		{
			if (bizType == "Expense_Claims")
			{
				return OptLogTemplate.ExpenseClaims_Approved;
			}
			return OptLogTemplate.None;
		}

		private static OptLogTemplate GetExpenseLogEditTemplate(string bizType)
		{
			if (bizType == "Expense_Claims")
			{
				return OptLogTemplate.ExpenseClaims_Edited;
			}
			return OptLogTemplate.None;
		}

		private static OptLogTemplate GetExpenseLogDeleteTemplate(string bizType)
		{
			if (bizType == "Expense_Claims")
			{
				return OptLogTemplate.ExpenseClaims_Deleted;
			}
			return OptLogTemplate.None;
		}

		private static OptLogTemplate GetExpenseLogNoteTemplate(string bizType)
		{
			if (bizType == "Expense_Claims")
			{
				return OptLogTemplate.ExpenseClaims_Note;
			}
			return OptLogTemplate.None;
		}
	}
}
