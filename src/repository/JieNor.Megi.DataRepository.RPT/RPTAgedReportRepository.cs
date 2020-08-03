using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.RPT
{
	public static class RPTAgedReportRepository
	{
		public static BizReportModel AgedPayablesList(MContext ctx, AgedRptFilterModel filter)
		{
			filter.AgedType = RPTAgedRptFilterEnum.Payables;
			List<RPTAgedReportModel> ageReportData = GetAgeReportData(ctx, filter);
			BizReportModel reportData = GetReportData(ctx, filter, ageReportData);
			reportData.Type = Convert.ToInt32(BizReportType.AgedPayables);
			return reportData;
		}

		public static BizReportModel AgedReceivablesList(MContext ctx, AgedRptFilterModel filter)
		{
			filter.AgedType = RPTAgedRptFilterEnum.Receivables;
			List<RPTAgedReportModel> ageReportData = GetAgeReportData(ctx, filter);
			BizReportModel reportData = GetReportData(ctx, filter, ageReportData);
			reportData.Type = Convert.ToInt32(BizReportType.AgedReceivables);
			return reportData;
		}

		private static BizReportModel GetReportData(MContext ctx, AgedRptFilterModel filter, List<RPTAgedReportModel> agedRptData)
		{
			BizReportModel bizReportModel = new BizReportModel();
			GetRptTitle(ctx, filter, bizReportModel);
			GetHeadRow(ctx, bizReportModel, filter);
			if (agedRptData != null && agedRptData.Count > 0)
			{
				if (filter.ShowInvoice)
				{
					GetItemRowWithInvoice(ctx, bizReportModel, filter, agedRptData);
				}
				else
				{
					GetGroupRow(ctx, bizReportModel, filter);
					GetItemRowNoInvoice(ctx, bizReportModel, filter, agedRptData);
				}
			}
			GetTotalRow(ctx, bizReportModel, filter, agedRptData);
			return bizReportModel;
		}

		private static void GetItemRowNoInvoice(MContext ctx, BizReportModel model, AgedRptFilterModel filter, List<RPTAgedReportModel> agedRptData)
		{
			IOrderedEnumerable<RPTAgedReportModel> orderedEnumerable = from x in agedRptData
			where x.MRowType == BizReportRowType.SubTotal
			orderby x.MContactName
			select x;
			foreach (RPTAgedReportModel item in orderedEnumerable)
			{
				RPTAgedReportModel subTotal = agedRptData.FirstOrDefault((RPTAgedReportModel f) => f.MRowType == BizReportRowType.SubTotal && f.MContactID == item.MContactID);
				GetSubTotalRow(ctx, model, filter, subTotal, false);
			}
		}

		private static void GetItemRowWithInvoice(MContext ctx, BizReportModel model, AgedRptFilterModel filter, List<RPTAgedReportModel> agedRptData)
		{
			IOrderedEnumerable<RPTAgedReportModel> orderedEnumerable = from x in agedRptData
			where x.MRowType == BizReportRowType.SubTotal
			orderby x.MContactName
			select x;
			foreach (RPTAgedReportModel item in orderedEnumerable)
			{
				BizReportRowModel bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.RowType = BizReportRowType.Group;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.MContactName,
					CellType = BizReportCellType.Text
				});
				model.AddRow(bizReportRowModel);
				List<RPTAgedReportModel> agedRptData2 = (from f in agedRptData
				where f.MRowType == BizReportRowType.Item && f.MContactID == item.MContactID
				select f).ToList();
				GetItemRow(ctx, model, agedRptData2);
				RPTAgedReportModel subTotal = agedRptData.FirstOrDefault((RPTAgedReportModel f) => f.MRowType == BizReportRowType.SubTotal && f.MContactID == item.MContactID);
				GetSubTotalRow(ctx, model, filter, subTotal, true);
			}
		}

		private static void GetSubTotalRow(MContext ctx, BizReportModel model, AgedRptFilterModel filter, RPTAgedReportModel subTotal, bool showTotal)
		{
			if (subTotal != null)
			{
				BizReportRowModel bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.RowType = BizReportRowType.Item;
				string value = subTotal.MContactName;
				if (showTotal)
				{
					bizReportRowModel.RowType = BizReportRowType.SubTotal;
					value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Total", "Total") + " " + subTotal.MContactName;
				}
				bizReportRowModel.UniqueValue = subTotal.MContactID;
				BizReportCellModel bizReportCellModel = new BizReportCellModel
				{
					Value = value,
					CellType = BizReportCellType.Text
				};
				bizReportCellModel.SubReport = GetInvoiceRptFilterModel(ctx, -1, subTotal.MContactID, subTotal.MContactName, filter);
				bizReportCellModel.BillIDS = subTotal.TotalAmtInvoiceIDS;
				bizReportRowModel.AddCell(bizReportCellModel);
				BizReportCellModel bizReportCellModel2 = new BizReportCellModel();
				decimal num = subTotal.MCurrentAmt;
				bizReportCellModel2.Value = num.ToString();
				bizReportCellModel2.CellType = BizReportCellType.Money;
				BizReportCellModel bizReportCellModel3 = bizReportCellModel2;
				bizReportCellModel3.SubReport = GetInvoiceRptFilterModel(ctx, 0, subTotal.MContactID, subTotal.MContactName, filter);
				bizReportCellModel3.BillIDS = subTotal.CurrentAmtInvoiceIDS;
				bizReportRowModel.AddCell(bizReportCellModel3);
				BizReportCellModel bizReportCellModel4 = new BizReportCellModel();
				num = subTotal.MMonthAmt1;
				bizReportCellModel4.Value = num.ToString();
				bizReportCellModel4.CellType = BizReportCellType.Money;
				BizReportCellModel bizReportCellModel5 = bizReportCellModel4;
				bizReportCellModel5.SubReport = GetInvoiceRptFilterModel(ctx, 1, subTotal.MContactID, subTotal.MContactName, filter);
				bizReportCellModel5.BillIDS = subTotal.MonthAmt1InvoiceIDS;
				bizReportRowModel.AddCell(bizReportCellModel5);
				BizReportCellModel bizReportCellModel6 = new BizReportCellModel();
				num = subTotal.MMonthAmt2;
				bizReportCellModel6.Value = num.ToString();
				bizReportCellModel6.CellType = BizReportCellType.Money;
				BizReportCellModel bizReportCellModel7 = bizReportCellModel6;
				bizReportCellModel7.SubReport = GetInvoiceRptFilterModel(ctx, 2, subTotal.MContactID, subTotal.MContactName, filter);
				bizReportCellModel7.BillIDS = subTotal.MonthAmt2InvoiceIDS;
				bizReportRowModel.AddCell(bizReportCellModel7);
				BizReportCellModel bizReportCellModel8 = new BizReportCellModel();
				num = subTotal.MMonthAmt3;
				bizReportCellModel8.Value = num.ToString();
				bizReportCellModel8.CellType = BizReportCellType.Money;
				BizReportCellModel bizReportCellModel9 = bizReportCellModel8;
				bizReportCellModel9.SubReport = GetInvoiceRptFilterModel(ctx, 3, subTotal.MContactID, subTotal.MContactName, filter);
				bizReportCellModel9.BillIDS = subTotal.MonthAmt3InvoiceIDS;
				bizReportRowModel.AddCell(bizReportCellModel9);
				BizReportCellModel bizReportCellModel10 = new BizReportCellModel();
				num = subTotal.MOlderAmt;
				bizReportCellModel10.Value = num.ToString();
				bizReportCellModel10.CellType = BizReportCellType.Money;
				BizReportCellModel bizReportCellModel11 = bizReportCellModel10;
				bizReportCellModel11.SubReport = GetInvoiceRptFilterModel(ctx, 4, subTotal.MContactID, subTotal.MContactName, filter);
				bizReportCellModel11.BillIDS = subTotal.OlderAmtInvoiceIDS;
				bizReportRowModel.AddCell(bizReportCellModel11);
				BizReportCellModel bizReportCellModel12 = new BizReportCellModel();
				num = subTotal.MTotalAmt;
				bizReportCellModel12.Value = num.ToString();
				bizReportCellModel12.CellType = BizReportCellType.Money;
				bizReportCellModel12.CellStatuType = BizReportCellStatuType.UpRed;
				BizReportCellModel bizReportCellModel13 = bizReportCellModel12;
				bizReportCellModel13.SubReport = GetInvoiceRptFilterModel(ctx, -1, subTotal.MContactID, subTotal.MContactName, filter);
				bizReportCellModel13.BillIDS = subTotal.TotalAmtInvoiceIDS;
				bizReportRowModel.AddCell(bizReportCellModel13);
				model.AddRow(bizReportRowModel);
			}
		}

		private static BizSubRptCreateModel GetInvoiceRptFilterModel(MContext ctx, int monthType, string contactID, string contactName, AgedRptFilterModel ageFilter)
		{
			RPTAgeInvoiceFilterModel rPTAgeInvoiceFilterModel = new RPTAgeInvoiceFilterModel();
			rPTAgeInvoiceFilterModel.MContactID = contactID;
			rPTAgeInvoiceFilterModel.AgedType = ageFilter.AgedType;
			rPTAgeInvoiceFilterModel.AgedByField = ageFilter.AgedByField;
			rPTAgeInvoiceFilterModel.AsAt = ageFilter.MEndDate;
			rPTAgeInvoiceFilterModel.MContactName = "";
			switch (monthType)
			{
			case -1:
				rPTAgeInvoiceFilterModel.DateFrom = new DateTime(2000, 1, 1);
				rPTAgeInvoiceFilterModel.DateTo = GetDate(ageFilter.MEndDateExt, false);
				break;
			case 0:
				rPTAgeInvoiceFilterModel.DateFrom = GetDate(ageFilter.MEndDate, true);
				rPTAgeInvoiceFilterModel.DateTo = GetDate(ageFilter.MEndDate, false);
				break;
			case 1:
				rPTAgeInvoiceFilterModel.DateFrom = GetDate(ageFilter.MEndDate1, true);
				rPTAgeInvoiceFilterModel.DateTo = GetDate(ageFilter.MEndDate1, false);
				break;
			case 2:
				rPTAgeInvoiceFilterModel.DateFrom = GetDate(ageFilter.MEndDate2, true);
				rPTAgeInvoiceFilterModel.DateTo = GetDate(ageFilter.MEndDate2, false);
				break;
			case 3:
				rPTAgeInvoiceFilterModel.DateFrom = GetDate(ageFilter.MEndDate3, true);
				rPTAgeInvoiceFilterModel.DateTo = GetDate(ageFilter.MEndDate3, false);
				break;
			case 4:
			{
				RPTAgeInvoiceFilterModel rPTAgeInvoiceFilterModel2 = rPTAgeInvoiceFilterModel;
				DateTime dateTime = ageFilter.MEndOldDate;
				dateTime = dateTime.AddYears(-5);
				rPTAgeInvoiceFilterModel2.DateFrom = dateTime.AddDays(1.0);
				rPTAgeInvoiceFilterModel.DateTo = GetDate(ageFilter.MEndOldDate, false);
				break;
			}
			}
			BizSubRptCreateModel bizSubRptCreateModel = new BizSubRptCreateModel();
			if (rPTAgeInvoiceFilterModel.AgedType == RPTAgedRptFilterEnum.Payables)
			{
				bizSubRptCreateModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ShowPurchaseInvoice", "显示采购单");
			}
			else
			{
				bizSubRptCreateModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Show Invoices", "Show Invoices");
			}
			bizSubRptCreateModel.ReportType = BizReportType.Invoices;
			bizSubRptCreateModel.ReportFilter = rPTAgeInvoiceFilterModel;
			return bizSubRptCreateModel;
		}

		private static DateTime GetDate(DateTime dt, bool beginDate = true)
		{
			if (beginDate)
			{
				return new DateTime(dt.Year, dt.Month, 1);
			}
			DateTime dateTime = dt.AddMonths(1);
			return new DateTime(dateTime.Year, dateTime.Month, 1).AddDays(-1.0);
		}

		private static void GetRptTitle(MContext ctx, AgedRptFilterModel filter, BizReportModel model)
		{
			if (filter.AgedType == RPTAgedRptFilterEnum.Payables)
			{
				model.HeaderTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Summary", "Summary");
				model.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "AgedPayables", "Aged Payables");
			}
			else
			{
				model.HeaderTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Summary", "Summary");
				model.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "AgedReceivables", "Aged Receivables");
			}
			model.Title2 = ctx.MOrgName;
			model.Title3 = filter.MEndDate.ToString("Y", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(ctx)));
		}

		private static void GetTotalRow(MContext ctx, BizReportModel model, AgedRptFilterModel filter, List<RPTAgedReportModel> agedRptData)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Total;
			if (filter.AgedType == RPTAgedRptFilterEnum.Payables)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "TotalPayables", "Total Payables"),
					CellType = BizReportCellType.Text
				});
			}
			else
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "TotalReceivables", "Total Receivables"),
					CellType = BizReportCellType.Text
				});
			}
			RPTAgedReportModel rPTAgedReportModel = agedRptData.FirstOrDefault((RPTAgedReportModel f) => f.MContactID == "-0" && f.MRowType == BizReportRowType.Total);
			decimal num;
			if (rPTAgedReportModel != null)
			{
				BizReportRowModel bizReportRowModel2 = bizReportRowModel;
				BizReportCellModel bizReportCellModel = new BizReportCellModel();
				num = rPTAgedReportModel.MCurrentAmt;
				bizReportCellModel.Value = num.ToString();
				bizReportCellModel.CellType = BizReportCellType.Money;
				bizReportRowModel2.AddCell(bizReportCellModel);
				BizReportRowModel bizReportRowModel3 = bizReportRowModel;
				BizReportCellModel bizReportCellModel2 = new BizReportCellModel();
				num = rPTAgedReportModel.MMonthAmt1;
				bizReportCellModel2.Value = num.ToString();
				bizReportCellModel2.CellType = BizReportCellType.Money;
				bizReportRowModel3.AddCell(bizReportCellModel2);
				BizReportRowModel bizReportRowModel4 = bizReportRowModel;
				BizReportCellModel bizReportCellModel3 = new BizReportCellModel();
				num = rPTAgedReportModel.MMonthAmt2;
				bizReportCellModel3.Value = num.ToString();
				bizReportCellModel3.CellType = BizReportCellType.Money;
				bizReportRowModel4.AddCell(bizReportCellModel3);
				BizReportRowModel bizReportRowModel5 = bizReportRowModel;
				BizReportCellModel bizReportCellModel4 = new BizReportCellModel();
				num = rPTAgedReportModel.MMonthAmt3;
				bizReportCellModel4.Value = num.ToString();
				bizReportCellModel4.CellType = BizReportCellType.Money;
				bizReportRowModel5.AddCell(bizReportCellModel4);
				BizReportRowModel bizReportRowModel6 = bizReportRowModel;
				BizReportCellModel bizReportCellModel5 = new BizReportCellModel();
				num = rPTAgedReportModel.MOlderAmt;
				bizReportCellModel5.Value = num.ToString();
				bizReportCellModel5.CellType = BizReportCellType.Money;
				bizReportRowModel6.AddCell(bizReportCellModel5);
				BizReportRowModel bizReportRowModel7 = bizReportRowModel;
				BizReportCellModel bizReportCellModel6 = new BizReportCellModel();
				num = rPTAgedReportModel.MTotalAmt;
				bizReportCellModel6.Value = num.ToString();
				bizReportCellModel6.CellType = BizReportCellType.Money;
				bizReportCellModel6.CellStatuType = BizReportCellStatuType.UpRed;
				bizReportRowModel7.AddCell(bizReportCellModel6);
			}
			model.AddRow(bizReportRowModel);
			BizReportRowModel bizReportRowModel8 = new BizReportRowModel();
			bizReportRowModel8.RowType = BizReportRowType.Total;
			bizReportRowModel8.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Percent", "Percent"),
				CellType = BizReportCellType.Text
			});
			RPTAgedReportModel rPTAgedReportModel2 = agedRptData.FirstOrDefault((RPTAgedReportModel f) => f.MContactID == "-1" && f.MRowType == BizReportRowType.Total);
			if (rPTAgedReportModel2 != null)
			{
				BizReportRowModel bizReportRowModel9 = bizReportRowModel8;
				BizReportCellModel bizReportCellModel7 = new BizReportCellModel();
				num = rPTAgedReportModel2.MCurrentAmt;
				bizReportCellModel7.Value = num.ToString() + "%";
				bizReportCellModel7.CellType = BizReportCellType.Money;
				bizReportRowModel9.AddCell(bizReportCellModel7);
				BizReportRowModel bizReportRowModel10 = bizReportRowModel8;
				BizReportCellModel bizReportCellModel8 = new BizReportCellModel();
				num = rPTAgedReportModel2.MMonthAmt1;
				bizReportCellModel8.Value = num.ToString() + "%";
				bizReportCellModel8.CellType = BizReportCellType.Money;
				bizReportRowModel10.AddCell(bizReportCellModel8);
				BizReportRowModel bizReportRowModel11 = bizReportRowModel8;
				BizReportCellModel bizReportCellModel9 = new BizReportCellModel();
				num = rPTAgedReportModel2.MMonthAmt2;
				bizReportCellModel9.Value = num.ToString() + "%";
				bizReportCellModel9.CellType = BizReportCellType.Money;
				bizReportRowModel11.AddCell(bizReportCellModel9);
				BizReportRowModel bizReportRowModel12 = bizReportRowModel8;
				BizReportCellModel bizReportCellModel10 = new BizReportCellModel();
				num = rPTAgedReportModel2.MMonthAmt3;
				bizReportCellModel10.Value = num.ToString() + "%";
				bizReportCellModel10.CellType = BizReportCellType.Money;
				bizReportRowModel12.AddCell(bizReportCellModel10);
				BizReportRowModel bizReportRowModel13 = bizReportRowModel8;
				BizReportCellModel bizReportCellModel11 = new BizReportCellModel();
				num = rPTAgedReportModel2.MOlderAmt;
				bizReportCellModel11.Value = num.ToString() + "%";
				bizReportCellModel11.CellType = BizReportCellType.Money;
				bizReportRowModel13.AddCell(bizReportCellModel11);
				bizReportRowModel8.AddCell(new BizReportCellModel
				{
					Value = "100%",
					CellType = BizReportCellType.Money
				});
			}
			model.AddRow(bizReportRowModel8);
		}

		private static void GetItemRow(MContext ctx, BizReportModel model, List<RPTAgedReportModel> agedRptData)
		{
			foreach (RPTAgedReportModel agedRptDatum in agedRptData)
			{
				if (agedRptDatum.MRowType != BizReportRowType.Total)
				{
					BizReportRowModel bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.Item;
					BizReportCellModel bizReportCellModel = new BizReportCellModel
					{
						Value = agedRptDatum.MOldBizDate.ToShortDateString() + " " + agedRptDatum.MInvoiceNo,
						CellType = BizReportCellType.Text,
						CellLink = GetCellLink(ctx, agedRptDatum)
					};
					bizReportCellModel.AddBillID(agedRptDatum.MInvoiceID);
					bizReportRowModel.AddCell(bizReportCellModel);
					BizReportCellModel bizReportCellModel2 = new BizReportCellModel();
					decimal num = agedRptDatum.MCurrentAmt;
					bizReportCellModel2.Value = num.ToString();
					bizReportCellModel2.CellType = BizReportCellType.Money;
					BizReportCellModel bizReportCellModel3 = bizReportCellModel2;
					bizReportCellModel3.AddBillID(agedRptDatum.MInvoiceID);
					bizReportRowModel.AddCell(bizReportCellModel3);
					BizReportCellModel bizReportCellModel4 = new BizReportCellModel();
					num = agedRptDatum.MMonthAmt1;
					bizReportCellModel4.Value = num.ToString();
					bizReportCellModel4.CellType = BizReportCellType.Money;
					BizReportCellModel bizReportCellModel5 = bizReportCellModel4;
					bizReportCellModel5.AddBillID(agedRptDatum.MInvoiceID);
					bizReportRowModel.AddCell(bizReportCellModel5);
					BizReportCellModel bizReportCellModel6 = new BizReportCellModel();
					num = agedRptDatum.MMonthAmt2;
					bizReportCellModel6.Value = num.ToString();
					bizReportCellModel6.CellType = BizReportCellType.Money;
					BizReportCellModel bizReportCellModel7 = bizReportCellModel6;
					bizReportCellModel7.AddBillID(agedRptDatum.MInvoiceID);
					bizReportRowModel.AddCell(bizReportCellModel7);
					BizReportCellModel bizReportCellModel8 = new BizReportCellModel();
					num = agedRptDatum.MMonthAmt3;
					bizReportCellModel8.Value = num.ToString();
					bizReportCellModel8.CellType = BizReportCellType.Money;
					BizReportCellModel bizReportCellModel9 = bizReportCellModel8;
					bizReportCellModel9.AddBillID(agedRptDatum.MInvoiceID);
					bizReportRowModel.AddCell(bizReportCellModel9);
					BizReportCellModel bizReportCellModel10 = new BizReportCellModel();
					num = agedRptDatum.MOlderAmt;
					bizReportCellModel10.Value = num.ToString();
					bizReportCellModel10.CellType = BizReportCellType.Money;
					BizReportCellModel bizReportCellModel11 = bizReportCellModel10;
					bizReportCellModel11.AddBillID(agedRptDatum.MInvoiceID);
					bizReportRowModel.AddCell(bizReportCellModel11);
					BizReportCellModel bizReportCellModel12 = new BizReportCellModel();
					num = agedRptDatum.MTotalAmt;
					bizReportCellModel12.Value = num.ToString();
					bizReportCellModel12.CellType = BizReportCellType.Money;
					BizReportCellModel bizReportCellModel13 = bizReportCellModel12;
					bizReportCellModel13.AddBillID(agedRptDatum.MInvoiceID);
					bizReportRowModel.AddCell(bizReportCellModel13);
					model.AddRow(bizReportRowModel);
				}
			}
		}

		private static void GetHeadRow(MContext ctx, BizReportModel model, AgedRptFilterModel filter)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = "",
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Current", "Current")
			});
			if (filter.AgedShowType == AgedShowType.MonthNumber)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = GetDateHeadTitle(ctx, 1)
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = GetDateHeadTitle(ctx, 2)
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = GetDateHeadTitle(ctx, 3)
				});
			}
			else
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = GetDateHeadTitle(ctx, filter.MEndDate1)
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = GetDateHeadTitle(ctx, filter.MEndDate2)
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = GetDateHeadTitle(ctx, filter.MEndDate3)
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Older", "Older")
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Total", "Total")
			});
			model.AddRow(bizReportRowModel);
		}

		private static string GetDateHeadTitle(MContext ctx, int number)
		{
			if (ctx.MLCID == "0x0009")
			{
				return $"{number} months";
			}
			return $"{number}个月前";
		}

		private static string GetDateHeadTitle(MContext ctx, DateTime period)
		{
			if (ctx.MLCID == "0x0009")
			{
				return period.ToString("MMMMMMMMMMMMMMMM", CultureInfo.CreateSpecificCulture("en-US"));
			}
			return $"{period.Month}月";
		}

		private static void GetGroupRow(MContext ctx, BizReportModel model, AgedRptFilterModel filter)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Group;
			if (filter.AgedType == RPTAgedRptFilterEnum.Payables)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Payables", "Payables"),
					CellType = BizReportCellType.Text
				});
			}
			else
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Receivables", "Receivables"),
					CellType = BizReportCellType.Text
				});
			}
			model.AddRow(bizReportRowModel);
		}

		private static List<RPTAgedReportModel> GetAgeReportData(MContext ctx, AgedRptFilterModel filter)
		{
			DataTable finishVerifData = GetFinishVerifData(ctx, filter);
			List<RPTAgedReportModel> list = ModelInfoManager.DataTableToList<RPTAgedReportModel>(finishVerifData);
			SetRate(ctx, filter, list);
			List<RPTAgedReportModel> list2 = new List<RPTAgedReportModel>();
			if (list == null || list.Count() == 0)
			{
				return new List<RPTAgedReportModel>();
			}
			IEnumerable<IGrouping<string, RPTAgedReportModel>> enumerable = from x in list
			group x by x.MInvoiceID;
			foreach (IGrouping<string, RPTAgedReportModel> item in enumerable)
			{
				List<RPTAgedReportModel> list3 = item.ToList();
				if (list3 != null && list3.Count() != 0)
				{
					RPTAgedReportModel rPTAgedReportModel = list3.First();
					SetMonthAmount(filter, list3, rPTAgedReportModel);
					decimal d = Math.Abs(rPTAgedReportModel.MCurrentAmt) + Math.Abs(rPTAgedReportModel.MMonthAmt1) + Math.Abs(rPTAgedReportModel.MMonthAmt2) + Math.Abs(rPTAgedReportModel.MMonthAmt3) + Math.Abs(rPTAgedReportModel.MOlderAmt);
					if (d != decimal.Zero)
					{
						list2.Add(rPTAgedReportModel);
					}
				}
			}
			GetSubTotalData(filter, list2, list2);
			GetTotalData(ctx, list2);
			return list2;
		}

		private static void SetRate(MContext ctx, AgedRptFilterModel filter, List<RPTAgedReportModel> detailData)
		{
			string currnecyID = GetCurrnecyID(ctx);
			foreach (RPTAgedReportModel detailDatum in detailData)
			{
				if (detailDatum.MCyID == currnecyID || detailDatum.MExchangeRate == decimal.Zero)
				{
					detailDatum.MExchangeRate = decimal.One;
					detailDatum.MHaveVerificationAmt = Math.Round(detailDatum.MHaveVerificationAmtFor, 2);
					detailDatum.MNoVerificationAmt = Math.Round(detailDatum.MNoVerificationAmtFor, 2);
					detailDatum.MTaxTotalAmt = Math.Round(detailDatum.MTaxTotalAmtFor, 2);
				}
				else
				{
					detailDatum.MHaveVerificationAmt = Math.Round(detailDatum.MHaveVerificationAmt, 2);
					detailDatum.MNoVerificationAmt = Math.Round(detailDatum.MNoVerificationAmt, 2);
					detailDatum.MTaxTotalAmt = Math.Round(detailDatum.MTaxTotalAmt, 2);
				}
			}
		}

		private static Dictionary<string, decimal> GetCurrencyRate(MContext ctx, AgedRptFilterModel filter, string sourceCyID)
		{
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT distinct a.MItemId,a.msourceCurrencyID,a.mtargetcurrencyid,a.mratedate,ifnull(a.MRate,0) MRate,ifnull(a.MUserRate,0) MUserRate ");
			stringBuilder.AppendLine("FROM T_BD_ExchangeRate a");
			stringBuilder.AppendLine("Inner join (");
			stringBuilder.AppendLine("        SELECT MItemId,MSourceCurrencyID,mtargetcurrencyid,max(MRatedate) as MRatedate ");
			stringBuilder.AppendLine("        FROM T_BD_ExchangeRate ");
			stringBuilder.AppendLine("        where morgid=@MOrgID AND MIsDelete=0  and msourceCurrencyID=@msourceCurrencyID and MRateDate <=@MRateDate");
			stringBuilder.AppendLine("        group by morgid,msourceCurrencyID,mtargetcurrencyid");
			stringBuilder.AppendLine("    ) b on a.MItemId=b.MItemId and a.msourceCurrencyID=b.msourceCurrencyID ");
			stringBuilder.AppendLine("           and a.mtargetcurrencyid=b.mtargetcurrencyid and a.mratedate=b.mratedate");
			stringBuilder.AppendLine("Where a.morgid=@MOrgID and a.MIsDelete=0 and a.msourceCurrencyID=@msourceCurrencyID and a.MRateDate <=@MRateDate");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@msourceCurrencyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MRateDate", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = sourceCyID;
			array[2].Value = filter.MEndDate;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					decimal num = default(decimal);
					decimal.TryParse(row["MUserRate"].ToString(), out num);
					if (num == decimal.Zero)
					{
						decimal.TryParse(row["MRate"].ToString(), out num);
					}
					if (num == decimal.Zero)
					{
						num = decimal.One;
					}
					dictionary.Add(row["mtargetcurrencyid"].ToString(), num);
				}
			}
			return dictionary;
		}

		private static string GetCurrnecyID(MContext ctx)
		{
			string sql = "select MCurrencyID from T_REG_Financial Where MOrgID=@MOrgID and MIsDelete = 0 ";
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

		private static void GetTotalData(MContext ctx, List<RPTAgedReportModel> lst)
		{
			List<RPTAgedReportModel> list = (from f in lst
			where f.MRowType == BizReportRowType.Item
			select f).ToList();
			if (list != null && list.Count != 0)
			{
				RPTAgedReportModel rPTAgedReportModel = new RPTAgedReportModel();
				rPTAgedReportModel.MRowType = BizReportRowType.Total;
				rPTAgedReportModel.MContactID = "-0";
				rPTAgedReportModel.MContactName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Total", "Total");
				rPTAgedReportModel.MCurrentAmt = (from x in list
				select x.MCurrentAmt).Sum();
				rPTAgedReportModel.MMonthAmt1 = (from x in list
				select x.MMonthAmt1).Sum();
				rPTAgedReportModel.MMonthAmt2 = (from x in list
				select x.MMonthAmt2).Sum();
				rPTAgedReportModel.MMonthAmt3 = (from x in list
				select x.MMonthAmt3).Sum();
				rPTAgedReportModel.MOlderAmt = (from x in list
				select x.MOlderAmt).Sum();
				lst.Add(rPTAgedReportModel);
				RPTAgedReportModel rPTAgedReportModel2 = new RPTAgedReportModel();
				rPTAgedReportModel2.MRowType = BizReportRowType.Total;
				rPTAgedReportModel2.MContactID = "-1";
				rPTAgedReportModel2.MContactName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Percent", "Percent");
				if (rPTAgedReportModel.MTotalAmt != decimal.Zero)
				{
					rPTAgedReportModel2.MCurrentAmt = Math.Round(rPTAgedReportModel.MCurrentAmt * 100m / rPTAgedReportModel.MTotalAmt, 2);
					rPTAgedReportModel2.MMonthAmt1 = Math.Round(rPTAgedReportModel.MMonthAmt1 * 100m / rPTAgedReportModel.MTotalAmt, 2);
					rPTAgedReportModel2.MMonthAmt2 = Math.Round(rPTAgedReportModel.MMonthAmt2 * 100m / rPTAgedReportModel.MTotalAmt, 2);
					rPTAgedReportModel2.MMonthAmt3 = Math.Round(rPTAgedReportModel.MMonthAmt3 * 100m / rPTAgedReportModel.MTotalAmt, 2);
					rPTAgedReportModel2.MOlderAmt = 100m - rPTAgedReportModel2.MCurrentAmt - rPTAgedReportModel2.MMonthAmt1 - rPTAgedReportModel2.MMonthAmt2 - rPTAgedReportModel2.MMonthAmt3;
				}
				lst.Add(rPTAgedReportModel2);
			}
		}

		private static void GetSubTotalData(AgedRptFilterModel filter, List<RPTAgedReportModel> detailData, List<RPTAgedReportModel> lst)
		{
			List<string> list = (from d in detailData
			select d.MContactID).Distinct().ToList();
			foreach (string item2 in list)
			{
				List<RPTAgedReportModel> contactData = (from f in detailData
				where f.MContactID == item2
				select f).ToList();
				RPTAgedReportModel item = SetMonthAmount(filter, contactData);
				lst.Add(item);
			}
		}

		private static void SetMonthAmount(AgedRptFilterModel filter, List<RPTAgedReportModel> contactData, RPTAgedReportModel apm)
		{
			if (contactData != null && contactData.Count != 0)
			{
				List<RPTAgedReportModel> list = (from f in contactData
				where $"{f.MBizDate:yyyy-MM-01}" == $"{filter.MEndDate:yyyy-MM-01}"
				select f).ToList();
				apm.MCurrentAmt = GetNoVerificationAmt(list);
				apm.CurrentAmtInvoiceIDS = (from x in list
				select x.MInvoiceID).ToList();
				List<RPTAgedReportModel> list2 = (from f in contactData
				where $"{f.MBizDate:yyyy-MM-01}" == $"{filter.MEndDate1:yyyy-MM-01}"
				select f).ToList();
				apm.MMonthAmt1 = GetNoVerificationAmt(list2);
				apm.MonthAmt1InvoiceIDS = (from x in list2
				select x.MInvoiceID).ToList();
				List<RPTAgedReportModel> list3 = (from f in contactData
				where $"{f.MBizDate:yyyy-MM-01}" == $"{filter.MEndDate2:yyyy-MM-01}"
				select f).ToList();
				apm.MMonthAmt2 = GetNoVerificationAmt(list3);
				apm.MonthAmt2InvoiceIDS = (from x in list3
				select x.MInvoiceID).ToList();
				List<RPTAgedReportModel> list4 = (from f in contactData
				where $"{f.MBizDate:yyyy-MM-01}" == $"{filter.MEndDate3:yyyy-MM-01}"
				select f).ToList();
				apm.MMonthAmt3 = GetNoVerificationAmt(list4);
				apm.MonthAmt1InvoiceIDS = (from x in list4
				select x.MInvoiceID).ToList();
				List<RPTAgedReportModel> list5 = (from f in contactData
				where f.MBizDate <= filter.MEndOldDate
				select f).ToList();
				apm.MOlderAmt = GetNoVerificationAmt(list5);
				apm.OlderAmtInvoiceIDS = (from x in list5
				select x.MInvoiceID).ToList();
				apm.TotalAmtInvoiceIDS = (from x in contactData
				select x.MInvoiceID).ToList();
			}
		}

		private static RPTAgedReportModel SetMonthAmount(AgedRptFilterModel filter, List<RPTAgedReportModel> contactData)
		{
			RPTAgedReportModel rPTAgedReportModel = new RPTAgedReportModel();
			rPTAgedReportModel.MRowType = BizReportRowType.SubTotal;
			rPTAgedReportModel.MContactID = contactData[0].MContactID;
			rPTAgedReportModel.MContactName = contactData[0].MContactName;
			if (contactData == null || contactData.Count == 0)
			{
				return rPTAgedReportModel;
			}
			List<RPTAgedReportModel> source = (from f in contactData
			where $"{f.MBizDate:yyyy-MM-01}" == $"{filter.MEndDate:yyyy-MM-01}"
			select f).ToList();
			rPTAgedReportModel.MCurrentAmt = source.Sum((RPTAgedReportModel x) => x.MCurrentAmt);
			rPTAgedReportModel.CurrentAmtInvoiceIDS = (from x in source
			select x.MInvoiceID).ToList();
			List<RPTAgedReportModel> source2 = (from f in contactData
			where $"{f.MBizDate:yyyy-MM-01}" == $"{filter.MEndDate1:yyyy-MM-01}"
			select f).ToList();
			rPTAgedReportModel.MMonthAmt1 = source2.Sum((RPTAgedReportModel x) => x.MMonthAmt1);
			rPTAgedReportModel.MonthAmt1InvoiceIDS = (from x in source2
			select x.MInvoiceID).ToList();
			List<RPTAgedReportModel> source3 = (from f in contactData
			where $"{f.MBizDate:yyyy-MM-01}" == $"{filter.MEndDate2:yyyy-MM-01}"
			select f).ToList();
			rPTAgedReportModel.MMonthAmt2 = source3.Sum((RPTAgedReportModel x) => x.MMonthAmt2);
			rPTAgedReportModel.MonthAmt2InvoiceIDS = (from x in source3
			select x.MInvoiceID).ToList();
			List<RPTAgedReportModel> source4 = (from f in contactData
			where $"{f.MBizDate:yyyy-MM-01}" == $"{filter.MEndDate3:yyyy-MM-01}"
			select f).ToList();
			rPTAgedReportModel.MMonthAmt3 = source4.Sum((RPTAgedReportModel x) => x.MMonthAmt3);
			rPTAgedReportModel.MonthAmt1InvoiceIDS = (from x in source4
			select x.MInvoiceID).ToList();
			List<RPTAgedReportModel> source5 = (from f in contactData
			where f.MBizDate <= filter.MEndOldDate
			select f).ToList();
			rPTAgedReportModel.MOlderAmt = source5.Sum((RPTAgedReportModel x) => x.MOlderAmt);
			rPTAgedReportModel.OlderAmtInvoiceIDS = (from x in source5
			select x.MInvoiceID).ToList();
			rPTAgedReportModel.TotalAmtInvoiceIDS = (from x in contactData
			select x.MInvoiceID).ToList();
			return rPTAgedReportModel;
		}

		private static decimal GetNoVerificationAmt(List<RPTAgedReportModel> list)
		{
			decimal result = default(decimal);
			if (list == null || list.Count() == 0)
			{
				return result;
			}
			List<RPTAgedReportModel> list2 = (from x in list
			where x.MType == "Invoice_Purchase_Red" || x.MType == "Invoice_Purchase_Red"
			select x).ToList();
			decimal d = (list2 != null && list2.Count() > 0) ? list2.Sum((RPTAgedReportModel x) => x.MHaveVerificationAmt) : decimal.Zero;
			List<RPTAgedReportModel> list3 = (from x in list
			where x.MType != "Invoice_Purchase_Red" && x.MType != "Invoice_Purchase_Red"
			select x).ToList();
			decimal d2 = (list3 != null && list3.Count() > 0) ? list3.Sum((RPTAgedReportModel x) => x.MHaveVerificationAmt) : decimal.Zero;
			return list.First().MTaxTotalAmt - d2 + d;
		}

		private static DataTable GetFinishVerifData(MContext ctx, AgedRptFilterModel filter)
		{
			string contactIDS = GetContactIDS(filter);
			string text = "";
			text = ((filter.AgedType != RPTAgedRptFilterEnum.Payables) ? string.Format("  (MType='{0}' Or MType='{1}') ", "Invoice_Sale", "Invoice_Sale_Red") : string.Format("  (MType='{0}' Or MType='{1}') ", "Invoice_Purchase", "Invoice_Purchase_Red"));
			string text2 = "";
			text2 = ((filter.AgedByField != AgedByField.InvoiceDate) ? "MDueDate" : "MBizDate");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select a.MContactID,convert(AES_DECRYPT(c.MName,'{0}') using utf8) as MContactName,a.MCyID as MCyID,a.MExchangeRate,d.MName as MCyName,a.MType,", "JieNor-001");
			stringBuilder.AppendLine(" a.MID as MInvoiceID,a.MNumber as MInvoiceNo,a.MReference,");
			stringBuilder.AppendLine(" a.MBizDate as MOldBizDate, ");
			stringBuilder.AppendLine(" (CASE WHEN a.MType='Invoice_Sale' OR a.MType='Invoice_Purchase' THEN " + text2 + " ELSE MBizDate END) as MDueDate, ");
			stringBuilder.AppendLine(" (CASE WHEN a.MType='Invoice_Sale' OR a.MType='Invoice_Purchase' THEN DATE_FORMAT(a." + text2 + ",'%Y-%m-01') ELSE DATE_FORMAT(a.MBizDate,'%Y-%m-01') END) as MBizDate, ");
			stringBuilder.AppendLine(" IFNULL(a.MTaxTotalAmtFor,0) as MTaxTotalAmtFor, ");
			stringBuilder.AppendLine(" IF(a.MType ='Invoice_Sale_Red' OR a.MType='Invoice_Purchase_Red' , IFNULL(e.MAmtFor,0)*-1 , IFNULL(e.MAmtFor,0)) as MHaveVerificationAmtFor, ");
			stringBuilder.AppendLine(" IFNULL(a.MTaxTotalAmtFor,0)-IF(a.MType ='Invoice_Sale_Red' OR a.MType='Invoice_Purchase_Red' , IFNULL(e.MAmtFor,0)*-1 , IFNULL(e.MAmtFor,0)) as MNoVerificationAmtFor, ");
			stringBuilder.AppendLine(" IFNULL(a.MTaxTotalAmt,0) as MTaxTotalAmt, ");
			stringBuilder.AppendLine(" IF(a.MType ='Invoice_Sale_Red' OR a.MType='Invoice_Purchase_Red' , IFNULL(e.MAmt,0)*-1 , IFNULL(e.MAmt,0)) as MHaveVerificationAmt, ");
			stringBuilder.AppendLine(" IFNULL(a.MTaxTotalAmt,0)-IF(a.MType ='Invoice_Sale_Red' OR a.MType='Invoice_Purchase_Red' , IFNULL(e.MAmt,0)*-1 , IFNULL(e.MAmt,0)) as MNoVerificationAmt ");
			stringBuilder.AppendLine(" from T_IV_Invoice a ");
			stringBuilder.AppendLine(" left join T_BD_Contacts_l c on a.MOrgID = c.MOrgID and  a.MContactID=c.MParentID and c.MLocaleID=@MLocaleID AND c.MIsDelete=0  ");
			stringBuilder.AppendLine(" left join T_Bas_Currency_L d on a.MCyID=d.MParentID and d.MLocaleID=@MLocaleID  AND d.MIsDelete=0  ");
			stringBuilder.AppendFormat(" left join (select max(g.MID) as mid , Sum(f.MAmtFor) as MAmtFor, SUM(f.MAmt) AS MAmt, f.MOrgID,f.MTargetBillID , f.MSourceBillID from t_iv_verification f \r\n                                            inner join t_iv_receive g on g.MOrgID=f.MOrgID and g.MIsDelete=0 and (g.MID = f.MSourceBillID or g.MID = f.MTargetBillID)\r\n                                            where f.MIsDelete=0 and g.MBizDate<=@MEndDate\r\n                                            group by MOrgID,MTargetBillID , MSourceBillID\r\n                                          UNION\r\n                                          select max(g.MID) as mid , Sum(f.MAmtFor) as MAmtFor, SUM(f.MAmt) AS MAmt,f.MOrgID,f.MTargetBillID , f.MSourceBillID from t_iv_verification f \r\n                                            inner join t_iv_payment g on g.MOrgID=f.MOrgID and g.MIsDelete=0 and (g.MID = f.MSourceBillID or g.MID = f.MTargetBillID) and f.MOrgID=@MOrgID\r\n                                            where f.MIsDelete=0 and g.MBizDate<=@MEndDate\r\n                                            group by MOrgID,MTargetBillID , MSourceBillID\r\n                                          UNION\r\n                                            select max(g.MID) as mid , Sum(f.MAmtFor) as MAmtFor, SUM(f.MAmt) AS MAmt,f.MOrgID,f.MTargetBillID , f.MSourceBillID from t_iv_verification f \r\n                                            inner join T_IV_Invoice g on g.MOrgID=f.MOrgID and g.MIsDelete=0 and (g.MID = f.MSourceBillID or g.MID = f.MTargetBillID)\r\n                                            where f.MIsDelete=0 and g.MBizDate<=@MEndDate  and f.MOrgID=@MOrgID\r\n                                            group by MOrgID,MTargetBillID , MSourceBillID\r\n                                        ) e on e.MOrgID=a.MOrgID and (e.MTargetBillID=a.MID or e.MSourceBillID = a.MID) and a.mid <> e.mid ", "Invoice_Sale_Red", "Invoice_Purchase_Red");
			stringBuilder.AppendLine(" Where ");
			stringBuilder.AppendLine(" (CASE WHEN a.MType='Invoice_Sale' OR a.MType='Invoice_Purchase' THEN " + text2 + "<=@MEndDate ELSE MBizDate <=@MEndDate END) ");
			stringBuilder.AppendLine("        And " + text);
			stringBuilder.AppendLine("        And a.MOrgID=@MOrgID ");
			stringBuilder.AppendLine("        And (a.MIsDelete=0 AND a.MStatus>=3 )");
			stringBuilder.AppendLine(" and (IFNULL(a.MTaxTotalAmtFor,0)- IF(a.MType ='Invoice_Sale_Red' OR a.MType='Invoice_Purchase_Red' , IFNULL(e.MAmtFor,0)*-1 , IFNULL(e.MAmtFor,0)) ) <> 0 ");
			if (!string.IsNullOrWhiteSpace(contactIDS))
			{
				stringBuilder.AppendLine("        And a.MContactID In (" + contactIDS + ") ");
			}
			stringBuilder.AppendLine(" order by c.MName ");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MEndDate", MySqlDbType.DateTime, 36)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			array[2].Value = filter.MEndDate;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			return dataSet.Tables[0];
		}

		private static string GetContactIDS(AgedRptFilterModel filter)
		{
			if (filter.MContactIDS == null || filter.MContactIDS.Count == 0)
			{
				return "";
			}
			List<string> list = new List<string>();
			foreach (string mContactID in filter.MContactIDS)
			{
				list.Add("'" + mContactID + "'");
			}
			return string.Join(",", list);
		}

		private static BizReportCellLinkModel GetCellLink(MContext ctx, RPTAgedReportModel model)
		{
			BizReportCellLinkModel bizReportCellLinkModel = new BizReportCellLinkModel();
			bizReportCellLinkModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ShowDetails", "Show Details");
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ViewInvoice", "View Invoice");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ViewCreditNote", "View Credit Note");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ViewBill", "View Bill");
			string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ViewBillCreditNote", "View Bill Credit Note");
			switch (model.MType)
			{
			case "Invoice_Sale":
				bizReportCellLinkModel.Url = $"/IV/Invoice/InvoiceView/{model.MInvoiceID}";
				bizReportCellLinkModel.Title = text;
				break;
			case "Invoice_Sale_Red":
				bizReportCellLinkModel.Url = $"/IV/Invoice/CreditNoteView/{model.MInvoiceID}";
				bizReportCellLinkModel.Title = text2;
				break;
			case "Invoice_Purchase":
				bizReportCellLinkModel.Url = $"/IV/Bill/BillView/{model.MInvoiceID}";
				bizReportCellLinkModel.Title = text3;
				break;
			case "Invoice_Purchase_Red":
				bizReportCellLinkModel.Url = $"/IV/Bill/CreditNoteView/{model.MInvoiceID}";
				bizReportCellLinkModel.Title = text4;
				break;
			}
			return bizReportCellLinkModel;
		}
	}
}
