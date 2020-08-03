using JieNor.Megi.BusinessContract.RI;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace JieNor.Megi.BusinessService.RI
{
	public class RIBusinessInspector : IRIInspectable
	{
		private GLCheckInvoiceRepository repository = new GLCheckInvoiceRepository();

		public RIBusinessInspector()
		{
			base.enginers = new List<Func<MContext, RICategoryModel, int, int, RIInspectionResult>>
			{
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(BZ_InvoiceSale),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(BZ_InvoicePurchase),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(BZ_Recieve),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(BZ_Payment),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(BZ_Transfer),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(BZ_Expense),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(BZ_BankBill),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(BZ_DocVoucher),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(BZ_BankBill2Doc)
			};
		}

		public RIInspectionResult BZ_InvoiceSale(MContext ctx, RICategoryModel category, int year, int period)
		{
			DateTime beginTime = DateTime.ParseExact((year * 100 + period).ToString() + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime endTime = beginTime.AddMonths(1).AddSeconds(-1.0);
			int invoice = repository.GetInvoice(ctx, beginTime, endTime, "Invoice_Sale");
			bool flag = invoice <= 0;
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = flag;
			rIInspectionResult.MMessageParam = (flag ? new string[0] : new string[1]
			{
				invoice.ToString()
			});
			rIInspectionResult.MUrlParam = (flag ? new string[1]
			{
				""
			} : new string[1]
			{
				"2"
			});
			return rIInspectionResult;
		}

		public RIInspectionResult BZ_InvoicePurchase(MContext ctx, RICategoryModel category, int year, int period)
		{
			DateTime beginTime = DateTime.ParseExact((year * 100 + period).ToString() + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime endTime = beginTime.AddMonths(1).AddSeconds(-1.0);
			int invoice = repository.GetInvoice(ctx, beginTime, endTime, "Invoice_Purchase");
			bool flag = invoice <= 0;
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = flag;
			rIInspectionResult.MMessageParam = (flag ? new string[0] : new string[1]
			{
				invoice.ToString()
			});
			rIInspectionResult.MUrlParam = (flag ? new string[1]
			{
				""
			} : new string[1]
			{
				"2"
			});
			return rIInspectionResult;
		}

		public RIInspectionResult BZ_Recieve(MContext ctx, RICategoryModel category, int year, int period)
		{
			DateTime beginTime = DateTime.ParseExact((year * 100 + period).ToString() + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime endTime = beginTime.AddMonths(1).AddSeconds(-1.0);
			RIInvoiceModel payment = repository.GetPayment(ctx, beginTime, endTime, false);
			int rowCounts = payment.RowCounts;
			bool flag = rowCounts <= 0;
			string[] mUrlParam = new string[0];
			if (!flag)
			{
				mUrlParam = new string[4]
				{
					payment.MBankID,
					"3",
					"1",
					payment.MBankTypeID
				};
			}
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = flag;
			rIInspectionResult.MNoLinkUrlIfPassed = flag;
			rIInspectionResult.MMessageParam = (flag ? new string[0] : new string[1]
			{
				rowCounts.ToString()
			});
			rIInspectionResult.MUrlParam = mUrlParam;
			return rIInspectionResult;
		}

		public RIInspectionResult BZ_Payment(MContext ctx, RICategoryModel category, int year, int period)
		{
			DateTime beginTime = DateTime.ParseExact((year * 100 + period).ToString() + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime endTime = beginTime.AddMonths(1).AddSeconds(-1.0);
			RIInvoiceModel payment = repository.GetPayment(ctx, beginTime, endTime, true);
			int rowCounts = payment.RowCounts;
			bool flag = rowCounts <= 0;
			string[] mUrlParam = new string[0];
			if (!flag)
			{
				mUrlParam = new string[4]
				{
					payment.MBankID,
					"3",
					"1",
					payment.MBankTypeID
				};
			}
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = flag;
			rIInspectionResult.MNoLinkUrlIfPassed = flag;
			rIInspectionResult.MMessageParam = (flag ? new string[0] : new string[1]
			{
				rowCounts.ToString()
			});
			rIInspectionResult.MUrlParam = mUrlParam;
			return rIInspectionResult;
		}

		public RIInspectionResult BZ_Transfer(MContext ctx, RICategoryModel category, int year, int period)
		{
			DateTime beginTime = DateTime.ParseExact((year * 100 + period).ToString() + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime endTime = beginTime.AddMonths(1).AddSeconds(-1.0);
			RIInvoiceModel transfer = repository.GetTransfer(ctx, beginTime, endTime);
			int rowCounts = transfer.RowCounts;
			bool flag = rowCounts <= 0;
			string[] mUrlParam = new string[0];
			if (!flag)
			{
				mUrlParam = new string[4]
				{
					transfer.MBankID ?? transfer.MBankIDFrom,
					"3",
					"1",
					transfer.MBankTypeID ?? transfer.MBankTypeIDFrom
				};
			}
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = flag;
			rIInspectionResult.MNoLinkUrlIfPassed = flag;
			rIInspectionResult.MMessageParam = (flag ? new string[0] : new string[1]
			{
				rowCounts.ToString()
			});
			rIInspectionResult.MUrlParam = mUrlParam;
			return rIInspectionResult;
		}

		public RIInspectionResult BZ_Expense(MContext ctx, RICategoryModel category, int year, int period)
		{
			DateTime beginTime = DateTime.ParseExact((year * 100 + period).ToString() + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime endTime = beginTime.AddMonths(1).AddSeconds(-1.0);
			int expense = repository.GetExpense(ctx, beginTime, endTime);
			bool flag = expense <= 0;
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = flag;
			rIInspectionResult.MMessageParam = (flag ? new string[0] : new string[1]
			{
				expense.ToString()
			});
			rIInspectionResult.MUrlParam = (flag ? new string[1]
			{
				""
			} : new string[1]
			{
				"1"
			});
			return rIInspectionResult;
		}

		public RIInspectionResult BZ_BankBill(MContext ctx, RICategoryModel category, int year, int period)
		{
			DateTime beginTime = DateTime.ParseExact((year * 100 + period).ToString() + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime endTime = beginTime.AddMonths(1).AddSeconds(-1.0);
			RIInvoiceModel bankBill = repository.GetBankBill(ctx, beginTime, endTime);
			int rowCounts = bankBill.RowCounts;
			bool flag = rowCounts <= 0;
			string[] mUrlParam = new string[0];
			if (!flag)
			{
				string text = (ctx.MOrgVersionID == 0) ? "1" : "3";
				mUrlParam = new string[4]
				{
					bankBill.MBankID,
					text,
					"1",
					bankBill.MBankTypeID
				};
			}
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = flag;
			rIInspectionResult.MNoLinkUrlIfPassed = flag;
			rIInspectionResult.MMessageParam = (flag ? new string[0] : new string[1]
			{
				rowCounts.ToString()
			});
			rIInspectionResult.MUrlParam = mUrlParam;
			return rIInspectionResult;
		}

		public RIInspectionResult BZ_BankBill2Doc(MContext ctx, RICategoryModel category, int year, int period)
		{
			IVBankBillStatusCountModel bankbillVoucherStatusSummary = IVBankBillEntryRepository.GetBankbillVoucherStatusSummary(ctx, year, period);
			string[] mUrlParam = new string[0];
			if (!string.IsNullOrEmpty(bankbillVoucherStatusSummary.MBankID) && !string.IsNullOrEmpty(bankbillVoucherStatusSummary.MBankTypeID))
			{
				mUrlParam = new string[4]
				{
					bankbillVoucherStatusSummary.MBankID,
					"1",
					"1",
					bankbillVoucherStatusSummary.MBankTypeID
				};
			}
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = (bankbillVoucherStatusSummary.NonGeneratedCount == 0);
			rIInspectionResult.MNoLinkUrlIfPassed = true;
			RIInspectionResult rIInspectionResult2 = rIInspectionResult;
			string[] obj = new string[4];
			int num = bankbillVoucherStatusSummary.AllCount;
			obj[0] = num.ToString();
			num = bankbillVoucherStatusSummary.GeneratedCount;
			obj[1] = num.ToString();
			num = bankbillVoucherStatusSummary.NonGeneratedCount;
			obj[2] = num.ToString();
			num = bankbillVoucherStatusSummary.UnGenerateCount;
			obj[3] = num.ToString();
			rIInspectionResult2.MMessageParam = obj;
			rIInspectionResult.MUrlParam = mUrlParam;
			return rIInspectionResult;
		}

		public RIInspectionResult BZ_DocVoucher(MContext ctx, RICategoryModel category, int year, int period)
		{
			int iVToVoucherBill = repository.GetIVToVoucherBill(ctx, year, period);
			bool flag = iVToVoucherBill <= 0;
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = flag;
			rIInspectionResult.MMessageParam = (flag ? new string[0] : new string[1]
			{
				iVToVoucherBill.ToString()
			});
			return rIInspectionResult;
		}
	}
}
