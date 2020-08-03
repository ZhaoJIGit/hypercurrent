using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVInvoiceLogRepository
	{
		public static void AddInvoiceEditLog(MContext ctx, IVInvoiceModel model)
		{
			OptLogTemplate optLogTemplate = OptLogTemplate.None;
			if (model.IsNew)
			{
				optLogTemplate = GetInvoiceLogAddTemplate(model.MType);
				AddInvoiceEditLog(ctx, optLogTemplate, model);
				if (model.MStatus == Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment))
				{
					optLogTemplate = GetInvoiceLogApproveTemplate(model.MType);
					AddInvoiceEditLog(ctx, optLogTemplate, model);
				}
			}
			else
			{
				optLogTemplate = ((model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment)) ? GetInvoiceLogEditTemplate(model.MType) : GetInvoiceLogApproveTemplate(model.MType));
				AddInvoiceEditLog(ctx, optLogTemplate, model);
			}
		}

		public static void AddInvoiceUnApproveLog(MContext ctx, IVInvoiceModel model)
		{
			OptLogTemplate invoiceLogUnApproveTemplate = GetInvoiceLogUnApproveTemplate(model.MType);
			OptLog.AddLog(invoiceLogUnApproveTemplate, ctx, model.MID, model.MNumber, model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public static List<CommandInfo> GetAddInvoiceEditLogCmd(MContext ctx, IVInvoiceModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			OptLogTemplate optLogTemplate = OptLogTemplate.None;
			if (model.IsNew)
			{
				optLogTemplate = GetInvoiceLogAddTemplate(model.MType);
				list.Add(OptLog.GetAddLogCommand(optLogTemplate, ctx, model.MID, model.MNumber, model.MContactID, Math.Abs(model.MTaxTotalAmtFor)));
				if (model.MStatus == Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment))
				{
					optLogTemplate = GetInvoiceLogApproveTemplate(model.MType);
					list.Add(OptLog.GetAddLogCommand(optLogTemplate, ctx, model.MID, model.MNumber, model.MContactID, Math.Abs(model.MTaxTotalAmtFor)));
				}
			}
			else
			{
				optLogTemplate = ((model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment)) ? GetInvoiceLogEditTemplate(model.MType) : GetInvoiceLogApproveTemplate(model.MType));
				list.Add(OptLog.GetAddLogCommand(optLogTemplate, ctx, model.MID, model.MNumber, model.MContactID, Math.Abs(model.MTaxTotalAmtFor)));
			}
			return list;
		}

		public static List<CommandInfo> GetInvoiceLogUpApproveCmd(MContext ctx, IVInvoiceModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			OptLogTemplate invoiceLogUnApproveTemplate = GetInvoiceLogUnApproveTemplate(model.MType);
			list.Add(OptLog.GetAddLogCommand(invoiceLogUnApproveTemplate, ctx, model.MID, model.MNumber, model.MContactID, Math.Abs(model.MTaxTotalAmtFor)));
			return list;
		}

		public static List<CommandInfo> GetInvoiceApprovalLogCmd(MContext ctx, IVInvoiceModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			OptLogTemplate invoiceLogApproveTemplate = GetInvoiceLogApproveTemplate(model.MType);
			list.Add(OptLog.GetAddLogCommand(invoiceLogApproveTemplate, ctx, model.MID, model.MNumber, model.MContactID, Math.Abs(model.MTaxTotalAmtFor)));
			return list;
		}

		public static List<CommandInfo> GetVerificationLogCmd(MContext ctx, string invoiceId, decimal amtFor)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			IVInvoiceModel invoiceEditModel = IVInvoiceRepository.GetInvoiceEditModel(ctx, invoiceId);
			if (invoiceEditModel == null)
			{
				return list;
			}
			if (invoiceEditModel.MType == "Invoice_Sale" || invoiceEditModel.MType == "Invoice_Purchase")
			{
				CommandInfo addLogCommand = OptLog.GetAddLogCommand(OptLogTemplate.Sale_Invoice_Credit, ctx, invoiceId, ctx.DateNow, Math.Abs(amtFor));
				list.Add(addLogCommand);
			}
			else
			{
				CommandInfo addLogCommand2 = OptLog.GetAddLogCommand(OptLogTemplate.Credit_Note_Apply, ctx, invoiceEditModel.MID, ctx.DateNow, Math.Abs(amtFor));
				list.Add(addLogCommand2);
			}
			return list;
		}

		public static List<CommandInfo> GetDeleteVerificationLogCmd(MContext ctx, string invoiceId, decimal amtFor)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			IVInvoiceModel invoiceEditModel = IVInvoiceRepository.GetInvoiceEditModel(ctx, invoiceId);
			if (invoiceEditModel == null)
			{
				return list;
			}
			if (invoiceEditModel.MType == "Invoice_Sale" || invoiceEditModel.MType == "Invoice_Purchase")
			{
				CommandInfo addLogCommand = OptLog.GetAddLogCommand(OptLogTemplate.Sale_Invoice_Credit_Delete, ctx, invoiceId, ctx.DateNow, Math.Abs(amtFor));
				list.Add(addLogCommand);
			}
			else
			{
				CommandInfo addLogCommand2 = OptLog.GetAddLogCommand(OptLogTemplate.Credit_Note_Apply_Delete, ctx, invoiceEditModel.MID, ctx.DateNow, Math.Abs(invoiceEditModel.MTaxTotalAmtFor));
				list.Add(addLogCommand2);
			}
			return list;
		}

		public static List<CommandInfo> GetCreditInvoiceLogCmd(MContext ctx, string invoiceId, decimal amtFor)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			CommandInfo addLogCommand = OptLog.GetAddLogCommand(OptLogTemplate.Sale_Invoice_Credit, ctx, invoiceId, ctx.DateNow, Math.Abs(amtFor));
			list.Add(addLogCommand);
			return list;
		}

		public static List<CommandInfo> GetCreditApplyLogCmd(MContext ctx, IVInvoiceModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			CommandInfo addLogCommand = OptLog.GetAddLogCommand(OptLogTemplate.Credit_Note_Apply, ctx, model.MID, ctx.DateNow, Math.Abs(model.MTaxTotalAmtFor));
			list.Add(addLogCommand);
			return list;
		}

		public static void AddInvoiceDeleteLog(MContext ctx, ParamBase param)
		{
			string[] values = param.KeyIDs.Split(',');
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter("MID", SqlOperators.In, values);
			sqlWhere.AddFilter("MIsDelete", SqlOperators.Equal, 1);
			List<IVInvoiceModel> dataModelList = ModelInfoManager.GetDataModelList<IVInvoiceModel>(ctx, sqlWhere, false, false);
			if (dataModelList.Count != 0)
			{
				foreach (IVInvoiceModel item in dataModelList)
				{
					OptLogTemplate invoiceLogDeleteTemplate = GetInvoiceLogDeleteTemplate(item.MType);
					OptLog.AddLog(invoiceLogDeleteTemplate, ctx, item.MID, item.MReference);
				}
			}
		}

		public static void AddInvoiceApprovalLog(MContext ctx, ParamBase param)
		{
			if (param.MOperationID.ToMInt32() == Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment))
			{
				string[] values = param.KeyIDs.Split(',');
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.AddFilter("MID", SqlOperators.In, values);
				List<IVInvoiceModel> dataModelList = ModelInfoManager.GetDataModelList<IVInvoiceModel>(ctx, sqlWhere, false, false);
				if (dataModelList.Count != 0)
				{
					foreach (IVInvoiceModel item in dataModelList)
					{
						OptLogTemplate invoiceLogApproveTemplate = GetInvoiceLogApproveTemplate(item.MType);
						AddInvoiceEditLog(ctx, invoiceLogApproveTemplate, item);
					}
				}
			}
		}

		public static void AddInvoiceExpectedInfoLog(MContext ctx, IVInvoiceModel model)
		{
			OptLogTemplate template = OptLogTemplate.None;
			string mType = model.MType;
			if (!(mType == "Invoice_Sale"))
			{
				if (mType == "Invoice_Purchase")
				{
					template = OptLogTemplate.Bill_NoteExpectedDate;
				}
			}
			else
			{
				template = OptLogTemplate.Sale_Invoice_NoteExpectedDate;
			}
			OptLog.AddLog(template, ctx, model.MID, model.MDesc, model.MExpectedDate);
		}

		public static void AddInvoicePaidLog(MContext ctx, string invoiceId, decimal veriAccount)
		{
			IVInvoiceModel dataEditModel = ModelInfoManager.GetDataEditModel<IVInvoiceModel>(ctx, invoiceId, false, true);
			if (dataEditModel != null)
			{
				OptLogTemplate addInvoicePaidLogTemplate = GetAddInvoicePaidLogTemplate(dataEditModel);
				decimal num = dataEditModel.MTaxTotalAmtFor - dataEditModel.MVerificationAmt;
				if (addInvoicePaidLogTemplate != 0)
				{
					OptLog.AddLog(addInvoicePaidLogTemplate, ctx, dataEditModel.MID, dataEditModel.MContactID, ctx.DateNow, Math.Abs(veriAccount).To2Decimal(), num);
				}
			}
		}

		public static CommandInfo GetAddInvoicePaidLogCommand(MContext ctx, string invoiceId, decimal veriAccount, DateTime paidDate)
		{
			IVInvoiceModel dataEditModel = ModelInfoManager.GetDataEditModel<IVInvoiceModel>(ctx, invoiceId, false, true);
			if (dataEditModel == null)
			{
				return null;
			}
			OptLogTemplate addInvoicePaidLogTemplate = GetAddInvoicePaidLogTemplate(dataEditModel);
			decimal num = dataEditModel.MTaxTotalAmtFor - dataEditModel.MVerificationAmt;
			if (addInvoicePaidLogTemplate == OptLogTemplate.None)
			{
				return null;
			}
			return OptLog.GetAddLogCommand(addInvoicePaidLogTemplate, ctx, dataEditModel.MID, dataEditModel.MContactID, paidDate, Math.Abs(veriAccount).To2Decimal(), num);
		}

		private static OptLogTemplate GetAddInvoicePaidLogTemplate(IVInvoiceModel model)
		{
			decimal d = model.MTaxTotalAmtFor - model.MVerificationAmt;
			if (d == decimal.Zero)
			{
				if (model.MType == "Invoice_Sale")
				{
					return OptLogTemplate.Sale_Invoice_Paid;
				}
				if (model.MType == "Invoice_Purchase")
				{
					return OptLogTemplate.Bill_Paid;
				}
			}
			else
			{
				if (model.MType == "Invoice_Sale" || model.MType == "Invoice_Sale_Red")
				{
					return OptLogTemplate.Sale_Invoice_PartiallyPaid;
				}
				if (model.MType == "Invoice_Purchase" || model.MType == "Invoice_Purchase_Red")
				{
					return OptLogTemplate.Bill_PartiallyPaid;
				}
			}
			return OptLogTemplate.None;
		}

		public static void AddInvoiceNoteLog(MContext ctx, IVInvoiceModel model)
		{
			OptLogTemplate invoiceLogNoteTemplate = GetInvoiceLogNoteTemplate(model.MType);
			OptLog.AddLog(invoiceLogNoteTemplate, ctx, model.MID, model.MDesc);
		}

		private static void AddInvoiceEditLog(MContext ctx, OptLogTemplate logTemplate, IVInvoiceModel model)
		{
			OptLog.AddLog(logTemplate, ctx, model.MID, model.MNumber, model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		private static OptLogTemplate GetInvoiceLogAddTemplate(string bizType)
		{
			switch (bizType)
			{
			case "Invoice_Sale":
				return OptLogTemplate.Sale_Invoice_Created;
			case "Invoice_Sale_Red":
				return OptLogTemplate.Sale_Credit_Note_Created;
			case "Invoice_Purchase":
				return OptLogTemplate.Bill_Created;
			case "Invoice_Purchase_Red":
				return OptLogTemplate.Bill_Credit_Note_Created;
			default:
				return OptLogTemplate.None;
			}
		}

		private static OptLogTemplate GetInvoiceLogApproveTemplate(string bizType)
		{
			switch (bizType)
			{
			case "Invoice_Sale":
				return OptLogTemplate.Sale_Invoice_Approved;
			case "Invoice_Sale_Red":
				return OptLogTemplate.Sale_Credit_Note_Approved;
			case "Invoice_Purchase":
				return OptLogTemplate.Bill_Approved;
			case "Invoice_Purchase_Red":
				return OptLogTemplate.Bill_Credit_Note_Approved;
			default:
				return OptLogTemplate.None;
			}
		}

		private static OptLogTemplate GetInvoiceLogEditTemplate(string bizType)
		{
			switch (bizType)
			{
			case "Invoice_Sale":
				return OptLogTemplate.Sale_Invoice_Edited;
			case "Invoice_Sale_Red":
				return OptLogTemplate.Sale_Credit_Note_Edited;
			case "Invoice_Purchase":
				return OptLogTemplate.Bill_Edited;
			case "Invoice_Purchase_Red":
				return OptLogTemplate.Bill_Credit_Note_Edited;
			default:
				return OptLogTemplate.None;
			}
		}

		private static OptLogTemplate GetInvoiceLogUnApproveTemplate(string bizType)
		{
			switch (bizType)
			{
			case "Invoice_Sale":
				return OptLogTemplate.Sale_Invoice_Reverse_Approved;
			case "Invoice_Sale_Red":
				return OptLogTemplate.Credit_Note_Reverse_Approved;
			case "Invoice_Purchase":
				return OptLogTemplate.Bill_Reverse_Approved;
			case "Invoice_Purchase_Red":
				return OptLogTemplate.Bill_Credit_Note_Reverse_Approved;
			default:
				return OptLogTemplate.None;
			}
		}

		private static OptLogTemplate GetInvoiceLogDeleteTemplate(string bizType)
		{
			switch (bizType)
			{
			case "Invoice_Sale":
				return OptLogTemplate.Sale_Invoice_Deleted;
			case "Invoice_Sale_Red":
				return OptLogTemplate.Sale_Credit_Note_Deleted;
			case "Invoice_Purchase":
				return OptLogTemplate.Bill_Deleted;
			case "Invoice_Purchase_Red":
				return OptLogTemplate.Bill_Credit_Note_Deleted;
			default:
				return OptLogTemplate.None;
			}
		}

		private static OptLogTemplate GetInvoiceLogNoteTemplate(string bizType)
		{
			switch (bizType)
			{
			case "Invoice_Sale":
				return OptLogTemplate.Sale_Invoice_Note;
			case "Invoice_Sale_Red":
				return OptLogTemplate.Sale_Credit_Note_Note;
			case "Invoice_Purchase":
				return OptLogTemplate.Bill_Note;
			case "Invoice_Purchase_Red":
				return OptLogTemplate.Bill_Credit_Note_Note;
			default:
				return OptLogTemplate.None;
			}
		}
	}
}
