using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.BD;
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTSubsidiaryLedgerBusiness : IRPTSubsidiaryLedgerBussiness, IRPTBizReportBusiness<RPTSubsidiaryLedgerFilterModel>
	{
		private BDAccountRepository AccountDal = new BDAccountRepository();

		private GLBalanceRepository BalanceDal = new GLBalanceRepository();

		private GLVoucherRepository VoucherDal = new GLVoucherRepository();

		private List<string> DataColumns;

		private List<RPTSubsidiaryLedgerModel> DataList;

		private List<RPTSubsidiaryLedgerModel> CheckTypeDataList = new List<RPTSubsidiaryLedgerModel>();

		private BDAccountModel Account;

		private List<GLBalanceModel> BalanceList;

		private List<GLVoucherModel> VoucherList;

		private List<BDAccountModel> AccoutList;

		private bool IncludeForeginCurrency = false;

		private string ReportTitle2CheckTypeValueName = string.Empty;

		private int CirculationCount = 0;

		private bool IsNavTo = false;

		private Dictionary<int, string> CheckTypeDic = new Dictionary<int, string>();

		public string GetBizReportJson(MContext ctx, RPTSubsidiaryLedgerFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			IsNavTo = !string.IsNullOrWhiteSpace(filter.NavReportID);
			if (IsNavTo)
			{
				string mReportID = filter.MReportID;
				string mAccountID = filter.MAccountID;
				filter = GLReportHelper.GetReportBaseFilterByReportID<RPTSubsidiaryLedgerFilterModel>(ctx, filter.NavReportID);
				filter.MStartAccountID = mAccountID;
				filter.MEndAccountID = mAccountID;
				filter.MReportID = mReportID;
			}
			if (string.IsNullOrEmpty(filter.MBaseCurrencyID))
			{
				filter.MBaseCurrencyID = ctx.MBasCurrencyID;
			}
			IncludeForeginCurrency = (!string.IsNullOrEmpty(filter.MCurrencyID) && filter.MCurrencyID != "0" && filter.MCurrencyID != filter.MBaseCurrencyID);
			CirculationCount = AccountHelper.GetCirculationCount(Convert.ToInt32(filter.MStartPeroid), Convert.ToInt32(filter.MEndPeroid));
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => GetBizReportModel(ctx, filter));
		}

		public BizReportModel GetBizReportModel(MContext ctx, RPTSubsidiaryLedgerFilterModel filter)
		{
			AccoutList = AccountDal.GetBaseBDAccountList(ctx, null, false, null);
			BDAccountModel defaultAccountModel = GetDefaultAccountModel(AccoutList, filter);
			filter.ChildrenAccountList = (from x in BDAccountHelper.GetChildrenAccountByRecursion(defaultAccountModel, AccoutList, false)
			select x.MItemID).ToList();
			List<GLBalanceModel> balanceList = GLReportHelper.GetBalanceList(ctx, filter, false, !IsNavTo, null);
			balanceList = (from x in balanceList
			where x.MPeriod != 0
			select x).ToList();
			if (string.IsNullOrWhiteSpace(filter.MAccountID))
			{
				filter.MAccountID = defaultAccountModel.MItemID;
			}
			List<GLVoucherModel> voucherList = GetVoucherList(ctx, filter);
			if (balanceList != null && balanceList.Count() > 0 && (voucherList == null || voucherList.Count() == 0))
			{
				balanceList = (from x in balanceList
				where Math.Abs(x.MBeginBalance) + Math.Abs(x.MCredit) + Math.Abs(x.MDebit) + Math.Abs(x.MYtdCredit) + Math.Abs(x.MYtdDebit) + Math.Abs(x.MEndBalance) != decimal.Zero
				select x).ToList();
			}
			return GetReportModel(ctx, filter, defaultAccountModel, balanceList, voucherList);
		}

		public string GetBizReportModelJson(MContext ctx, RPTSubsidiaryLedgerFilterModel filter)
		{
			if (string.IsNullOrEmpty(filter.MBaseCurrencyID))
			{
				filter.MBaseCurrencyID = ctx.MBasCurrencyID;
			}
			IncludeForeginCurrency = (!string.IsNullOrEmpty(filter.MCurrencyID) && filter.MCurrencyID != "0" && filter.MCurrencyID != filter.MBaseCurrencyID);
			MContext ctx2 = (filter.Context != null) ? filter.Context : ctx;
			BizReportModel bizReportModel = GetBizReportModel(ctx2, filter);
			return JsonConvert.SerializeObject(bizReportModel);
		}

		private BDAccountModel GetDefaultAccountModel(List<BDAccountModel> accountList, RPTSubsidiaryLedgerFilterModel filter)
		{
			BDAccountModel bDAccountModel = null;
			if (filter != null && !string.IsNullOrWhiteSpace(filter.MAccountID))
			{
				bDAccountModel = (from x in accountList
				where x.MItemID == filter.MAccountID
				select x).FirstOrDefault();
			}
			else if (!string.IsNullOrWhiteSpace(filter.MStartAccountID))
			{
				bDAccountModel = (from x in accountList
				where x.MItemID == filter.MStartAccountID
				select x).FirstOrDefault();
			}
			if (bDAccountModel == null)
			{
				bDAccountModel = (from x in accountList
				where x.MCode == "1001"
				select x).First();
			}
			return bDAccountModel;
		}

		private List<GLVoucherModel> GetVoucherList(MContext ctx, RPTSubsidiaryLedgerFilterModel filter)
		{
			GLVoucherListFilterModel gLVoucherListFilterModel = new GLVoucherListFilterModel();
			GLVoucherRepository gLVoucherRepository = new GLVoucherRepository();
			if (!string.IsNullOrWhiteSpace(filter.MAccountID))
			{
				gLVoucherListFilterModel.AccountIDList = (gLVoucherListFilterModel.AccountIDList ?? new List<string>());
				gLVoucherListFilterModel.AccountIDList.Add(filter.MAccountID);
				if (filter.ChildrenAccountList != null)
				{
					gLVoucherListFilterModel.AccountIDList.AddRange(filter.ChildrenAccountList);
				}
			}
			gLVoucherListFilterModel.Status = (filter.IncludeUnapprovedVoucher ? null : "1");
			gLVoucherListFilterModel.MStartYearPeriod = ((!string.IsNullOrWhiteSpace(filter.MStartPeroid)) ? int.Parse(filter.MStartPeroid) : 0);
			gLVoucherListFilterModel.MEndYearPeriod = ((!string.IsNullOrWhiteSpace(filter.MEndPeroid)) ? int.Parse(filter.MEndPeroid) : 0);
			gLVoucherListFilterModel.IncludeDraft = false;
			gLVoucherListFilterModel.CheckGroupValueId = filter.CheckGroupValueId;
			return gLVoucherRepository.GetVoucherListIncludeEntry(ctx, gLVoucherListFilterModel);
		}

		public BizReportModel GetReportModel(MContext ctx, RPTSubsidiaryLedgerFilterModel filter, BDAccountModel account, List<GLBalanceModel> balanceList, List<GLVoucherModel> voucherList)
		{
			BizReportModel bizReportModel = new BizReportModel();
			BalanceList = balanceList;
			Account = account;
			VoucherList = voucherList;
			bizReportModel.Type = Convert.ToInt32(BizReportType.SubsidiaryLedger);
			SetCommonData(ctx, filter);
			SetDataColumns(ctx, filter);
			SetTitle(bizReportModel, filter, ctx);
			SetRowHead(bizReportModel, filter, ctx);
			SetRowData(bizReportModel, filter, ctx);
			return bizReportModel;
		}

		private void SetCommonData(MContext context, RPTSubsidiaryLedgerFilterModel filter)
		{
			DataColumns = new List<string>();
			DataList = new List<RPTSubsidiaryLedgerModel>();
			List<GLBalanceModel> list = null;
			list = ((!filter.IncludeCheckType) ? (from x in BalanceList
			where x.MAccountID == Account.MItemID && x.MCheckGroupValueID == "0"
			select x).ToList() : (from x in BalanceList
			where x.MAccountID == Account.MItemID && x.MCheckGroupValueID != "0"
			select x).ToList());
			if (filter.CheckTypeValueList != null && filter.CheckTypeValueList.Count() > 0)
			{
				list = FilterBalanceList(list, filter.CheckTypeValueList);
			}
			DataList = GetSubsidiaryLedgerModels(context, list, Account, filter);
		}

		private List<GLBalanceModel> FilterBalanceList(List<GLBalanceModel> balanceList, List<NameValueModel> checkTypeList)
		{
			List<GLBalanceModel> result = new List<GLBalanceModel>();
			if (balanceList == null || balanceList.Count() == 0)
			{
				return result;
			}
			IEnumerable<IGrouping<string, string>> enumerable = from x in checkTypeList
			group x.MValue by x.MName;
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
						balanceList = (from x in balanceList
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MContactID)
						select x).ToList();
						break;
					case 1:
						balanceList = (from x in balanceList
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MEmployeeID)
						select x).ToList();
						break;
					case 3:
						balanceList = (from x in balanceList
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MExpItemID)
						select x).ToList();
						break;
					case 2:
						balanceList = (from x in balanceList
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MMerItemID)
						select x).ToList();
						break;
					case 4:
						balanceList = (from x in balanceList
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MPaItemID) || checkTypeValue.Contains(x.MCheckGroupValueModel.MPaItemGroupID)
						select x).ToList();
						break;
					case 5:
						balanceList = (from x in balanceList
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem1)
						select x).ToList();
						break;
					case 6:
						balanceList = (from x in balanceList
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem2)
						select x).ToList();
						break;
					case 7:
						balanceList = (from x in balanceList
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem3)
						select x).ToList();
						break;
					case 8:
						balanceList = (from x in balanceList
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem4)
						select x).ToList();
						break;
					case 9:
						balanceList = (from x in balanceList
						where checkTypeValue.Contains(x.MCheckGroupValueModel.MTrackItem5)
						select x).ToList();
						break;
					}
				}
			}
			return balanceList;
		}

		private void SetDataColumns(MContext ctx, RPTSubsidiaryLedgerFilterModel filter)
		{
			DataColumns = new List<string>();
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Date", "Date");
			DataColumns.Add(text);
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "VoucherNumber", "Voucher number");
			DataColumns.Add(text2);
			if (filter.IncludeCheckType)
			{
				string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountName", "Name");
				DataColumns.Add(text3);
				string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "CheckType", "核算维度");
				DataColumns.Add(text4);
			}
			string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Summary", "Summary");
			DataColumns.Add(text5);
			string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Debit", "Debit");
			if (IncludeForeginCurrency)
			{
				string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "DebitOriginalCurrency", "Debit(Original currency)");
				DataColumns.Add(text7);
				text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "DebitStandardCurrency", "Debit(Standard currency)");
			}
			DataColumns.Add(text6);
			string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Credit", "Credit");
			if (IncludeForeginCurrency)
			{
				string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "CreditOriginalCurrency", "Credit(Original currency)");
				DataColumns.Add(text9);
				text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "CreditStandardCurrency", "Credit(Standard currency)");
			}
			DataColumns.Add(text8);
			string text10 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Direction", "Direction");
			DataColumns.Add(text10);
			string text11 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Balance", "Balance");
			if (IncludeForeginCurrency)
			{
				string text12 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BalanceOriginalCurrency", "Balance(Original currency)");
				DataColumns.Add(text12);
				text11 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BalanceStandardCurrency", "Balance(Standard currency)");
			}
			DataColumns.Add(text11);
		}

		private void SetTitle(BizReportModel reportModel, RPTSubsidiaryLedgerFilterModel filter, MContext context)
		{
			reportModel.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "SubsidiaryLedger", "Subsidiary Ledger");
			reportModel.Title2 = $"{Account.MNumber}-{Account.MName}";
			if (!string.IsNullOrWhiteSpace(ReportTitle2CheckTypeValueName))
			{
				reportModel.Title2 = reportModel.Title2 + "_" + ReportTitle2CheckTypeValueName;
			}
			reportModel.Title3 = context.MOrgName;
			string text = Convert.ToString(filter.MStartPeroid);
			text = ((text.Length <= 5) ? (text.Substring(0, text.Length - 1) + "-" + text.Substring(4, 1)) : (text.Substring(0, text.Length - 2) + "-" + text.Substring(4, 2)));
			string text2 = Convert.ToString(filter.MEndPeroid);
			text2 = ((text2.Length <= 5) ? (text2.Substring(0, text2.Length - 1) + "-" + text2.Substring(4, 1)) : (text2.Substring(0, text2.Length - 2) + "-" + text2.Substring(4, 2)));
			reportModel.Title4 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ForThePeriod", "For the period ") + " " + text + " " + COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "To", "to") + " " + text2;
		}

		private void SetRowHead(BizReportModel model, RPTSubsidiaryLedgerFilterModel filter, MContext context)
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

		private void SetRowData(BizReportModel model, RPTSubsidiaryLedgerFilterModel filter, MContext context)
		{
			if (DataList != null && DataList.Count() != 0)
			{
				IEnumerable<IGrouping<string, RPTSubsidiaryLedgerModel>> enumerable = from x in DataList
				group x by x.GroupValue;
				List<RPTSubsidiaryLedgerModel> list = new List<RPTSubsidiaryLedgerModel>();
				foreach (IGrouping<string, RPTSubsidiaryLedgerModel> item in enumerable)
				{
					List<RPTSubsidiaryLedgerModel> list2 = item.ToList();
					string key = item.Key;
					bool flag = !filter.MDisplayNoAccurrenceAmount && CheckIsZoreData(context, list2, 0);
					bool flag2 = !filter.MDisplayZeorEndBalance && CheckIsZoreData(context, list2, 1);
					if ((flag || flag2) && !list2.Exists((RPTSubsidiaryLedgerModel x) => x.AmountType == 3))
					{
						list.AddRange(list2);
					}
				}
				if (list.Count() > 0)
				{
					list.ForEach(delegate(RPTSubsidiaryLedgerModel x)
					{
						DataList.Remove(x);
					});
				}
				foreach (RPTSubsidiaryLedgerModel data in DataList)
				{
					BalanceBizReportRowModel balanceBizReportRowModel = ToBizReportRowModel(context, data, filter);
					if (balanceBizReportRowModel != null)
					{
						model.AddRow(balanceBizReportRowModel);
					}
				}
			}
		}

		private bool CheckIsZoreData(MContext ctx, List<RPTSubsidiaryLedgerModel> list, int filterType)
		{
			bool result = true;
			RPTSubsidiaryLedgerModel rPTSubsidiaryLedgerModel = list.FirstOrDefault((RPTSubsidiaryLedgerModel x) => x.AmountType == 1);
			if (rPTSubsidiaryLedgerModel == null)
			{
				return result;
			}
			decimal d = (filterType == 0) ? (Math.Abs(rPTSubsidiaryLedgerModel.MDebit) + Math.Abs(rPTSubsidiaryLedgerModel.MCredit)) : Math.Abs(rPTSubsidiaryLedgerModel.MBalance);
			if (d > decimal.Zero)
			{
				result = false;
			}
			return result;
		}

		private BalanceBizReportRowModel ToBizReportRowModel(MContext ctx, RPTSubsidiaryLedgerModel item, RPTSubsidiaryLedgerFilterModel filter)
		{
			BalanceBizReportRowModel balanceBizReportRowModel = new BalanceBizReportRowModel();
			balanceBizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = item.MDate.ToString("yyyy-MM-dd"),
				CellType = BizReportCellType.Text
			});
			string value = string.IsNullOrWhiteSpace(item.MVoucherMNumber) ? "" : ("GL-" + item.MVoucherMNumber);
			balanceBizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = value,
				CellType = BizReportCellType.Text,
				CellLink = GetCellLink(ctx, item)
			});
			if (filter.IncludeCheckType)
			{
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.MAccountName,
					CellType = BizReportCellType.Text
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.MCheckTypeName,
					CellType = BizReportCellType.Text
				});
			}
			balanceBizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = item.MSummary,
				CellType = BizReportCellType.Text
			});
			decimal d = default(decimal);
			if (IncludeForeginCurrency)
			{
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MDebitFor, ""),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MDebit, ""),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MCreditFor, ""),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MCredit, ""),
					CellType = BizReportCellType.Money
				});
				d += Math.Abs(item.MDebitFor);
				d += Math.Abs(item.MDebit);
				d += Math.Abs(item.MCreditFor);
				d += Math.Abs(item.MCredit);
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.MDC,
					CellType = BizReportCellType.Text
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MBalanceFor, "0"),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MBalance, "0"),
					CellType = BizReportCellType.Money
				});
				d += Math.Abs(item.MBalanceFor);
				d += Math.Abs(item.MBalance);
			}
			else
			{
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MDebit, ""),
					CellType = BizReportCellType.Money
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MCredit, ""),
					CellType = BizReportCellType.Money
				});
				d += Math.Abs(item.MDebit);
				d += Math.Abs(item.MCredit);
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.MDC,
					CellType = BizReportCellType.Text
				});
				balanceBizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(item.MBalance, "0"),
					CellType = BizReportCellType.Money
				});
				d += Math.Abs(item.MBalance);
			}
			balanceBizReportRowModel.TotalValue = Math.Abs(item.MBalance);
			balanceBizReportRowModel.CurrentTotalAmount = Math.Abs(item.MDebit) + Math.Abs(item.MCredit);
			return balanceBizReportRowModel;
		}

		private BizReportCellLinkModel GetCellLink(MContext ctx, RPTSubsidiaryLedgerModel rptModel)
		{
			BizReportCellLinkModel bizReportCellLinkModel = new BizReportCellLinkModel();
			bizReportCellLinkModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewVoucher", "View Voucher");
			bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewVoucher", "View Voucher");
			bizReportCellLinkModel.Url = $"GL/GLVoucher/GLVoucherEdit?MItemID={rptModel.MVoucherID}";
			return bizReportCellLinkModel;
		}

		private List<RPTSubsidiaryLedgerModel> GetSubsidiaryLedgerModels(MContext ctx, List<GLBalanceModel> balanceList, BDAccountModel accountModel, RPTSubsidiaryLedgerFilterModel filter)
		{
			List<RPTSubsidiaryLedgerModel> list = new List<RPTSubsidiaryLedgerModel>();
			if (!filter.IncludeCheckType)
			{
				IOrderedEnumerable<IGrouping<int, GLBalanceModel>> orderedEnumerable = from x in balanceList
				group x by x.MYearPeriod into x
				orderby x.Key
				select x;
				bool needInitLine = true;
				foreach (IGrouping<int, GLBalanceModel> item in orderedEnumerable)
				{
					List<GLBalanceModel> list2 = item.ToList();
					if (list2 != null && list2.Count() != 0)
					{
						List<RPTSubsidiaryLedgerModel> list3 = ToRptSubsidiaryLedgerModel(ctx, list2, accountModel, item.Key, needInitLine, filter);
						if (list3.Count() != 0)
						{
							needInitLine = false;
							list.AddRange(list3);
						}
					}
				}
			}
			else
			{
				List<IGrouping<string, GLBalanceModel>> list4 = (from x in balanceList
				group x by x.MCheckGroupValueID).ToList();
				foreach (IGrouping<string, GLBalanceModel> item2 in list4)
				{
					List<GLBalanceModel> list5 = item2.ToList();
					if (list5 != null && list5.Count() != 0)
					{
						IOrderedEnumerable<IGrouping<int, GLBalanceModel>> orderedEnumerable2 = from x in list5
						group x by x.MYearPeriod into x
						orderby x.Key
						select x;
						bool needInitLine2 = true;
						foreach (IGrouping<int, GLBalanceModel> item3 in orderedEnumerable2)
						{
							List<GLBalanceModel> list6 = item3.ToList();
							if (list6 != null && list6.Count() != 0)
							{
								List<RPTSubsidiaryLedgerModel> list7 = ToRptSubsidiaryLedgerModel(ctx, list6, accountModel, item3.Key, needInitLine2, filter);
								if (list7.Count() != 0)
								{
									needInitLine2 = false;
									list.AddRange(list7);
								}
							}
						}
					}
				}
			}
			return list;
		}

		private List<RPTSubsidiaryLedgerModel> ToRptSubsidiaryLedgerModel(MContext ctx, List<GLBalanceModel> balanceList, BDAccountModel accountModel, int yearPeriod, bool needInitLine, RPTSubsidiaryLedgerFilterModel filter)
		{
			List<RPTSubsidiaryLedgerModel> list = new List<RPTSubsidiaryLedgerModel>();
			if (balanceList == null || balanceList.Count() == 0)
			{
				return list;
			}
			string mCheckGroupValueID = balanceList.First().MCheckGroupValueID;
			GLCheckGroupValueModel mCheckGroupValueModel = balanceList.First().MCheckGroupValueModel;
			string groupId = UUIDHelper.GetGuid();
			List<int> temp;
			for (int i = 0; i < 4; i++)
			{
				RPTSubsidiaryLedgerModel rPTSubsidiaryLedgerModel = new RPTSubsidiaryLedgerModel();
				rPTSubsidiaryLedgerModel.GroupValue = groupId;
				rPTSubsidiaryLedgerModel.MAccountID = accountModel.MItemID;
				rPTSubsidiaryLedgerModel.MAccountParentID = accountModel.MParentID;
				if (balanceList.Exists((GLBalanceModel x) => x.MCheckGroupValueID != "0") && i == 0)
				{
					rPTSubsidiaryLedgerModel.MAccountName = accountModel.MFullName;
					rPTSubsidiaryLedgerModel.MCheckTypeName = GetCheckTypeDisplayString(ctx, mCheckGroupValueModel);
					rPTSubsidiaryLedgerModel.MCheckTypeName = (string.IsNullOrWhiteSpace(rPTSubsidiaryLedgerModel.MCheckTypeName) ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "UnKownCheckTypeValue", "未指定") : rPTSubsidiaryLedgerModel.MCheckTypeName);
				}
				rPTSubsidiaryLedgerModel.MNumberID = accountModel.MNumber;
				rPTSubsidiaryLedgerModel.MPeriod = Convert.ToString(yearPeriod);
				switch (i)
				{
				case 0:
					if (needInitLine)
					{
						rPTSubsidiaryLedgerModel.MSummary = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "InitialBalance", "Initial balance");
						if (accountModel.MDC == 1)
						{
							rPTSubsidiaryLedgerModel.MDebit = balanceList.Sum((GLBalanceModel x) => x.MBeginBalance);
							rPTSubsidiaryLedgerModel.MDebitFor = balanceList.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
							rPTSubsidiaryLedgerModel.MDC = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Borrow", "Borrow");
						}
						else
						{
							rPTSubsidiaryLedgerModel.MCredit = balanceList.Sum((GLBalanceModel x) => x.MBeginBalance);
							rPTSubsidiaryLedgerModel.MCreditFor = balanceList.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
							rPTSubsidiaryLedgerModel.MDC = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Loan", "Loan");
						}
						rPTSubsidiaryLedgerModel.MBalance = balanceList.Sum((GLBalanceModel x) => x.MBeginBalance);
						rPTSubsidiaryLedgerModel.MBalanceFor = balanceList.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
						rPTSubsidiaryLedgerModel.MDate = AccountHelper.GetDateTimeByYearPeriod(rPTSubsidiaryLedgerModel.MPeriod, 0);
						rPTSubsidiaryLedgerModel.MDC = GLReportHelper.GetBalanceDirection(ctx, accountModel.MDC, rPTSubsidiaryLedgerModel.MBalance);
						if (balanceList.Sum((GLBalanceModel x) => x.MCredit) > decimal.Zero || balanceList.Sum((GLBalanceModel x) => x.MDebit) > decimal.Zero || balanceList.Sum((GLBalanceModel x) => x.MEndBalance) > decimal.Zero)
						{
							rPTSubsidiaryLedgerModel.IsShowInitBalance = true;
						}
						rPTSubsidiaryLedgerModel.AmountType = 0;
						list.Add(rPTSubsidiaryLedgerModel);
					}
					break;
				case 1:
				{
					temp = AccountHelper.GetYearPeriod(Convert.ToString(yearPeriod));
					List<GLBalanceModel> list2 = (from x in balanceList
					where x.MYear == temp[0] && x.MPeriod == temp[1] && x.MAccountID == accountModel.MItemID
					select x).ToList();
					List<string> list3 = new List<string>();
					List<RPTSubsidiaryLedgerModel> list4 = new List<RPTSubsidiaryLedgerModel>();
					foreach (GLBalanceModel item in list2)
					{
						if (!list3.Contains(item.MAccountID))
						{
							List<RPTSubsidiaryLedgerModel> vochuerRptItems = GetVochuerRptItems(ctx, temp[0], temp[1], accountModel, item, list2.Sum((GLBalanceModel x) => x.MBeginBalance), list2.Sum((GLBalanceModel x) => x.MBeginBalanceFor), filter.MCurrencyID);
							list4.AddRange(vochuerRptItems);
							list3.Add(item.MAccountID);
						}
					}
					if (list4.Count() > 0)
					{
						list4 = (from x in list4
						orderby x.MDate
						select x).ToList();
						list4.ForEach(delegate(RPTSubsidiaryLedgerModel x)
						{
							x.GroupValue = groupId;
							x.AmountType = 3;
						});
						list.AddRange(list4);
					}
					break;
				}
				case 2:
					rPTSubsidiaryLedgerModel.MSummary = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "TotalOfThisPeriod", "Total of this period");
					rPTSubsidiaryLedgerModel.MDebit = balanceList.Sum((GLBalanceModel x) => x.MDebit);
					rPTSubsidiaryLedgerModel.MDebitFor = balanceList.Sum((GLBalanceModel x) => x.MDebitFor);
					rPTSubsidiaryLedgerModel.MCredit = balanceList.Sum((GLBalanceModel x) => x.MCredit);
					rPTSubsidiaryLedgerModel.MCreditFor = balanceList.Sum((GLBalanceModel x) => x.MCreditFor);
					rPTSubsidiaryLedgerModel.AmountType = 1;
					goto IL_0781;
				default:
					{
						rPTSubsidiaryLedgerModel.MSummary = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "AccumulativeTotalOfThisYear", "Accumulative total of this year");
						rPTSubsidiaryLedgerModel.MDebit = balanceList.Sum((GLBalanceModel x) => x.MYtdDebit);
						rPTSubsidiaryLedgerModel.MDebitFor = balanceList.Sum((GLBalanceModel x) => x.MYtdDebitFor);
						rPTSubsidiaryLedgerModel.MCredit = balanceList.Sum((GLBalanceModel x) => x.MYtdCredit);
						rPTSubsidiaryLedgerModel.MCreditFor = balanceList.Sum((GLBalanceModel x) => x.MYtdCreditFor);
						rPTSubsidiaryLedgerModel.AmountType = 2;
						goto IL_0781;
					}
					IL_0781:
					if (Convert.ToString(yearPeriod) == filter.MEndPeroid)
					{
						if (accountModel.MDC == 1)
						{
							rPTSubsidiaryLedgerModel.MBalance = balanceList.Sum((GLBalanceModel x) => x.MBeginBalance) + balanceList.Sum((GLBalanceModel x) => x.MDebit) - balanceList.Sum((GLBalanceModel x) => x.MCredit);
							rPTSubsidiaryLedgerModel.MBalanceFor = balanceList.Sum((GLBalanceModel x) => x.MBeginBalanceFor) + balanceList.Sum((GLBalanceModel x) => x.MDebitFor) - balanceList.Sum((GLBalanceModel x) => x.MCreditFor);
						}
						else
						{
							rPTSubsidiaryLedgerModel.MBalance = balanceList.Sum((GLBalanceModel x) => x.MBeginBalance) + balanceList.Sum((GLBalanceModel x) => x.MCredit) - balanceList.Sum((GLBalanceModel x) => x.MDebit);
							rPTSubsidiaryLedgerModel.MBalanceFor = balanceList.Sum((GLBalanceModel x) => x.MBeginBalanceFor) + balanceList.Sum((GLBalanceModel x) => x.MCreditFor) - balanceList.Sum((GLBalanceModel x) => x.MDebitFor);
						}
					}
					else
					{
						rPTSubsidiaryLedgerModel.MBalance = balanceList.Sum((GLBalanceModel x) => x.MEndBalance);
						rPTSubsidiaryLedgerModel.MBalanceFor = balanceList.Sum((GLBalanceModel x) => x.MEndBalanceFor);
					}
					rPTSubsidiaryLedgerModel.MDate = AccountHelper.GetDateTimeByYearPeriod(rPTSubsidiaryLedgerModel.MPeriod, 1);
					rPTSubsidiaryLedgerModel.MDC = GLReportHelper.GetBalanceDirection(ctx, accountModel.MDC, rPTSubsidiaryLedgerModel.MBalance);
					list.Add(rPTSubsidiaryLedgerModel);
					break;
				}
			}
			return list;
		}

		private List<RPTSubsidiaryLedgerModel> GetVochuerRptItems(MContext ctx, int year, int period, BDAccountModel account, GLBalanceModel balance, decimal balanceTotal, decimal balanceForTotal, string currencyId = null)
		{
			List<RPTSubsidiaryLedgerModel> list = new List<RPTSubsidiaryLedgerModel>();
			List<GLVoucherModel> source = (from x in VoucherList
			where x.MYear == year && x.MPeriod == period
			select x).ToList();
			source = (from x in source
			orderby x.MDate, int.Parse(x.MNumber)
			select x).ToList();
			decimal num = default(decimal);
			decimal num2 = default(decimal);
			foreach (GLVoucherModel item in source)
			{
				List<GLVoucherEntryModel> list2 = item.MVoucherEntrys;
				if (list2 != null)
				{
					if (balance.MCheckGroupValueID != "0")
					{
						list2 = (from x in list2
						where x.MCheckGroupValueID == balance.MCheckGroupValueID
						select x).ToList();
					}
					if (list2 != null && list2.Count() != 0)
					{
						List<BDAccountModel> accoutList = AccoutList;
						List<BDAccountModel> childrenByRecursion = AccountDal.GetChildrenByRecursion(accoutList, account);
						foreach (BDAccountModel item2 in childrenByRecursion)
						{
							GLVoucherEntryModel gLVoucherEntryModel = null;
							List<GLVoucherEntryModel> list3 = new List<GLVoucherEntryModel>();
							if (string.IsNullOrWhiteSpace(currencyId) || currencyId == "0")
							{
								list3 = (from x in list2
								where x.MAccountID == item2.MItemID
								select x).ToList();
								gLVoucherEntryModel = (from x in list2
								where x.MAccountID == item2.MItemID
								select x).FirstOrDefault();
							}
							else
							{
								list3 = (from x in list2
								where x.MAccountID == item2.MItemID && x.MCurrencyID == currencyId
								select x).ToList();
								gLVoucherEntryModel = (from x in list2
								where x.MAccountID == item2.MItemID && x.MCurrencyID == currencyId
								select x).FirstOrDefault();
							}
							if (list3 != null && list3.Count() > 0)
							{
								foreach (GLVoucherEntryModel item3 in list3)
								{
									list.Add(ToRptSubsidiaryLedgerModel(ctx, item, item3, account, balance, balanceTotal, balanceForTotal, out num, out num2));
									balanceForTotal = num2;
									balanceTotal = num;
								}
							}
						}
					}
				}
			}
			return list;
		}

		private RPTSubsidiaryLedgerModel ToRptSubsidiaryLedgerModel(MContext ctx, GLVoucherModel voucherModel, GLVoucherEntryModel voucherEntryModel, BDAccountModel account, GLBalanceModel balance, decimal balanceTotal, decimal balanceForTotal, out decimal endBalnce, out decimal endBalanceFor)
		{
			endBalnce = default(decimal);
			endBalanceFor = default(decimal);
			RPTSubsidiaryLedgerModel rPTSubsidiaryLedgerModel = new RPTSubsidiaryLedgerModel();
			rPTSubsidiaryLedgerModel.MDate = voucherModel.MDate;
			rPTSubsidiaryLedgerModel.MVoucherID = voucherModel.MItemID;
			rPTSubsidiaryLedgerModel.MVoucherMNumber = voucherModel.MNumber;
			rPTSubsidiaryLedgerModel.MSummary = voucherEntryModel.MExplanation;
			if (voucherEntryModel.MDebit != decimal.Zero)
			{
				rPTSubsidiaryLedgerModel.MDebit = voucherEntryModel.MAmount;
				rPTSubsidiaryLedgerModel.MDebitFor = voucherEntryModel.MAmountFor;
			}
			else
			{
				rPTSubsidiaryLedgerModel.MCredit = voucherEntryModel.MAmount;
				rPTSubsidiaryLedgerModel.MCreditFor = voucherEntryModel.MAmountFor;
			}
			if (account.MDC == 1)
			{
				rPTSubsidiaryLedgerModel.MBalance = balanceTotal + rPTSubsidiaryLedgerModel.MDebit - rPTSubsidiaryLedgerModel.MCredit;
				rPTSubsidiaryLedgerModel.MBalanceFor = balanceForTotal + rPTSubsidiaryLedgerModel.MDebitFor - rPTSubsidiaryLedgerModel.MCreditFor;
			}
			else
			{
				rPTSubsidiaryLedgerModel.MBalance = balanceTotal + rPTSubsidiaryLedgerModel.MCredit - rPTSubsidiaryLedgerModel.MDebit;
				rPTSubsidiaryLedgerModel.MBalanceFor = balanceForTotal + rPTSubsidiaryLedgerModel.MCreditFor - rPTSubsidiaryLedgerModel.MDebitFor;
			}
			endBalnce = rPTSubsidiaryLedgerModel.MBalance;
			endBalanceFor = rPTSubsidiaryLedgerModel.MBalanceFor;
			rPTSubsidiaryLedgerModel.MDC = GLReportHelper.GetBalanceDirection(ctx, account.MDC, endBalnce);
			return rPTSubsidiaryLedgerModel;
		}

		private string GetCheckTypeDisplayString(MContext ctx, GLCheckGroupValueModel checkGroupValue)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (checkGroupValue == null)
			{
				return "";
			}
			GLUtility gLUtility = new GLUtility();
			if (!string.IsNullOrWhiteSpace(checkGroupValue.MContactName))
			{
				bool flag = CheckTypeDic.Keys.Contains(0);
				int num = 0;
				string text = flag ? CheckTypeDic[num] : gLUtility.GetCheckTypeName(ctx, num);
				if (!flag)
				{
					CheckTypeDic.Add(num, text);
				}
				stringBuilder.AppendFormat("{0}:{1} ", text, checkGroupValue.MContactName);
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValue.MEmployeeName))
			{
				bool flag2 = CheckTypeDic.Keys.Contains(1);
				int num2 = 1;
				string text2 = flag2 ? CheckTypeDic[num2] : gLUtility.GetCheckTypeName(ctx, num2);
				if (!flag2)
				{
					CheckTypeDic.Add(num2, text2);
				}
				stringBuilder.AppendFormat("{0}:{1} ", text2, checkGroupValue.MEmployeeName);
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValue.MExpItemID))
			{
				bool flag3 = CheckTypeDic.Keys.Contains(3);
				int num3 = 3;
				string text3 = flag3 ? CheckTypeDic[num3] : gLUtility.GetCheckTypeName(ctx, num3);
				if (!flag3)
				{
					CheckTypeDic.Add(num3, text3);
				}
				stringBuilder.AppendFormat("{0}:{1} ", text3, checkGroupValue.MExpItemName);
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValue.MMerItemName))
			{
				bool flag4 = CheckTypeDic.Keys.Contains(2);
				int num4 = 2;
				string text4 = flag4 ? CheckTypeDic[num4] : gLUtility.GetCheckTypeName(ctx, num4);
				if (!flag4)
				{
					CheckTypeDic.Add(num4, text4);
				}
				stringBuilder.AppendFormat("{0}:{1} ", text4, checkGroupValue.MMerItemName);
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValue.MPaItemGroupName) || !string.IsNullOrWhiteSpace(checkGroupValue.MPaItemName))
			{
				bool flag5 = CheckTypeDic.Keys.Contains(4);
				int num5 = 4;
				string text5 = flag5 ? CheckTypeDic[num5] : gLUtility.GetCheckTypeName(ctx, num5);
				if (!flag5)
				{
					CheckTypeDic.Add(num5, text5);
				}
				string arg = (!string.IsNullOrWhiteSpace(checkGroupValue.MPaItemGroupName)) ? checkGroupValue.MPaItemGroupName : checkGroupValue.MPaItemName;
				stringBuilder.AppendFormat("{0}:{1} ", text5, arg);
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValue.MTrackItem1))
			{
				bool flag6 = CheckTypeDic.Keys.Contains(5);
				int num6 = 5;
				string text6 = flag6 ? CheckTypeDic[num6] : gLUtility.GetCheckTypeName(ctx, num6);
				if (!flag6)
				{
					CheckTypeDic.Add(num6, text6);
				}
				stringBuilder.AppendFormat("{0}:{1} ", text6, checkGroupValue.MTrackItem1Name);
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValue.MTrackItem2))
			{
				bool flag7 = CheckTypeDic.Keys.Contains(6);
				int num7 = 6;
				string text7 = flag7 ? CheckTypeDic[num7] : gLUtility.GetCheckTypeName(ctx, num7);
				if (!flag7)
				{
					CheckTypeDic.Add(num7, text7);
				}
				stringBuilder.AppendFormat("{0}:{1} ", text7, checkGroupValue.MTrackItem2Name);
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValue.MTrackItem3))
			{
				bool flag8 = CheckTypeDic.Keys.Contains(7);
				int num8 = 7;
				string text8 = flag8 ? CheckTypeDic[num8] : gLUtility.GetCheckTypeName(ctx, num8);
				if (!flag8)
				{
					CheckTypeDic.Add(num8, text8);
				}
				stringBuilder.AppendFormat("{0}:{1} ", text8, checkGroupValue.MTrackItem3Name);
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValue.MTrackItem4))
			{
				bool flag9 = CheckTypeDic.Keys.Contains(8);
				int num9 = 8;
				string text9 = flag9 ? CheckTypeDic[num9] : gLUtility.GetCheckTypeName(ctx, num9);
				if (!flag9)
				{
					CheckTypeDic.Add(num9, text9);
				}
				stringBuilder.AppendFormat("{0}:{1} ", text9, checkGroupValue.MTrackItem4Name);
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValue.MTrackItem5))
			{
				bool flag10 = CheckTypeDic.Keys.Contains(9);
				int num10 = 9;
				string text10 = flag10 ? CheckTypeDic[num10] : gLUtility.GetCheckTypeName(ctx, num10);
				if (!flag10)
				{
					CheckTypeDic.Add(num10, text10);
				}
				stringBuilder.AppendFormat("{0}:{1} ", text10, checkGroupValue.MTrackItem5Name);
			}
			return stringBuilder.ToString();
		}

		private void SetDataSummary(BizReportModel model, RPTSubsidiaryLedgerFilterModel filter, MContext context)
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
				Value = "",
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total"),
				CellType = BizReportCellType.Text
			});
			decimal d = default(decimal);
			List<RPTSubsidiaryLedgerModel> source = (from x in DataList
			where x.MAccountParentID == "0"
			select x).ToList();
			if (IncludeForeginCurrency)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(source.Sum((RPTSubsidiaryLedgerModel x) => x.MDebitFor), ""),
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(source.Sum((RPTSubsidiaryLedgerModel x) => x.MDebit), ""),
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(source.Sum((RPTSubsidiaryLedgerModel x) => x.MCreditFor), ""),
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(source.Sum((RPTSubsidiaryLedgerModel x) => x.MCredit), ""),
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = "",
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(source.Sum((RPTSubsidiaryLedgerModel x) => x.MBalanceFor), ""),
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(source.Sum((RPTSubsidiaryLedgerModel x) => x.MBalance), ""),
					CellType = BizReportCellType.Money
				});
			}
			else
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(source.Sum((RPTSubsidiaryLedgerModel x) => x.MDebit), ""),
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(source.Sum((RPTSubsidiaryLedgerModel x) => x.MCredit), ""),
					CellType = BizReportCellType.Money
				});
				d += source.Sum((RPTSubsidiaryLedgerModel x) => x.MDebit);
				d += source.Sum((RPTSubsidiaryLedgerModel x) => x.MCredit);
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = "",
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = AccountHelper.MoneyToString(source.Sum((RPTSubsidiaryLedgerModel x) => x.MBalance), ""),
					CellType = BizReportCellType.Money
				});
			}
			model.AddRow(bizReportRowModel);
		}

		public List<BizReportModel> GetBatchReportList(MContext ctx, RPTSubsidiaryLedgerFilterModel filter)
		{
			IsNavTo = !string.IsNullOrWhiteSpace(filter.NavReportID);
			if (IsNavTo)
			{
				string mReportID = filter.MReportID;
				string mAccountID = filter.MAccountID;
				filter = GLReportHelper.GetReportBaseFilterByReportID<RPTSubsidiaryLedgerFilterModel>(ctx, filter.NavReportID);
				filter.MStartAccountID = mAccountID;
				filter.MEndAccountID = mAccountID;
				filter.MReportID = mReportID;
			}
			if (string.IsNullOrEmpty(filter.MBaseCurrencyID))
			{
				filter.MBaseCurrencyID = ctx.MBasCurrencyID;
			}
			IncludeForeginCurrency = (!string.IsNullOrEmpty(filter.MCurrencyID) && filter.MCurrencyID != "0" && filter.MCurrencyID != filter.MBaseCurrencyID);
			CirculationCount = AccountHelper.GetCirculationCount(Convert.ToInt32(filter.MStartPeroid), Convert.ToInt32(filter.MEndPeroid));
			List<BDAccountModel> list = AccoutList = AccountDal.GetBaseBDAccountList(ctx, null, false, null);
			List<string> list2 = new List<string>();
			if (filter.AccountIDNameList != null && filter.AccountIDNameList.Count() > 0)
			{
				list2 = filter.AccountIDNameList.Keys.ToList();
			}
			else if (!string.IsNullOrWhiteSpace(filter.MAccountIDs))
			{
				list2 = filter.MAccountIDs.Split(',').ToList();
			}
			if (list2 == null || list2.Count() == 0)
			{
				return new List<BizReportModel>();
			}
			list2 = (filter.AccountIdList = list2.Distinct().ToList());
			filter.MAccountID = "";
			List<GLBalanceModel> balanceList = GLReportHelper.GetBalanceList(ctx, filter, false, !IsNavTo, null);
			balanceList = (from x in balanceList
			where x.MPeriod != 0
			select x).ToList();
			List<string> list4 = new List<string>();
			foreach (string item in list2)
			{
				BDAccountModel bDAccountModel = (from x in list
				where x.MItemID == item
				select x).FirstOrDefault();
				if (bDAccountModel != null)
				{
					List<string> list5 = (from x in BDAccountHelper.GetChildrenAccountByRecursion(bDAccountModel, list, true)
					select x.MItemID).ToList();
					if (list5 != null)
					{
						list4.AddRange(list5);
					}
				}
			}
			if (string.IsNullOrWhiteSpace(filter.MAccountID))
			{
				filter.MAccountID = list.First().MItemID;
			}
			filter.ChildrenAccountList = list4;
			filter.AccountIdList = list4;
			List<GLVoucherModel> voucherList = GetVoucherList(ctx, filter);
			if (balanceList != null && balanceList.Count() > 0 && (voucherList == null || voucherList.Count() == 0))
			{
				balanceList = (from x in balanceList
				where Math.Abs(x.MBeginBalance) + Math.Abs(x.MCredit) + Math.Abs(x.MDebit) + Math.Abs(x.MYtdCredit) + Math.Abs(x.MYtdDebit) + Math.Abs(x.MEndBalance) != decimal.Zero
				select x).ToList();
			}
			List<BizReportModel> list6 = new List<BizReportModel>();
			foreach (string item2 in list2)
			{
				BDAccountModel tempAccount = (from x in list
				where x.MItemID == item2
				select x).FirstOrDefault();
				if (tempAccount != null)
				{
					List<string> tempChildrenAccountList = (from x in BDAccountHelper.GetChildrenAccountByRecursion(tempAccount, list, true)
					select x.MItemID).ToList();
					RPTSubsidiaryLedgerFilterModel rPTSubsidiaryLedgerFilterModel = filter;
					rPTSubsidiaryLedgerFilterModel.MAccountID = tempAccount.MItemID;
					rPTSubsidiaryLedgerFilterModel.ChildrenAccountList = tempChildrenAccountList;
					List<GLBalanceModel> balanceList2 = (from x in balanceList
					where x.MAccountID == tempAccount.MItemID
					select x).ToList();
					List<GLVoucherModel> voucherList2 = (from x in voucherList
					where x.MVoucherEntrys.Exists((GLVoucherEntryModel y) => tempChildrenAccountList.Contains(y.MAccountID))
					select x).ToList();
					BizReportModel reportModel = GetReportModel(ctx, rPTSubsidiaryLedgerFilterModel, tempAccount, balanceList2, voucherList2);
					if (reportModel != null && reportModel.Rows != null && reportModel.Rows.Count() > 1)
					{
						list6.Add(reportModel);
					}
				}
			}
			return list6;
		}
	}
}
