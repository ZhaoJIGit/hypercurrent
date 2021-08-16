using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Common.Utility;
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
    public class RPTAccountBalanceBusiness : IRPTAccountBalanceBusiness, IRPTBizReportBusiness<RPTAccountBalanceFilterModel>
    {
        private BDAccountRepository AccountDal = new BDAccountRepository();

        private GLBalanceRepository BalanceDal = new GLBalanceRepository();

        private Dictionary<string, List<string>> DataColumns;

        private List<RPTAccountBalanceModel> DataList;

        private List<RPTAccountBalanceModel> CheckTypeDataList = new List<RPTAccountBalanceModel>();

        private List<RPTAccountBalanceModel> SummaryDataList = new List<RPTAccountBalanceModel>();

        private List<BDAccountModel> AccountList;

        private List<GLBalanceModel> BalanceList;

        private List<GLVoucherModel> VoucherList;

        private Dictionary<string, int> AccountLevels;

        private bool IncludeForeginCurrency = false;

        public string GetBizReportJson(MContext ctx, RPTAccountBalanceFilterModel filter)
        {
            IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
            if (string.IsNullOrEmpty(filter.MBaseCurrencyID))
            {
                filter.MBaseCurrencyID = ctx.MBasCurrencyID;
            }
            IncludeForeginCurrency = (!string.IsNullOrEmpty(filter.MCurrencyID) && filter.MCurrencyID != "0" && filter.MCurrencyID != filter.MBaseCurrencyID);
            return iRPTReportBusiness.GetBizReportJson(ctx, filter, delegate
            {
                AccountList = GLReportHelper.GetAccountList(ctx, filter);
                BalanceList = GLReportHelper.GetBalanceList(ctx, filter, true, true, AccountList);
                AccountLevels = GLReportHelper.GetAccountLevels(ctx, filter, AccountList);
                if (!filter.MDisplayNoAccurrenceAmount)
                {
                    VoucherList = GetVoucherList(ctx, filter);
                }
                return GetBalanceReportModel(ctx, filter);
            });
        }

        private List<GLVoucherModel> GetVoucherList(MContext ctx, RPTAccountBalanceFilterModel filter)
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

        public BizReportModel GetBalanceReportModel(MContext ctx, RPTAccountBalanceFilterModel filter)
        {
            BizReportModel bizReportModel = new BizReportModel();
            bizReportModel.Type = Convert.ToInt32(BizReportType.AccountBalance);
            SetCommonData(ctx, filter);
            SetDataColumns(ctx, filter);
            SetTitle(bizReportModel, filter, ctx);
            SetRowHead(bizReportModel, filter, ctx);
            SetRowData(bizReportModel, filter, ctx);
            SetDataSummary(bizReportModel, filter, ctx);
            return bizReportModel;
        }

        private void SetCommonData(MContext context, RPTAccountBalanceFilterModel filter)
        {
            DataList = new List<RPTAccountBalanceModel>();
            List<BDAccountModel> list = AccountList;
            List<string> accountIdListInBalance = (from x in BalanceList
                                                   select x.MAccountID).ToList();
            if (accountIdListInBalance != null && accountIdListInBalance.Count() > 0)
            {
                list = (from x in AccountList
                        where accountIdListInBalance.Contains(x.MItemID)
                        select x).ToList();
            }
            if (filter.AccountIdList != null && filter.AccountIdList.Count() > 0)
            {
                list = (from x in AccountList
                        where filter.AccountIdList.Contains(x.MItemID)
                        select x).ToList();
            }
            foreach (BDAccountModel item in list)
            {
                List<GLBalanceModel> balanceList = (from x in BalanceList
                                                    where x.MAccountID == item.MItemID && x.MCheckGroupValueID == "0"
                                                    select x).ToList();
                RPTAccountBalanceModel rPTAccountBalanceModel = ToRptAccountBalanceModel(context, balanceList, item, false);
                if (rPTAccountBalanceModel != null)
                {
                    rPTAccountBalanceModel.IsParentAccount = AccountList.Any((BDAccountModel x) => !string.IsNullOrWhiteSpace(x.MParentID) && x.MParentID == item.MItemID);
                    DataList.Add(rPTAccountBalanceModel);
                }
                bool flag = AccountList.Exists((BDAccountModel x) => x.MParentID == item.MItemID);
                if (filter.IncludeCheckType && !flag && item.MCheckGroupID != "0")
                {
                    List<GLBalanceModel> list2 = (from x in BalanceList
                                                  where x.MAccountID == item.MItemID && x.MCheckGroupValueID != "0"
                                                  select x).ToList();
                    if (list2 != null && list2.Count() > 0)
                    {
                        List<string> list3 = (from x in list2
                                              group x by x.MCheckGroupValueID into y
                                              select y.Key).ToList();
                        foreach (string item2 in list3)
                        {
                            List<GLBalanceModel> balanceList2 = (from x in list2
                                                                 where x.MCheckGroupValueID == item2
                                                                 select x).ToList();
                            RPTAccountBalanceModel rPTAccountBalanceModel2 = ToRptAccountBalanceModel(context, balanceList2, item, true);
                            rPTAccountBalanceModel2.IsParentAccount = false;
                            if (rPTAccountBalanceModel2 != null)
                            {
                                CheckTypeDataList.Add(rPTAccountBalanceModel2);
                            }
                        }
                    }
                }
            }
        }

        private void SetDataColumns(MContext ctx, RPTAccountBalanceFilterModel filter)
        {
            DataColumns = new Dictionary<string, List<string>>();
            string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountingCode", "科目代码");
            DataColumns.Add(text, null);
            string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountName", "Name");
            DataColumns.Add(text2, null);
            string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "InitBalance", "期初余额");
            List<string> childrenColumnList = GetChildrenColumnList(ctx, text3, IncludeForeginCurrency);
            DataColumns.Add(text3, childrenColumnList);
            string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CurrentPeriodBalance", "本期发生额");
            DataColumns.Add(text4, GetChildrenColumnList(ctx, text4, IncludeForeginCurrency));
            string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CumulativeBalance", "本年累计发生额");
            DataColumns.Add(text5, GetChildrenColumnList(ctx, text5, IncludeForeginCurrency));
            string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "EndBalance", "期末余额");
            DataColumns.Add(text6, GetChildrenColumnList(ctx, text6, IncludeForeginCurrency));
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

        private void SetTitle(BizReportModel reportModel, RPTAccountBalanceFilterModel filter, MContext context)
        {
            reportModel.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "AccountBalancesReport", "科目余额表");
            reportModel.Title2 = context.MOrgName;

            string text = "";
            if (!string.IsNullOrWhiteSpace(filter.MStartPeroid)) {
                text = Convert.ToString(filter.MStartPeroid);
                text = ((text.Length <= 5) ? (text.Substring(0, text.Length - 1) + "-" + text.Substring(4, 1)) : (text.Substring(0, text.Length - 2) + "-" + text.Substring(4, 2)));
            }
            string text2 = "";
            if (!string.IsNullOrWhiteSpace(filter.MEndPeroid)) {
                text2 = Convert.ToString(filter.MEndPeroid);
                text2 = ((text2.Length <= 5) ? (text2.Substring(0, text2.Length - 1) + "-" + text2.Substring(4, 1)) : (text2.Substring(0, text2.Length - 2) + "-" + text2.Substring(4, 2)));
            }
            reportModel.Title3 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ForThePeriod", "For the period ") + " " + text + " " + COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "To", "to") + " " + text2;

        }

        private void SetRowHead(BizReportModel model, RPTAccountBalanceFilterModel filter, MContext context)
        {
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    BalanceBizReportRowModel balanceBizReportRowModel = new BalanceBizReportRowModel();
                    balanceBizReportRowModel.RowType = BizReportRowType.Header;
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
                        balanceBizReportRowModel.AddCell(bizReportCellModel);
                    }
                    model.AddRow(balanceBizReportRowModel);
                }
                else
                {
                    BalanceBizReportRowModel balanceBizReportRowModel2 = new BalanceBizReportRowModel();
                    balanceBizReportRowModel2.RowType = BizReportRowType.Header;
                    foreach (KeyValuePair<string, List<string>> dataColumn2 in DataColumns)
                    {
                        if (dataColumn2.Value != null && dataColumn2.Value.Count() > 0)
                        {
                            foreach (string item in dataColumn2.Value)
                            {
                                BizReportCellModel model2 = new BizReportCellModel
                                {
                                    Value = item,
                                    CellType = BizReportCellType.Text
                                };
                                balanceBizReportRowModel2.AddCell(model2);
                            }
                        }
                    }
                    model.AddRow(balanceBizReportRowModel2);
                }
            }
        }

        private void SetRowData(BizReportModel model, RPTAccountBalanceFilterModel filter, MContext context)
        {
            if (DataList.Count() == 0 && CheckTypeDataList.Count() != 0)
            {
                CheckTypeDataList = BDAccountHelper.OrderBy(CheckTypeDataList);
                foreach (RPTAccountBalanceModel checkTypeData in CheckTypeDataList)
                {
                    BalanceBizReportRowModel balanceBizReportRowModel = ToBizReportSubRowModel(context, checkTypeData, filter);
                    balanceBizReportRowModel.RowType = BizReportRowType.Item;
                    bool flag = !filter.MDisplayNoAccurrenceAmount && JudgeTotalIsZore(balanceBizReportRowModel, 0, filter.IncludeCheckType);
                    bool flag2 = !filter.MDisplayZeorEndBalance && JudgeTotalIsZore(balanceBizReportRowModel, 1, filter.IncludeCheckType);
                    if (!flag && !flag2)
                    {
                        model.AddRow(balanceBizReportRowModel);
                        SummaryDataList.Add(checkTypeData);
                    }
                }
            }
            else
            {
                DataList = BDAccountHelper.OrderBy(DataList);
                List<BalanceBizReportRowModel> list = new List<BalanceBizReportRowModel>();
                List<RPTAccountBalanceModel> list2 = (from x in DataList
                                                      where x.MAccountParentID == "0"
                                                      select x).ToList();
                List<RPTAccountBalanceModel> list3 = (from x in DataList
                                                      where x.MAccountParentID != "0" && !string.IsNullOrEmpty(x.MAccountParentID)
                                                      select x).ToList();
                List<string> list4 = new List<string>();
                foreach (RPTAccountBalanceModel item in list2)
                {
                    BalanceBizReportRowModel balanceBizReportRowModel2 = ToBalanceBizReportRowModel(context, item, filter);
                    if (balanceBizReportRowModel2 != null)
                    {
                        balanceBizReportRowModel2.SubRows = new List<BizReportRowModel>();
                        List<BalanceBizReportRowModel> subRowModelByRecursion = GetSubRowModelByRecursion(item, list3, list4, context, filter);
                        if (subRowModelByRecursion != null)
                        {
                            balanceBizReportRowModel2.SubRows.AddRange(subRowModelByRecursion);
                        }
                        bool flag3 = !filter.MDisplayNoAccurrenceAmount && JudgeTotalIsZore(balanceBizReportRowModel2, 0, filter.IncludeCheckType);
                        bool flag4 = !filter.MDisplayZeorEndBalance && JudgeTotalIsZore(balanceBizReportRowModel2, 1, filter.IncludeCheckType);
                        if (!flag3 && !flag4)
                        {
                            for (int num = balanceBizReportRowModel2.SubRows.Count - 1; num >= 0; num--)
                            {
                                if (filter.AccountLevel > 0 && AccountLevels[((BalanceBizReportRowModel)balanceBizReportRowModel2.SubRows[num]).MAccountID] > filter.AccountLevel)
                                {
                                    balanceBizReportRowModel2.SubRows.RemoveAt(num);
                                }
                            }
                            list.Add(balanceBizReportRowModel2);
                            SummaryDataList.Add(item);
                        }
                    }
                }
                if (list3.Count() != 0)
                {
                    string text = string.Join(",", list4.ToArray());
                    foreach (RPTAccountBalanceModel item2 in list3)
                    {
                        if (list4.Count() == 0 || !list4.Contains(item2.MAccountID))
                        {
                            BalanceBizReportRowModel balanceBizReportRowModel3 = ToBalanceBizReportRowModel(context, item2, filter);
                            if (balanceBizReportRowModel3 != null && (filter.AccountLevel <= 0 || AccountLevels[balanceBizReportRowModel3.MAccountID] <= filter.AccountLevel))
                            {
                                balanceBizReportRowModel3.SubRows = new List<BizReportRowModel>();
                                List<BalanceBizReportRowModel> subRowModelByRecursion2 = GetSubRowModelByRecursion(item2, list3, list4, context, filter);
                                if (subRowModelByRecursion2 != null)
                                {
                                    balanceBizReportRowModel3.SubRows.AddRange(subRowModelByRecursion2);
                                }
                                bool flag5 = !filter.MDisplayNoAccurrenceAmount && JudgeTotalIsZore(balanceBizReportRowModel3, 0, filter.IncludeCheckType);
                                bool flag6 = !filter.MDisplayZeorEndBalance && JudgeTotalIsZore(balanceBizReportRowModel3, 1, filter.IncludeCheckType);
                                if (!flag5 && !flag6)
                                {
                                    list.Add(balanceBizReportRowModel3);
                                    SummaryDataList.Add(item2);
                                }
                            }
                        }
                    }
                }
                if (list.Count() > 0)
                {
                    SortRowList(list);
                    model.Rows.AddRange(list);
                }
            }
        }

        private void SortRowList(List<BalanceBizReportRowModel> rows)
        {
            rows.Sort(delegate (BalanceBizReportRowModel x, BalanceBizReportRowModel y)
            {
                string text = x.Cells[0].Value;
                string text2 = y.Cells[0].Value;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    text = GetAccountNumber4Order(text).Replace(".", "");
                }
                if (!string.IsNullOrWhiteSpace(text2))
                {
                    text2 = GetAccountNumber4Order(text2).Replace(".", "");
                }
                return text.CompareTo(text2);
            });
        }

        private bool JudgeTotalIsZore(BalanceBizReportRowModel row, int filterType, bool IncludeCheckType)
        {
            bool flag = true;
            if (row == null)
            {
                return flag;
            }
            decimal d = (filterType == 0) ? row.CurrentTotalAmount : row.TotalValue;
            if (d > decimal.Zero)
            {
                flag = false;
            }
            else
            {
                List<BizReportRowModel> subRows = row.SubRows;
                if (subRows == null || subRows.Count() == 0)
                {
                    if (VoucherList != null && filterType == 0)
                    {
                        flag = !VoucherList.Any((GLVoucherModel t) => t.MVoucherEntrys.Any((GLVoucherEntryModel et) => et.MAccountID == row.MAccountID && (!IncludeCheckType || et.MCheckGroupValueID == row.MCheckGroupValueID)));
                    }
                    return flag;
                }
                foreach (BalanceBizReportRowModel item in subRows)
                {
                    flag = (flag && JudgeTotalIsZore(item, filterType, IncludeCheckType));
                    if (!flag)
                    {
                        break;
                    }
                }
            }
            return flag;
        }

        private List<BalanceBizReportRowModel> GetSubRowModelByRecursion(RPTAccountBalanceModel parentModel, List<RPTAccountBalanceModel> rptModels, List<string> alreadyUseRptModelId, MContext ctx, RPTAccountBalanceFilterModel filter)
        {
            List<BalanceBizReportRowModel> list = new List<BalanceBizReportRowModel>();
            IEnumerable<RPTAccountBalanceModel> enumerable = from x in rptModels
                                                             where x.MAccountParentID == parentModel.MAccountID && x.MCheckGroupValueID == "0"
                                                             select x;
            if (enumerable == null || enumerable.Count() == 0)
            {
                if (filter.IncludeCheckType)
                {
                    List<RPTAccountBalanceModel> list2 = (from x in CheckTypeDataList
                                                          where x.MAccountID == parentModel.MAccountID
                                                          select x).ToList();
                    if (list2 != null && list2.Count() > 0)
                    {
                        foreach (RPTAccountBalanceModel item in list2)
                        {
                            if ((string.IsNullOrWhiteSpace(item.MCheckGroupValueID) && !(item.MCheckGroupValueID != "0")) || !string.IsNullOrWhiteSpace(item.MCheckGroupValueName))
                            {
                                BalanceBizReportRowModel balanceBizReportRowModel = new BalanceBizReportRowModel();
                                balanceBizReportRowModel = ToBizReportSubRowModel(ctx, item, filter);
                                bool flag = !filter.MDisplayNoAccurrenceAmount && JudgeTotalIsZore(balanceBizReportRowModel, 0, filter.IncludeCheckType);
                                bool flag2 = !filter.MDisplayZeorEndBalance && JudgeTotalIsZore(balanceBizReportRowModel, 1, filter.IncludeCheckType);
                                if (!flag && !flag2)
                                {
                                    list.Add(balanceBizReportRowModel);
                                }
                            }
                        }
                    }
                }
                alreadyUseRptModelId.Add(parentModel.MAccountID);
                return list;
            }
            foreach (RPTAccountBalanceModel item2 in enumerable)
            {
                BalanceBizReportRowModel balanceBizReportRowModel2 = new BalanceBizReportRowModel();
                balanceBizReportRowModel2 = ToBizReportSubRowModel(ctx, item2, filter);
                if (balanceBizReportRowModel2 != null)
                {
                    balanceBizReportRowModel2.SubRows.AddRange(GetSubRowModelByRecursion(item2, rptModels, alreadyUseRptModelId, ctx, filter));
                    bool flag3 = !filter.MDisplayNoAccurrenceAmount && JudgeTotalIsZore(balanceBizReportRowModel2, 0, filter.IncludeCheckType);
                    bool flag4 = !filter.MDisplayZeorEndBalance && JudgeTotalIsZore(balanceBizReportRowModel2, 1, filter.IncludeCheckType);
                    if (!flag3 && !flag4)
                    {
                        list.Add(balanceBizReportRowModel2);
                    }
                }
                alreadyUseRptModelId.Add(parentModel.MAccountID);
                alreadyUseRptModelId.Add(item2.MAccountID);
            }
            return list;
        }

        private BalanceBizReportRowModel ToBalanceBizReportRowModel(MContext ctx, RPTAccountBalanceModel item, RPTAccountBalanceFilterModel filter)
        {
            BalanceBizReportRowModel balanceBizReportRowModel = new BalanceBizReportRowModel();
            balanceBizReportRowModel.SubRows = new List<BizReportRowModel>();
            balanceBizReportRowModel.MAccountID = item.MAccountID;
            balanceBizReportRowModel.MCheckGroupValueID = item.MCheckGroupValueID;
            balanceBizReportRowModel.AddCell(new BizReportCellModel
            {
                Value = item.MNumberID,
                CellType = BizReportCellType.TreeText,
                CellLink = GetCellLink(ctx, item, filter)
            });
            balanceBizReportRowModel.AddCell(new BizReportCellModel
            {
                Value = item.MAccountName,
                CellType = BizReportCellType.Text,
                CellLink = GetCellLink(ctx, item, filter)
            });
            string defaultValue = "";
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
            }
            balanceBizReportRowModel.TotalValue = Math.Abs(item.MEndDebitBalance) + Math.Abs(item.MEndCreditBalance);
            balanceBizReportRowModel.CurrentTotalAmount = Math.Abs(item.MDebit) + Math.Abs(item.MCredit);
            return balanceBizReportRowModel;
        }

        private BizReportCellLinkModel GetCellLink(MContext ctx, RPTAccountBalanceModel rptModel, RPTAccountBalanceFilterModel filter)
        {
            BizReportCellLinkModel bizReportCellLinkModel = new BizReportCellLinkModel();
            string text3 = bizReportCellLinkModel.Title = (bizReportCellLinkModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "SubsidiaryLedger", "Subsidiary Ledger"));
            bizReportCellLinkModel.Url = "/Report/Report2/42?accountId=" + rptModel.MAccountID;
            bizReportCellLinkModel.DisabledEvent = true;
            return bizReportCellLinkModel;
        }

        private BalanceBizReportRowModel ToBizReportSubRowModel(MContext ctx, RPTAccountBalanceModel item, RPTAccountBalanceFilterModel filter)
        {
            BalanceBizReportRowModel balanceBizReportRowModel = new BalanceBizReportRowModel();
            balanceBizReportRowModel.RowType = BizReportRowType.SubItem;
            balanceBizReportRowModel.MAccountID = item.MAccountID;
            balanceBizReportRowModel.MCheckGroupValueID = item.MCheckGroupValueID;
            balanceBizReportRowModel.SubRows = new List<BizReportRowModel>();
            if (filter.IncludeCheckType && !string.IsNullOrEmpty(item.MCheckGroupValueName))
            {
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = "",
                    CellType = BizReportCellType.Text
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = item.MAccountName + "_" + item.MCheckGroupValueName,
                    CellType = BizReportCellType.TreeText
                });
            }
            else
            {
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = item.MNumberID,
                    CellType = BizReportCellType.TreeText,
                    CellLink = GetCellLink(ctx, item, filter)
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = item.MAccountName,
                    CellType = BizReportCellType.TreeText,
                    CellLink = GetCellLink(ctx, item, filter)
                });
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
                d += Math.Abs(item.MBeginDebitBalance);
                d += Math.Abs(item.MBeginCreditBalance);
                d += Math.Abs(item.MBeginDebitBalanceFor);
                d += Math.Abs(item.MBeginCreditBalanceFor);
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
            }
            balanceBizReportRowModel.TotalValue = Math.Abs(item.MEndDebitBalance) + Math.Abs(item.MEndCreditBalance);
            balanceBizReportRowModel.CurrentTotalAmount = Math.Abs(item.MDebit) + Math.Abs(item.MCredit);
            return balanceBizReportRowModel;
        }

        private string GetAccountNumber4Order(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                return number;
            }
            List<string> list = number.Split('.').ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i].ToFixLengthString(4, "0");
            }
            return string.Join(".", list);
        }

        private RPTAccountBalanceModel ToRptAccountBalanceModel(MContext ctx, List<GLBalanceModel> balanceList, BDAccountModel accountModel, bool isCheckType = false)
        {
            RPTAccountBalanceModel rPTAccountBalanceModel = new RPTAccountBalanceModel();
            rPTAccountBalanceModel.MAccountID = accountModel.MItemID;
            rPTAccountBalanceModel.MAccountParentID = accountModel.MParentID;
            rPTAccountBalanceModel.MAccountName = accountModel.MName;
            rPTAccountBalanceModel.MNumberID = accountModel.MNumber;
            if (balanceList == null || balanceList.Count() == 0)
            {
                return null;
            }
            GLBalanceModel gLBalanceModel = balanceList.FirstOrDefault();
            rPTAccountBalanceModel.MCheckGroupValueID = gLBalanceModel.MCheckGroupValueID;
            rPTAccountBalanceModel.MCheckGroupValueName = GLBalanceBusiness.GetCheckTypeValueName(ctx, gLBalanceModel);
            int maxYearPeriod = balanceList.Max((GLBalanceModel x) => x.MYearPeriod);
            int minYearPeriod = balanceList.Min((GLBalanceModel x) => x.MYearPeriod);
            IEnumerable<GLBalanceModel> source = from x in balanceList
                                                 where x.MYearPeriod == minYearPeriod
                                                 select x;
            IEnumerable<GLBalanceModel> source2 = from x in balanceList
                                                  where x.MYearPeriod == maxYearPeriod
                                                  select x;
            if (accountModel.MDC == 1)
            {
                rPTAccountBalanceModel.MBeginDebitBalance = source.Sum((GLBalanceModel x) => x.MBeginBalance);
                rPTAccountBalanceModel.MBeginDebitBalanceFor = source.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
                rPTAccountBalanceModel.MEndDebitBalance = source2.Sum((GLBalanceModel x) => x.MBeginBalance) + source2.Sum((GLBalanceModel x) => x.MDebit) - source2.Sum((GLBalanceModel x) => x.MCredit);
                rPTAccountBalanceModel.MEndDebitBalanceFor = source2.Sum((GLBalanceModel x) => x.MBeginBalanceFor) + source2.Sum((GLBalanceModel x) => x.MDebitFor) - source2.Sum((GLBalanceModel x) => x.MCreditFor);
            }
            else
            {
                rPTAccountBalanceModel.MBeginCreditBalance = source.Sum((GLBalanceModel x) => x.MBeginBalance);
                rPTAccountBalanceModel.MBeginCreditBalanceFor = source.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
                rPTAccountBalanceModel.MEndCreditBalance = source2.Sum((GLBalanceModel x) => x.MBeginBalance) + source2.Sum((GLBalanceModel x) => x.MCredit) - source2.Sum((GLBalanceModel x) => x.MDebit);
                rPTAccountBalanceModel.MEndCreditBalanceFor = source2.Sum((GLBalanceModel x) => x.MBeginBalanceFor) + source2.Sum((GLBalanceModel x) => x.MCreditFor) - source2.Sum((GLBalanceModel x) => x.MDebitFor);
            }
            rPTAccountBalanceModel.MCredit = balanceList.Sum((GLBalanceModel x) => x.MCredit);
            rPTAccountBalanceModel.MCreditFor = balanceList.Sum((GLBalanceModel x) => x.MCreditFor);
            rPTAccountBalanceModel.MDebit = balanceList.Sum((GLBalanceModel x) => x.MDebit);
            rPTAccountBalanceModel.MDebitFor = balanceList.Sum((GLBalanceModel x) => x.MDebitFor);
            rPTAccountBalanceModel.MYtdCredit = source2.Sum((GLBalanceModel x) => x.MYtdCredit);
            rPTAccountBalanceModel.MYtdCreditFor = source2.Sum((GLBalanceModel x) => x.MYtdCreditFor);
            rPTAccountBalanceModel.MYtdDebit = source2.Sum((GLBalanceModel x) => x.MYtdDebit);
            rPTAccountBalanceModel.MYtdDebitFor = source2.Sum((GLBalanceModel x) => x.MYtdDebitFor);
            return rPTAccountBalanceModel;
        }

        private void SetDataSummary(BizReportModel model, RPTAccountBalanceFilterModel filter, MContext context)
        {
            BalanceBizReportRowModel balanceBizReportRowModel = new BalanceBizReportRowModel();
            balanceBizReportRowModel.RowType = BizReportRowType.Total;
            balanceBizReportRowModel.AddCell(new BizReportCellModel
            {
                Value = "",
                CellType = BizReportCellType.Text
            });
            balanceBizReportRowModel.AddCell(new BizReportCellModel
            {
                Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total"),
                CellType = BizReportCellType.Text
            });
            decimal d = default(decimal);
            List<RPTAccountBalanceModel> list = null;
            if (filter.IsLeafAccount)
            {
                if (SummaryDataList != null && SummaryDataList.Count > 0)
                {
                    List<string> list2 = (from x in SummaryDataList
                                          select x.MAccountID).ToList();
                    List<BDAccountModel> childrenList = BDAccountHelper.GetChildrenList(AccountList);
                    List<string> leafAccountIdList = (from x in childrenList
                                                      select x.MItemID).ToList();
                    list = (from x in SummaryDataList
                            where leafAccountIdList.Contains(x.MAccountID)
                            select x).ToList();
                }
                else
                {
                    list = new List<RPTAccountBalanceModel>();
                }
            }
            else
            {
                list = ((SummaryDataList == null) ? new List<RPTAccountBalanceModel>() : SummaryDataList);
            }
            string defaultValue = "";
            if (IncludeForeginCurrency)
            {
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MBeginDebitBalanceFor), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MBeginDebitBalance), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MBeginCreditBalanceFor), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MBeginCreditBalance), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MDebitFor), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MDebit), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MCreditFor), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MCredit), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MYtdDebitFor), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MYtdDebit), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MYtdCreditFor), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MYtdCredit), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MEndDebitBalanceFor), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MEndDebitBalance), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MEndCreditBalanceFor), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MEndCreditBalance), defaultValue),
                    CellType = BizReportCellType.Money
                });
            }
            else
            {
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MBeginDebitBalance), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MBeginCreditBalance), defaultValue),
                    CellType = BizReportCellType.Money
                });
                d += list.Sum((RPTAccountBalanceModel x) => x.MBeginDebitBalance);
                d += list.Sum((RPTAccountBalanceModel x) => x.MBeginCreditBalance);
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MDebit), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MCredit), defaultValue),
                    CellType = BizReportCellType.Money
                });
                d += list.Sum((RPTAccountBalanceModel x) => x.MDebit);
                d += list.Sum((RPTAccountBalanceModel x) => x.MCredit);
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MYtdDebit), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MYtdCredit), defaultValue),
                    CellType = BizReportCellType.Money
                });
                d += list.Sum((RPTAccountBalanceModel x) => x.MYtdDebit);
                d += list.Sum((RPTAccountBalanceModel x) => x.MYtdCredit);
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MEndDebitBalance), defaultValue),
                    CellType = BizReportCellType.Money
                });
                balanceBizReportRowModel.AddCell(new BizReportCellModel
                {
                    Value = AccountHelper.MoneyToString(list.Sum((RPTAccountBalanceModel x) => x.MEndCreditBalance), defaultValue),
                    CellType = BizReportCellType.Money
                });
                d += list.Sum((RPTAccountBalanceModel x) => x.MEndDebitBalance);
                d += list.Sum((RPTAccountBalanceModel x) => x.MEndCreditBalance);
            }
            model.AddRow(balanceBizReportRowModel);
        }

        public BizReportModel GetReportModel(MContext ctx, RPTAccountBalanceFilterModel filter)
        {
            IncludeForeginCurrency = (!string.IsNullOrEmpty(filter.MCurrencyID) && filter.MCurrencyID != "0" && filter.MCurrencyID != ctx.MBasCurrencyID);
            AccountList = GLReportHelper.GetAccountList(ctx, filter);
            BalanceList = GLReportHelper.GetBalanceList(ctx, filter, true, true, null);
            return GetBalanceReportModel(ctx, filter);
        }
    }
}
