using JieNor.Megi.Core;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JieNor.Megi.DataRepository.RPT.Biz
{
	public class RPTAssetDepreciationRepository
	{
		private static Dictionary<string, List<string>> DataColumns;

		public static BizReportModel GetAssetDepreciationData(MContext context, RPTAssetDepreciationFilterModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel
			{
				Type = Convert.ToInt32(BizReportType.AssetDepreciation)
			};
			SetTitle(context, bizReportModel, filter);
			SetDataColumns(context, filter);
			SetRowHead(bizReportModel, filter, context);
			SetRowData(context, bizReportModel, filter);
			return bizReportModel;
		}

		private static void SetTitle(MContext context, BizReportModel model, RPTAssetDepreciationFilterModel filter)
		{
			model.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "AssetDepreciation", "固定资产明细账");
			model.Title2 = context.MOrgName;
			model.Title3 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ForThePeriod", "For the period ") + " " + filter.MStartPeroid + " " + COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "To", "to") + " " + filter.MEndPeroid;
		}

		private static void SetDataColumns(MContext context, RPTAssetDepreciationFilterModel filter)
		{
			DataColumns = new Dictionary<string, List<string>>();
			DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "VoucherDate", "日期"), null);
			DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "VoucherNum", "凭证字号"), null);
			DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Reference", "摘要"), null);
			if (filter.IncludeCheckType)
			{
				DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "AccountDimension", "核算维度"), null);
			}
			string text = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "OriginalAmount", "原值");
			List<string> childrenColumnList = GetChildrenColumnList(context, text);
			DataColumns.Add(text, childrenColumnList);
			string text2 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "AccumulateDepreciation", "累计折旧");
			DataColumns.Add(text2, GetChildrenColumnList(context, text2));
			DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "NetWorth", "净值"), null);
			string text3 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "DepreciationReserves", "减值准备");
			DataColumns.Add(text3, GetChildrenColumnList(context, text3));
			DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "EndBalance", "净额"), null);
		}

		private static void SetRowHead(BizReportModel reportModel, RPTAssetDepreciationFilterModel filter, MContext ctx)
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

		private static List<string> GetChildrenColumnList(MContext ctx, string baseColumnName)
		{
			List<string> list = new List<string>();
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "Debit", "Debit");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "Credit", "Credit");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "Balance", "Balance");
			list.Add(text);
			list.Add(text2);
			list.Add(text3);
			return list;
		}

		private static void SetRowData(MContext context, BizReportModel model, RPTAssetDepreciationFilterModel filter)
		{
			List<RPTAssetDepreciationModel> list = new List<RPTAssetDepreciationModel>();
			List<string> list2 = new List<string>
			{
				"1601",
				"1602",
				"1603"
			};
			DateTime dateTime = DateTime.ParseExact(filter.MStartPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime dateTime2 = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			int num = (dateTime2.Year - dateTime.Year) * 12 + (dateTime2.Month - dateTime.Month) + 1;
			List<RPTAssetDepreciationModel> blanceData = GetBlanceData(context, filter);
			filter.MStartDate = dateTime;
			DateTime dateTime3 = dateTime2.AddMonths(1);
			filter.MEndDate = dateTime3.AddDays(-1.0);
			List<RPTAssetDepreciationModel> source = GetVoucherData(context, filter);
			if (filter.IncludeCheckType && filter.CheckTypeValueList != null && filter.CheckTypeValueList.Count > 0)
			{
				foreach (NameValueModel checkTypeValue in filter.CheckTypeValueList)
				{
					switch (checkTypeValue.MName)
					{
					case "0":
						source = (from m in source
						where m.MContactIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "1":
						source = (from m in source
						where m.MEmployeeIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "2":
						source = (from m in source
						where m.MMerItemIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "3":
						source = (from m in source
						where m.MExpItemIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "4":
						source = (from m in source
						where m.MPaItemIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "5":
						source = (from m in source
						where m.MTrackItem1Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "6":
						source = (from m in source
						where m.MTrackItem2Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "7":
						source = (from m in source
						where m.MTrackItem3Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "8":
						source = (from m in source
						where m.MTrackItem4Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "9":
						source = (from m in source
						where m.MTrackItem5Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					}
				}
			}
			DateTime tempPeriod = dateTime;
			decimal num2 = default(decimal);
			decimal num3 = default(decimal);
			decimal num4 = default(decimal);
			for (int i = 0; i < num; i++)
			{
				int tempYearPeriod = tempPeriod.Year * 100 + tempPeriod.Month;
				List<RPTAssetDepreciationModel> source2 = (from m in blanceData
				where m.MYearPeriod == tempYearPeriod
				select m).ToList();
				RPTAssetDepreciationModel rPTAssetDepreciationModel = new RPTAssetDepreciationModel();
				rPTAssetDepreciationModel.VoucherDate = tempPeriod.ToString("yyyy-MM-dd");
				rPTAssetDepreciationModel.VoucherNumber = "";
				rPTAssetDepreciationModel.Explanation = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "BeginningBalance", "期初余额");
				RPTAssetDepreciationModel rPTAssetDepreciationModel2 = source2.FirstOrDefault((RPTAssetDepreciationModel m) => m.MNumberID == "1601");
				if (rPTAssetDepreciationModel2 != null)
				{
					rPTAssetDepreciationModel.OriginDebit = rPTAssetDepreciationModel2.MBeginBalance;
					rPTAssetDepreciationModel.OriginCredit = decimal.Zero;
					rPTAssetDepreciationModel.OrginEndBalance = rPTAssetDepreciationModel2.MBeginBalance;
					num2 = rPTAssetDepreciationModel.OrginEndBalance;
				}
				RPTAssetDepreciationModel rPTAssetDepreciationModel3 = source2.FirstOrDefault((RPTAssetDepreciationModel m) => m.MNumberID == "1602");
				if (rPTAssetDepreciationModel3 != null)
				{
					rPTAssetDepreciationModel.DepreciationDebit = decimal.Zero;
					rPTAssetDepreciationModel.DepreciationCredit = rPTAssetDepreciationModel3.MBeginBalance;
					rPTAssetDepreciationModel.DepreciationEndBalance = rPTAssetDepreciationModel3.MBeginBalance;
					num3 = rPTAssetDepreciationModel.DepreciationEndBalance;
				}
				RPTAssetDepreciationModel rPTAssetDepreciationModel4 = source2.FirstOrDefault((RPTAssetDepreciationModel m) => m.MNumberID == "1603");
				if (rPTAssetDepreciationModel4 != null)
				{
					rPTAssetDepreciationModel.DepreciationReservesDebit = decimal.Zero;
					rPTAssetDepreciationModel.DepreciationReservesCredit = rPTAssetDepreciationModel4.MBeginBalance;
					rPTAssetDepreciationModel.DepreciationReservesEndBalance = rPTAssetDepreciationModel4.MBeginBalance;
					num4 = rPTAssetDepreciationModel.DepreciationReservesEndBalance;
				}
				SetRowItemData(context, model, filter, rPTAssetDepreciationModel);
				rPTAssetDepreciationModel.IsBalance = true;
				rPTAssetDepreciationModel.MYearPeriod = tempYearPeriod;
				List<RPTAssetDepreciationModel> source3 = (from m in source
				where m.MYear == tempPeriod.Year && m.MPeriod == tempPeriod.Month
				orderby m.MDate
				select m).ToList();
				List<RPTAssetDepreciationModel> list3 = new List<RPTAssetDepreciationModel>();
				list3 = ((!filter.IncludeCheckType) ? (from p in source3
				group p by new
				{
					p.MNumber
				} into g
				select new RPTAssetDepreciationModel
				{
					MNumber = g.Key.MNumber
				}).ToList() : (from p in source3
				group p by new
				{
					p.MNumber,
					p.MCheckGroupValueID
				} into g
				select new RPTAssetDepreciationModel
				{
					MNumber = g.Key.MNumber,
					MCheckGroupValueID = g.Key.MCheckGroupValueID
				}).ToList());
				foreach (RPTAssetDepreciationModel item in list3)
				{
					List<RPTAssetDepreciationModel> list4 = new List<RPTAssetDepreciationModel>();
					list4 = ((!filter.IncludeCheckType) ? (from m in source3
					where m.MNumber == item.MNumber
					select m).ToList() : (from m in source3
					where m.MNumber == item.MNumber && m.MCheckGroupValueID == item.MCheckGroupValueID
					select m).ToList());
					RPTAssetDepreciationModel rPTAssetDepreciationModel5 = list4.FirstOrDefault();
					string text = "";
					if (rPTAssetDepreciationModel5 != null)
					{
						RPTAssetDepreciationModel rPTAssetDepreciationModel6 = rPTAssetDepreciationModel5;
						dateTime3 = rPTAssetDepreciationModel5.MDate;
						rPTAssetDepreciationModel6.VoucherDate = dateTime3.ToString("yyyy-MM-dd");
						rPTAssetDepreciationModel5.VoucherNumber = rPTAssetDepreciationModel5.MNumber;
						rPTAssetDepreciationModel5.Explanation = rPTAssetDepreciationModel5.MSummary;
						List<RPTAssetDepreciationModel> source4 = (from m in list4
						where m.MNumberID.StartsWith("1601")
						select m).ToList();
						if (source4.Any())
						{
							text += GetCheckGroupValue(context, source4.FirstOrDefault());
							rPTAssetDepreciationModel5.OriginDebit = source4.Sum((RPTAssetDepreciationModel m) => m.MDebit);
							rPTAssetDepreciationModel5.OriginCredit = source4.Sum((RPTAssetDepreciationModel m) => m.MCredit);
							rPTAssetDepreciationModel5.OrginEndBalance = num2 + rPTAssetDepreciationModel5.OriginDebit - rPTAssetDepreciationModel5.OriginCredit;
							num2 = rPTAssetDepreciationModel5.OrginEndBalance;
						}
						else
						{
							rPTAssetDepreciationModel5.OrginEndBalance = num2;
						}
						List<RPTAssetDepreciationModel> source5 = (from m in list4
						where m.MNumberID.StartsWith("1602")
						select m).ToList();
						if (source5.Any())
						{
							text += GetCheckGroupValue(context, source5.FirstOrDefault());
							rPTAssetDepreciationModel5.DepreciationDebit = source5.Sum((RPTAssetDepreciationModel m) => m.MDebit);
							rPTAssetDepreciationModel5.DepreciationCredit = source5.Sum((RPTAssetDepreciationModel m) => m.MCredit);
							rPTAssetDepreciationModel5.DepreciationEndBalance = num3 - rPTAssetDepreciationModel5.DepreciationDebit + rPTAssetDepreciationModel5.DepreciationCredit;
							num3 = rPTAssetDepreciationModel5.DepreciationEndBalance;
						}
						else
						{
							rPTAssetDepreciationModel5.DepreciationEndBalance = num3;
						}
						List<RPTAssetDepreciationModel> source6 = (from m in list4
						where m.MNumberID.StartsWith("1603")
						select m).ToList();
						if (source6.Any())
						{
							text += GetCheckGroupValue(context, source6.FirstOrDefault());
							rPTAssetDepreciationModel5.DepreciationReservesDebit = source6.Sum((RPTAssetDepreciationModel m) => m.MDebit);
							rPTAssetDepreciationModel5.DepreciationReservesCredit = source6.Sum((RPTAssetDepreciationModel m) => m.MCredit);
							rPTAssetDepreciationModel5.DepreciationReservesEndBalance = num4 - rPTAssetDepreciationModel5.DepreciationReservesDebit + rPTAssetDepreciationModel5.DepreciationReservesCredit;
							num4 = rPTAssetDepreciationModel5.DepreciationReservesEndBalance;
						}
						else
						{
							rPTAssetDepreciationModel5.DepreciationReservesEndBalance = num4;
						}
						rPTAssetDepreciationModel5.GroupValueStr = text;
						SetRowItemData(context, model, filter, rPTAssetDepreciationModel5);
						rPTAssetDepreciationModel5.MYearPeriod = tempYearPeriod;
						list.Add(rPTAssetDepreciationModel5);
					}
				}
				RPTAssetDepreciationModel rPTAssetDepreciationModel7 = new RPTAssetDepreciationModel();
				RPTAssetDepreciationModel rPTAssetDepreciationModel8 = rPTAssetDepreciationModel7;
				dateTime3 = tempPeriod.AddMonths(1);
				dateTime3 = dateTime3.AddDays(-1.0);
				rPTAssetDepreciationModel8.VoucherDate = dateTime3.ToString("yyyy-MM-dd");
				rPTAssetDepreciationModel7.VoucherNumber = "";
				rPTAssetDepreciationModel7.Explanation = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "PeriodSummary", "本期合计");
				List<RPTAssetDepreciationModel> source7 = (from m in list
				where m.MYearPeriod == tempYearPeriod
				select m).ToList();
				rPTAssetDepreciationModel7.OriginDebit = source7.Sum((RPTAssetDepreciationModel m) => m.OriginDebit);
				rPTAssetDepreciationModel7.OriginCredit = source7.Sum((RPTAssetDepreciationModel m) => m.OriginCredit);
				rPTAssetDepreciationModel7.OrginEndBalance = num2;
				rPTAssetDepreciationModel7.DepreciationDebit = source7.Sum((RPTAssetDepreciationModel m) => m.DepreciationDebit);
				rPTAssetDepreciationModel7.DepreciationCredit = source7.Sum((RPTAssetDepreciationModel m) => m.DepreciationCredit);
				rPTAssetDepreciationModel7.DepreciationEndBalance = num3;
				rPTAssetDepreciationModel7.DepreciationReservesDebit = source7.Sum((RPTAssetDepreciationModel m) => m.DepreciationReservesDebit);
				rPTAssetDepreciationModel7.DepreciationReservesCredit = source7.Sum((RPTAssetDepreciationModel m) => m.DepreciationReservesCredit);
				rPTAssetDepreciationModel7.DepreciationReservesEndBalance = num4;
				SetRowItemData(context, model, filter, rPTAssetDepreciationModel7);
				RPTAssetDepreciationModel yearSummaryModel = GetYearSummaryModel(context, filter, tempPeriod);
				yearSummaryModel.OrginEndBalance = num2;
				yearSummaryModel.DepreciationEndBalance = num3;
				yearSummaryModel.DepreciationReservesEndBalance = num4;
				yearSummaryModel.Explanation = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "YearSummary", "本年累计");
				SetRowItemData(context, model, filter, yearSummaryModel);
				tempPeriod = tempPeriod.AddMonths(1);
			}
		}

		private static RPTAssetDepreciationModel GetYearSummaryModel(MContext context, RPTAssetDepreciationFilterModel filter, DateTime currentDateTime)
		{
			RPTAssetDepreciationModel rPTAssetDepreciationModel = new RPTAssetDepreciationModel();
			DateTime dateTime2 = filter.MStartDate = DateTime.ParseExact(currentDateTime.Year + "0101", "yyyyMMdd", CultureInfo.CurrentCulture);
			filter.MEndDate = currentDateTime.AddMonths(1).AddDays(-1.0);
			List<RPTAssetDepreciationModel> voucherData = GetVoucherData(context, filter);
			List<RPTAssetDepreciationModel> source = (from m in voucherData
			where m.MNumberID.StartsWith("1601")
			select m).ToList();
			if (source.Any())
			{
				rPTAssetDepreciationModel.OriginDebit = source.Sum((RPTAssetDepreciationModel m) => m.MDebit);
				rPTAssetDepreciationModel.OriginCredit = source.Sum((RPTAssetDepreciationModel m) => m.MCredit);
			}
			List<RPTAssetDepreciationModel> source2 = (from m in voucherData
			where m.MNumberID.StartsWith("1602")
			select m).ToList();
			if (source2.Any())
			{
				rPTAssetDepreciationModel.DepreciationDebit = source2.Sum((RPTAssetDepreciationModel m) => m.MDebit);
				rPTAssetDepreciationModel.DepreciationCredit = source2.Sum((RPTAssetDepreciationModel m) => m.MCredit);
			}
			List<RPTAssetDepreciationModel> source3 = (from m in voucherData
			where m.MNumberID.StartsWith("1603")
			select m).ToList();
			if (source3.Any())
			{
				rPTAssetDepreciationModel.DepreciationReservesDebit = source3.Sum((RPTAssetDepreciationModel m) => m.MDebit);
				rPTAssetDepreciationModel.DepreciationReservesCredit = source3.Sum((RPTAssetDepreciationModel m) => m.MCredit);
			}
			return rPTAssetDepreciationModel;
		}

		private static void BalanceListFilter(MContext context, RPTAssetDepreciationFilterModel filter, List<RPTAssetDepreciationModel> balanceList)
		{
			if (filter.CheckTypeValueList != null && filter.CheckTypeValueList.Count() > 0)
			{
				balanceList = FilterBalanceList(balanceList, filter.CheckTypeValueList);
			}
		}

		private static List<RPTAssetDepreciationModel> FilterBalanceList(List<RPTAssetDepreciationModel> balanceList, List<NameValueModel> checkTypeList)
		{
			List<RPTAssetDepreciationModel> result = new List<RPTAssetDepreciationModel>();
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

		private static void SetRowItemData(MContext context, BizReportModel model, RPTAssetDepreciationFilterModel filter, RPTAssetDepreciationModel item)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Item;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.VoucherDate),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(string.IsNullOrWhiteSpace(item.VoucherNumber) ? "" : ("GL-" + item.VoucherNumber)),
				CellType = BizReportCellType.Text,
				CellLink = GetCellLink(context, item)
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.Explanation),
				CellType = BizReportCellType.Text
			});
			if (filter.IncludeCheckType)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(item.GroupValueStr),
					CellType = BizReportCellType.Text
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.OriginDebit),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.OriginCredit),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.OrginEndBalance),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.DepreciationDebit),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.DepreciationCredit),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.DepreciationEndBalance),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.OrginEndBalance - item.DepreciationEndBalance),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.DepreciationReservesDebit),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.DepreciationReservesCredit),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.DepreciationReservesEndBalance),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.OrginEndBalance - item.DepreciationEndBalance - item.DepreciationReservesEndBalance),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private static List<RPTAssetDepreciationModel> GetBlanceData(MContext context, RPTAssetDepreciationFilterModel filter)
		{
			string sql = " SELECT t0.MCode AS MNumberID,t1.* FROM\r\n                               ( SELECT * FROM t_bd_account WHERE MOrgID= @MOrgID AND MIsDelete=0 AND MIsActive=1\r\n                                    AND MCode IN( '1601','1602','1603') ) t0\r\n                               INNER JOIN  ( SELECT * FROM t_gl_balance WHERE MOrgID = @MOrgID AND MIsDelete=0 AND MIsActive=1\r\n                                    AND MCheckGroupValueID = '0'  AND MYearPeriod BETWEEN @MStartPeroid AND @MEndPeroid ) t1\r\n                               ON t0.MItemID = t1. MAccountID ";
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStartPeroid", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MEndPeroid", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MOrgID;
			array[1].Value = context.MLCID;
			array[2].Value = filter.MStartPeroid;
			array[3].Value = filter.MEndPeroid;
			return ModelInfoManager.GetDataModelBySql<RPTAssetDepreciationModel>(context, sql, array);
		}

		private static List<RPTAssetDepreciationModel> GetVoucherData(MContext context, RPTAssetDepreciationFilterModel filter)
		{
			string sql = GetSql(filter);
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStartDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MEndDate", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MOrgID;
			array[1].Value = context.MLCID;
			array[2].Value = filter.MStartDate;
			array[3].Value = filter.MEndDate;
			return ModelInfoManager.GetDataModelBySql<RPTAssetDepreciationModel>(context, sql, array);
		}

		private static string GetSql(RPTAssetDepreciationFilterModel filter)
		{
			string str = " SELECT t1.*,t2.MCheckGroupValueID,t3.MCode AS MNumberID,t2.MExplanation AS MSummary,t2.MDebit,t2.MCredit ";
			if (filter.IncludeCheckType)
			{
				str += " ,t2.MCheckGroupValueID,CONVERT ( AES_DECRYPT(t6.MName, '{0}') USING utf8 ) AS MContactNameExp,\r\n                                F_GETUSERNAME (t7.MFirstName, t7.MLastName) AS MEmployeeNameExp,concat(t8_0.MNumber, ':', t8.MDesc) AS MMerItemNameExp,\r\n                                t9.MName AS MExpItemNameExp,t10.MName AS MPaItemNameExp,t10_0.MGroupID AS MPaItemGroupIDExp,\r\n                                t10_1.MName AS MPaItemGroupNameExp,t11.MName AS MTrackItem1NameExp,t11_2.MName AS MTrackItem1GroupNameExp,\r\n                                t12.MName AS MTrackItem2NameExp,t12_2.MName AS MTrackItem2GroupNameExp,t13.MName AS MTrackItem3NameExp,\r\n                                t13_2.MName AS MTrackItem3GroupNameExp,t14.MName AS MTrackItem4NameExp,t14_2.MName AS MTrackItem4GroupNameExp,\r\n                                t15.MName AS MTrackItem5NameExp,t15_2.MName AS MTrackItem5GroupNameExp,t100.MContactID AS MContactIDExp,\r\n                                t100.MEmployeeID AS MEmployeeIDExp,t100.MMerItemID AS MMerItemIDExp,t100.MPaItemID AS MPaItemIDExp,t100.MTrackItem1 AS MTrackItem1Exp,\r\n                                t100.MTrackItem2 AS MTrackItem2Exp,t100.MTrackItem3 AS MTrackItem3Exp,t100.MTrackItem4 AS MTrackItem4Exp,t100.MTrackItem5 AS MTrackItem5Exp ";
			}
			str += " FROM ( SELECT * FROM t_gl_voucher WHERE MOrgID= @MOrgID AND MIsDelete=0  AND MStatus = 1 AND MDate BETWEEN @MStartDate AND @MEndDate ) t1\r\n                               INNER JOIN t_gl_voucherentry t2 ON t2.MID = t1.MitemID AND t2.MIsDelete = 0 \r\n                               INNER JOIN  ( SELECT * FROM t_bd_account WHERE  MOrgID= @MOrgID AND MIsDelete=0 \r\n                                AND ( MCode LIKE  '1601%' OR MCode LIKE  '1602%' OR MCode LIKE  '1603%' ) ) t3 ON t3.MItemID = t2.MAccountID ";
			if (filter.IncludeCheckType)
			{
				str += " LEFT JOIN t_gl_checkgroupvalue t100 ON t100.MItemID = t2.MCheckGroupValueID\r\n                                AND t100.MOrgID = t1.MOrgID AND t100.MIsDelete = 0 \r\n                                LEFT JOIN t_bd_contacts_l t6 ON t6.MParentID = t100.MContactID\r\n                                AND t6.MLocaleID = @MLCID AND t6.MOrgID = t1.MOrgID  AND t6.MIsDelete = 0\r\n                                LEFT JOIN t_bd_employees_l t7 ON t7.MParentID = t100.MEmployeeID\r\n                                AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID = @MLCID\r\n                                LEFT JOIN t_bd_item t8_0 ON t8_0.MItemID = t100.MMerItemID\r\n                                AND t8_0.MOrgID = t1.MOrgId AND t8_0.MIsDelete = t1.MIsDelete\r\n                                LEFT JOIN t_bd_item_l t8 ON t8.MParentID = t100.MMerItemID\r\n                                AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID = @MLCID\r\n                                LEFT JOIN t_bd_expenseitem_l t9 ON t9.MParentID = t100.MExpItemID\r\n                                AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID = @MLCID\r\n                                LEFT JOIN t_pa_payitem_l t10 ON t10.MParentID = t100.MPaItemID\r\n                                AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID = @MLCID\r\n                                LEFT JOIN t_pa_payitem t10_0 ON t10_0.MItemID = t100.MPaItemID\r\n                                AND t10_0.MOrgID = t1.MOrgId AND t10_0.MIsDelete = t1.MIsDelete\r\n                                LEFT JOIN t_pa_payitemgroup_l t10_1 ON t10_1.MParentID = t100.MPaItemID \r\n                                AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID = @MLCID\r\n                                LEFT JOIN t_bd_trackentry_l t11 ON t11.MParentID = t100.MTrackItem1\r\n                                AND t11.MLocaleID = @MLCID AND t11.MOrgID = t1.MOrgID AND t11.MIsDelete = 0\r\n                                LEFT JOIN t_bd_trackentry t11_1 ON t11_1.MEntryID = t100.MTrackItem1\r\n                                AND t11_1.MOrgID = t1.MOrgID AND t11_1.MIsDelete = 0\r\n                                LEFT JOIN t_bd_track_l t11_2 ON t11_2.MParentID = t11_1.MItemID\r\n                                AND t11_2.MLocaleID = @MLCID AND t11_2.MOrgID = t1.MOrgID AND t11_2.MIsDelete = 0\r\n                                LEFT JOIN t_bd_trackentry_l t12 ON t12.MParentID = t100.MTrackItem2\r\n                                AND t12.MLocaleID = @MLCID AND t12.MOrgID = t1.MOrgID AND t12.MIsDelete = 0\r\n                                LEFT JOIN t_bd_trackentry t12_1 ON t12_1.MEntryID = t100.MTrackItem2\r\n                                AND t12_1.MOrgID = t1.MOrgID AND t12_1.MIsDelete = 0\r\n                                LEFT JOIN t_bd_track_l t12_2 ON t12_2.MParentID = t12_1.MItemID\r\n                                AND t12_2.MLocaleID =  @MLCID AND t12_2.MOrgID = t1.MOrgID AND t12_2.MIsDelete = 0\r\n                                LEFT JOIN t_bd_trackentry_l t13 ON t13.MParentID = t100.MTrackItem3\r\n                                AND t13.MLocaleID =  @MLCID AND t13.MOrgID = t1.MOrgID AND t13.MIsDelete = 0\r\n                                LEFT JOIN t_bd_trackentry t13_1 ON t13_1.MEntryID = t100.MTrackItem3\r\n                                AND t13_1.MOrgID = t1.MOrgID AND t13_1.MIsDelete = 0\r\n                                LEFT JOIN t_bd_track_l t13_2 ON t13_2.MParentID = t13_1.MItemID\r\n                                AND t13_2.MLocaleID = @MLCID AND t13_2.MOrgID = t1.MOrgID AND t13_2.MIsDelete = 0\r\n                                LEFT JOIN t_bd_trackentry_l t14 ON t14.MParentID = t100.MTrackItem4\r\n                                AND t14.MLocaleID = @MLCID AND t14.MOrgID = t1.MOrgID AND t14.MIsDelete = 0\r\n                                LEFT JOIN t_bd_trackentry t14_1 ON t14_1.MEntryID = t100.MTrackItem4\r\n                                AND t14_1.MOrgID = t1.MOrgID AND t14_1.MIsDelete = 0\r\n                                LEFT JOIN t_bd_track_l t14_2 ON t14_2.MParentID = t14_1.MItemID\r\n                                AND t14_2.MLocaleID = @MLCID AND t14_2.MOrgID = t1.MOrgID AND t14_2.MIsDelete = 0\r\n                                LEFT JOIN t_bd_trackentry_l t15 ON t15.MParentID = t100.MTrackItem5\r\n                                AND t15.MLocaleID = @MLCID AND t15.MOrgID = t1.MOrgID AND t15.MIsDelete = 0\r\n                                LEFT JOIN t_bd_trackentry t15_1 ON t15_1.MEntryID = t100.MTrackItem5\r\n                                AND t15_1.MOrgID = t1.MOrgID AND t15_1.MIsDelete = 0\r\n                                LEFT JOIN t_bd_track_l t15_2 ON t15_2.MParentID = t15_1.MItemID\r\n                                AND t15_2.MLocaleID = @MLCID AND t15_2.MOrgID = t1.MOrgID AND t15_2.MIsDelete = 0 ";
			}
			return string.Format(str, "JieNor-001");
		}

		private static BizReportCellLinkModel GetCellLink(MContext ctx, RPTAssetDepreciationModel rptModel)
		{
			BizReportCellLinkModel bizReportCellLinkModel = new BizReportCellLinkModel();
			bizReportCellLinkModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewVoucher", "View Voucher");
			bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewVoucher", "View Voucher");
			bizReportCellLinkModel.Url = $"GL/GLVoucher/GLVoucherEdit?MItemID={rptModel.MItemID}";
			return bizReportCellLinkModel;
		}

		public static string GetCheckGroupValue(MContext ctx, RPTAssetDepreciationModel model)
		{
			string text = "";
			if (!string.IsNullOrWhiteSpace(model.MContactNameExp))
			{
				text = text + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Contact", "联系人") + ":" + model.MContactNameExp + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MEmployeeNameExp))
			{
				text = text + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Employee", "员工") + ":" + model.MEmployeeNameExp + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MMerItemNameExp))
			{
				text = text + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "MerItem", "商品项目") + ":" + model.MMerItemNameExp + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MExpItemNameExp))
			{
				text = text + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItem", "费用项目") + ":" + model.MExpItemNameExp + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MPaItemGroupNameExp))
			{
				text = text + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PaSalaryItem", "工资项目") + ":" + model.MPaItemGroupNameExp + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MTrackItem1NameExp))
			{
				text = text + model.MTrackItem1GroupNameExp + ":" + model.MTrackItem1NameExp + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MTrackItem2NameExp))
			{
				text = text + model.MTrackItem2GroupNameExp + ":" + model.MTrackItem2NameExp + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MTrackItem3NameExp))
			{
				text = text + model.MTrackItem3GroupNameExp + ":" + model.MTrackItem3NameExp + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MTrackItem4NameExp))
			{
				text = text + model.MTrackItem4GroupNameExp + ":" + model.MTrackItem4NameExp + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MTrackItem5NameExp))
			{
				text = text + model.MTrackItem5GroupNameExp + ":" + model.MTrackItem5NameExp + ";";
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				text = text.TrimEnd(';') + ".";
			}
			return text;
		}
	}
}
