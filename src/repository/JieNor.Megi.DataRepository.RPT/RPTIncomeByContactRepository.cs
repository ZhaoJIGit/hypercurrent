using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.RPT
{
	public class RPTIncomeByContactRepository
	{
		private DateTime dataStartDate;

		private DateTime dataEndDate;

		private List<string> dataColumns;

		private List<RPTIncomeByContactModel> dataList;

		private List<RPTIncomeByContactModel> GetPerPeriodIncomeData(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select t2.MParentID as MContactID,convert(AES_DECRYPT(t2.MName,'{0}') using utf8) as MContactName,t1.MBizDate, ifnull(t1.MTaxTotalAmt,0) - ifnull(t1.MTaxAmt,0) as MAmount", "JieNor-001");
			stringBuilder.AppendLine(" from  T_IV_Invoice t1 ");
			stringBuilder.AppendLine(" inner join T_BD_Contacts_L t2 on  t1.MOrgID = t2.MOrgID and t1.MContactID=t2.MParentID and t2.MLocaleID=@MLCID AND t2.MIsDelete=0 ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID AND t1.MIsDelete=0 AND  t1.MStatus>=3 and t1.MBizDate>= @MStartDate and t1.MBizDate<=@MEndDate ");
			stringBuilder.AppendLine(" and (t1.MType=@Invoice_Sale or t1.MType=@Invoice_Sale_Red) ");
			stringBuilder.AppendLine(" union all ");
			stringBuilder.AppendFormat(" select t2.MParentID as MContactID,convert(AES_DECRYPT(t2.MName,'{0}') using utf8) as MContactName,t1.MBizDate, ifnull(t1.MTaxTotalAmt,0)-ifnull(MVerificationAmt,0)/ifnull(MExchangeRate,1) as MAmount", "JieNor-001");
			stringBuilder.AppendLine(" from  T_IV_Receive t1");
			stringBuilder.AppendLine(" inner join T_BD_Contacts_L t2 on  t1.MOrgID = t2.MOrgID and t1.MContactID=t2.MParentID and t2.MLocaleID=@MLCID AND t2.MIsDelete=0 ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID AND t1.MIsDelete=0 AND  t1.MBizDate>= @MStartDate and t1.MBizDate<=@MEndDate ");
			stringBuilder.AppendLine(" and t1.MType=@Receive_Sale AND abs(MTaxTotalAmtFor)>abs(MVerificationAmt)");
			stringBuilder.AppendLine(" union all ");
			stringBuilder.AppendFormat(" select t2.MParentID as MContactID,convert(AES_DECRYPT(t2.MName,'{0}') using utf8) as MContactName,t1.MBizDate, -(abs(ifnull(t1.MTaxTotalAmt,0))- abs(ifnull(MVerificationAmt,0))/abs(ifnull(MExchangeRate,1))) as MAmount", "JieNor-001");
			stringBuilder.AppendLine(" from  T_IV_Payment t1");
			stringBuilder.AppendLine(" inner join T_BD_Contacts_L t2 on t1.MOrgID = t2.MOrgID and  t1.MContactID=t2.MParentID and t2.MLocaleID=@MLCID AND t2.MIsDelete=0 ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID AND t1.MIsDelete=0 AND  t1.MBizDate>= @MStartDate and t1.MBizDate<=@MEndDate ");
			stringBuilder.AppendLine(" and t1.MType=@Pay_PurReturn AND abs(MTaxTotalAmtFor)>abs(MVerificationAmt)");
			MySqlParameter[] array = new MySqlParameter[8]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStartDate", MySqlDbType.DateTime),
				new MySqlParameter("@MEndDate", MySqlDbType.DateTime),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@Invoice_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Sale_Red", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Receive_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Pay_PurReturn", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = dataStartDate;
			array[2].Value = dataEndDate;
			array[3].Value = ctx.MLCID;
			array[4].Value = "Invoice_Sale";
			array[5].Value = "Invoice_Sale_Red";
			array[6].Value = "Receive_Sale";
			array[7].Value = "Pay_PurReturn";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<RPTIncomeByContactModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public BizReportModel GetIncomeByContactList(RPTIncomeByContactFilterModel filter, MContext ctx)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.IncomeByContact);
			SetCommonData(filter, ctx);
			SetTitle(bizReportModel, filter, ctx);
			SetRowHead(bizReportModel, filter, ctx);
			SetRowData(bizReportModel, filter, ctx);
			SetDataSummary(bizReportModel, filter, ctx);
			return bizReportModel;
		}

		private void SetTitle(BizReportModel model, RPTIncomeByContactFilterModel filter, MContext context)
		{
			model.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "IncomebyContact", "Income by Contact");
			model.Title2 = context.MOrgName;
			string text = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "IncomeByContact_title3", "For the {0} ended {1}");
			string titlePeriod = GetTitlePeriod(context, filter.MPeriod);
			DateTime dateTime = filter.MEndDate;
			dateTime = dateTime.AddMonths(1);
			dateTime = dateTime.AddDays(-1.0);
			model.Title3 = string.Format(text, titlePeriod, dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(context))));
		}

		private void SetRowHead(BizReportModel model, RPTIncomeByContactFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Contact", "Contact"),
				CellType = BizReportCellType.Text
			});
			foreach (string dataColumn in dataColumns)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = dataColumn,
					CellType = BizReportCellType.Money
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total"),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private void SetRowData(BizReportModel model, RPTIncomeByContactFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			IEnumerable<IGrouping<string, RPTIncomeByContactModel>> enumerable = from gr in dataList
			group gr by gr.MContactID;
			foreach (IGrouping<string, RPTIncomeByContactModel> item in enumerable)
			{
				bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.RowType = BizReportRowType.Item;
				string mContactID = item.ToList()[0].MContactID;
				string mContactName = item.ToList()[0].MContactName;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = mContactName,
					CellType = BizReportCellType.Text,
					SubReport = GetBizSubRptModel(context, mContactID, dataStartDate, dataEndDate)
				});
				for (int i = 0; i < dataColumns.Count; i++)
				{
					DateTime startDate = dataEndDate.AddDays(1.0).AddMonths(-(filter.MPeriod * (i + 1)));
					DateTime endDate = dataEndDate.AddMonths(-(filter.MPeriod * i));
					decimal value = (from w in item.ToList()
					where w.MBizDate >= startDate && w.MBizDate <= endDate
					select w).Sum((RPTIncomeByContactModel s) => s.MAmount);
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(value),
						CellType = BizReportCellType.Money,
						SubReport = GetBizSubRptModel(context, mContactID, startDate, endDate)
					});
				}
				decimal value2 = item.ToList().Sum((RPTIncomeByContactModel s) => s.MAmount);
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(value2),
					CellType = BizReportCellType.Money,
					SubReport = GetBizSubRptModel(context, mContactID, dataStartDate, dataEndDate)
				});
				model.AddRow(bizReportRowModel);
			}
		}

		private void SetDataSummary(BizReportModel model, RPTIncomeByContactFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Total;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total"),
				CellType = BizReportCellType.Text
			});
			for (int i = 0; i < dataColumns.Count; i++)
			{
				DateTime startDate = dataEndDate.AddMonths(-(filter.MPeriod * (i + 1))).AddDays(1.0);
				DateTime endDate = dataEndDate.AddMonths(-(filter.MPeriod * i));
				decimal value = (from w in dataList
				where w.MBizDate >= startDate && w.MBizDate <= endDate
				select w).Sum((RPTIncomeByContactModel s) => s.MAmount);
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(value)
				});
			}
			decimal value2 = dataList.Sum((RPTIncomeByContactModel s) => s.MAmount);
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(value2)
			});
			model.AddRow(bizReportRowModel);
		}

		private void SetCommonData(RPTIncomeByContactFilterModel filter, MContext context)
		{
			dataColumns = new List<string>();
			dataList = new List<RPTIncomeByContactModel>();
			DateTime dateTime = filter.MEndDate;
			dateTime = dateTime.Date;
			DateTime dateTime2 = filter.MEndDate;
			dateTime2 = dateTime2.Date;
			DateTime dateTime3 = dateTime.AddDays((double)(1 - dateTime2.Day));
			dataStartDate = dateTime3.AddMonths(1 - filter.MPeriod * (filter.MPeriodCount + 1));
			dateTime = dateTime3.AddMonths(1);
			dataEndDate = dateTime.AddDays(-1.0);
			for (int i = 0; i <= filter.MPeriodCount; i++)
			{
				DateTime dateTime4 = dateTime3.AddMonths(-(filter.MPeriod * i));
				string mLCID = context.MLCID;
				DateMonth month = (DateMonth)dateTime4.Month;
				string key = month.ToString();
				month = (DateMonth)dateTime4.Month;
				string item = COMMultiLangRepository.GetText(mLCID, LangModule.Report, key, month.ToString()) + "-" + dateTime4.Year.ToString().Substring(2);
				dataColumns.Add(item);
			}
			dataList = GetPerPeriodIncomeData(context);
		}

		private string GetTitlePeriod(MContext ctx, int period)
		{
			switch (period)
			{
			case 1:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "month", "month");
			case 2:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "2months", "2 months");
			case 3:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "3months", "3 months");
			case 6:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "6months", "6 months");
			case 12:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "12months", "12 months");
			default:
				return "";
			}
		}

		private BizSubRptCreateModel GetBizSubRptModel(MContext ctx, string contactId, DateTime startDate, DateTime endDate)
		{
			BizSubRptCreateModel bizSubRptCreateModel = new BizSubRptCreateModel();
			bizSubRptCreateModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Showtransactions", "Show transactions");
			bizSubRptCreateModel.ReportType = BizReportType.IncomeTransactions;
			RPTIncomeTransFilterModel rPTIncomeTransFilterModel = new RPTIncomeTransFilterModel();
			rPTIncomeTransFilterModel.MContactID = contactId;
			rPTIncomeTransFilterModel.MStartDate = startDate;
			rPTIncomeTransFilterModel.MEndDate = endDate;
			bizSubRptCreateModel.ReportFilter = rPTIncomeTransFilterModel;
			return bizSubRptCreateModel;
		}
	}
}
