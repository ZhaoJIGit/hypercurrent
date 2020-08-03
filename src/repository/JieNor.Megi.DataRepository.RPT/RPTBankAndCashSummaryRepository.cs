using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.RPT
{
	public static class RPTBankAndCashSummaryRepository
	{
		public static BizReportModel BankAndCashSummaryList(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel();
			if (filter.IsShowDetail)
			{
				bizReportModel.Type = Convert.ToInt32(BizReportType.CashSummary);
			}
			else
			{
				bizReportModel.Type = Convert.ToInt32(BizReportType.BankAndCashSummary);
			}
			return GetReportData(ctx, filter, bizReportModel);
		}

		private static BizReportModel GetReportData(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter, BizReportModel model)
		{
			DateTime mEndDate = filter.MEndDate;
			if (filter.MEndDate >= filter.MStartDate)
			{
				if (filter.IsShowDetail)
				{
					GetDetailReporData(ctx, filter, model);
				}
				else
				{
					GetTotalReporData(ctx, filter, model);
				}
			}
			return model;
		}

		private static void GetDetailReporData(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter, BizReportModel model)
		{
			List<BankAndCashSummaryEntryModel> detailVerifData = GetDetailVerifData(ctx, filter);
			decimal openingBlance = filter.OpeningBlance;
			if (filter.MRate == decimal.Zero)
			{
				SetEntryRate(ctx, filter, detailVerifData);
			}
			model.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BankAndCashSummaryDetail", "Bank And Cash Summary Detail");
			model.Title2 = ctx.MOrgName;
			model.Title3 = RPTBaseREpository.GetDateHeadTitle(ctx, filter.MStartDate, filter.MEndDate);
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Date", "Date");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Type", "Type");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Transaction", "Transaction");
			string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Reference", "Reference");
			string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Received", "Received");
			string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Spent", "Spent");
			BizReportRowModel model2 = SetItemRow(ctx, 0, filter.IsShowDetail, text, text2, text3, text4, text5, text6, "", "");
			model.AddRow(model2);
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Group;
			model.AddRow(bizReportRowModel);
			detailDataRow(ctx, model, filter, detailVerifData);
		}

		private static void detailDataRow(MContext ctx, BizReportModel model, RPTBankAndCashSummaryFilterBaseModel filter, List<BankAndCashSummaryEntryModel> detailData)
		{
			decimal num = default(decimal);
			decimal num2 = default(decimal);
			string value = "";
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Date", "Date");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "OpeningBlance", "Opening Blance");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Total", "Total");
			string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ClosingBlance", "Closing Blance");
			decimal beginBalance = GetBeginBalance(ctx, filter);
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Item;
			BizReportRowModel bizReportRowModel2 = bizReportRowModel;
			string mAccountID = filter.MAccountID;
			DateTime dateTime = filter.MStartDate;
			bizReportRowModel2.UniqueValue = $"Opening{mAccountID}{dateTime.ToShortDateString()}";
			bizReportRowModel = SetItemRow(ctx, 3, filter.IsShowDetail, "", "", text2, "", Convert.ToString(beginBalance), "", "", "");
			model.AddRow(bizReportRowModel);
			List<BankAndCashSummaryEntryModel> list = (from f in detailData
			where f.MBizDate >= filter.MStartDate
			select f).ToList();
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					value = list[i].MBizDate.ToMShortDateString(null);
					bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.Item;
					bizReportRowModel.UniqueValue = $"{filter.MAccountID}{list[i].MBizDate.ToMShortDateString(null)}";
					decimal num3 = default(decimal);
					decimal num4 = default(decimal);
					if (filter.ShowInUSD)
					{
						num3 = list[i].Received;
						num4 = list[i].Payment;
					}
					else
					{
						num3 = list[i].ReceivedFor;
						num4 = list[i].PaymentFor;
					}
					bizReportRowModel = SetItemRow(ctx, 5, filter.IsShowDetail, Convert.ToString(value), list[i].MType, list[i].Transaction, list[i].Reference, (num3 == decimal.Zero && num4 > decimal.Zero) ? "" : Convert.ToString(num3), (num4 == decimal.Zero && num3 > decimal.Zero) ? "" : Convert.ToString(num4), list[i].MBizObject, list[i].MID);
					model.AddRow(bizReportRowModel);
					num += num3;
					num2 += num4;
				}
				BizReportRowModel model2 = SetItemRow(ctx, 3, filter.IsShowDetail, text3, "", "", "", Convert.ToString(num), Convert.ToString(num2), "", "");
				model.AddRow(model2);
			}
			BizReportRowModel bizReportRowModel3 = SetItemRow(ctx, 3, filter.IsShowDetail, Convert.ToString(value), "", text4, "", Convert.ToString(beginBalance + num - num2), "", "", "");
			BizReportRowModel bizReportRowModel4 = bizReportRowModel3;
			string mAccountID2 = filter.MAccountID;
			dateTime = filter.MEndDate;
			bizReportRowModel4.UniqueValue = $"Closing{mAccountID2}{dateTime.ToShortDateString()}";
			model.AddRow(bizReportRowModel3);
			if (filter.MRate != decimal.One && filter.ShowInUSD && filter.MRate > decimal.Zero)
			{
				decimal beginBal = beginBalance / filter.MRate;
				decimal endBal = (beginBalance + num - num2) / filter.MRate;
				GetRBDetail(ctx, filter, model, beginBal, endBal);
			}
		}

		private static decimal GetBeginBalance(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter)
		{
			RPTBankAndCashSummaryFilterBaseModel beginBalanceFilter = GetBeginBalanceFilter(ctx, filter);
			BankAndCashSummaryEntryModel detailVerifTotalData = GetDetailVerifTotalData(ctx, beginBalanceFilter);
			if (detailVerifTotalData == null)
			{
				return decimal.Zero;
			}
			if (filter.MRate != decimal.One && filter.ShowInUSD && filter.MRate > decimal.Zero)
			{
				return (detailVerifTotalData.IniBlance + detailVerifTotalData.Received - detailVerifTotalData.Payment) * filter.MRate;
			}
			return detailVerifTotalData.IniBlance + detailVerifTotalData.Received - detailVerifTotalData.Payment;
		}

		private static RPTBankAndCashSummaryFilterBaseModel GetBeginBalanceFilter(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter)
		{
			RPTBankAndCashSummaryFilterBaseModel rPTBankAndCashSummaryFilterBaseModel = new RPTBankAndCashSummaryFilterBaseModel();
			rPTBankAndCashSummaryFilterBaseModel.MAccountID = filter.MAccountID;
			rPTBankAndCashSummaryFilterBaseModel.MAccountName = filter.MAccountName;
			rPTBankAndCashSummaryFilterBaseModel.MStartDate = ctx.MBeginDate;
			rPTBankAndCashSummaryFilterBaseModel.MEndDate = filter.MStartDate.AddDays(-1.0);
			rPTBankAndCashSummaryFilterBaseModel.ShowInUSD = filter.ShowInUSD;
			rPTBankAndCashSummaryFilterBaseModel.MRate = filter.MRate;
			rPTBankAndCashSummaryFilterBaseModel.MTrackItem1 = filter.MTrackItem1;
			rPTBankAndCashSummaryFilterBaseModel.MTrackItem2 = filter.MTrackItem2;
			rPTBankAndCashSummaryFilterBaseModel.MTrackItem3 = filter.MTrackItem3;
			rPTBankAndCashSummaryFilterBaseModel.MTrackItem4 = filter.MTrackItem4;
			rPTBankAndCashSummaryFilterBaseModel.MTrackItem5 = filter.MTrackItem5;
			return rPTBankAndCashSummaryFilterBaseModel;
		}

		private static BankAndCashSummaryEntryModel GetBeginBalRowModel(RPTBankAndCashSummaryFilterBaseModel filter, List<BankAndCashSummaryEntryModel> detailData)
		{
			BankAndCashSummaryEntryModel bankAndCashSummaryEntryModel = new BankAndCashSummaryEntryModel();
			bankAndCashSummaryEntryModel.MBizDate = filter.MStartDate;
			if (detailData == null || detailData.Count == 0)
			{
				return bankAndCashSummaryEntryModel;
			}
			bankAndCashSummaryEntryModel.MBankAccountName = detailData[0].MBankAccountName;
			bankAndCashSummaryEntryModel.MCyID = detailData[0].MCyID;
			bankAndCashSummaryEntryModel.MCyRate = detailData[0].MCyRate;
			foreach (BankAndCashSummaryEntryModel detailDatum in detailData)
			{
				if (detailDatum.MBizDate < filter.MStartDate)
				{
					BankAndCashSummaryEntryModel bankAndCashSummaryEntryModel2 = bankAndCashSummaryEntryModel;
					bankAndCashSummaryEntryModel2.OpeningBlance += detailDatum.IniBlance + detailDatum.Received - detailDatum.Payment;
				}
			}
			return bankAndCashSummaryEntryModel;
		}

		private static void GetRBDetail(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter, BizReportModel model, decimal beginBal, decimal endBal)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "RevaluedBalance", "Revalued Balance");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "OpeningBlance", "Opening Blance");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "NetMovement", "Net Movement");
			string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ClosingBlance", "Closing Blance");
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = text,
				CellType = BizReportCellType.Text
			});
			model.AddRow(bizReportRowModel);
			string st = filter.MStartDate.ToMShortDateString(null);
			BizReportRowModel bizReportRowModel2 = SetItemRow(ctx, 1, filter.IsShowDetail, st, "", text2, "", Convert.ToString(beginBal), "", "", "");
			BizReportRowModel bizReportRowModel3 = bizReportRowModel2;
			string mCyID = filter.MCyID;
			string mAccountID = filter.MAccountID;
			DateTime dateTime = filter.MStartDate;
			bizReportRowModel3.UniqueValue = $"OpeningIn{mCyID}{mAccountID}{dateTime.ToShortDateString()}";
			model.AddRow(bizReportRowModel2);
			BizReportRowModel bizReportRowModel4 = SetItemRow(ctx, 1, filter.IsShowDetail, st, "", text4, "", Convert.ToString(endBal), "", "", "");
			BizReportRowModel bizReportRowModel5 = bizReportRowModel4;
			string mCyID2 = filter.MCyID;
			string mAccountID2 = filter.MAccountID;
			dateTime = filter.MEndDate;
			bizReportRowModel5.UniqueValue = $"ClosingIn{mCyID2}{mAccountID2}{dateTime.ToShortDateString()}";
			model.AddRow(bizReportRowModel4);
			BizReportRowModel model2 = SetItemRow(ctx, 3, filter.IsShowDetail, "", "", text3, "", Convert.ToString(endBal - beginBal), "", "", "");
			model.AddRow(model2);
		}

		private static BizReportRowModel SetItemRow(MContext ctx, int rowtype, bool IsShowDetail, string st1, string st2, string st3, string st4, string st5, string st6, string bizObject = "", string mid = "")
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			if (rowtype == 0)
			{
				bizReportRowModel.RowType = BizReportRowType.Header;
			}
			if (rowtype == 1)
			{
				bizReportRowModel.RowType = BizReportRowType.Item;
			}
			if (rowtype == 2)
			{
				bizReportRowModel.RowType = BizReportRowType.Total;
			}
			if (rowtype == 3)
			{
				bizReportRowModel.RowType = BizReportRowType.SubTotal;
			}
			if (IsShowDetail)
			{
				BizReportCellLinkModel cellLink = GetCellLink(ctx, bizObject, mid);
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st1,
					CellType = BizReportCellType.Text,
					CellLink = cellLink
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st2,
					CellType = BizReportCellType.Text,
					CellLink = cellLink
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st3,
					CellType = BizReportCellType.Text,
					CellLink = cellLink
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st4,
					CellType = BizReportCellType.Text,
					CellLink = cellLink
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st5,
					CellType = BizReportCellType.Money,
					CellLink = cellLink
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st6,
					CellType = BizReportCellType.Money,
					CellLink = cellLink
				});
			}
			else
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st1,
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st2,
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st3,
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st4,
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st5,
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = st6,
					CellType = BizReportCellType.Money
				});
			}
			return bizReportRowModel;
		}

		private static BizReportCellLinkModel GetCellLink(MContext ctx, string bizObject, string mid)
		{
			if (string.IsNullOrEmpty(bizObject) || string.IsNullOrEmpty(mid))
			{
				return null;
			}
			BizReportCellLinkModel bizReportCellLinkModel = new BizReportCellLinkModel();
			bizReportCellLinkModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ShowDetails", "Show Details");
			switch (bizObject)
			{
			case "Payment":
				bizReportCellLinkModel.Url = $"/IV/Payment/PaymentView/{mid}";
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewPayment", "View Payment");
				break;
			case "Receive":
				bizReportCellLinkModel.Url = $"/IV/Receipt/ReceiptView/{mid}";
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewReceive", "View Receive");
				break;
			case "Transfer":
				bizReportCellLinkModel.Url = $"/IV/IVTransfer/IVTransferHome?MID={mid}";
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewTransfer", "View Transfer");
				break;
			}
			return bizReportCellLinkModel;
		}

		private static void GetTotalReporData(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter, BizReportModel model)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BankAccounts", "Bank Accounts");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "OpeningBlance", "Opening Blance");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Received", "Received");
			string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Spent", "Spent");
			string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "FXGain", "FX Gain");
			string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ClosingBlance", "Closing Blance");
			string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Total", "Total");
			model.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BankAndCashSummary", "Bank And Cash Summary");
			model.Title2 = MText.Encode(ctx.MOrgName);
			model.Title3 = RPTBaseREpository.GetDateHeadTitle(ctx, filter.MStartDate, filter.MEndDate);
			BizReportRowModel model2 = SetItemRow(ctx, 0, filter.IsShowDetail, text, text2, text3, text4, text5, text6, "", "");
			model.AddRow(model2);
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Group;
			model.AddRow(bizReportRowModel);
			decimal num = default(decimal);
			decimal num2 = default(decimal);
			decimal num3 = default(decimal);
			decimal num4 = default(decimal);
			decimal num5 = default(decimal);
			List<RPTBankAndCashSummaryModel> totalVerifData = GetTotalVerifData(ctx, filter);
			SetRate(ctx, filter, totalVerifData);
			for (int i = 0; i < totalVerifData.Count; i++)
			{
				BizReportRowModel bizReportRowModel2 = new BizReportRowModel();
				bizReportRowModel2.UniqueValue = totalVerifData[i].MBankAccountID;
				bizReportRowModel2.RowType = BizReportRowType.Item;
				BizReportCellModel bizReportCellModel = new BizReportCellModel
				{
					Value = Convert.ToString(totalVerifData[i].MBankAccountName),
					CellType = BizReportCellType.Text
				};
				bizReportCellModel.SubReport = GetDetailRptFilterModel(ctx, totalVerifData[i], filter);
				bizReportRowModel2.AddCell(bizReportCellModel);
				BizReportCellModel bizReportCellModel2 = new BizReportCellModel
				{
					Value = Convert.ToString(totalVerifData[i].IniBlance + totalVerifData[i].OpeningBlance),
					CellType = BizReportCellType.Money
				};
				bizReportCellModel2.SubReport = bizReportCellModel.SubReport;
				bizReportRowModel2.AddCell(bizReportCellModel2);
				BizReportCellModel model3 = new BizReportCellModel
				{
					Value = Convert.ToString(totalVerifData[i].TReceived),
					CellType = BizReportCellType.Money
				};
				bizReportCellModel2.SubReport = bizReportCellModel.SubReport;
				bizReportRowModel2.AddCell(model3);
				BizReportCellModel model4 = new BizReportCellModel
				{
					Value = Convert.ToString(totalVerifData[i].TSpent),
					CellType = BizReportCellType.Money
				};
				bizReportCellModel2.SubReport = bizReportCellModel.SubReport;
				bizReportRowModel2.AddCell(model4);
				BizReportCellModel model5 = new BizReportCellModel
				{
					Value = Convert.ToString(totalVerifData[i].FXGain),
					CellType = BizReportCellType.Money
				};
				bizReportCellModel2.SubReport = bizReportCellModel.SubReport;
				bizReportRowModel2.AddCell(model5);
				BizReportCellModel model6 = new BizReportCellModel
				{
					Value = Convert.ToString(totalVerifData[i].ClosingBlance),
					CellType = BizReportCellType.Money
				};
				bizReportCellModel2.SubReport = bizReportCellModel.SubReport;
				bizReportRowModel2.AddCell(model6);
				model.AddRow(bizReportRowModel2);
				num += totalVerifData[i].IniBlance + totalVerifData[i].OpeningBlance;
				num2 += totalVerifData[i].TReceived;
				num3 += totalVerifData[i].TSpent;
				num4 += totalVerifData[i].FXGain;
				num5 += totalVerifData[i].ClosingBlance;
			}
			BizReportRowModel bizReportRowModel3 = new BizReportRowModel();
			bizReportRowModel3 = SetItemRow(ctx, 2, filter.IsShowDetail, text7, Convert.ToString(num), Convert.ToString(num2), Convert.ToString(num3), Convert.ToString(num4), Convert.ToString(num5), "", "");
			model.AddRow(bizReportRowModel3);
		}

		private static BizSubRptCreateModel GetDetailRptFilterModel(MContext ctx, RPTBankAndCashSummaryModel rowModel, RPTBankAndCashSummaryFilterBaseModel filter)
		{
			RPTBankAndCashSummaryFilterBaseModel rPTBankAndCashSummaryFilterBaseModel = new RPTBankAndCashSummaryFilterBaseModel();
			rPTBankAndCashSummaryFilterBaseModel.OpeningBlance = rowModel.OpeningBlance;
			rPTBankAndCashSummaryFilterBaseModel.MAccountID = rowModel.MBankAccountID;
			rPTBankAndCashSummaryFilterBaseModel.MAccountName = rowModel.MBankAccountName;
			rPTBankAndCashSummaryFilterBaseModel.MRate = rowModel.MCyRate;
			rPTBankAndCashSummaryFilterBaseModel.MCyID = rowModel.MCyID;
			rPTBankAndCashSummaryFilterBaseModel.MStartDate = filter.MStartDate;
			rPTBankAndCashSummaryFilterBaseModel.MEndDate = filter.MEndDate;
			rPTBankAndCashSummaryFilterBaseModel.ShowInUSD = true;
			BizSubRptCreateModel bizSubRptCreateModel = new BizSubRptCreateModel();
			bizSubRptCreateModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ShowDetail", "Show Detail");
			bizSubRptCreateModel.ReportType = BizReportType.CashSummary;
			bizSubRptCreateModel.ReportFilter = rPTBankAndCashSummaryFilterBaseModel;
			return bizSubRptCreateModel;
		}

		private static List<RPTBankAndCashSummaryModel> GetTotalVerifData(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter)
		{
			List<RPTBankAndCashSummaryModel> totalVerifDataByFilter = GetTotalVerifDataByFilter(ctx, GetBeginBalanceFilter(ctx, filter));
			List<RPTBankAndCashSummaryModel> totalVerifDataByFilter2 = GetTotalVerifDataByFilter(ctx, filter);
			if (totalVerifDataByFilter == null || totalVerifDataByFilter2 == null)
			{
				return totalVerifDataByFilter2;
			}
			List<string> balAccountIDList = (from t in totalVerifDataByFilter
			select t.MBankAccountID).ToList();
			List<string> normalAccountIDList = (from t in totalVerifDataByFilter2
			select t.MBankAccountID).ToList();
			List<RPTBankAndCashSummaryModel> list = new List<RPTBankAndCashSummaryModel>();
			List<RPTBankAndCashSummaryModel> list2 = (from p in totalVerifDataByFilter2
			join t in totalVerifDataByFilter on p.MBankAccountID equals t.MBankAccountID
			select new RPTBankAndCashSummaryModel
			{
				IniBlance = t.IniBlance + t.TReceived - t.TSpent,
				TReceived = p.TReceived,
				TSpent = p.TSpent,
				TSpentFor = p.TSpentFor,
				TReceivedFor = p.TReceivedFor,
				MBankAccountID = p.MBankAccountID,
				MBankAccountName = p.MBankAccountName,
				MCyID = p.MCyID
			}).ToList();
			List<RPTBankAndCashSummaryModel> list3 = (from t in totalVerifDataByFilter
			where !normalAccountIDList.Contains(t.MBankAccountID)
			select t).ToList();
			List<RPTBankAndCashSummaryModel> list4 = (from t in totalVerifDataByFilter2
			where !balAccountIDList.Contains(t.MBankAccountID)
			select t).ToList();
			if (list2 != null)
			{
				list.AddRange(list2);
			}
			if (list3 != null)
			{
				list.AddRange(list3);
			}
			if (list4 != null)
			{
				list.AddRange(list4);
			}
			return list;
		}

		private static List<RPTBankAndCashSummaryModel> GetTotalVerifDataByFilter(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter)
		{
			MySqlParameter[] cmdParms = null;
			string detailVerifDataSql = GetDetailVerifDataSql(ctx, filter, out cmdParms);
			detailVerifDataSql = $"SELECT MBankAccountName,MBankAccountID,MCyID, SUM(IniBlance) as IniBlance, SUM(Payment) as TSpent, SUM(Received) as TReceived , SUM(PaymentFor) as TSpentFor, SUM(ReceivedFor)  as TReceivedFor \r\n                                         FROM ({detailVerifDataSql}) p group by MBankAccountID,MBankAccountName,MCyID ";
			return ModelInfoManager.GetDataModelBySql<RPTBankAndCashSummaryModel>(ctx, detailVerifDataSql, cmdParms);
		}

		private static List<BankAndCashSummaryEntryModel> GetDetailVerifData(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter)
		{
			MySqlParameter[] cmdParms = null;
			string detailVerifDataSql = GetDetailVerifDataSql(ctx, filter, out cmdParms);
			detailVerifDataSql = $"SELECT * FROM ({detailVerifDataSql}) p order by MBizDate ";
			List<BankAndCashSummaryEntryModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BankAndCashSummaryEntryModel>(ctx, detailVerifDataSql, cmdParms);
			return SetMTypeLang(dataModelBySql, ctx);
		}

		private static BankAndCashSummaryEntryModel GetDetailVerifTotalData(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter)
		{
			MySqlParameter[] cmdParms = null;
			string detailVerifDataSql = GetDetailVerifDataSql(ctx, filter, out cmdParms);
			detailVerifDataSql = $"SELECT SUM(IniBlance) as IniBlance, SUM(Payment) as Payment, SUM(Received) as Received, SUM(PaymentFor) as PaymentFor, SUM(ReceivedFor)  as ReceivedFor \r\n                                         FROM ({detailVerifDataSql}) p";
			return ModelInfoManager.GetDataModel<BankAndCashSummaryEntryModel>(ctx, detailVerifDataSql, cmdParms);
		}

		private static bool IsFilterHasTrack(RPTBankAndCashSummaryFilterBaseModel filter)
		{
			if (!string.IsNullOrEmpty(filter.MTrackItem1) && !(filter.MTrackItem1 == "0") && !(filter.MTrackItem1 == "1"))
			{
				goto IL_00ff;
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem2) && !(filter.MTrackItem2 == "0") && !(filter.MTrackItem2 == "1"))
			{
				goto IL_00ff;
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem3) && !(filter.MTrackItem3 == "0") && !(filter.MTrackItem3 == "1"))
			{
				goto IL_00ff;
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem4) && !(filter.MTrackItem4 == "0") && !(filter.MTrackItem4 == "1"))
			{
				goto IL_00ff;
			}
			int num = (string.IsNullOrEmpty(filter.MTrackItem5) || filter.MTrackItem5 == "0" || filter.MTrackItem5 == "1") ? 1 : 0;
			goto IL_0100;
			IL_00ff:
			num = 0;
			goto IL_0100;
			IL_0100:
			if (num != 0)
			{
				return false;
			}
			return true;
		}

		private static string GetDetailVerifDataSql(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter, out MySqlParameter[] parameters)
		{
			DateTime dateTime = filter.MStartDate;
			filter.MStartDate = ((dateTime.Year <= 1900) ? new DateTime(1900, 1, 1) : filter.MStartDate);
			dateTime = filter.MEndDate;
			dateTime = dateTime.Date;
			dateTime = dateTime.AddDays(1.0);
			filter.MEndDate = dateTime.AddSeconds(-1.0);
			bool flag = IsFilterHasTrack(filter);
			StringBuilder stringBuilder = new StringBuilder();
			if (!flag)
			{
				stringBuilder.AppendFormat(" SELECT b.MID, '' AS MBizObject, b.MDate as MBizDate, '{0}' AS Transaction, ' ' AS MType,\r\n                ' ' AS Reference, b.MBeginBalance AS IniBlance, 0 AS Payment, 0 AS Received, 0 AS PaymentFor, 0 AS ReceivedFor, d.MCyID,d.MItemID AS MBankAccountID,c.MName AS MBankAccountName\r\n            FROM t_gl_initbankbalance b\r\n            INNER JOIN T_BD_BankAccount_l c ON b.MAccountID = c.MParentID  AND c.MLocaleID = @MLocaleID AND c.MIsDelete=0 \r\n            INNER JOIN T_BD_BankAccount d ON d.MItemID = c.MParentID AND d.MIsDelete=0\r\n            WHERE d.MOrgID = @MOrgID  AND b.MIsDelete=0  ", COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "OpeningBlance", "Opening Blance"));
				if (!string.IsNullOrWhiteSpace(filter.MAccountName))
				{
					stringBuilder.AppendLine(" and c.MName=@MAccountName ");
				}
				if (!string.IsNullOrWhiteSpace(filter.MAccountID))
				{
					stringBuilder.AppendLine(" and b.MAccountID=@MAccountId ");
				}
				stringBuilder.AppendLine(" UNION ALL ");
			}
			stringBuilder.AppendFormat("SELECT b.MID, 'Payment' AS MBizObject, b.MBizDate, IFNULL(convert(AES_DECRYPT(a.MName,'{0}') using utf8),F_GetUserName(e.MFirstName,e.MLastName)) AS Transaction, b.MType, \r\n            b.MReference AS Reference, 0 AS IniBlance, b.MTaxTotalAmt AS Payment, 0 AS Received, b.MTaxTotalAmtFor AS PaymentFor, 0 AS ReceivedFor, d.MCyID,d.MItemID AS MBankAccountID,c.MName AS MBankAccountName\r\n            FROM ({1}) b\r\n            INNER JOIN T_BD_BankAccount_l c ON b.MBankID = c.MParentID AND c.MLocaleID = @MLocaleID AND c.MIsDelete=0\r\n            INNER JOIN T_BD_BankAccount d ON d.MItemID = c.MParentID AND d.MIsDelete=0\r\n            LEFT JOIN T_BD_Contacts_l a ON b.MContactID = a.MParentID AND a.MLocaleID = @MLocaleID AND a.MIsDelete=0\r\n            LEFT JOIN T_BD_Employees_L e on b.MContactID=e.MParentID and e.MLocaleID=@MLocaleID AND e.MIsDelete=0\r\n            WHERE  d.MOrgID = @MOrgID ", "JieNor-001", GetPaymentTableSql(filter));
			if (!string.IsNullOrWhiteSpace(filter.MAccountName))
			{
				stringBuilder.AppendLine(" and c.MName=@MAccountName ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MAccountID))
			{
				stringBuilder.AppendLine(" and b.MBankID=@MAccountId ");
			}
			stringBuilder.AppendLine(" union all ");
			stringBuilder.AppendFormat("SELECT \r\n                b.MID,  'Receive' AS MBizObject, b.MBizDate,\r\n                 IFNULL(convert(AES_DECRYPT(a.MName,'{0}') using utf8),F_GetUserName(e.MFirstName,e.MLastName)) AS Transaction,\r\n                b.MType,\r\n                b.MReference AS Reference,\r\n                0 AS IniBlance,\r\n                0 AS Payment,\r\n                b.MTaxTotalAmt AS Received,\r\n                0 AS PaymentFor,\r\n                b.MTaxTotalAmtFor AS ReceivedFor,\r\n                d.MCyID,d.MItemID AS MBankAccountID,c.MName AS MBankAccountName\r\n            FROM  ({1}) b \r\n            INNER JOIN T_BD_BankAccount_l c ON b.MOrgID = c.MOrgID and  b.MBankID = c.MParentID AND c.MLocaleID = @MLocaleID AND c.MIsDelete=0\r\n            INNER JOIN T_BD_BankAccount d ON b.MOrgID = d.MOrgID and  d.MItemID = c.MParentID  AND d.MIsDelete=0\r\n            LEFT JOIN T_BD_Contacts_l a ON b.MOrgID = a.MOrgID and  b.MContactID = a.MParentID AND a.MLocaleID = @MLocaleID  AND a.MIsDelete=0\r\n            LEFT JOIN T_BD_Employees_L e on b.MOrgID = e.MOrgID and  b.MContactID=e.MParentID and e.MLocaleID=@MLocaleID AND e.MIsDelete=0\r\n            WHERE d.MOrgID = @MOrgID  ", "JieNor-001", GetReceiveTableSql(filter));
			if (!string.IsNullOrWhiteSpace(filter.MAccountName))
			{
				stringBuilder.AppendLine(" and c.MName=@MAccountName ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MAccountID))
			{
				stringBuilder.AppendLine(" and b.MBankID=@MAccountId ");
			}
			if (!flag)
			{
				stringBuilder.AppendLine(" UNION ALL ");
				stringBuilder.AppendLine("SELECT distinct b.MID,  'Transfer' AS MBizObject, b.MBizDate, c.MName AS Transaction, 'Transfer_Roll_Out' MType,\r\n                b.MReference AS Reference, 0 AS IniBlance, b.MFromTotalAmt AS Payment, 0 AS Received,b.MFromTotalAmtFor AS PaymentFor, 0 AS ReceivedFor,d.MCyID,d.MItemID AS MBankAccountID,c.MName AS MBankAccountName\r\n            FROM T_IV_Transfer b\r\n            INNER JOIN T_BD_BankAccount_l c ON b.MOrgID = c.MOrgID and  b.MFromAcctID = c.MParentID AND c.MLocaleID = @MLocaleID AND c.MIsDelete=0\r\n            INNER JOIN T_BD_BankAccount d ON b.MOrgID = d.MOrgID and  d.MItemID = c.MParentID AND d.MIsDelete=0\r\n            WHERE d.MOrgID = @MOrgID AND b.MIsDelete=0 AND b.MBizDate >= @MStartDate and b.MBizDate <= @MEndDate");
				if (!string.IsNullOrWhiteSpace(filter.MAccountName))
				{
					stringBuilder.AppendLine(" and c.MName=@MAccountName ");
				}
				if (!string.IsNullOrWhiteSpace(filter.MAccountID))
				{
					stringBuilder.AppendLine(" and b.MFromAcctID=@MAccountId ");
				}
				stringBuilder.AppendLine(" union all ");
				stringBuilder.AppendLine("SELECT  distinct b.MID,'Transfer' AS MBizObject,  b.MBizDate, c.MName AS Transaction,  'Transfer_Roll_In' MType,\r\n                b.MReference AS Reference,  0 AS IniBlance, 0 AS Payment, b.MToTotalAmt AS Received, 0 AS PaymentFor, b.MToTotalAmtFor AS ReceivedFor, d.MCyID,d.MItemID AS MBankAccountID,c.MName AS MBankAccountName\r\n            FROM T_IV_Transfer b \r\n            INNER JOIN T_BD_BankAccount_l c ON b.MOrgID = c.MOrgID and  b.MToAcctID = c.MParentID AND c.MLocaleID = @MLocaleID AND c.MIsDelete=0\r\n            INNER JOIN T_BD_BankAccount d ON  b.MOrgID = d.MOrgID and d.MItemID = c.MParentID AND d.MIsDelete=0\r\n            WHERE d.MOrgID = @MOrgID AND b.MIsDelete=0 AND b.MBizDate >= @MStartDate and b.MBizDate <= @MEndDate");
				if (!string.IsNullOrWhiteSpace(filter.MAccountName))
				{
					stringBuilder.AppendLine(" and c.MName=@MAccountName ");
				}
				if (!string.IsNullOrWhiteSpace(filter.MAccountID))
				{
					stringBuilder.AppendLine(" and b.MToAcctID=@MAccountId ");
				}
			}
			parameters = new MySqlParameter[11]
			{
				new MySqlParameter("@MAccountName", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MAccountId", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStartDate", MySqlDbType.DateTime, 36),
				new MySqlParameter("@MEndDate", MySqlDbType.DateTime, 36),
				new MySqlParameter("@MTrackItem1", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTrackItem2", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTrackItem3", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTrackItem4", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTrackItem5", MySqlDbType.VarChar, 36)
			};
			parameters[0].Value = filter.MAccountName;
			parameters[1].Value = filter.MAccountID;
			parameters[2].Value = ctx.MLCID;
			parameters[3].Value = ctx.MOrgID;
			parameters[4].Value = filter.MStartDate;
			parameters[5].Value = filter.MEndDate;
			parameters[6].Value = filter.MTrackItem1;
			parameters[7].Value = filter.MTrackItem2;
			parameters[8].Value = filter.MTrackItem3;
			parameters[9].Value = filter.MTrackItem4;
			parameters[10].Value = filter.MTrackItem5;
			return stringBuilder.ToString();
		}

		private static string GetPaymentTableSql(RPTBankAndCashSummaryFilterBaseModel filter)
		{
			return $"SELECT m.MID,m.MOrgID,m.MBankID,m.MBizDate,m.MContactID,m.MType,m.MReference,SUM(n.MTaxAmount) AS MTaxTotalAmt,SUM(n.MTaxAmountFor) AS MTaxTotalAmtFor FROM T_IV_Payment m\n                            INNER JOIN T_IV_PaymentEntry n ON m.MOrgID = n.MOrgID and  m.MID=n.MID AND n.MIsDelete=0\n                            WHERE m.MOrgID = @MOrgID AND m.MIsDelete=0 and m.MBizDate >= @MStartDate and m.MBizDate <= @MEndDate {GetTrackingFilterString(filter)}\n                            GROUP BY m.MID";
		}

		private static string GetReceiveTableSql(RPTBankAndCashSummaryFilterBaseModel filter)
		{
			return $"SELECT m.MID,m.MOrgID,m.MBankID,m.MBizDate,m.MContactID,m.MType,m.MReference,SUM(n.MTaxAmount) AS MTaxTotalAmt,SUM(n.MTaxAmountFor) AS MTaxTotalAmtFor FROM T_IV_Receive m\n                            INNER JOIN T_IV_ReceiveEntry n ON  m.MOrgID = n.MOrgID and  m.MID=n.MID AND n.MIsDelete=0\n                            WHERE m.MOrgID = @MOrgID AND m.MIsDelete=0 and m.MBizDate >= @MStartDate and m.MBizDate <= @MEndDate {GetTrackingFilterString(filter)}\n                            GROUP BY m.MID";
		}

		private static string GetTrackingFilterString(RPTBankAndCashSummaryFilterBaseModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetTrackingFilterString("MTrackItem1", filter.MTrackItem1));
			stringBuilder.Append(GetTrackingFilterString("MTrackItem2", filter.MTrackItem2));
			stringBuilder.Append(GetTrackingFilterString("MTrackItem3", filter.MTrackItem3));
			stringBuilder.Append(GetTrackingFilterString("MTrackItem4", filter.MTrackItem4));
			stringBuilder.Append(GetTrackingFilterString("MTrackItem5", filter.MTrackItem5));
			return stringBuilder.ToString();
		}

		private static string GetTrackingFilterString(string columnName, string trackingId)
		{
			if (string.IsNullOrEmpty(trackingId) || trackingId == "0")
			{
				return "";
			}
			if (trackingId == "1")
			{
				return $"AND IFNULL({columnName},'')='' ";
			}
			return string.Format("AND {0}=@{0} ", columnName);
		}

		private static List<BankAndCashSummaryEntryModel> SetMTypeLang(List<BankAndCashSummaryEntryModel> list, MContext ctx)
		{
			list.ForEach(delegate(BankAndCashSummaryEntryModel x)
			{
				if (!string.IsNullOrWhiteSpace(x.MType))
				{
					x.MType = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, x.MType, x.MType);
				}
			});
			return list;
		}

		private static void SetRate(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter, List<RPTBankAndCashSummaryModel> detailData)
		{
			string currnecyID = GetCurrnecyID(ctx);
			Dictionary<string, decimal> baseCurrencyRate = new REGCurrencyRepository().GetBaseCurrencyRate(ctx, filter.MEndDate);
			foreach (RPTBankAndCashSummaryModel detailDatum in detailData)
			{
				if (detailDatum.MCyID == currnecyID || detailDatum.MCyID == null)
				{
					detailDatum.MCyRate = decimal.One;
				}
				else if (baseCurrencyRate.ContainsKey(detailDatum.MCyID))
				{
					detailDatum.MCyRate = baseCurrencyRate[detailDatum.MCyID];
				}
			}
		}

		private static string GetCurrnecyID(MContext ctx)
		{
			string sql = "select MCurrencyID from T_REG_Financial Where MOrgID=@MOrgID AND MIsDelete=0 ";
			MySqlParameter mySqlParameter = new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36);
			mySqlParameter.Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, mySqlParameter);
			if (single != null)
			{
				return single.ToString();
			}
			return "CNY";
		}

		private static void SetEntryRate(MContext ctx, RPTBankAndCashSummaryFilterBaseModel filter, List<BankAndCashSummaryEntryModel> detailData)
		{
			string currnecyID = GetCurrnecyID(ctx);
			Dictionary<string, decimal> baseCurrencyRate = new REGCurrencyRepository().GetBaseCurrencyRate(ctx, filter.MEndDate);
			foreach (BankAndCashSummaryEntryModel detailDatum in detailData)
			{
				if (detailDatum.MCyID == currnecyID || detailDatum.MCyID == null)
				{
					detailDatum.MCyRate = decimal.One;
				}
				else if (baseCurrencyRate.ContainsKey(detailDatum.MCyID))
				{
					detailDatum.MCyRate = baseCurrencyRate[detailDatum.MCyID];
					filter.MRate = detailDatum.MCyRate;
					filter.MCyID = detailDatum.MCyID;
				}
			}
		}
	}
}
