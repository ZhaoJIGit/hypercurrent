using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.Log
{
	public static class IVInvoiceLogHelper
	{
		public static List<CommandInfo> GetSaveLogCmd(MContext ctx, IVInvoiceModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			IIVInvoiceLog iIVInvoiceLog = IVInvoiceLogFactory.CreateInstance(model.MType);
			switch (model.MStatus)
			{
			case 1:
				if (model.IsNew)
				{
					list.Add(iIVInvoiceLog.GetCreateLogCmd(ctx, model));
				}
				else
				{
					list.Add(iIVInvoiceLog.GetEditLogCmd(ctx, model));
				}
				break;
			case 2:
				if (model.IsNew)
				{
					list.Add(iIVInvoiceLog.GetCreateLogCmd(ctx, model));
					list.Add(iIVInvoiceLog.GetSubmitForApprovalLogCmd(ctx, model));
				}
				else
				{
					list.Add(iIVInvoiceLog.GetEditLogCmd(ctx, model));
					list.Add(iIVInvoiceLog.GetSubmitForApprovalLogCmd(ctx, model));
				}
				break;
			case 3:
				if (model.IsNew)
				{
					list.Add(iIVInvoiceLog.GetCreateLogCmd(ctx, model));
				}
				list.Add(iIVInvoiceLog.GetApproveLogCmd(ctx, model));
				break;
			case 4:
				list.Add(iIVInvoiceLog.GetUpdateExpectedDateLogCmd(ctx, model));
				break;
			}
			return list;
		}

		public static List<CommandInfo> GetApproveLogCmd(MContext ctx, IVInvoiceModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			IIVInvoiceLog iIVInvoiceLog = IVInvoiceLogFactory.CreateInstance(model.MType);
			list.Add(iIVInvoiceLog.GetApproveLogCmd(ctx, model));
			return list;
		}

		public static List<CommandInfo> GetUnApproveLogCmd(MContext ctx, IVInvoiceModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			IIVInvoiceLog iIVInvoiceLog = IVInvoiceLogFactory.CreateInstance(model.MType);
			list.Add(iIVInvoiceLog.GetUnApproveLogCmd(ctx, model));
			return list;
		}

		public static List<CommandInfo> GetSubmitForApprovalLogCmd(MContext ctx, IVInvoiceModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			IIVInvoiceLog iIVInvoiceLog = IVInvoiceLogFactory.CreateInstance(model.MType);
			list.Add(iIVInvoiceLog.GetSubmitForApprovalLogCmd(ctx, model));
			return list;
		}

		public static List<CommandInfo> GetPaidLogCmd(MContext ctx, IVInvoiceModel model, DateTime paidDate, decimal paidAmtFor)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			IIVInvoiceLog iIVInvoiceLog = IVInvoiceLogFactory.CreateInstance(model.MType);
			list.Add(iIVInvoiceLog.GetPaidLogCmd(ctx, model, paidDate, paidAmtFor));
			return list;
		}

		public static List<CommandInfo> GetVerificationLogCmd(MContext ctx, IVInvoiceModel model, decimal amtFor)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			IIVInvoiceLog iIVInvoiceLog = IVInvoiceLogFactory.CreateInstance(model.MType);
			list.Add(iIVInvoiceLog.GetVerificationLogCmd(ctx, model, amtFor));
			return list;
		}

		public static List<CommandInfo> GetDeleteVerificationLogCmd(MContext ctx, IVInvoiceModel model, decimal amtFor)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			IIVInvoiceLog iIVInvoiceLog = IVInvoiceLogFactory.CreateInstance(model.MType);
			list.Add(iIVInvoiceLog.GetDeleteVerificationLogCmd(ctx, model, amtFor));
			return list;
		}

		public static void EditAccountLog(MContext ctx, List<string> invoiceIdList)
		{
			EditAccountAndGenerateVoucherLog(ctx, invoiceIdList, false);
		}

		public static void GenerateVoucherLog(MContext ctx, List<string> invoiceIdList)
		{
			EditAccountAndGenerateVoucherLog(ctx, invoiceIdList, true);
		}

		private static void EditAccountAndGenerateVoucherLog(MContext ctx, List<string> invoiceIdList, bool generateVoucher)
		{
			if (invoiceIdList != null && invoiceIdList.Count != 0)
			{
				List<IVInvoiceModel> invoiceList = IVInvoiceRepository.GetInvoiceList(ctx, invoiceIdList);
				if (invoiceList != null && invoiceList.Count != 0)
				{
					List<CommandInfo> list = new List<CommandInfo>();
					foreach (IVInvoiceModel item in invoiceList)
					{
						IIVInvoiceLog iIVInvoiceLog = IVInvoiceLogFactory.CreateInstance(item.MType);
						list.Add(iIVInvoiceLog.GetEditAccountLogCmd(ctx, item));
						if (generateVoucher)
						{
							list.Add(iIVInvoiceLog.GetGenrateVoucherLogCmd(ctx, item));
						}
					}
					DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
					dynamicDbHelperMySQL.ExecuteSqlTran(list);
				}
			}
		}
	}
}
