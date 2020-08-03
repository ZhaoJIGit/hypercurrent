using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVRepeatInvoiceLogRepository
	{
		public static CommandInfo GetAddRepeatInvoiceEditLogCmd(MContext ctx, IVRepeatInvoiceModel model)
		{
			OptLogTemplate template = OptLogTemplate.None;
			if (model.IsNew)
			{
				string mType = model.MType;
				if (!(mType == "Invoice_Sale"))
				{
					if (mType == "Invoice_Purchase")
					{
						template = OptLogTemplate.Bill_Created;
					}
				}
				else
				{
					template = OptLogTemplate.Bill_Created;
				}
			}
			else
			{
				string mType2 = model.MType;
				if (!(mType2 == "Invoice_Sale"))
				{
					if (mType2 == "Invoice_Purchase")
					{
						template = OptLogTemplate.Bill_Edited;
					}
				}
				else
				{
					template = OptLogTemplate.Bill_Edited;
				}
			}
			return OptLog.GetAddLogCommand(template, ctx, model.MID, model.MReference, model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public static void AddRepeatInvoiceUpdateStatusLog(MContext ctx, ParamBase param)
		{
			SqlWhere filter = new SqlWhere().AddFilter("MID", SqlOperators.In, param.KeyIDs.Split(','));
			List<IVRepeatInvoiceModel> dataModelList = ModelInfoManager.GetDataModelList<IVRepeatInvoiceModel>(ctx, filter, false, false);
			if (dataModelList.Count != 0)
			{
				foreach (IVRepeatInvoiceModel item in dataModelList)
				{
					OptLogTemplate template = OptLogTemplate.None;
					string mType = item.MType;
					if (!(mType == "Invoice_Sale"))
					{
						if (mType == "Invoice_Purchase")
						{
							template = OptLogTemplate.Bill_RepeatInvoice_UpdateStatus;
						}
					}
					else
					{
						template = OptLogTemplate.Sale_RepeatInvoice_UpdateStatus;
					}
					OptLog.AddLog(template, ctx, item.MID, GetStatusText(ctx, item.MStatus));
				}
			}
		}

		private static string GetStatusText(MContext ctx, int status)
		{
			string result = string.Empty;
			switch (status)
			{
			case 1:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "SaveAsDraft", "Save as Draft");
				break;
			case 2:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Approve", "Approve");
				break;
			case 3:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ApproveForSending", "Approve for Sending");
				break;
			}
			return result;
		}

		public static void AddRepeatInvoiceDeleteLog(MContext ctx, ParamBase param)
		{
			SqlWhere filter = new SqlWhere().AddFilter("MID", SqlOperators.In, param.KeyIDs.Split(','));
			List<IVRepeatInvoiceModel> dataModelList = ModelInfoManager.GetDataModelList<IVRepeatInvoiceModel>(ctx, filter, false, false);
			if (dataModelList.Count != 0)
			{
				foreach (IVRepeatInvoiceModel item in dataModelList)
				{
					OptLogTemplate template = OptLogTemplate.None;
					string mType = item.MType;
					if (!(mType == "Invoice_Sale"))
					{
						if (mType == "Invoice_Purchase")
						{
							template = OptLogTemplate.Bill_RepeatInvoice_Deleted;
						}
					}
					else
					{
						template = OptLogTemplate.Sale_RepeatInvoice_Deleted;
					}
					OptLog.AddLog(template, ctx, item.MID, item.MReference);
				}
			}
		}

		public static void AddRepeatInvoiceExpectedInfoLog(MContext ctx, IVRepeatInvoiceModel model)
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

		public static void AddRepeatInvoiceNoteLog(MContext ctx, IVRepeatInvoiceModel model)
		{
			OptLogTemplate template = OptLogTemplate.None;
			string mType = model.MType;
			if (!(mType == "Invoice_Sale"))
			{
				if (mType == "Invoice_Purchase")
				{
					template = OptLogTemplate.Bill_Note;
				}
			}
			else
			{
				template = OptLogTemplate.Sale_Invoice_Note;
			}
			OptLog.AddLog(template, ctx, model.MID, model.MDesc);
		}
	}
}
