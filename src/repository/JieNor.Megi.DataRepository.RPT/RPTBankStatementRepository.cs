using JieNor.Megi.Core;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JieNor.Megi.DataRepository.RPT
{
	public class RPTBankStatementRepository
	{
		public static BizReportModel GetReport(MContext ctx, RPTBankStatementFilterModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.BankStatement);
			List<IVBankBillStatementModel> bankBillStatementList = IVBankBillEntryRepository.GetBankBillStatementList(ctx, filter);
			SetReportTitle(ctx, filter, bizReportModel);
			SetReportHead(ctx, filter, bizReportModel);
			SetStatement(ctx, filter, bizReportModel, bankBillStatementList);
			return bizReportModel;
		}

		private static void SetReportTitle(MContext ctx, RPTBankStatementFilterModel filter, BizReportModel model)
		{
			model.HeaderTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Summary", "Summary");
			model.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BankStatement", "Bank Statement");
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			BDBankAccountEditModel dataModel = bDBankAccountRepository.GetDataModel(ctx, filter.MBankAccountID, false);
			if (dataModel != null)
			{
				MultiLanguageFieldList multiLanguageFieldList = (from t in dataModel.MultiLanguage
				where t.MFieldName == "MName"
				select t).FirstOrDefault();
				if (multiLanguageFieldList != null)
				{
					model.Title2 = multiLanguageFieldList.MMultiLanguageValue;
				}
			}
			model.Title3 = ctx.MOrgName;
			model.Title4 = filter.MEndDate.ToString("Y", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(ctx)));
		}

		private static void SetReportHead(MContext ctx, RPTBankStatementFilterModel filter, BizReportModel model)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Date),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Description),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "DateImportedIntoMegi", "Date imported into Megi"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Reference),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "Reconciled", "Reconciled"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Source", "Source"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Amount),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "Balance", "Balance"),
				CellType = BizReportCellType.Text
			});
			model.AddRow(bizReportRowModel);
		}

		private static void SetStatement(MContext ctx, RPTBankStatementFilterModel filter, BizReportModel model, List<IVBankBillStatementModel> statementList)
		{
			if (statementList != null && statementList.Count != 0)
			{
				IVBankBillStatementModel iVBankBillStatementModel = statementList[0];
				if (iVBankBillStatementModel.MDate > filter.MFromDate)
				{
					iVBankBillStatementModel.MDate = filter.MFromDate;
				}
				SetOpeningStatement(ctx, model, iVBankBillStatementModel);
				for (int i = 1; i < statementList.Count - 1; i++)
				{
					IVBankBillStatementModel statement = statementList[i];
					SetStatementItem(ctx, model, statement);
				}
				IVBankBillStatementModel statement2 = statementList[0];
				SetClosingStatement(ctx, model, statement2);
			}
		}

		private static void SetOpeningStatement(MContext ctx, BizReportModel model, IVBankBillStatementModel statement)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "OpeningBalance", "Opening Balance");
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.SubTotal;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = statement.MDate.ToOrgZoneDateString(ctx),
				CellType = BizReportCellType.Date
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = text,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = statement.MBalance.ToString(),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private static void SetClosingStatement(MContext ctx, BizReportModel model, IVBankBillStatementModel statement)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ClosingBalance", "Closing Balance");
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.SubTotal;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = text,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = statement.MBalance.ToString(),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private static void SetStatementItem(MContext ctx, BizReportModel model, IVBankBillStatementModel statement)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Item;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = statement.MDate.ToOrgZoneDateString(ctx),
				CellType = BizReportCellType.Date
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = statement.MDesc,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = statement.MImportDate.ToString(),
				CellType = BizReportCellType.Date
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = statement.MRef,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = string.Empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = statement.MBalance.ToString(),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}
	}
}
