using JieNor.Megi.BusinessContract;
using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataModel.RPT.GL;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.RPT.Biz;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.RPT
{
	public class AccountingDimensionSummaryBusiness : IRPTAccountDimensionSummaryBusiness, IRPTBizReportBusiness<RPTAccountDemensionSummaryFilterModel>
	{
		private BDAccountRepository AccountDal = new BDAccountRepository();

		private GLBalanceRepository BalanceDal = new GLBalanceRepository();

		private Dictionary<string, List<string>> DataColumns;

		private List<RPTAccountDimensionSummaryModel> DataList;

		private List<BDAccountModel> AccountList;

		private List<GLBalanceModel> BalanceList;

		private List<GLVoucherModel> VoucherList;

		private bool IncludeForeginCurrency = false;

		private FilterSchemeContentModel FilterSchemeContent;

		private List<string> CheckTypeName = new List<string>();

		public string GetBizReportJson(MContext ctx, RPTAccountDemensionSummaryFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			if (string.IsNullOrEmpty(filter.BaseCurrencyId))
			{
				filter.BaseCurrencyId = ctx.MBasCurrencyID;
			}
			IncludeForeginCurrency = (!string.IsNullOrEmpty(filter.MCurrencyID) && filter.MCurrencyID != "0" && filter.MCurrencyID != filter.BaseCurrencyId);
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, delegate
			{
				FilterSchemeContent = GetFilterScheme(ctx, filter);
				AccountList = GetAccountList(ctx, filter);
				BalanceList = GetBalanceList(ctx, filter);
				if (!filter.MDisplayNoAccurrenceAmount)
				{
					VoucherList = GetVoucherList(ctx, filter);
				}
				return GetReportModel(ctx, filter);
			});
		}

		private List<GLVoucherModel> GetVoucherList(MContext ctx, RPTAccountDemensionSummaryFilterModel filter)
		{
			GLVoucherListFilterModel gLVoucherListFilterModel = new GLVoucherListFilterModel();
			GLVoucherRepository gLVoucherRepository = new GLVoucherRepository();
			if (!string.IsNullOrEmpty(filter.MAccountID))
			{
				gLVoucherListFilterModel.AccountIDList = (from t in AccountList
				select t.MAccountID).Distinct().ToList();
			}
			gLVoucherListFilterModel.Status = (filter.IncludeUnapprovedVoucher ? null : "1");
			gLVoucherListFilterModel.MStartYearPeriod = ((!string.IsNullOrWhiteSpace(filter.MStartPeroid)) ? int.Parse(filter.MStartPeroid) : 0);
			gLVoucherListFilterModel.MEndYearPeriod = ((!string.IsNullOrWhiteSpace(filter.MEndPeroid)) ? int.Parse(filter.MEndPeroid) : 0);
			gLVoucherListFilterModel.IncludeDraft = false;
			gLVoucherListFilterModel.CheckGroupValueId = filter.CheckGroupValueId;
			return gLVoucherRepository.GetVoucherListIncludeEntry(ctx, gLVoucherListFilterModel);
		}

		private List<BDAccountModel> GetAccountList(MContext ctx, RPTAccountDemensionSummaryFilterModel filter)
		{
			SqlWhere filter2 = new SqlWhere();
			List<BDAccountModel> baseBDAccountList = AccountDal.GetBaseBDAccountList(ctx, filter2, filter.IsShowDisabledAccount, null);
			List<BDAccountModel> list = null;
			if (!string.IsNullOrWhiteSpace(filter.MAccountID))
			{
				BDAccountModel bDAccountModel = (from x in baseBDAccountList
				where x.MItemID == filter.MAccountID
				select x).First();
				list = BDAccountHelper.GetChildrenAccountByRecursion(bDAccountModel, baseBDAccountList, false);
				if (list == null || list.Count() == 0)
				{
					baseBDAccountList.Clear();
					baseBDAccountList.Add(bDAccountModel);
					return baseBDAccountList;
				}
				return BDAccountHelper.GetChildrenList(list);
			}
			return BDAccountHelper.GetChildrenList(baseBDAccountList);
		}

		private List<GLBalanceModel> GetBalanceList(MContext ctx, RPTAccountDemensionSummaryFilterModel filter)
		{
			List<GLBalanceModel> result = new List<GLBalanceModel>();
			if (FilterSchemeContent == null)
			{
				return result;
			}
			GLReportBaseFilterModel gLReportBaseFilterModel = new GLReportBaseFilterModel();
			gLReportBaseFilterModel.IncludeDisabledAccount = filter.IsShowDisabledAccount;
			gLReportBaseFilterModel.IncludeUnapprovedVoucher = filter.IncludeUnapprovedVoucher;
			gLReportBaseFilterModel.MDisplayNoAccurrenceAmount = filter.MDisplayNoAccurrenceAmount;
			gLReportBaseFilterModel.MDisplayZeorEndBalance = filter.MDisplayZeorEndBalance;
			gLReportBaseFilterModel.OnlyCheckType = true;
			gLReportBaseFilterModel.MCurrencyID = filter.MCurrencyID;
			gLReportBaseFilterModel.MStartPeroid = Convert.ToString(filter.MStartPeroid);
			gLReportBaseFilterModel.MEndPeroid = Convert.ToString(filter.MEndPeroid);
			if (AccountList.Count() == 0)
			{
				return result;
			}
			gLReportBaseFilterModel.MAccountID = string.Join(",", (from x in AccountList
			select x.MItemID).ToList());
			gLReportBaseFilterModel.IncludeCheckType = true;
			result = GLReportHelper.GetBalanceList(ctx, gLReportBaseFilterModel, true, true, null);
			if (result == null || result.Count() == 0)
			{
				return result;
			}
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			for (int i = 0; i < result.Count(); i++)
			{
				if (JudgeBalanceIsMatch(ctx, result[i]))
				{
					list.Add(result[i]);
				}
			}
			list = (from x in list
			orderby x.MNumber
			select x).ToList();
			list.ForEach(delegate(GLBalanceModel x)
			{
				BDAccountModel bDAccountModel = AccountList.FirstOrDefault((BDAccountModel y) => y.MItemID == x.MAccountID);
				if (bDAccountModel != null)
				{
					x.MDC = bDAccountModel.MDC;
				}
			});
			return list;
		}

		private bool JudgeBalanceIsMatch(MContext ctx, GLBalanceModel balance)
		{
			bool result = false;
			GLCheckGroupValueModel mCheckGroupValueModel = balance.MCheckGroupValueModel;
			if (mCheckGroupValueModel == null)
			{
				return result;
			}
			List<FilterSchemeCheckGroupModel> checkGroup = FilterSchemeContent.CheckGroup;
			bool flag = true;
			foreach (FilterSchemeCheckGroupModel item in checkGroup)
			{
				switch (item.FieldType)
				{
				case 0:
					flag = (flag && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID));
					break;
				case 1:
					flag = (flag && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeID));
					break;
				case 2:
					flag = (flag && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MMerItemID));
					break;
				case 3:
					flag = (flag && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MExpItemID));
					break;
				case 4:
					flag = (flag && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemID));
					break;
				case 5:
					flag = (flag && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem1));
					break;
				case 6:
					flag = (flag && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem2));
					break;
				case 7:
					flag = (flag && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem3));
					break;
				case 8:
					flag = (flag && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem4));
					break;
				case 9:
					flag = (flag && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem5));
					break;
				default:
					flag = false;
					break;
				}
				if (!flag)
				{
					return flag;
				}
			}
			return flag;
		}

		private bool GetConditionResult(FilterSchemeConditionModel condition, object value, int valueType)
		{
			bool result = true;
			if (condition.MCompare == 1)
			{
				switch (valueType)
				{
				case 1:
				{
					int num = 0;
					if (int.TryParse(condition.MCompareValue, out num))
					{
						result = (Convert.ToInt32(value) == num);
					}
					break;
				}
				case 2:
				{
					decimal d = default(decimal);
					if (decimal.TryParse(condition.MCompareValue, out d))
					{
						result = (Convert.ToDecimal(value) == d);
					}
					break;
				}
				case 3:
					result = (string.Compare(Convert.ToString(value), Convert.ToString(condition.MCompareValue)) == 0);
					break;
				}
			}
			else if (condition.MCompare == 3)
			{
				switch (valueType)
				{
				case 1:
				{
					int num2 = 0;
					if (int.TryParse(condition.MCompareValue, out num2))
					{
						result = (Convert.ToInt32(value) > num2);
					}
					break;
				}
				case 2:
				{
					decimal d2 = default(decimal);
					if (decimal.TryParse(condition.MCompareValue, out d2))
					{
						result = (Convert.ToDecimal(value) > d2);
					}
					break;
				}
				case 3:
					result = false;
					break;
				}
			}
			else if (condition.MCompare == 4)
			{
				switch (valueType)
				{
				case 1:
				{
					int num3 = 0;
					if (int.TryParse(condition.MCompareValue, out num3))
					{
						result = (Convert.ToInt32(value) >= num3);
					}
					break;
				}
				case 2:
				{
					decimal d3 = default(decimal);
					if (decimal.TryParse(condition.MCompareValue, out d3))
					{
						result = (Convert.ToDecimal(value) >= d3);
					}
					break;
				}
				case 3:
					result = false;
					break;
				}
			}
			else if (condition.MCompare == 7)
			{
				switch (valueType)
				{
				case 1:
					result = false;
					break;
				case 2:
					result = false;
					break;
				case 3:
					result = Convert.ToString(value).Contains(Convert.ToString(condition.MCompareValue));
					break;
				}
			}
			else if (condition.MCompare == 10)
			{
				switch (valueType)
				{
				case 1:
					result = false;
					break;
				case 2:
					result = false;
					break;
				case 3:
					result = !string.IsNullOrWhiteSpace(Convert.ToString(value));
					break;
				}
			}
			else if (condition.MCompare == 9)
			{
				switch (valueType)
				{
				case 1:
					result = false;
					break;
				case 2:
					result = false;
					break;
				case 3:
					result = string.IsNullOrWhiteSpace(Convert.ToString(value));
					break;
				}
			}
			else if (condition.MCompare == 5)
			{
				switch (valueType)
				{
				case 1:
				{
					int num4 = 0;
					if (int.TryParse(condition.MCompareValue, out num4))
					{
						result = (Convert.ToInt32(value) < num4);
					}
					break;
				}
				case 2:
				{
					decimal d4 = default(decimal);
					if (decimal.TryParse(condition.MCompareValue, out d4))
					{
						result = (Convert.ToDecimal(value) < d4);
					}
					break;
				}
				case 3:
					result = false;
					break;
				}
			}
			else if (condition.MCompare == 6)
			{
				switch (valueType)
				{
				case 1:
				{
					int num5 = 0;
					if (int.TryParse(condition.MCompareValue, out num5))
					{
						result = (Convert.ToInt32(value) <= num5);
					}
					break;
				}
				case 2:
				{
					decimal d5 = default(decimal);
					if (decimal.TryParse(condition.MCompareValue, out d5))
					{
						result = (Convert.ToDecimal(value) <= d5);
					}
					break;
				}
				case 3:
					result = false;
					break;
				}
			}
			else if (condition.MCompare == 2)
			{
				switch (valueType)
				{
				case 1:
				{
					int num6 = 0;
					if (int.TryParse(condition.MCompareValue, out num6))
					{
						result = (Convert.ToInt32(value) != num6);
					}
					break;
				}
				case 2:
				{
					decimal d6 = default(decimal);
					if (decimal.TryParse(condition.MCompareValue, out d6))
					{
						result = (Convert.ToDecimal(value) != d6);
					}
					break;
				}
				case 3:
					result = (Convert.ToString(value) != Convert.ToString(condition.MCompareValue));
					break;
				}
			}
			else if (condition.MCompare == 8)
			{
				switch (valueType)
				{
				case 1:
					result = false;
					break;
				case 2:
					result = false;
					break;
				case 3:
					result = !Convert.ToString(value).Contains(Convert.ToString(condition.MCompareValue));
					break;
				}
			}
			return result;
		}

		public static FilterSchemeContentModel GetFilterScheme(MContext ctx, RPTAccountDemensionSummaryFilterModel filter)
		{
			RPTFilterSchemeRepository rPTFilterSchemeRepository = new RPTFilterSchemeRepository();
			RPTFilterSchemeModel dataModel = rPTFilterSchemeRepository.GetDataModel(ctx, filter.FilterSchemeId, false);
			if (dataModel == null)
			{
				return null;
			}
			return JsonConvert.DeserializeObject<FilterSchemeContentModel>(dataModel.MContent);
		}

		private List<string> GetCheckTypeNameList(MContext ctx)
		{
			List<string> list = new List<string>();
			if (FilterSchemeContent == null)
			{
				return list;
			}
			List<FilterSchemeCheckGroupModel> checkGroup = FilterSchemeContent.CheckGroup;
			GLUtility gLUtility = new GLUtility();
			foreach (FilterSchemeCheckGroupModel item in checkGroup)
			{
				string checkTypeName = gLUtility.GetCheckTypeName(ctx, item.FieldType);
				if (!string.IsNullOrWhiteSpace(checkTypeName))
				{
					list.Add(checkTypeName);
				}
			}
			return list;
		}

		private BizReportModel GetReportModel(MContext ctx, RPTAccountDemensionSummaryFilterModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.AccountDimensionSummary);
			SetCommonData(ctx, filter);
			SetDataColumns(ctx, filter);
			SetTitle(bizReportModel, filter, ctx);
			SetRowHead(bizReportModel, filter, ctx);
			SetRowData(bizReportModel, filter, ctx);
			return bizReportModel;
		}

		private void SetCommonData(MContext ctx, RPTAccountDemensionSummaryFilterModel filter)
		{
			if (FilterSchemeContent != null)
			{
				DataList = new List<RPTAccountDimensionSummaryModel>();
				if (BalanceList != null && BalanceList.Count() != 0)
				{
					IEnumerable<IGrouping<int, GLBalanceModel>> source = from x in BalanceList
					group x by x.MYearPeriod;
					source = from x in source
					orderby x.Key
					select x;
					foreach (IGrouping<int, GLBalanceModel> item3 in source)
					{
						List<GLBalanceModel> list = item3.ToList();
						List<GLBalanceModel> list2 = new List<GLBalanceModel>();
						if (list.Count() != 0)
						{
							if (filter.IsSubtotalByAccountDemension)
							{
								Dictionary<string, List<GLBalanceModel>> balanceCheckTypeGroupBy = GetBalanceCheckTypeGroupBy(list);
								foreach (List<GLBalanceModel> value in balanceCheckTypeGroupBy.Values)
								{
									if (value != null || value.Count() != 0)
									{
										List<GLBalanceModel> list3 = new List<GLBalanceModel>();
										IEnumerable<IGrouping<string, GLBalanceModel>> enumerable = from x in value
										orderby x.MNumber
										group x by x.MAccountID;
										foreach (IGrouping<string, GLBalanceModel> item4 in enumerable)
										{
											if (item4 != null && item4.Count() != 0)
											{
												List<GLBalanceModel> list4 = new List<GLBalanceModel>();
												list4.AddRange(item4.ToList());
												BDAccountModel bDAccountModel = (from x in AccountList
												where x.MItemID == item4.Key
												select x).FirstOrDefault();
												if (bDAccountModel != null)
												{
													RPTAccountDimensionSummaryModel item = ToRptModel(list4, bDAccountModel);
													if (JudgeConditionIsMatch(list4))
													{
														list2.AddRange(list4);
														list3.AddRange(list4);
														DataList.Add(item);
													}
												}
											}
										}
										if (list3.Count() > 0)
										{
											RPTAccountDimensionSummaryModel subTotalRptModel = GetSubTotalRptModel(ctx, list3, filter, false);
											DataList.Add(subTotalRptModel);
										}
									}
								}
							}
							else
							{
								Dictionary<string, List<GLBalanceModel>> balanceCheckTypeGroupBy2 = GetBalanceCheckTypeGroupBy(list);
								Dictionary<string, List<GLBalanceModel>> dictionary = new Dictionary<string, List<GLBalanceModel>>();
								List<RPTAccountDimensionSummaryModel> list5 = new List<RPTAccountDimensionSummaryModel>();
								foreach (List<GLBalanceModel> value2 in balanceCheckTypeGroupBy2.Values)
								{
									if (value2 != null && value2.Count() != 0)
									{
										IEnumerable<IGrouping<string, GLBalanceModel>> enumerable2 = from x in value2
										group x by x.MAccountID;
										foreach (IGrouping<string, GLBalanceModel> item5 in enumerable2)
										{
											List<GLBalanceModel> list6 = item5.ToList();
											BDAccountModel bDAccountModel2 = (from x in AccountList
											where x.MItemID == item5.Key
											select x).FirstOrDefault();
											if (bDAccountModel2 != null)
											{
												RPTAccountDimensionSummaryModel item2 = ToRptModel(list6, bDAccountModel2);
												if (JudgeConditionIsMatch(list6))
												{
													list5.Add(item2);
													if (dictionary.Keys.Contains(item5.Key))
													{
														dictionary[item5.Key].AddRange(list6);
													}
													else
													{
														dictionary[item5.Key] = list6;
													}
													list2.AddRange(list6);
												}
											}
										}
									}
								}
								list5 = (from x in list5
								orderby x.AccountNumber
								select x).ToList();
								if (filter.IsSubtotalByAccount)
								{
									string text = "";
									for (int i = 0; i < list5.Count(); i++)
									{
										RPTAccountDimensionSummaryModel rPTAccountDimensionSummaryModel = list5[i];
										if (i == 0)
										{
											text = rPTAccountDimensionSummaryModel.AccountId;
										}
										if (text != rPTAccountDimensionSummaryModel.AccountId)
										{
											List<GLBalanceModel> balanceList = dictionary[text];
											RPTAccountDimensionSummaryModel subTotalRptModel2 = GetSubTotalRptModel(ctx, balanceList, filter, false);
											DataList.Add(subTotalRptModel2);
											text = rPTAccountDimensionSummaryModel.AccountId;
										}
										DataList.Add(rPTAccountDimensionSummaryModel);
										if (i == list5.Count() - 1)
										{
											List<GLBalanceModel> balanceList2 = dictionary[text];
											RPTAccountDimensionSummaryModel subTotalRptModel3 = GetSubTotalRptModel(ctx, balanceList2, filter, false);
											DataList.Add(subTotalRptModel3);
											text = rPTAccountDimensionSummaryModel.AccountId;
										}
									}
								}
								else
								{
									DataList.AddRange(list5);
								}
							}
							if (filter.IsCrossPeriodSubtotal)
							{
								RPTAccountDimensionSummaryModel subTotalRptModel4 = GetSubTotalRptModel(ctx, list2, filter, true);
								DataList.Add(subTotalRptModel4);
							}
						}
					}
				}
			}
		}

		private Dictionary<string, List<GLBalanceModel>> GetBalanceCheckTypeGroupBy(List<GLBalanceModel> balanceList)
		{
			int num = balanceList.Count();
			Dictionary<string, List<GLBalanceModel>> dictionary = new Dictionary<string, List<GLBalanceModel>>();
			List<string> list = new List<string>();
			for (int i = 0; i < num; i++)
			{
				GLBalanceModel balance = balanceList[i];
				if (!list.Contains(balance.MItemID))
				{
					GLCheckGroupValueModel checkGroup = balance.MCheckGroupValueModel;
					if (checkGroup == null)
					{
						list.Add(balance.MItemID);
					}
					else
					{
						string text = GetCheckTypeName(checkGroup);
						List<GLBalanceModel> list2 = (from y in balanceList
						where y.MItemID != balance.MItemID && (!FilterSchemeContent.CheckGroup.Exists((FilterSchemeCheckGroupModel z) => z.FieldType == 0) || y.MCheckGroupValueModel.MContactID == checkGroup.MContactID) && (!FilterSchemeContent.CheckGroup.Exists((FilterSchemeCheckGroupModel z) => z.FieldType == 1) || y.MCheckGroupValueModel.MEmployeeID == checkGroup.MEmployeeID) && (!FilterSchemeContent.CheckGroup.Exists((FilterSchemeCheckGroupModel z) => z.FieldType == 3) || y.MCheckGroupValueModel.MExpItemID == checkGroup.MExpItemID) && (!FilterSchemeContent.CheckGroup.Exists((FilterSchemeCheckGroupModel z) => z.FieldType == 2) || y.MCheckGroupValueModel.MMerItemID == checkGroup.MMerItemID) && (!FilterSchemeContent.CheckGroup.Exists((FilterSchemeCheckGroupModel z) => z.FieldType == 4) || y.MCheckGroupValueModel.MPaItemID == checkGroup.MPaItemID) && (!FilterSchemeContent.CheckGroup.Exists((FilterSchemeCheckGroupModel z) => z.FieldType == 5) || y.MCheckGroupValueModel.MTrackItem1 == checkGroup.MTrackItem1) && (!FilterSchemeContent.CheckGroup.Exists((FilterSchemeCheckGroupModel z) => z.FieldType == 6) || y.MCheckGroupValueModel.MTrackItem2 == checkGroup.MTrackItem2) && (!FilterSchemeContent.CheckGroup.Exists((FilterSchemeCheckGroupModel z) => z.FieldType == 7) || y.MCheckGroupValueModel.MTrackItem3 == checkGroup.MTrackItem3) && (!FilterSchemeContent.CheckGroup.Exists((FilterSchemeCheckGroupModel z) => z.FieldType == 8) || y.MCheckGroupValueModel.MTrackItem4 == checkGroup.MTrackItem4) && (!FilterSchemeContent.CheckGroup.Exists((FilterSchemeCheckGroupModel z) => z.FieldType == 9) || y.MCheckGroupValueModel.MTrackItem5 == checkGroup.MTrackItem5)
						select y).ToList();
						list2 = ((list2 == null) ? new List<GLBalanceModel>() : list2);
						list.AddRange((from x in list2
						select x.MItemID).ToList());
						list2.Add(balance);
						if (string.IsNullOrWhiteSpace(text))
						{
							text = i.ToString();
						}
						if (dictionary.Keys.Contains(text))
						{
							dictionary[text].AddRange(list2);
						}
						else
						{
							dictionary.Add(text, list2);
						}
					}
				}
			}
			if (dictionary.Count() > 0)
			{
				dictionary = (from p in dictionary
				orderby p.Key
				select p).ToDictionary((KeyValuePair<string, List<GLBalanceModel>> p) => p.Key, (KeyValuePair<string, List<GLBalanceModel>> o) => o.Value);
			}
			return dictionary;
		}

		private string GetCheckTypeName(GLCheckGroupValueModel checkGroupValue)
		{
			string text = "";
			if (FilterSchemeContent != null && FilterSchemeContent.CheckGroup != null && FilterSchemeContent.CheckGroup.Count() > 0)
			{
				foreach (FilterSchemeCheckGroupModel item in FilterSchemeContent.CheckGroup)
				{
					switch (item.FieldType)
					{
					case 0:
						text += checkGroupValue.MContactName;
						break;
					case 1:
						text += checkGroupValue.MEmployeeName;
						break;
					case 3:
						text += checkGroupValue.MExpItemName;
						break;
					case 2:
						text += checkGroupValue.MMerItemName;
						break;
					case 4:
						text += ((!string.IsNullOrWhiteSpace(checkGroupValue.MPaItemGroupName)) ? checkGroupValue.MPaItemGroupName : checkGroupValue.MPaItemName);
						break;
					case 5:
						text += checkGroupValue.MTrackItem1Name;
						break;
					case 6:
						text += checkGroupValue.MTrackItem2Name;
						break;
					case 7:
						text += checkGroupValue.MTrackItem3Name;
						break;
					case 8:
						text += checkGroupValue.MTrackItem4Name;
						break;
					case 9:
						text += checkGroupValue.MTrackItem5Name;
						break;
					}
				}
			}
			return text;
		}

		private RPTAccountDimensionSummaryModel GetSubTotalRptModel(MContext ctx, List<GLBalanceModel> balanceList, RPTAccountDemensionSummaryFilterModel filter, bool isCrossPriod = false)
		{
			RPTAccountDimensionSummaryModel rPTAccountDimensionSummaryModel = new RPTAccountDimensionSummaryModel();
			string text = isCrossPriod ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ThisPeriodSubTotal", "本期小计") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "SubTotal", "小计");
			int num = FilterSchemeContent.CheckGroup.Count();
			if (filter.IsSubtotalByAccount)
			{
				rPTAccountDimensionSummaryModel.AccountNumber = text;
				for (int i = 0; i < num; i++)
				{
					rPTAccountDimensionSummaryModel.CheckTypeValueList.Add("");
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					string item = (j == 0) ? text : "";
					rPTAccountDimensionSummaryModel.CheckTypeValueList.Add(item);
				}
			}
			foreach (GLBalanceModel balance in balanceList)
			{
				if (balance.MDC == 1)
				{
					RPTAccountDimensionSummaryModel rPTAccountDimensionSummaryModel2 = rPTAccountDimensionSummaryModel;
					rPTAccountDimensionSummaryModel2.MBeginDebitBalance += balance.MBeginBalance;
					RPTAccountDimensionSummaryModel rPTAccountDimensionSummaryModel3 = rPTAccountDimensionSummaryModel;
					rPTAccountDimensionSummaryModel3.MBeginDebitBalanceFor += balance.MBeginBalanceFor;
					RPTAccountDimensionSummaryModel rPTAccountDimensionSummaryModel4 = rPTAccountDimensionSummaryModel;
					rPTAccountDimensionSummaryModel4.MEndDebitBalance += balance.MBeginBalance + balance.MDebit - balance.MCredit;
					RPTAccountDimensionSummaryModel rPTAccountDimensionSummaryModel5 = rPTAccountDimensionSummaryModel;
					rPTAccountDimensionSummaryModel5.MEndDebitBalanceFor += balance.MBeginBalanceFor + balance.MDebitFor - balance.MCreditFor;
				}
				else
				{
					RPTAccountDimensionSummaryModel rPTAccountDimensionSummaryModel6 = rPTAccountDimensionSummaryModel;
					rPTAccountDimensionSummaryModel6.MBeginCreditBalance += balance.MBeginBalance;
					RPTAccountDimensionSummaryModel rPTAccountDimensionSummaryModel7 = rPTAccountDimensionSummaryModel;
					rPTAccountDimensionSummaryModel7.MBeginCreditBalanceFor += balance.MBeginBalanceFor;
					RPTAccountDimensionSummaryModel rPTAccountDimensionSummaryModel8 = rPTAccountDimensionSummaryModel;
					rPTAccountDimensionSummaryModel8.MEndCreditBalance += balance.MBeginBalance + balance.MCredit - balance.MDebit;
					RPTAccountDimensionSummaryModel rPTAccountDimensionSummaryModel9 = rPTAccountDimensionSummaryModel;
					rPTAccountDimensionSummaryModel9.MEndCreditBalanceFor += balance.MBeginBalanceFor + balance.MCreditFor - balance.MDebitFor;
				}
			}
			rPTAccountDimensionSummaryModel.MCredit = balanceList.Sum((GLBalanceModel x) => x.MCredit);
			rPTAccountDimensionSummaryModel.MCreditFor = balanceList.Sum((GLBalanceModel x) => x.MCreditFor);
			rPTAccountDimensionSummaryModel.MDebit = balanceList.Sum((GLBalanceModel x) => x.MDebit);
			rPTAccountDimensionSummaryModel.MDebitFor = balanceList.Sum((GLBalanceModel x) => x.MDebitFor);
			rPTAccountDimensionSummaryModel.MYtdCredit = balanceList.Sum((GLBalanceModel x) => x.MYtdCredit);
			rPTAccountDimensionSummaryModel.MYtdCreditFor = balanceList.Sum((GLBalanceModel x) => x.MYtdCreditFor);
			rPTAccountDimensionSummaryModel.MYtdDebit = balanceList.Sum((GLBalanceModel x) => x.MYtdDebit);
			rPTAccountDimensionSummaryModel.MYtdDebitFor = balanceList.Sum((GLBalanceModel x) => x.MYtdDebitFor);
			return rPTAccountDimensionSummaryModel;
		}

		private RPTAccountDimensionSummaryModel ToRptModel(List<GLBalanceModel> balanceList, BDAccountModel account)
		{
			RPTAccountDimensionSummaryModel rPTAccountDimensionSummaryModel = new RPTAccountDimensionSummaryModel();
			rPTAccountDimensionSummaryModel.BalanceList = balanceList;
			GLBalanceModel gLBalanceModel = balanceList.First();
			rPTAccountDimensionSummaryModel.MYearPeriod = gLBalanceModel.MYearPeriod;
			rPTAccountDimensionSummaryModel.AccountId = account.MItemID;
			rPTAccountDimensionSummaryModel.AccountName = account.MName;
			rPTAccountDimensionSummaryModel.AccountNumber = account.MNumber;
			rPTAccountDimensionSummaryModel.CheckTypeValueList = GetBalanceCheckValueList(gLBalanceModel);
			if (account.MDC == 1)
			{
				rPTAccountDimensionSummaryModel.MBeginDebitBalance = balanceList.Sum((GLBalanceModel x) => x.MBeginBalance);
				rPTAccountDimensionSummaryModel.MBeginDebitBalanceFor = balanceList.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
				rPTAccountDimensionSummaryModel.MEndDebitBalance = balanceList.Sum((GLBalanceModel x) => x.MBeginBalance) + balanceList.Sum((GLBalanceModel x) => x.MDebit) - balanceList.Sum((GLBalanceModel x) => x.MCredit);
				rPTAccountDimensionSummaryModel.MEndDebitBalanceFor = balanceList.Sum((GLBalanceModel x) => x.MBeginBalanceFor) + balanceList.Sum((GLBalanceModel x) => x.MDebitFor) - balanceList.Sum((GLBalanceModel x) => x.MCreditFor);
			}
			else
			{
				rPTAccountDimensionSummaryModel.MBeginCreditBalance = balanceList.Sum((GLBalanceModel x) => x.MBeginBalance);
				rPTAccountDimensionSummaryModel.MBeginCreditBalanceFor = balanceList.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
				rPTAccountDimensionSummaryModel.MEndCreditBalance = balanceList.Sum((GLBalanceModel x) => x.MBeginBalance) + balanceList.Sum((GLBalanceModel x) => x.MCredit) - balanceList.Sum((GLBalanceModel x) => x.MDebit);
				rPTAccountDimensionSummaryModel.MEndCreditBalanceFor = balanceList.Sum((GLBalanceModel x) => x.MBeginBalanceFor) + balanceList.Sum((GLBalanceModel x) => x.MCreditFor) - balanceList.Sum((GLBalanceModel x) => x.MDebitFor);
			}
			rPTAccountDimensionSummaryModel.MCredit = balanceList.Sum((GLBalanceModel x) => x.MCredit);
			rPTAccountDimensionSummaryModel.MCreditFor = balanceList.Sum((GLBalanceModel x) => x.MCreditFor);
			rPTAccountDimensionSummaryModel.MDebit = balanceList.Sum((GLBalanceModel x) => x.MDebit);
			rPTAccountDimensionSummaryModel.MDebitFor = balanceList.Sum((GLBalanceModel x) => x.MDebitFor);
			rPTAccountDimensionSummaryModel.MYtdCredit = balanceList.Sum((GLBalanceModel x) => x.MYtdCredit);
			rPTAccountDimensionSummaryModel.MYtdCreditFor = balanceList.Sum((GLBalanceModel x) => x.MYtdCreditFor);
			rPTAccountDimensionSummaryModel.MYtdDebit = balanceList.Sum((GLBalanceModel x) => x.MYtdDebit);
			rPTAccountDimensionSummaryModel.MYtdDebitFor = balanceList.Sum((GLBalanceModel x) => x.MYtdDebitFor);
			return rPTAccountDimensionSummaryModel;
		}

		private List<string> GetBalanceCheckValueList(GLBalanceModel balance)
		{
			List<string> list = new List<string>();
			List<FilterSchemeCheckGroupModel> checkGroup = FilterSchemeContent.CheckGroup;
			if (checkGroup == null || checkGroup.Count() == 0)
			{
				return list;
			}
			GLCheckGroupValueModel mCheckGroupValueModel = balance.MCheckGroupValueModel;
			foreach (FilterSchemeCheckGroupModel item in checkGroup)
			{
				if (item.FieldType == 0)
				{
					string mContactName = mCheckGroupValueModel.MContactName;
					if (!string.IsNullOrWhiteSpace(mContactName))
					{
						list.Add(mContactName);
					}
				}
				else if (item.FieldType == 1)
				{
					string mEmployeeName = mCheckGroupValueModel.MEmployeeName;
					if (!string.IsNullOrWhiteSpace(mEmployeeName))
					{
						list.Add(mEmployeeName);
					}
				}
				else if (item.FieldType == 2)
				{
					string mMerItemName = mCheckGroupValueModel.MMerItemName;
					if (!string.IsNullOrWhiteSpace(mMerItemName))
					{
						list.Add(mMerItemName);
					}
				}
				else if (item.FieldType == 3)
				{
					string mExpItemName = mCheckGroupValueModel.MExpItemName;
					if (!string.IsNullOrWhiteSpace(mExpItemName))
					{
						list.Add(mExpItemName);
					}
				}
				else if (item.FieldType == 4)
				{
					string text = string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemName) ? mCheckGroupValueModel.MPaItemGroupName : mCheckGroupValueModel.MPaItemName;
					if (!string.IsNullOrWhiteSpace(text))
					{
						list.Add(text);
					}
				}
				else if (item.FieldType == 5)
				{
					string mTrackItem1Name = mCheckGroupValueModel.MTrackItem1Name;
					if (!string.IsNullOrWhiteSpace(mTrackItem1Name))
					{
						list.Add(mTrackItem1Name);
					}
				}
				else if (item.FieldType == 6)
				{
					string mTrackItem2Name = mCheckGroupValueModel.MTrackItem2Name;
					if (!string.IsNullOrWhiteSpace(mTrackItem2Name))
					{
						list.Add(mTrackItem2Name);
					}
				}
				else if (item.FieldType == 7)
				{
					string mTrackItem3Name = mCheckGroupValueModel.MTrackItem3Name;
					if (!string.IsNullOrWhiteSpace(mTrackItem3Name))
					{
						list.Add(mTrackItem3Name);
					}
				}
				else if (item.FieldType == 8)
				{
					string mTrackItem4Name = mCheckGroupValueModel.MTrackItem4Name;
					if (!string.IsNullOrWhiteSpace(mTrackItem4Name))
					{
						list.Add(mTrackItem4Name);
					}
				}
				else if (item.FieldType == 9)
				{
					string mTrackItem5Name = mCheckGroupValueModel.MTrackItem5Name;
					if (!string.IsNullOrWhiteSpace(mTrackItem5Name))
					{
						list.Add(mTrackItem5Name);
					}
				}
			}
			return list;
		}

		private void SetDataSummary(BizReportModel reportModel, RPTAccountDemensionSummaryFilterModel filter, MContext ctx)
		{
			if (FilterSchemeContent != null)
			{
				BizReportRowModel bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.RowType = BizReportRowType.Total;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = "",
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Total", "Total"),
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = "",
					CellType = BizReportCellType.Text
				});
				int num = FilterSchemeContent.CheckGroup.Count();
				for (int i = 0; i < num; i++)
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = "",
						CellType = BizReportCellType.Text
					});
				}
				List<RPTAccountDimensionSummaryModel> source = (from x in DataList
				where !string.IsNullOrWhiteSpace(x.AccountId)
				select x).ToList();
				string defaultValue = "";
				if (IncludeForeginCurrency)
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MBeginDebitBalanceFor), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MBeginDebitBalance), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MBeginCreditBalanceFor), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MBeginCreditBalance), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MDebitFor), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MDebit), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MCreditFor), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MCredit), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MYtdDebitFor), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MYtdDebit), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MYtdCreditFor), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MYtdCredit), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MEndDebitBalanceFor), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MEndDebitBalance), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MEndCreditBalanceFor), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MEndCreditBalance), defaultValue),
						CellType = BizReportCellType.Money
					});
				}
				else
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MBeginDebitBalance), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MBeginCreditBalance), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MDebit), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MCredit), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MYtdDebit), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MYtdCredit), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MEndDebitBalance), defaultValue),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = AccountHelper.MoneyToString(source.Sum((RPTAccountDimensionSummaryModel x) => x.MEndCreditBalance), defaultValue),
						CellType = BizReportCellType.Money
					});
				}
				reportModel.AddRow(bizReportRowModel);
			}
		}

		private void SetRowData(BizReportModel reportModel, RPTAccountDemensionSummaryFilterModel filter, MContext ctx)
		{
			List<BizReportRowModel> list = new List<BizReportRowModel>();
			if (DataList != null && DataList.Count() > 0)
			{
				List<RPTAccountDimensionSummaryModel> list2 = new List<RPTAccountDimensionSummaryModel>();
				foreach (RPTAccountDimensionSummaryModel data in DataList)
				{
					BizReportRowModel bizReportRowModel = ToBizReportRowModel(ctx, data, filter);
					if (bizReportRowModel != null)
					{
						list2.Add(data);
						list.Add(bizReportRowModel);
					}
				}
				DataList = list2;
			}
			reportModel.Rows.AddRange(list);
		}

		private bool CheckIsZoreData(MContext ctx, BalanceBizReportRowModel report, int filterType)
		{
			return true;
		}

		private bool JudgeConditionIsMatch(List<GLBalanceModel> balanceList)
		{
			bool flag = false;
			List<FilterSchemeConditionModel> condition = FilterSchemeContent.Condition;
			if (condition == null || condition.Count() == 0)
			{
				return true;
			}
			Dictionary<int, CompareResultModel> dictionary = new Dictionary<int, CompareResultModel>();
			for (int i = 0; i < condition.Count(); i++)
			{
				FilterSchemeConditionModel filterSchemeConditionModel = condition[i];
				CompareResultModel compareResultModel = new CompareResultModel();
				int nextResultCompareLogic = (filterSchemeConditionModel.MLogic == 0) ? 1 : filterSchemeConditionModel.MLogic;
				switch (filterSchemeConditionModel.MField)
				{
				case 110:
				{
					bool conditionResult7 = GetConditionResult(filterSchemeConditionModel, balanceList.Sum((GLBalanceModel x) => x.MBeginBalance), 2);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult7,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 130:
				{
					bool conditionResult15 = GetConditionResult(filterSchemeConditionModel, balanceList.Sum((GLBalanceModel x) => x.MCredit), 2);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult15,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 120:
				{
					bool conditionResult8 = GetConditionResult(filterSchemeConditionModel, balanceList.Sum((GLBalanceModel x) => x.MDebit), 2);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult8,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 150:
				{
					bool conditionResult16 = GetConditionResult(filterSchemeConditionModel, balanceList.Sum((GLBalanceModel x) => x.MYtdCredit), 2);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult16,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 140:
				{
					bool conditionResult9 = GetConditionResult(filterSchemeConditionModel, balanceList.Sum((GLBalanceModel x) => x.MYtdDebit), 2);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult9,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 160:
				{
					bool conditionResult14 = GetConditionResult(filterSchemeConditionModel, balanceList.Sum((GLBalanceModel x) => x.MEndBalance), 2);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult14,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 0:
				{
					bool conditionResult13 = GetConditionResult(filterSchemeConditionModel, balanceList.First().MCheckGroupValueModel.MContactName, 3);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult13,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 1:
				{
					bool conditionResult12 = GetConditionResult(filterSchemeConditionModel, balanceList.First().MCheckGroupValueModel.MEmployeeName, 3);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult12,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 2:
				{
					bool conditionResult11 = GetConditionResult(filterSchemeConditionModel, balanceList.First().MCheckGroupValueModel.MMerItemName, 3);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult11,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 3:
				{
					bool conditionResult10 = GetConditionResult(filterSchemeConditionModel, balanceList.First().MCheckGroupValueModel.MExpItemName, 3);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult10,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 4:
				{
					string value = string.IsNullOrWhiteSpace(balanceList.First().MCheckGroupValueModel.MPaItemName) ? balanceList.First().MCheckGroupValueModel.MPaItemName : balanceList.First().MCheckGroupValueModel.MPaItemGroupName;
					bool conditionResult6 = GetConditionResult(filterSchemeConditionModel, value, 3);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult6,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 5:
				{
					bool conditionResult5 = GetConditionResult(filterSchemeConditionModel, balanceList.First().MCheckGroupValueModel.MTrackItem1Name, 3);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult5,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 6:
				{
					bool conditionResult4 = GetConditionResult(filterSchemeConditionModel, balanceList.First().MCheckGroupValueModel.MTrackItem2Name, 3);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult4,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 7:
				{
					bool conditionResult3 = GetConditionResult(filterSchemeConditionModel, balanceList.First().MCheckGroupValueModel.MTrackItem3Name, 3);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult3,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 8:
				{
					bool conditionResult2 = GetConditionResult(filterSchemeConditionModel, balanceList.First().MCheckGroupValueModel.MTrackItem4Name, 3);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult2,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				case 9:
				{
					bool conditionResult = GetConditionResult(filterSchemeConditionModel, balanceList.First().MCheckGroupValueModel.MTrackItem5Name, 3);
					compareResultModel = new CompareResultModel
					{
						CompareResult = conditionResult,
						NextResultCompareLogic = nextResultCompareLogic
					};
					dictionary.Add(i, compareResultModel);
					break;
				}
				default:
					compareResultModel = new CompareResultModel();
					dictionary.Add(i, compareResultModel);
					break;
				}
				if (dictionary.Count > 1)
				{
					CompareResultModel compareResultModel2 = dictionary[i - 1];
					if ((compareResultModel2.NextResultCompareLogic == 0 || compareResultModel2.NextResultCompareLogic == 1) && (!compareResultModel2.CompareResult || !compareResultModel.CompareResult))
					{
						return false;
					}
					flag = (flag || compareResultModel.CompareResult);
				}
				else
				{
					flag = compareResultModel.CompareResult;
				}
			}
			return flag;
		}

		private bool GetLogicOperation(int logic, bool one, bool two, int index)
		{
			//bool flag = false;
			if (index == 0)
			{
				return one;
			}
			return (logic != 1) ? (one | two) : (one & two);
		}

		private BalanceBizReportRowModel ToBizReportRowModel(MContext ctx, RPTAccountDimensionSummaryModel item, RPTAccountDemensionSummaryFilterModel filter)
		{
			BalanceBizReportRowModel balanceBizReportRowModel = new BalanceBizReportRowModel();
			if (item.MYearPeriod == 0)
			{
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = "",
					CellType = BizReportCellType.Text
				});
			}
			else
			{
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.MYearPeriod.ToString(),
					CellType = BizReportCellType.Text
				});
			}
			if (filter.IsSubtotalByAccount)
			{
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.AccountNumber,
					CellType = BizReportCellType.Text
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.AccountName,
					CellType = BizReportCellType.Text
				});
				if (item.CheckTypeValueList != null && item.CheckTypeValueList.Count() > 0)
				{
					foreach (string checkTypeValue in item.CheckTypeValueList)
					{
						balanceBizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = checkTypeValue,
							CellType = BizReportCellType.Text
						});
					}
				}
			}
			else if (filter.IsSubtotalByAccountDemension)
			{
				if (item.CheckTypeValueList != null && item.CheckTypeValueList.Count() > 0)
				{
					foreach (string checkTypeValue2 in item.CheckTypeValueList)
					{
						balanceBizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = checkTypeValue2,
							CellType = BizReportCellType.Text
						});
					}
				}
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.AccountNumber,
					CellType = BizReportCellType.Text
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.AccountName,
					CellType = BizReportCellType.Text
				});
			}
			else
			{
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.AccountNumber,
					CellType = BizReportCellType.Text
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.AccountName,
					CellType = BizReportCellType.Text
				});
				if (item.CheckTypeValueList != null && item.CheckTypeValueList.Count() > 0)
				{
					foreach (string checkTypeValue3 in item.CheckTypeValueList)
					{
						balanceBizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = checkTypeValue3,
							CellType = BizReportCellType.Text
						});
					}
				}
			}
			string defaultValue = "";
			decimal d = default(decimal);
			if (IncludeForeginCurrency)
			{
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MBeginDebitBalanceFor, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MBeginDebitBalance, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MBeginCreditBalanceFor, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MBeginCreditBalance, defaultValue),
					CellType = BizReportCellType.Money
				});
				d += item.MBeginDebitBalanceFor;
				d += item.MBeginDebitBalance;
				d += item.MBeginCreditBalanceFor;
				d += item.MBeginCreditBalance;
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MDebitFor, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MDebit, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MCreditFor, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MCredit, defaultValue),
					CellType = BizReportCellType.Money
				});
				d += item.MDebitFor;
				d += item.MDebit;
				d += item.MCreditFor;
				d += item.MCredit;
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MYtdDebitFor, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MYtdDebit, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MYtdCreditFor, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MYtdCredit, defaultValue),
					CellType = BizReportCellType.Money
				});
				d += item.MYtdDebitFor;
				d += item.MYtdDebit;
				d += item.MYtdCreditFor;
				d += item.MYtdCredit;
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MEndDebitBalanceFor, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MEndDebitBalance, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MEndCreditBalanceFor, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MEndCreditBalance, defaultValue),
					CellType = BizReportCellType.Money
				});
				d += item.MEndDebitBalanceFor;
				d += item.MEndDebitBalance;
				d += item.MEndCreditBalanceFor;
				d += item.MEndCreditBalance;
			}
			else
			{
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MBeginDebitBalance, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MBeginCreditBalance, defaultValue),
					CellType = BizReportCellType.Money
				});
				d += Math.Abs(item.MBeginDebitBalance);
				d += Math.Abs(item.MBeginCreditBalance);
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MDebit, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MCredit, defaultValue),
					CellType = BizReportCellType.Money
				});
				d += Math.Abs(item.MDebit);
				d += Math.Abs(item.MCredit);
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MYtdDebit, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MYtdCredit, defaultValue),
					CellType = BizReportCellType.Money
				});
				d += Math.Abs(item.MYtdDebit);
				d += Math.Abs(item.MYtdCredit);
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MEndDebitBalance, defaultValue),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MEndCreditBalance, defaultValue),
					CellType = BizReportCellType.Money
				});
				d += Math.Abs(item.MEndDebitBalance);
				d += Math.Abs(item.MEndCreditBalance);
			}
			balanceBizReportRowModel.TotalValue = Math.Abs(item.MEndDebitBalance) + Math.Abs(item.MEndCreditBalance);
			balanceBizReportRowModel.CurrentTotalAmount = Math.Abs(item.MDebit) + Math.Abs(item.MCredit);
			if (!filter.MDisplayZeorEndBalance && balanceBizReportRowModel.TotalValue == decimal.Zero)
			{
				return null;
			}
			if (!filter.MDisplayNoAccurrenceAmount && balanceBizReportRowModel.CurrentTotalAmount == decimal.Zero && !JudgeHasCurrenceAmount(item))
			{
				return null;
			}
			return balanceBizReportRowModel;
		}

		private bool JudgeHasCurrenceAmount(RPTAccountDimensionSummaryModel item)
		{
			if (item.BalanceList == null || item.BalanceList.Count <= 0)
			{
				return false;
			}
			List<FilterSchemeCheckGroupModel> checkGroup = FilterSchemeContent.CheckGroup;
			foreach (GLBalanceModel balance in item.BalanceList)
			{
				if (VoucherList.Any((GLVoucherModel t) => t.MYear == balance.MYear && t.MPeriod == balance.MPeriod && t.MVoucherEntrys.Any((GLVoucherEntryModel y) => y.MAccountID == balance.MAccountID && y.MCheckGroupValueID == balance.MCheckGroupValueID)))
				{
					return true;
				}
			}
			return false;
		}

		private void SetRowHead(BizReportModel reportModel, RPTAccountDemensionSummaryFilterModel filter, MContext ctx)
		{
			for (int i = 0; i < 2; i++)
			{
				if (i == 0)
				{
					BizReportRowModel bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.Header;
					foreach (KeyValuePair<string, List<string>> dataColumn in DataColumns)
					{
						bool flag = dataColumn.Value != null && dataColumn.Value.Count() > 0;
						BizReportCellModel bizReportCellModel = new BizReportCellModel
						{
							Value = dataColumn.Key,
							CellType = BizReportCellType.Text
						};
						bizReportCellModel.ColumnSpan = ((!flag) ? 1 : dataColumn.Value.Count());
						bizReportCellModel.RowSpan = (flag ? 1 : 2);
						bizReportRowModel.AddCell(bizReportCellModel);
					}
					reportModel.AddRow(bizReportRowModel);
				}
				else
				{
					BizReportRowModel bizReportRowModel2 = new BizReportRowModel();
					bizReportRowModel2.RowType = BizReportRowType.Header;
					foreach (KeyValuePair<string, List<string>> dataColumn2 in DataColumns)
					{
						if (dataColumn2.Value != null && dataColumn2.Value.Count() > 0)
						{
							foreach (string item in dataColumn2.Value)
							{
								BizReportCellModel model = new BizReportCellModel
								{
									Value = item,
									CellType = BizReportCellType.Text
								};
								bizReportRowModel2.AddCell(model);
							}
						}
					}
					reportModel.AddRow(bizReportRowModel2);
				}
			}
		}

		private void SetTitle(BizReportModel reportModel, RPTAccountDemensionSummaryFilterModel filter, MContext ctx)
		{
			reportModel.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "AccountDimensionSummaryReport", "核算维度汇总表");
			reportModel.Title2 = ctx.MOrgName;
			string text = Convert.ToString(filter.MStartPeroid);
			string text2 = Convert.ToString(filter.MEndPeroid);
			reportModel.Title3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ForThePeriod", "For the period ") + " " + text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "To", "to") + " " + text2;
		}

		private void SetDataColumns(MContext ctx, RPTAccountDemensionSummaryFilterModel filter)
		{
			DataColumns = new Dictionary<string, List<string>>();
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "YearPeriod", "年度期间");
			DataColumns.Add(text, null);
			if (filter.IsSubtotalByAccount || (!filter.IsSubtotalByAccountDemension && !filter.IsSubtotalByAccount))
			{
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountingCode", "科目代码");
				DataColumns.Add(text2, null);
				string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountName", "Name");
				DataColumns.Add(text3, null);
				List<string> checkTypeNameList = GetCheckTypeNameList(ctx);
				if (checkTypeNameList.Count() > 0)
				{
					foreach (string item in checkTypeNameList)
					{
						DataColumns.Add(item, null);
					}
				}
			}
			else if (filter.IsSubtotalByAccountDemension)
			{
				List<string> checkTypeNameList2 = GetCheckTypeNameList(ctx);
				if (checkTypeNameList2.Count() > 0)
				{
					foreach (string item2 in checkTypeNameList2)
					{
						DataColumns.Add(item2, null);
					}
				}
				string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountingCode", "科目代码");
				DataColumns.Add(text4, null);
				string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountName", "Name");
				DataColumns.Add(text5, null);
			}
			string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "InitBalance", "期初余额");
			List<string> childrenColumnList = GetChildrenColumnList(ctx, text6, IncludeForeginCurrency);
			DataColumns.Add(text6, childrenColumnList);
			string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CurrentPeriodBalance", "本期发生额");
			DataColumns.Add(text7, GetChildrenColumnList(ctx, text7, IncludeForeginCurrency));
			string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CumulativeBalance", "本年累计发生额");
			DataColumns.Add(text8, GetChildrenColumnList(ctx, text8, IncludeForeginCurrency));
			string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "EndBalance", "期末余额");
			DataColumns.Add(text9, GetChildrenColumnList(ctx, text9, IncludeForeginCurrency));
		}

		private List<string> GetChildrenColumnList(MContext ctx, string baseColumnName, bool isForegin)
		{
			List<string> list = new List<string>();
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "Debit", "Debit");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "Credit", "Credit");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "StandardCurrency", "Standard currency");
			string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "OriginalCurrency", "Original currency");
			if (isForegin)
			{
				list.Add(text + "(" + text4 + ")");
				list.Add(text + "(" + text3 + ")");
				list.Add(text2 + "(" + text4 + ")");
				list.Add(text2 + "(" + text3 + ")");
			}
			else
			{
				list.Add(text);
				list.Add(text2);
			}
			return list;
		}
	}
}
