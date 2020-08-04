using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Formula;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.IV;
using JieNor.Megi.ServiceContract.REG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class FormaluController : ApiController
	{
		private int NormalFilterType = 1;

		private int ExcludeCLPFilterType = 2;

		private DateTime GLBeginDate;

		[HttpPost]
		public HttpResponseMessage BatchRefreshAcctFormalu(string token, List<BatchFormaluModel> formulaList)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				BatchFormaluModel batchFormaluModel = formulaList.FirstOrDefault();
				if (batchFormaluModel != null)
				{
					string apiModule = batchFormaluModel.ApiModule;
					if (!string.IsNullOrEmpty(apiModule))
					{
						string text = "";
						switch (apiModule)
						{
						case "Export":
						case "Pivotable":
							text = "Other_ReportsExport";
							break;
						case "Function":
							text = "General_LedgerChange";
							break;
						default:
							text = "General_LedgerView";
							break;
						}
						if (!AccessHelper.HaveAccess(text, token))
						{
							return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
						}
					}
				}
				int maxYear = 0;
				int maxPeriod = 0;
				IGLSettlement sysService = ServiceHostManager.GetSysService<IGLSettlement>();
				using (sysService as IDisposable)
				{
					List<DateTime> resultData = sysService.GetSettledPeriodFromBeginDate(true, token).ResultData;
					if (resultData != null && resultData.Count() > 0)
					{
						DateTime dateTime = resultData.Max();
						maxYear = dateTime.Year;
						maxPeriod = dateTime.Month;
					}
				}
				List<GLBalanceModel> balanceList = null;
				List<GLBalanceModel> collection = null;
				List<GLVoucherModel> transferVoucherList = null;
				List<GLInitBalanceModel> initBalanceList = null;
				List<BDAccountModel> accountList = null;
				BASCurrencyViewModel bASCurrencyViewModel = null;
				List<GLBalanceModel> list = null;
				IREGCurrency sysService2 = ServiceHostManager.GetSysService<IREGCurrency>();
				using (sysService2 as IDisposable)
				{
					bASCurrencyViewModel = sysService2.GetBaseCurrency(token).ResultData;
				}
				IGLExcel sysService3 = ServiceHostManager.GetSysService<IGLExcel>();
				using (sysService3 as IDisposable)
				{
					accountList = sysService3.GetAccountList(token).ResultData;
				}
				Dictionary<int, GLBalanceListFilterModel> balanceFilterByFormulaCondition = GetBalanceFilterByFormulaCondition(formulaList, maxYear, maxPeriod, accountList);
				string mCurrencyID = bASCurrencyViewModel.MCurrencyID;
				IGLExcel sysService4 = ServiceHostManager.GetSysService<IGLExcel>();
				using (sysService4 as IDisposable)
				{
					foreach (int key in balanceFilterByFormulaCondition.Keys)
					{
						GLBalanceListFilterModel gLBalanceListFilterModel = balanceFilterByFormulaCondition[key];
						List<GLBalanceModel> resultData2 = sysService4.GetBalanceListByFilter(gLBalanceListFilterModel, token).ResultData;
						if (key == NormalFilterType)
						{
							balanceList = resultData2;
						}
						else if (key == ExcludeCLPFilterType)
						{
							collection = resultData2;
							bool includeCheckType = gLBalanceListFilterModel.IncludeCheckType;
							if (includeCheckType)
							{
								gLBalanceListFilterModel.IncludeCheckType = !includeCheckType;
								list = sysService4.GetBalanceListByFilter(gLBalanceListFilterModel, token).ResultData;
								gLBalanceListFilterModel.IncludeCheckType = includeCheckType;
							}
							transferVoucherList = sysService4.GetTransferVoucherList(balanceFilterByFormulaCondition[key], token).ResultData;
							initBalanceList = sysService4.GetInitBalanceList(balanceFilterByFormulaCondition[key], token).ResultData;
							GLBeginDate = sysService4.GetGLBeginDate(token).ResultData;
						}
					}
				}
				foreach (BatchFormaluModel formula in formulaList)
				{
					GLBalanceListFilterModel filterByFormula = formula.GetFilterByFormula<GLBalanceListFilterModel>();
					filterByFormula.AccountIDS = FilterAccountInFormula(accountList, filterByFormula);
					List<GLBalanceModel> list2 = null;
					if (filterByFormula.ExcludeCLPVoucher)
					{
						List<GLBalanceModel> list3 = new List<GLBalanceModel>();
						if (list != null && !filterByFormula.IncludeCheckType)
						{
							list3.AddRange(list);
						}
						else
						{
							list3.AddRange(collection);
						}
						list2 = GetBalanceListByFormula(list3, filterByFormula, maxYear, maxPeriod, transferVoucherList, initBalanceList, accountList);
					}
					else
					{
						list2 = GetBalanceListByFormula(balanceList, filterByFormula, maxYear, maxPeriod);
					}
					if (list2 != null && list2.Count() > 0)
					{
						decimal num = formula.FormulaResult = GetFormulaResult(formula, list2, mCurrencyID, accountList);
					}
					else
					{
						formula.FormulaResult = decimal.Zero;
					}
				}
				return ResponseHelper.toJson(formulaList, true, null, false);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}

		private decimal GetFormulaResult(BatchFormaluModel formula, List<GLBalanceModel> balanceList, string baseCurrency, List<BDAccountModel> accountList)
		{
			decimal num = default(decimal);
			GLBalanceListFilterModel gLBalanceListFilterModel = formula.AnalysisMegiAcct();
			bool flag = string.IsNullOrWhiteSpace(gLBalanceListFilterModel.MCurrencyID) || gLBalanceListFilterModel.MCurrencyID == baseCurrency;
			int maxYearPeriodqm;
			switch (formula.FormulaDataType)
			{
			case 0:
				num = (flag ? balanceList.Sum((GLBalanceModel x) => x.MDebit) : balanceList.Sum((GLBalanceModel x) => x.MDebitFor));
				break;
			case 1:
				num = (flag ? balanceList.Sum((GLBalanceModel x) => x.MCredit) : balanceList.Sum((GLBalanceModel x) => x.MCreditFor));
				break;
			case 2:
			{
				foreach (GLBalanceModel balance in balanceList)
				{
					num = ((balance.MDC != 1) ? (num + (flag ? (balance.MCredit - balance.MDebit) : (balance.MCreditFor - balance.MDebitFor))) : (num + (flag ? (balance.MDebit - balance.MCredit) : (balance.MDebitFor - balance.MCreditFor))));
				}
				return num;
			}
			case 3:
			case 4:
			{
				int maxYearPeriod = balanceList.Max((GLBalanceModel x) => x.MYearPeriod);
				balanceList = (from x in balanceList
				where x.MYearPeriod == maxYearPeriod
				select x).ToList();
				num = ((formula.FormulaDataType != 3) ? (flag ? balanceList.Sum((GLBalanceModel x) => x.MYtdCredit) : balanceList.Sum((GLBalanceModel x) => x.MYtdCreditFor)) : (flag ? balanceList.Sum((GLBalanceModel x) => x.MYtdDebit) : balanceList.Sum((GLBalanceModel x) => x.MYtdDebitFor)));
				break;
			}
			case 5:
			{
				int minYearPeriod = balanceList.Min((GLBalanceModel x) => x.MYearPeriod);
				balanceList = (from x in balanceList
				where x.MYearPeriod == minYearPeriod
				select x).ToList();
				int selectedAccountDirection = GetSelectedAccountDirection(accountList, gLBalanceListFilterModel);
				List<GLBalanceModel> source = (from x in balanceList
				where x.MDC == 1
				select x).ToList();
				List<GLBalanceModel> source2 = (from x in balanceList
				where x.MDC == -1
				select x).ToList();
				source = (source ?? new List<GLBalanceModel>());
				source2 = (source2 ?? new List<GLBalanceModel>());
				num = ((!flag) ? ((selectedAccountDirection == -1) ? (source2.Sum((GLBalanceModel x) => x.MBeginBalanceFor) - source.Sum((GLBalanceModel x) => x.MBeginBalanceFor)) : (source.Sum((GLBalanceModel x) => x.MBeginBalanceFor) - source2.Sum((GLBalanceModel x) => x.MBeginBalanceFor))) : ((selectedAccountDirection == -1) ? (source2.Sum((GLBalanceModel x) => x.MBeginBalance) - source.Sum((GLBalanceModel x) => x.MBeginBalance)) : (source.Sum((GLBalanceModel x) => x.MBeginBalance) - source2.Sum((GLBalanceModel x) => x.MBeginBalance))));
				break;
			}
			case 6:
				maxYearPeriodqm = balanceList.Max((GLBalanceModel x) => x.MYearPeriod);
				balanceList = (from x in balanceList
				where x.MYearPeriod == maxYearPeriodqm
				select x).ToList();
				if (balanceList == null || balanceList.Count == 0)
				{
					num = default(decimal);
				}
				else
				{
					int selectedAccountDirection = GetSelectedAccountDirection(accountList, gLBalanceListFilterModel);
					balanceList.ForEach(delegate(GLBalanceModel x)
					{
						if (x.MDC == 1)
						{
							x.MEndBalance = x.MBeginBalance + x.MDebit - x.MCredit;
							x.MEndBalanceFor = x.MBeginBalanceFor + x.MDebitFor - x.MCreditFor;
						}
						else
						{
							x.MEndBalance = x.MBeginBalance + x.MCredit - x.MDebit;
							x.MEndBalanceFor = x.MBeginBalanceFor + x.MCreditFor - x.MDebitFor;
						}
					});
					List<GLBalanceModel> source = (from x in balanceList
					where x.MDC == 1
					select x).ToList();
					List<GLBalanceModel> source2 = (from x in balanceList
					where x.MDC == -1
					select x).ToList();
					num = ((!flag) ? ((selectedAccountDirection == -1) ? (source2.Sum((GLBalanceModel x) => x.MEndBalanceFor) - source.Sum((GLBalanceModel x) => x.MEndBalanceFor)) : (source.Sum((GLBalanceModel x) => x.MEndBalanceFor) - source2.Sum((GLBalanceModel x) => x.MEndBalanceFor))) : ((selectedAccountDirection == -1) ? (source2.Sum((GLBalanceModel x) => x.MEndBalance) - source.Sum((GLBalanceModel x) => x.MEndBalance)) : (source.Sum((GLBalanceModel x) => x.MEndBalance) - source2.Sum((GLBalanceModel x) => x.MEndBalance))));
				}
				break;
			case 7:
				num = (flag ? balanceList.Sum((GLBalanceModel x) => x.MExcludeTransferVoucherActualAmount) : balanceList.Sum((GLBalanceModel x) => x.MExcludeTransferVoucherActualAmountFor));
				break;
			case 8:
				maxYearPeriodqm = balanceList.Max((GLBalanceModel x) => x.MYearPeriod);
				balanceList = (from x in balanceList
				where x.MYearPeriod == maxYearPeriodqm
				select x).ToList();
				num = (flag ? balanceList.Sum((GLBalanceModel x) => x.MExcludeTransferVoucherYTDAmount) : balanceList.Sum((GLBalanceModel x) => x.MExcludeTransferVoucherYTDAmountFor));
				break;
			}
			return num;
		}

		private int GetSelectedAccountDirection(List<BDAccountModel> accountList, GLBalanceListFilterModel filter)
		{
			int result = 0;
			List<string> accountIdList = filter.AccountIDS;
			if (accountIdList != null && accountIdList.Count != 0)
			{
				List<BDAccountModel> list = (from x in accountList
				where accountIdList.Contains(x.MItemID)
				select x).ToList();
				List<BDAccountModel> list2 = (from x in list
				where x.MDC == 1
				select x).ToList();
				List<BDAccountModel> list3 = (from x in list
				where x.MDC == -1
				select x).ToList();
				if (list2.Count == list.Count)
				{
					result = 1;
				}
				else if (list3.Count == list.Count)
				{
					result = -1;
				}
				return result;
			}
			return result;
		}

		private Dictionary<int, GLBalanceListFilterModel> GetBalanceFilterByFormulaCondition(List<BatchFormaluModel> formulaList, int maxYear, int maxPeriod, List<BDAccountModel> accountList)
		{
			Dictionary<int, GLBalanceListFilterModel> dictionary = new Dictionary<int, GLBalanceListFilterModel>();
			GLBalanceListFilterModel gLBalanceListFilterModel = null;
			GLBalanceListFilterModel gLBalanceListFilterModel2 = null;
			bool flag = false;
			foreach (BatchFormaluModel formula in formulaList)
			{
				GLBalanceListFilterModel filterByFormula = formula.GetFilterByFormula<GLBalanceListFilterModel>();
				if (filterByFormula != null)
				{
					GLBalanceListFilterModel gLBalanceListFilterModel3 = null;
					if (filterByFormula.ExcludeCLPVoucher)
					{
						gLBalanceListFilterModel2 = ((gLBalanceListFilterModel2 == null) ? new GLBalanceListFilterModel() : gLBalanceListFilterModel2);
						gLBalanceListFilterModel2.IsIncludeChildrenAccount = true;
						gLBalanceListFilterModel3 = gLBalanceListFilterModel2;
					}
					else
					{
						gLBalanceListFilterModel = ((gLBalanceListFilterModel == null) ? new GLBalanceListFilterModel() : gLBalanceListFilterModel);
						gLBalanceListFilterModel.IsIncludeChildrenAccount = true;
						gLBalanceListFilterModel3 = gLBalanceListFilterModel;
					}
					gLBalanceListFilterModel3.StartYear = ((gLBalanceListFilterModel3.StartYear == 0 || gLBalanceListFilterModel3.StartYear > filterByFormula.StartYear) ? filterByFormula.StartYear : gLBalanceListFilterModel3.StartYear);
					gLBalanceListFilterModel3.StartPeriod = ((gLBalanceListFilterModel3.StartPeriod == 0 || gLBalanceListFilterModel3.StartPeriod > filterByFormula.StartPeriod) ? filterByFormula.StartPeriod : gLBalanceListFilterModel3.StartPeriod);
					gLBalanceListFilterModel3.EndYear = ((gLBalanceListFilterModel3.EndYear == 0 || gLBalanceListFilterModel3.EndYear < filterByFormula.EndYear) ? filterByFormula.EndYear : gLBalanceListFilterModel3.EndYear);
					gLBalanceListFilterModel3.EndPeriod = ((gLBalanceListFilterModel3.EndPeriod == 0 || gLBalanceListFilterModel3.EndPeriod < filterByFormula.EndPeriod) ? filterByFormula.EndPeriod : gLBalanceListFilterModel3.EndPeriod);
					if (filterByFormula.AccountIDS == null || filterByFormula.AccountIDS.Count() == 0)
					{
						gLBalanceListFilterModel3.AccountIDS = (gLBalanceListFilterModel3.AccountIDS ?? new List<string>());
						gLBalanceListFilterModel3.AccountIDS.Clear();
						flag = true;
					}
					else if (!flag)
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
				if (gLBalanceListFilterModel.AccountIDS != null && gLBalanceListFilterModel.AccountIDS.Count() > 0)
				{
					gLBalanceListFilterModel.AccountIDS = gLBalanceListFilterModel.AccountIDS.Distinct().ToList();
				}
				dictionary.Add(NormalFilterType, gLBalanceListFilterModel);
			}
			if (gLBalanceListFilterModel2 != null)
			{
				gLBalanceListFilterModel2.RequestIsNotFromFormula = true;
				if (gLBalanceListFilterModel2.EndYear * 100 + gLBalanceListFilterModel2.EndPeriod > maxYear * 100 + maxPeriod)
				{
					gLBalanceListFilterModel2.EndYear = maxYear;
					gLBalanceListFilterModel2.EndPeriod = maxPeriod;
				}
				if (gLBalanceListFilterModel2.AccountIDS != null && gLBalanceListFilterModel2.AccountIDS.Count() > 0)
				{
					gLBalanceListFilterModel2.AccountIDS = gLBalanceListFilterModel2.AccountIDS.Distinct().ToList();
				}
				dictionary.Add(ExcludeCLPFilterType, gLBalanceListFilterModel2);
			}
			return dictionary;
		}

		private List<GLBalanceModel> GetBalanceListByFormula(List<GLBalanceModel> balanceList, GLBalanceListFilterModel balanceFilter, int maxYear, int maxPeriod)
		{
			if (balanceList != null && balanceList.Count() != 0)
			{
				List<GLBalanceModel> list = null;
				if (balanceFilter.EndYear * 100 + balanceFilter.EndPeriod > maxYear * 100 + maxPeriod)
				{
					balanceFilter.EndYear = maxYear;
					balanceFilter.EndPeriod = maxPeriod;
				}
				list = balanceList.Where(delegate(GLBalanceModel x)
				{
					if (x.MYearPeriod >= balanceFilter.StartYear * 100 + balanceFilter.StartPeriod)
					{
						return x.MYearPeriod <= balanceFilter.EndYear * 100 + balanceFilter.EndPeriod;
					}
					return false;
				}).ToList();
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
									list2 = list2?.Where(delegate(GLBalanceModel x)
									{
										if (!checkTypeValue.Contains(x.MCheckGroupValueModel.MPaItemID))
										{
											return checkTypeValue.Contains(x.MCheckGroupValueModel.MPaItemGroupID);
										}
										return true;
									}).ToList();
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
						list = list2;
					}
				}
				return list;
			}
			return null;
		}

		private List<GLInitBalanceModel> GetInitBalanceListByFormula(List<GLInitBalanceModel> balanceList, GLBalanceListFilterModel balanceFilter)
		{
			if (balanceList != null && balanceList.Count() != 0)
			{
				List<GLInitBalanceModel> list = balanceList;
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
						List<GLInitBalanceModel> list2 = list;
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
									list2 = list2?.Where(delegate(GLInitBalanceModel x)
									{
										if (!checkTypeValue.Contains(x.MCheckGroupValueModel.MPaItemID))
										{
											return checkTypeValue.Contains(x.MCheckGroupValueModel.MPaItemGroupID);
										}
										return true;
									}).ToList();
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
						list = list2;
					}
				}
				return list;
			}
			return null;
		}

		private List<GLBalanceModel> GetBalanceListByFormula(List<GLBalanceModel> balanceList, GLBalanceListFilterModel balanceFilter, int maxYear, int maxPeriod, List<GLVoucherModel> transferVoucherList, List<GLInitBalanceModel> initBalanceList, List<BDAccountModel> accountList)
		{
			List<GLBalanceModel> balanceListByFormula = GetBalanceListByFormula(balanceList, balanceFilter, maxYear, maxPeriod);
			if (balanceListByFormula != null && balanceListByFormula.Count() != 0)
			{
				List<GLInitBalanceModel> initBalanceListByFormula = GetInitBalanceListByFormula(initBalanceList, balanceFilter);
				return FilterTransferBalanceList(transferVoucherList, balanceListByFormula, accountList, balanceFilter, initBalanceListByFormula);
			}
			return balanceListByFormula;
		}

		private List<GLBalanceModel> FilterTransferBalanceList(List<GLVoucherModel> transferVoucherList, List<GLBalanceModel> balanceList, List<BDAccountModel> accountList, GLBalanceListFilterModel filter, List<GLInitBalanceModel> initBalanceList)
		{
			if (transferVoucherList != null && balanceList != null)
			{
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
				List<GLBalanceModel>.Enumerator enumerator;
				if (filter.FormaluDataType == 7)
				{
					enumerator = balanceList.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							GLBalanceModel balance = enumerator.Current;
							BDAccountModel bDAccountModel = (from x in accountList
							where x.MItemID == balance.MAccountID
							select x).FirstOrDefault();
							if (bDAccountModel != null)
							{
								List<string> accountIdList = new List<string>();
								accountIdList.Add(bDAccountModel.MItemID);
								List<BDAccountModel> childrenAccountByRecursion = GetChildrenAccountByRecursion(bDAccountModel, accountList, false);
								if (childrenAccountByRecursion != null)
								{
									accountIdList.AddRange(from x in childrenAccountByRecursion
									select x.MItemID);
								}
								List<GLVoucherEntryModel> list = transferVoucherEntryList.Where(delegate(GLVoucherEntryModel x)
								{
									if (accountIdList.Contains(x.MAccountID) && x.MCurrencyID == balance.MCurrencyID)
									{
										return x.MYear * 100 + x.MPeriod == balance.MYearPeriod;
									}
									return false;
								}).ToList();
								if (flag)
								{
									list = (from x in list
									where x.MCheckGroupValueID == balance.MCheckGroupValueID
									select x).ToList();
								}
								ExcludeTransferVoucherAmountThisPeriod(balance, list, bDAccountModel);
							}
						}
						return balanceList;
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				if (filter.FormaluDataType == 8)
				{
					bool flag2 = filter.EndYear * 100 + filter.EndPeriod == GLBeginDate.Year * 100 + GLBeginDate.Month;
					enumerator = balanceList.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							GLBalanceModel balance2 = enumerator.Current;
							BDAccountModel bDAccountModel2 = (from x in accountList
							where x.MItemID == balance2.MAccountID
							select x).FirstOrDefault();
							if (bDAccountModel2 != null)
							{
								List<string> accountIdList2 = new List<string>();
								accountIdList2.Add(bDAccountModel2.MItemID);
								List<BDAccountModel> childrenAccountByRecursion2 = GetChildrenAccountByRecursion(bDAccountModel2, accountList, false);
								if (childrenAccountByRecursion2 != null)
								{
									accountIdList2.AddRange(from x in childrenAccountByRecursion2
									select x.MItemID);
								}
								List<GLVoucherEntryModel> source = transferVoucherEntryList.Where(delegate(GLVoucherEntryModel x)
								{
									if (accountIdList2.Contains(x.MAccountID) && x.MCurrencyID == balance2.MCurrencyID && x.MYear * 100 + x.MPeriod >= balance2.MYear * 100 + 1)
									{
										return x.MYear * 100 + x.MPeriod <= balance2.MYearPeriod;
									}
									return false;
								}).ToList();
								if (flag)
								{
									source = (from x in source
									where x.MCheckGroupValueID == balance2.MCheckGroupValueID
									select x).ToList();
								}
								bool flag3 = bDAccountModel2.MDC == 1;
								if (flag3)
								{
									balance2.MExcludeTransferVoucherYTDAmount = balance2.MYtdDebit - (balance2.MYtdCredit - (source.Sum((GLVoucherEntryModel x) => x.MCredit) - source.Sum((GLVoucherEntryModel x) => x.MDebit)));
									balance2.MExcludeTransferVoucherYTDAmountFor = balance2.MYtdDebitFor - (balance2.MYtdCreditFor - (source.Sum((GLVoucherEntryModel x) => x.MCreditFor) - source.Sum((GLVoucherEntryModel x) => x.MDebitFor)));
								}
								else
								{
									balance2.MExcludeTransferVoucherYTDAmount = balance2.MYtdCredit - (balance2.MYtdDebit - (source.Sum((GLVoucherEntryModel x) => x.MDebit) - source.Sum((GLVoucherEntryModel x) => x.MCredit)));
									balance2.MExcludeTransferVoucherYTDAmountFor = balance2.MYtdCreditFor - (balance2.MYtdDebitFor - (source.Sum((GLVoucherEntryModel x) => x.MDebitFor) - source.Sum((GLVoucherEntryModel x) => x.MCreditFor)));
								}
								if (flag2 && initBalanceList != null)
								{
									GLInitBalanceModel gLInitBalanceModel = initBalanceList.Where(delegate(GLInitBalanceModel x)
									{
										if (x.MCurrencyID == balance2.MCurrencyID && x.MAccountID == balance2.MAccountID)
										{
											return x.MCheckGroupValueID == balance2.MCheckGroupValueID;
										}
										return false;
									}).FirstOrDefault();
									if (gLInitBalanceModel != null)
									{
										GLBalanceModel gLBalanceModel = balance2;
										gLBalanceModel.MExcludeTransferVoucherYTDAmount += (flag3 ? gLInitBalanceModel.MYtdCredit : gLInitBalanceModel.MYtdDebit);
										GLBalanceModel gLBalanceModel2 = balance2;
										gLBalanceModel2.MExcludeTransferVoucherYTDAmountFor += (flag3 ? gLInitBalanceModel.MYtdCreditFor : gLInitBalanceModel.MYtdDebitFor);
									}
								}
							}
						}
						return balanceList;
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				return balanceList;
			}
			return balanceList;
		}

		private List<string> FilterAccountInFormula(List<BDAccountModel> accountList, GLBalanceListFilterModel filter)
		{
			List<string> list = filter.AccountIDS;
			if (accountList != null && accountList.Count() != 0)
			{
				if (filter.AccountIDS == null || filter.AccountIDS.Count() == 0)
				{
					list = (from x in accountList
					where x.MParentID == "0"
					select x.MItemID).ToList();
				}
				List<string> result = new List<string>();
				result.AddRange(list);
				if (list.Count() > 1)
				{
					foreach (string item in list)
					{
						List<BDAccountModel> childrenAccountByRecursion = GetChildrenAccountByRecursion(accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == item), accountList, false);
						List<string> childAccountIdList = (from x in childrenAccountByRecursion
						select x.MItemID).ToList();
						List<string> list2 = list.Where(delegate(string x)
						{
							if (x != item)
							{
								return childAccountIdList.Contains(x);
							}
							return false;
						}).ToList();
						if (list2.Count() > 0)
						{
							list2.ForEach(delegate(string x)
							{
								if (result.Exists((string y) => y == x))
								{
									result.Remove(x);
								}
							});
						}
					}
				}
				if (result.Count() > 0)
				{
					result = result.Distinct().ToList();
				}
				return result;
			}
			return list;
		}

		public static List<BDAccountModel> GetChildrenAccountByRecursion(BDAccountModel parentAccount, List<BDAccountModel> accoutList, bool isSelf)
		{
			List<BDAccountModel> list = new List<BDAccountModel>();
			if (isSelf)
			{
				list.Add(parentAccount);
			}
			List<BDAccountModel> list2 = (from x in accoutList
			where x.MParentID == parentAccount.MItemID
			select x).ToList();
			if (list2 != null && list2.Count() != 0)
			{
				{
					foreach (BDAccountModel item in list2)
					{
						list.AddRange(GetChildrenAccountByRecursion(item, accoutList, true));
					}
					return list;
				}
			}
			return list;
		}

		private void ExcludeTransferVoucherAmountThisPeriod(GLBalanceModel balance, List<GLVoucherEntryModel> voucherEntryList, BDAccountModel account)
		{
			bool num = account.MDC == 1;
			decimal d = (voucherEntryList != null) ? (voucherEntryList.Sum((GLVoucherEntryModel x) => x.MCredit) - voucherEntryList.Sum((GLVoucherEntryModel x) => x.MDebit)) : decimal.Zero;
			decimal d2 = (voucherEntryList != null) ? (voucherEntryList.Sum((GLVoucherEntryModel x) => x.MCreditFor) - voucherEntryList.Sum((GLVoucherEntryModel x) => x.MDebitFor)) : decimal.Zero;
			decimal d3 = (voucherEntryList != null) ? (voucherEntryList.Sum((GLVoucherEntryModel x) => x.MDebit) - voucherEntryList.Sum((GLVoucherEntryModel x) => x.MCredit)) : decimal.Zero;
			decimal d4 = (voucherEntryList != null) ? (voucherEntryList.Sum((GLVoucherEntryModel x) => x.MDebitFor) - voucherEntryList.Sum((GLVoucherEntryModel x) => x.MCreditFor)) : decimal.Zero;
			if (num)
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

		public HttpResponseMessage BitchRefreshInvoiceFormalu(string token, List<BatchFormaluModel> formulaList)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				new List<IVInvoiceListFilterModel>();
				IIVInvoice sysService = ServiceHostManager.GetSysService<IIVInvoice>();
				using (sysService as IDisposable)
				{
					foreach (BatchFormaluModel formula in formulaList)
					{
						decimal formulaResult = default(decimal);
						IVInvoiceListFilterModel filterByFormula = formula.GetFilterByFormula<IVInvoiceListFilterModel>();
						List<IVInvoiceListModel> rows = sysService.GetInvoiceList(filterByFormula, token).ResultData.rows;
						List<IVInvoiceListModel> list = new List<IVInvoiceListModel>();
						foreach (IGrouping<string, IVInvoiceListModel> item in from x in rows
						group x by x.MID)
						{
							List<IVInvoiceListModel> list2 = item.ToList();
							if (list2 != null && list2.Count() > 0)
							{
								list.Add(list2.First());
							}
						}
						if (list != null && list.Count() > 0)
						{
							switch (formula.FormulaDataType)
							{
							case 0:
								formulaResult = list.Sum((IVInvoiceListModel x) => x.MTaxTotalAmt);
								break;
							case 1:
								formulaResult = list.Sum((IVInvoiceListModel x) => x.MTotalAmt);
								break;
							case 2:
								formulaResult = list.Sum((IVInvoiceListModel x) => x.MTaxTotalAmt) - list.Sum((IVInvoiceListModel x) => x.MTotalAmt);
								break;
							case 3:
								formulaResult = list.Sum((IVInvoiceListModel x) => x.MTaxTotalAmt) - list.Sum((IVInvoiceListModel x) => x.MVerificationAmt);
								break;
							case 4:
								formulaResult = list.Sum((IVInvoiceListModel x) => x.MVerificationAmt);
								break;
							}
						}
						formula.FormulaResult = formulaResult;
					}
				}
				return ResponseHelper.toJson(formulaList, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}
	}
}
