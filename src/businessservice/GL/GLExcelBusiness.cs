using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Formula;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.GL
{
	public class GLExcelBusiness : IGLExcelBusiness, IDataContract<GLExcelModel>
	{
		public GLVoucherBusiness voucher = new GLVoucherBusiness();

		public GLBalanceBusiness balance = new GLBalanceBusiness();

		public GLExcelRepository excelRep = new GLExcelRepository();

		private GLExcelFormulaHelper formulaHelper = new GLExcelFormulaHelper();

		public List<GLVoucherModel> GetVoucherListByFilter(MContext ctx, GLBalanceListFilterModel filter)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
			List<string> accountIDS = filter.AccountIDS;
			List<string> list = new List<string>();
			if (accountIDS != null)
			{
				foreach (string item in accountIDS)
				{
					list.AddRange(GetChildrenAccountIdList(item, baseBDAccountList));
				}
			}
			filter.AccountIDS = list;
			return excelRep.GetVoucherListByFilter(ctx, filter);
		}

		private List<string> GetChildrenAccountIdList(string parentAccountId, List<BDAccountModel> accountList)
		{
			List<string> list = new List<string>();
			List<BDAccountModel> list2 = (from x in accountList
			where x.MParentID == parentAccountId
			select x).ToList();
			if (list2 == null || list2.Count() == 0)
			{
				list.Add(parentAccountId);
				return list;
			}
			foreach (BDAccountModel item in list2)
			{
				list.AddRange(GetChildrenAccountIdList(item.MItemID, accountList));
			}
			return list;
		}

		public List<GLBalanceModel> GetBalanceListByFilter(MContext ctx, GLBalanceListFilterModel filter)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<string> accountIDS = filter.AccountIDS;
			List<BDAccountModel> list = null;
			if (filter.FormaluDataType == 7 || filter.FormaluDataType == 8)
			{
				if (filter.AccountIDS == null || filter.AccountIDS.Count() == 0)
				{
					return new List<GLBalanceModel>();
				}
				list = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
				List<BDAccountModel> list2 = (from x in list
				where filter.AccountIDS.Contains(x.MItemID)
				select x).ToList();
				if (list2 == null || list2.Count() == 0)
				{
					return new List<GLBalanceModel>();
				}
				List<string> profitLossAccountTypeList = GetProfitLossAccountTypeList(ctx);
				if (list2.Exists((BDAccountModel x) => !profitLossAccountTypeList.Contains(x.MAccountTypeID)))
				{
					return new List<GLBalanceModel>();
				}
			}
			if (!filter.IsIncludeChildrenAccount)
			{
				List<string> list3 = new List<string>();
				list = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
				if (accountIDS != null && accountIDS.Count() > 0)
				{
					foreach (string item in accountIDS)
					{
						BDAccountModel bDAccountModel = (from x in list
						where x.MItemID == item
						select x).FirstOrDefault();
						if (bDAccountModel != null)
						{
							string mParentID = bDAccountModel.MParentID;
							if (!accountIDS.Contains(mParentID))
							{
								list3.Add(item);
							}
						}
					}
					filter.AccountIDS = list3;
				}
			}
			else
			{
				filter.AccountIDS = accountIDS;
			}
			if (filter.IsIncludeChildrenAccount && (filter.AccountIDS == null || filter.AccountIDS.Count() == 0))
			{
				list = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
				if (list != null)
				{
					filter.AccountIDS = (from x in list
					select x.MItemID).ToList();
				}
			}
			else
			{
				if (!filter.RequestIsNotFromFormula && (filter.AccountIDS == null || filter.AccountIDS.Count() == 0))
				{
					return new List<GLBalanceModel>();
				}
				if (filter.RequestIsNotFromFormula && (filter.AccountIDS == null || filter.AccountIDS.Count() == 0))
				{
					if (list == null)
					{
						list = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
					}
					filter.AccountIDS = (from x in list
					where x.MParentID == "0"
					select x.MItemID).ToList();
				}
			}
			List<GLBalanceModel> list4 = excelRep.GetBalanceListByCheckGroupFilter(ctx, filter);
			if (filter.ExcludeCLPVoucher)
			{
				GLPeriodTransferBusiness gLPeriodTransferBusiness = new GLPeriodTransferBusiness();
				List<GLVoucherModel> list5 = null;
				list5 = ((filter.FormaluDataType != 8) ? gLPeriodTransferBusiness.GetTransferVoucherList(ctx, filter.StartYear, filter.StartPeriod, filter.EndYear, filter.EndPeriod, 7, 1) : gLPeriodTransferBusiness.GetTransferVoucherList(ctx, filter.StartYear, 1, filter.EndYear, filter.EndPeriod, 7, 1));
				if (list5 != null)
				{
					list4 = FilterTransferBalanceList(ctx, list5, list4, list, filter);
				}
			}
			return list4;
		}

		public List<GLVoucherModel> GetTransferVoucherList(MContext ctx, GLBalanceListFilterModel filter)
		{
			GLPeriodTransferBusiness gLPeriodTransferBusiness = new GLPeriodTransferBusiness();
			return gLPeriodTransferBusiness.GetTransferVoucherList(ctx, filter.StartYear, 1, filter.EndYear, filter.EndPeriod, 7, 1);
		}

		public List<BDAccountModel> GetAccountList(MContext ctx)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			return bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
		}

		private List<string> GetProfitLossAccountTypeList(MContext ctx)
		{
			AccountTypeEnum accountTypeEnum = new AccountTypeEnum(ctx.MAccountTableID);
			return new List<string>
			{
				accountTypeEnum.OperatingRevenue,
				accountTypeEnum.OtherRevenue,
				accountTypeEnum.OperatingCostsAndTaxes,
				accountTypeEnum.OtherLoss,
				accountTypeEnum.PeriodCharge,
				accountTypeEnum.IncomeTax
			};
		}

		private List<GLBalanceModel> FilterTransferBalanceList(MContext ctx, List<GLVoucherModel> transferVoucherList, List<GLBalanceModel> balanceList, List<BDAccountModel> accountList, GLBalanceListFilterModel filter)
		{
			if (transferVoucherList == null || balanceList == null)
			{
				return balanceList;
			}
			bool flag = filter.IncludeCheckType && filter.CheckTypeValueList != null && filter.CheckTypeValueList.Count() > 0;
			List<GLVoucherEntryModel> transferVoucherEntryList = new List<GLVoucherEntryModel>();
			transferVoucherList.ForEach(delegate(GLVoucherModel x)
			{
				if (x.MVoucherEntrys != null)
				{
					x.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel y)
					{
						y.MYear = x.MYear;
						y.MPeriod = x.MPeriod;
					});
					transferVoucherEntryList.AddRange(x.MVoucherEntrys);
				}
			});
			if (flag)
			{
				List<string> checkGroupValueIdList = (from x in balanceList
				select x.MCheckGroupValueID).ToList();
				transferVoucherEntryList = (from x in transferVoucherEntryList
				where checkGroupValueIdList.Contains(x.MCheckGroupValueID)
				select x).ToList();
			}
			if (!string.IsNullOrWhiteSpace(filter.MCurrencyID))
			{
				transferVoucherEntryList = (from x in transferVoucherEntryList
				where x.MCurrencyID == filter.MCurrencyID
				select x).ToList();
			}
			List<string> checkTypeGroupValueIdList = null;
			if (flag)
			{
				checkTypeGroupValueIdList = (from x in balanceList
				select x.MCheckGroupValueID).ToList();
				transferVoucherEntryList = (from x in transferVoucherEntryList
				where checkTypeGroupValueIdList.Contains(x.MCheckGroupValueID)
				select x).ToList();
			}
			if (transferVoucherEntryList == null)
			{
				return balanceList;
			}
			if (filter.FormaluDataType == 7)
			{
				foreach (GLBalanceModel balance3 in balanceList)
				{
					BDAccountModel bDAccountModel = (from x in accountList
					where x.MItemID == balance3.MAccountID
					select x).FirstOrDefault();
					if (bDAccountModel != null)
					{
						List<string> accountIdList = new List<string>();
						accountIdList.Add(bDAccountModel.MItemID);
						List<BDAccountModel> childrenAccountByRecursion = BDAccountHelper.GetChildrenAccountByRecursion(bDAccountModel, accountList, false);
						if (childrenAccountByRecursion != null)
						{
							accountIdList.AddRange(from x in childrenAccountByRecursion
							select x.MItemID);
						}
						List<GLVoucherEntryModel> voucherEntryList = (from x in transferVoucherEntryList
						where accountIdList.Contains(x.MAccountID) && x.MCurrencyID == balance3.MCurrencyID && x.MYear * 100 + x.MPeriod == balance3.MYearPeriod
						select x).ToList();
						ExcludeTransferVoucherAmountThisPeriod(ctx, balance3, voucherEntryList, bDAccountModel);
					}
				}
			}
			else if (filter.FormaluDataType == 8)
			{
				int endYearPeriod = filter.EndYearPeriod;
				DateTime mGLBeginDate = ctx.MGLBeginDate;
				int num = mGLBeginDate.Year * 100;
				mGLBeginDate = ctx.MGLBeginDate;
				bool flag2 = endYearPeriod == num + mGLBeginDate.Month;
				List<GLInitBalanceModel> list = new List<GLInitBalanceModel>();
				if (flag2)
				{
					List<string> list2 = (from x in balanceList
					select x.MAccountID).Distinct().ToList();
					GLInitBalanceListFilterModel gLInitBalanceListFilterModel = new GLInitBalanceListFilterModel();
					if (!string.IsNullOrWhiteSpace(filter.MCurrencyID))
					{
						gLInitBalanceListFilterModel.Equal("MCurrencyID", filter.MCurrencyID);
					}
					if (flag && checkTypeGroupValueIdList != null)
					{
						gLInitBalanceListFilterModel.In("MCheckGroupValueID", checkTypeGroupValueIdList);
					}
					if (list2 != null && list2.Count() > 0)
					{
						gLInitBalanceListFilterModel.In("MAccountID", list2);
					}
					GLInitBalanceBusiness gLInitBalanceBusiness = new GLInitBalanceBusiness();
					list = gLInitBalanceBusiness.GetInitBalanceList(ctx, gLInitBalanceListFilterModel);
				}
				foreach (GLBalanceModel balance4 in balanceList)
				{
					BDAccountModel bDAccountModel2 = (from x in accountList
					where x.MItemID == balance4.MAccountID
					select x).FirstOrDefault();
					if (bDAccountModel2 != null)
					{
						List<string> accountIdList2 = new List<string>();
						accountIdList2.Add(bDAccountModel2.MItemID);
						List<BDAccountModel> childrenAccountByRecursion2 = BDAccountHelper.GetChildrenAccountByRecursion(bDAccountModel2, accountList, false);
						if (childrenAccountByRecursion2 != null)
						{
							accountIdList2.AddRange(from x in childrenAccountByRecursion2
							select x.MItemID);
						}
						List<GLVoucherEntryModel> source = (from x in transferVoucherEntryList
						where accountIdList2.Contains(x.MAccountID) && x.MCurrencyID == balance4.MCurrencyID && x.MYear + x.MPeriod <= balance4.MYearPeriod
						select x).ToList();
						if (flag)
						{
							source = (from x in source
							where x.MCheckGroupValueID == balance4.MCheckGroupValueID
							select x).ToList();
						}
						bool flag3 = bDAccountModel2.MDC == 1;
						if (flag3)
						{
							balance4.MExcludeTransferVoucherYTDAmount = balance4.MYtdDebit - (balance4.MYtdCredit - (source.Sum((GLVoucherEntryModel x) => x.MCredit) - source.Sum((GLVoucherEntryModel x) => x.MDebit)));
							balance4.MExcludeTransferVoucherYTDAmountFor = balance4.MDebitFor - (balance4.MYtdCreditFor - (source.Sum((GLVoucherEntryModel x) => x.MCreditFor) - source.Sum((GLVoucherEntryModel x) => x.MDebitFor)));
						}
						else
						{
							balance4.MExcludeTransferVoucherYTDAmount = balance4.MYtdCredit - (balance4.MYtdDebit - (source.Sum((GLVoucherEntryModel x) => x.MDebit) - source.Sum((GLVoucherEntryModel x) => x.MCredit)));
							balance4.MExcludeTransferVoucherYTDAmountFor = balance4.MYtdCreditFor - (balance4.MYtdDebitFor - (source.Sum((GLVoucherEntryModel x) => x.MDebitFor) - source.Sum((GLVoucherEntryModel x) => x.MCreditFor)));
						}
						if (flag2 && list != null)
						{
							GLInitBalanceModel gLInitBalanceModel = (from x in list
							where x.MCurrencyID == balance4.MCurrencyID && x.MAccountID == balance4.MAccountID
							select x).FirstOrDefault();
							if (gLInitBalanceModel != null)
							{
								GLBalanceModel gLBalanceModel = balance4;
								gLBalanceModel.MExcludeTransferVoucherYTDAmount += (flag3 ? gLInitBalanceModel.MYtdCredit : gLInitBalanceModel.MYtdDebit);
								GLBalanceModel gLBalanceModel2 = balance4;
								gLBalanceModel2.MExcludeTransferVoucherYTDAmountFor += (flag3 ? gLInitBalanceModel.MYtdCreditFor : gLInitBalanceModel.MYtdDebitFor);
							}
						}
					}
				}
			}
			return balanceList;
		}

		public List<GLInitBalanceModel> GetInitBalanceList(MContext ctx, GLBalanceListFilterModel filter)
		{
			GLInitBalanceListFilterModel gLInitBalanceListFilterModel = new GLInitBalanceListFilterModel();
			if (!string.IsNullOrWhiteSpace(filter.MCurrencyID))
			{
				gLInitBalanceListFilterModel.Equal("MCurrencyID", filter.MCurrencyID);
			}
			if (filter.AccountIDS != null && filter.AccountIDS.Count() > 0)
			{
				gLInitBalanceListFilterModel.In("MAccountID", filter.AccountIDS);
			}
			if (filter.IncludeCheckType)
			{
				gLInitBalanceListFilterModel.IncludeCheckTypeData = true;
			}
			GLInitBalanceBusiness gLInitBalanceBusiness = new GLInitBalanceBusiness();
			return gLInitBalanceBusiness.GetInitBalanceList(ctx, gLInitBalanceListFilterModel);
		}

		private void ExcludeTransferVoucherAmountThisPeriod(MContext ctx, GLBalanceModel balance, List<GLVoucherEntryModel> voucherEntryList, BDAccountModel account)
		{
			bool flag = account.MDC == 1;
			decimal d = (voucherEntryList != null) ? (voucherEntryList.Sum((GLVoucherEntryModel x) => x.MCredit) - voucherEntryList.Sum((GLVoucherEntryModel x) => x.MDebit)) : decimal.Zero;
			decimal d2 = (voucherEntryList != null) ? (voucherEntryList.Sum((GLVoucherEntryModel x) => x.MCreditFor) - voucherEntryList.Sum((GLVoucherEntryModel x) => x.MDebitFor)) : decimal.Zero;
			decimal d3 = (voucherEntryList != null) ? (voucherEntryList.Sum((GLVoucherEntryModel x) => x.MDebit) - voucherEntryList.Sum((GLVoucherEntryModel x) => x.MCredit)) : decimal.Zero;
			decimal d4 = (voucherEntryList != null) ? (voucherEntryList.Sum((GLVoucherEntryModel x) => x.MDebitFor) - voucherEntryList.Sum((GLVoucherEntryModel x) => x.MCreditFor)) : decimal.Zero;
			if (flag)
			{
				balance.MExcludeTransferVoucherActualAmount = balance.MDebit - (balance.MCredit - d);
				balance.MExcludeTransferVoucherActualAmountFor = balance.MDebitFor - (balance.MCreditFor - d2);
			}
			else
			{
				balance.MExcludeTransferVoucherActualAmount = balance.MCredit - (balance.MDebit - d3);
				balance.MExcludeTransferVoucherActualAmountFor = balance.MCreditFor - (balance.MDebitFor - d4);
			}
		}

		public DateTime GetGLBeginDate(MContext ctx)
		{
			return ctx.MGLBeginDate;
		}

		[Obsolete]
		public List<GLBalanceModel> GetBalanceListWithTrackByFilter(MContext ctx, GLBalanceListFilterModel filter)
		{
			List<GLVoucherModel> list = GetVoucherListByFilter(ctx, filter).ToList();
			List<GLVoucherEntryModel> entrys = new List<GLVoucherEntryModel>();
			list.ForEach(delegate(GLVoucherModel x)
			{
				entrys.AddRange(x.MVoucherEntrys);
			});
			if (filter.AccountIDS != null && filter.AccountIDS.Count > 0)
			{
				entrys = (from x in entrys
				where filter.AccountIDS.Contains(x.MAccountID)
				select x).ToList();
			}
			entrys = (from x in entrys
			orderby x.MAccountID
			orderby x.MTrackItem1
			orderby x.MTrackItem2
			orderby x.MTrackItem3
			orderby x.MTrackItem4
			orderby x.MTrackItem5
			select x).ToList();
			List<GLBalanceModel> list2 = new List<GLBalanceModel>();
			if (entrys == null || entrys.Count == 0)
			{
				return list2;
			}
			GLVoucherEntryModel currentEntry = entrys[0];
			GLBalanceModel gLBalanceModel = new GLBalanceModel
			{
				MAccountID = currentEntry.MAccountID,
				MDebit = currentEntry.MDebit,
				MCredit = currentEntry.MCredit,
				MYearPeriod = (from x in list
				where x.MItemID == currentEntry.MID
				select x).First().MYear * 100 + (from x in list
				where x.MItemID == currentEntry.MID
				select x).First().MPeriod,
				MCurrencyID = currentEntry.MCurrencyID,
				MTrackItem1 = currentEntry.MTrackItem1,
				MTrackItem2 = currentEntry.MTrackItem2,
				MTrackItem3 = currentEntry.MTrackItem3,
				MTrackItem4 = currentEntry.MTrackItem4,
				MTrackItem5 = currentEntry.MTrackItem5
			};
			for (int i = 1; i < entrys.Count; i++)
			{
				if (entrys[i].MAccountID != currentEntry.MAccountID || entrys[i].MTrackItem1 != currentEntry.MTrackItem1 || entrys[i].MTrackItem2 != currentEntry.MTrackItem2 || entrys[i].MTrackItem3 != currentEntry.MTrackItem3 || entrys[i].MTrackItem4 != currentEntry.MTrackItem4 || entrys[i].MTrackItem5 != currentEntry.MTrackItem5)
				{
					list2.Add(gLBalanceModel);
					gLBalanceModel = new GLBalanceModel
					{
						MAccountID = entrys[i].MAccountID,
						MDebit = entrys[i].MDebit,
						MCredit = entrys[i].MCredit,
						MCurrencyID = entrys[i].MCurrencyID,
						MTrackItem1 = entrys[i].MTrackItem1,
						MTrackItem2 = entrys[i].MTrackItem2,
						MTrackItem3 = entrys[i].MTrackItem3,
						MTrackItem4 = entrys[i].MTrackItem4,
						MTrackItem5 = entrys[i].MTrackItem5,
						MYearPeriod = (from x in list
						where x.MItemID == currentEntry.MID
						select x).First().MYear * 100 + (from x in list
						where x.MItemID == currentEntry.MID
						select x).First().MPeriod
					};
				}
				else
				{
					GLBalanceModel gLBalanceModel2 = gLBalanceModel;
					gLBalanceModel2.MDebit += entrys[i].MDebit;
					GLBalanceModel gLBalanceModel3 = gLBalanceModel;
					gLBalanceModel3.MCredit += entrys[i].MCredit;
				}
				currentEntry = entrys[i];
			}
			list2.Add(gLBalanceModel);
			List<string> list3 = new List<string>();
			foreach (GLBalanceModel item in list2)
			{
				if (!list3.Contains(item.MAccountID))
				{
					filter.StartPeriod = 1;
					filter.AccountIDS.Clear();
					filter.AccountIDS.Add(item.MAccountID);
					decimal[] ytdBalanceByTrack = GetYtdBalanceByTrack(ctx, filter);
					item.MYtdCredit = ytdBalanceByTrack[0];
					item.MYtdDebit = ytdBalanceByTrack[1];
					list3.Add(item.MAccountID);
				}
			}
			return list2;
		}

		public decimal[] GetYtdBalanceByTrack(MContext ctx, GLBalanceListFilterModel filter)
		{
			decimal[] array = new decimal[2];
			List<GLVoucherModel> list = (from x in GetVoucherListByFilter(ctx, filter)
			where x.MTransferTypeID != 7
			select x).ToList();
			foreach (GLVoucherModel item in list)
			{
				List<GLVoucherEntryModel> mVoucherEntrys = item.MVoucherEntrys;
				if (mVoucherEntrys != null && mVoucherEntrys.Count() > 0)
				{
					foreach (GLVoucherEntryModel item2 in mVoucherEntrys)
					{
						ref decimal val = ref array[0];
						val += item2.MCredit;
						ref decimal val2 = ref array[1];
						val2 += item2.MDebit;
					}
				}
			}
			return array;
		}

		public List<BatchFormaluModel> RefreshFormula(MContext ctx, List<BatchFormaluModel> formulaList)
		{
			int num = 1;
			int num2 = 2;
			List<DateTime> settledPeriodFromBeginDate = new GLSettlementRepository().GetSettledPeriodFromBeginDate(ctx, true, true);
			int maxYear = 0;
			int maxPeriod = 0;
			if (settledPeriodFromBeginDate != null && settledPeriodFromBeginDate.Count() > 0)
			{
				DateTime dateTime = settledPeriodFromBeginDate.Max();
				maxYear = dateTime.Year;
				maxPeriod = dateTime.Month;
			}
			Dictionary<int, GLBalanceListFilterModel> balanceFilterByFormulaCondition = GetBalanceFilterByFormulaCondition(formulaList, maxYear, maxPeriod);
			List<GLBalanceModel> balanceList = null;
			List<GLBalanceModel> balanceList2 = null;
			List<GLVoucherModel> transferVoucherList = null;
			List<GLInitBalanceModel> initBalanceList = null;
			List<BDAccountModel> accountList = null;
			foreach (int key in balanceFilterByFormulaCondition.Keys)
			{
				List<GLBalanceModel> balanceListByFilter = GetBalanceListByFilter(ctx, balanceFilterByFormulaCondition[key]);
				if (key == num)
				{
					balanceList = balanceListByFilter;
				}
				else if (key == num2)
				{
					balanceList2 = balanceListByFilter;
					transferVoucherList = GetTransferVoucherList(ctx, balanceFilterByFormulaCondition[key]);
					initBalanceList = GetInitBalanceList(ctx, balanceFilterByFormulaCondition[key]);
				}
			}
			foreach (BatchFormaluModel formula in formulaList)
			{
				GLBalanceListFilterModel filterByFormula = formulaHelper.GetFilterByFormula<GLBalanceListFilterModel>(formula);
				List<GLBalanceModel> list = null;
				list = ((!filterByFormula.ExcludeCLPVoucher) ? GetBalanceListByFormula(balanceList, filterByFormula, maxYear, maxPeriod) : GetBalanceListByFormula(ctx, balanceList2, filterByFormula, maxYear, maxPeriod, transferVoucherList, initBalanceList, accountList));
				if (list != null && list.Count() > 0)
				{
					decimal num3 = formula.FormulaResult = GetFormulaResult(formula, list);
				}
				else
				{
					formula.FormulaResult = decimal.Zero;
				}
			}
			return formulaList;
		}

		private List<GLBalanceModel> GetBalanceListByFormula(MContext ctx, List<GLBalanceModel> balanceList, GLBalanceListFilterModel balanceFilter, int maxYear, int maxPeriod, List<GLVoucherModel> transferVoucherList, List<GLInitBalanceModel> initBalanceList, List<BDAccountModel> accountList)
		{
			List<GLBalanceModel> balanceListByFormula = GetBalanceListByFormula(balanceList, balanceFilter, maxYear, maxPeriod);
			if (balanceListByFormula == null || balanceListByFormula.Count() == 0)
			{
				return balanceListByFormula;
			}
			if (transferVoucherList != null && transferVoucherList.Count() > 0)
			{
				balanceList = FilterTransferBalanceList(ctx, transferVoucherList, balanceList, accountList, balanceFilter);
			}
			return balanceList;
		}

		private List<GLBalanceModel> GetBalanceListByFormula(List<GLBalanceModel> balanceList, GLBalanceListFilterModel balanceFilter, int maxYear, int maxPeriod)
		{
			if (balanceList == null || balanceList.Count() == 0)
			{
				return null;
			}
			List<GLBalanceModel> list = null;
			if (balanceFilter.EndYear * 100 + balanceFilter.EndPeriod > maxYear * 100 + maxPeriod)
			{
				balanceFilter.EndYear = maxYear;
				balanceFilter.EndPeriod = maxPeriod;
			}
			list = (from x in balanceList
			where x.MYearPeriod >= balanceFilter.StartYear * 100 + balanceFilter.StartPeriod && x.MYearPeriod <= balanceFilter.EndYear * 100 + balanceFilter.EndPeriod
			select x).ToList();
			if (list != null && balanceFilter.AccountIDS != null && balanceFilter.AccountIDS.Count() > 0)
			{
				list = (from x in list
				where balanceFilter.AccountIDS.Contains(x.MAccountID)
				select x).ToList();
			}
			if (list != null && !string.IsNullOrWhiteSpace(balanceFilter.MCurrencyID))
			{
				list = (from x in list
				where x.MCurrencyID == balanceFilter.MCurrencyID
				select x).ToList();
			}
			if (list != null && balanceFilter.IncludeCheckType)
			{
				list = (from x in list
				where x.MCheckGroupValueID != "0"
				select x).ToList();
				if (balanceFilter.CheckTypeValueList != null && balanceFilter.CheckTypeValueList.Count() > 0)
				{
					IEnumerable<IGrouping<string, string>> enumerable = from x in balanceFilter.CheckTypeValueList
					group x.MValue by x.MName;
					List<GLBalanceModel> list2 = list;
					foreach (IGrouping<string, string> item in enumerable)
					{
						string key = item.Key;
						List<string> checkTypeValue = item.ToList();
						int num = -1;
						if (int.TryParse(key, out num))
						{
							switch (num)
							{
							case 0:
								list2 = (from x in list2
								where checkTypeValue.Contains(x.MCheckGroupValueModel.MContactID)
								select x).ToList();
								break;
							case 1:
								list2 = (from x in list2
								where checkTypeValue.Contains(x.MCheckGroupValueModel.MEmployeeID)
								select x).ToList();
								break;
							case 3:
								list2 = (from x in list2
								where checkTypeValue.Contains(x.MCheckGroupValueModel.MExpItemID)
								select x).ToList();
								break;
							case 2:
								list2 = (from x in list2
								where checkTypeValue.Contains(x.MCheckGroupValueModel.MMerItemID)
								select x).ToList();
								break;
							case 4:
								list2 = (from x in list2
								where checkTypeValue.Contains(x.MCheckGroupValueModel.MPaItemID) || checkTypeValue.Contains(x.MCheckGroupValueModel.MPaItemGroupID)
								select x).ToList();
								break;
							case 5:
								list2 = (from x in list2
								where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem1)
								select x).ToList();
								break;
							case 6:
								list2 = (from x in list2
								where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem2)
								select x).ToList();
								break;
							case 7:
								list2 = (from x in list2
								where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem3)
								select x).ToList();
								break;
							case 8:
								list2 = (from x in list2
								where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem4)
								select x).ToList();
								break;
							case 9:
								list2 = (from x in list2
								where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem5)
								select x).ToList();
								break;
							}
						}
					}
				}
			}
			return list;
		}

		private Dictionary<int, GLBalanceListFilterModel> GetBalanceFilterByFormulaCondition(List<BatchFormaluModel> formulaList, int maxYear, int maxPeriod)
		{
			Dictionary<int, GLBalanceListFilterModel> dictionary = new Dictionary<int, GLBalanceListFilterModel>();
			GLBalanceListFilterModel gLBalanceListFilterModel = null;
			GLBalanceListFilterModel gLBalanceListFilterModel2 = null;
			foreach (BatchFormaluModel formula in formulaList)
			{
				GLBalanceListFilterModel filterByFormula = formulaHelper.GetFilterByFormula<GLBalanceListFilterModel>(formula);
				if (filterByFormula != null)
				{
					GLBalanceListFilterModel gLBalanceListFilterModel3 = null;
					if (filterByFormula.ExcludeCLPVoucher)
					{
						gLBalanceListFilterModel2 = ((gLBalanceListFilterModel2 == null) ? new GLBalanceListFilterModel() : gLBalanceListFilterModel2);
						gLBalanceListFilterModel3 = gLBalanceListFilterModel2;
					}
					else
					{
						gLBalanceListFilterModel = ((gLBalanceListFilterModel == null) ? new GLBalanceListFilterModel() : gLBalanceListFilterModel);
						gLBalanceListFilterModel3 = gLBalanceListFilterModel;
					}
					gLBalanceListFilterModel3.StartYear = ((gLBalanceListFilterModel3.StartYear != 0 && gLBalanceListFilterModel3.StartYear > filterByFormula.StartYear) ? gLBalanceListFilterModel3.StartYear : filterByFormula.StartYear);
					gLBalanceListFilterModel3.StartPeriod = ((gLBalanceListFilterModel3.StartPeriod == 0 || filterByFormula.StartPeriod < gLBalanceListFilterModel3.StartPeriod) ? filterByFormula.StartPeriod : gLBalanceListFilterModel3.StartPeriod);
					gLBalanceListFilterModel3.EndYear = ((gLBalanceListFilterModel3.EndYear != 0 && gLBalanceListFilterModel3.EndYear > filterByFormula.EndYear) ? gLBalanceListFilterModel3.EndYear : filterByFormula.EndYear);
					gLBalanceListFilterModel3.EndPeriod = ((gLBalanceListFilterModel.EndPeriod == 0 || filterByFormula.EndPeriod > gLBalanceListFilterModel3.EndPeriod) ? filterByFormula.EndPeriod : gLBalanceListFilterModel3.EndPeriod);
					if (filterByFormula.AccountIDS == null || filterByFormula.AccountIDS.Count() == 0)
					{
						gLBalanceListFilterModel3.AccountIDS.Clear();
					}
					else
					{
						gLBalanceListFilterModel3.AccountIDS = ((gLBalanceListFilterModel3.AccountIDS == null) ? new List<string>() : gLBalanceListFilterModel3.AccountIDS);
						gLBalanceListFilterModel3.AccountIDS.AddRange(filterByFormula.AccountIDS);
					}
					gLBalanceListFilterModel3.IncludeCheckType = (gLBalanceListFilterModel3.IncludeCheckType || filterByFormula.IncludeCheckType);
				}
			}
			if (gLBalanceListFilterModel != null)
			{
				gLBalanceListFilterModel.RequestIsNotFromFormula = true;
				if (gLBalanceListFilterModel.EndYear * 100 + gLBalanceListFilterModel.EndPeriod > maxYear * 100 + maxPeriod)
				{
					gLBalanceListFilterModel.EndYear = maxYear;
					gLBalanceListFilterModel.EndPeriod = maxPeriod;
				}
				dictionary.Add(1, gLBalanceListFilterModel);
			}
			if (gLBalanceListFilterModel2 != null)
			{
				gLBalanceListFilterModel2.RequestIsNotFromFormula = true;
				if (gLBalanceListFilterModel2.EndYear * 100 + gLBalanceListFilterModel2.EndPeriod > maxYear * 100 + maxPeriod)
				{
					gLBalanceListFilterModel2.EndYear = maxYear;
					gLBalanceListFilterModel2.EndPeriod = maxPeriod;
				}
				dictionary.Add(2, gLBalanceListFilterModel2);
			}
			return dictionary;
		}

		private decimal GetFormulaResult(BatchFormaluModel formula, List<GLBalanceModel> balanceList)
		{
			decimal num = default(decimal);
			int maxYearPeriodqm;
			switch (formula.FormulaDataType)
			{
			case 0:
				num = balanceList.Sum((GLBalanceModel x) => x.MDebit);
				break;
			case 1:
				num = balanceList.Sum((GLBalanceModel x) => x.MCredit);
				break;
			case 2:
				foreach (GLBalanceModel balance2 in balanceList)
				{
					num = ((balance2.MDC != 1) ? (num + (balance2.MCredit - balance2.MDebit)) : (num + (balance2.MDebit - balance2.MCredit)));
				}
				break;
			case 3:
			case 4:
			{
				int maxYearPeriod = balanceList.Max((GLBalanceModel x) => x.MYearPeriod);
				balanceList = (from x in balanceList
				where x.MYearPeriod == maxYearPeriod
				select x).ToList();
				num = ((formula.FormulaDataType == 3) ? balanceList.Sum((GLBalanceModel x) => x.MYtdDebit) : balanceList.Sum((GLBalanceModel x) => x.MYtdCredit));
				break;
			}
			case 5:
			{
				int minYearPeriod = balanceList.Min((GLBalanceModel x) => x.MYearPeriod);
				balanceList = (from x in balanceList
				where x.MYearPeriod == minYearPeriod
				select x).ToList();
				num = balanceList.Sum((GLBalanceModel x) => x.MBeginBalance);
				break;
			}
			case 6:
				maxYearPeriodqm = balanceList.Max((GLBalanceModel x) => x.MYearPeriod);
				balanceList = (from x in balanceList
				where x.MYearPeriod == maxYearPeriodqm
				select x).ToList();
				foreach (GLBalanceModel balance3 in balanceList)
				{
					num = ((balance3.MDC != 1) ? (num + (balance3.MBeginBalance + balance3.MCredit - balance3.MDebit)) : (num + (balance3.MBeginBalance + balance3.MDebit - balance3.MCredit)));
				}
				break;
			case 7:
				num = balanceList.Sum((GLBalanceModel x) => x.MExcludeTransferVoucherActualAmount);
				break;
			case 8:
				maxYearPeriodqm = balanceList.Max((GLBalanceModel x) => x.MYearPeriod);
				balanceList = (from x in balanceList
				where x.MYearPeriod == maxYearPeriodqm
				select x).ToList();
				num = balanceList.Sum((GLBalanceModel x) => x.MExcludeTransferVoucherYTDAmount);
				break;
			}
			return num;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return false;
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return false;
		}

		public OperationResult InsertOrUpdate(MContext ctx, GLExcelModel modelData, string fields = null)
		{
			return null;
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<GLExcelModel> modelData, string fields = null)
		{
			return null;
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return null;
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return null;
		}

		public GLExcelModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return null;
		}

		public GLExcelModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return null;
		}

		public List<GLExcelModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return null;
		}

		public DataGridJson<GLExcelModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return null;
		}
	}
}
