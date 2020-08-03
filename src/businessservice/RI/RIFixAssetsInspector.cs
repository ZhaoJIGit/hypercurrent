using JieNor.Megi.BusinessContract.RI;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.RI
{
	public class RIFixAssetsInspector : IRIInspectable
	{
		public RIFixAssetsInspector()
		{
			base.enginers = new List<Func<MContext, RICategoryModel, int, int, RIInspectionResult>>
			{
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(FA_Cash),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(FA_BankDeposit),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(FA_Material),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(FA_Inventory),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(FA_FixAssets),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(FA_CIP),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(FA_InvisibleAssets),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(FA_OtherAssets)
			};
		}

		public RIInspectionResult FA_Cash(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceNegative(ctx, category, year, period, new List<string>
			{
				"1001"
			}, null, false, null);
		}

		public RIInspectionResult FA_BankDeposit(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceNegative(ctx, category, year, period, new List<string>
			{
				"1002"
			}, null, false, null);
		}

		public RIInspectionResult FA_Material(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceNegative(ctx, category, year, period, new List<string>
			{
				"1403"
			}, null, false, null);
		}

		public RIInspectionResult FA_Inventory(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceNegative(ctx, category, year, period, new List<string>
			{
				"1405"
			}, null, false, null);
		}

		public RIInspectionResult FA_FixAssets(MContext ctx, RICategoryModel category, int year, int period)
		{
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			List<RIInspectionResult> list = new List<RIInspectionResult>();
			BDAccountModel bDAccountModel = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == "1601");
			BDAccountModel bDAccountModel2 = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == "1602");
			BDAccountModel bDAccountModel3 = null;
			if (bDAccountModel == null || bDAccountModel2 == null)
			{
				return new RIInspectionResult
				{
					MPassed = true,
					MNoLinkUrlIfPassed = true,
					MAccountIsDisable = true
				};
			}
			if (ctx.MAccountTableID == "1")
			{
				bDAccountModel3 = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == "1603");
				if (bDAccountModel3 == null)
				{
					return new RIInspectionResult
					{
						MPassed = true,
						MNoLinkUrlIfPassed = true,
						MAccountIsDisable = true
					};
				}
			}
			decimal d = (from x in dataPool.BalanceList
			where x.MAccountCode == "1601"
			select x).Sum((GLBalanceModel x) => x.MBeginBalance + (decimal)x.MDC * (x.MDebit - x.MCredit));
			decimal d2 = (from x in dataPool.BalanceList
			where x.MAccountCode == "1602"
			select x).Sum((GLBalanceModel x) => x.MBeginBalance + (decimal)x.MDC * (x.MDebit - x.MCredit));
			decimal d3 = (from x in dataPool.BalanceList
			where x.MAccountCode == "1603"
			select x).Sum((GLBalanceModel x) => x.MBeginBalance + (decimal)x.MDC * (x.MDebit - x.MCredit));
			int num;
			if (d - d2 - d3 < decimal.Zero)
			{
				List<RIInspectionResult> list2 = list;
				RIInspectionResult rIInspectionResult = new RIInspectionResult
				{
					MMessageParam = new string[5]
					{
						bDAccountModel.MFullName,
						COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "Balance", "余额"),
						" < ",
						bDAccountModel2.MFullName,
						COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "Balance", "余额") + " + " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "Accounts", "科目") + ((bDAccountModel3 == null) ? "" : "[") + ((bDAccountModel3 == null) ? "" : bDAccountModel3.MFullName) + ((bDAccountModel3 == null) ? "" : "]") + ((bDAccountModel3 == null) ? "" : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "Balance", "余额"))
					}
				};
				RIInspectionResult rIInspectionResult2 = rIInspectionResult;
				string[] obj = new string[2];
				num = year * 100 + period;
				obj[0] = num.ToString();
				obj[1] = bDAccountModel.MItemID;
				rIInspectionResult2.MUrlParam = obj;
				list2.Add(rIInspectionResult);
			}
			BDAccountModel bDAccountModel4 = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == "660205");
			if (bDAccountModel4 != null)
			{
				decimal d4 = (from x in dataPool.BalanceList
				where x.MAccountCode == "1602"
				select x).Sum((GLBalanceModel x) => x.MCredit);
				decimal d5 = (from x in dataPool.BalanceList
				where x.MAccountCode == "660205"
				select x).Sum((GLBalanceModel x) => x.MDebit);
				if (d4 - d5 != decimal.Zero)
				{
					List<RIInspectionResult> list3 = list;
					RIInspectionResult rIInspectionResult = new RIInspectionResult
					{
						MMessageParam = new string[5]
						{
							bDAccountModel2.MFullName,
							COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "CreditAccrual", "贷方发生额"),
							"!=",
							bDAccountModel4.MFullName,
							COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "DebitAccrual", "借方发生额")
						}
					};
					RIInspectionResult rIInspectionResult3 = rIInspectionResult;
					string[] obj2 = new string[2];
					num = year * 100 + period;
					obj2[0] = num.ToString();
					obj2[1] = bDAccountModel.MItemID;
					rIInspectionResult3.MUrlParam = obj2;
					list3.Add(rIInspectionResult);
				}
			}
			if (list.Count == 0)
			{
				return new RIInspectionResult
				{
					MPassed = true,
					MNoLinkUrlIfPassed = true
				};
			}
			if (list.Count == 1)
			{
				return new RIInspectionResult
				{
					MPassed = false,
					MUrlParam = list[0].MUrlParam,
					MMessageParam = list[0].MMessageParam
				};
			}
			if (list.Count == 2)
			{
				RIInspectionResult rIInspectionResult = new RIInspectionResult();
				rIInspectionResult.MPassed = false;
				rIInspectionResult.MMessageParam = new string[1]
				{
					"2"
				};
				rIInspectionResult.children = list;
				return rIInspectionResult;
			}
			return new RIInspectionResult
			{
				MPassed = true,
				MNoLinkUrlIfPassed = true
			};
		}

		public RIInspectionResult FA_CIP(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceNegative(ctx, category, year, period, new List<string>
			{
				"1604"
			}, null, false, null);
		}

		public RIInspectionResult FA_InvisibleAssets(MContext ctx, RICategoryModel category, int year, int period)
		{
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			BDAccountModel bDAccountModel = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == "1701");
			BDAccountModel bDAccountModel2 = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == "1702");
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
			where x.MAccountCode == "1701"
			select x).Sum((GLBalanceModel x) => x.MBeginBalance + (decimal)x.MDC * (x.MDebit - x.MCredit));
			decimal d2 = (from x in dataPool.BalanceList
			where x.MAccountCode == "1702"
			select x).Sum((GLBalanceModel x) => x.MBeginBalance + (decimal)x.MDC * (x.MDebit - x.MCredit));
			if (d - d2 < decimal.Zero)
			{
				RIInspectionResult rIInspectionResult = new RIInspectionResult();
				rIInspectionResult.MPassed = false;
				rIInspectionResult.MMessageParam = new string[2]
				{
					bDAccountModel.MFullName,
					bDAccountModel2.MFullName
				};
				rIInspectionResult.MUrlParam = new string[2]
				{
					(year * 100 + period).ToString(),
					bDAccountModel.MItemID
				};
				return rIInspectionResult;
			}
			return new RIInspectionResult
			{
				MPassed = true,
				MNoLinkUrlIfPassed = true
			};
		}

		public RIInspectionResult FA_OtherAssets(MContext ctx, RICategoryModel category, int year, int period)
		{
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			List<string> accountCodeList = (from x in dataPool.AccountWithParentList
			where x.MAccountGroupID.Split(',').Contains("1")
			select x.MCode).ToList();
			return BalanceNegative(ctx, category, year, period, new List<string>
			{
				"1601",
				"1001",
				"1002",
				"1122",
				"1123",
				"1221",
				"1405",
				"1701",
				"1604",
				"1403"
			}, null, true, accountCodeList);
		}

		private RIInspectionResult BalanceNegative(MContext ctx, RICategoryModel category, int year, int period, List<string> codes, RIInspectionResult result = null, bool except = false, List<string> accountCodeList = null)
		{
			result = (result ?? new RIInspectionResult
			{
				children = new List<RIInspectionResult>()
			});
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			List<RIInspectionResult> list = new List<RIInspectionResult>();
			List<BDAccountModel> accountWithParentList = dataPool.AccountWithParentList;
			if (CheckAllAccountIsDisable(codes, accountWithParentList))
			{
				result.MPassed = true;
				result.MAccountIsDisable = true;
				result.MNoLinkUrlIfPassed = true;
				return result;
			}
			List<GLBalanceModel> balanceList = (from x in dataPool.LeafAccountBalanceList
			where x.MCheckGroupValueID == "0" && (accountCodeList == null || accountCodeList.Contains(x.MAccountCode)) && containsStartWith(codes, x.MAccountCode, except)
			select x).ToList();
			balanceList = (from x in GroupBalance(balanceList)
			where x.MBeginBalance + (decimal)x.MDC * (x.MDebit - x.MCredit) < decimal.Zero
			select x).ToList();
			int i = 0;
			int num;
			while (i < balanceList.Count)
			{
				List<RIInspectionResult> list2 = list;
				RIInspectionResult rIInspectionResult = new RIInspectionResult
				{
					MMessageParam = new string[3]
					{
						dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MItemID == balanceList[i].MAccountID).MFullName,
						ctx.MBasCurrencyID,
						(balanceList[i].MBeginBalance + (decimal)balanceList[i].MDC * (balanceList[i].MDebit - balanceList[i].MCredit)).ToMoneyFormat()
					}
				};
				RIInspectionResult rIInspectionResult2 = rIInspectionResult;
				string[] obj = new string[2];
				num = year * 100 + period;
				obj[0] = num.ToString();
				obj[1] = balanceList[i].MAccountID;
				rIInspectionResult2.MUrlParam = obj;
				rIInspectionResult.MNoLinkUrlIfPassed = true;
				list2.Add(rIInspectionResult);
				num = ++i;
			}
			result.children.AddRange(list);
			result.MPassed = (result.children.Count == 0);
			RIInspectionResult rIInspectionResult3 = result;
			string[] obj2 = new string[1];
			num = result.children.Count;
			obj2[0] = num.ToString();
			rIInspectionResult3.MMessageParam = obj2;
			result.MNoLinkUrlIfPassed = true;
			return result;
		}

		private bool CheckAllAccountIsDisable(List<string> codes, List<BDAccountModel> childrenAccount)
		{
			bool result = true;
			if (codes != null && codes.Count() > 0)
			{
				foreach (string code in codes)
				{
					BDAccountModel bDAccountModel = childrenAccount.FirstOrDefault((BDAccountModel x) => x.MCode.IndexOf(code) == 0);
					if (bDAccountModel != null)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		private bool containsStartWith(List<string> a, string b, bool except = false)
		{
			bool flag = false;
			int num = 0;
			while (num < a.Count)
			{
				if (!b.StartsWith(a[num]))
				{
					num++;
					continue;
				}
				flag = true;
				break;
			}
			return except ? (!flag) : flag;
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
