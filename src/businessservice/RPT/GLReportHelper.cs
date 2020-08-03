using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.BD.AccountItem;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JieNor.Megi.BusinessService.RPT
{
	public class GLReportHelper
	{
		public static string GetBalanceDerectionName(int value, string langId)
		{
			string result = "";
			switch (value)
			{
			case 0:
				result = COMMultiLangRepository.GetText(langId, LangModule.Report, "Balance", "平");
				break;
			case 1:
				result = COMMultiLangRepository.GetText(langId, LangModule.Report, "Borrow", "借");
				break;
			case -1:
				result = COMMultiLangRepository.GetText(langId, LangModule.Report, "Loan", "贷");
				break;
			}
			return result;
		}

		public static List<GLBalanceModel> GetBalanceList(MContext ctx, GLReportBaseFilterModel filter, bool isAccountBalance = true, bool filterByCheckType = true, List<BDAccountModel> accountList = null)
		{
			GLBalanceRepository gLBalanceRepository = new GLBalanceRepository();
			if (string.IsNullOrEmpty(filter.MAccountID))
			{
				if (filter.AccountIdList == null || filter.AccountIdList.Count <= 0)
				{
					filter.AccountIdList = GetAccountLevels(ctx, filter, accountList).Keys.ToList();
				}
			}
			else
			{
				filter.AccountIdList = new List<string>();
				if (filter.MAccountID.IndexOf(',') >= 0)
				{
					List<string> collection = filter.MAccountID.Split(',').ToList();
					filter.AccountIdList.AddRange(collection);
				}
				else
				{
					filter.AccountIdList.Add(filter.MAccountID);
				}
			}
			List<GLBalanceModel> list = gLBalanceRepository.GetBalanceListToGLReport(ctx, filter);
			if (filter.IncludeUnapprovedVoucher)
			{
				list = MergeBalanceAndVoucher(ctx, list, filter);
			}
			if (filterByCheckType)
			{
				list = FilterBalanceListByCheckTypeValue(list, filter, isAccountBalance);
			}
			return list;
		}

		public static Dictionary<string, int> GetAccountLevels(MContext ctx, GLReportBaseFilterModel filter, List<BDAccountModel> accountList = null)
		{
			List<AccountItemTreeModel> accountIdsByFilter = GetAccountIdsByFilter(ctx, filter, null, accountList);
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (AccountItemTreeModel item in accountIdsByFilter)
			{
				if (!dictionary.ContainsKey(item.id))
				{
					dictionary.Add(item.id, item.AccountLevel);
					GetAllChildId(item.children, ref dictionary);
				}
			}
			return dictionary;
		}

		private static void GetAllChildId(List<AccountItemTreeModel> list, ref Dictionary<string, int> accountLevels)
		{
			if (list != null && list.Count > 0)
			{
				foreach (AccountItemTreeModel item in list)
				{
					if (!accountLevels.ContainsKey(item.id))
					{
						accountLevels.Add(item.id, item.AccountLevel);
						GetAllChildId(item.children, ref accountLevels);
					}
				}
			}
		}

		public static List<GLBalanceModel> FilterBalanceListByCheckTypeValue(List<GLBalanceModel> balanceList, GLReportBaseFilterModel filter, bool isAccountBalance)
		{
			bool flag = filter.IncludeCheckType && filter.CheckTypeValueList != null && filter.CheckTypeValueList.Count() > 0;
			if (!flag)
			{
				return balanceList;
			}
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			List<GLBalanceModel> list2 = (from x in balanceList
			where x.MCheckGroupValueID == "0"
			select x).ToList();
			if ((isAccountBalance || !flag) && list2 != null)
			{
				list.AddRange(list2);
			}
			List<GLBalanceModel> list3 = (from x in balanceList
			where x.MCheckGroupValueID != "0"
			select x).ToList();
			if (list3 == null || list3.Count() == 0)
			{
				return list;
			}
			if (filter.CheckTypeValueList == null || filter.CheckTypeValueList.Count() == 0)
			{
				list.AddRange(list3);
				return list;
			}
			IEnumerable<IGrouping<string, string>> enumerable = from x in filter.CheckTypeValueList
			group x.MValue by x.MName;
			List<GLBalanceModel> list4 = list3;
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
						list4 = (from x in list4
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MContactID)
						select x).ToList();
						break;
					case 1:
						list4 = (from x in list4
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MEmployeeID)
						select x).ToList();
						break;
					case 3:
						list4 = (from x in list4
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MExpItemID)
						select x).ToList();
						break;
					case 2:
						list4 = (from x in list4
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MMerItemID)
						select x).ToList();
						break;
					case 4:
						list4 = (from x in list4
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MPaItemID) || checkTypeValue.Contains(x.MCheckGroupValueModel.MPaItemGroupID)
						select x).ToList();
						break;
					case 5:
						list4 = (from x in list4
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem1)
						select x).ToList();
						break;
					case 6:
						list4 = (from x in list4
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem2)
						select x).ToList();
						break;
					case 7:
						list4 = (from x in list4
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem3)
						select x).ToList();
						break;
					case 8:
						list4 = (from x in list4
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem4)
						select x).ToList();
						break;
					case 9:
						list4 = (from x in list4
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem5)
						select x).ToList();
						break;
					}
				}
			}
			if (list4 != null && list4.Count() > 0)
			{
				if (!isAccountBalance)
				{
					List<string> accountIdList = (from x in list4
					select x.MAccountID).ToList();
					List<GLBalanceModel> list5 = (from x in list2
					where accountIdList.Contains(x.MAccountID)
					select x).ToList();
					if (list5 != null)
					{
						list.AddRange(list5);
					}
				}
				list.AddRange(list4);
			}
			return list;
		}

		private static List<GLBalanceModel> MergeBalanceAndVoucher(MContext ctx, List<GLBalanceModel> balanceList, GLReportBaseFilterModel filter)
		{
			GLSettlementRepository gLSettlementRepository = new GLSettlementRepository();
			int lastSettementPeriod = gLSettlementRepository.GetLastSettementPeriod(ctx);
			DateTime dateTime;
			int firstUnSettlementPeriod;
			if (lastSettementPeriod != 0)
			{
				dateTime = DateTime.ParseExact(lastSettementPeriod + "01", "yyyyMMdd", CultureInfo.CurrentCulture).AddMonths(1);
				firstUnSettlementPeriod = int.Parse(dateTime.ToString("yyyyMM"));
			}
			else
			{
				dateTime = ctx.MGLBeginDate;
				firstUnSettlementPeriod = int.Parse(dateTime.ToString("yyyyMM"));
			}
			List<int> virtualSettlementPeriodList = GetVirtualSettlementPeriod(ctx, filter, firstUnSettlementPeriod);
			if (virtualSettlementPeriodList == null || virtualSettlementPeriodList.Count() == 0)
			{
				return balanceList;
			}
			List<GLBalanceModel> firstUnSettlementBalanceList = GetFirstUnSettlementBalanceList(ctx, virtualSettlementPeriodList, balanceList, filter);
			GLBalanceRepository gLBalanceRepository = new GLBalanceRepository();
			List<GLBalanceModel> source = (!filter.IncludeUnapprovedVoucher) ? new List<GLBalanceModel>() : GetBalanceListFromUnApproveVoucher(ctx, virtualSettlementPeriodList, filter);
			List<GLBalanceModel> updateBalanceList = new List<GLBalanceModel>();
			bool isProcessParent = !filter.OnlyCheckType;
			int i;
			for (i = 0; i < virtualSettlementPeriodList.Count(); i++)
			{
				List<GLBalanceModel> list;
				if (i == 0)
				{
					list = firstUnSettlementBalanceList;
				}
				else
				{
					int preUnSettlementPeriod = virtualSettlementPeriodList[i - 1];
					List<GLBalanceModel> prePeriodBalanceList = (from x in updateBalanceList
					where x.MYearPeriod == preUnSettlementPeriod
					select x).ToList();
					list = GetBalanceListFromPrePeriod(ctx, prePeriodBalanceList, virtualSettlementPeriodList[i]);
				}
				List<GLBalanceModel> list2 = (from x in source
				where x.MYearPeriod == virtualSettlementPeriodList[i]
				select x).ToList();
				if (list2 == null || list2.Count() == 0)
				{
					updateBalanceList.AddRange(list);
				}
				else
				{
					List<GLBalanceModel> updateBalanceList2 = gLBalanceRepository.GetUpdateBalanceList(ctx, list2, list, isProcessParent);
					if (updateBalanceList2 != null)
					{
						updateBalanceList.AddRange(updateBalanceList2);
					}
					foreach (GLBalanceModel item in list)
					{
						if (!updateBalanceList.Exists((GLBalanceModel x) => x.MAccountID == item.MAccountID && x.MCurrencyID == item.MCurrencyID && x.MCheckGroupValueID == item.MCheckGroupValueID && x.MYearPeriod == item.MYearPeriod))
						{
							updateBalanceList.Add(item);
						}
					}
				}
			}
			updateBalanceList = (from x in updateBalanceList
			where x.MYearPeriod >= Convert.ToInt32(filter.MStartPeroid) && x.MYearPeriod <= Convert.ToInt32(filter.MEndPeroid)
			select x).ToList();
			balanceList.ForEach(delegate(GLBalanceModel x)
			{
				if (!updateBalanceList.Exists((GLBalanceModel y) => y.MAccountID == x.MAccountID && y.MCurrencyID == x.MCurrencyID && y.MYearPeriod == x.MYearPeriod && y.MCheckGroupValueID == x.MCheckGroupValueID))
				{
					updateBalanceList.Add(x);
				}
			});
			updateBalanceList = (from x in updateBalanceList
			where filter.AccountIdList.Contains(x.MAccountID)
			select x).ToList();
			return updateBalanceList;
		}

		private static List<GLBalanceModel> GetFirstUnSettlementBalanceList(MContext ctx, List<int> virtualSettlementPeriodList, List<GLBalanceModel> balanceList, GLReportBaseFilterModel filter)
		{
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			int firstPeriod = virtualSettlementPeriodList.First();
			list = (from x in balanceList
			where x.MYearPeriod == firstPeriod
			select x).ToList();
			if (list == null || list.Count() == 0)
			{
				GLReportBaseFilterModel gLReportBaseFilterModel = new GLReportBaseFilterModel();
				gLReportBaseFilterModel = gLReportBaseFilterModel.Copy(filter);
				gLReportBaseFilterModel.MStartPeroid = Convert.ToString(firstPeriod);
				gLReportBaseFilterModel.MEndPeroid = Convert.ToString(firstPeriod);
				gLReportBaseFilterModel.IncludeUnapprovedVoucher = false;
				gLReportBaseFilterModel.IncludeCheckType = true;
				list = GetBalanceList(ctx, gLReportBaseFilterModel, true, true, null);
			}
			if (list != null && list.Count() > 0)
			{
				list.ForEach(delegate(GLBalanceModel x)
				{
					int mDC = x.MAccountModel.MDC;
					x.MEndBalance = ((mDC == 1) ? (x.MBeginBalance + x.MDebit - x.MCredit) : (x.MBeginBalance + x.MCredit - x.MDebit));
					x.MEndBalanceFor = ((mDC == 1) ? (x.MBeginBalanceFor + x.MDebitFor - x.MCreditFor) : (x.MBeginBalanceFor + x.MCreditFor - x.MDebitFor));
				});
			}
			return list;
		}

		private static List<GLBalanceModel> GetBalanceListFromUnApproveVoucher(MContext ctx, List<int> virtualSettlementPeriodList, GLReportBaseFilterModel filter)
		{
			List<GLBalanceModel> result = new List<GLBalanceModel>();
			GLVoucherListFilterModel gLVoucherListFilterModel = new GLVoucherListFilterModel();
			gLVoucherListFilterModel.MStartYearPeriod = virtualSettlementPeriodList.First();
			gLVoucherListFilterModel.MEndYearPeriod = virtualSettlementPeriodList.Last();
			gLVoucherListFilterModel.Status = "0";
			gLVoucherListFilterModel.AccountIDList = new List<string>();
			gLVoucherListFilterModel.AccountIDList.AddRange(filter.AccountIdList);
			filter.ChildrenAccountList = BDAccountHelper.GetLeafAccountIDList(filter.AccountIdList, GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountWithParentList);
			gLVoucherListFilterModel.MCurrencyID = filter.MCurrencyID;
			GLVoucherRepository gLVoucherRepository = new GLVoucherRepository();
			if (!string.IsNullOrWhiteSpace(filter.CheckGroupValueId))
			{
				gLVoucherListFilterModel.CheckGroupValueId = filter.CheckGroupValueId;
			}
			if (gLVoucherListFilterModel.AccountIDList != null && filter.ChildrenAccountList != null)
			{
				gLVoucherListFilterModel.AccountIDList.AddRange(filter.ChildrenAccountList);
			}
			List<GLVoucherModel> voucherListIncludeEntry = gLVoucherRepository.GetVoucherListIncludeEntry(ctx, gLVoucherListFilterModel);
			if (voucherListIncludeEntry == null || voucherListIncludeEntry.Count() == 0)
			{
				return result;
			}
			GLBalanceRepository gLBalanceRepository = new GLBalanceRepository();
			return gLBalanceRepository.GetUpdateBalanceList(ctx, voucherListIncludeEntry, 1);
		}

		private static List<int> GetVirtualSettlementPeriod(MContext ctx, GLReportBaseFilterModel filter, int firstUnSettlementPeriod)
		{
			List<int> list = new List<int>();
			int num = Convert.ToInt32(filter.MStartPeroid);
			int num2 = Convert.ToInt32(filter.MEndPeroid);
			if (num2 < firstUnSettlementPeriod)
			{
				return list;
			}
			if ((num2 >= firstUnSettlementPeriod && num <= firstUnSettlementPeriod) || num >= firstUnSettlementPeriod)
			{
				num = firstUnSettlementPeriod;
			}
			DateTime dateTime = DateTime.ParseExact(num + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime t = DateTime.ParseExact(num2 + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime t2 = dateTime;
			while (t2 <= t)
			{
				list.Add(t2.Year * 100 + t2.Month);
				t2 = t2.AddMonths(1);
			}
			return list;
		}

		private static List<GLBalanceModel> GetBalanceListFromPrePeriod(MContext ctx, List<GLBalanceModel> prePeriodBalanceList, int virtualSettlementPeriod)
		{
			DateTime dateTime = DateTime.ParseExact(virtualSettlementPeriod + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			int year = dateTime.Year;
			int month = dateTime.Month;
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			foreach (GLBalanceModel prePeriodBalance in prePeriodBalanceList)
			{
				GLBalanceModel balanceFromPrePeriod = GLBalanceRepository.GetBalanceFromPrePeriod(prePeriodBalance, year, month);
				list.Add(balanceFromPrePeriod);
			}
			return list;
		}

		public static List<BDAccountModel> GetAccountList(MContext ctx, GLReportBaseFilterModel filter)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			SqlWhere sqlWhere = new SqlWhere();
			if (!string.IsNullOrEmpty(filter.MAccountID))
			{
				sqlWhere.In("MItemID", filter.MAccountID.Split(','));
			}
			List<BDAccountModel> list = bDAccountRepository.GetBaseBDAccountList(ctx, sqlWhere, filter.IncludeDisabledAccount, null);
			if (filter.IsLeafAccount && list != null)
			{
				List<BDAccountModel> tempList = new List<BDAccountModel>();
				list.ForEach(delegate(BDAccountModel x)
				{
					if (!list.Exists((BDAccountModel y) => y.MParentID == x.MItemID))
					{
						tempList.Add(x);
					}
				});
				list = tempList;
				list = BDAccountHelper.OrderBy(list);
			}
			return list;
		}

		public static List<string> GetAccountIdListByFilter(MContext ctx, GLReportBaseFilterModel filter, List<AccountItemTreeModel> accountTreeList = null)
		{
			List<string> list = BDAccountHelper.GetAccountIdListByFilter(ctx, filter.MStartAccountID, filter.MEndAccountID, filter.AccountLevel, accountTreeList);
			if (list != null && list.Count() > 0)
			{
				list = list.Distinct().ToList();
			}
			return list;
		}

		public static List<AccountItemTreeModel> GetAccountIdsByFilter(MContext ctx, GLReportBaseFilterModel filter, List<AccountItemTreeModel> accountTreeList, List<BDAccountModel> accountList = null)
		{
			List<string> list = new List<string>();
			BDAccountListFilterModel bDAccountListFilterModel = new BDAccountListFilterModel();
			bDAccountListFilterModel.AccountEndIndex = filter.AccountEndIndex;
			bDAccountListFilterModel.AccountLevel = filter.AccountLevel;
			bDAccountListFilterModel.AccountStartIndex = filter.AccountStartIndex;
			bDAccountListFilterModel.IsLeafAccount = filter.IsLeafAccount;
			bool ignoreEndIndex = accountList?.Exists((BDAccountModel x) => x.MParentID == filter.MEndAccountID) ?? false;
			return BDAccountHelper.FilterAccountTreeList(ctx, bDAccountListFilterModel, accountTreeList, ignoreEndIndex);
		}

		public static string GetBalanceDirection(MContext ctx, int accountDirection, decimal amount)
		{
			int num = (!(amount == decimal.Zero)) ? accountDirection : 0;
			string result = "";
			switch (num)
			{
			case 0:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "even", "平");
				break;
			case 1:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Borrow", "Borrow");
				break;
			case -1:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Loan", "Loan");
				break;
			}
			return result;
		}

		public static T GetReportBaseFilterByReportID<T>(MContext ctx, string reportId)
		{
			RPTReportModel reportModel = RPTReportRepository.GetReportModel(reportId, ctx);
			if (reportModel != null)
			{
				string mContent = reportModel.MContent;
				JObject jObject = JObject.Parse(mContent);
				string value = jObject["Filter"].ToString();
				return JsonConvert.DeserializeObject<T>(value);
			}
			return default(T);
		}
	}
}
