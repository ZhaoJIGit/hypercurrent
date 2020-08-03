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
	public class RPTCashFlowStatementBusiness : RPTGLBaseService, IRPTCashFlowStatementBusiness, IRPTBizReportBusiness<RPTCashFlowStatementFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTCashFlowStatementFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => GetBizReportModel(ctx, filter));
		}

		private BizReportModel GetBizReportModel(MContext ctx, RPTCashFlowStatementFilterModel filter)
		{
			if (filter.MToDate < filter.MFromDate)
			{
				string mFromDateString = filter.MFromDateString;
				filter.MFromDateString = filter.MToDateString;
				filter.MToDateString = mFromDateString;
			}
			List<RPTReportConfigModel> configList = GetConfigList(ctx, filter);
			if (ctx.MAccountTableID == "1")
			{
				RPTReportConfigModel rPTReportConfigModel = configList.FirstOrDefault((RPTReportConfigModel t) => t.MItemID == "1_103_40");
				if (rPTReportConfigModel != null)
				{
					RPTIncomeStatementBusiness rPTIncomeStatementBusiness = new RPTIncomeStatementBusiness();
					RPTIncomeStatementBusiness rPTIncomeStatementBusiness2 = rPTIncomeStatementBusiness;
					RPTIncomeStatementFilterModel filter2 = new RPTIncomeStatementFilterModel
					{
						MType = filter.MType,
						MFromDateString = filter.MFromDateString,
						MToDateString = filter.MToDateString
					};
					decimal num = rPTReportConfigModel.MAmount = rPTIncomeStatementBusiness2.GetNetProfits(ctx, filter2);
				}
			}
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.IncomeStatement);
			SetTitle(ctx, filter, bizReportModel);
			SetHeaderRow(ctx, filter, bizReportModel);
			SetItemRow(configList, bizReportModel);
			SetEmptyRow(bizReportModel);
			if (ctx.MAccountTableID == "1")
			{
				SetAdditionalHeaderRow(ctx, filter, bizReportModel);
				SetAdditionalItemRow(configList, bizReportModel);
			}
			return bizReportModel;
		}

		private void SetEmptyRow(BizReportModel model)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Item;
			bizReportRowModel.AddCell(new BizReportCellModel());
			bizReportRowModel.AddCell(new BizReportCellModel());
			bizReportRowModel.AddCell(new BizReportCellModel());
			model.AddRow(bizReportRowModel);
		}

		private void SetTitle(MContext ctx, RPTCashFlowStatementFilterModel filter, BizReportModel model)
		{
			model.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "CashFlowStatement", "Cash Flow Statement");
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

		private void SetHeaderRow(MContext ctx, RPTCashFlowStatementFilterModel filter, BizReportModel model)
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
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = base.GetDateHeadTitle(ctx, filter.MFromDate),
					CellType = BizReportCellType.Money
				});
			}
			else
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = base.GetDateHeadTitle(ctx, filter.MFromDate, filter.MToDate, true),
					CellType = BizReportCellType.Money
				});
			}
			model.AddRow(bizReportRowModel);
		}

		private List<RPTReportConfigModel> GetConfigList(MContext ctx, RPTCashFlowStatementFilterModel filter)
		{
			List<RPTReportConfigModel> configList = base.GetConfigList(ctx, RPTGLReportType.CashFlowStatement);
			List<RPTGLAccountModel> cashFlowStatementList = RPTCashFlowStatementRepository.GetCashFlowStatementList(filter, ctx);
			ResetDataByBalanceSheet(ctx, filter, cashFlowStatementList);
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountListModel> bDAccountList = bDAccountRepository.GetBDAccountList(ctx, "");
			base.ResetData(ctx, cashFlowStatementList, bDAccountList, filter.MFromDate, filter.MToDate);
			return base.ConvertAmount(ctx, configList, cashFlowStatementList);
		}

		private void ResetDataByIncomeStatement()
		{
		}

		private void ResetDataByBalanceSheet(MContext ctx, RPTCashFlowStatementFilterModel filter, List<RPTGLAccountModel> periodAcct)
		{
			if (!(ctx.MAccountTableID == "2") && periodAcct != null && periodAcct.Count != 0)
			{
				RPTBalanceSheetBusiness rPTBalanceSheetBusiness = new RPTBalanceSheetBusiness();
				RPTBalanceSheetBusiness rPTBalanceSheetBusiness2 = rPTBalanceSheetBusiness;
				RPTBalanceSheetFilterModel rPTBalanceSheetFilterModel = new RPTBalanceSheetFilterModel();
				DateTime dateTime = filter.MToDate;
				rPTBalanceSheetFilterModel.MDateString = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
				List<RPTReportConfigModel> endBalReportConfigData = rPTBalanceSheetBusiness2.GetEndBalReportConfigData(ctx, rPTBalanceSheetFilterModel);
				List<RPTReportConfigModel> list = new List<RPTReportConfigModel>();
				RPTBalanceSheetFilterModel rPTBalanceSheetFilterModel2 = new RPTBalanceSheetFilterModel();
				dateTime = filter.MFromDateString.ToMDateTime();
				dateTime = dateTime.AddMonths(-1);
				rPTBalanceSheetFilterModel2.MDateString = dateTime.ToString("yyyy-MM-dd");
				RPTBalanceSheetFilterModel rPTBalanceSheetFilterModel3 = rPTBalanceSheetFilterModel2;
				if (rPTBalanceSheetFilterModel3.MDate < ctx.MGLBeginDate)
				{
					List<RPTGLAccountModel> initBalanceList = rPTBalanceSheetBusiness.GetInitBalanceList(rPTBalanceSheetFilterModel3, ctx);
					RPTGLAccountModel rPTGLAccountModel = initBalanceList.FirstOrDefault((RPTGLAccountModel t) => t.MAccountCode == "1122");
					RPTGLAccountModel rPTGLAccountModel2 = initBalanceList.FirstOrDefault((RPTGLAccountModel t) => t.MAccountCode == "1231");
					RPTGLAccountModel rPTGLAccountModel3 = initBalanceList.FirstOrDefault((RPTGLAccountModel t) => t.MAccountCode == "2203");
					RPTGLAccountModel rPTGLAccountModel4 = initBalanceList.FirstOrDefault((RPTGLAccountModel t) => t.MAccountCode == "2202");
					RPTGLAccountModel rPTGLAccountModel5 = initBalanceList.FirstOrDefault((RPTGLAccountModel t) => t.MAccountCode == "1123");
					RPTReportConfigModel rPTReportConfigModel = new RPTReportConfigModel();
					rPTReportConfigModel.MItemID = "1_102_5";
					decimal num = default(decimal);
					decimal num2 = default(decimal);
					decimal num3 = default(decimal);
					decimal num4 = default(decimal);
					if (rPTGLAccountModel != null)
					{
						num = rPTGLAccountModel.MBeginBalAmt;
					}
					if (rPTGLAccountModel2 != null)
					{
						num -= rPTGLAccountModel2.MBeginBalAmt;
					}
					if (rPTGLAccountModel3 != null)
					{
						num2 = rPTGLAccountModel3.MBeginBalAmt;
					}
					if (rPTGLAccountModel5 != null)
					{
						num4 = rPTGLAccountModel5.MBeginBalAmt;
					}
					RPTReportConfigModel rPTReportConfigModel2 = new RPTReportConfigModel();
					rPTReportConfigModel2.MItemID = "1_102_39";
					if (rPTGLAccountModel4 != null)
					{
						num3 = rPTGLAccountModel4.MBeginBalAmt;
					}
					if (num > decimal.Zero)
					{
						rPTReportConfigModel.MAmount = num;
					}
					else
					{
						num = default(decimal);
					}
					if (num2 < decimal.Zero)
					{
						rPTReportConfigModel.MAmount = num - num2;
					}
					if (num3 > decimal.Zero)
					{
						rPTReportConfigModel2.MAmount = num3;
					}
					else
					{
						rPTReportConfigModel2.MAmount = decimal.Zero;
					}
					if (num4 < decimal.Zero)
					{
						rPTReportConfigModel2.MAmount -= num4;
					}
					list.Add(rPTReportConfigModel);
					list.Add(rPTReportConfigModel2);
				}
				else
				{
					list = rPTBalanceSheetBusiness.GetEndBalReportConfigData(ctx, rPTBalanceSheetFilterModel3);
				}
				RPTGLAccountModel rPTGLAccountModel6 = (from t in periodAcct
				where t.MAccountCode == "1122"
				select t).FirstOrDefault();
				RPTGLAccountModel rPTGLAccountModel7 = (from t in periodAcct
				where t.MAccountCode == "2202"
				select t).FirstOrDefault();
				if (rPTGLAccountModel6 != null)
				{
					RPTGLAccountModel rPTGLAccountModel8 = new RPTGLAccountModel
					{
						MAccountCode = "BS1122",
						MDC = rPTGLAccountModel6.MDC
					};
					RPTReportConfigModel rPTReportConfigModel3 = list.FirstOrDefault((RPTReportConfigModel t) => t.MItemID == "1_102_5");
					if (rPTReportConfigModel3 != null)
					{
						rPTGLAccountModel8.MBeginBalAmt = rPTReportConfigModel3.MAmount;
					}
					rPTReportConfigModel3 = endBalReportConfigData.FirstOrDefault((RPTReportConfigModel t) => t.MItemID == "1_102_5");
					if (rPTReportConfigModel3 != null)
					{
						rPTGLAccountModel8.MDebitAmt = rPTReportConfigModel3.MAmount - rPTGLAccountModel8.MBeginBalAmt;
					}
					rPTGLAccountModel8.MEndBalAmt = rPTGLAccountModel8.MBeginBalAmt + rPTGLAccountModel8.MDebitAmt;
					periodAcct.Add(rPTGLAccountModel8);
				}
				if (rPTGLAccountModel7 != null)
				{
					RPTGLAccountModel rPTGLAccountModel9 = new RPTGLAccountModel
					{
						MAccountCode = "BS2202",
						MDC = rPTGLAccountModel7.MDC
					};
					RPTReportConfigModel rPTReportConfigModel4 = list.FirstOrDefault((RPTReportConfigModel t) => t.MItemID == "1_102_39");
					if (rPTReportConfigModel4 != null)
					{
						rPTGLAccountModel9.MBeginBalAmt = rPTReportConfigModel4.MAmount;
					}
					rPTReportConfigModel4 = endBalReportConfigData.FirstOrDefault((RPTReportConfigModel t) => t.MItemID == "1_102_39");
					if (rPTReportConfigModel4 != null)
					{
						rPTGLAccountModel9.MCreditAmt = rPTReportConfigModel4.MAmount - rPTGLAccountModel9.MBeginBalAmt;
					}
					rPTGLAccountModel9.MEndBalAmt = rPTGLAccountModel9.MBeginBalAmt + rPTGLAccountModel9.MCreditAmt;
					periodAcct.Add(rPTGLAccountModel9);
				}
			}
		}

		private void SetItemRow(List<RPTReportConfigModel> configList, BizReportModel model)
		{
			configList = (from t in configList
			where string.IsNullOrEmpty(t.MTag)
			orderby t.MSequence
			select t).ToList();
			if (configList != null)
			{
				foreach (RPTReportConfigModel config in configList)
				{
					BizReportRowModel rowModel = GetRowModel(config);
					model.AddRow(rowModel);
				}
			}
		}

		private void SetAdditionalHeaderRow(MContext ctx, RPTCashFlowStatementFilterModel filter, BizReportModel model)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Additional", "Additional"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Number", "Number"),
				CellType = BizReportCellType.Text
			});
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
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = base.GetDateHeadTitle(ctx, filter.MFromDate),
					CellType = BizReportCellType.Money
				});
			}
			else
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = base.GetDateHeadTitle(ctx, filter.MFromDate, filter.MToDate, false),
					CellType = BizReportCellType.Money
				});
			}
			model.AddRow(bizReportRowModel);
		}

		private void SetAdditionalItemRow(List<RPTReportConfigModel> configList, BizReportModel model)
		{
			configList = (from t in configList
			where !string.IsNullOrEmpty(t.MTag)
			orderby t.MSequence
			select t).ToList();
			if (configList != null)
			{
				foreach (RPTReportConfigModel config in configList)
				{
					BizReportRowModel rowModel = GetRowModel(config);
					model.AddRow(rowModel);
				}
			}
		}

		private BizReportRowModel GetRowModel(RPTReportConfigModel config)
		{
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
			}
			else
			{
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
