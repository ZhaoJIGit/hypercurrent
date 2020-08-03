using JieNor.Megi.BusinessContract.RI;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JieNor.Megi.BusinessService.RI
{
	public class RIOtherInspector : IRIInspectable
	{
		private GLCheckOtherRepository otherRepository = new GLCheckOtherRepository();

		public RIOtherInspector()
		{
			base.enginers = new List<Func<MContext, RICategoryModel, int, int, RIInspectionResult>>
			{
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(OT_FixAssets),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(OT_BalanceSheet),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(OT_VoucherNumberBreak),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(OT_NewAccount)
			};
		}

		public RIInspectionResult OT_FixAssets(MContext ctx, RICategoryModel category, int year, int period)
		{
			DateTime dateTime = DateTime.ParseExact((year * 100 + period).ToString() + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			bool mPassed = otherRepository.CheckFixAsset(ctx, year, period);
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = mPassed;
			rIInspectionResult.MUrlParam = new string[1]
			{
				dateTime.ToString("yyyyMM")
			};
			return rIInspectionResult;
		}

		public RIInspectionResult OT_BalanceSheet(MContext ctx, RICategoryModel category, int year, int period)
		{
			RPTBalanceSheetBusiness rPTBalanceSheetBusiness = new RPTBalanceSheetBusiness();
			DateTime date = DateTime.ParseExact((year * 100 + period).ToString() + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			bool mPassed = rPTBalanceSheetBusiness.IsBalance(ctx, date);
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = mPassed;
			rIInspectionResult.MUrlParam = new string[1]
			{
				date.ToString("yyyy-MM-dd 00:00:00")
			};
			return rIInspectionResult;
		}

		public RIInspectionResult OT_VoucherNumberBreak(MContext ctx, RICategoryModel category, int year, int period)
		{
			List<string> list = otherRepository.CheckVoucherNumber(ctx, year, period);
			bool mPassed = true;
			List<string> list2 = new List<string>();
			if (list != null && list.Count > 0)
			{
				List<int> list3 = (from x in list
				select int.Parse(x)).ToList();
				int num = list3.Max();
				for (int i = 1; i <= num; i++)
				{
					if (!list3.Contains(i))
					{
						list2.Add(COMResourceHelper.ToVoucherNumber(ctx, null, i));
						mPassed = false;
						if (list2.Count == 10)
						{
							break;
						}
					}
				}
			}
			RIInspectionResult rIInspectionResult = new RIInspectionResult();
			rIInspectionResult.MPassed = mPassed;
			rIInspectionResult.MMessageParam = new string[1]
			{
				string.Join(",", list2.ToArray())
			};
			return rIInspectionResult;
		}

		public RIInspectionResult OT_NewAccount(MContext ctx, RICategoryModel category, int year, int period)
		{
			RIInspectionResult rIInspectionResult = new RIInspectionResult
			{
				children = new List<RIInspectionResult>()
			};
			int num = year * 12 + period;
			DateTime mGLBeginDate = ctx.MGLBeginDate;
			int num2 = mGLBeginDate.Year * 12;
			mGLBeginDate = ctx.MGLBeginDate;
			if (num > num2 + mGLBeginDate.Month + 6)
			{
				GLDataPool.RemovePool(ctx, false);
				GLDataPool dataPool = base.GetDataPool(ctx, year, period, 7, true);
				List<RIInspectionResult> list = new List<RIInspectionResult>();
				List<GLBalanceModel> balanceList = (from x in dataPool.LeafAccountBalanceList
				where x.MCheckGroupValueID == "0"
				select x).ToList();
				balanceList = GroupBalance(balanceList);
				List<KeyValuePair<int, List<GLBalanceModel>>> lastBalanceList = dataPool.GetLastBalanceList(7);
				int num5;
				if (lastBalanceList.Count == 6)
				{
					int i = 0;
					while (i < balanceList.Count)
					{
						KeyValuePair<int, List<GLBalanceModel>> keyValuePair = lastBalanceList[0];
						decimal num3 = (from x in keyValuePair.Value
						where x.MCheckGroupValueID == "0" && x.MAccountID == balanceList[i].MAccountID
						select x).Sum((GLBalanceModel x) => x.MEndBalance);
						decimal num4 = balanceList[i].MBeginBalance + (decimal)balanceList[i].MDC * (balanceList[i].MDebit - balanceList[i].MCredit);
						if (!(num3 == num4))
						{
							bool flag = false;
							for (int j = 1; j < lastBalanceList.Count; j++)
							{
								if (flag)
								{
									break;
								}
								keyValuePair = lastBalanceList[j];
								List<GLBalanceModel> source = (from x in keyValuePair.Value
								where x.MCheckGroupValueID == "0" && x.MAccountID == balanceList[i].MAccountID
								select x).ToList();
								if (source.Sum((GLBalanceModel x) => x.MEndBalance) != num3)
								{
									flag = true;
								}
							}
							if (!flag)
							{
								BDAccountModel bDAccountModel = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MItemID == balanceList[i].MAccountID);
								List<RIInspectionResult> list2 = list;
								RIInspectionResult rIInspectionResult2 = new RIInspectionResult
								{
									MMessageParam = new string[5]
									{
										bDAccountModel.MFullName,
										"6",
										ctx.MBasCurrencyID,
										num3.ToMoneyFormat(),
										num4.ToMoneyFormat()
									}
								};
								RIInspectionResult rIInspectionResult3 = rIInspectionResult2;
								string[] obj = new string[2];
								num5 = year * 100 + period;
								obj[0] = num5.ToString();
								obj[1] = bDAccountModel.MItemID;
								rIInspectionResult3.MUrlParam = obj;
								list2.Add(rIInspectionResult2);
							}
						}
						num5 = ++i;
					}
				}
				rIInspectionResult.children.AddRange(list);
				RIInspectionResult rIInspectionResult4 = rIInspectionResult;
				string[] obj2 = new string[1];
				num5 = list.Count;
				obj2[0] = num5.ToString();
				rIInspectionResult4.MMessageParam = obj2;
			}
			rIInspectionResult.MPassed = (rIInspectionResult.children.Count == 0);
			rIInspectionResult.MNoLinkUrlIfPassed = true;
			return rIInspectionResult;
		}

		public List<GLBalanceModel> GroupBalance(List<GLBalanceModel> list)
		{
			return (from x in list
			group x by new
			{
				x.MAccountID,
				x.MDC
			} into y
			select new GLBalanceModel
			{
				MAccountID = y.Key.MAccountID,
				MDC = y.Key.MDC,
				MBeginBalance = y.Sum((GLBalanceModel z) => z.MBeginBalance),
				MDebit = y.Sum((GLBalanceModel z) => z.MDebit),
				MCredit = y.Sum((GLBalanceModel z) => z.MCredit)
			}).ToList();
		}
	}
}
