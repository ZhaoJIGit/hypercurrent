using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTGeneralLedgerBusiness : IRPTGeneralLedgerBusiness, IRPTBizReportBusiness<RPTGeneralLedgerFilterModel>
	{
		private BDAccountRepository AccountDal = new BDAccountRepository();

		private GLBalanceRepository BalanceDal = new GLBalanceRepository();

		private List<string> DataColumns;

		private List<RPTGeneralLedgerModel> DataList;

		private List<RPTGeneralLedgerModel> CheckTypeDataList = new List<RPTGeneralLedgerModel>();

		private List<BDAccountModel> AccountList;

		private List<GLBalanceModel> BalanceList;

		private List<GLVoucherModel> VoucherList;

		private Dictionary<string, int> AccountLevels;

		private bool IncludeForeginCurrency = false;

		private string EmptyChecktypeGroupValueID = "";

		private int CirculationCount = 0;

		public string GetBizReportJson(MContext ctx, RPTGeneralLedgerFilterModel filter)
		{
			MLogger.Log("GetBizReportJson-1::"+Newtonsoft.Json.JsonConvert.SerializeObject(filter));
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			if (string.IsNullOrEmpty(filter.MBaseCurrencyID))
			{
				filter.MBaseCurrencyID = ctx.MBasCurrencyID;
			}
			IncludeForeginCurrency = (!string.IsNullOrEmpty(filter.MCurrencyID) && filter.MCurrencyID != "0" && filter.MCurrencyID != filter.MBaseCurrencyID);
			MLogger.Log("GetBizReportJson-2");

			CirculationCount = AccountHelper.GetCirculationCount(Convert.ToInt32(filter.MStartPeroid), Convert.ToInt32(filter.MEndPeroid));
			MLogger.Log("GetBizReportJson-2");

			return iRPTReportBusiness.GetBizReportJson(ctx, filter, delegate
			{
				AccountList = GLReportHelper.GetAccountList(ctx, filter);
				BalanceList = GLReportHelper.GetBalanceList(ctx, filter, false, true, null);
				AccountLevels = GLReportHelper.GetAccountLevels(ctx, filter, AccountList);
				if (!filter.MDisplayNoAccurrenceAmount)
				{
					VoucherList = GetVoucherList(ctx, filter);
				}
				return GetBalanceReportModel(ctx, filter);
			});
		}

		private List<GLVoucherModel> GetVoucherList(MContext ctx, RPTGeneralLedgerFilterModel filter)
		{
			GLVoucherListFilterModel gLVoucherListFilterModel = new GLVoucherListFilterModel();
			GLVoucherRepository gLVoucherRepository = new GLVoucherRepository();
			gLVoucherListFilterModel.Status = (filter.IncludeUnapprovedVoucher ? null : "1");
			gLVoucherListFilterModel.MStartYearPeriod = ((!string.IsNullOrWhiteSpace(filter.MStartPeroid)) ? int.Parse(filter.MStartPeroid) : 0);
			gLVoucherListFilterModel.MEndYearPeriod = ((!string.IsNullOrWhiteSpace(filter.MEndPeroid)) ? int.Parse(filter.MEndPeroid) : 0);
			gLVoucherListFilterModel.IncludeDraft = false;
			gLVoucherListFilterModel.CheckGroupValueId = filter.CheckGroupValueId;
			return gLVoucherRepository.GetVoucherListIncludeEntry(ctx, gLVoucherListFilterModel);
		}

		private List<GLBalanceModel> GetBalanceList(MContext ctx, RPTGeneralLedgerFilterModel filter)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.LessOrEqual("MYearPeriod", filter.MEndPeroid);
			sqlWhere.GreaterOrEqual("MYearPeriod", filter.MStartPeroid);
			if (!string.IsNullOrEmpty(filter.MCurrencyID) && filter.MCurrencyID != "0")
			{
				sqlWhere.Equal("MCurrencyID", filter.MCurrencyID);
			}
			if (!filter.IncludeCheckType)
			{
				sqlWhere.Equal("MCheckGroupValueID", "0");
			}
			return BalanceDal.GetBalanceListIncludeCheckGroupValue(ctx, sqlWhere, filter.IncludeCheckType);
		}

		public BizReportModel GetBalanceReportModel(MContext ctx, RPTGeneralLedgerFilterModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.GeneralLedger);
			SetCommonData(ctx, filter);
			SetDataColumns(ctx, filter);
			SetTitle(bizReportModel, filter, ctx);
			SetRowHead(bizReportModel, filter, ctx);
			SetRowData(bizReportModel, filter, ctx);
			return bizReportModel;
		}

		private void SetCommonData(MContext context, RPTGeneralLedgerFilterModel filter)
		{
			DataColumns = new List<string>();
			DataList = new List<RPTGeneralLedgerModel>();
			List<BDAccountModel> list = AccountList;
			if (filter.AccountIdList != null && filter.AccountIdList.Count() > 0)
			{
				list = (from x in list
				where filter.AccountIdList.Contains(x.MItemID)
				select x).ToList();
			}
			foreach (BDAccountModel item in list)
			{
				List<GLBalanceModel> list2 = (from x in BalanceList
				where x.MAccountID == item.MItemID && x.MCheckGroupValueID == "0"
				select x).ToList();
				if (list2 != null && list2.Count() > 0)
				{
					List<RPTGeneralLedgerModel> collection = ToRptGeneralLedgerModels(list2, item, context, filter);
					DataList.AddRange(collection);
				}
				bool flag = AccountList.Exists((BDAccountModel x) => x.MParentID == item.MItemID);
				if (filter.IncludeCheckType && !flag && item.MCheckGroupID != "0")
				{
					List<GLBalanceModel> list3 = (from x in BalanceList
					where x.MAccountID == item.MItemID && x.MCheckGroupValueID != "0"
					select x).ToList();
					if (list3 != null && list3.Count() > 0)
					{
						List<string> list4 = (from x in list3
						group x by x.MCheckGroupValueID into y
						select y.Key).ToList();
						foreach (string item2 in list4)
						{
							List<GLBalanceModel> balanceList = (from x in list3
							where x.MCheckGroupValueID == item2
							select x).ToList();
							List<RPTGeneralLedgerModel> collection2 = ToRptGeneralLedgerModels(balanceList, item, context, filter);
							DataList.AddRange(collection2);
						}
					}
				}
			}
		}

		private void SetDataColumns(MContext ctx, RPTGeneralLedgerFilterModel filter)
		{
			DataColumns = new List<string>();
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "AccountCode", "Code");
			DataColumns.Add(text);
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "AccountName", "Name");
			DataColumns.Add(text2);
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "AccountingPeriod", "Accounting period");
			DataColumns.Add(text3);
			string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Summary", "Summary");
			DataColumns.Add(text4);
			string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Debit", "Debit");
			if (IncludeForeginCurrency)
			{
				string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "DebitOriginalCurrency", "Debit(Original currency)");
				DataColumns.Add(text6);
				text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "DebitStandardCurrency", "Debit(Standard currency)");
			}
			DataColumns.Add(text5);
			string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Credit", "Credit");
			if (IncludeForeginCurrency)
			{
				string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "CreditOriginalCurrency", "Credit(Original currency)");
				DataColumns.Add(text8);
				text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "CreditStandardCurrency", "Credit(Standard currency)");
			}
			DataColumns.Add(text7);
			string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Direction", "Direction");
			DataColumns.Add(text9);
			string text10 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "EndBalance", "余额");
			if (IncludeForeginCurrency)
			{
				string text11 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BalanceOriginalCurrency", "Balance(Original currency)");
				DataColumns.Add(text11);
				text10 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BalanceStandardCurrency", "Balance(Standard currency)");
			}
			DataColumns.Add(text10);
		}

		private void SetTitle(BizReportModel reportModel, RPTGeneralLedgerFilterModel filter, MContext context)
		{
			reportModel.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "GeneralLedger", "General Ledger");
			reportModel.Title2 = context.MOrgName;
			string text = Convert.ToString(filter.MStartPeroid);
			text = ((text.Length <= 5) ? (text.Substring(0, text.Length - 1) + "-" + text.Substring(4, 1)) : (text.Substring(0, text.Length - 2) + "-" + text.Substring(4, 2)));
			string text2 = Convert.ToString(filter.MEndPeroid);
			text2 = ((text2.Length <= 5) ? (text2.Substring(0, text2.Length - 1) + "-" + text2.Substring(4, 1)) : (text2.Substring(0, text2.Length - 2) + "-" + text2.Substring(4, 2)));
			reportModel.Title3 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ForThePeriod", "For the period ") + " " + text + " " + COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "To", "to") + " " + text2;
		}

		private void SetRowHead(BizReportModel model, RPTGeneralLedgerFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			int num = 0;
			foreach (string dataColumn in DataColumns)
			{
				BizReportCellType cellType = BizReportCellType.Text;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = dataColumn,
					CellType = cellType
				});
				num++;
			}
			model.AddRow(bizReportRowModel);
		}

		private void SetRowData(BizReportModel model, RPTGeneralLedgerFilterModel filter, MContext context)
		{
			IEnumerable<IGrouping<string, RPTGeneralLedgerModel>> enumerable = from x in DataList
			group x by x.GroupByFeild;
			if (DataList.Count() == 0 && CheckTypeDataList.Count() > 0)
			{
				enumerable = from x in CheckTypeDataList
				group x by x.GroupByFeild;
			}
			foreach (IGrouping<string, RPTGeneralLedgerModel> item in enumerable)
			{
				string key = item.Key;
				List<RPTGeneralLedgerModel> list = item.ToList();
				list = (filter.MDisplayNoAccurrenceAmount ? list : FilterZoreRows(context, list, enumerable, 1, filter.IncludeCheckType));
				list = (filter.MDisplayZeorEndBalance ? list : FilterZoreRows(context, list, enumerable, 2, filter.IncludeCheckType));
				List<BizReportRowModel> list2 = ToBizReportRowModelList(context, filter, list);
				if (list2 != null && list2.Count() != 0)
				{
					list2.ForEach(delegate(BizReportRowModel x)
					{
						model.AddRow(x);
					});
				}
			}
		}

		private List<RPTGeneralLedgerModel> FilterZoreRows(MContext ctx, List<RPTGeneralLedgerModel> rptList, IEnumerable<IGrouping<string, RPTGeneralLedgerModel>> groupData, int filterType, bool IncludeCheckType)
		{
			List<RPTGeneralLedgerModel> list = new List<RPTGeneralLedgerModel>();
			if (rptList == null || rptList.Count() == 0)
			{
				return list;
			}
			string accountId = rptList.First().MAccountID;
			BDAccountModel bDAccountModel = (from x in AccountList
			where x.MItemID == accountId
			select x).FirstOrDefault();
			if (bDAccountModel == null)
			{
				return list;
			}
			List<BDAccountModel> childrenAccountByRecursion = BDAccountHelper.GetChildrenAccountByRecursion(bDAccountModel, AccountList, false);
			bool flag = childrenAccountByRecursion != null && childrenAccountByRecursion.Count() > 0;
			IEnumerable<IGrouping<string, RPTGeneralLedgerModel>> enumerable = from x in rptList
			group x by x.MPeriod;
			foreach (IGrouping<string, RPTGeneralLedgerModel> item in enumerable)
			{
				string period = item.Key;
				List<RPTGeneralLedgerModel> list2 = item.ToList();
				RPTGeneralLedgerModel balanceRpt = list2.FirstOrDefault((RPTGeneralLedgerModel x) => x.AmountType == filterType);
				if (balanceRpt == null)
				{
					return list;
				}
				string text = balanceRpt.GroupByFeild.Replace(accountId, "");
				if (filterType == 1 && (Math.Abs(balanceRpt.MCredit) > decimal.Zero || Math.Abs(balanceRpt.MDebit) > decimal.Zero))
				{
					goto IL_023f;
				}
				if (filterType == 2 && Math.Abs(balanceRpt.MBalance) > decimal.Zero)
				{
					goto IL_023f;
				}
				int num = (filterType == 1 && VoucherList.Any((GLVoucherModel t) => t.MYear == Convert.ToInt32(period.Substring(0, 4)) && t.MPeriod == Convert.ToInt32(period.Substring(4, 2)) && t.MVoucherEntrys.Any((GLVoucherEntryModel et) => et.MAccountID == balanceRpt.MAccountID && (!IncludeCheckType || et.MCheckGroupValueID == balanceRpt.MCheckGroupValueID)))) ? 1 : 0;
				goto IL_0240;
				IL_023f:
				num = 1;
				goto IL_0240;
				IL_02d3:
				int num2;
				if (num2 == 0 && (balanceRpt.MBalance == decimal.Zero & flag))
				{
					foreach (BDAccountModel item2 in childrenAccountByRecursion)
					{
						bool flag2 = false;
						foreach (IGrouping<string, RPTGeneralLedgerModel> item3 in from x in groupData
						where x.Key.StartsWith(item2.MItemID)
						select x)
						{
							List<RPTGeneralLedgerModel> list3 = item3.ToList();
							if (list3 != null && list3.Count() != 0)
							{
								List<RPTGeneralLedgerModel> list4 = list3.Where((RPTGeneralLedgerModel x) => x.MPeriod == period).ToList();
								if (list4 != null && list4.Count() != 0)
								{
									foreach (RPTGeneralLedgerModel item4 in list4)
									{
										if (filterType == 1 && item4 != null && (Math.Abs(item4.MCredit) > decimal.Zero || Math.Abs(item4.MDebit) > decimal.Zero))
										{
											goto IL_04f9;
										}
										if (filterType == 2 && item4 != null && Math.Abs(item4.MBalance) > decimal.Zero)
										{
											goto IL_04f9;
										}
										int num3 = (filterType == 1 && VoucherList.Any((GLVoucherModel t) => t.MYear == Convert.ToInt32(period.Substring(0, 4)) && t.MPeriod == Convert.ToInt32(period.Substring(4, 2)) && t.MVoucherEntrys.Any((GLVoucherEntryModel et) => et.MAccountID == item4.MAccountID && (!IncludeCheckType || et.MCheckGroupValueID == item4.MCheckGroupValueID)))) ? 1 : 0;
										goto IL_04fa;
										IL_04f9:
										num3 = 1;
										goto IL_04fa;
										IL_04fa:
										if (num3 != 0)
										{
											flag2 = true;
											list.AddRange(list2);
											break;
										}
									}
									if (flag2)
									{
										break;
									}
								}
							}
						}
						if (flag2)
						{
							break;
						}
					}
				}
				continue;
				IL_02cb:
				num2 = ((!flag) ? 1 : 0);
				goto IL_02d3;
				IL_0240:
				if (num != 0)
				{
					list.AddRange(list2);
					continue;
				}
				if (filterType == 1 && Math.Abs(balanceRpt.MCredit) == decimal.Zero && Math.Abs(balanceRpt.MDebit) == decimal.Zero)
				{
					goto IL_02cb;
				}
				if (filterType == 2 && Math.Abs(balanceRpt.MBalance) == decimal.Zero)
				{
					goto IL_02cb;
				}
				num2 = 0;
				goto IL_02d3;
			}
			return list;
		}

		private List<BizReportRowModel> ToBizReportRowModelList(MContext ctx, RPTGeneralLedgerFilterModel filter, List<RPTGeneralLedgerModel> rptList)
		{
			int rowSpan = rptList.Count();
			int num = 0;
			List<BizReportRowModel> list = new List<BizReportRowModel>();
			foreach (RPTGeneralLedgerModel rpt in rptList)
			{
				if (filter.AccountLevel <= 0 || AccountLevels[rpt.MAccountID] <= filter.AccountLevel)
				{
					BizReportRowModel bizReportRowModel = new BizReportRowModel();
					if (num == 0)
					{
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = rpt.MNumberID,
							CellType = BizReportCellType.Text,
							CellLink = GetCellLink(ctx, rpt, filter),
							RowSpan = rowSpan
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = rpt.MAccountName,
							CellType = BizReportCellType.Text,
							CellLink = GetCellLink(ctx, rpt, filter),
							RowSpan = rowSpan
						});
					}
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = rpt.MPeriod,
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = rpt.MSummary,
						CellType = BizReportCellType.Text
					});
					decimal d = default(decimal);
					if (IncludeForeginCurrency)
					{
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = AccountHelper.MoneyToString(rpt.MDebitFor, ""),
							CellType = BizReportCellType.Money
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = AccountHelper.MoneyToString(rpt.MDebit, ""),
							CellType = BizReportCellType.Money
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = AccountHelper.MoneyToString(rpt.MCreditFor, ""),
							CellType = BizReportCellType.Money
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = AccountHelper.MoneyToString(rpt.MCredit, ""),
							CellType = BizReportCellType.Money
						});
						d += Math.Abs(rpt.MDebitFor);
						d += Math.Abs(rpt.MDebit);
						d += Math.Abs(rpt.MCreditFor);
						d += Math.Abs(rpt.MCredit);
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = rpt.MDC,
							CellType = BizReportCellType.Text
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = AccountHelper.MoneyToString(rpt.MBalanceFor, "0"),
							CellType = BizReportCellType.Money
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = AccountHelper.MoneyToString(rpt.MBalance, "0"),
							CellType = BizReportCellType.Money
						});
						d += Math.Abs(rpt.MBalanceFor);
						d += Math.Abs(rpt.MBalance);
					}
					else
					{
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = AccountHelper.MoneyToString(rpt.MDebit, ""),
							CellType = BizReportCellType.Money
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = AccountHelper.MoneyToString(rpt.MCredit, ""),
							CellType = BizReportCellType.Money
						});
						d += Math.Abs(rpt.MDebit);
						d += Math.Abs(rpt.MCredit);
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = rpt.MDC,
							CellType = BizReportCellType.Text
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = AccountHelper.MoneyToString(rpt.MBalance, "0"),
							CellType = BizReportCellType.Money
						});
						d += Math.Abs(rpt.MBalance);
					}
					bizReportRowModel.TotalValue = d;
					list.Add(bizReportRowModel);
					num++;
				}
			}
			return list;
		}

		private BizReportCellLinkModel GetCellLink(MContext ctx, RPTGeneralLedgerModel rptModel, RPTGeneralLedgerFilterModel filter)
		{
			BizReportCellLinkModel bizReportCellLinkModel = new BizReportCellLinkModel();
			string text3 = bizReportCellLinkModel.Title = (bizReportCellLinkModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "SubsidiaryLedger", "Subsidiary Ledger"));
			bizReportCellLinkModel.Url = "/Report/Report2/42?accountId=" + rptModel.MAccountID;
			bizReportCellLinkModel.DisabledEvent = true;
			return bizReportCellLinkModel;
		}

		private BizSubRptCreateModel GetBizSubRptModel(MContext ctx, RPTGeneralLedgerFilterModel filter, RPTGeneralLedgerModel rptModel)
		{
			BizSubRptCreateModel bizSubRptCreateModel = new BizSubRptCreateModel();
			bizSubRptCreateModel.Text = rptModel.MAccountName + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "SubsidiaryLedger", "Subsidiary Ledger");
			bizSubRptCreateModel.ReportType = BizReportType.SubsidiaryLedger;
			RPTSubsidiaryLedgerFilterModel rPTSubsidiaryLedgerFilterModel = (RPTSubsidiaryLedgerFilterModel)(bizSubRptCreateModel.ReportFilter = new RPTSubsidiaryLedgerFilterModel());
			return bizSubRptCreateModel;
		}

		private List<RPTGeneralLedgerModel> ToRptGeneralLedgerModels(List<GLBalanceModel> balanceList, BDAccountModel accountModel, MContext ctx, RPTGeneralLedgerFilterModel filter)
		{
			List<RPTGeneralLedgerModel> list = new List<RPTGeneralLedgerModel>();
			for (int i = 0; i < CirculationCount; i++)
			{
				int yearPeriod = AccountHelper.GetYearPeriod(Convert.ToInt32(filter.MStartPeroid), i);
				List<GLBalanceModel> list2 = (from x in balanceList
				where x.MYearPeriod == yearPeriod
				select x).ToList();
				if (list2 != null && list2.Count() != 0 && !(list2.First().MCheckGroupValueID == EmptyChecktypeGroupValueID))
				{
					for (int j = 0; j < 3; j++)
					{
						RPTGeneralLedgerModel rPTGeneralLedgerModel = new RPTGeneralLedgerModel();
						rPTGeneralLedgerModel.MAccountID = accountModel.MItemID;
						rPTGeneralLedgerModel.MAccountParentID = accountModel.MParentID;
						GLBalanceModel gLBalanceModel = list2.First();
						RPTGeneralLedgerModel rPTGeneralLedgerModel2 = rPTGeneralLedgerModel;
						string text = rPTGeneralLedgerModel2.MCheckGroupValueID = gLBalanceModel.MCheckGroupValueID;
						string text2 = text;
						rPTGeneralLedgerModel.AmountType = j;
						if (filter.IncludeCheckType && !string.IsNullOrEmpty(text2) && text2 != "0")
						{
							string checkTypeValueName = GLBalanceBusiness.GetCheckTypeValueName(ctx, gLBalanceModel);
							if (string.IsNullOrWhiteSpace(checkTypeValueName))
							{
								EmptyChecktypeGroupValueID = text2;
								return list;
							}
							rPTGeneralLedgerModel.MAccountName = accountModel.MName + "_" + checkTypeValueName;
							rPTGeneralLedgerModel.MNumberID = "";
							rPTGeneralLedgerModel.GroupByFeild = accountModel.MItemID + text2;
						}
						else
						{
							rPTGeneralLedgerModel.MAccountName = accountModel.MName;
							rPTGeneralLedgerModel.MNumberID = accountModel.MNumber;
							rPTGeneralLedgerModel.GroupByFeild = accountModel.MItemID;
						}
						rPTGeneralLedgerModel.MPeriod = Convert.ToString(yearPeriod);
						if (j == 0 && i == 0)
						{
							rPTGeneralLedgerModel.MSummary = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "InitialBalance", "Initial balance");
							if (accountModel.MDC == 1)
							{
								rPTGeneralLedgerModel.MDebit = list2.Sum((GLBalanceModel x) => x.MBeginBalance);
								rPTGeneralLedgerModel.MDebitFor = list2.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
							}
							else
							{
								rPTGeneralLedgerModel.MCredit = list2.Sum((GLBalanceModel x) => x.MBeginBalance);
								rPTGeneralLedgerModel.MCreditFor = list2.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
							}
							rPTGeneralLedgerModel.MBalance = list2.Sum((GLBalanceModel x) => x.MBeginBalance);
							rPTGeneralLedgerModel.MBalanceFor = list2.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
							rPTGeneralLedgerModel.MDC = GLReportHelper.GetBalanceDirection(ctx, accountModel.MDC, rPTGeneralLedgerModel.MBalance);
							if (list2.Sum((GLBalanceModel x) => x.MCredit) > decimal.Zero || list2.Sum((GLBalanceModel x) => x.MDebit) > decimal.Zero || list2.Sum((GLBalanceModel x) => x.MEndBalance) > decimal.Zero)
							{
								rPTGeneralLedgerModel.IsShowInitBalance = true;
							}
							goto IL_0846;
						}
						if (i <= 0 || j != 0)
						{
							if (j == 1)
							{
								rPTGeneralLedgerModel.MSummary = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "TotalOfThisPeriod", "Total of this period");
								rPTGeneralLedgerModel.MDebit = list2.Sum((GLBalanceModel x) => x.MDebit);
								rPTGeneralLedgerModel.MDebitFor = list2.Sum((GLBalanceModel x) => x.MDebitFor);
								rPTGeneralLedgerModel.MCredit = list2.Sum((GLBalanceModel x) => x.MCredit);
								rPTGeneralLedgerModel.MCreditFor = list2.Sum((GLBalanceModel x) => x.MCreditFor);
							}
							else
							{
								rPTGeneralLedgerModel.MSummary = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "AccumulativeTotalOfThisYear", "Accumulative total of this year");
								rPTGeneralLedgerModel.MDebit = list2.Sum((GLBalanceModel x) => x.MYtdDebit);
								rPTGeneralLedgerModel.MDebitFor = list2.Sum((GLBalanceModel x) => x.MYtdDebitFor);
								rPTGeneralLedgerModel.MCredit = list2.Sum((GLBalanceModel x) => x.MYtdCredit);
								rPTGeneralLedgerModel.MCreditFor = list2.Sum((GLBalanceModel x) => x.MYtdCreditFor);
							}
							if (i == CirculationCount - 1)
							{
								if (accountModel.MDC == 1)
								{
									rPTGeneralLedgerModel.MBalance = list2.Sum((GLBalanceModel x) => x.MBeginBalance) + list2.Sum((GLBalanceModel x) => x.MDebit) - list2.Sum((GLBalanceModel x) => x.MCredit);
									rPTGeneralLedgerModel.MBalanceFor = list2.Sum((GLBalanceModel x) => x.MBeginBalanceFor) + list2.Sum((GLBalanceModel x) => x.MDebitFor) - list2.Sum((GLBalanceModel x) => x.MCreditFor);
								}
								else
								{
									rPTGeneralLedgerModel.MBalance = list2.Sum((GLBalanceModel x) => x.MBeginBalance) + list2.Sum((GLBalanceModel x) => x.MCredit) - list2.Sum((GLBalanceModel x) => x.MDebit);
									rPTGeneralLedgerModel.MBalanceFor = list2.Sum((GLBalanceModel x) => x.MBeginBalanceFor) + list2.Sum((GLBalanceModel x) => x.MCreditFor) - list2.Sum((GLBalanceModel x) => x.MDebitFor);
								}
							}
							else
							{
								rPTGeneralLedgerModel.MBalance = list2.Sum((GLBalanceModel x) => x.MEndBalance);
								rPTGeneralLedgerModel.MBalanceFor = list2.Sum((GLBalanceModel x) => x.MEndBalanceFor);
							}
							rPTGeneralLedgerModel.MDC = GLReportHelper.GetBalanceDirection(ctx, accountModel.MDC, rPTGeneralLedgerModel.MBalance);
							goto IL_0846;
						}
						continue;
						IL_0846:
						list.Add(rPTGeneralLedgerModel);
					}
				}
			}
			return list;
		}
	}
}
