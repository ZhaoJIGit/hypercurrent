using JieNor.Megi.BusinessContract.IO;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.IO
{
	public class IOImportBusiness : IIOImportBusiness
	{
		private IVExpenseBusiness expenseBiz = new IVExpenseBusiness();

		private IVInvoiceBusiness invoiceBiz = new IVInvoiceBusiness();

		public ImportResult ImportData(MContext ctx, ImportTypeEnum type, IOImportDataModel data)
		{
			if (data == null || data.EffectiveData == null || data.EffectiveData.Rows.Count == 0)
			{
				return new ImportResult
				{
					Success = false,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NoDataToImport", "That's no data to import!")
				};
			}
			IImport import = ImportFactory.CreateInstance(type, data);
			return import.ImportData(ctx, data);
		}

		public ImportTemplateModel GetTemplateModel(MContext ctx, ImportTypeEnum type)
		{
			switch (type)
			{
			case ImportTypeEnum.Expense:
				return expenseBiz.GetImportTemplateModel(ctx);
			case ImportTypeEnum.Invoice:
				return invoiceBiz.GetImportTemplateModel(ctx, "Invoice_Sale", true);
			case ImportTypeEnum.Purchase:
				return invoiceBiz.GetImportTemplateModel(ctx, "Invoice_Purchase", true);
			default:
				return null;
			}
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx, ImportTypeEnum type)
		{
			switch (type)
			{
			case ImportTypeEnum.Expense:
			case ImportTypeEnum.ExpenseSmart:
				return expenseBiz.GetTemplateConfig(ctx);
			case ImportTypeEnum.Invoice:
				return invoiceBiz.GetTemplateConfig(ctx, "Invoice_Sale", true);
			case ImportTypeEnum.Purchase:
				return invoiceBiz.GetTemplateConfig(ctx, "Invoice_Purchase", true);
			default:
				return null;
			}
		}

		public List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, ImportTypeEnum type)
		{
			switch (type)
			{
			case ImportTypeEnum.Expense:
				return expenseBiz.GetTemplateBasicData(ctx, null);
			case ImportTypeEnum.Invoice:
				return invoiceBiz.GetTemplateBasicData(ctx, "Invoice_Sale", true, null);
			case ImportTypeEnum.Purchase:
				return invoiceBiz.GetTemplateBasicData(ctx, "Invoice_Purchase", true, null);
			default:
				return null;
			}
		}
	}
}
