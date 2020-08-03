using JieNor.Megi.BusinessContract.RI;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.RI
{
	public class RIMonthlyInspector : IRIInspectable
	{
		public RIMonthlyInspector()
		{
			base.enginers = new List<Func<MContext, RICategoryModel, int, int, RIInspectionResult>>
			{
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ML_TransferCost),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ML_Depreciation),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ML_AmortizationExpense),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ML_WagesOnAccount),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ML_FinalTransfer),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ML_TransferVAT),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ML_IncomeTax),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ML_TransferProfitLoss),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(ML_TransferNDP)
			};
		}

		public RIInspectionResult ML_TransferCost(MContext ctx, RICategoryModel category, int year, int period)
		{
			return GetResultByCreditDebitHasValue(ctx, category, year, period, "6401", 1, true);
		}

		public RIInspectionResult ML_Depreciation(MContext ctx, RICategoryModel category, int year, int period)
		{
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			BDAccountModel bDAccountModel = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == "1602");
			BDAccountModel bDAccountModel2 = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == "660205");
			if (bDAccountModel == null || bDAccountModel2 == null)
			{
				return new RIInspectionResult
				{
					MPassed = true,
					MNoLinkUrlIfPassed = true,
					MAccountIsDisable = true
				};
			}
			decimal d = (from x in dataPool.BalanceList
			where x.MAccountCode == "1602"
			select x).Sum((GLBalanceModel x) => x.MCredit);
			decimal d2 = (from x in dataPool.BalanceList
			where x.MAccountCode == "660205"
			select x).Sum((GLBalanceModel x) => x.MDebit);
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = (d == d2 && d != decimal.Zero);
			rIInspectionResult.MUrlParam = new string[2]
			{
				(year * 100 + period).ToString(),
				bDAccountModel2.MItemID
			};
			rIInspectionResult.MNoLinkUrlIfPassed = true;
			return rIInspectionResult;
		}

		public RIInspectionResult ML_AmortizationExpense(MContext ctx, RICategoryModel category, int year, int period)
		{
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			BDAccountModel bDAccountModel = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == "1801");
			BDAccountModel bDAccountModel2 = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == "660207");
			if (bDAccountModel == null || bDAccountModel2 == null)
			{
				return new RIInspectionResult
				{
					MPassed = true,
					MNoLinkUrlIfPassed = true,
					MAccountIsDisable = true
				};
			}
			decimal d = (from x in dataPool.BalanceList
			where x.MAccountCode == "1801"
			select x).Sum((GLBalanceModel x) => x.MCredit);
			decimal d2 = (from x in dataPool.BalanceList
			where x.MAccountCode == "660207"
			select x).Sum((GLBalanceModel x) => x.MDebit);
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = (d == d2 && d != decimal.Zero);
			rIInspectionResult.MUrlParam = new string[2]
			{
				(year * 100 + period).ToString(),
				bDAccountModel.MItemID
			};
			rIInspectionResult.MNoLinkUrlIfPassed = true;
			return rIInspectionResult;
		}

		public RIInspectionResult ML_WagesOnAccount(MContext ctx, RICategoryModel category, int year, int period)
		{
			return new RIInspectionResult
			{
				MPassed = true,
				MNoLinkUrlIfPassed = true
			};
		}

		public RIInspectionResult ML_FinalTransfer(MContext ctx, RICategoryModel category, int year, int period)
		{
			return GetPeriodTransferResult(ctx, category, year, period, 9, "660302");
		}

		public RIInspectionResult ML_TransferVAT(MContext ctx, RICategoryModel category, int year, int period)
		{
			return GetResultByCreditDebitHasValue(ctx, category, year, period, "222102", -1, true);
		}

		public RIInspectionResult ML_IncomeTax(MContext ctx, RICategoryModel category, int year, int period)
		{
			return GetResultByCreditDebitHasValue(ctx, category, year, period, "6801", 1, true);
		}

		public RIInspectionResult ML_TransferProfitLoss(MContext ctx, RICategoryModel category, int year, int period)
		{
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			BDAccountModel account = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == "4103");
			int num;
			RIInspectionResult rIInspectionResult;
			if (account == null)
			{
				rIInspectionResult = new RIInspectionResult();
				rIInspectionResult.MPassed = true;
				rIInspectionResult.MAccountIsDisable = true;
				RIInspectionResult rIInspectionResult2 = rIInspectionResult;
				string[] obj = new string[2];
				num = year * 100 + period;
				obj[0] = num.ToString();
				obj[1] = string.Empty;
				rIInspectionResult2.MUrlParam = obj;
				rIInspectionResult.MNoLinkUrlIfPassed = true;
				return rIInspectionResult;
			}
			GLBalanceModel gLBalanceModel = dataPool.BalanceList.FirstOrDefault((GLBalanceModel x) => x.MAccountID == account.MItemID && x.MCheckGroupValueID == "0");
			if (gLBalanceModel == null || (gLBalanceModel.MCredit == decimal.Zero && gLBalanceModel.MDebit == decimal.Zero && gLBalanceModel.MCreditFor == decimal.Zero && gLBalanceModel.MDebitFor == decimal.Zero))
			{
				rIInspectionResult = new RIInspectionResult();
				rIInspectionResult.MPassed = false;
				RIInspectionResult rIInspectionResult3 = rIInspectionResult;
				string[] obj2 = new string[2];
				num = year * 100 + period;
				obj2[0] = num.ToString();
				obj2[1] = string.Empty;
				rIInspectionResult3.MUrlParam = obj2;
				rIInspectionResult.MNoLinkUrlIfPassed = true;
				return rIInspectionResult;
			}
			AccountTypeEnum accountTypeEnum = new AccountTypeEnum(ctx.MAccountTableID);
			List<string> itemListByType = BDAccountRepository.GetItemListByType(dataPool.AccountWithParentList, new List<string>
			{
				accountTypeEnum.OperatingRevenue,
				accountTypeEnum.OtherRevenue
			}, false, true);
			List<string> itemListByType2 = BDAccountRepository.GetItemListByType(dataPool.AccountWithParentList, new List<string>
			{
				accountTypeEnum.OperatingCostsAndTaxes,
				accountTypeEnum.OtherLoss,
				accountTypeEnum.PeriodCharge,
				accountTypeEnum.IncomeTax
			}, false, true);
			bool mPassed = true;
			for (int num2 = 0; num2 < dataPool.BalanceList.Count; num2++)
			{
				int num3;
				if ((itemListByType.Contains(dataPool.BalanceList[num2].MAccountID) || itemListByType2.Contains(dataPool.BalanceList[num2].MAccountID)) && dataPool.BalanceList[num2].MCheckGroupValueID == "0")
				{
					num3 = ((dataPool.BalanceList[num2].MBeginBalance + (decimal)dataPool.BalanceList[num2].MDC * (dataPool.BalanceList[num2].MDebit - dataPool.BalanceList[num2].MCredit) != decimal.Zero) ? 1 : 0);
					goto IL_02af;
				}
				num3 = 0;
				goto IL_02af;
				IL_02af:
				if (num3 != 0)
				{
					mPassed = false;
					break;
				}
			}
			rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = mPassed;
			RIInspectionResult rIInspectionResult4 = rIInspectionResult;
			string[] obj3 = new string[2];
			num = year * 100 + period;
			obj3[0] = num.ToString();
			obj3[1] = ((account != null) ? account.MItemID : account.MItemID);
			rIInspectionResult4.MUrlParam = obj3;
			rIInspectionResult.MNoLinkUrlIfPassed = true;
			return rIInspectionResult;
		}

		public RIInspectionResult ML_TransferNDP(MContext ctx, RICategoryModel category, int year, int period)
		{
			if (period != 12)
			{
				return new RIInspectionResult
				{
					MPassed = true,
					MNoNeedInspect = true,
					MNoLinkUrlIfPassed = true
				};
			}
			return GetResultByBalanceHasValue(ctx, category, year, period, "4103", -1, false);
		}

		private RIInspectionResult GetPeriodTransferResult(MContext ctx, RICategoryModel category, int year, int period, int typeID, string code)
		{
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			BDAccountModel bDAccountModel = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == code);
			if (bDAccountModel == null)
			{
				return new RIInspectionResult
				{
					MPassed = true,
					MNoLinkUrlIfPassed = true,
					MAccountIsDisable = true
				};
			}
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = (dataPool.PeriodTransferList != null && dataPool.PeriodTransferList.Count > 0 && dataPool.PeriodTransferList.Exists((GLPeriodTransferModel x) => x.MTransferTypeID == typeID));
			rIInspectionResult.MUrlParam = new string[2]
			{
				(year * 100 + period).ToString(),
				(bDAccountModel == null) ? string.Empty : bDAccountModel.MItemID
			};
			rIInspectionResult.MNoLinkUrlIfPassed = true;
			return rIInspectionResult;
		}

		private RIInspectionResult GetResultByCreditDebitHasValue(MContext ctx, RICategoryModel category, int year, int period, string code, int dir, bool hasValue = true)
		{
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			BDAccountModel bDAccountModel = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == code);
			if (bDAccountModel == null)
			{
				return new RIInspectionResult
				{
					MNoLinkUrlIfPassed = true,
					MPassed = true,
					MAccountIsDisable = true
				};
			}
			bool flag = dataPool.BalanceList.Exists((GLBalanceModel x) => x.MAccountCode.StartsWith(code) && ((dir == 1) ? (x.MDebit != decimal.Zero || x.MDebitFor != decimal.Zero) : ((dir == -1) ? (x.MCredit != decimal.Zero || x.MCreditFor != decimal.Zero) : (x.MDebit != decimal.Zero || x.MDebitFor != decimal.Zero || x.MCredit != decimal.Zero || x.MCreditFor != decimal.Zero))));
			flag = (hasValue ? flag : (!flag));
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = flag;
			rIInspectionResult.MUrlParam = new string[2]
			{
				(year * 100 + period).ToString(),
				(bDAccountModel == null) ? string.Empty : bDAccountModel.MItemID
			};
			rIInspectionResult.MNoLinkUrlIfPassed = true;
			return rIInspectionResult;
		}

		private RIInspectionResult GetResultByBalanceHasValue(MContext ctx, RICategoryModel category, int year, int period, string code, int dir = 1, bool hasValue = true)
		{
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			BDAccountModel bDAccountModel = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == code);
			if (bDAccountModel == null)
			{
				return new RIInspectionResult
				{
					MNoLinkUrlIfPassed = true,
					MPassed = true,
					MAccountIsDisable = true
				};
			}
			bool flag = dataPool.BalanceList.Exists((GLBalanceModel x) => x.MAccountCode.StartsWith(code) && x.MBeginBalance + ((dir == 1) ? x.MDebit : x.MCredit) - ((dir == 1) ? x.MCredit : x.MDebit) != decimal.Zero);
			flag = (hasValue ? flag : (!flag));
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = flag;
			rIInspectionResult.MUrlParam = new string[2]
			{
				(year * 100 + period).ToString(),
				(bDAccountModel == null) ? string.Empty : bDAccountModel.MItemID
			};
			rIInspectionResult.MNoLinkUrlIfPassed = true;
			return rIInspectionResult;
		}
	}
}
