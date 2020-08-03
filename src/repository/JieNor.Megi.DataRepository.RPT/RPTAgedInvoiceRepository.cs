using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.BD;
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
	public static class RPTAgedInvoiceRepository
	{
		public static BizReportModel InvoicesAgedList(MContext ctx, RPTAgeInvoiceFilterModel filter)
		{
			List<RPTAgeInvoiceModel> invoiceAgeReportData = GetInvoiceAgeReportData(ctx, filter);
			BizReportModel reportData = GetReportData(ctx, filter, invoiceAgeReportData);
			reportData.Type = Convert.ToInt32(BizReportType.Invoices);
			return reportData;
		}

		private static List<RPTAgeInvoiceModel> GetInvoiceAgeReportData(MContext ctx, RPTAgeInvoiceFilterModel filter)
		{
			DataTable finishVerifData = GetFinishVerifData(ctx, filter);
			List<RPTAgeInvoiceModel> list = ModelInfoManager.DataTableToList<RPTAgeInvoiceModel>(finishVerifData);
			foreach (RPTAgeInvoiceModel item in list)
			{
				item.MHaveVerificationAmt = Math.Round(item.MHaveVerificationAmtFor, 2);
				item.MNoVerificationAmt = Math.Round(item.MTaxTotalAmt, 2);
				item.MTaxTotalAmt = Math.Round(item.MTaxTotalAmt, 2);
				item.MReconciledAmt = Math.Round(item.MReconciledAmtFor, 2);
				item.MTotalAmt = Math.Round(item.MTaxTotalAmtFor, 2);
				RPTAgeInvoiceModel rPTAgeInvoiceModel = item;
				DateTime dateTime = item.MDueDate;
				DateTime date = dateTime.Date;
				dateTime = DateTime.Now;
				rPTAgeInvoiceModel.MDueAmt = Math.Round((date > dateTime.Date) ? decimal.Zero : item.MTaxTotalAmt, 2);
				RPTAgeInvoiceModel rPTAgeInvoiceModel2 = item;
				dateTime = item.MDueDate;
				DateTime date2 = dateTime.Date;
				dateTime = DateTime.Now;
				rPTAgeInvoiceModel2.MDueAmtFor = Math.Round((date2 > dateTime.Date) ? decimal.Zero : item.MTaxTotalAmtFor, 2);
				item.MPaidAmt = item.MHaveVerificationAmtFor;
				TimeSpan timeSpan;
				if (filter.AgedByField == AgedByField.InvoiceDate)
				{
					RPTAgeInvoiceModel rPTAgeInvoiceModel3 = item;
					DateTime mDueDate2 = item.MDueDate;
					object mDueDay;
					if (!(item.MDueDate == DateTime.MinValue))
					{
						DateTime mOldBizDate = item.MOldBizDate;
						dateTime = ctx.DateNow;
						timeSpan = mOldBizDate - dateTime.Date;
						mDueDay = Convert.ToString(Convert.ToInt32(timeSpan.TotalDays));
					}
					else
					{
						mDueDay = string.Empty;
					}
					rPTAgeInvoiceModel3.MDueDay = (string)mDueDay;
				}
				else
				{
					RPTAgeInvoiceModel rPTAgeInvoiceModel4 = item;
					DateTime mDueDate3 = item.MDueDate;
					object mDueDay2;
					if (!(item.MDueDate == DateTime.MinValue))
					{
						DateTime mDueDate = item.MDueDate;
						dateTime = ctx.DateNow;
						timeSpan = mDueDate - dateTime.Date;
						mDueDay2 = Convert.ToString(Convert.ToInt32(timeSpan.TotalDays));
					}
					else
					{
						mDueDay2 = string.Empty;
					}
					rPTAgeInvoiceModel4.MDueDay = (string)mDueDay2;
				}
			}
			GetTotalRowModel(list);
			return list;
		}

		private static void GetTotalRowModel(List<RPTAgeInvoiceModel> detailData)
		{
			RPTAgeInvoiceModel rPTAgeInvoiceModel = new RPTAgeInvoiceModel();
			rPTAgeInvoiceModel.MRowType = BizReportRowType.Total;
			rPTAgeInvoiceModel.MDueAmt = detailData.Sum((RPTAgeInvoiceModel f) => f.MDueAmt);
			rPTAgeInvoiceModel.MDueAmtFor = detailData.Sum((RPTAgeInvoiceModel f) => f.MDueAmtFor);
			rPTAgeInvoiceModel.MHaveVerificationAmt = detailData.Sum((RPTAgeInvoiceModel f) => f.MHaveVerificationAmt);
			rPTAgeInvoiceModel.MHaveVerificationAmtFor = detailData.Sum((RPTAgeInvoiceModel f) => f.MHaveVerificationAmtFor);
			rPTAgeInvoiceModel.MNoVerificationAmt = detailData.Sum((RPTAgeInvoiceModel f) => f.MNoVerificationAmt);
			rPTAgeInvoiceModel.MNoVerificationAmtFor = detailData.Sum((RPTAgeInvoiceModel f) => f.MNoVerificationAmtFor);
			rPTAgeInvoiceModel.MPaidAmt = detailData.Sum((RPTAgeInvoiceModel f) => f.MPaidAmt);
			rPTAgeInvoiceModel.MReconciledAmt = detailData.Sum((RPTAgeInvoiceModel f) => f.MReconciledAmt);
			rPTAgeInvoiceModel.MTaxTotalAmt = detailData.Sum((RPTAgeInvoiceModel f) => f.MTaxTotalAmt);
			rPTAgeInvoiceModel.MTaxTotalAmtFor = detailData.Sum((RPTAgeInvoiceModel f) => f.MTaxTotalAmtFor);
			rPTAgeInvoiceModel.MTotalAmt = detailData.Sum((RPTAgeInvoiceModel f) => f.MTotalAmt);
			detailData.Add(rPTAgeInvoiceModel);
		}

		private static BizReportModel GetReportData(MContext ctx, RPTAgeInvoiceFilterModel filter, List<RPTAgeInvoiceModel> rptData)
		{
			BizReportModel bizReportModel = new BizReportModel();
			GetRptTitle(ctx, filter, bizReportModel);
			GetHeadRow(ctx, bizReportModel, filter);
			bool needSum = true;
			GetItemRow(ctx, bizReportModel, rptData, out needSum);
			GetTotalRow(ctx, bizReportModel, rptData, needSum);
			return bizReportModel;
		}

		private static void GetTotalRow(MContext ctx, BizReportModel model, List<RPTAgeInvoiceModel> agedRptData, bool needSum)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Total", "Total");
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Total;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = text,
				CellType = BizReportCellType.Text
			});
			RPTAgeInvoiceModel rPTAgeInvoiceModel = agedRptData.FirstOrDefault((RPTAgeInvoiceModel f) => f.MRowType == BizReportRowType.Total);
			if (rPTAgeInvoiceModel != null)
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
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = " ",
					CellType = BizReportCellType.Text
				});
				BizReportRowModel bizReportRowModel2 = bizReportRowModel;
				BizReportCellModel bizReportCellModel = new BizReportCellModel();
				object value;
				decimal num;
				if (!needSum)
				{
					value = " ";
				}
				else
				{
					num = rPTAgeInvoiceModel.MTotalAmt;
					value = num.ToString();
				}
				bizReportCellModel.Value = (string)value;
				bizReportCellModel.CellType = BizReportCellType.Money;
				bizReportRowModel2.AddCell(bizReportCellModel);
				BizReportRowModel bizReportRowModel3 = bizReportRowModel;
				BizReportCellModel bizReportCellModel2 = new BizReportCellModel();
				object value2;
				if (!needSum)
				{
					value2 = " ";
				}
				else
				{
					num = rPTAgeInvoiceModel.MPaidAmt;
					value2 = num.ToString();
				}
				bizReportCellModel2.Value = (string)value2;
				bizReportCellModel2.CellType = BizReportCellType.Money;
				bizReportRowModel3.AddCell(bizReportCellModel2);
				BizReportRowModel bizReportRowModel4 = bizReportRowModel;
				BizReportCellModel bizReportCellModel3 = new BizReportCellModel();
				object value3;
				if (!needSum)
				{
					value3 = " ";
				}
				else
				{
					num = rPTAgeInvoiceModel.MDueAmtFor;
					value3 = num.ToString();
				}
				bizReportCellModel3.Value = (string)value3;
				bizReportCellModel3.CellType = BizReportCellType.Money;
				bizReportRowModel4.AddCell(bizReportCellModel3);
				BizReportRowModel bizReportRowModel5 = bizReportRowModel;
				BizReportCellModel bizReportCellModel4 = new BizReportCellModel();
				num = rPTAgeInvoiceModel.MDueAmt;
				bizReportCellModel4.Value = num.ToString();
				bizReportCellModel4.CellType = BizReportCellType.Money;
				bizReportRowModel5.AddCell(bizReportCellModel4);
			}
			model.AddRow(bizReportRowModel);
		}

		private static void GetItemRow(MContext ctx, BizReportModel model, List<RPTAgeInvoiceModel> rptData, out bool needSum)
		{
			needSum = true;
			if (rptData != null && rptData.Count != 0)
			{
				string currnecyID = GetCurrnecyID(ctx);
				foreach (RPTAgeInvoiceModel rptDatum in rptData)
				{
					if (rptDatum.MRowType != BizReportRowType.Total)
					{
						bool flag = !rptDatum.MCyID.Equals(currnecyID);
						if (needSum & flag)
						{
							needSum = false;
						}
						string arg = flag ? rptDatum.MCyID : string.Empty;
						BizReportRowModel bizReportRowModel = new BizReportRowModel();
						bizReportRowModel.RowType = BizReportRowType.Item;
						BizReportRowModel bizReportRowModel2 = bizReportRowModel;
						string mInvoiceID = rptDatum.MInvoiceID;
						DateTime dateTime = rptDatum.MOldBizDate;
						bizReportRowModel2.UniqueValue = $"{mInvoiceID}{dateTime.ToShortDateString()}";
						BizReportCellModel bizReportCellModel = new BizReportCellModel();
						dateTime = rptDatum.MOldBizDate;
						bizReportCellModel.Value = dateTime.ToLongDateString();
						bizReportCellModel.CellType = BizReportCellType.Date;
						bizReportCellModel.CellLink = GetCellLink(ctx, rptDatum);
						BizReportCellModel bizReportCellModel2 = bizReportCellModel;
						bizReportCellModel2.AddBillID(rptDatum.MInvoiceID);
						bizReportRowModel.AddCell(bizReportCellModel2);
						BizReportCellModel bizReportCellModel3 = new BizReportCellModel
						{
							Value = ((rptDatum.MType == "Invoice_Sale" || rptDatum.MType == "Invoice_Sale_Red") ? rptDatum.MInvoiceNo : rptDatum.MReference),
							CellType = BizReportCellType.Text,
							CellLink = GetCellLink(ctx, rptDatum)
						};
						bizReportCellModel3.AddBillID(rptDatum.MInvoiceID);
						bizReportRowModel.AddCell(bizReportCellModel3);
						BizReportCellModel bizReportCellModel4 = new BizReportCellModel();
						dateTime = rptDatum.MDueDate;
						bizReportCellModel4.Value = dateTime.ToShortDateString();
						bizReportCellModel4.CellType = BizReportCellType.Date;
						BizReportCellModel bizReportCellModel5 = bizReportCellModel4;
						DateTime mDueDate = rptDatum.MDueDate;
						if (rptDatum.MDueDate != DateTime.MinValue)
						{
							bizReportCellModel5.AddBillID(rptDatum.MInvoiceID);
							bizReportCellModel5.CellLink = GetCellLink(ctx, rptDatum);
						}
						bizReportRowModel.AddCell(bizReportCellModel5);
						BizReportCellModel bizReportCellModel6 = new BizReportCellModel
						{
							Value = rptDatum.MDueDay,
							CellType = BizReportCellType.Text
						};
						if (!string.IsNullOrWhiteSpace(rptDatum.MDueDay))
						{
							bizReportCellModel6.AddBillID(rptDatum.MInvoiceID);
							bizReportCellModel6.CellLink = GetCellLink(ctx, rptDatum);
						}
						bizReportRowModel.AddCell(bizReportCellModel6);
						BizReportCellModel bizReportCellModel7 = new BizReportCellModel
						{
							Value = $"{arg} {rptDatum.MTotalAmt.To2Decimal()}",
							CellType = BizReportCellType.Money,
							CellLink = GetCellLink(ctx, rptDatum)
						};
						bizReportCellModel7.AddBillID(rptDatum.MInvoiceID);
						bizReportRowModel.AddCell(bizReportCellModel7);
						BizReportCellModel bizReportCellModel8 = new BizReportCellModel
						{
							Value = $"{arg} {rptDatum.MPaidAmt.To2Decimal()}",
							CellType = BizReportCellType.Money,
							CellLink = GetCellLink(ctx, rptDatum)
						};
						bizReportCellModel8.AddBillID(rptDatum.MInvoiceID);
						bizReportRowModel.AddCell(bizReportCellModel8);
						BizReportCellModel bizReportCellModel9 = new BizReportCellModel
						{
							Value = $"{arg} {rptDatum.MDueAmtFor.To2Decimal()}",
							CellType = BizReportCellType.Money,
							CellLink = GetCellLink(ctx, rptDatum)
						};
						bizReportCellModel9.AddBillID(rptDatum.MInvoiceID);
						bizReportRowModel.AddCell(bizReportCellModel9);
						BizReportCellModel bizReportCellModel10 = new BizReportCellModel
						{
							Value = rptDatum.MDueAmt.To2Decimal(),
							CellType = BizReportCellType.Money,
							CellLink = GetCellLink(ctx, rptDatum)
						};
						bizReportCellModel10.AddBillID(rptDatum.MInvoiceID);
						bizReportRowModel.AddCell(bizReportCellModel10);
						model.AddRow(bizReportRowModel);
					}
				}
			}
		}

		private static void GetHeadRow(MContext ctx, BizReportModel model, RPTAgeInvoiceFilterModel filter)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			if (filter.AgedType == RPTAgedRptFilterEnum.Payables)
			{
				empty = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "PurchaseInvoiceDate", "Purchase Invoice Date");
				empty2 = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Reference);
				empty3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Paid", "Paid");
			}
			else
			{
				empty = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "InvoiceDate", "Invoice Date");
				empty2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "InvoiceNo", "Invoice No");
				empty3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Received", "Received");
			}
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "DueDate", "Due Date");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "DueDays", "Due Days");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Total", "Total");
			string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Due", "Due");
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = empty,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = empty2,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = text,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = text2,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = text3,
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = empty3,
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "DueAmountForeignCurrency", "到期金额(原币)"),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "DueAmountBaseCurrency", "到期金额(本位币)"),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private static void GetRptTitle(MContext ctx, RPTAgeInvoiceFilterModel filter, BizReportModel model)
		{
			model.HeaderTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Invoices", "Invoices");
			if (filter.AgedType == RPTAgedRptFilterEnum.Payables)
			{
				model.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "PurchaseInvoice", "Purchase Invoice");
			}
			else
			{
				model.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Invoices", "Invoices");
			}
			BDContactsRepository bDContactsRepository = new BDContactsRepository();
			BDContactsInfoModel statementContData = bDContactsRepository.GetStatementContData(ctx, filter.MContactID);
			string obj = (statementContData == null || string.IsNullOrEmpty(statementContData.MName)) ? filter.MContactName : statementContData.MName;
			string text2 = model.Title2 = obj;
			DateTime dateTime = filter.DateFrom;
			string str = dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(ctx)));
			dateTime = filter.DateTo;
			model.Title3 = str + "--" + dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(ctx)));
		}

		private static void SetRate(MContext ctx, RPTAgeInvoiceFilterModel filter, List<RPTAgeInvoiceModel> detailData)
		{
			string currnecyID = GetCurrnecyID(ctx);
			Dictionary<string, decimal> currencyRate = GetCurrencyRate(ctx, filter, currnecyID);
			foreach (RPTAgeInvoiceModel detailDatum in detailData)
			{
				if (detailDatum.MCyID == currnecyID)
				{
					detailDatum.MCyRate = decimal.One;
					detailDatum.MHaveVerificationAmt = Math.Round(detailDatum.MHaveVerificationAmtFor, 2);
					detailDatum.MNoVerificationAmt = Math.Round(detailDatum.MNoVerificationAmtFor, 2);
					detailDatum.MTaxTotalAmt = Math.Round(detailDatum.MTaxTotalAmtFor, 2);
					detailDatum.MReconciledAmt = Math.Round(detailDatum.MReconciledAmtFor, 2);
				}
				else if (currencyRate.ContainsKey(detailDatum.MCyID))
				{
					detailDatum.MCyRate = currencyRate[detailDatum.MCyID];
					detailDatum.MHaveVerificationAmt = Math.Round(detailDatum.MHaveVerificationAmtFor / currencyRate[detailDatum.MCyID], 2);
					detailDatum.MNoVerificationAmt = Math.Round(detailDatum.MNoVerificationAmtFor / currencyRate[detailDatum.MCyID], 2);
					detailDatum.MTaxTotalAmt = Math.Round(detailDatum.MTaxTotalAmtFor / currencyRate[detailDatum.MCyID], 2);
					detailDatum.MReconciledAmt = Math.Round(detailDatum.MReconciledAmtFor / currencyRate[detailDatum.MCyID], 2);
				}
			}
		}

		private static Dictionary<string, decimal> GetCurrencyRate(MContext ctx, RPTAgeInvoiceFilterModel filter, string sourceCyID)
		{
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT distinct a.MItemId,a.msourceCurrencyID,a.mtargetcurrencyid,a.mratedate,ifnull(a.MRate,0) MRate,ifnull(a.MUserRate,0) MUserRate ");
			stringBuilder.AppendLine("FROM T_BD_ExchangeRate a");
			stringBuilder.AppendLine("Inner join (");
			stringBuilder.AppendLine("        SELECT MItemId,MSourceCurrencyID,mtargetcurrencyid,max(MRatedate) as MRatedate ");
			stringBuilder.AppendLine("        FROM T_BD_ExchangeRate ");
			stringBuilder.AppendLine("        where morgid=@MOrgID AND MIsDelete=0 and msourceCurrencyID=@msourceCurrencyID and MRateDate <=@MRateDate");
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
			array[2].Value = filter.AsAt;
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
			string sql = "select MCurrencyID from T_REG_Financial Where MOrgID=@MOrgID AND MIsDelete=0  ";
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

		private static string GetCurrnecyName(MContext ctx, string cyID)
		{
			string sql = "select MName from T_Bas_Currency_L Where MParentID=@MParentID AND MIsDelete=0  ";
			MySqlParameter mySqlParameter = new MySqlParameter("@MParentID", MySqlDbType.VarChar, 36);
			mySqlParameter.Value = cyID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, mySqlParameter);
			if (single != null)
			{
				return single.ToString();
			}
			return "人民币";
		}

		private static DataTable GetFinishVerifData(MContext ctx, RPTAgeInvoiceFilterModel filter)
		{
			string text = "";
			text = ((filter.AgedType != RPTAgedRptFilterEnum.Payables) ? string.Format("  (MType='{0}' Or MType='{1}') ", "Invoice_Sale", "Invoice_Sale_Red") : string.Format("  (MType='{0}' Or MType='{1}') ", "Invoice_Purchase", "Invoice_Purchase_Red"));
			string text2 = "";
			text2 = ((filter.AgedByField != AgedByField.InvoiceDate) ? "MDueDate" : "MBizDate");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select a.MContactID,convert(AES_DECRYPT(c.MName,'{0}') using utf8) as MContactName,a.MCyID as MCyID,d.MName as MCyName,a.MType,", "JieNor-001");
			stringBuilder.AppendLine(" a.MID as MInvoiceID,a.MNumber as MInvoiceNo,a.MReference,");
			stringBuilder.AppendLine(" a.MBizDate as MOldBizDate,");
			stringBuilder.AppendLine(" a.MExchangeRate as MCyRate,");
			stringBuilder.AppendLine(" (CASE WHEN a.MType='Invoice_Sale' OR a.MType='Invoice_Purchase' THEN MDueDate ELSE MBizDate END) as MDueDate, ");
			stringBuilder.AppendLine(" (CASE WHEN a.MType='Invoice_Sale' OR a.MType='Invoice_Purchase' THEN DATE_FORMAT(a." + text2 + ",'%Y-%m-01') ELSE DATE_FORMAT(a.MBizDate,'%Y-%m-01') END) as MBizDate, ");
			stringBuilder.AppendLine(" IFNULL(a.MTaxTotalAmtFor,0) as MTaxTotalAmtFor, ");
			stringBuilder.AppendLine(" IFNULL(a.MTaxTotalAmt,0) as MTaxTotalAmt, ");
			stringBuilder.AppendLine(" IFNULL(a.MTotalAmt,0) as MTotalAmt, ");
			stringBuilder.AppendLine(" IFNULL(a.MVerifyAmtFor,0) as MHaveVerificationAmtFor, ");
			stringBuilder.AppendLine(" IFNULL(a.MTaxTotalAmtFor,0)-IFNULL(a.MVerifyAmtFor,0) as MNoVerificationAmtFor ");
			stringBuilder.AppendLine(" from T_IV_Invoice a ");
			stringBuilder.AppendLine(" left join T_BD_Contacts_l c on a.MContactID=c.MParentID and c.MLocaleID=@MLocaleID  AND c.MIsDelete=0 ");
			stringBuilder.AppendLine(" left join T_Bas_Currency_L d on a.MCyID=d.MParentID and d.MLocaleID=@MLocaleID AND d.MIsDelete=0 ");
			stringBuilder.AppendLine(" Where (CASE WHEN MType='Invoice_Sale' OR MType='Invoice_Purchase' ");
			stringBuilder.AppendLine("             THEN " + text2 + " >= @DateFrom And " + text2 + " <= @DateTo ");
			stringBuilder.AppendLine("             ELSE MBizDate>=@DateFrom AND MBizDate<=@DateTo ");
			stringBuilder.AppendLine("             END) ");
			stringBuilder.AppendLine("        And " + text);
			stringBuilder.AppendLine("        And a.MOrgID=@MOrgID ");
			stringBuilder.AppendLine("    and IFNULL(a.MTaxTotalAmtFor,0)-IFNULL(a.MVerifyAmtFor,0)<>0 ");
			stringBuilder.AppendLine("        And (a.MIsDelete=0 AND (a.MStatus=3 or a.MStatus=4) )");
			stringBuilder.AppendLine("        And a.MContactID = @MContactID ");
			stringBuilder.AppendLine(" order by c.MName ");
			MySqlParameter[] array = new MySqlParameter[6]
			{
				new MySqlParameter("@AsAt", MySqlDbType.DateTime),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@DateTo", MySqlDbType.DateTime),
				new MySqlParameter("@DateFrom", MySqlDbType.DateTime)
			};
			array[0].Value = filter.AsAt;
			array[1].Value = ctx.MOrgID;
			array[2].Value = filter.MContactID;
			array[3].Value = ctx.MLCID;
			array[4].Value = filter.DateTo;
			array[5].Value = filter.DateFrom;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			return dataSet.Tables[0];
		}

		private static BizReportCellLinkModel GetCellLink(MContext ctx, RPTAgeInvoiceModel model)
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
