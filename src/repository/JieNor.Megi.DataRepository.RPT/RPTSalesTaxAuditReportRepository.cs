using JieNor.Megi.Core;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JieNor.Megi.DataRepository.RPT
{
	public class RPTSalesTaxAuditReportRepository
	{
		public static BizReportModel GetSalesTax(RPTSalesTaxAuditReportFilterModel filter, MContext context)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.SalesTaxReport);
			SetTitle(bizReportModel, filter, context);
			SetRowHead(bizReportModel, filter, context);
			SetRowData(bizReportModel, filter, context);
			return bizReportModel;
		}

		private static void SetTitle(BizReportModel model, RPTSalesTaxBaseFilterModel filter, MContext context)
		{
			model.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "TaxAuditReport", "Tax Audit Report");
			model.Title2 = context.MOrgName;
			string[] obj = new string[6]
			{
				COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ForThePeriod", "For the period "),
				null,
				null,
				null,
				null,
				null
			};
			DateTime dateTime = filter.MFromDate;
			obj[1] = dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(context)));
			obj[2] = " ";
			obj[3] = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "To", "to");
			obj[4] = " ";
			dateTime = filter.MToDate;
			obj[5] = dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(context)));
			model.Title3 = string.Concat(obj);
		}

		private static void SetRowHead(BizReportModel model, RPTSalesTaxBaseFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Date", "Date"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Reference", "Reference"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Details", "Details"),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Gross", "Gross"),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Tax", "Tax"),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Net", "Net"),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private static void SetRowData(BizReportModel model, RPTSalesTaxBaseFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			List<RPTSalesTaxReportDetailsModel> salesTaxReportDetails = RPTSalesTaxReportRepository.GetSalesTaxReportDetails(filter, context);
			List<string> list = (from s in salesTaxReportDetails
			select s.MTaxID).Distinct().ToList();
			foreach (string item in list)
			{
				List<RPTSalesTaxReportDetailsModel> list2 = (from w in salesTaxReportDetails
				where w.MTaxID.Equals(item)
				select w).ToList();
				bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.RowType = BizReportRowType.SubTotal;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = (from s in list2
					select s.MRTTaxName).FirstOrDefault(),
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
				model.AddRow(bizReportRowModel);
				foreach (RPTSalesTaxReportDetailsModel item2 in list2)
				{
					bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.Item;
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = item2.MBizDate.ToString("yyyy-MM-dd"),
						CellType = BizReportCellType.Text,
						CellLink = GetCellLink(context, item2)
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = item2.MReference,
						CellType = BizReportCellType.Text,
						CellLink = GetCellLink(context, item2)
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = item2.MDesc,
						CellType = BizReportCellType.Text,
						CellLink = GetCellLink(context, item2)
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item2.MRTGross),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item2.MRTTax),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item2.MRTNet),
						CellType = BizReportCellType.Money
					});
					model.AddRow(bizReportRowModel);
				}
				bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.RowType = BizReportRowType.SubTotal;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total") + " " + (from s in list2
					select s.MRTTaxName).FirstOrDefault(),
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(list2.Sum((RPTSalesTaxReportDetailsModel s) => s.MRTGross)),
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(list2.Sum((RPTSalesTaxReportDetailsModel s) => s.MRTTax)),
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(list2.Sum((RPTSalesTaxReportDetailsModel s) => s.MRTNet)),
					CellType = BizReportCellType.Money
				});
				model.AddRow(bizReportRowModel);
			}
		}

		private static BizReportCellLinkModel GetCellLink(MContext ctx, RPTSalesTaxReportDetailsModel data)
		{
			BizReportCellLinkModel bizReportCellLinkModel = new BizReportCellLinkModel();
			bizReportCellLinkModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ShowDetails", "Show Details");
			switch (data.MType)
			{
			case "Invoice_Sale":
				bizReportCellLinkModel.Url = $"/IV/Invoice/InvoiceView/{data.MID}";
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewInvoice", "View Invoice");
				break;
			case "Invoice_Sale_Red":
				bizReportCellLinkModel.Url = $"/IV/Invoice/CreditNoteView/{data.MID}";
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewCreditNote", "View Credit Note");
				break;
			case "Invoice_Purchase":
				bizReportCellLinkModel.Url = $"/IV/Bill/BillView/{data.MID}";
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewBill", "View Bill");
				break;
			case "Invoice_Purchase_Red":
				bizReportCellLinkModel.Url = $"/IV/Bill/CreditNoteView/{data.MID}";
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewCreditNote", "View Bill Credit Note");
				break;
			}
			return bizReportCellLinkModel;
		}
	}
}
