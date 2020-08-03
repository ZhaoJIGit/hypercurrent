using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTIncomeStatementBusiness : RPTGLBaseService, IRPTIncomeStatementBusiness, IRPTBizReportBusiness<RPTIncomeStatementFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTIncomeStatementFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => GetBizReportModel(ctx, filter));
		}

		private BizReportModel GetBizReportModel(MContext ctx, RPTIncomeStatementFilterModel filter)
		{
			if (filter.MToDate < filter.MFromDate)
			{
				string mFromDateString = filter.MFromDateString;
				filter.MFromDateString = filter.MToDateString;
				filter.MToDateString = mFromDateString;
			}
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.IncomeStatement);
			SetTitle(ctx, filter, bizReportModel);
			SetHeaderRow(ctx, filter, bizReportModel);
			SetItemRow(ctx, filter, bizReportModel);
			return bizReportModel;
		}

		private void SetTitle(MContext ctx, RPTIncomeStatementFilterModel filter, BizReportModel model)
		{
			model.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "IncomeStatement", "Income Statement");
			model.Title2 = ctx.MOrgName;
			DateTime dateTime = filter.MFromDate;
			int year = dateTime.Year;
			dateTime = filter.MToDate;
			int num;
			if (year == dateTime.Year)
			{
				dateTime = filter.MFromDate;
				int month = dateTime.Month;
				dateTime = filter.MToDate;
				num = ((month == dateTime.Month) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			if (num != 0)
			{
				model.Title3 = base.GetDateHeadTitle(ctx, filter.MFromDate);
			}
			else
			{
				model.Title3 = base.GetDateHeadTitle(ctx, filter.MFromDate, filter.MToDate, false);
			}
		}

		private void SetHeaderRow(MContext ctx, RPTIncomeStatementFilterModel filter, BizReportModel model)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Project", "Project"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Number", "Number"),
				CellType = BizReportCellType.Text
			});
			DateTime dateTime;
			if (ctx.MAccountTableID == "2" && filter.MCompareSpan == 0)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "YTDTotal", "YTD"),
					CellType = BizReportCellType.Money
				});
			}
			else if (ctx.MAccountTableID == "2" && filter.MCompareSpan == 1)
			{
				int mCompareSpan = filter.MCompareSpan;
				dateTime = filter.MFromDate;
				DateTime startDate = dateTime.AddYears(-1);
				dateTime = filter.MToDate;
				BizReportCellModel amountTitle = GetAmountTitle(ctx, mCompareSpan, false, startDate, dateTime.AddYears(-1));
				bizReportRowModel.AddCell(amountTitle);
			}
			BizReportCellModel amountTitle2 = GetAmountTitle(ctx, filter.MCompareSpan, true, filter.MFromDate, filter.MToDate);
			bizReportRowModel.AddCell(amountTitle2);
			if (ctx.MAccountTableID != "2" && filter.MCompareSpan == 0)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "YTDTotal", "YTD"),
					CellType = BizReportCellType.Money
				});
			}
			else if (ctx.MAccountTableID != "2" && filter.MCompareSpan == 1)
			{
				int mCompareSpan2 = filter.MCompareSpan;
				dateTime = filter.MFromDate;
				DateTime startDate2 = dateTime.AddYears(-1);
				dateTime = filter.MToDate;
				BizReportCellModel amountTitle3 = GetAmountTitle(ctx, mCompareSpan2, false, startDate2, dateTime.AddYears(-1));
				bizReportRowModel.AddCell(amountTitle3);
			}
			model.AddRow(bizReportRowModel);
		}

		private BizReportCellModel GetAmountTitle(MContext ctx, int compareType, bool isCurrentPeriod, DateTime startDate, DateTime endDate)
		{
			BizReportCellModel bizReportCellModel = new BizReportCellModel();
			bizReportCellModel.CellType = BizReportCellType.Money;
			if (startDate.Year == endDate.Year && startDate.Month == endDate.Month)
			{
				if (startDate.Month == ctx.DateNow.Month)
				{
					bizReportCellModel.Value = (isCurrentPeriod ? base.GetCurrentDateHeadTitle(ctx) : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "LastPeriodAmount", "上期金额"));
				}
				else
				{
					bizReportCellModel.Value = base.GetDateHeadTitle(ctx, startDate);
				}
			}
			else
			{
				bizReportCellModel.Value = base.GetDateHeadTitle(ctx, startDate, endDate, true);
			}
			return bizReportCellModel;
		}

		private void SetItemRow(MContext ctx, RPTIncomeStatementFilterModel filter, BizReportModel model)
		{
			List<RPTReportConfigModel> list = null;
			List<RPTReportConfigModel> list2 = null;
			List<RPTReportConfigModel> list3 = null;
			SetData(ctx, filter, out list, out list2, out list3);
			if (list != null && list2 != null && list3 != null)
			{
				foreach (RPTReportConfigModel item in list3)
				{
					BizReportRowModel rowModel = GetRowModel(ctx, item, list, list2);
					model.AddRow(rowModel);
				}
			}
		}

		private void SetData(MContext ctx, RPTIncomeStatementFilterModel filter, out List<RPTReportConfigModel> yearList, out List<RPTReportConfigModel> periodList, out List<RPTReportConfigModel> mainYearList)
		{
			List<RPTReportConfigModel> configList = base.GetConfigList(ctx, RPTGLReportType.IncomeStatement);
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountListModel> bDAccountList = bDAccountRepository.GetBDAccountList(ctx, "");
			if (filter.MType == "Monthly")
			{
				filter.MToDateString = filter.MFromDateString;
			}
			List<RPTGLAccountModel> list = null;
			RPTIncomeStatementFilterModel rPTIncomeStatementFilterModel = new RPTIncomeStatementFilterModel();
			DateTime dateTime;
			if (filter.MCompareSpan == 1)
			{
				DateTime mFromDate = filter.MFromDate;
				dateTime = ctx.MGLBeginDate;
				if (mFromDate < dateTime.AddYears(1))
				{
					dateTime = ctx.MGLBeginDate;
					dateTime = dateTime.AddYears(1);
					filter.MFromDateString = dateTime.ToString();
				}
				if (filter.MType == "Monthly")
				{
					filter.MToDateString = filter.MFromDateString;
				}
				RPTIncomeStatementFilterModel rPTIncomeStatementFilterModel2 = rPTIncomeStatementFilterModel;
				dateTime = filter.MFromDate;
				dateTime = dateTime.AddYears(-1);
				rPTIncomeStatementFilterModel2.MFromDateString = dateTime.ToString();
				if (filter.MType == "Monthly")
				{
					rPTIncomeStatementFilterModel.MToDateString = rPTIncomeStatementFilterModel.MFromDateString;
				}
				else
				{
					RPTIncomeStatementFilterModel rPTIncomeStatementFilterModel3 = rPTIncomeStatementFilterModel;
					dateTime = Convert.ToDateTime(filter.MToDateString);
					dateTime = dateTime.Date;
					dateTime = dateTime.AddYears(-1);
					rPTIncomeStatementFilterModel3.MToDateString = dateTime.ToString();
				}
				list = RPTIncomeStatementRepository.GetIncomeStatementList(rPTIncomeStatementFilterModel, ctx);
			}
			else
			{
				dateTime = filter.MToDate;
				int year = dateTime.Year;
				dateTime = ctx.MGLBeginDate;
				list = ((year != dateTime.Year) ? RPTIncomeStatementRepository.GetYearIncomeStatementList(filter, ctx) : RPTIncomeStatementRepository.GetYearIncomeStatementListByConversionYear(filter, ctx));
			}
			if (filter.MCompareSpan == 1)
			{
				base.ResetData(ctx, list, bDAccountList, rPTIncomeStatementFilterModel.MFromDate, rPTIncomeStatementFilterModel.MToDate);
			}
			else
			{
				List<RPTGLAccountModel> list2 = list;
				List<BDAccountListModel> acctList = bDAccountList;
				dateTime = filter.MToDate;
				base.ResetData(ctx, list2, acctList, new DateTime(dateTime.Year, 1, 1), filter.MToDate);
			}
			List<RPTGLAccountModel> incomeStatementList = RPTIncomeStatementRepository.GetIncomeStatementList(filter, ctx);
			base.ResetData(ctx, incomeStatementList, bDAccountList, filter.MFromDate, filter.MToDate);
			yearList = base.ConvertAmount(ctx, configList, list);
			periodList = base.ConvertAmount(ctx, configList, incomeStatementList);
			mainYearList = null;
			if (yearList != null && periodList != null)
			{
				mainYearList = (from t in yearList
				where string.IsNullOrEmpty(t.MParentID)
				orderby t.MSequence
				select t).ToList();
			}
		}

		public decimal GetNetProfits(MContext ctx, RPTIncomeStatementFilterModel filter)
		{
			List<RPTReportConfigModel> list = null;
			List<RPTReportConfigModel> list2 = null;
			List<RPTReportConfigModel> list3 = null;
			SetData(ctx, filter, out list, out list2, out list3);
			if (list == null || list2 == null || list3 == null)
			{
				return decimal.Zero;
			}
			string netProfitsId = "";
			if (ctx.MAccountTableID == "1")
			{
				netProfitsId = "1_101_32";
			}
			else
			{
				netProfitsId = "2_101_32";
			}
			RPTReportConfigModel rPTReportConfigModel = list2.FirstOrDefault((RPTReportConfigModel t) => t.MItemID == netProfitsId);
			return rPTReportConfigModel?.MAmount ?? decimal.Zero;
		}

		private BizReportRowModel GetRowModel(MContext ctx, RPTReportConfigModel config, List<RPTReportConfigModel> yearList, List<RPTReportConfigModel> periodList)
		{
			BizReportRowModel rowModel = GetRowModel(ctx, config, periodList);
			List<RPTReportConfigModel> list = (from t in yearList
			where t.MParentID == config.MItemID
			orderby t.MSequence
			select t).ToList();
			if (list != null && list.Count > 0)
			{
				foreach (RPTReportConfigModel item in list)
				{
					BizReportRowModel rowModel2 = GetRowModel(ctx, item, yearList, periodList);
					rowModel.SubRows.Add(rowModel2);
				}
			}
			return rowModel;
		}

		private BizReportRowModel GetRowModel(MContext ctx, RPTReportConfigModel config, List<RPTReportConfigModel> periodList)
		{
			RPTReportConfigModel rPTReportConfigModel = (from t in periodList
			where t.MItemID == config.MItemID
			select t).FirstOrDefault();
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.UniqueValue = config.MItemID;
			bizReportRowModel.RowType = config.MReportRowType;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = config.MName,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = config.MNumber.ToString(),
				CellType = BizReportCellType.Text
			});
			if (string.IsNullOrEmpty(config.MNumber))
			{
				bizReportRowModel.AddCell(new BizReportCellModel());
				bizReportRowModel.AddCell(new BizReportCellModel());
			}
			else if (ctx.MAccountTableID == "2")
			{
				bizReportRowModel.AddCell(GetCellModel(config.MAmount));
				bizReportRowModel.AddCell(GetCellModel(rPTReportConfigModel.MAmount));
			}
			else
			{
				bizReportRowModel.AddCell(GetCellModel(rPTReportConfigModel.MAmount));
				bizReportRowModel.AddCell(GetCellModel(config.MAmount));
			}
			return bizReportRowModel;
		}

		private BizReportCellModel GetCellModel(decimal amount)
		{
			if (amount == decimal.Zero)
			{
				return new BizReportCellModel();
			}
			return new BizReportCellModel
			{
				Value = amount.ToString(),
				CellType = BizReportCellType.Money
			};
		}
	}
}
