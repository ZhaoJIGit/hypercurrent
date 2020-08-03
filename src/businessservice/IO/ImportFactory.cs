using JieNor.Megi.BusinessService.IO.Biz;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.EntityModel.Enum;

namespace JieNor.Megi.BusinessService.IO
{
	public class ImportFactory
	{
		public static IImport CreateInstance(ImportTypeEnum type, IOImportDataModel data)
		{
			switch (type)
			{
			case ImportTypeEnum.Voucher:
				return new VoucherImport();
			case ImportTypeEnum.Invoice:
			case ImportTypeEnum.Purchase:
				return new InvoiceImport(type);
			case ImportTypeEnum.Expense:
				return new ExpenseImport();
			case ImportTypeEnum.InFaPiao:
				return new InPutFaPiaoImport();
			case ImportTypeEnum.OutFaPiao:
				if (data.SolutionID == 999999.ToString())
				{
					return new HangTianFaPiaoImport();
				}
				return new OutPutFaPiaoImport();
			case ImportTypeEnum.ExpenseSmart:
				return new SmartOrgExpenseImport();
			default:
				return new BasicImport();
			}
		}
	}
}
