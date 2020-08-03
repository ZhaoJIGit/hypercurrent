using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.Log
{
	public static class IVExpenseLogHelper
	{
		private static readonly IVExpenseLogBase _expLog = new IVExpenseLogBase();

		public static List<CommandInfo> GetSaveLogCmd(MContext ctx, IVExpenseModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			switch (model.MStatus)
			{
			case 1:
				if (model.IsNew)
				{
					list.Add(_expLog.GetCreateLogCmd(ctx, model));
				}
				else
				{
					list.Add(_expLog.GetEditLogCmd(ctx, model));
				}
				break;
			case 2:
				if (model.IsNew)
				{
					list.Add(_expLog.GetCreateLogCmd(ctx, model));
					list.Add(_expLog.GetSubmitForApprovalLogCmd(ctx, model));
				}
				else
				{
					list.Add(_expLog.GetEditLogCmd(ctx, model));
					list.Add(_expLog.GetSubmitForApprovalLogCmd(ctx, model));
				}
				break;
			case 3:
				if (model.IsNew)
				{
					list.Add(_expLog.GetCreateLogCmd(ctx, model));
				}
				list.Add(_expLog.GetApproveLogCmd(ctx, model));
				break;
			case 4:
				if (model.IsNew)
				{
					list.Add(_expLog.GetCreateLogCmd(ctx, model));
				}
				break;
			}
			return list;
		}

		public static List<CommandInfo> GetApproveLogCmd(MContext ctx, IVExpenseModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.Add(_expLog.GetApproveLogCmd(ctx, model));
			return list;
		}

		public static List<CommandInfo> GetUnApproveLogCmd(MContext ctx, IVExpenseModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.Add(_expLog.GetUnApproveLogCmd(ctx, model));
			return list;
		}

		public static List<CommandInfo> GetSubmitForApprovalLogCmd(MContext ctx, IVExpenseModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.Add(_expLog.GetSubmitForApprovalLogCmd(ctx, model));
			return list;
		}

		public static List<CommandInfo> GetPaidLogCmd(MContext ctx, IVExpenseModel model, DateTime paidDate, decimal paidAmtFor)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.Add(_expLog.GetPaidLogCmd(ctx, model, paidDate, paidAmtFor));
			return list;
		}

		public static List<CommandInfo> GetVerificationLogCmd(MContext ctx, IVExpenseModel model, decimal amtFor)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.Add(_expLog.GetVerificationLogCmd(ctx, model, amtFor));
			return list;
		}

		public static List<CommandInfo> GetDeleteVerificationLogCmd(MContext ctx, IVExpenseModel model, decimal amtFor)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.Add(_expLog.GetDeleteVerificationLogCmd(ctx, model, amtFor));
			return list;
		}

		public static void EditAccountLog(MContext ctx, List<string> expenseIdList)
		{
			EditAccountAndGenerateVoucherLog(ctx, expenseIdList, false);
		}

		public static void GenerateVoucherLog(MContext ctx, List<string> expenseIdList)
		{
			EditAccountAndGenerateVoucherLog(ctx, expenseIdList, true);
		}

		private static void EditAccountAndGenerateVoucherLog(MContext ctx, List<string> expenseIdList, bool generateVoucher)
		{
			if (expenseIdList != null && expenseIdList.Count != 0)
			{
				List<IVExpenseModel> expenseList = IVExpenseRepository.GetExpenseList(ctx, expenseIdList);
				if (expenseList != null && expenseList.Count != 0)
				{
					List<CommandInfo> list = new List<CommandInfo>();
					foreach (IVExpenseModel item in expenseList)
					{
						list.Add(_expLog.GetEditAccountLogCmd(ctx, item));
						if (generateVoucher)
						{
							list.Add(_expLog.GetGenrateVoucherLogCmd(ctx, item));
						}
					}
					DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
					dynamicDbHelperMySQL.ExecuteSqlTran(list);
				}
			}
		}
	}
}
