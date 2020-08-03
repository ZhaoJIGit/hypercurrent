using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVGoldenTaxInvoiceLogRepository
	{
		public static void AddGoldenTaxInvoiceEditLog(MContext ctx, IVGoldenTaxInvoiceModel model)
		{
			OptLogTemplate template = OptLogTemplate.None;
			if (model.IsNew)
			{
				switch (model.MInvoiceSource)
				{
				case 1:
					template = OptLogTemplate.Sale_Invoice_Created;
					break;
				case 2:
					template = OptLogTemplate.Bill_Created;
					break;
				}
			}
			else
			{
				switch (model.MInvoiceSource)
				{
				case 1:
					template = OptLogTemplate.Sale_Invoice_Edited;
					break;
				case 2:
					template = OptLogTemplate.Bill_Edited;
					break;
				}
			}
			OptLog.AddLog(template, ctx, model.MID);
		}

		public static void AddGoldenTaxInvoiceDeleteLog(MContext ctx, ParamBase param)
		{
			SqlWhere filter = new SqlWhere().AddFilter("MID", SqlOperators.In, param.KeyIDs.Split(','));
			List<IVGoldenTaxInvoiceModel> dataModelList = ModelInfoManager.GetDataModelList<IVGoldenTaxInvoiceModel>(ctx, filter, false, false);
			if (dataModelList.Count != 0)
			{
				foreach (IVGoldenTaxInvoiceModel item in dataModelList)
				{
					OptLogTemplate template = OptLogTemplate.None;
					switch (item.MInvoiceSource)
					{
					case 1:
						template = OptLogTemplate.Sale_GoldenTaxInvoice_Deleted;
						break;
					case 2:
						template = OptLogTemplate.Bill_GoldenTaxInvoice_Deleted;
						break;
					}
					OptLog.AddLog(template, ctx, item.MID, item.MReference);
				}
			}
		}

		public static void AddGoldenTaxInvoiceUpdateExpressInfoLog(MContext ctx, IVGoldenTaxInvoiceModel model)
		{
			OptLogTemplate template = OptLogTemplate.None;
			switch (model.MInvoiceSource)
			{
			case 1:
				template = OptLogTemplate.Sale_GoldenTaxInvoice_Deleted;
				break;
			case 2:
				template = OptLogTemplate.Bill_GoldenTaxInvoice_UpdateExpressInfo;
				break;
			}
			OptLog.AddLog(template, ctx, model.MID, model.MExpressCompany, model.MExpressNumber, model.MExpressDate);
		}

		public static void AddGoldenTaxInvoiceUpdatePrintStatusLog(MContext ctx, ParamBase param)
		{
			SqlWhere filter = new SqlWhere().AddFilter("MID", SqlOperators.In, param.KeyIDs.Split(','));
			List<IVGoldenTaxInvoiceModel> dataModelList = ModelInfoManager.GetDataModelList<IVGoldenTaxInvoiceModel>(ctx, filter, false, false);
			if (dataModelList.Count != 0)
			{
				foreach (IVGoldenTaxInvoiceModel item in dataModelList)
				{
					OptLogTemplate template = OptLogTemplate.None;
					switch (item.MInvoiceSource)
					{
					case 1:
						template = OptLogTemplate.Sale_GoldenTaxInvoice_UpdatePrintStatus;
						break;
					case 2:
						template = OptLogTemplate.Bill_GoldenTaxInvoice_UpdatePrintStatus;
						break;
					}
					OptLog.AddLog(template, ctx, item.MID, GetPrintStatusText(ctx, item.MIsPrint));
				}
			}
		}

		public static void AddGoldenTaxInvoiceNoteLog(MContext ctx, IVGoldenTaxInvoiceModel model)
		{
			OptLogTemplate template = OptLogTemplate.None;
			switch (model.MInvoiceSource)
			{
			case 1:
				template = OptLogTemplate.Sale_GoldenTaxInvoice_Note;
				break;
			case 2:
				template = OptLogTemplate.Bill_GoldenTaxInvoice_Note;
				break;
			}
			OptLog.AddLog(template, ctx, model.MID, model.MReference);
		}

		private static string GetPrintStatusText(MContext ctx, bool isPrint)
		{
			if (isPrint)
			{
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "HasPrinted", "Has printed");
			}
			return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "NoPrint", "No print");
		}
	}
}
