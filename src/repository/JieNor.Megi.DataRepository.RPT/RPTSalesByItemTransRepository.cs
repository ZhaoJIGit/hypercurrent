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
	public class RPTSalesByItemTransRepository
	{
		private static DataSet GetItemSalse(RPTSalesByItemTransFilterModel model, MContext context)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select t1.MID,t1.MBizDate,t1.MCyID,convert(AES_DECRYPT(t3.MName,'{0}') using utf8) as MContactName,t4.MDesc as MItemDesc,t2.MPrice,t2.MDiscount, ", "JieNor-001");
			stringBuilder.AppendLine(" ifnull(t2.MQty,0) as MQty,ifnull(t2.MTaxAmount,0) as MAmount,ifnull(t2.MTaxAmountFor,0) as MAmountFor, ");
			stringBuilder.AppendLine(" t1.MType as MBillType,'' as MBankNo,t1.MNumber,t1.MReference ");
			stringBuilder.AppendLine(" from  T_IV_Invoice t1 ");
			stringBuilder.AppendLine(" inner join T_IV_InvoiceEntry t2 on t1.MOrgID = t2.MOrgID and  t1.MID=t2.MID AND t2.MIsDelete=0  ");
			stringBuilder.AppendLine(" inner join T_BD_Contacts_l t3 on t1.MOrgID = t3.MOrgID and  t1.MContactID=t3.MParentID and t3.MLocaleID=@MLCID AND t3.MIsDelete=0  ");
			stringBuilder.AppendLine(" inner join T_BD_Item_L t4 on t1.MOrgID = t4.MOrgID and  t2.MItemID=t4.MParentID and t4.MLocaleID=@MLCID AND t4.MIsDelete=0  ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID and t1.MIsDelete=0  and t2.MItemID=@MItemID and t1.MStatus>=3 and t1.MBizDate>= @MStartDate and t1.MBizDate<=@MEndDate ");
			stringBuilder.AppendLine(" and (t1.MType=@Invoice_Sale or t1.MType=@Invoice_Sale_Red) ");
			stringBuilder.AppendLine(GetCurrencySql(model.MCurrency));
			stringBuilder.AppendLine(" union all ");
			stringBuilder.AppendFormat(" select t1.MID,t1.MBizDate,t1.MCyID,convert(AES_DECRYPT(t3.MName,'{0}') using utf8) as MContactName,t4.MDesc as MItemDesc,t2.MPrice,t2.MDiscount, ", "JieNor-001");
			stringBuilder.AppendLine(" ifnull(t2.MQty,0) as MQty,ifnull(t2.MTaxAmount,0) as MAmount,ifnull(t2.MTaxAmountFor,0) as MAmountFor, ");
			stringBuilder.AppendLine(" t1.MType as MBillType,t1.MBankID as MBankNo,t1.MNumber,t1.MReference ");
			stringBuilder.AppendLine(" from  T_IV_Receive t1");
			stringBuilder.AppendLine(" inner join T_IV_ReceiveEntry t2 on  t1.MOrgID = t2.MOrgID and t1.MID=t2.MID AND t2.MIsDelete=0  ");
			stringBuilder.AppendLine(" inner join T_BD_Contacts_l t3 on t1.MOrgID = t3.MOrgID and  t1.MContactID=t3.MParentID and t3.MLocaleID=@MLCID AND t3.MIsDelete=0  ");
			stringBuilder.AppendLine(" inner join T_BD_Item_L t4 on t1.MOrgID = t4.MOrgID and  t2.MItemID=t4.MParentID and t4.MLocaleID=@MLCID AND t4.MIsDelete=0 ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID and t1.MIsDelete=0  and t2.MItemID=@MItemID and t1.MBizDate>= @MStartDate and t1.MBizDate<=@MEndDate ");
			stringBuilder.AppendLine(" and t1.MType=@Receive_Sale");
			stringBuilder.AppendLine(GetCurrencySql(model.MCurrency));
			stringBuilder.AppendLine(" union all ");
			stringBuilder.AppendFormat(" select t1.MID,t1.MBizDate,t1.MCyID,convert(AES_DECRYPT(t3.MName,'{0}') using utf8) as MContactName,t4.MDesc as MItemDesc,t2.MPrice,t2.MDiscount, ", "JieNor-001");
			stringBuilder.AppendLine(" ifnull(t2.MQty,0) as MQty,-ifnull(t2.MTaxAmount,0) as MAmount,-ifnull(t2.MTaxAmountFor,0) as MAmountFor, ");
			stringBuilder.AppendLine(" t1.MType as MBillType,t1.MBankID as MBankNo,t1.MNumber,t1.MReference ");
			stringBuilder.AppendLine(" from  T_IV_Payment t1");
			stringBuilder.AppendLine(" inner join T_IV_PaymentEntry t2 on  t1.MOrgID = t2.MOrgID and t1.MID=t2.MID AND t2.MIsDelete=0  ");
			stringBuilder.AppendLine(" inner join T_BD_Contacts_l t3 on  t1.MOrgID = t3.MOrgID and t1.MContactID=t3.MParentID and t3.MLocaleID=@MLCID  AND t3.MIsDelete=0  ");
			stringBuilder.AppendLine(" inner join T_BD_Item_L t4 on t1.MOrgID = t4.MOrgID and  t2.MItemID=t4.MParentID and t4.MLocaleID=@MLCID AND t4.MIsDelete=0 ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID and t1.MIsDelete=0  and t2.MItemID=@MItemID and t1.MBizDate>= @MStartDate and t1.MBizDate<=@MEndDate ");
			stringBuilder.AppendLine(" and t1.MType=@Pay_PurReturn");
			stringBuilder.AppendLine(GetCurrencySql(model.MCurrency));
			stringBuilder.AppendLine(" order by MBizDate,MContactName,MAmountFor");
			MySqlParameter[] array = new MySqlParameter[10]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStartDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MEndDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Sale_Red", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Receive_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Pay_PurReturn", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MOrgID;
			array[1].Value = context.MLCID;
			array[2].Value = model.MItem;
			array[3].Value = model.MCurrency;
			DateTime dateTime = DateTime.Now;
			dateTime = dateTime.Date;
			DateTime dateTime2 = DateTime.Now;
			dateTime2 = dateTime2.Date;
			DateTime dateTime3 = dateTime.AddDays((double)(1 - dateTime2.Day));
			MySqlParameter obj = array[4];
			DateTime mStartDate = model.MStartDate;
			dateTime = Convert.ToDateTime(model.MStartDate);
			obj.Value = dateTime.ToString("yyyy-MM-dd");
			MySqlParameter obj2 = array[5];
			DateTime mEndDate = model.MEndDate;
			dateTime = Convert.ToDateTime(model.MEndDate);
			obj2.Value = dateTime.ToString("yyyy-MM-dd");
			array[6].Value = "Invoice_Sale";
			array[7].Value = "Invoice_Sale_Red";
			array[8].Value = "Receive_Sale";
			array[9].Value = "Pay_PurReturn";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(context);
			return dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
		}

		private static string GetCurrencySql(string currencyNumber)
		{
			if (string.IsNullOrWhiteSpace(currencyNumber) || currencyNumber.Equals("All Foreign Currencies", StringComparison.OrdinalIgnoreCase))
			{
				return string.Empty;
			}
			return " and t1.MCyID=@MCyID";
		}

		private static string GetOrgDefaultCurrency(MContext ctx)
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

		private static bool IsAllCurrency(string currencyNumber)
		{
			if (string.IsNullOrWhiteSpace(currencyNumber) || currencyNumber.Equals("All Foreign Currencies", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return false;
		}

		public static BizReportModel GetSalesByItemTransactions(RPTSalesByItemTransFilterModel filter, MContext context)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.SalesByItem);
			SetTitle(bizReportModel, filter, context);
			SetRowHead(bizReportModel, filter, context);
			decimal sumQty = default(decimal);
			decimal sumAmount = default(decimal);
			SetRowData(bizReportModel, filter, context, out sumQty, out sumAmount);
			SetDataSummary(bizReportModel, filter, context, sumQty, sumAmount);
			return bizReportModel;
		}

		private static void SetTitle(BizReportModel model, RPTSalesByItemTransFilterModel filter, MContext context)
		{
			model.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ItemSales", "Item Sales");
			model.Title2 = context.MOrgName;
			string[] obj = new string[7]
			{
				COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ForThePeriod", "For the period "),
				" ",
				null,
				null,
				null,
				null,
				null
			};
			DateTime dateTime = filter.MStartDate;
			obj[2] = dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(context)));
			obj[3] = " ";
			obj[4] = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "To", "to");
			obj[5] = " ";
			dateTime = filter.MEndDate;
			obj[6] = dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(context)));
			model.Title3 = string.Concat(obj);
		}

		private static void SetRowHead(BizReportModel model, RPTSalesByItemTransFilterModel filter, MContext context)
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
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.IV, "IVTo", "To"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Description", "Description"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Quantity", "Quantity"),
				CellType = BizReportCellType.Money
			});
			if (!IsAllCurrency(filter.MCurrency) && !filter.MCurrency.Equals(GetOrgDefaultCurrency(context)))
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "UnitPrice", "Unit Price") + " " + filter.MCurrency,
					CellType = BizReportCellType.Price
				});
			}
			else
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "UnitPrice", "Unit Price"),
					CellType = BizReportCellType.Price
				});
			}
			if (IsAllCurrency(filter.MCurrency))
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Discount", "Disc %"),
				CellType = BizReportCellType.Money
			});
			if (!IsAllCurrency(filter.MCurrency) && !filter.MCurrency.Equals(GetOrgDefaultCurrency(context)))
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total") + " " + filter.MCurrency,
					CellType = BizReportCellType.Money
				});
			}
			else
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total"),
					CellType = BizReportCellType.Money
				});
			}
			if (IsAllCurrency(filter.MCurrency))
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total") + " " + GetOrgDefaultCurrency(context),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private static void SetRowData(BizReportModel model, RPTSalesByItemTransFilterModel filter, MContext context, out decimal sumQty, out decimal sumAmount)
		{
			decimal num = default(decimal);
			decimal num2 = default(decimal);
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			DataSet itemSalse = GetItemSalse(filter, context);
			foreach (DataRow row in itemSalse.Tables[0].Rows)
			{
				bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.RowType = BizReportRowType.Item;
				string empty = string.Empty;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToDateTime(row["MBizDate"]).ToString("yyyy-MM-dd"),
					CellType = BizReportCellType.Text,
					CellLink = GetCellLink(context, row)
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MContactName"]),
					CellType = BizReportCellType.Text,
					CellLink = GetCellLink(context, row)
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MItemDesc"]),
					CellType = BizReportCellType.Text,
					CellLink = GetCellLink(context, row)
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MQty"])
				});
				num += Convert.ToDecimal(row["MQty"]);
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MPrice"]),
					CellType = BizReportCellType.Price
				});
				if (IsAllCurrency(filter.MCurrency))
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(row["MCyID"]),
						CellType = BizReportCellType.Text
					});
				}
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MDiscount"])
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MAmountFor"])
				});
				if (IsAllCurrency(filter.MCurrency))
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(row["MCyID"]),
						CellType = BizReportCellType.Text
					});
				}
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MAmount"])
				});
				num2 += Convert.ToDecimal(row["MAmount"]);
				model.AddRow(bizReportRowModel);
			}
			sumQty = num;
			sumAmount = num2;
		}

		private static void SetDataSummary(BizReportModel model, RPTSalesByItemTransFilterModel filter, MContext context, decimal sumQty, decimal sumAmount)
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
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(sumQty)
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = " ",
				CellType = BizReportCellType.Text
			});
			if (IsAllCurrency(filter.MCurrency))
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
				Value = Convert.ToString(sumAmount)
			});
			model.AddRow(bizReportRowModel);
		}

		private static BizReportCellLinkModel GetCellLink(MContext ctx, DataRow data)
		{
			BizReportCellLinkModel bizReportCellLinkModel = new BizReportCellLinkModel();
			bizReportCellLinkModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ShowDetails", "Show Details");
			switch (Convert.ToString(data["MBillType"]))
			{
			case "Invoice_Sale":
				bizReportCellLinkModel.Url = string.Format("/IV/Invoice/InvoiceView/{0}", Convert.ToString(data["MID"]));
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ViewInvoice", "View Invoice");
				break;
			case "Invoice_Sale_Red":
				bizReportCellLinkModel.Url = string.Format("/IV/Invoice/CreditNoteView/{0}", Convert.ToString(data["MID"]));
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ViewCreditNote", "View Credit Note");
				break;
			case "Receive_Sale":
				bizReportCellLinkModel.Url = string.Format("/IV/Receipt/ReceiptView/{0}?acctId={1}", Convert.ToString(data["MID"]), Convert.ToString(data["MBankNo"]));
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "ReceiveMoney", "Receive Money");
				break;
			case "Pay_PurReturn":
				bizReportCellLinkModel.Url = string.Format("/IV/Payment/PaymentView/{0}?acctId={1}", Convert.ToString(data["MID"]), Convert.ToString(data["MBankNo"]));
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "ViewRefund", "View Refund");
				break;
			}
			return bizReportCellLinkModel;
		}
	}
}
