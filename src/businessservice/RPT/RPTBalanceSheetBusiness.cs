using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTBalanceSheetBusiness : RPTGLBaseService, IRPTBalanceSheetBusiness, IRPTBizReportBusiness<RPTBalanceSheetFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTBalanceSheetFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, delegate
			{
				IsBalance(ctx, filter.MDate);
				return GetBizReportModel(ctx, filter);
			});
		}

		private BizReportModel GetBizReportModel(MContext ctx, RPTBalanceSheetFilterModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.BalanceSheet);
			SetTitle(ctx, filter, bizReportModel);
			SetHeaderRow(ctx, filter, bizReportModel);
			SetItemRow(ctx, filter, bizReportModel);
			return bizReportModel;
		}

		private void SetTitle(MContext ctx, RPTBalanceSheetFilterModel filter, BizReportModel model)
		{
			model.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BalanceSheet", "Balance Sheet");
			model.Title2 = ctx.MOrgName;
			model.Title3 = base.GetDateHeadTitle(ctx, filter.MDate);
		}

		private void SetHeaderRow(MContext ctx, RPTBalanceSheetFilterModel filter, BizReportModel model)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "RptBalance", "Balance"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Number", "Number"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "RptEndingBalance", "Ending balance"),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "RptBeginningBalance", "Beginning balance"),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "RptLiabilityAndEquity", "Liability and equity"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Number", "Number"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "RptEndingBalance", "Ending balance"),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "RptBeginningBalance", "Beginning balance"),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private void SetItemRow(MContext ctx, RPTBalanceSheetFilterModel filter, BizReportModel model)
		{
			List<RPTReportConfigModel> configList = base.GetConfigList(ctx, RPTGLReportType.BalanceSheet);
			List<RPTGLAccountModel> endBalanceList = RPTBalanceSheetRepository.GetEndBalanceList(filter, ctx);
			List<RPTGLAccountModel> list = null;
			DateTime dateTime = filter.MDate;
			int year = dateTime.Year;
			dateTime = ctx.MGLBeginDate;
			list = ((year != dateTime.Year) ? RPTBalanceSheetRepository.GetBeginBalanceListForOtherYear(filter, ctx) : RPTBalanceSheetRepository.GetBeginBalanceList(filter, ctx));
			List<RPTReportConfigModel> list2 = base.ConvertAmount(ctx, configList, endBalanceList);
			List<RPTReportConfigModel> beginBalList = base.ConvertAmount(ctx, configList, list);
			List<RPTReportConfigModel> list3 = (from t in list2
			where string.IsNullOrEmpty(t.MParentID) && string.IsNullOrEmpty(t.MTag)
			orderby t.MSequence
			select t).ToList();
			foreach (RPTReportConfigModel item in list3)
			{
				BizReportRowModel rowModel = GetRowModel(ctx, item, list2, beginBalList);
				model.AddRow(rowModel);
			}
		}

		private BizReportRowModel GetRowModel(MContext ctx, RPTReportConfigModel config, List<RPTReportConfigModel> endBalList, List<RPTReportConfigModel> beginBalList)
		{
			BizReportRowModel rowModelWithTag = GetRowModelWithTag(ctx, config, endBalList, beginBalList);
			List<RPTReportConfigModel> list = (from t in endBalList
			where t.MParentID == config.MItemID
			orderby t.MSequence
			select t).ToList();
			if (list != null && list.Count > 0)
			{
				foreach (RPTReportConfigModel item in list)
				{
					BizReportRowModel rowModel = GetRowModel(ctx, item, endBalList, beginBalList);
					rowModelWithTag.SubRows.Add(rowModel);
				}
			}
			return rowModelWithTag;
		}

		private BizReportRowModel GetRowModelWithTag(MContext ctx, RPTReportConfigModel config, List<RPTReportConfigModel> endBalList, List<RPTReportConfigModel> beginBalList)
		{
			RPTReportConfigModel rPTReportConfigModel = (from t in beginBalList
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
			else
			{
				bizReportRowModel.AddCell(GetCellModel(config.MAmount));
				bizReportRowModel.AddCell(GetCellModel(rPTReportConfigModel.MAmount));
			}
			RPTReportConfigModel rPTReportConfigModel2 = (from t in endBalList
			where t.MTag == config.MItemID
			select t).FirstOrDefault();
			RPTReportConfigModel rPTReportConfigModel3 = (from t in beginBalList
			where t.MTag == config.MItemID
			select t).FirstOrDefault();
			if (rPTReportConfigModel2 != null && rPTReportConfigModel3 != null)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = rPTReportConfigModel2.MName,
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = rPTReportConfigModel2.MNumber.ToString(),
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(GetCellModel(rPTReportConfigModel2.MAmount));
				bizReportRowModel.AddCell(GetCellModel(rPTReportConfigModel3.MAmount));
			}
			else
			{
				bizReportRowModel.AddCell(new BizReportCellModel());
				bizReportRowModel.AddCell(new BizReportCellModel());
				bizReportRowModel.AddCell(new BizReportCellModel());
				bizReportRowModel.AddCell(new BizReportCellModel());
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

		public List<RPTReportConfigModel> GetEndBalReportConfigData(MContext ctx, RPTBalanceSheetFilterModel filter)
		{
			List<RPTReportConfigModel> configList = base.GetConfigList(ctx, RPTGLReportType.BalanceSheet);
			if (filter.MDate < ctx.MGLBeginDate)
			{
				List<RPTGLAccountModel> initBalanceList = RPTBalanceSheetRepository.GetInitBalanceList(filter, ctx);
				return base.ConvertAmount(ctx, configList, initBalanceList);
			}
			List<RPTGLAccountModel> endBalanceList = RPTBalanceSheetRepository.GetEndBalanceList(filter, ctx);
			return base.ConvertAmount(ctx, configList, endBalanceList);
		}

		public List<RPTGLAccountModel> GetInitBalanceList(RPTBalanceSheetFilterModel filter, MContext ctx)
		{
			return RPTBalanceSheetRepository.GetInitBalanceList(filter, ctx);
		}

		public bool IsBalance(MContext ctx, DateTime date)
		{
			RPTBalanceSheetFilterModel rPTBalanceSheetFilterModel = new RPTBalanceSheetFilterModel();
			rPTBalanceSheetFilterModel.MDateString = date.ToString("yyyy-MM-01");
			List<RPTReportConfigModel> configList = base.GetConfigList(ctx, RPTGLReportType.BalanceSheet);
			List<RPTGLAccountModel> endBalanceList = RPTBalanceSheetRepository.GetEndBalanceList(rPTBalanceSheetFilterModel, ctx);
			List<RPTReportConfigModel> list = base.ConvertAmount(ctx, configList, endBalanceList);
			if (list == null || list.Count == 0)
			{
				return true;
			}
			string assetId = "";
			string liabilityId = "";
			decimal d = default(decimal);
			decimal d2 = default(decimal);
			if (ctx.MAccountTableID == "1")
			{
				assetId = "1_102_34";
				liabilityId = "1_102_68";
			}
			else
			{
				assetId = "2_102_32";
				liabilityId = "2_102_58";
			}
			RPTReportConfigModel rPTReportConfigModel = (from t in list
			where t.MItemID == assetId
			select t).FirstOrDefault();
			RPTReportConfigModel rPTReportConfigModel2 = (from t in list
			where t.MItemID == liabilityId
			select t).FirstOrDefault();
			if (rPTReportConfigModel != null)
			{
				d = rPTReportConfigModel.MAmount;
			}
			if (rPTReportConfigModel2 != null)
			{
				d2 = rPTReportConfigModel2.MAmount;
			}
			return d == d2;
		}
	}
}
