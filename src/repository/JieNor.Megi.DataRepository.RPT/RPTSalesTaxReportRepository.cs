using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.REG;
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
	public class RPTSalesTaxReportRepository
	{
		public static List<RPTSalesTaxReportDetailsModel> GetSalesTaxReportDetails(RPTSalesTaxBaseFilterModel model, MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select MID,MBillType,MType,MReference,MDesc,MBizDate,MNumber,MTaxID,MRTTaxName,MRTTaxRate,sum(MRTGross) as MRTGross,sum(MRTTax) as MRTTax,sum(MRTNet) as MRTNet");
			stringBuilder.AppendLine(" from(");
			stringBuilder.Append(GetSalesTaxReportDetailsSql(ctx));
			stringBuilder.AppendLine(" ) t");
			stringBuilder.AppendLine(" group by MID,MBillType,MReference,MDesc,MBizDate,MNumber,MTaxID,MRTTaxName,MRTTaxRate");
			stringBuilder.AppendLine(" order by MBizDate,MReference");
			return GetSalesTaxReport(model, ctx, stringBuilder);
		}

		private static List<RPTSalesTaxReportDetailsModel> GetSalesTaxReportTotal(RPTSalesTaxBaseFilterModel model, MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select MTaxID,MRTTaxName,MRTTaxRate,MRTCompName,MRTCompTaxRate,sum(MRTGross) as MRTGross,sum(MRTTax) as MRTTax,sum(MRTNet) as MRTNet");
			stringBuilder.AppendLine(" from(");
			stringBuilder.Append(GetSalesTaxReportDetailsSql(ctx));
			stringBuilder.AppendLine(" ) t");
			stringBuilder.AppendLine(" group by MTaxID,MRTTaxName,MRTTaxRate,MRTCompName,MRTCompTaxRate");
			stringBuilder.AppendLine(" order by MRTTaxName,MRTCompName");
			return GetSalesTaxReport(model, ctx, stringBuilder);
		}

		private static string GetSalesTaxReportDetailsSql(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select a.MID,a.MBillType,a.MType,a.MReference,a.MDesc,a.MBizDate,a.MNumber,b.MTaxID,b.MRTTaxName,b.MRTTaxRate,b.MRTCompName,b.MRTCompTaxRate, ");
			stringBuilder.AppendLine(" a.MRTGross*b.MRTRate as MRTGross,a.MRTTax*b.MRTRate as MRTTax,a.MRTNet*b.MRTRate as MRTNet");
			stringBuilder.AppendLine(" from");
			stringBuilder.AppendLine(" (");
			stringBuilder.AppendLine(" select t1.MID,'Receive' as MBillType,t1.MType,t1.MReference,t1.MDesc,t1.MBizDate,t1.MNumber,t2.MTaxID,(abs(t2.MTaxAmount)-abs(t2.MTaxAmt)) as MRTGross,abs(t2.MTaxAmt) as MRTTax,abs(t2.MTaxAmount) as MRTNet");
			stringBuilder.AppendLine(" from T_IV_Invoice t1");
			stringBuilder.AppendLine(" join T_IV_InvoiceEntry t2 on t1.MID=t2.MID AND t2.MIsDelete=0 and t1.MOrgID = t2.MOrgID ");
			stringBuilder.AppendLine(" where t1.MIsDelete=0  and t1.MStatus>=3 and t1.MOrgID=@MOrgID  and (t1.MType='Invoice_Sale' or t1.MType='Invoice_Purchase_Red') ");
			if (!IsSmallTaxPlayer(ctx))
			{
				stringBuilder.AppendLine(" union all");
				stringBuilder.AppendLine(" select t1.MID,'Payment' as MBillType,t1.MType,t1.MReference,t1.MDesc,t1.MBizDate,t1.MNumber,t2.MTaxID,-(abs(t2.MTaxAmount)-abs(t2.MTaxAmt)) as MRTGross,-abs(t2.MTaxAmt) as MRTTax,-abs(t2.MTaxAmount) as MRTNet");
				stringBuilder.AppendLine(" from T_IV_Invoice t1");
				stringBuilder.AppendLine(" join T_IV_InvoiceEntry t2 on t1.MID=t2.MID AND t2.MIsDelete=0 and t1.MOrgID = t2.MOrgID ");
				stringBuilder.AppendLine(" where t1.MIsDelete=0 and t1.MStatus>=3 and t1.MOrgID=@MOrgID and (t1.MType='Invoice_Purchase' or t1.MType='Invoice_Sale_Red')");
			}
			stringBuilder.AppendLine(" ) a");
			stringBuilder.AppendLine(" join ");
			stringBuilder.AppendLine(" (");
			stringBuilder.AppendLine(" select t1.MItemID as MTaxID,concat(t2.MName,'(',convert(t1.MTaxRate,decimal(23,2)),'%)') as MRTTaxName,t1.MTaxRate as MRTTaxRate, IFNULL(t2.MName,t3.MName) as MRTCompName,");
			stringBuilder.AppendLine(" concat(convert(IFNULL(t3.MTaxRate,t1.MTaxRate),decimal(23,2)),'%') as MRTCompTaxRate,case when t1.MTaxRate=0 then 0 else IFNULL(t3.MTaxRate/t1.MTaxRate,t1.MTaxRate) end as MRTRate");
			stringBuilder.AppendLine(" from T_REG_TaxRate t1");
			stringBuilder.AppendLine(" join T_REG_TaxRate_L t2 on t1.MItemID=t2.MParentID and t2.MLocaleID=@MLCID AND t2.MIsDelete=0 ");
			stringBuilder.AppendLine(" LEFT join T_REG_TaxRateEntry t3 on t1.MItemID=t3.MItemID AND t3.MIsDelete=0  ");
			stringBuilder.AppendLine(" where t1.MIsDelete=0 and t1.MOrgID=@MOrgID");
			stringBuilder.AppendLine(" ) b on a.MTaxID=b.MTaxID");
			stringBuilder.AppendLine(" where a.MBizDate>=@MFromDate and a.MBizDate<=@MToDate");
			return stringBuilder.ToString();
		}

		private static List<RPTSalesTaxReportDetailsModel> GetSalesTaxReport(RPTSalesTaxBaseFilterModel model, MContext ctx, StringBuilder strSql)
		{
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MFromDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MToDate", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			DateTime dateTime = DateTime.Now;
			dateTime = dateTime.Date;
			DateTime dateTime2 = DateTime.Now;
			dateTime2 = dateTime2.Date;
			DateTime dateTime3 = dateTime.AddDays((double)(1 - dateTime2.Day));
			MySqlParameter obj = array[2];
			DateTime mFromDate = model.MFromDate;
			dateTime = Convert.ToDateTime(model.MFromDate);
			obj.Value = dateTime.ToString("yyyy-MM-dd");
			MySqlParameter obj2 = array[3];
			DateTime mToDate = model.MToDate;
			dateTime = Convert.ToDateTime(model.MToDate);
			obj2.Value = dateTime.ToString("yyyy-MM-dd");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<RPTSalesTaxReportDetailsModel>(dynamicDbHelperMySQL.Query(strSql.ToString(), array));
		}

		private static DataSet GetTaxBaseInfo(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select t1.MTaxNo from T_REG_Financial t1");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID and MIsDelete = 0 ");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
		}

		private static string GetFormatString(MContext ctx, DataSet ds)
		{
			if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(Convert.ToString(ds.Tables[0].Rows[0]["MTaxNo"])))
			{
				return Convert.ToString(ds.Tables[0].Rows[0]["MTaxNo"]);
			}
			return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "None", "None");
		}

		public static BizReportModel GetSalesTax(RPTSalesTaxReportFilterModel filter, MContext context)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.SalesTaxReport);
			SetTitle(bizReportModel, filter, context);
			SetTaxBaseInfo(bizReportModel, filter, context);
			SetRowHead(bizReportModel, filter, context);
			SetRowData(bizReportModel, filter, context);
			return bizReportModel;
		}

		private static void SetTitle(BizReportModel model, RPTSalesTaxBaseFilterModel filter, MContext context)
		{
			model.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "TaxSummary", "Tax Summary");
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

		private static void SetTaxBaseInfo(BizReportModel model, RPTSalesTaxBaseFilterModel filter, MContext context)
		{
			DataSet taxBaseInfo = GetTaxBaseInfo(context);
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Item;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "TaxIDNumber", "Tax ID Number"),
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
			if (filter.ShowByTaxRate)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = GetFormatString(context, taxBaseInfo),
				CellType = BizReportCellType.TextRight
			});
			model.AddRow(bizReportRowModel);
		}

		private static void SetRowHead(BizReportModel model, RPTSalesTaxBaseFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Tax", "Tax"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Rate", "Rate"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Net", "Net"),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Tax", "Tax"),
				CellType = BizReportCellType.Money
			});
			if (filter.ShowByTaxRate)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Gross", "Gross"),
					CellType = BizReportCellType.Money
				});
			}
			model.AddRow(bizReportRowModel);
		}

		private static void SetRowData(BizReportModel model, RPTSalesTaxBaseFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			List<RPTSalesTaxReportDetailsModel> salesTaxReportTotal = GetSalesTaxReportTotal(filter, context);
			if (filter.ShowByTaxRate)
			{
				List<string> list = (from s in salesTaxReportTotal
				select s.MTaxID).Distinct().ToList();
				foreach (string item in list)
				{
					if (list.IndexOf(item) == 0)
					{
						bizReportRowModel = new BizReportRowModel();
						bizReportRowModel.RowType = BizReportRowType.Group;
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "TaxesByTaxRate", "Taxes by Tax Rate"),
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
					}
					List<RPTSalesTaxReportDetailsModel> list2 = (from w in salesTaxReportTotal
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
					model.AddRow(bizReportRowModel);
					foreach (RPTSalesTaxReportDetailsModel item2 in list2)
					{
						bizReportRowModel = new BizReportRowModel();
						bizReportRowModel.RowType = BizReportRowType.Item;
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = item2.MRTCompName,
							CellType = BizReportCellType.Text
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = item2.MRTCompTaxRate,
							CellType = BizReportCellType.Text
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = " ",
							CellType = BizReportCellType.Text
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = Convert.ToString(item2.MRTTax),
							CellType = BizReportCellType.Money
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = " ",
							CellType = BizReportCellType.Text
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
						Value = Convert.ToString(list2.Sum((RPTSalesTaxReportDetailsModel s) => s.MRTNet)),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(list2.Sum((RPTSalesTaxReportDetailsModel s) => s.MRTTax)),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(list2.Sum((RPTSalesTaxReportDetailsModel s) => s.MRTGross)),
						CellType = BizReportCellType.Money
					});
					model.AddRow(bizReportRowModel);
				}
				bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.RowType = BizReportRowType.Total;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "TotalForTaxCodes", "Total for Tax Codes"),
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(salesTaxReportTotal.Sum((RPTSalesTaxReportDetailsModel s) => s.MRTNet)),
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(salesTaxReportTotal.Sum((RPTSalesTaxReportDetailsModel s) => s.MRTTax)),
					CellType = BizReportCellType.Money
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(salesTaxReportTotal.Sum((RPTSalesTaxReportDetailsModel s) => s.MRTGross)),
					CellType = BizReportCellType.Money
				});
				model.AddRow(bizReportRowModel);
			}
			if (filter.ShowByTaxComponent)
			{
				List<string> list3 = (from s in salesTaxReportTotal
				select s.MRTCompName).Distinct().ToList();
				foreach (string item3 in list3)
				{
					if (list3.IndexOf(item3) == 0)
					{
						bizReportRowModel = new BizReportRowModel();
						bizReportRowModel.RowType = BizReportRowType.Group;
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "TaxesByTaxComponent", "Taxes by Tax Component"),
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
						if (filter.ShowByTaxRate)
						{
							bizReportRowModel.AddCell(new BizReportCellModel
							{
								Value = " ",
								CellType = BizReportCellType.Text
							});
						}
						model.AddRow(bizReportRowModel);
					}
					List<RPTSalesTaxReportDetailsModel> list4 = (from w in salesTaxReportTotal
					where w.MRTCompName.Equals(item3)
					select w).ToList();
					bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.SubTotal;
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = (from s in list4
						select s.MRTCompName).FirstOrDefault(),
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
					if (filter.ShowByTaxRate)
					{
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = " ",
							CellType = BizReportCellType.Text
						});
					}
					model.AddRow(bizReportRowModel);
					foreach (RPTSalesTaxReportDetailsModel item4 in list4)
					{
						bizReportRowModel = new BizReportRowModel();
						bizReportRowModel.RowType = BizReportRowType.Item;
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = item4.MRTTaxName,
							CellType = BizReportCellType.Text
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = item4.MRTCompTaxRate,
							CellType = BizReportCellType.Text
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = Convert.ToString(item4.MRTNet),
							CellType = BizReportCellType.Money
						});
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = Convert.ToString(item4.MRTTax),
							CellType = BizReportCellType.Money
						});
						if (filter.ShowByTaxRate)
						{
							bizReportRowModel.AddCell(new BizReportCellModel
							{
								Value = " ",
								CellType = BizReportCellType.Text
							});
						}
						model.AddRow(bizReportRowModel);
					}
					bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.SubTotal;
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total") + " " + (from s in list4
						select s.MRTCompName).FirstOrDefault(),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = " ",
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(list4.Sum((RPTSalesTaxReportDetailsModel s) => s.MRTNet)),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(list4.Sum((RPTSalesTaxReportDetailsModel s) => s.MRTTax)),
						CellType = BizReportCellType.Money
					});
					if (filter.ShowByTaxRate)
					{
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = " ",
							CellType = BizReportCellType.Text
						});
					}
					model.AddRow(bizReportRowModel);
				}
			}
		}

		private static bool IsSmallTaxPlayer(MContext ctx)
		{
			REGFinancialRepository rEGFinancialRepository = new REGFinancialRepository();
			REGFinancialModel rEGFinancialModel = new REGFinancialModel();
			rEGFinancialModel.MAppID = ctx.MAppID;
			rEGFinancialModel.MOrgID = ctx.MOrgID;
			rEGFinancialModel = rEGFinancialRepository.GetByOrgID(ctx, rEGFinancialModel);
			return rEGFinancialModel.MTaxPayer == "2";
		}
	}
}
