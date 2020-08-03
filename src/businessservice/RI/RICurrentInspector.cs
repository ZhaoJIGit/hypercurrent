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
	public class RICurrentInspector : IRIInspectable
	{
		public RICurrentInspector()
		{
			base.enginers = new List<Func<MContext, RICategoryModel, int, int, RIInspectionResult>>
			{
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_Receivable_BalanceNegative),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_Receivable_BalanceVariable),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_Receivable_BalancePrereceive),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_Prepay_BalanceNegative),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_Prepay_BalanceVariable),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_OtherReceivable_BalanceNegative),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_OtherReceivable_BalanceVariable),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_OtherReceivable_BalanceOtherPayable),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_Payable_BalanceNegative),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_Payable_BalanceVariable),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_Payable_BalancePrepay),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_Prereceive_BalanceNegative),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_Prereceive_BalanceVariable),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_OtherPayable_BalanceNegative),
				new Func<MContext, RICategoryModel, int, int, RIInspectionResult>(CT_OtherPayable_BalanceVariable)
			};
		}

		public RIInspectionResult CT_Receivable_BalanceNegative(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceNegative(ctx, category, year, period, "1122", false, null);
		}

		public RIInspectionResult CT_Receivable_BalanceVariable(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceVariable(ctx, category, year, period, "1122", false, null);
		}

		public RIInspectionResult CT_Receivable_BalancePrereceive(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceWithTwoAccount(ctx, category, year, period, "1122", "2203", false, null);
		}

		public RIInspectionResult CT_Prepay_BalanceNegative(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceNegative(ctx, category, year, period, "1123", false, null);
		}

		public RIInspectionResult CT_Prepay_BalanceVariable(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceVariable(ctx, category, year, period, "1123", false, null);
		}

		public RIInspectionResult CT_OtherReceivable_BalanceNegative(MContext ctx, RICategoryModel category, int year, int period)
		{
			RIInspectionResult result = BalanceNegative(ctx, category, year, period, "1221", false, null);
			return BalanceNegative(ctx, category, year, period, "1221", true, result);
		}

		public RIInspectionResult CT_OtherReceivable_BalanceVariable(MContext ctx, RICategoryModel category, int year, int period)
		{
			RIInspectionResult result = BalanceVariable(ctx, category, year, period, "1221", false, null);
			return BalanceVariable(ctx, category, year, period, "1221", true, result);
		}

		public RIInspectionResult CT_OtherReceivable_BalanceOtherPayable(MContext ctx, RICategoryModel category, int year, int period)
		{
			RIInspectionResult result = BalanceWithTwoAccount(ctx, category, year, period, "1221", "2241", false, null);
			return BalanceWithTwoAccount(ctx, category, year, period, "1221", "2241", true, result);
		}

		public RIInspectionResult CT_Payable_BalanceNegative(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceNegative(ctx, category, year, period, "2202", false, null);
		}

		public RIInspectionResult CT_Payable_BalanceVariable(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceVariable(ctx, category, year, period, "2202", false, null);
		}

		public RIInspectionResult CT_Payable_BalancePrepay(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceWithTwoAccount(ctx, category, year, period, "2202", "1123", false, null);
		}

		public RIInspectionResult CT_Prereceive_BalanceNegative(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceNegative(ctx, category, year, period, "2203", false, null);
		}

		public RIInspectionResult CT_Prereceive_BalanceVariable(MContext ctx, RICategoryModel category, int year, int period)
		{
			return BalanceVariable(ctx, category, year, period, "2203", false, null);
		}

		public RIInspectionResult CT_OtherPayable_BalanceNegative(MContext ctx, RICategoryModel category, int year, int period)
		{
			RIInspectionResult result = BalanceNegative(ctx, category, year, period, "2241", false, null);
			return BalanceNegative(ctx, category, year, period, "2241", true, result);
		}

		public RIInspectionResult CT_OtherPayable_BalanceVariable(MContext ctx, RICategoryModel category, int year, int period)
		{
			RIInspectionResult result = BalanceVariable(ctx, category, year, period, "2241", false, null);
			return BalanceVariable(ctx, category, year, period, "2241", true, result);
		}

		private RIInspectionResult BalanceNegative(MContext ctx, RICategoryModel category, int year, int period, string accountCode, bool checkEmployee = false, RIInspectionResult result = null)
		{
			result = (result ?? new RIInspectionResult
			{
				MPassed = true,
				MNoLinkUrlIfPassed = true,
				children = new List<RIInspectionResult>()
			});
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			List<BDAccountModel> accountWithParentList = dataPool.AccountWithParentList;
			List<RIInspectionResult> list = new List<RIInspectionResult>();
			List<GLBalanceModel> balanceList = (from x in dataPool.LeafAccountBalanceList
			where x.MCheckGroupValueID != dataPool.EmptyCheckGroupValueID && x.MAccountCode.StartsWith(accountCode) && ((!checkEmployee) ? (!string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MContactID)) : (!string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MEmployeeID)))
			select x).ToList();
			balanceList = (from x in GroupBalanceByCheckGroupValue(balanceList, checkEmployee)
			where x.MBeginBalance + (decimal)x.MDC * (x.MDebit - x.MCredit) < decimal.Zero
			select x).ToList();
			int i = 0;
			int num;
			while (i < balanceList.Count)
			{
				BDAccountModel bDAccountModel = accountWithParentList.FirstOrDefault((BDAccountModel x) => x.MItemID == balanceList[i].MAccountID);
				if (bDAccountModel == null)
				{
					result.MPassed = true;
					result.MAccountIsDisable = true;
					return result;
				}
				List<RIInspectionResult> list2 = list;
				RIInspectionResult rIInspectionResult = new RIInspectionResult
				{
					MMessageParam = new string[5]
					{
						(!checkEmployee) ? COMMultiLangRepository.GetText(LangModule.Common, ctx.MLCID, "Contact") : COMMultiLangRepository.GetText(LangModule.BD, ctx.MLCID, "Employee"),
						(!checkEmployee) ? balanceList[i].MCheckGroupValueModel.MContactName : balanceList[i].MCheckGroupValueModel.MEmployeeName,
						dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MItemID == balanceList[i].MAccountID).MFullName,
						ctx.MBasCurrencyID,
						(balanceList[i].MBeginBalance + (decimal)balanceList[i].MDC * (balanceList[i].MDebit - balanceList[i].MCredit)).ToMoneyFormat()
					}
				};
				RIInspectionResult rIInspectionResult2 = rIInspectionResult;
				string[] obj = new string[4];
				num = year * 100 + period;
				obj[0] = num.ToString();
				obj[1] = ((!checkEmployee) ? "0" : "1");
				obj[2] = ((!checkEmployee) ? balanceList[i].MCheckGroupValueModel.MContactID : balanceList[i].MCheckGroupValueModel.MEmployeeID);
				obj[3] = balanceList[i].MAccountID;
				rIInspectionResult2.MUrlParam = obj;
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

		private RIInspectionResult BalanceVariable(MContext ctx, RICategoryModel category, int year, int period, string accountCode, bool checkEmployee = false, RIInspectionResult result = null)
		{
			result = (result ?? new RIInspectionResult
			{
				MPassed = true,
				children = new List<RIInspectionResult>(),
				MNoLinkUrlIfPassed = true
			});
			int num = (category.MSetting.MSettingParam == null || string.IsNullOrWhiteSpace(category.MSetting.MSettingParam.MCompareValue) || int.Parse(category.MSetting.MSettingParam.MCompareValue) <= 0) ? 6 : int.Parse(category.MSetting.MSettingParam.MCompareValue);
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, num, true);
			List<RIInspectionResult> list = new List<RIInspectionResult>();
			List<GLBalanceModel> balanceList = (from x in dataPool.LeafAccountBalanceList
			where x.MCheckGroupValueID != dataPool.EmptyCheckGroupValueID && x.MAccountCode.StartsWith(accountCode) && ((!checkEmployee) ? (!string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MContactID)) : (!string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MEmployeeID)))
			select x).ToList();
			if (balanceList == null || balanceList.Count == 0)
			{
				return result;
			}
			List<GLBalanceModel> list2 = GroupBalanceByCheckGroupValue(balanceList, checkEmployee);
			List<KeyValuePair<int, List<GLBalanceModel>>> lastBalanceList = dataPool.GetLastBalanceList(num);
			int num9;
			if (lastBalanceList.Count == num - 1)
			{
				List<GLBalanceModel> list3 = new List<GLBalanceModel>();
				int i;
				for (i = 0; i < list2.Count; num9 = ++i)
				{
					GLBalanceModel currentBalance = list2[i];
					KeyValuePair<int, List<GLBalanceModel>> keyValuePair = lastBalanceList[0];
					List<GLBalanceModel> source = (from x in keyValuePair.Value
					where ((!checkEmployee) ? (x.MCheckGroupValueModel.MContactID == currentBalance.MCheckGroupValueModel.MContactID) : (x.MCheckGroupValueModel.MEmployeeID == currentBalance.MCheckGroupValueModel.MEmployeeID)) && x.MAccountID == currentBalance.MAccountID
					select x).ToList();
					decimal num2 = source.Sum((GLBalanceModel x) => x.MBeginBalance);
					decimal num3 = source.Sum((GLBalanceModel x) => x.MDebit);
					decimal num4 = source.Sum((GLBalanceModel x) => x.MCredit);
					bool flag;
					int num8;
					if (!(num2 == decimal.Zero))
					{
						flag = false;
						decimal num5 = 0.0m;
						decimal num6 = 0.0m;
						for (int j = 0; j < lastBalanceList.Count && !flag; j++)
						{
							keyValuePair = lastBalanceList[j];
							List<GLBalanceModel> source2 = (from x in keyValuePair.Value
							where ((!checkEmployee) ? (x.MCheckGroupValueModel.MContactID == balanceList[i].MContactID) : (x.MCheckGroupValueModel.MEmployeeID == balanceList[i].MCheckGroupValueModel.MEmployeeID)) && x.MAccountID == balanceList[i].MAccountID
							select x).ToList();
							num5 = source2.Sum((GLBalanceModel x) => x.MDebit);
							num6 = source2.Sum((GLBalanceModel x) => x.MCredit);
							int num7;
							if (!(num5 == decimal.Zero) || !(num6 == decimal.Zero))
							{
								if (balanceList[i].MDC == 1 && num5 < decimal.Zero)
								{
									goto IL_040c;
								}
								if (balanceList[i].MDC == 1 && num6 > decimal.Zero)
								{
									goto IL_040c;
								}
								if (balanceList[i].MDC == -1 && num6 < decimal.Zero)
								{
									goto IL_040c;
								}
								num7 = ((balanceList[i].MDC == -1 && num5 > decimal.Zero) ? 1 : 0);
								goto IL_040d;
							}
							continue;
							IL_040d:
							flag = ((byte)num7 != 0);
							continue;
							IL_040c:
							num7 = 1;
							goto IL_040d;
						}
						if (!flag)
						{
							if (balanceList[i].MDC == 1 && currentBalance.MDebit < decimal.Zero)
							{
								goto IL_0552;
							}
							if (balanceList[i].MDC == 1 && currentBalance.MCredit > decimal.Zero)
							{
								goto IL_0552;
							}
							if (balanceList[i].MDC == -1 && currentBalance.MCredit < decimal.Zero)
							{
								goto IL_0552;
							}
							num8 = ((balanceList[i].MDC == -1 && currentBalance.MDebit > decimal.Zero) ? 1 : 0);
							goto IL_0553;
						}
						goto IL_0559;
					}
					continue;
					IL_0552:
					num8 = 1;
					goto IL_0553;
					IL_0559:
					if (!flag)
					{
						List<RIInspectionResult> list4 = list;
						RIInspectionResult rIInspectionResult = new RIInspectionResult
						{
							MMessageParam = new string[7]
							{
								(!checkEmployee) ? COMMultiLangRepository.GetText(LangModule.Common, ctx.MLCID, "Contact") : COMMultiLangRepository.GetText(LangModule.BD, ctx.MLCID, "Employee"),
								(!checkEmployee) ? currentBalance.MCheckGroupValueModel.MContactName : currentBalance.MCheckGroupValueModel.MEmployeeName,
								dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MItemID == currentBalance.MAccountID).MFullName,
								num.ToString(),
								ctx.MBasCurrencyID,
								num2.ToMoneyFormat(),
								(currentBalance.MBeginBalance + (decimal)currentBalance.MDC * (currentBalance.MDebit - currentBalance.MCredit)).ToMoneyFormat()
							}
						};
						RIInspectionResult rIInspectionResult2 = rIInspectionResult;
						string[] obj = new string[4];
						num9 = year * 100 + period;
						obj[0] = num9.ToString();
						obj[1] = ((!checkEmployee) ? "0" : "1");
						obj[2] = ((!checkEmployee) ? currentBalance.MCheckGroupValueModel.MContactID : currentBalance.MCheckGroupValueModel.MEmployeeID);
						obj[3] = currentBalance.MAccountID;
						rIInspectionResult2.MUrlParam = obj;
						list4.Add(rIInspectionResult);
					}
					continue;
					IL_0553:
					flag = ((byte)num8 != 0);
					bool flag2 = (byte)num8 != 0;
					goto IL_0559;
				}
			}
			result.children.AddRange(list);
			result.MPassed = (result.children.Count == 0);
			RIInspectionResult rIInspectionResult3 = result;
			string[] obj2 = new string[1];
			num9 = result.children.Count;
			obj2[0] = num9.ToString();
			rIInspectionResult3.MMessageParam = obj2;
			result.MNoLinkUrlIfPassed = true;
			return result;
		}

		private RIInspectionResult BalanceWithTwoAccount(MContext ctx, RICategoryModel category, int year, int period, string aCode, string bCode, bool checkEmployee = false, RIInspectionResult result = null)
		{
			result = (result ?? new RIInspectionResult
			{
				MPassed = true,
				MNoLinkUrlIfPassed = true,
				children = new List<RIInspectionResult>()
			});
			GLDataPool dataPool = base.GetDataPool(ctx, year, period, 6, true);
			List<RIInspectionResult> list = new List<RIInspectionResult>();
			BDAccountModel bDAccountModel = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == aCode);
			BDAccountModel bDAccountModel2 = dataPool.AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == bCode);
			if (bDAccountModel == null || bDAccountModel2 == null)
			{
				return new RIInspectionResult
				{
					MPassed = true,
					MNoLinkUrlIfPassed = true
				};
			}
			List<GLBalanceModel> aAccountBalanceList = (from x in dataPool.LeafAccountBalanceList
			where x.MCheckGroupValueID != dataPool.EmptyCheckGroupValueID && x.MAccountCode.StartsWith(aCode) && x.MBeginBalance + (decimal)x.MDC * (x.MDebit - x.MCredit) != decimal.Zero && ((!checkEmployee) ? (!string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MContactID)) : (!string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MEmployeeID)))
			select x).ToList();
			aAccountBalanceList = GroupBalanceByCheckGroupValue(aAccountBalanceList, checkEmployee);
			List<GLBalanceModel> balanceList = (from x in dataPool.LeafAccountBalanceList
			where x.MCheckGroupValueID != dataPool.EmptyCheckGroupValueID && x.MAccountCode.StartsWith(bCode) && x.MBeginBalance + (decimal)x.MDC * (x.MDebit - x.MCredit) != decimal.Zero && ((!checkEmployee) ? (!string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MContactID)) : (!string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MEmployeeID)))
			select x).ToList();
			balanceList = GroupBalanceByCheckGroupValue(balanceList, checkEmployee);
			int i = 0;
			int num;
			while (i < aAccountBalanceList.Count)
			{
				IEnumerable<GLBalanceModel> enumerable = from x in balanceList
				where (!checkEmployee) ? (x.MCheckGroupValueModel.MContactID == aAccountBalanceList[i].MCheckGroupValueModel.MContactID) : (x.MCheckGroupValueModel.MEmployeeID == aAccountBalanceList[i].MCheckGroupValueModel.MEmployeeID)
				select x;
				if (enumerable != null && enumerable.Count() > 0)
				{
					List<RIInspectionResult> list2 = list;
					RIInspectionResult rIInspectionResult = new RIInspectionResult
					{
						MMessageParam = new string[4]
						{
							(!checkEmployee) ? COMMultiLangRepository.GetText(LangModule.Common, ctx.MLCID, "Contact") : COMMultiLangRepository.GetText(LangModule.BD, ctx.MLCID, "Employee"),
							(!checkEmployee) ? aAccountBalanceList[i].MCheckGroupValueModel.MContactName : aAccountBalanceList[i].MCheckGroupValueModel.MEmployeeName,
							bDAccountModel.MFullName,
							bDAccountModel2.MFullName
						}
					};
					RIInspectionResult rIInspectionResult2 = rIInspectionResult;
					string[] obj = new string[4];
					num = year * 100 + period;
					obj[0] = num.ToString();
					obj[1] = ((!checkEmployee) ? "0" : "1");
					obj[2] = ((!checkEmployee) ? aAccountBalanceList[i].MCheckGroupValueModel.MContactID : aAccountBalanceList[i].MCheckGroupValueModel.MEmployeeID);
					obj[3] = aAccountBalanceList[i].MAccountID;
					rIInspectionResult2.MUrlParam = obj;
					list2.Add(rIInspectionResult);
				}
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

		private List<GLBalanceModel> GroupBalanceByCheckGroupValue(List<GLBalanceModel> balanceList, bool checkEmployee = false)
		{
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			if (!checkEmployee)
			{
				return (from x in balanceList
				group x by new
				{
					x.MCheckGroupValueModel.MContactID,
					x.MCheckGroupValueModel.MContactName,
					x.MDC,
					x.MAccountID
				} into y
				select new GLBalanceModel
				{
					MCheckGroupValueModel = new GLCheckGroupValueModel
					{
						MContactID = y.Key.MContactID,
						MContactName = y.Key.MContactName
					},
					MBeginBalance = y.Sum((GLBalanceModel z) => z.MBeginBalance),
					MDebit = y.Sum((GLBalanceModel z) => z.MDebit),
					MCredit = y.Sum((GLBalanceModel z) => z.MCredit),
					MDC = y.Key.MDC,
					MAccountID = y.Key.MAccountID,
					MEndBalance = y.Sum((GLBalanceModel z) => z.MBeginBalance) + (decimal)y.Key.MDC * (y.Sum((GLBalanceModel z) => z.MDebit) - y.Sum((GLBalanceModel z) => z.MCredit))
				}).ToList();
			}
			return (from x in balanceList
			group x by new
			{
				x.MCheckGroupValueModel.MEmployeeID,
				x.MCheckGroupValueModel.MEmployeeName,
				x.MDC,
				x.MAccountID
			} into y
			select new GLBalanceModel
			{
				MCheckGroupValueModel = new GLCheckGroupValueModel
				{
					MEmployeeID = y.Key.MEmployeeID,
					MEmployeeName = y.Key.MEmployeeName
				},
				MBeginBalance = y.Sum((GLBalanceModel z) => z.MBeginBalance),
				MDebit = y.Sum((GLBalanceModel z) => z.MDebit),
				MCredit = y.Sum((GLBalanceModel z) => z.MCredit),
				MDC = y.Key.MDC,
				MAccountID = y.Key.MAccountID,
				MEndBalance = y.Sum((GLBalanceModel z) => z.MBeginBalance) + (decimal)y.Key.MDC * (y.Sum((GLBalanceModel z) => z.MDebit) - y.Sum((GLBalanceModel z) => z.MCredit))
			}).ToList();
		}
	}
}
