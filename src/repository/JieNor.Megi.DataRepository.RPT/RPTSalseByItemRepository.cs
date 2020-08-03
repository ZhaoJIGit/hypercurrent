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
	public class RPTSalseByItemRepository
	{
		private static DataSet GetSalesByItemList(RPTSalseByItemFilterModel model, MContext context)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (model.ShowInCNY)
			{
				stringBuilder.AppendLine(" select MItemID,MNumber, concat(MNumber,'(',MDesc,')') as MName,MSalPrice,MDesc, ");
				stringBuilder.AppendLine(" ifnull(sum(MTotQty),0) as MTotQty,ifnull(sum(MTotAmount),0) as MTotAmount ");
				stringBuilder.AppendLine(" from( ");
			}
			stringBuilder.AppendLine(" select MItemID,MNumber,concat(MNumber,'(',MDesc,')') as MName,MSalPrice,MDesc,MCyID, ");
			stringBuilder.AppendLine(" ifnull(sum(MQty),0) as MTotQty,ifnull(sum(MAmount),0) as MTotAmount,ifnull(sum(MAmountFor),0) as MTotAmountFor ");
			stringBuilder.AppendLine(" from( ");
			stringBuilder.AppendLine(" select t3.MItemID,t3.MNumber,t4.MName,t3.MSalPrice,t4.MDesc,t1.MCyID, ");
			stringBuilder.AppendLine(" ifnull(t2.MQty,0) as MQty,ifnull(t2.MTaxAmount,0) as MAmount,ifnull(t2.MTaxAmountFor,0) as MAmountFor");
			stringBuilder.AppendLine(" from  T_IV_Invoice t1 ");
			stringBuilder.AppendLine(" inner join T_IV_InvoiceEntry t2 on t2.MOrgID = t1.MOrgID and  t1.MID=t2.MID AND t2.MIsDelete=0  ");
			stringBuilder.AppendLine(" inner join T_BD_Item t3 on t3.MOrgID = t1.MOrgID and  t2.MItemID=t3.MItemID AND t3.MIsDelete=0  ");
			stringBuilder.AppendLine(" inner join T_BD_Item_L t4 on t4.MOrgID = t1.MOrgID and t3.MItemID=t4.MParentID and t4.MLocaleID=@MLCID AND t4.MIsDelete=0 ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID  and t1.MIsDelete=0 and t1.MStatus>=3 and t1.MBizDate>= @MStartDate and t1.MBizDate<=@MEndDate ");
			stringBuilder.AppendLine(" and (t1.MType=@Invoice_Sale or t1.MType=@Invoice_Sale_Red) ");
			stringBuilder.AppendLine(" )t  ");
			stringBuilder.AppendLine(" group by MItemID,MNumber,MName,MSalPrice,MDesc,MCyID ");
			if (model.ShowInCNY)
			{
				stringBuilder.AppendLine(" )y  ");
				stringBuilder.AppendLine(" group by MItemID,MNumber,MName,MSalPrice,MDesc  ");
			}
			if (model.MSortBy == "1")
			{
				stringBuilder.AppendLine("order by MNumber,MDesc desc ");
			}
			else if (model.MSortBy == "2")
			{
				stringBuilder.AppendLine("order by MDesc,MNumber desc ");
			}
			MySqlParameter[] array = new MySqlParameter[8]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStartDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MEndDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@Invoice_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Sale_Red", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Receive_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Pay_PurReturn", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MOrgID;
			DateTime dateTime = DateTime.Now;
			dateTime = dateTime.Date;
			DateTime dateTime2 = DateTime.Now;
			dateTime2 = dateTime2.Date;
			DateTime dateTime3 = dateTime.AddDays((double)(1 - dateTime2.Day));
			MySqlParameter obj = array[1];
			DateTime mStartDate = model.MStartDate;
			dateTime = Convert.ToDateTime(model.MStartDate);
			obj.Value = dateTime.ToString("yyyy-MM-dd");
			MySqlParameter obj2 = array[2];
			DateTime mEndDate = model.MEndDate;
			dateTime = Convert.ToDateTime(model.MEndDate);
			obj2.Value = dateTime.ToString("yyyy-MM-dd");
			array[3].Value = context.MLCID;
			array[4].Value = "Invoice_Sale";
			array[5].Value = "Invoice_Sale_Red";
			array[6].Value = "Receive_Sale";
			array[7].Value = "Pay_PurReturn";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(context);
			return dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
		}

		private static decimal GetOthersalse(RPTSalseByItemFilterModel model, MContext context)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select ifnull((( ");
			stringBuilder.AppendLine(" select ifnull(Sum(ifnull(MTaxTotalAmt,0)-ifnull(MVerifyAmt,0)),0) as MAmount ");
			stringBuilder.AppendLine(" from  T_IV_Receive t1 ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID  and t1.MIsDelete=0 AND  ifnull(MTaxTotalAmtFor,0)-ifnull(MVerificationAmt,0)>0");
			stringBuilder.AppendLine(" and t1.MBizDate>= @MStartDate and t1.MBizDate<=@MEndDate and t1.MType='Receive_Sale')");
			stringBuilder.AppendLine(" - ");
			stringBuilder.AppendLine(" (select ifnull(Sum(ifnull(MTaxTotalAmt,0)-ifnull(abs(MVerifyAmt),0)),0) as MAmount ");
			stringBuilder.AppendLine(" from  T_IV_Payment t1 ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID  and t1.MIsDelete=0 AND  ifnull(MTaxTotalAmtFor,0)-ifnull(MVerificationAmt,0)>0 ");
			stringBuilder.AppendLine(" and t1.MBizDate>= @MStartDate and t1.MBizDate<=@MEndDate and t1.MType='Pay_PurReturn'");
			stringBuilder.AppendLine(" )),0) as  MTotAmount");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStartDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MEndDate", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MOrgID;
			DateTime dateTime = DateTime.Now;
			dateTime = dateTime.Date;
			DateTime dateTime2 = DateTime.Now;
			dateTime2 = dateTime2.Date;
			DateTime dateTime3 = dateTime.AddDays((double)(1 - dateTime2.Day));
			MySqlParameter obj = array[1];
			DateTime mStartDate = model.MStartDate;
			dateTime = Convert.ToDateTime(model.MStartDate);
			obj.Value = dateTime.ToString("yyyy-MM-dd");
			MySqlParameter obj2 = array[2];
			DateTime mEndDate = model.MEndDate;
			dateTime = Convert.ToDateTime(model.MEndDate);
			obj2.Value = dateTime.ToString("yyyy-MM-dd");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(context);
			DataTable dataTable = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0];
			if (dataTable.Rows.Count > 0)
			{
				return Convert.ToDecimal(dataTable.Rows[0]["MTotAmount"]);
			}
			return decimal.Zero;
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

		public static BizReportModel GetSalesByItem(RPTSalseByItemFilterModel filter, MContext context)
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

		private static void SetTitle(BizReportModel model, RPTSalseByItemFilterModel filter, MContext context)
		{
			model.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "SalesByItem", "Sales by Item");
			model.Title2 = context.MOrgName;
			string[] obj = new string[7]
			{
				COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ForThePeriod", "For the period"),
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

		private static void SetRowHead(BizReportModel model, RPTSalseByItemFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Item", "Item"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "CurrentUnitPrice", "Current Unit Price"),
				CellType = BizReportCellType.Price
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "QuantitySold", "Quantity Sold"),
				CellType = BizReportCellType.Money
			});
			if (!filter.ShowInCNY)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total") + " " + GetOrgDefaultCurrency(context),
					CellType = BizReportCellType.Money
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total"),
				CellType = BizReportCellType.Money
			});
			if (!filter.ShowInCNY)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Money
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "AveragePrice", "Average Price"),
				CellType = BizReportCellType.Price
			});
			if (!filter.ShowInCNY)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Money
				});
			}
			model.AddRow(bizReportRowModel);
		}

		private static void SetRowData(BizReportModel model, RPTSalseByItemFilterModel filter, MContext context, out decimal sumQty, out decimal sumAmount)
		{
			decimal num = default(decimal);
			decimal num2 = default(decimal);
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			DataSet salesByItemList = GetSalesByItemList(filter, context);
			foreach (DataRow row in salesByItemList.Tables[0].Rows)
			{
				BizSubRptCreateModel bizSubRptCreateModel = new BizSubRptCreateModel();
				bizSubRptCreateModel.Text = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Showtransactions", "Show transactions");
				bizSubRptCreateModel.ReportType = BizReportType.SalesByItemTransactions;
				RPTSalesByItemTransFilterModel rPTSalesByItemTransFilterModel = new RPTSalesByItemTransFilterModel();
				rPTSalesByItemTransFilterModel.MStartDate = filter.MStartDate;
				rPTSalesByItemTransFilterModel.MEndDate = filter.MEndDate;
				bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.RowType = BizReportRowType.Item;
				rPTSalesByItemTransFilterModel.MItem = Convert.ToString(row["MItemID"]);
				if (!filter.ShowInCNY)
				{
					rPTSalesByItemTransFilterModel.MCurrency = Convert.ToString(row["MCyID"]);
				}
				else
				{
					rPTSalesByItemTransFilterModel.MCurrency = "All Foreign Currencies";
				}
				bizSubRptCreateModel.ReportFilter = rPTSalesByItemTransFilterModel;
				if (filter.MSortBy == "1")
				{
					if (string.IsNullOrWhiteSpace(Convert.ToString(row["MDesc"])))
					{
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = Convert.ToString(row["MNumber"]),
							CellType = BizReportCellType.Text,
							SubReport = bizSubRptCreateModel
						});
					}
					else
					{
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = Convert.ToString(row["MNumber"]) + "-" + Convert.ToString(row["MDesc"]),
							CellType = BizReportCellType.Text,
							SubReport = bizSubRptCreateModel
						});
					}
				}
				else if (filter.MSortBy == "2")
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = string.Format("{0}({1})", Convert.ToString(row["MDesc"]), Convert.ToString(row["MNumber"])),
						CellType = BizReportCellType.Text,
						SubReport = bizSubRptCreateModel
					});
				}
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MSalPrice"]),
					CellType = BizReportCellType.Price
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MTotQty"])
				});
				num += (decimal)Convert.ToInt32(row["MTotQty"]);
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(row["MTotAmount"])
				});
				num2 += Convert.ToDecimal(row["MTotAmount"]);
				if (!filter.ShowInCNY)
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(row["MTotAmountFor"])
					});
				}
				if (!filter.ShowInCNY)
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = (Convert.ToString(row["MCyID"]).Equals(GetOrgDefaultCurrency(context)) ? string.Empty : Convert.ToString(row["MCyID"])),
						CellType = BizReportCellType.Text
					});
				}
				if (!filter.ShowInCNY)
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(Convert.ToDecimal(row["MTotAmountFor"]) / ((Convert.ToDecimal(row["MTotQty"]) == decimal.Zero) ? decimal.One : Convert.ToDecimal(row["MTotQty"]))),
						CellType = BizReportCellType.Price
					});
				}
				else
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(Convert.ToDecimal(row["MTotAmount"]) / ((Convert.ToDecimal(row["MTotQty"]) == decimal.Zero) ? decimal.One : Convert.ToDecimal(row["MTotQty"]))),
						CellType = BizReportCellType.Price
					});
				}
				if (!filter.ShowInCNY)
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = (Convert.ToString(row["MCyID"]).Equals(GetOrgDefaultCurrency(context)) ? string.Empty : Convert.ToString(row["MCyID"])),
						CellType = BizReportCellType.Text
					});
				}
				model.AddRow(bizReportRowModel);
			}
			sumQty = num;
			sumAmount = num2;
		}

		private static void SetDataSummary(BizReportModel model, RPTSalseByItemFilterModel filter, MContext context, decimal sumQty, decimal sumAmount)
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
				Value = Convert.ToString(sumQty)
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(sumAmount)
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = " ",
				CellType = BizReportCellType.Text
			});
			model.AddRow(bizReportRowModel);
			bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Summary", "Summary"),
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
			bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Item;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "SalesByItem", "Sales by item"),
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
				Value = Convert.ToString(sumAmount)
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = " ",
				CellType = BizReportCellType.Text
			});
			model.AddRow(bizReportRowModel);
			decimal othersalse = GetOthersalse(filter, context);
			bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Item;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "OtherSales", "Other Sales"),
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
				Value = Convert.ToString(othersalse)
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = " ",
				CellType = BizReportCellType.Text
			});
			model.AddRow(bizReportRowModel);
			bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Total;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "TotalSales", "Total Sales"),
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
				Value = Convert.ToString(sumAmount + othersalse)
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = " ",
				CellType = BizReportCellType.Text
			});
			model.AddRow(bizReportRowModel);
		}
	}
}
