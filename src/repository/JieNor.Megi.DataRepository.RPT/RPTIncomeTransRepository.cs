using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Text;

namespace JieNor.Megi.DataRepository.RPT
{
	public class RPTIncomeTransRepository
	{
		private DataSet dataSet;

		private DataSet GetPerContactIncomeData(RPTIncomeTransFilterModel filter, MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select t1.MID,t1.MBizDate,t1.MReference ,t1.MCyID,ifnull(t1.MTaxTotalAmtFor,0)- ifnull(t1.MTaxAmtFor,0) as MAmountFor, ifnull(t1.MTaxTotalAmt,0)- ifnull(t1.MTaxAmt,0) as MAmount,");
			stringBuilder.AppendFormat(" case t1.MType when @Invoice_Sale then '{0}' when @Invoice_Sale_Red then '{1}' else '' end as MDetails,", COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Invoice_Sale", "Invoice"), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "CreditNote", "Credit Note"));
			stringBuilder.AppendLine(" t1.MType as MBillType,'' as MBankNo,t1.MReference as MExReference");
			stringBuilder.AppendLine(" from  T_IV_Invoice t1 ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID AND t1.MIsDelete=0  and MContactID=@MContactID and t1.MStatus>=3 and (t1.MType=@Invoice_Sale or t1.MType=@Invoice_Sale_Red) ");
			if (!isNullDate(filter.MStartDate))
			{
				stringBuilder.AppendLine(" and t1.MBizDate >= @MStartDate ");
			}
			if (!isNullDate(filter.MEndDate))
			{
				stringBuilder.AppendLine(" and t1.MBizDate <= @MEndDate ");
			}
			stringBuilder.AppendLine(" union all ");
			stringBuilder.AppendLine(" select t1.MID,t1.MBizDate,t1.MReference,t1.MCyID,ifnull(t1.MTaxTotalAmtFor,0) as MAmountFor,  ifnull(t1.MTaxTotalAmt,0)-ifnull(MVerificationAmt,0)/ifnull(MExchangeRate,1) as MAmount,");
			stringBuilder.AppendFormat(" '{0}' as MDetails,t1.MType as MBillType,t1.MBankID as MBankNo,t1.MNumber as MExReference", COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Receive", "Receipt"));
			stringBuilder.AppendLine(" from  T_IV_Receive t1");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID AND t1.MIsDelete=0  and MContactID=@MContactID and t1.MType=@Receive_Sale  AND abs(MTaxTotalAmtFor)>abs(MVerificationAmt) ");
			if (!isNullDate(filter.MStartDate))
			{
				stringBuilder.AppendLine(" and t1.MBizDate >= @MStartDate ");
			}
			if (!isNullDate(filter.MEndDate))
			{
				stringBuilder.AppendLine(" and t1.MBizDate <= @MEndDate ");
			}
			stringBuilder.AppendLine(" union all ");
			stringBuilder.AppendLine(" select t1.MID,t1.MBizDate,t1.MReference,t1.MCyID,ifnull(t1.MTaxTotalAmtFor,0) as MAmountFor, -(abs(ifnull(t1.MTaxTotalAmt,0))- abs(ifnull(MVerificationAmt,0))/abs(ifnull(MExchangeRate,1))) as MAmount,");
			stringBuilder.AppendFormat(" '{0}' as MDetails,t1.MType as MBillType,t1.MBankID as MBankNo,t1.MNumber as MExReference", COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Refund", "Refund"));
			stringBuilder.AppendLine(" from  T_IV_Payment t1");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID AND t1.MIsDelete=0  and MContactID=@MContactID and t1.MType=@Pay_PurReturn  AND abs(MTaxTotalAmtFor)>abs(MVerificationAmt) ");
			if (!isNullDate(filter.MStartDate))
			{
				stringBuilder.AppendLine(" and t1.MBizDate >= @MStartDate ");
			}
			if (!isNullDate(filter.MEndDate))
			{
				stringBuilder.AppendLine(" and t1.MBizDate <= @MEndDate ");
			}
			MySqlParameter[] array = new MySqlParameter[8]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStartDate", MySqlDbType.DateTime),
				new MySqlParameter("@MEndDate", MySqlDbType.DateTime),
				new MySqlParameter("@Invoice_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Sale_Red", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Receive_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Pay_PurReturn", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = filter.MContactID;
			array[2].Value = filter.MStartDate;
			array[3].Value = filter.MEndDate;
			array[4].Value = "Invoice_Sale";
			array[5].Value = "Invoice_Sale_Red";
			array[6].Value = "Receive_Sale";
			array[7].Value = "Pay_PurReturn";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
		}

		public BizReportModel GetIncomeTransactionsList(RPTIncomeTransFilterModel filter, MContext ctx)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.IncomeTransactions);
			SetDataSet(filter, ctx);
			SetTitle(bizReportModel, filter, ctx);
			SetRowHead(bizReportModel, filter, ctx);
			decimal sumAmount = default(decimal);
			SetRowData(bizReportModel, filter, ctx, out sumAmount);
			SetDataSummary(bizReportModel, filter, ctx, sumAmount);
			return bizReportModel;
		}

		private void SetTitle(BizReportModel model, RPTIncomeTransFilterModel filter, MContext context)
		{
			model.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "IncomeTransactions", "Income Transactions");
			model.Title2 = context.MOrgName;
			DateTime dateTime;
			if (!isNullDate(filter.MStartDate))
			{
				string text = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "From", "From");
				dateTime = filter.MStartDate;
				model.Title3 = text + " " + dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(context))) + " ";
			}
			if (!isNullDate(filter.MEndDate))
			{
				string title = model.Title3;
				string text2 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "To", "to");
				dateTime = filter.MEndDate;
				model.Title3 = title + text2 + " " + dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(context)));
			}
		}

		private void SetRowHead(BizReportModel model, RPTIncomeTransFilterModel filter, MContext context)
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
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Common, "BizBillType", "Type"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Reference", "Reference"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total"),
				CellType = BizReportCellType.Money
			});
			if (isIncludeForeignCurr(context))
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total") + " " + GetOrgDefaultCurrency(context),
					CellType = BizReportCellType.Money
				});
			}
			model.AddRow(bizReportRowModel);
		}

		private void SetRowData(BizReportModel model, RPTIncomeTransFilterModel filter, MContext context, out decimal sumAmount)
		{
			decimal num = default(decimal);
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.RowType = BizReportRowType.Item;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToDateTime(row["MBizDate"]).ToString("yyyy-MM-dd"),
					CellType = BizReportCellType.Text,
					CellLink = GetCellLink(context, row)
				});
				string empty = string.Empty;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MDetails"]),
					CellType = BizReportCellType.Text,
					CellLink = GetCellLink(context, row)
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MReference"]),
					CellType = BizReportCellType.Text,
					CellLink = GetCellLink(context, row)
				});
				if (isIncludeForeignCurr(context))
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(row["MAmountFor"])
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = (Convert.ToString(row["MCyID"]).Equals(GetOrgDefaultCurrency(context)) ? string.Empty : Convert.ToString(row["MCyID"])),
						CellType = BizReportCellType.Text
					});
				}
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MAmount"])
				});
				num += Convert.ToDecimal(row["MAmount"]);
				model.AddRow(bizReportRowModel);
			}
			sumAmount = num;
		}

		private void SetDataSummary(BizReportModel model, RPTIncomeTransFilterModel filter, MContext context, decimal sumAmount)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Total;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total"),
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
			if (isIncludeForeignCurr(context))
			{
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
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(sumAmount)
			});
			model.AddRow(bizReportRowModel);
		}

		private void SetDataSet(RPTIncomeTransFilterModel filter, MContext context)
		{
			dataSet = GetPerContactIncomeData(filter, context);
		}

		private bool isNullDate(DateTime date)
		{
			if (date.ToString("yyyy-MM-dd").Equals("0001-01-01", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return false;
		}

		private string GetOrgDefaultCurrency(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" select MCurrencyID  from T_REG_Financial ");
			stringBuilder.Append(" where MIsDelete=0 and MOrgID=@MOrgID ");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dataTable = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0];
			if (dataTable.Rows.Count > 0)
			{
				return dataTable.Rows[0]["MCurrencyID"].ToString();
			}
			return string.Empty;
		}

		private bool isIncludeForeignCurr(MContext context)
		{
			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				if (!Convert.ToString(row["MCyID"]).Equals(GetOrgDefaultCurrency(context), StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private BizReportCellLinkModel GetCellLink(MContext ctx, DataRow data)
		{
			BizReportCellLinkModel bizReportCellLinkModel = new BizReportCellLinkModel();
			bizReportCellLinkModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ShowDetails", "Show Details");
			switch (Convert.ToString(data["MBillType"]))
			{
			case "Invoice_Sale":
				bizReportCellLinkModel.Url = string.Format("/IV/Invoice/InvoiceView/{0}", Convert.ToString(data["MID"]));
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewInvoice", "View Invoice");
				break;
			case "Invoice_Sale_Red":
				bizReportCellLinkModel.Url = string.Format("/IV/Invoice/CreditNoteView/{0}", Convert.ToString(data["MID"]));
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewCreditNote", "View Credit Note");
				break;
			case "Receive_Sale":
				bizReportCellLinkModel.Url = string.Format("/IV/Receipt/ReceiptView/{0}?acctId={1}", Convert.ToString(data["MID"]), Convert.ToString(data["MBankNo"]));
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ReceiveMoney", "Receive Money");
				break;
			case "Pay_PurReturn":
				bizReportCellLinkModel.Url = string.Format("/IV/Payment/PaymentView/{0}?acctId={1}", Convert.ToString(data["MID"]), Convert.ToString(data["MBankNo"]));
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Refund", "Refund");
				break;
			}
			return bizReportCellLinkModel;
		}
	}
}
