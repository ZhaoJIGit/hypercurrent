using JieNor.Megi.Common.Converter;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.Mail;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Invoice;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.Log;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVInvoiceRepository : IVBaseRepository<IVInvoiceModel>
	{
		private string multLangFieldSql = "\r\n            ,t4.MName{0} as MContactInfo_MName{0}\r\n            ,t6.MName{0} AS InvoiceEntry_MTaxRate_MName{0} ";

		public string CommonSelect = string.Format("SELECT \n                                t1.MID,\n                                t1.MID as MInvoiceID,\n                                t1.MID as MCreditNoteID,\n                                t1.MReference,\n                                (case when t1.MType='Invoice_Sale' then t1.MNumber when t1.MType='Invoice_Sale_Red' then t1.MNumber else '' end) AS MInvoiceNumber,\n                                (case when t1.MType='Invoice_Sale_Red' then t1.MNumber when t1.MType='Invoice_Sale' then t1.MNumber else '' end) AS MCreditNoteNumber,\n                                t1.MDueDate,\n                                t1.MExpectedDate,\n                                t1.MStatus,\n                                t1.MType,\n                                t1.MBizDate,\n                                t1.MUrl,\n                                t1.MCyID,\n                                t1.MIsSent,\n                                t1.MContactID,\n                                ifnull(t8.MAttachCount,0) as MAttachCount,\n                                ABS(t1.MTotalAmtFor) AS MTotalAmtFor,\n                                ABS(t1.MTotalAmt) AS MTotalAmt,\n                                ABS(t1.MTaxTotalAmtFor) AS MTaxTotalAmtFor,\n                                ABS(t1.MTaxTotalAmt) AS MTaxTotalAmt,\n                                ABS(t1.MTaxAmtFor) AS MTaxAmtFor,\n                                ABS(t1.MTaxAmt) AS MTaxAmt,\n                                ABS(t1.MVerifyAmt) AS MVerifyAmt,\n                                ABS(t1.MVerifyAmtFor) AS MVerifyAmtFor,\n                                ABS(t1.MTotalDiscountAmtFor) AS MTotalDiscountAmtFor,\n                                t1.MTaxID,\n                                t1.MExchangeRate AS MCurrencyRate,\n                                t1.MDesc,\n                                t1.MModifyDate,\n                                t1.MCreateDate,\n                                t1.MCreateBy,\n                                t7.MItemID AS InvoiceEntry_MItemID,\n                                t7.MNumber AS InvoiceEntry_MItemCode,\n                                t2.MID as InvoiceEntry_MID,\n                                t2.MEntryID as InvoiceEntry_MLineItemID,\n                                t2.MDesc as InvoiceEntry_MDesc,\n                                ABS(t2.MQty) as InvoiceEntry_MQty,\n                                ABS(t2.MPrice) as InvoiceEntry_MPrice,\n                                t2.MDiscount as InvoiceEntry_MDiscount,\n                                ABS(t2.MAmountFor) as InvoiceEntry_MAmountFor,\n                                ABS(t2.MAmount) as InvoiceEntry_MAmount,\n                                ABS(t2.MTaxAmountFor) as InvoiceEntry_MTaxAmountFor,\n                                ABS(t2.MTaxAmount) as InvoiceEntry_MTaxAmount,\n                                ABS(t2.MTaxAmtFor) as InvoiceEntry_MTaxAmtFor,\n                                ABS(t2.MTaxAmt) as InvoiceEntry_MTaxAmt,\n                                t2.MTaxID as InvoiceEntry_MTaxID,\n                                t2.MSeq AS InvoiceEntry_MSeq,\n                                t2.MTrackItem1 as InvoiceEntry_MTrackItem1,\n                                t2.MTrackItem2 as InvoiceEntry_MTrackItem2,\n                                t2.MTrackItem3 as InvoiceEntry_MTrackItem3,\n                                t2.MTrackItem4 as InvoiceEntry_MTrackItem4,\n                                t2.MTrackItem5 as InvoiceEntry_MTrackItem5,\n                                t2.MCreateDate as InvoiceEntry_MCreateDate,\n                                t2.MModifyDate as InvoiceEntry_MModifyDate,\n                                t3.MItemID as MContactInfo_MContactID,\n                                t3.MIsCustomer as MContactInfo_MIsCustomer,\n                                t3.MIsSupplier as MContactInfo_MIsSupplier,\n                                t3.MIsOther as MContactInfo_MIsOther,\n                                t4.MName as MContactInfo_MName,\n                                t3.MIsActive AS MContactInfo_MIsActive,\n                                t5.MItemID AS InvoiceEntry_MTaxRate_MTaxRateID,\n                                t6.MName AS InvoiceEntry_MTaxRate_MName,\n                                t5.MEffectiveTaxRate AS InvoiceEntry_MTaxRate_MEffectiveTaxRate\n                                #_#lang_field0#_#  \n                            FROM\n                                t_iv_invoice t1\n                            INNER JOIN t_iv_invoiceentry t2 ON t1.MID = t2.MID\n                                AND t1.MOrgID = t2.MOrgID\n                                AND t1.MIsDelete = t2.MIsDelete\n                            LEFT JOIN t_bd_contacts t3 ON t1.MContactID = t3.MItemID \n                                AND t3.MOrgID = t1.MOrgID\n                                AND t3.MIsDelete = t1.MIsDelete\n                            LEFT JOIN @_@t_bd_contacts_l@_@ t4  ON t4.MParentID = t3.MItemID\n                                AND t4.MOrgID = t1.MOrgID\n                                AND t4.MIsDelete = t1.MIsDelete \n                            LEFT JOIN t_reg_taxrate t5 ON t2.MTaxID=t5.MItemID\n                                AND t5.MOrgID = t2.MOrgID\n                                AND t5.MIsDelete = t2.MIsDelete \n                            LEFT JOIN @_@t_reg_taxrate_l@_@ t6 ON t6.MParentID = t5.MItemID\n                                AND t6.MOrgID = t5.MOrgID\n                                AND t6.MIsDelete = t5.MIsDelete \n                            LEFT JOIN t_bd_Item t7 ON t2.MItemID = t7.MItemID \n                                AND t2.MOrgID = t7.MOrgID\n                                AND t2.MIsDelete = t7.MIsDelete\n                            LEFT JOIN (SELECT \n                                            MOrgID, MParentID, COUNT(*) AS MAttachCount\n                                        FROM\n                                            T_IV_InvoiceAttachment\n                                        WHERE\n                                            MOrgID = @MOrgID AND MIsDelete = 0\n                                        GROUP BY MOrgID , MParentID) t8 ON t8.MOrgID = t1.MOrgID and t8.MParentID = t1.MID                        \n                            WHERE\n                                t1.MOrgID = @MOrgID\n                                    AND t1.MIsDelete = 0 ", "JieNor-001");

		public DataGridJson<IVAPIInvoiceModel> GetInvoice(MContext ctx, GetParam param)
		{
			string commonSelect = CommonSelect;
			commonSelect = $"{commonSelect} AND (t1.MType='Invoice_Sale' OR t1.MType='Invoice_Purchase')";
			return new APIDataRepository().Get<IVAPIInvoiceModel>(ctx, param, commonSelect, multLangFieldSql, false, true, null);
		}

		public DataGridJson<IVAPICreditNoteModel> GetCreditNote(MContext ctx, GetParam param)
		{
			string commonSelect = CommonSelect;
			commonSelect = $"{commonSelect} AND (t1.MType='Invoice_Sale_Red' OR t1.MType='Invoice_Purchase_Red')";
			return new APIDataRepository().Get<IVAPICreditNoteModel>(ctx, param, commonSelect, multLangFieldSql, false, true, null);
		}

		public static OperationResult AddInvoiceNoteLog(MContext ctx, IVInvoiceModel model)
		{
			IVInvoiceLogRepository.AddInvoiceNoteLog(ctx, model);
			return new OperationResult
			{
				Success = true
			};
		}

		public static IVContactInvoiceSummaryModel GetInvoiceSummaryModelByContact(MContext ctx, string contactId)
		{
			string sql = string.Format("SELECT (SELECT SUM(MTaxTotalAmt) FROM T_IV_Invoice \r\n                        WHERE MIsDelete = 0 AND MOrgID=@MOrgID AND MStatus>0 AND (MType='Invoice_Sale' OR MType='Invoice_Sale_Red') AND MContactID=@MContactID {0}) as SaleAmount,\r\n                        (SELECT SUM(MTaxTotalAmt) FROM T_IV_Invoice \r\n                        WHERE MIsDelete = 0 AND MOrgID=@MOrgID AND MStatus>0 AND (MType='Invoice_Purchase' OR MType='Invoice_Purchase_Red') AND MContactID=@MContactID {0}) as BillAmount", ctx.IsSelfData ? " AND MCreatorID=@MUserID " : "");
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MContactID", contactId),
				new MySqlParameter("@MUserID", ctx.MUserID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dataTable = dynamicDbHelperMySQL.Query(sql, cmdParms).Tables[0];
			IVContactInvoiceSummaryModel iVContactInvoiceSummaryModel = new IVContactInvoiceSummaryModel();
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return iVContactInvoiceSummaryModel;
			}
			DataRow dataRow = dataTable.Rows[0];
			iVContactInvoiceSummaryModel.SaleAmount = dataRow["SaleAmount"].ToMDecimal();
			iVContactInvoiceSummaryModel.BillAmount = dataRow["BillAmount"].ToMDecimal();
			return iVContactInvoiceSummaryModel;
		}

		public static IVInvoiceSummaryModel GetInvoiceSummaryModel(MContext ctx, string type, DateTime startDate, DateTime endDate)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder.Append("SELECT SUM(1) as AllCount, SUM(MTaxTotalAmt) as AllAmount,\r\n\t                    SUM(CASE WHEN MStatus = 1 THEN 1 ELSE 0 END) AS DraftCount,SUM(CASE WHEN MStatus = 1 THEN MTaxTotalAmt-MVerifyAmt ELSE 0 END) AS DraftAmount,\r\n\t                    SUM(CASE WHEN MStatus = 2 THEN 1 ELSE 0 END) AS WaitingApprovalCount,SUM(CASE WHEN MStatus = 2 THEN MTaxTotalAmt-MVerifyAmt ELSE 0 END) AS WaitingApprovalAmount,\r\n\t                    SUM(CASE WHEN MStatus = 3 THEN 1 ELSE 0 END) AS WaitingPaymentCount,SUM(CASE WHEN MStatus = 3 THEN MTaxTotalAmt-MVerifyAmt ELSE 0 END) AS WaitingPaymentAmount,\r\n                        SUM(CASE WHEN (MStatus = 4 OR (MStatus=3 AND ifnull(MVerifyAmtFor,0)<>0)) THEN 1 ELSE 0 END) AS PaidCount,\r\n                        SUM(CASE WHEN (MStatus = 4 OR (MStatus=3 AND ifnull(MVerifyAmtFor,0)<>0)) THEN MVerifyAmt ELSE 0 END) AS PaidAmount,\r\n                        SUM(CASE WHEN MStatus = 3 AND MDueDate <= @MNow then  1 ELSE 0 END) AS DueCount,\r\n                        SUM(CASE WHEN MStatus = 3 AND MDueDate <= @MNow then  MTaxTotalAmt-MVerifyAmt ELSE 0 END) AS DueAmount\r\n                        FROM T_IV_Invoice \r\n                        WHERE MIsDelete = 0 AND MOrgID=@MOrgID AND LOCATE(@MType, MType)=1 and MBizDate >=@StartDate AND MBizDate <= @EndDate ");
			stringBuilder2.Append("SELECT SUM(1) as RepeatingCount, SUM(MTaxTotalAmt) as RepeatingAmount FROM T_IV_RepeatInvoice \r\n                        WHERE MIsDelete = 0 AND MOrgID=@MOrgID AND MType=@MType and MBizDate >=@StartDate AND MBizDate <= @EndDate ");
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND MCreatorID=@MCreatorID ");
				stringBuilder2.Append(" AND MCreatorID=@MCreatorID ");
			}
			MySqlParameter[] array = new MySqlParameter[6]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@StartDate", MySqlDbType.DateTime),
				new MySqlParameter("@EndDate", MySqlDbType.DateTime),
				new MySqlParameter("@MCreatorID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MNow", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = type;
			array[2].Value = startDate;
			array[3].Value = endDate;
			array[4].Value = ctx.MUserID;
			MySqlParameter obj = array[5];
			DateTime dateTime = ctx.DateNow;
			dateTime = dateTime.Date;
			dateTime = dateTime.AddDays(1.0);
			obj.Value = dateTime.AddSeconds(-1.0);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dataTable = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0];
			IVInvoiceSummaryModel iVInvoiceSummaryModel = new IVInvoiceSummaryModel();
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return iVInvoiceSummaryModel;
			}
			DataRow dataRow = dataTable.Rows[0];
			iVInvoiceSummaryModel.AllCount = dataRow["AllCount"].ToMInt32();
			iVInvoiceSummaryModel.DraftCount = dataRow["DraftCount"].ToMInt32();
			iVInvoiceSummaryModel.DraftAmount = dataRow["DraftAmount"].ToMDecimal();
			iVInvoiceSummaryModel.WaitingApprovalCount = dataRow["WaitingApprovalCount"].ToMInt32();
			iVInvoiceSummaryModel.WaitingApprovalAmount = dataRow["WaitingApprovalAmount"].ToMDecimal();
			iVInvoiceSummaryModel.WaitingPaymentCount = dataRow["WaitingPaymentCount"].ToMInt32();
			iVInvoiceSummaryModel.WaitingPaymentAmount = dataRow["WaitingPaymentAmount"].ToMDecimal();
			iVInvoiceSummaryModel.PaidCount = dataRow["PaidCount"].ToMInt32();
			iVInvoiceSummaryModel.PaidAmount = dataRow["PaidAmount"].ToMDecimal();
			iVInvoiceSummaryModel.AllAmount = iVInvoiceSummaryModel.DraftAmount + iVInvoiceSummaryModel.WaitingApprovalAmount + iVInvoiceSummaryModel.WaitingPaymentAmount + iVInvoiceSummaryModel.PaidAmount;
			iVInvoiceSummaryModel.DueCount = dataRow["DueCount"].ToMInt32();
			iVInvoiceSummaryModel.DueAmount = dataRow["DueAmount"].ToMDecimal();
			try
			{
				DataTable dataTable2 = dynamicDbHelperMySQL.Query(stringBuilder2.ToString(), array).Tables[0];
				if (dataTable2 != null || dataTable2.Rows.Count > 0)
				{
					DataRow dataRow2 = dataTable2.Rows[0];
					iVInvoiceSummaryModel.RepeatingCount = dataRow2["RepeatingCount"].ToMInt32();
					iVInvoiceSummaryModel.RepeatingAmount = dataRow2["RepeatingAmount"].ToMDecimal();
				}
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
			}
			return iVInvoiceSummaryModel;
		}

		public static List<IVInvoiceModel> GetInvoiceList(MContext ctx, List<string> keyIdList)
		{
			return ModelInfoManager.GetDataModelList<IVInvoiceModel>(ctx, keyIdList);
		}

		public static List<NameValueModel> GetFPInvoiceSummary(MContext ctx, IVInvoiceListFilterModel filter)
		{
			List<NameValueModel> list = new List<NameValueModel>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select ifnull(ee.MIssueStatus,0) as MIssueStatus, a.MTaxID from T_IV_Invoice a ", "JieNor-001");
			stringBuilder.Append(" LEFT JOIN T_BD_Contacts_l b ON a.MOrgID = b.MOrgID and a.MContactID=b.MParentID and b.MOrgID = a.MOrgID AND b.MLocaleID=@MLocaleID and b.MIsDelete=0 ");
			stringBuilder.Append(" LEFT JOIN (select e.MOrgID,e.MInvoiceID,e.MTableID,f.MIssueStatus,f.MNumber\r\n                            from t_fp_invoice_table e\r\n                            inner JOIN t_fp_table f ON f.MOrgID = e.MOrgID and e.MTableID = f.MItemID and e.MIsDelete = 0 and f.MIsDelete = 0\r\n                            where e.MOrgID = @MOrgID)ee on ee.MOrgID = a.MOrgID and a.MID = ee.MInvoiceID");
			stringBuilder.Append(" WHERE a.MIsDelete = 0  AND a.MOrgID=@MOrgID");
			if (!string.IsNullOrWhiteSpace(filter.MType))
			{
				stringBuilder.Append(" AND LOCATE(@MType, a.MType)=1");
			}
			stringBuilder.Append(GetInvoiceListFilterSql(ctx, filter));
			stringBuilder.Append(" GROUP BY a.MID");
			MySqlParameter[] invoiceListParameters = GetInvoiceListParameters(ctx, filter);
			List<IVInvoiceListModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<IVInvoiceListModel>(ctx, stringBuilder.ToString(), invoiceListParameters);
			List<NameValueModel> list2 = list;
			NameValueModel obj = new NameValueModel
			{
				MName = "-1"
			};
			int num = dataModelBySql.Count;
			obj.MValue = num.ToString();
			list2.Add(obj);
			List<NameValueModel> list3 = list;
			NameValueModel nameValueModel = new NameValueModel();
			num = 0;
			nameValueModel.MName = num.ToString();
			num = dataModelBySql.Count((IVInvoiceListModel f) => f.MIssueStatus == 0 && f.MTaxID != "No_Tax");
			nameValueModel.MValue = num.ToString();
			list3.Add(nameValueModel);
			List<NameValueModel> list4 = list;
			NameValueModel nameValueModel2 = new NameValueModel();
			num = 1;
			nameValueModel2.MName = num.ToString();
			num = dataModelBySql.Count((IVInvoiceListModel f) => f.MIssueStatus == 1 && f.MTaxID != "No_Tax");
			nameValueModel2.MValue = num.ToString();
			list4.Add(nameValueModel2);
			List<NameValueModel> list5 = list;
			NameValueModel nameValueModel3 = new NameValueModel();
			num = 2;
			nameValueModel3.MName = num.ToString();
			num = dataModelBySql.Count((IVInvoiceListModel f) => f.MIssueStatus == 2 && f.MTaxID != "No_Tax");
			nameValueModel3.MValue = num.ToString();
			list5.Add(nameValueModel3);
			return list;
		}

		public static List<IVInvoiceListModel> GetInvoiceList(MContext ctx, IVInvoiceListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select  aa.*,group_concat(DISTINCT d.MAttachID) AS MAttachIDs from (");
			stringBuilder.AppendFormat("select a.*,c.MCurrencyID AS MOrgCyID,  convert(AES_DECRYPT(b.MName,'{0}') using utf8)  AS MContactName,", "JieNor-001");
			stringBuilder.Append(" ee.MTableID,ifnull(ee.MIssueStatus,0) as MIssueStatus,ee.MNumber as MTableNumber from T_IV_Invoice a ");
			stringBuilder.Append(" LEFT JOIN T_BD_Contacts_l b ON b.MOrgID = a.MOrgID and  a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID and b.MIsDelete=0 ");
			stringBuilder.Append(" LEFT JOIN T_REG_Financial c ON c.MOrgID = a.MOrgID and c.MIsDelete=0 ");
			stringBuilder.Append(" LEFT JOIN (select e.MOrgID,e.MInvoiceID,e.MTableID,f.MIssueStatus,f.MNumber\r\n                            from t_fp_invoice_table e\r\n                            inner JOIN t_fp_table f ON f.MOrgID = e.MOrgID and e.MTableID = f.MItemID and e.MIsDelete = 0 and f.MIsDelete = 0\r\n                            where e.MOrgID = @MOrgID)ee on ee.MOrgID = a.MOrgID and a.MID = ee.MInvoiceID");
			stringBuilder.Append(" WHERE a.MIsDelete = 0 AND a.MOrgID=@MOrgID");
			if (!string.IsNullOrWhiteSpace(filter.MType))
			{
				stringBuilder.Append(" AND LOCATE(@MType, a.MType)=1");
			}
			stringBuilder.Append(GetInvoiceListFilterSql(ctx, filter));
			if (string.IsNullOrEmpty(filter.Sort))
			{
				if (filter.IsFromExcel || filter.IsFromExport)
				{
					stringBuilder.Append(" ORDER BY a.MNumber ASC");
				}
				else
				{
					stringBuilder.Append(" ORDER BY a.MBizDate DESC,a.MCreateDate DESC ");
				}
			}
			else if (filter.Sort == "MContactName")
			{
				stringBuilder.AppendFormat(" ORDER BY  MContactName", filter.Order);
			}
			else
			{
				stringBuilder.AppendFormat(" ORDER BY a.{0} {1}", filter.Sort, filter.Order);
			}
			stringBuilder.Append(filter.PageSqlString);
			stringBuilder.Append(" )aa");
			stringBuilder.Append(" LEFT JOIN T_IV_InvoiceAttachment d ON d.MOrgID = aa.MOrgID and aa.MID=d.MParentID and d.MIsDelete=0 ");
			stringBuilder.Append(" GROUP BY aa.MID");
			if (string.IsNullOrEmpty(filter.Sort))
			{
				if (filter.IsFromExcel || filter.IsFromExport)
				{
					stringBuilder.Append(" ORDER BY aa.MNumber ASC");
				}
				else
				{
					stringBuilder.Append(" ORDER BY aa.MBizDate DESC,aa.MCreateDate DESC ");
				}
			}
			else if (filter.Sort == "MContactName")
			{
				stringBuilder.AppendFormat(" ORDER BY  MContactName", filter.Order);
			}
			else
			{
				stringBuilder.AppendFormat(" ORDER BY aa.{0} {1}", filter.Sort, filter.Order);
			}
			MySqlParameter[] invoiceListParameters = GetInvoiceListParameters(ctx, filter);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dt = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), invoiceListParameters).Tables[0];
			return ModelInfoManager.DataTableToList<IVInvoiceListModel>(dt);
		}

		public static List<IVInvoiceModel> GetInitInvoiceList(MContext ctx)
		{
			string sql = " select * from T_IV_Invoice where MOrgID=@MOrgID and MBizDate <@MBizDate and MIsDelete=0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MBizDate", ctx.MBeginDate)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dt = dynamicDbHelperMySQL.Query(sql, cmdParms).Tables[0];
			return ModelInfoManager.DataTableToList<IVInvoiceModel>(dt);
		}

		public static List<IVInvoiceModel> GetInvoiceListIncludeEntry(MContext ctx, SqlWhere filter)
		{
			if (filter == null)
			{
				throw new NullReferenceException("filter can not be null");
			}
			return ModelInfoManager.GetDataModelList<IVInvoiceModel>(ctx, filter, false, true);
		}

		public static int GetInvoiceTotalCount(MContext ctx, IVInvoiceListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select COUNT(*) from T_IV_Invoice a ");
			stringBuilder.Append(" LEFT JOIN T_BD_Contacts_l b ON a.MOrgID = b.MOrgID and  a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID and b.MIsDelete=0 ");
			stringBuilder.Append(" LEFT JOIN (select e.MOrgID,e.MInvoiceID,e.MTableID,f.MIssueStatus,f.MNumber\r\n                            from t_fp_invoice_table e\r\n                            inner JOIN t_fp_table f ON f.MOrgID = e.MOrgID and e.MTableID = f.MItemID and e.MIsDelete = 0 and f.MIsDelete = 0\r\n                            where e.MOrgID = @MOrgID)ee on ee.MOrgID = a.MOrgID and a.MID = ee.MInvoiceID");
			stringBuilder.Append(" WHERE a.MIsDelete = 0 AND a.MOrgID=@MOrgID AND LOCATE(@MType, a.MType)=1 ");
			stringBuilder.Append(GetInvoiceListFilterSql(ctx, filter));
			MySqlParameter[] invoiceListParameters = GetInvoiceListParameters(ctx, filter);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(stringBuilder.ToString(), invoiceListParameters);
			return Convert.ToInt32(single.ToString());
		}

		private static MySqlParameter[] GetInvoiceListParameters(MContext ctx, IVInvoiceListFilterModel filter)
		{
			MySqlParameter[] array = new MySqlParameter[18]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStatus", MySqlDbType.Int32, 2),
				new MySqlParameter("@MKeyword", MySqlDbType.VarChar, 500),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 200),
				new MySqlParameter("@MStartDate", MySqlDbType.DateTime),
				new MySqlParameter("@MEndDate", MySqlDbType.DateTime),
				new MySqlParameter("@MCreatorID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MConversionDate", MySqlDbType.DateTime),
				new MySqlParameter("@MIsSent", MySqlDbType.Bit),
				new MySqlParameter("@MTrackItem1", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTrackItem2", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTrackItem3", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTrackItem4", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTrackItem5", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MIssueStatus", MySqlDbType.Int32, 11),
				new MySqlParameter("@MTableID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = filter.MType;
			array[3].Value = filter.MStatus;
			array[4].Value = (string.IsNullOrWhiteSpace(filter.Keyword) ? filter.Keyword : filter.Keyword.Replace("\\", "\\\\"));
			array[5].Value = filter.MContactID;
			array[6].Value = filter.MStartDate;
			array[7].Value = filter.MEndDate;
			array[8].Value = ctx.MUserID;
			array[9].Value = filter.MConversionDate;
			array[10].Value = !filter.MUnsentOnly;
			array[11].Value = filter.MTrackItem1;
			array[12].Value = filter.MTrackItem2;
			array[13].Value = filter.MTrackItem3;
			array[14].Value = filter.MTrackItem4;
			array[15].Value = filter.MTrackItem5;
			array[16].Value = filter.MIssueStatus;
			return array;
		}

		private static string GetInvoiceListFilterSql(MContext ctx, IVInvoiceListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (filter.MStatus > 0)
			{
				if (filter.IsFromFapiao)
				{
					stringBuilder.Append(" AND a.MStatus >= @MStatus ");
				}
				else if (filter.MStatus == Convert.ToInt32(IVInvoiceStatusEnum.Paid))
				{
					stringBuilder.AppendFormat(" AND (MStatus = @MStatus OR (MStatus='{0}' AND ifnull(MVerificationAmt,0)<>0)) ", Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment));
				}
				else
				{
					stringBuilder.Append(" AND a.MStatus = @MStatus ");
				}
			}
			if (filter.IsFromFapiao && filter.MIssueStatus > -1)
			{
				stringBuilder.AppendFormat(" AND ifnull(ee.MIssueStatus,0)=@MIssueStatus");
				stringBuilder.Append(" AND a.MTaxID != 'No_Tax' ");
			}
			if (filter.MIsOnlyInitData)
			{
				stringBuilder.Append(" AND a.MBizDate<@MConversionDate ");
			}
			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				if (filter.IsFromFapiao)
				{
					stringBuilder.AppendFormat(" AND ( (a.MNumber like concat('%',@MKeyword,'%') OR a.MReference LIKE concat('%',@MKeyword,'%') OR abs(a.MTaxTotalAmtFor)=@MKeyword\r\n                        OR convert(AES_DECRYPT(b.MName,'{0}') using utf8) LIKE concat('%',@MKeyword,'%') OR lpad(ee.MNumber, 4, '0') like concat('%',replace(replace(UPPER(@MKeyword), 'SFP', ''),'PFP',''),'%'))) ", "JieNor-001");
				}
				else
				{
					stringBuilder.AppendFormat(" AND ( (a.MNumber like concat('%',@MKeyword,'%') OR a.MReference LIKE concat('%',@MKeyword,'%') OR abs(a.MTaxTotalAmtFor)=@MKeyword\r\n                        OR convert(AES_DECRYPT(b.MName,'{0}') using utf8) LIKE concat('%',@MKeyword,'%'))) ", "JieNor-001");
				}
			}
			if (!string.IsNullOrEmpty(filter.MContactID))
			{
				if (filter.MContactID.IndexOf(',') > 0)
				{
					List<string> values = filter.MContactID.Split(',').ToList();
					string arg = "'" + string.Join("','", values) + "'";
					stringBuilder.AppendFormat(" and a.MContactID in ({0})", arg);
				}
				else
				{
					stringBuilder.Append(" AND a.MContactID=@MContactID ");
				}
			}
			if (filter.MUnsentOnly)
			{
				stringBuilder.Append(" AND a.MIsSent=@MIsSent ");
			}
			DateTime value = new DateTime(1900, 1, 1);
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND a.MCreatorID=@MCreatorID ");
			}
			switch (filter.MSearchWithin)
			{
			case IVInvoiceSearchWithinEnum.AnyDate:
			{
				string value5 = string.Empty;
				if (filter.MStartDate > (DateTime?)value)
				{
					value5 = " AND (a.MBizDate>=@MStartDate OR a.MDueDate>=@MStartDate OR a.MExpectedDate>=@MStartDate ) ";
				}
				if (filter.MEndDate > (DateTime?)value)
				{
					value5 = " AND (a.MBizDate<= @MEndDate OR a.MDueDate<=@MEndDate OR a.MExpectedDate<=@MEndDate) ";
				}
				if (filter.MStartDate > (DateTime?)value && filter.MEndDate > (DateTime?)value)
				{
					value5 = " AND ( (a.MBizDate BETWEEN @MStartDate AND @MEndDate) OR (a.MDueDate BETWEEN @MStartDate AND @MEndDate) OR (a.MExpectedDate BETWEEN @MStartDate AND @MEndDate) ) ";
				}
				stringBuilder.Append(value5);
				break;
			}
			case IVInvoiceSearchWithinEnum.DueDate:
			{
				string value3 = string.Empty;
				if (filter.MStartDate > (DateTime?)value)
				{
					value3 = " AND a.MDueDate>=@MStartDate ";
				}
				if (filter.MEndDate > (DateTime?)value)
				{
					value3 = " AND a.MDueDate<=@MEndDate ";
				}
				if (filter.MStartDate > (DateTime?)value && filter.MEndDate > (DateTime?)value)
				{
					value3 = " AND a.MDueDate  BETWEEN  @MStartDate AND @MEndDate ";
				}
				stringBuilder.Append(value3);
				break;
			}
			case IVInvoiceSearchWithinEnum.TransactionDate:
			{
				string value4 = string.Empty;
				if (filter.MStartDate > (DateTime?)value)
				{
					value4 = " AND a.MBizDate>=@MStartDate  ";
				}
				if (filter.MEndDate > (DateTime?)value)
				{
					value4 = " AND a.MBizDate<= @MEndDate ";
				}
				if (filter.MStartDate > (DateTime?)value && filter.MEndDate > (DateTime?)value)
				{
					value4 = " AND a.MBizDate BETWEEN @MStartDate AND @MEndDate ";
				}
				stringBuilder.Append(value4);
				break;
			}
			case IVInvoiceSearchWithinEnum.ExpectedDate:
			{
				string value2 = string.Empty;
				if (filter.MStartDate > (DateTime?)value)
				{
					value2 = " AND a.MExpectedDate>=@MStartDate  ";
				}
				if (filter.MEndDate > (DateTime?)value)
				{
					value2 = " AND a.MExpectedDate<= @MEndDate ";
				}
				if (filter.MStartDate > (DateTime?)value && filter.MEndDate > (DateTime?)value)
				{
					value2 = " AND a.MExpectedDate BETWEEN @MStartDate AND @MEndDate ";
				}
				stringBuilder.Append(value2);
				break;
			}
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem1))
			{
				stringBuilder.Append(GetTrackItemFilter(filter.MTrackItem1, "1"));
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem2))
			{
				stringBuilder.Append(GetTrackItemFilter(filter.MTrackItem2, "2"));
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem3))
			{
				stringBuilder.Append(GetTrackItemFilter(filter.MTrackItem3, "3"));
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem4))
			{
				stringBuilder.Append(GetTrackItemFilter(filter.MTrackItem4, "4"));
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem5))
			{
				stringBuilder.Append(GetTrackItemFilter(filter.MTrackItem5, "5"));
			}
			return stringBuilder.ToString();
		}

		private static string GetTrackItemFilter(string trackItemValue, string trackItemIndex)
		{
			if (trackItemValue == "-1")
			{
				return $" AND EXISTS(select * from T_IV_InvoiceEntry ae where a.MID=ae.MID AND ae.MIsDelete=0 AND ae.MTrackItem{trackItemIndex}  is  null)";
			}
			return string.Format(" AND EXISTS(select * from T_IV_InvoiceEntry ae where a.MID=ae.MID AND ae.MIsDelete=0 AND ae.MTrackItem{0}=@MTrackItem{0})", trackItemIndex);
		}

		public static OperationResult UpdateInvoice(MContext ctx, IVInvoiceModel model)
		{
			if (!string.IsNullOrWhiteSpace(model.MID))
			{
				OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, model.MID);
				if (model.MStatus >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) && !operationResult.Success)
				{
					return operationResult;
				}
			}
			ResetInvoiceAmt(model);
			if (model.InvoiceEntry != null && model.InvoiceEntry.Count > 0)
			{
				foreach (IVInvoiceEntryModel item in model.InvoiceEntry)
				{
					ResetInvoiceEntryAmt(model.MType, item);
				}
			}
			model.MNumber = GetInvoiceNumber(model.MType, ctx.MOrgID, ctx, model.MID, model.MNumber);
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVInvoiceModel>(ctx, model, null, true));
			list.AddRange(IVInvoiceLogHelper.GetSaveLogCmd(ctx, model));
			int num;
			if ((model.MType == "Invoice_Sale_Red" || model.MType == "Invoice_Purchase_Red") && model.MIsCopyCredit)
			{
				num = ((model.MStatus == Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment)) ? 1 : 0);
				goto IL_014d;
			}
			num = 0;
			goto IL_014d;
			IL_014d:
			if (num != 0)
			{
				IVInvoiceModel dataEditModel = ModelInfoManager.GetDataEditModel<IVInvoiceModel>(ctx, model.MInvCopyID, false, true);
				if (dataEditModel != null && model.MContactID == dataEditModel.MContactID && model.MCyID == dataEditModel.MCyID)
				{
					IVVerificationModel iVVerificationModel = new IVVerificationModel();
					iVVerificationModel.MSourceBillID = model.MID;
					iVVerificationModel.MSourceBillType = "Invoice";
					iVVerificationModel.MTargetBillID = model.MInvCopyID;
					iVVerificationModel.MTargetBillType = "Invoice";
					iVVerificationModel.MDirection = 0;
					decimal num2 = (Math.Abs(model.MTaxTotalAmtFor) > Math.Abs(dataEditModel.MTaxTotalAmtFor - dataEditModel.MVerifyAmtFor)) ? Math.Abs(dataEditModel.MTaxTotalAmtFor - dataEditModel.MVerifyAmtFor) : Math.Abs(model.MTaxTotalAmtFor);
					if (num2 != decimal.Zero)
					{
						iVVerificationModel.MAmount = num2;
						iVVerificationModel.MAmtFor = num2;
						iVVerificationModel.MAmt = Math.Abs(model.MTaxTotalAmt);
						list.AddRange(IVVerificationRepository.GetNewVerificationCmd(ctx, iVVerificationModel));
						list.AddRange(IVInvoiceLogRepository.GetCreditInvoiceLogCmd(ctx, dataEditModel.MID, model.MTaxTotalAmtFor));
						list.AddRange(IVInvoiceLogRepository.GetCreditApplyLogCmd(ctx, model));
					}
				}
			}
			if (model.MBizDate >= ctx.MBeginDate)
			{
				OperationResult operationResult2 = GLInterfaceRepository.GenerateVouchersByBills(ctx, new List<IVInvoiceModel>
				{
					model
				}, null);
				if (operationResult2.Success)
				{
					list.AddRange(operationResult2.OperationCommands);
					goto IL_0343;
				}
				return operationResult2;
			}
			if (ctx.MInitBalanceOver)
			{
				OperationResult operationResult3 = new OperationResult();
				operationResult3.Success = false;
				operationResult3.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InitBalanceIsOver", "The initial balance has been completed and is not allowed to initialize the document operation!");
				return operationResult3;
			}
			goto IL_0343;
			IL_0343:
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num3 = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return new OperationResult
			{
				Success = (num3 > 0),
				ObjectID = model.MID
			};
		}

		private static List<CommandInfo> GetAddContactCmd(MContext ctx, string contactId, string type)
		{
			if (string.IsNullOrWhiteSpace(contactId))
			{
				return null;
			}
			string text = "1";
			switch (type)
			{
			case "Invoice_Sale":
			case "Invoice_Sale_Red":
				text = "1";
				break;
			case "Invoice_Purchase":
			case "Invoice_Purchase_Red":
				text = "2";
				break;
			}
			if (ModelInfoManager.ExistsByFilter<BDContactsTypeLinkModel>(ctx, new SqlWhere().AddDeleteFilter("MIsDelete", SqlOperators.Equal, false).Equal("MContactID", contactId).Equal("MTypeID", text)))
			{
				return null;
			}
			BDContactsTypeLinkModel bDContactsTypeLinkModel = new BDContactsTypeLinkModel();
			bDContactsTypeLinkModel.MTypeID = text;
			bDContactsTypeLinkModel.MContactID = contactId;
			return ModelInfoManager.GetInsertOrUpdateCmd<BDContactsTypeLinkModel>(ctx, bDContactsTypeLinkModel, null, true);
		}

		public static List<CommandInfo> GetImportInvoiceCmdList(MContext ctx, List<IVInvoiceModel> list)
		{
			List<CommandInfo> list2 = new List<CommandInfo>();
			int num = 0;
			DateTime dateNow = ctx.DateNow;
			foreach (IVInvoiceModel item in list)
			{
				if (string.IsNullOrWhiteSpace(item.MID))
				{
					item.MID = UUIDHelper.GetGuid();
					item.IsNew = true;
					item.MCreateDate = dateNow.AddSeconds((double)(-num));
				}
				item.TableName = new IVInvoiceModel().TableName;
				if (item.MBizDate < ctx.MBeginDate)
				{
					item.MStatus = 3;
				}
				else
				{
					item.MStatus = 1;
				}
				ResetInvoiceAmt(item);
				if (item.InvoiceEntry != null && item.InvoiceEntry.Count > 0)
				{
					int num2 = 1;
					foreach (IVInvoiceEntryModel item2 in item.InvoiceEntry)
					{
						if (string.IsNullOrWhiteSpace(item2.MEntryID))
						{
							item2.MEntryID = UUIDHelper.GetGuid();
							item2.IsNew = true;
						}
						ResetInvoiceEntryAmt(item.MType, item2);
						item2.MSeq = num2;
						num2++;
					}
				}
				if (string.IsNullOrWhiteSpace(item.MNumber))
				{
					item.MNumber = GetInvoiceAutoNumber(item.MType, ctx.MOrgID, ctx);
				}
				List<CommandInfo> addInvoiceEditLogCmd = IVInvoiceLogRepository.GetAddInvoiceEditLogCmd(ctx, item);
				list2.AddRange(addInvoiceEditLogCmd);
				if (item.MBizDate >= ctx.MBeginDate)
				{
					OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBill(ctx, item, null);
					if (operationResult.Success)
					{
						list2.AddRange(operationResult.OperationCommands);
					}
				}
				num++;
			}
			CheckInvoiceNoDuplicate(list);
			foreach (IVInvoiceModel item3 in list)
			{
				list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVInvoiceModel>(ctx, item3, null, true));
			}
			return list2;
		}

		public static OperationResult UnApproveInvoice(MContext ctx, IVInvoiceModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = list;
			CommandInfo commandInfo = new CommandInfo
			{
				CommandText = "UPDATE T_IV_Invoice SET MStatus = @MStatus WHERE MID = @MID AND MOrgID=@MOrgID"
			};
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MStatus", Convert.ToInt32(IVInvoiceStatusEnum.Draft)),
				new MySqlParameter("@MID", model.MID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			list2.Add(commandInfo);
			RecordStatus status = RecordStatus.Draft;
			OperationResult operationResult = GLInterfaceRepository.TransferBillCreatedVouchersByStatus(ctx, model.MID, status);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			list.AddRange(operationResult.OperationCommands);
			list.AddRange(IVInvoiceLogHelper.GetUnApproveLogCmd(ctx, model));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return new OperationResult
			{
				Success = (num > 0),
				ObjectID = model.MID
			};
		}

		public static OperationResult ApproveInvoice(MContext ctx, ParamBase param)
		{
			string[] array = param.KeyIDSWithNoSingleQuote.Split(',');
			List<string> list = array.ToList();
			List<CommandInfo> list2 = new List<CommandInfo>();
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			int num = 0;
			string empty = string.Empty;
			operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, list);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			List<IVInvoiceModel> modelList = GetModelList(ctx, list);
			List<MySqlParameter> list3 = new List<MySqlParameter>
			{
				new MySqlParameter("@MStatus", Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment)),
				new MySqlParameter("@Draft", Convert.ToInt32(IVInvoiceStatusEnum.Draft)),
				new MySqlParameter("@WaitingApproval", Convert.ToInt32(IVInvoiceStatusEnum.WaitingApproval)),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			string inFilterQuery = GLUtility.GetInFilterQuery(list, ref list3, "M_ID");
			List<CommandInfo> list4 = list2;
			CommandInfo obj = new CommandInfo
			{
				CommandText = "UPDATE T_IV_Invoice SET MStatus = @MStatus WHERE MID " + inFilterQuery + " AND MOrgID=@MOrgID AND (MStatus =@Draft OR MStatus=@WaitingApproval) "
			};
			DbParameter[] array2 = obj.Parameters = list3.ToArray();
			list4.Add(obj);
			foreach (IVInvoiceModel item in modelList)
			{
				list2.AddRange(IVInvoiceLogHelper.GetApproveLogCmd(ctx, item));
			}
			RecordStatus status = RecordStatus.Saved;
			OperationResult operationResult2 = GLInterfaceRepository.TransferBillsCreatedVouchersByStatus(ctx, (from x in modelList
			select x.MID).Distinct().ToList(), status);
			if (!operationResult2.Success)
			{
				return operationResult2;
			}
			list2.AddRange(operationResult2.OperationCommands);
			if (operationResult.Success && list2.Count > 0)
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				int num2 = dynamicDbHelperMySQL.ExecuteSqlTran(list2);
			}
			if (num == array.Length)
			{
				operationResult.Success = false;
				operationResult.Message = empty;
				return operationResult;
			}
			return operationResult;
		}

		public static OperationResult UpdateInvoiceStatus(MContext ctx, ParamBase param)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = list;
			CommandInfo commandInfo = new CommandInfo
			{
				CommandText = $"UPDATE T_IV_Invoice SET MStatus = @MStatus WHERE MOrgID=@MOrgID AND MID in ({param.KeyIDSWithSingleQuote})"
			};
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[2]
			{
				new MySqlParameter("@MStatus", param.MOperationID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			list2.Add(commandInfo);
			string text = param.KeyIDSWithNoSingleQuote.Replace("'", "");
			List<string> billIds = text.Split(',').ToList();
			string mOperationID = param.MOperationID;
			int num = Convert.ToInt32(IVInvoiceStatusEnum.Draft);
			int num2;
			if (!(mOperationID == num.ToString()))
			{
				string mOperationID2 = param.MOperationID;
				num = Convert.ToInt32(IVInvoiceStatusEnum.WaitingApproval);
				if (!(mOperationID2 == num.ToString()))
				{
					num2 = 0;
					goto IL_00e6;
				}
			}
			num2 = -1;
			goto IL_00e6;
			IL_00e6:
			RecordStatus status = (RecordStatus)num2;
			operationResult = GLInterfaceRepository.TransferBillsCreatedVouchersByStatus(ctx, billIds, status);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			string mOperationID3 = param.MOperationID;
			num = Convert.ToInt32(IVInvoiceStatusEnum.WaitingApproval);
			if (mOperationID3 == num.ToString())
			{
				string[] array2 = param.KeyIDSWithNoSingleQuote.Split(',');
				string[] array3 = array2;
				foreach (string pkID in array3)
				{
					IVInvoiceModel invoiceModel = GetInvoiceModel(ctx, pkID);
					list.AddRange(IVInvoiceLogHelper.GetSubmitForApprovalLogCmd(ctx, invoiceModel));
				}
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num3 = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			IVInvoiceLogRepository.AddInvoiceApprovalLog(ctx, param);
			operationResult.Success = (num3 > 0);
			return operationResult;
		}

		public static void UpdateInvoiceExpectedInfo(MContext ctx, IVInvoiceModel model)
		{
			string empty = string.Empty;
			empty = ((!model.MType.EqualsIgnoreCase("Expense_Claims")) ? "T_IV_Invoice" : "T_IV_Expense");
			string sql = $"UPDATE {empty} SET MExpectedDate = @MExpectedDate,MDesc=@MDesc WHERE MID=@MID AND MOrgID=@MOrgID ";
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MExpectedDate", MySqlDbType.DateTime),
				new MySqlParameter("@MDesc", MySqlDbType.VarChar, 500),
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			array[0].Value = ((model.MExpectedDate < new DateTime(1900, 1, 1)) ? DateTime.Now.Date : model.MExpectedDate);
			array[1].Value = model.MDesc;
			array[2].Value = model.MID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSql(sql, array);
			if (model.MType.EqualsIgnoreCase("Expense_Claims"))
			{
				IVExpenseLogRepository.AddExpenseExpectedInfoLog(ctx, model);
			}
			else
			{
				IVInvoiceLogRepository.AddInvoiceExpectedInfoLog(ctx, model);
			}
		}

		public static IVInvoiceModel GetInvoiceEditModel(MContext ctx, string pkID, string bizType)
		{
			IVInvoiceModel iVInvoiceModel = ModelInfoManager.GetDataEditModel<IVInvoiceModel>(ctx, pkID, false, true);
			if (iVInvoiceModel == null)
			{
				iVInvoiceModel = new IVInvoiceModel();
				iVInvoiceModel.MNumber = GetInvoiceAutoNumber(bizType, ctx.MOrgID, ctx);
			}
			ResetInvoiceAmt(iVInvoiceModel);
			if (iVInvoiceModel.InvoiceEntry != null && iVInvoiceModel.InvoiceEntry.Count > 0)
			{
				foreach (IVInvoiceEntryModel item in iVInvoiceModel.InvoiceEntry)
				{
					ResetInvoiceEntryAmt(iVInvoiceModel.MType, item);
				}
			}
			return iVInvoiceModel;
		}

		public static IVInvoiceModel GetNextUnApproveInvoiceID(MContext ctx, DateTime dt, string bizType)
		{
			if (dt.Year <= 1900)
			{
				dt = DateTime.Now;
			}
			string text = "";
			text = ((!(bizType == "Invoice_Sale") && !(bizType == "Invoice_Sale_Red")) ? "AND (MType='Invoice_Purchase' or MType='Invoice_Purchase_Red')" : " AND (MType='Invoice_Sale' or MType='Invoice_Sale_Red') ");
			string sql = $"SELECT * FROM T_IV_Invoice \r\n                                        WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MCreateDate < @CreateDate {text} AND (MStatus=@Draft or MStatus=@WaitingApproval)\r\n                                        order by MCreateDate desc limit 0,1 ";
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@CreateDate", dt),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@Draft", Convert.ToInt32(IVInvoiceStatusEnum.Draft)),
				new MySqlParameter("@WaitingApproval", Convert.ToInt32(IVInvoiceStatusEnum.WaitingApproval))
			};
			return ModelInfoManager.GetDataModel<IVInvoiceModel>(ctx, sql, cmdParms);
		}

		public static IVInvoiceModel GetInvoiceEditModel(MContext ctx, string pkID)
		{
			return ModelInfoManager.GetDataEditModel<IVInvoiceModel>(ctx, pkID, false, true);
		}

		public static IVInvoiceModel GetInvoiceCopyModel(MContext ctx, string pkID, bool isCopyCredit)
		{
			IVInvoiceModel model = ModelInfoManager.GetDataEditModel<IVInvoiceModel>(ctx, pkID, false, true);
			if (model == null)
			{
				model = new IVInvoiceModel();
			}
			ResetInvoiceAmt(model);
			List<BDCheckInactiveModel> bDInactiveList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).BDInactiveList;
			if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == model.MContactID && m.ObjectType == "Contact") && !isCopyCredit)
			{
				model.MContactID = "";
			}
			model.MID = string.Empty;
			model.MStatus = Convert.ToInt32(IVInvoiceStatusEnum.Draft);
			model.IsNew = true;
			if (model.InvoiceEntry != null && model.InvoiceEntry.Count > 0)
			{
				foreach (IVInvoiceEntryModel item in model.InvoiceEntry)
				{
					ResetInvoiceEntryAmt(model.MType, item);
					item.MEntryID = "";
					item.MID = "";
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MItemID && m.ObjectType == "Item") && !isCopyCredit)
					{
						item.MItemID = "";
					}
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem1 && m.ObjectType == "Track") && !isCopyCredit)
					{
						item.MTrackItem1 = "";
					}
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem2 && m.ObjectType == "Track") && !isCopyCredit)
					{
						item.MTrackItem2 = "";
					}
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem3 && m.ObjectType == "Track") && !isCopyCredit)
					{
						item.MTrackItem3 = "";
					}
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem4 && m.ObjectType == "Track") && !isCopyCredit)
					{
						item.MTrackItem4 = "";
					}
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem5 && m.ObjectType == "Track") && !isCopyCredit)
					{
						item.MTrackItem5 = "";
					}
				}
			}
			if (isCopyCredit)
			{
				string mType = model.MType;
				if (!(mType == "Invoice_Sale"))
				{
					if (mType == "Invoice_Purchase")
					{
						model.MType = "Invoice_Purchase_Red";
					}
				}
				else
				{
					model.MType = "Invoice_Sale_Red";
				}
				ResetCopyInvoiceAmt(ctx, model);
			}
			model.MNumber = GetInvoiceAutoNumber(model.MType, ctx.MOrgID, ctx);
			return model;
		}

		public static IVInvoiceModel GetInvoiceModel(MContext ctx, string pkID)
		{
			return ModelInfoManager.GetDataEditModel<IVInvoiceModel>(ctx, pkID, false, true);
		}

		public static OperationResult DeleteInvoiceList(MContext ctx, ParamBase param)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> deleteBillCmd = IVBaseRepository<IVInvoiceModel>.GetDeleteBillCmd<IVInvoiceModel>(ctx, param, operationResult);
			if (operationResult.Success)
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(deleteBillCmd) > 0);
				IVInvoiceLogRepository.AddInvoiceDeleteLog(ctx, param);
			}
			return operationResult;
		}

		public static string GetChartStackedDictionary(MContext ctx, string Type, DateTime startDate, DateTime endDate, string contactId = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			double num = 0.0;
			List<ChartColumnStacked2DModel> list = (List<ChartColumnStacked2DModel>)(dictionary["data"] = GetUserInviteInfo(ctx, out num, Type, startDate, endDate, contactId));
			dictionary["labels"] = GetLables(ctx);
			dictionary["scalSpace"] = Math.Ceiling(num / 30.0) * 10.0;
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Serialize(dictionary);
		}

		public static List<string> GetExistInvoiceNumberList(MContext ctx, List<string> numberList)
		{
			List<string> list = new List<string>();
			if (numberList == null || !numberList.Any())
			{
				return list;
			}
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list2 = new List<string>();
			List<MySqlParameter> list3 = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			for (int i = 0; i < numberList.Count(); i++)
			{
				string text = $"@MNumber{i}";
				list2.Add(text);
				list3.Add(new MySqlParameter(text, numberList[i]));
			}
			stringBuilder.Append("select MNumber from T_IV_Invoice ");
			stringBuilder.AppendFormat("WHERE MOrgID=@MOrgID AND MNumber in ({0}) and MIsDelete=0 ", string.Join(",", list2));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), list3.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					list.Add(Convert.ToString(row[0]));
				}
			}
			return (from v in list
			orderby v
			select v).ToList();
		}

		public static string GetInvoiceNumber(string type, string orgId, MContext ctx, string invoiceId, string updateNumber)
		{
			//bool flag = false;
			if (!CheckInvoiceNumberIsExist(ctx, type, orgId, invoiceId, updateNumber) && !string.IsNullOrWhiteSpace(updateNumber))
			{
				return updateNumber;
			}
			return GetInvoiceAutoNumber(type, orgId, ctx);
		}

		public static bool CheckInvoiceNumberIsExist(MContext ctx, string type, string orgId, string invoiceId, string updateNumber)
		{
			bool result = false;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select Count(*) AS Total from T_IV_Invoice ");
			stringBuilder.AppendFormat("WHERE MOrgID=@MOrgID AND MNumber = @MNumber and MIsDelete=0 \r\n                              {0}", (type == "Invoice_Sale" || type == "Invoice_Sale_Red") ? " AND (MType='Invoice_Sale' or MType='Invoice_Sale_Red') " : " AND MType=@MType ");
			if (!string.IsNullOrEmpty(invoiceId))
			{
				stringBuilder.Append(" AND MID <> @MID ");
			}
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MNumber", MySqlDbType.VarChar, 100),
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = orgId;
			array[1].Value = type;
			array[2].Value = updateNumber;
			array[3].Value = invoiceId;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(stringBuilder.ToString(), array);
			if (Convert.ToDecimal(single) > decimal.Zero)
			{
				result = true;
			}
			return result;
		}

		public static IVAutoNumberModel GetMaxAutoNumber(MContext ctx)
		{
			string text = "select MType, MAX(RIGHT(MNumber,4)) AS MNo from T_IV_Invoice \n                        WHERE MIsDelete = 0 AND MNumber REGEXP '^(INV|BINV|CN|BCN)-[0-9]{4}$'\n                        GROUP BY MType";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(text.ToString(), cmdParms);
			IVAutoNumberModel iVAutoNumberModel = new IVAutoNumberModel();
			if (dataSet != null || dataSet.Tables.Count > 0)
			{
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					string text2 = row["MType"].ToString();
					int num = Convert.ToInt32(row["MNo"]);
					switch (text2)
					{
					case "Invoice_Sale":
						iVAutoNumberModel.InvoiceSale = num;
						break;
					case "Invoice_Purchase":
						iVAutoNumberModel.InvoicePurchase = num;
						break;
					case "Invoice_Sale_Red":
						iVAutoNumberModel.InvoiceSaleRed = num;
						break;
					case "Invoice_Purchase_Red":
						iVAutoNumberModel.InvoicePurchaseRed = num;
						break;
					}
				}
			}
			return iVAutoNumberModel;
		}

		private static string GetInvoiceAutoNumber(string type, string orgId, MContext ctx)
		{
			string text = "";
			StringBuilder stringBuilder;
			MySqlParameter[] array;
			DynamicDbHelperMySQL dynamicDbHelperMySQL;
			object single;
			switch (type)
			{
			case "Invoice_Sale":
				text = "INV-";
				goto IL_0070;
			case "Invoice_Sale_Red":
				text = "CN-";
				goto IL_0070;
			case "Invoice_Purchase":
				text = "BINV-";
				goto IL_0070;
			case "Invoice_Purchase_Red":
				text = "BCN-";
				goto IL_0070;
			default:
				{
					return string.Empty;
				}
				IL_0070:
				stringBuilder = new StringBuilder();
				stringBuilder.Append("select RIGHT(MNumber,4) AS MNo from T_IV_Invoice ");
				stringBuilder.AppendFormat(" WHERE MIsDelete=0 AND MType=@MType AND MOrgID=@MOrgID AND MNumber REGEXP '{0}[0-9]{{4}}$'", text);
				stringBuilder.Append("ORDER BY MNumber DESC limit 1");
				array = new MySqlParameter[2]
				{
					new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
					new MySqlParameter("@MType", MySqlDbType.VarChar, 36)
				};
				array[0].Value = orgId;
				array[1].Value = type;
				dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				single = dynamicDbHelperMySQL.GetSingle(stringBuilder.ToString(), array);
				if (single != null && single != DBNull.Value)
				{
					int num = 0;
					int.TryParse(single.ToString(), out num);
					num++;
					if (num == 1)
					{
						return $"{text}0001";
					}
					return $"{text}{num.ToString().PadLeft(4, '0')}";
				}
				return $"{text}0001";
			}
		}

		private static DataSet GetUserInviteInfoDataSet(MContext ctx, out double maxAmount, string Type, DateTime startDate, DateTime endDate, DateTime curMonthFirstDay, DateTime NextMonthLastDay, string contactId = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = string.Empty;
			if (!string.IsNullOrWhiteSpace(contactId))
			{
				value = " AND a.MContactID=@MContactID";
			}
			stringBuilder.AppendFormat(" select a.MContactID,  convert(AES_DECRYPT(b.MName,'{0}') using utf8)  as MContactName,b.MFirstName,b.MLastName,'' as MYearMonth,(sum(a.MTaxTotalAmt)-sum(a.MVerifyAmt)) as MComingAmt, ", "JieNor-001");
			stringBuilder.AppendLine(" case when (sum(a.MTaxTotalAmt)-sum(a.MVerifyAmt))<0 then '1' else '-1' end as MChartDueOrOwing ");
			stringBuilder.AppendLine(" from T_IV_Invoice a ");
			stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts_l b ON a.MOrgID = b.MOrgID and a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete = 0");
			stringBuilder.AppendFormat(" WHERE a.MIsDelete = 0 AND a.MStatus=3 AND a.MType in ({0}) AND a.MOrgID=@MOrgID ", Type);
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND a.MCreatorID=@MCreatorID ");
			}
			stringBuilder.AppendLine(" AND a.MBizDate <=@EndDate and a.MBizDate >=@StartDate ");
			stringBuilder.AppendLine(" AND str_to_date(date_format(case when a.MType='Invoice_Sale' or a.MType='Invoice_Purchase' then a.MDueDate else a.MBizDate end,'%Y/%m/%d'),'%Y/%m/%d')<@MFirstDate ");
			stringBuilder.Append(value);
			stringBuilder.AppendLine(" GROUP BY a.MContactID, b.MName,b.MFirstName,b.MLastName");
			stringBuilder.AppendLine(" UNION ALL ");
			stringBuilder.AppendFormat(" select a.MContactID, convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName,b.MFirstName,b.MLastName,DATE_FORMAT(case when a.MType='Invoice_Sale' or a.MType='Invoice_Purchase' then a.MDueDate else a.MBizDate end,'%Y-%m') MYearMonth,(sum(a.MTaxTotalAmt)-sum(a.MVerifyAmt)) as MComingAmt, ", "JieNor-001");
			stringBuilder.AppendLine(" case when (sum(a.MTaxTotalAmt)-sum(a.MVerifyAmt))<0 then '1' else '-1' end as MChartDueOrOwing ");
			stringBuilder.AppendLine(" from T_IV_Invoice a ");
			stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts_l b ON a.MOrgID = b.MOrgID and a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete = 0");
			stringBuilder.AppendFormat(" WHERE a.MIsDelete = 0 AND a.MStatus=3 AND a.MType in ({0}) AND a.MOrgID=@MOrgID ", Type);
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND a.MCreatorID=@MCreatorID ");
			}
			stringBuilder.AppendLine(" AND a.MBizDate <=@EndDate and a.MBizDate >=@StartDate ");
			stringBuilder.AppendLine(" AND str_to_date(date_format(case when a.MType='Invoice_Sale' or a.MType='Invoice_Purchase' then a.MDueDate else a.MBizDate end,'%Y/%m/%d'),'%Y/%m/%d')>=@MFirstDate AND str_to_date(date_format(case when a.MType='Invoice_Sale' or a.MType='Invoice_Purchase' then a.MDueDate else a.MBizDate end,'%Y/%m/%d'),'%Y/%m/%d')<=@MLastDate ");
			stringBuilder.Append(value);
			stringBuilder.AppendLine(" GROUP BY a.MContactID, b.MName,b.MFirstName,b.MLastName,MYearMonth");
			stringBuilder.AppendLine(" ORDER BY MComingAmt DESC,MYearMonth");
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@StartDate", MySqlDbType.DateTime),
				new MySqlParameter("@EndDate", MySqlDbType.DateTime),
				new MySqlParameter("@MFirstDate", MySqlDbType.DateTime),
				new MySqlParameter("@MLastDate", MySqlDbType.DateTime),
				new MySqlParameter("@MCreatorID", MySqlDbType.VarChar, 36)
			};
			list[0].Value = ctx.MOrgID;
			list[1].Value = ctx.MLCID;
			list[2].Value = startDate;
			list[3].Value = endDate;
			list[4].Value = curMonthFirstDay;
			list[5].Value = NextMonthLastDay;
			list[6].Value = ctx.MUserID;
			if (!string.IsNullOrWhiteSpace(contactId))
			{
				list.Add(new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36));
				list[7].Value = contactId;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet result = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), list.ToArray());
			maxAmount = GetMaxAmount(ctx, stringBuilder, list.ToArray());
			return result;
		}

		private static List<ChartColumnStacked2DModel> GetUserInviteInfo(MContext ctx, out double maxAmount, string Type, DateTime startDate, DateTime endDate, string contactId = null)
		{
			DateTime dateTime = ctx.DateNow;
			DateTime date = dateTime.Date;
			DateTime curMonthFirstDay = date.AddDays((double)(1 - date.Day));
			dateTime = curMonthFirstDay.AddMonths(5);
			DateTime nextMonthLastDay = dateTime.AddDays(-1.0);
			DataSet userInviteInfoDataSet = GetUserInviteInfoDataSet(ctx, out maxAmount, Type, startDate, endDate, curMonthFirstDay, nextMonthLastDay, contactId);
			List<ChartColumnStacked2DModel> result = new List<ChartColumnStacked2DModel>();
			if (userInviteInfoDataSet.Tables[0].Rows.Count > 0)
			{
				DataTable dtGrp = GetGroupContactList(ctx, startDate, endDate, nextMonthLastDay, Type, contactId).Tables[0];
				result = DataTableToChartList(userInviteInfoDataSet.Tables[0], dtGrp);
			}
			return result;
		}

		private static List<ChartColumnStacked2DModel> DataTableToChartList(DataTable dt, DataTable dtGrp)
		{
			DateTime now = DateTime.Now;
			List<ChartColumnStacked2DModel> list = new List<ChartColumnStacked2DModel>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < dtGrp.Rows.Count; i++)
				{
					double[] array = new double[6];
					string[] array2 = new string[6];
					ChartColumnStacked2DModel chartColumnStacked2DModel = new ChartColumnStacked2DModel();
					for (int j = 0; j < count; j++)
					{
						if (dt.Rows[j]["MContactID"].ToString() == dtGrp.Rows[i]["MContactID"].ToString())
						{
							string text = dt.Rows[j]["MYearMonth"].ToString();
							if (string.IsNullOrWhiteSpace(text))
							{
								array[0] = Math.Round(Convert.ToDouble(dt.Rows[j]["MComingAmt"]), 2);
								array2[0] = Convert.ToString(dt.Rows[j]["MChartDueOrOwing"]);
							}
							else
							{
								for (int k = 0; k < 5; k++)
								{
									string a = text;
									DateTime dateTime = now.AddMonths(k);
									string str = dateTime.ToString("yyyy");
									dateTime = now.AddMonths(k);
									if (a == str + "-" + dateTime.ToString("MM"))
									{
										array[k + 1] = Math.Round(Convert.ToDouble(dt.Rows[j]["MComingAmt"]), 2);
										array2[k + 1] = Convert.ToString(dt.Rows[j]["MChartDueOrOwing"]);
										break;
									}
								}
							}
						}
					}
					chartColumnStacked2DModel.value = array;
					var obj = new
					{
						name = dtGrp.Rows[i]["MContactName"].ToString(),
						MContactID = dtGrp.Rows[i]["MContactID"].ToString(),
						MChartFirstName = dtGrp.Rows[i]["MFirstName"].ToString(),
						MChartLastName = dtGrp.Rows[i]["MLastName"].ToString(),
						MChartDueOrOwing = string.Join(",", array2)
					};
					chartColumnStacked2DModel.name = new JavaScriptSerializer().Serialize(obj);
					list.Add(chartColumnStacked2DModel);
				}
			}
			return list;
		}

		private static DataSet GetGroupContactList(MContext ctx, DateTime startDate, DateTime endDate, DateTime NextMonthLastDay, string Type, string contactId = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select a.MContactID, convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName,b.MFirstName,b.MLastName", "JieNor-001");
			stringBuilder.AppendLine(" from T_IV_Invoice a");
			stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts_l b ON b.MOrgID = a.MOrgID and a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete = 0");
			stringBuilder.AppendFormat(" WHERE a.MIsDelete = 0 AND a.MStatus=3 AND a.MType in ({0}) AND a.MOrgID=@MOrgID AND a.MDueDate<=@MLastDate ", Type);
			stringBuilder.AppendLine(" AND a.MBizDate <=@EndDate AND a.MBizDate >=@StartDate ");
			if (!string.IsNullOrWhiteSpace(contactId))
			{
				stringBuilder.Append(" AND a.MContactID=@MContactID");
			}
			stringBuilder.AppendLine(" GROUP BY a.MContactID, b.MName,b.MFirstName,b.MLastName");
			stringBuilder.AppendLine(" ORDER BY (sum(a.MTaxTotalAmt)-sum(a.MVerificationAmt)) DESC");
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MLastDate", MySqlDbType.DateTime),
				new MySqlParameter("@StartDate", MySqlDbType.DateTime),
				new MySqlParameter("@EndDate", MySqlDbType.DateTime)
			};
			list[0].Value = ctx.MOrgID;
			list[1].Value = ctx.MLCID;
			list[2].Value = NextMonthLastDay;
			list[3].Value = startDate;
			list[4].Value = endDate;
			if (!string.IsNullOrWhiteSpace(contactId))
			{
				list.Add(new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36));
				list[5].Value = contactId;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Query(stringBuilder.ToString(), list.ToArray());
		}

		private static string[] GetLables(MContext ctx)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_Jan", "Jan");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_Feb", "Feb");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_Mar", "Mar");
			string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_Apr", "Apr");
			string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_May", "May");
			string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_Jun", "Jun");
			string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_Jul", "Jul");
			string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_Aug", "Aug");
			string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_Sep", "Sep");
			string text10 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_Oct", "Oct");
			string text11 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_Nov", "Nov");
			string text12 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Month_Dec", "Dec");
			string text13 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Older", "Older");
			string[] result = new string[0];
			switch (DateTime.Now.Month.ToString())
			{
			case "1":
				result = new string[6]
				{
					text13,
					text,
					text2,
					text3,
					text4,
					text5
				};
				break;
			case "2":
				result = new string[6]
				{
					text13,
					text2,
					text3,
					text4,
					text5,
					text6
				};
				break;
			case "3":
				result = new string[6]
				{
					text13,
					text3,
					text4,
					text5,
					text6,
					text7
				};
				break;
			case "4":
				result = new string[6]
				{
					text13,
					text4,
					text5,
					text6,
					text7,
					text8
				};
				break;
			case "5":
				result = new string[6]
				{
					text13,
					text5,
					text6,
					text7,
					text8,
					text9
				};
				break;
			case "6":
				result = new string[6]
				{
					text13,
					text6,
					text7,
					text8,
					text9,
					text10
				};
				break;
			case "7":
				result = new string[6]
				{
					text13,
					text7,
					text8,
					text9,
					text10,
					text11
				};
				break;
			case "8":
				result = new string[6]
				{
					text13,
					text8,
					text9,
					text10,
					text11,
					text12
				};
				break;
			case "9":
				result = new string[6]
				{
					text13,
					text9,
					text10,
					text11,
					text12,
					text
				};
				break;
			case "10":
				result = new string[6]
				{
					text13,
					text10,
					text11,
					text12,
					text,
					text2
				};
				break;
			case "11":
				result = new string[6]
				{
					text13,
					text11,
					text12,
					text,
					text2,
					text3
				};
				break;
			case "12":
				result = new string[6]
				{
					text13,
					text12,
					text,
					text2,
					text3,
					text4
				};
				break;
			}
			return result;
		}

		private static double GetMaxAmount(MContext ctx, StringBuilder strSql, MySqlParameter[] parameters)
		{
			double result = 100.0;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select sum(ifnull(MComingAmt,0)) MMaxAmt from (");
			stringBuilder.AppendLine(strSql.ToString());
			stringBuilder.AppendLine(" ) t");
			stringBuilder.AppendLine(" GROUP BY MYearMonth");
			stringBuilder.AppendLine(" ORDER BY MMaxAmt DESC");
			stringBuilder.AppendLine(" limit 1");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), parameters);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				result = Convert.ToDouble(dataSet.Tables[0].Rows[0]["MMaxAmt"]);
			}
			return result;
		}

		private static void CheckInvoiceNoDuplicate(List<IVInvoiceModel> list)
		{
			List<string> list2 = (from f in list
			select f.MNumber into c
			group c by c into g
			where g.Count() > 1
			select g into f
			select f.Key).ToList();
			if (list2.Any())
			{
				List<int> list3 = new List<int>();
				int item = 0;
				string[] array = null;
				foreach (IVInvoiceModel item2 in list)
				{
					string[] array2 = item2.MNumber.Split('-');
					if (array2.Length == 2 && int.TryParse(array2[1], out item))
					{
						list3.Add(item);
					}
				}
				foreach (string item3 in list2)
				{
					List<IVInvoiceModel> list4 = (from f in list
					where f.MNumber == Convert.ToString(item3)
					select f).ToList();
					for (int i = 1; i < list4.Count(); i++)
					{
						array = list4[i].MNumber.Split('-');
						if (array.Length == 2)
						{
							list4[i].MNumber = $"{array[0]}-{Convert.ToString(list3.Max() + i).PadLeft(4, '0')}";
						}
					}
				}
			}
		}

		public static List<ChartPie2DModel> GetChartPieDictionary(MContext ctx, string Type, DateTime startDate, DateTime endDate)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select MContactID, MContactName, MFirstName, MLastName,ifnull(sum(ifnull(MOwingAmt,0)),0) as MOwingAllAmt,ifnull(sum(ifnull(MDueAmt,0)),0) as MDueAllAmt");
			stringBuilder.AppendLine(" from (");
			stringBuilder.AppendFormat(" select a.MContactID,convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName,b.MFirstName,b.MLastName,sum(a.MTaxTotalAmt)-sum(a.MVerifyAmt) as MOwingAmt,0 as MDueAmt ", "JieNor-001");
			stringBuilder.AppendLine(" from T_IV_Invoice a ");
			stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts_l b ON b.MOrgID = a.MOrgID and a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete = 0");
			stringBuilder.AppendFormat(" WHERE a.MIsDelete = 0 AND a.MBizDate <=@EndDate and a.MBizDate >=@StartDate AND a.MStatus=3 AND a.MType in ({0}) AND a.MOrgID=@MOrgID ", Type);
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND a.MCreatorID=@MCreatorID ");
			}
			stringBuilder.AppendLine(" GROUP BY a.MContactID, b.MName,b.MFirstName,b.MLastName");
			stringBuilder.AppendLine(" UNION ALL ");
			stringBuilder.AppendFormat(" select a.MContactID, convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName,b.MFirstName,b.MLastName,0 as MOwingAmt,sum(a.MTaxTotalAmt)-sum(a.MVerifyAmt) as MDueAmt ", "JieNor-001");
			stringBuilder.AppendLine(" from T_IV_Invoice a ");
			stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts_l b ON a.MOrgID = b.MOrgID and a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete = 0");
			stringBuilder.AppendFormat(" WHERE  a.MIsDelete = 0 AND a.MBizDate <=@EndDate and a.MBizDate >=@StartDate AND a.MStatus=3 AND a.MType in ({0}) AND a.MOrgID=@MOrgID ", Type);
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND a.MCreatorID=@MCreatorID ");
			}
			stringBuilder.AppendLine(" AND str_to_date(date_format(case when a.MType='Invoice_Sale' or a.MType='Invoice_Purchase' then a.MDueDate else a.MBizDate end,'%Y/%m/%d'),'%Y/%m/%d')<@NowDate ");
			stringBuilder.AppendLine(" GROUP BY a.MContactID, b.MName,b.MFirstName,b.MLastName");
			stringBuilder.AppendLine(" )t");
			stringBuilder.AppendLine(" GROUP BY MContactID, MContactName, MFirstName, MLastName");
			stringBuilder.AppendLine(" HAVING MOwingAllAmt>0");
			stringBuilder.AppendLine(" ORDER BY MOwingAllAmt DESC");
			MySqlParameter[] array = new MySqlParameter[6]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@StartDate", MySqlDbType.DateTime),
				new MySqlParameter("@EndDate", MySqlDbType.DateTime),
				new MySqlParameter("@NowDate", MySqlDbType.DateTime),
				new MySqlParameter("@MCreatorID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = startDate;
			array[3].Value = endDate;
			array[4].Value = ctx.DateNow;
			array[5].Value = ctx.MUserID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			List<ChartPie2DModel> result = new List<ChartPie2DModel>();
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				result = DataTableToChartPieList(dataSet.Tables[0]);
			}
			return result;
		}

		public static List<ChartPie2DModel> DataTableToChartPieList(DataTable dt)
		{
			List<ChartPie2DModel> list = new List<ChartPie2DModel>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					ChartPie2DModel chartPie2DModel = new ChartPie2DModel();
					chartPie2DModel.name = dt.Rows[i]["MContactName"].ToString();
					chartPie2DModel.value = Math.Round(Convert.ToDecimal(dt.Rows[i]["MOwingAllAmt"]), 2);
					chartPie2DModel.MOverDue = Math.Round(Convert.ToDecimal(dt.Rows[i]["MDueAllAmt"]), 2);
					chartPie2DModel.MContactID = dt.Rows[i]["MContactID"].ToString();
					chartPie2DModel.MChartFirstName = dt.Rows[i]["MFirstName"].ToString();
					chartPie2DModel.MChartLastName = dt.Rows[i]["MLastName"].ToString();
					list.Add(chartPie2DModel);
				}
			}
			return list;
		}

		public static List<IVStatementsModel> GetStatementData(MContext ctx, IVStatementListFilterModel param)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT MContactID,MContactName,MEmail,MPAttention,MPStreet,MPCityID,MPRegion,MPPostalNo,MPCountryName,SUM(MBalanceAmt) as MBalanceAmt,SUM(MOverdueAmt) as MOverdueAmt,MIsActive FROM (");
			stringBuilder.AppendFormat(" select a.MContactID, convert(AES_DECRYPT(bl.MName,'{0}') using utf8) as MContactName,convert(AES_DECRYPT(b.MEmail,'{0}') using utf8) as MEmail,b.MIsActive,\r\n                                    bl.MPAttention,bl.MPStreet,bl.MPCityID,bl.MPRegion,b.MPPostalNo,t11.MName as MPCountryName,", "JieNor-001");
			stringBuilder.AppendLine(" (a.MTaxTotalAmt-a.MVerifyAmt) as MBalanceAmt, ");
			stringBuilder.AppendLine(" (CASE WHEN  (a.MType=@Invoice_Sale AND str_to_date(date_format(a.MDueDate,'%Y/%m/%d'),'%Y/%m/%d')<@EndDate) \r\n                                        OR  (a.MType=@Invoice_Sale_Red AND str_to_date(date_format(a.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')<@EndDate) \r\n                                then a.MTaxTotalAmt- a.MVerifyAmt else 0 end) as MOverdueAmt ");
			stringBuilder.AppendLine(" from T_IV_Invoice a ");
			stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts b ON a.MContactID=b.MItemID AND b.MIsDelete = 0");
			stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts_l bl ON b.MItemID=bl.MParentID AND bl.MLocaleID=@MLocaleID AND bl.MIsDelete = 0");
			stringBuilder.AppendLine(" LEFT JOIN T_BAS_Country_L t11 ON b.MPCountryID=t11.MParentID AND t11.MLocaleID=@MLocaleID AND t11.MIsDelete = 0");
			stringBuilder.AppendLine(" where a.MOrgID=@MOrgID AND a.MIsDelete=0 and str_to_date(date_format(a.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')<=@EndDate ");
			if (param.StatementType == "Outstanding")
			{
				stringBuilder.AppendLine(" AND a.MStatus=3 ");
			}
			else
			{
				stringBuilder.AppendLine(" AND str_to_date(date_format(a.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')>=@StartDate ");
			}
			stringBuilder.AppendLine("  AND (a.MType=@Invoice_Sale or a.MType=@Invoice_Sale_Red) ");
			stringBuilder.AppendLine(" ) a ");
			if (!string.IsNullOrWhiteSpace(param.Filter))
			{
				stringBuilder.AppendLine(" WHERE CONCAT_WS(',',MContactName,MEmail,MPAttention,MPStreet,MPCityID,MPRegion,MPPostalNo,MPCountryName) LIKE @Filter");
			}
			stringBuilder.AppendLine("  GROUP BY MContactID ");
			MySqlParameter[] array = new MySqlParameter[7]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@EndDate", MySqlDbType.DateTime),
				new MySqlParameter("@Filter", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Sale_Red", MySqlDbType.VarChar, 36),
				new MySqlParameter("@StartDate", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = param.EndDate;
			array[3].Value = "%" + param.Filter + "%";
			array[4].Value = "Invoice_Sale";
			array[5].Value = "Invoice_Sale_Red";
			array[6].Value = param.StartDate;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			List<IVStatementsModel> list = DataTableToStatementsList(ctx, dataSet.Tables[0]);
			return list.Sort(param.Sort, param.Order);
		}

		public static List<IVStatementsModel> DataTableToStatementsList(MContext ctx, DataTable dt)
		{
			List<IVStatementsModel> list = new List<IVStatementsModel>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					IVStatementsModel iVStatementsModel = new IVStatementsModel();
					iVStatementsModel.MItemID = dt.Rows[i]["MContactID"].ToString();
					iVStatementsModel.MName = dt.Rows[i]["MContactName"].ToString();
					iVStatementsModel.MEmail = dt.Rows[i]["MEmail"].ToString();
					iVStatementsModel.MBalance = Math.Round(Convert.ToDecimal(dt.Rows[i]["MBalanceAmt"]), 2);
					iVStatementsModel.MOverdue = Math.Round(Convert.ToDecimal(dt.Rows[i]["MOverdueAmt"]), 2);
					if (ctx.MLCID == "0x0009")
					{
						iVStatementsModel.MAddress = dt.Rows[i]["MPAttention"] + GetJoinString(dt.Rows[i]["MPStreet"].ToString()) + GetJoinString(dt.Rows[i]["MPCityID"].ToString()) + GetJoinString(dt.Rows[i]["MPRegion"].ToString()) + GetJoinString(dt.Rows[i]["MPCountryName"].ToString()) + GetJoinString(dt.Rows[i]["MPPostalNo"].ToString());
					}
					else
					{
						iVStatementsModel.MAddress = dt.Rows[i]["MPAttention"] + GetJoinString(dt.Rows[i]["MPCountryName"].ToString()) + GetJoinString(dt.Rows[i]["MPRegion"].ToString()) + GetJoinString(dt.Rows[i]["MPCityID"].ToString()) + GetJoinString(dt.Rows[i]["MPStreet"].ToString()) + GetJoinString(dt.Rows[i]["MPPostalNo"].ToString());
					}
					iVStatementsModel.MIsActive = (dt.Rows[i]["MIsActive"].ToString() == "1");
					list.Add(iVStatementsModel);
				}
			}
			return list;
		}

		public static string GetJoinString(string joinStr)
		{
			string result = string.Empty;
			if (!string.IsNullOrWhiteSpace(joinStr))
			{
				result = "," + joinStr;
			}
			return result;
		}

		public static List<IVViewStatementModel> GetViewStatementData(MContext ctx, string contactID, string type, DateTime BeginDate, DateTime EndDate)
		{
			if (type == "Outstanding")
			{
				return GetOutstandingData(ctx, contactID, EndDate, true);
			}
			if (type == "Activity")
			{
				return GetActivityData(ctx, contactID, BeginDate, EndDate);
			}
			return null;
		}

		private static List<IVViewStatementModel> GetOutstandingData(MContext ctx, string contactID, DateTime EndDate, bool isCanEqual = true)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select t1.MID,t1.MBizDate,t1.MNumber,t1.MReference,");
			stringBuilder.AppendLine("case t1.MType");
			stringBuilder.AppendLine("when @Invoice_Sale_Red then ''");
			stringBuilder.AppendLine("else t1.MDueDate end as MDueDate,");
			stringBuilder.AppendLine("ifnull(t1.MTaxTotalAmtFor,0) as MTaxTotalAmtFor,");
			stringBuilder.AppendLine("ifnull(t2.MPayments,0) as MPayments,(ifnull(t1.MTaxTotalAmtFor,0)-ifnull(t2.MPayments,0)) as MBalance,t1.MCyID as MCurrencyID,t3.MName as MCurrencyName");
			stringBuilder.AppendLine("from T_IV_Invoice t1");
			stringBuilder.AppendLine("left join (");
			stringBuilder.AppendLine(" select MBillID,sum(ifnull(MPayments,0)) MPayments from (");
			stringBuilder.AppendLine(" select a.MSourceBillID as MBillID,sum(ifnull(a.MAmount,0)) MPayments");
			stringBuilder.AppendLine(" from T_IV_Verification a ");
			stringBuilder.AppendLine(" join T_IV_Receive b on a.MOrgID = b.MOrgID and  a.MTargetBillID=b.MID and b.MIsDelete=0 ");
			stringBuilder.AppendLine("where a.MIsDelete=0 and a.MOrgID = @MOrgID ");
			if (isCanEqual)
			{
				stringBuilder.AppendLine("and str_to_date(date_format(b.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')<=@EndDate");
			}
			else
			{
				stringBuilder.AppendLine("and str_to_date(date_format(b.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')<@EndDate");
			}
			stringBuilder.AppendLine(" group by a.MSourceBillID");
			stringBuilder.AppendLine(" union all");
			stringBuilder.AppendLine(" select a.MTargetBillID as MBillID,sum(ifnull(a.MAmount,0)) MPayments");
			stringBuilder.AppendLine(" from T_IV_Verification a ");
			stringBuilder.AppendLine(" join T_IV_Receive b on a.MOrgID = b.MOrgID and  a.MSourceBillID=b.MID and b.MIsDelete=0 ");
			stringBuilder.AppendLine("where a.MIsDelete=0 AND a.MOrgID = @MOrgID ");
			if (isCanEqual)
			{
				stringBuilder.AppendLine("and str_to_date(date_format(b.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')<=@EndDate");
			}
			else
			{
				stringBuilder.AppendLine("and str_to_date(date_format(b.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')<@EndDate");
			}
			stringBuilder.AppendLine(" group by a.MTargetBillID");
			stringBuilder.AppendLine(" ) v1 group by MBillID ");
			stringBuilder.AppendLine(") t2 on t1.MID=t2.MBillID");
			stringBuilder.AppendLine("left join T_Bas_Currency_L t3 on t1.MCyID=t3.MParentID and t3.MLocaleID=@MLocaleID and t3.MIsDelete=0 ");
			stringBuilder.AppendLine("where t1.MIsDelete=0 AND t1.MStatus=3 AND t1.MOrgID=@MOrgID and t1.MContactID=@contactID and (t1.MType=@Invoice_Sale or t1.MType=@Invoice_Sale_Red)");
			if (isCanEqual)
			{
				stringBuilder.AppendLine("and str_to_date(date_format(t1.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')<=@EndDate");
			}
			else
			{
				stringBuilder.AppendLine("and str_to_date(date_format(t1.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')<@EndDate");
			}
			stringBuilder.AppendLine("order by t1.MCyID,t1.MBizDate,t1.MNumber");
			MySqlParameter[] array = new MySqlParameter[6]
			{
				new MySqlParameter("@contactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@EndDate", MySqlDbType.DateTime),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@Invoice_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Sale_Red", MySqlDbType.VarChar, 36)
			};
			array[0].Value = contactID;
			array[1].Value = EndDate;
			array[2].Value = ctx.MOrgID;
			array[3].Value = ctx.MLCID;
			array[4].Value = "Invoice_Sale";
			array[5].Value = "Invoice_Sale_Red";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			return FormatDataList(dataSet.Tables[0], false);
		}

		public static List<IVViewStatementModel> GetViewStatementOpeningBalanceDate(MContext ctx, string statementType, string contactId, DateTime beginDate)
		{
			if (statementType == "Activity")
			{
				return GetOutstandingData(ctx, contactId, beginDate, false);
			}
			return new List<IVViewStatementModel>();
		}

		private static List<IVViewStatementModel> GetActivityData(MContext ctx, string contactID, DateTime BeginDate, DateTime EndDate)
		{
			string str = "select t1.MID,t1.MBizDate,t1.MNumber,t1.MReference,\r\n                        case t1.MType when @Invoice_Sale_Red then '' else t1.MDueDate end as MDueDate,\r\n                        ifnull(t1.MTaxTotalAmtFor,0) as MTaxTotalAmtFor,\r\n                        0 as MPayments,0 MBalance,t1.MCyID as MCurrencyID,t2.MName as MCurrencyName,t1.MType\r\n                        from T_IV_Invoice t1\r\n                        left join T_Bas_Currency_L t2 on t1.MCyID=t2.MParentID and t2.MLocaleID=@MLocaleID and t2.MIsDelete=0\r\n                        where t1.MIsDelete=0 AND t1.MOrgID=@MOrgID and (t1.MType=@Invoice_Sale or t1.MType=@Invoice_Sale_Red)\r\n                        and t1.MContactID=@contactID\r\n                        and t1.MBizDate>@BeginDate and t1.MBizDate<@EndDate\r\n                        union all ";
			str += "select t1.MID,t1.MBizDate,t1.MNumber,t1.MReference,'' as MDueDate,0 as MTaxTotalAmtFor,t1.MTaxTotalAmtFor as MPayments,\r\n                        0 MBalance,t1.MCyID as MCurrencyID,t2.MName as MCurrencyName,t1.Mtype\r\n                        from T_IV_Receive t1\r\n                        left join T_Bas_Currency_L t2 on t1.MCyID=t2.MParentID and t2.MLocaleID=@MLocaleID and t2.MIsDelete=0\r\n                        where t1.MIsDelete=0 AND t1.MOrgID=@MOrgID and t1.MContactID=@contactID and t1.Mtype='Receive_Sale'\r\n                        and t1.MBizDate>@BeginDate and t1.MBizDate<@EndDate\r\n                        union all ";
			str += "select t1.MID,t1.MBizDate,t1.MNumber,t1.MReference,'' as MDueDate,0 as MTaxTotalAmtFor,-t1.MTaxTotalAmtFor as MPayments,\r\n                        0 MBalance,t1.MCyID as MCurrencyID,t2.MName as MCurrencyName,t1.Mtype\r\n                        from t_iv_payment t1\r\n                        left join T_Bas_Currency_L t2 on t1.MCyID=t2.MParentID and t2.MLocaleID=@MLocaleID and t2.MIsDelete=0\r\n                        where t1.MIsDelete=0 AND t1.MOrgID=@MOrgID and t1.MContactID=@contactID and t1.Mtype='Pay_PurReturn'\r\n                        and t1.MBizDate>@BeginDate and t1.MBizDate<@EndDate ";
			str += "order by MCurrencyID,MBizDate,locate(rtrim(MType),'Invoice_Sale,Invoice_Sale_Red,Receive_Sale,Pay_PurReturn'),MNumber";
			MySqlParameter[] array = new MySqlParameter[7]
			{
				new MySqlParameter("@contactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@BeginDate", MySqlDbType.DateTime),
				new MySqlParameter("@EndDate", MySqlDbType.DateTime),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@Invoice_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Sale_Red", MySqlDbType.VarChar, 36)
			};
			array[0].Value = contactID;
			array[1].Value = BeginDate.ToDayFirstSecond();
			array[2].Value = EndDate.ToDayLastSecond();
			array[3].Value = ctx.MOrgID;
			array[4].Value = ctx.MLCID;
			array[5].Value = "Invoice_Sale";
			array[6].Value = "Invoice_Sale_Red";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(str.ToString(), array);
			return FormatDataList(dataSet.Tables[0], true);
		}

		private static List<IVViewStatementModel> FormatDataList(DataTable dt, bool isActivity = false)
		{
			List<IVViewStatementModel> list = new List<IVViewStatementModel>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					IVViewStatementModel iVViewStatementModel = new IVViewStatementModel();
					iVViewStatementModel.MID = dt.Rows[i]["MID"].ToString();
					IVViewStatementModel iVViewStatementModel2 = iVViewStatementModel;
					DateTime dateTime = Convert.ToDateTime(dt.Rows[i]["MBizDate"]);
					iVViewStatementModel2.MDate = dateTime.ToString("MM/dd/yyyy");
					iVViewStatementModel.MActivity = GetActivityName(Convert.ToDecimal(dt.Rows[i]["MTaxTotalAmtFor"]), Convert.ToDecimal(dt.Rows[i]["MPayments"]), isActivity) + dt.Rows[i]["MNumber"].ToString();
					iVViewStatementModel.MReference = dt.Rows[i]["MReference"].ToString();
					if (!string.IsNullOrWhiteSpace(dt.Rows[i]["MDueDate"].ToString()))
					{
						IVViewStatementModel iVViewStatementModel3 = iVViewStatementModel;
						dateTime = Convert.ToDateTime(dt.Rows[i]["MDueDate"]);
						iVViewStatementModel3.MDueDate = dateTime.ToString("MM/dd/yyyy");
					}
					else
					{
						iVViewStatementModel.MDueDate = dt.Rows[i]["MDueDate"].ToString();
					}
					iVViewStatementModel.MInvoiceAmount = Math.Round(Convert.ToDecimal(dt.Rows[i]["MTaxTotalAmtFor"]), 2);
					iVViewStatementModel.MPayments = Math.Round(Convert.ToDecimal(dt.Rows[i]["MPayments"]), 2);
					iVViewStatementModel.MBalance = Math.Round(Convert.ToDecimal(dt.Rows[i]["MBalance"]), 2);
					iVViewStatementModel.MCurrencyID = dt.Rows[i]["MCurrencyID"].ToString();
					iVViewStatementModel.MCurrencyName = dt.Rows[i]["MCurrencyName"].ToString();
					list.Add(iVViewStatementModel);
				}
			}
			return list;
		}

		private static string GetActivityName(decimal InvoiceAmount, decimal Payments, bool isActivity)
		{
			string result = string.Empty;
			if (isActivity)
			{
				if (InvoiceAmount > decimal.Zero && Payments == decimal.Zero)
				{
					result = "Invoice #";
				}
				else if (InvoiceAmount < decimal.Zero && Payments == decimal.Zero)
				{
					result = "Credit Note #";
				}
				else if (InvoiceAmount == decimal.Zero && Payments > decimal.Zero)
				{
					result = "Payment on Invoice #";
				}
				else if (InvoiceAmount == decimal.Zero && Payments < decimal.Zero)
				{
					result = "Cash Refund on Credit Note #";
				}
				else if (InvoiceAmount > decimal.Zero && Payments > decimal.Zero)
				{
					result = "Prepayment #";
				}
			}
			else if (InvoiceAmount > decimal.Zero)
			{
				result = "Invoice #";
			}
			else if (InvoiceAmount < decimal.Zero)
			{
				result = "Credit Note #";
			}
			return result;
		}

		public static string GetOverPastDictionary(MContext ctx, string contactID)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			double num = 0.0;
			List<ChartColumnStacked2DModel> list = (List<ChartColumnStacked2DModel>)(dictionary["data"] = GetOverPastInfo(ctx, contactID, out num));
			dictionary["labels"] = GetOverPastLables();
			dictionary["scalSpace"] = Math.Ceiling(num / 30.0) * 10.0;
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Serialize(dictionary);
		}

		private static List<ChartColumnStacked2DModel> GetOverPastInfo(MContext ctx, string contactID, out double maxAmount)
		{
			StringBuilder stringBuilder = new StringBuilder();
			DateTime dateTime = DateTime.Now;
			DateTime date = dateTime.Date;
			DateTime dateTime2 = date.AddDays((double)(1 - date.Day));
			dateTime = dateTime2.AddMonths(1);
			DateTime dateTime3 = dateTime.AddDays(-1.0);
			DateTime dateTime4 = dateTime2.AddMonths(-11);
			stringBuilder.AppendLine(" select 'Money In' as name,DATE_FORMAT(MBizDate,'%Y-%m') MYearMonth,sum(a.MTaxTotalAmt) as MComingAmt ");
			stringBuilder.AppendLine(" from T_IV_Receive a ");
			stringBuilder.AppendLine(" WHERE a.MIsDelete = 0 AND a.MOrgID=@MOrgID and a.MContactID=@contactID");
			stringBuilder.AppendLine(" AND str_to_date(date_format(a.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')>=@PreMonthFirstDay ");
			stringBuilder.AppendLine(" AND str_to_date(date_format(a.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')<=@curMonthLastDay ");
			stringBuilder.AppendLine(" GROUP BY DATE_FORMAT(a.MBizDate,'%Y-%m')");
			stringBuilder.AppendLine(" UNION ALL ");
			stringBuilder.AppendLine(" select 'Money Out' as name,DATE_FORMAT(MBizDate,'%Y-%m') MYearMonth,sum(a.MTaxTotalAmt) as MComingAmt ");
			stringBuilder.AppendLine(" from T_IV_Payment a ");
			stringBuilder.AppendLine(" WHERE a.MIsDelete = 0  AND a.MOrgID=@MOrgID and a.MContactID=@contactID");
			stringBuilder.AppendLine(" AND str_to_date(date_format(a.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')>=@PreMonthFirstDay ");
			stringBuilder.AppendLine(" AND str_to_date(date_format(a.MBizDate,'%Y/%m/%d'),'%Y/%m/%d')<=@curMonthLastDay ");
			stringBuilder.AppendLine(" GROUP BY DATE_FORMAT(a.MBizDate,'%Y-%m')");
			stringBuilder.AppendLine(" ORDER BY MComingAmt DESC,MYearMonth");
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@contactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@PreMonthFirstDay", MySqlDbType.DateTime),
				new MySqlParameter("@curMonthLastDay", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = contactID;
			array[3].Value = dateTime4;
			array[4].Value = dateTime3;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			maxAmount = GetMaxAmount(ctx, stringBuilder, array);
			List<ChartColumnStacked2DModel> result = new List<ChartColumnStacked2DModel>();
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				result = DataTableToOverPastList(dataSet.Tables[0]);
			}
			return result;
		}

		private static List<ChartColumnStacked2DModel> DataTableToOverPastList(DataTable dt)
		{
			DateTime now = DateTime.Now;
			List<ChartColumnStacked2DModel> list = new List<ChartColumnStacked2DModel>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				List<string> list2 = new List<string>
				{
					"Money In",
					"Money Out"
				};
				foreach (string item in list2)
				{
					ChartColumnStacked2DModel chartColumnStacked2DModel = new ChartColumnStacked2DModel();
					double[] array = new double[12];
					chartColumnStacked2DModel.name = item;
					if (item == "Money In")
					{
						chartColumnStacked2DModel.color = "#00ff00";
					}
					else
					{
						chartColumnStacked2DModel.color = "#ff0000";
					}
					for (int i = 0; i < count; i++)
					{
						if (dt.Rows[i]["name"].ToString() == item)
						{
							string text = dt.Rows[i]["MYearMonth"].ToString();
							for (int j = 0; j < 12; j++)
							{
								string a = text;
								DateTime dateTime = now.AddMonths(j - 11);
								string str = dateTime.ToString("yyyy");
								dateTime = now.AddMonths(j - 11);
								if (a == str + "-" + dateTime.ToString("MM"))
								{
									array[j] = Convert.ToDouble(dt.Rows[i]["MComingAmt"]);
									break;
								}
							}
						}
					}
					chartColumnStacked2DModel.value = array;
					list.Add(chartColumnStacked2DModel);
				}
			}
			return list;
		}

		private static string[] GetOverPastLables()
		{
			string[] result = new string[0];
			switch (DateTime.Now.Month.ToString())
			{
			case "1":
				result = new string[12]
				{
					"Feb",
					"Mar",
					"Apr",
					"May",
					"Jun",
					"Jul",
					"Aug",
					"Sep",
					"Oct",
					"Nov",
					"Dec",
					"Jan"
				};
				break;
			case "2":
				result = new string[12]
				{
					"Mar",
					"Apr",
					"May",
					"Jun",
					"Jul",
					"Aug",
					"Sep",
					"Oct",
					"Nov",
					"Dec",
					"Jan",
					"Feb"
				};
				break;
			case "3":
				result = new string[12]
				{
					"Apr",
					"May",
					"Jun",
					"Jul",
					"Aug",
					"Sep",
					"Oct",
					"Nov",
					"Dec",
					"Jan",
					"Feb",
					"Mar"
				};
				break;
			case "4":
				result = new string[12]
				{
					"May",
					"Jun",
					"Jul",
					"Aug",
					"Sep",
					"Oct",
					"Nov",
					"Dec",
					"Jan",
					"Feb",
					"Mar",
					"Apr"
				};
				break;
			case "5":
				result = new string[12]
				{
					"Jun",
					"Jul",
					"Aug",
					"Sep",
					"Oct",
					"Nov",
					"Dec",
					"Jan",
					"Feb",
					"Mar",
					"Apr",
					"May"
				};
				break;
			case "6":
				result = new string[12]
				{
					"Jul",
					"Aug",
					"Sep",
					"Oct",
					"Nov",
					"Dec",
					"Jan",
					"Feb",
					"Mar",
					"Apr",
					"May",
					"Jun"
				};
				break;
			case "7":
				result = new string[12]
				{
					"Aug",
					"Sep",
					"Oct",
					"Nov",
					"Dec",
					"Jan",
					"Feb",
					"Mar",
					"Apr",
					"May",
					"Jun",
					"Jul"
				};
				break;
			case "8":
				result = new string[12]
				{
					"Sep",
					"Oct",
					"Nov",
					"Dec",
					"Jan",
					"Feb",
					"Mar",
					"Apr",
					"May",
					"Jun",
					"Jul",
					"Aug"
				};
				break;
			case "9":
				result = new string[12]
				{
					"Oct",
					"Nov",
					"Dec",
					"Jan",
					"Feb",
					"Mar",
					"Apr",
					"May",
					"Jun",
					"Jul",
					"Aug",
					"Sep"
				};
				break;
			case "10":
				result = new string[12]
				{
					"Nov",
					"Dec",
					"Jan",
					"Feb",
					"Mar",
					"Apr",
					"May",
					"Jun",
					"Jul",
					"Aug",
					"Sep",
					"Oct"
				};
				break;
			case "11":
				result = new string[12]
				{
					"Dec",
					"Jan",
					"Feb",
					"Mar",
					"Apr",
					"May",
					"Jun",
					"Jul",
					"Aug",
					"Sep",
					"Oct",
					"Nov"
				};
				break;
			case "12":
				result = new string[12]
				{
					"Jan",
					"Feb",
					"Mar",
					"Apr",
					"May",
					"Jun",
					"Jul",
					"Aug",
					"Sep",
					"Oct",
					"Nov",
					"Dec"
				};
				break;
			}
			return result;
		}

		private static void ResetInvoiceAmt(IVInvoiceModel model)
		{
			if (model != null)
			{
				model.MTotalAmtFor = model.MTotalAmtFor.ToRound(2);
				model.MTotalAmt = model.MTotalAmt.ToRound(2);
				model.MTaxTotalAmtFor = model.MTaxTotalAmtFor.ToRound(2);
				model.MTaxTotalAmt = model.MTaxTotalAmt.ToRound(2);
				if (model.MType == "Invoice_Purchase_Red" || model.MType == "Invoice_Sale_Red")
				{
					model.MTotalAmtFor = -model.MTotalAmtFor;
					model.MTotalAmt = -model.MTotalAmt;
					model.MTaxTotalAmtFor = -model.MTaxTotalAmtFor;
					model.MTaxTotalAmt = -model.MTaxTotalAmt;
				}
			}
		}

		private static void ResetInvoiceEntryAmt(string invoiceType, IVInvoiceEntryModel model)
		{
			if (model != null)
			{
				model.MTaxAmount = model.MTaxAmount.ToRound(2);
				model.MTaxAmountFor = model.MTaxAmountFor.ToRound(2);
				model.MAmount = model.MAmount.ToRound(2);
				model.MAmountFor = model.MAmountFor.ToRound(2);
				model.MTaxAmtFor = model.MTaxAmtFor.ToRound(2);
				model.MTaxAmt = model.MTaxAmt.ToRound(2);
				if (invoiceType == "Invoice_Purchase_Red" || invoiceType == "Invoice_Sale_Red")
				{
					model.MQty = -model.MQty;
					model.MTaxAmount = -model.MTaxAmount;
					model.MTaxAmountFor = -model.MTaxAmountFor;
					model.MAmount = -model.MAmount;
					model.MAmountFor = -model.MAmountFor;
					model.MTaxAmtFor = -model.MTaxAmtFor;
					model.MTaxAmt = -model.MTaxAmt;
				}
			}
		}

		private static void ResetCopyInvoiceAmt(MContext ctx, IVInvoiceModel model)
		{
			if (!(model.MVerificationAmt == decimal.Zero))
			{
				List<IVInvoiceEntryModel> invoiceEntry = model.InvoiceEntry;
				if (invoiceEntry != null || invoiceEntry.Count != 0)
				{
					List<REGTaxRateModel> source = new List<REGTaxRateModel>();
					if (model.MTaxID == "Tax_Exclusive" || model.MTaxID == "Tax_Inclusive")
					{
						REGTaxRateRepository rEGTaxRateRepository = new REGTaxRateRepository();
						source = rEGTaxRateRepository.GetList(ctx, null, false);
					}
					decimal num = Math.Abs(model.MTaxTotalAmtFor) - Math.Abs(model.MVerificationAmt);
					decimal d = Math.Abs(model.MTaxTotalAmtFor);
					decimal one = decimal.One;
					decimal num2 = num;
					for (int i = 0; i < invoiceEntry.Count; i++)
					{
						IVInvoiceEntryModel entryModel = invoiceEntry[i];
						one = (entryModel.MTaxAmountFor / d).ToRound(2);
						decimal d2 = Math.Round(num * one, 2);
						if (i == invoiceEntry.Count - 1)
						{
							d2 = num2;
						}
						REGTaxRateModel rEGTaxRateModel = (from t in source
						where t.MItemID == entryModel.MTaxID
						select t).FirstOrDefault();
						decimal d3 = (rEGTaxRateModel == null) ? decimal.Zero : (rEGTaxRateModel.MEffectiveTaxRate / 100m);
						if (model.MTaxID == "Tax_Exclusive")
						{
							entryModel.MQty = (d2 / ((decimal.One + d3) * entryModel.MPrice) / (decimal.One - entryModel.MDiscount / 100m) * 10000m).ToTruncate() / 10000m;
							entryModel.MTaxAmtFor = ((entryModel.MQty * entryModel.MPrice * (decimal.One - entryModel.MDiscount / 100m)).ToRound(2) * d3).ToRound(2);
						}
						else
						{
							entryModel.MQty = (d2 / entryModel.MPrice / (decimal.One - entryModel.MDiscount / 100m) * 10000m).ToTruncate() / 10000m;
							entryModel.MTaxAmtFor = ((entryModel.MQty * entryModel.MPrice * (decimal.One - entryModel.MDiscount / 100m)).ToRound(2) - (entryModel.MQty * entryModel.MPrice * (decimal.One - entryModel.MDiscount / 100m)).ToRound(2) / (decimal.One + d3)).ToRound(2);
						}
						num2 -= entryModel.MTaxAmountFor;
					}
					if (!(num2 == decimal.Zero))
					{
						IVInvoiceEntryModel lastEntryModel = invoiceEntry[invoiceEntry.Count - 1];
						string mTaxID = lastEntryModel.MTaxID;
						decimal num3 = num2;
						if (model.MTaxID == "Tax_Exclusive")
						{
							decimal d4 = default(decimal);
							REGTaxRateModel rEGTaxRateModel2 = (from t in source
							where t.MItemID == lastEntryModel.MTaxID
							select t).FirstOrDefault();
							if (rEGTaxRateModel2 != null)
							{
								d4 = rEGTaxRateModel2.MEffectiveTaxRate / 100m;
								mTaxID = rEGTaxRateModel2.MItemID;
							}
							num3 = (num2 / (decimal.One + d4)).ToRound(2);
							if ((num3 * (decimal.One + d4)).ToRound(2) != num2)
							{
								rEGTaxRateModel2 = (from t in source
								where t.MEffectiveTaxRate == decimal.Zero
								select t).FirstOrDefault();
								mTaxID = ((rEGTaxRateModel2 == null) ? "" : rEGTaxRateModel2.MItemID);
								num3 = num2;
							}
						}
						List<IVInvoiceEntryModel> invoiceEntry2 = model.InvoiceEntry;
						IVInvoiceEntryModel iVInvoiceEntryModel = new IVInvoiceEntryModel();
						iVInvoiceEntryModel.MItemID = lastEntryModel.MItemID;
						iVInvoiceEntryModel.MDesc = lastEntryModel.MDesc;
						iVInvoiceEntryModel.MQty = decimal.One;
						iVInvoiceEntryModel.MPrice = num3;
						iVInvoiceEntryModel.MAmountFor = num3;
						iVInvoiceEntryModel.MTaxID = mTaxID;
						iVInvoiceEntryModel.MTaxAmtFor = decimal.Zero;
						iVInvoiceEntryModel.MTrackItem1 = lastEntryModel.MTrackItem1;
						iVInvoiceEntryModel.MTrackItem2 = lastEntryModel.MTrackItem2;
						iVInvoiceEntryModel.MTrackItem3 = lastEntryModel.MTrackItem3;
						iVInvoiceEntryModel.MTrackItem4 = lastEntryModel.MTrackItem4;
						iVInvoiceEntryModel.MTrackItem5 = lastEntryModel.MTrackItem5;
						invoiceEntry2.Add(iVInvoiceEntryModel);
						model.InvoiceEntry = invoiceEntry2;
					}
				}
			}
		}

		private static void set()
		{
		}

		private static decimal GetTaxRateById(MContext ctx, string taxId)
		{
			REGTaxRateModel dataEditModel = ModelInfoManager.GetDataEditModel<REGTaxRateModel>(ctx, taxId, false, true);
			return dataEditModel?.MEffectiveTaxRateDecimal ?? decimal.Zero;
		}

		public static List<IVInvoiceSendModel> GetSendInvoiceList(MContext ctx, IVInvoiceSendParam param)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (param.SendType == EmailSendTypeEnum.Invoice)
			{
				stringBuilder.AppendFormat("select t1.MID,t1.MNumber as MInvNumber,t1.MContactID,t1.MIsSent, convert(AES_DECRYPT(t3.MName,'{0}') using utf8)  as MContactName,t3.MFirstName,t3.MLastName,convert(AES_DECRYPT(t2.MEmail,'{0}') using utf8) as MContactEmail,t2.MNetWorkKey as MContactNetwork ", "JieNor-001");
				stringBuilder.AppendLine(" from T_IV_Invoice t1");
				stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts t2 ON t1.MContactID=t2.MItemID  and t2.MIsDelete=0 ");
				stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts_l t3 ON t2.MItemID=t3.MParentID AND t3.MLocaleID=@MLocaleID  and t3.MIsDelete=0 ");
				stringBuilder.AppendLine(" where t1.MIsDelete=0 AND t2.MIsDelete=0 AND t1.MID in (" + param.KeyIDSWithSingleQuote + ")  ");
			}
			else if (param.SendType == EmailSendTypeEnum.Statement)
			{
				stringBuilder.AppendFormat("select t1.MItemID as MContactID,convert(AES_DECRYPT(t2.MName,'{0}') using utf8) as MContactName,t2.MFirstName,t2.MLastName,convert(AES_DECRYPT(t1.MEmail,'{0}') using utf8) as MContactEmail", "JieNor-001");
				stringBuilder.AppendLine(" from T_BD_Contacts t1 ");
				stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts_l t2 ON t2.MOrgID = t1.MOrgID and  t1.MItemID=t2.MParentID AND t2.MLocaleID=@MLocaleID and t2.MIsDelete=0 ");
				stringBuilder.AppendLine(" where t1.MIsDelete=0 AND t1.MItemID in (" + param.KeyIDSWithSingleQuote + ") and t1.MOrgID = @MOrgID  ");
			}
			else if (param.SendType == EmailSendTypeEnum.RepeatingInvoice)
			{
				if (!string.IsNullOrWhiteSpace(param.KeyIDSWithSingleQuote))
				{
					stringBuilder.AppendFormat("select t1.MID,ifnull(t1.MTaxTotalAmtFor,0) as MAmount,t1.MContactID,t1.MIsMarkAsSent as MIsSent,t1.MIsIncludePDFAttachment,t1.MIsSendMeACopy, convert(AES_DECRYPT(t3.MName,'{0}') using utf8)  as MContactName,t3.MFirstName,t3.MLastName,convert(AES_DECRYPT(t2.MEmail,'{0}') using utf8) as MContactEmail,t2.MNetWorkKey as MContactNetwork ", "JieNor-001");
					stringBuilder.AppendLine(" from T_IV_RepeatInvoice t1");
					stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts t2 ON t1.MOrgID = t2.MOrgID and  t1.MContactID=t2.MItemID  and t2.MIsDelete=0 ");
					stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts_l t3 ON t3.MOrgID = t1.MOrgID and t2.MItemID=t3.MParentID AND t3.MLocaleID=@MLocaleID  and t3.MIsDelete=0 ");
					stringBuilder.AppendLine(" where t1.MIsDelete=0 AND t2.MIsDelete=0 AND t1.MID in (" + param.KeyIDSWithSingleQuote + ")  and t1.MOrgID = @MOrgID ");
				}
				else
				{
					stringBuilder.AppendFormat(" select '' as MID,0 as MAmount,t2.MItemID as MContactID,1 as MIsSent,0 as MIsIncludePDFAttachment,0 as MIsSendMeACopy,convert(AES_DECRYPT(t3.MName,'{0}') using utf8) as MContactName,t3.MFirstName,t3.MLastName,convert(AES_DECRYPT(t2.MEmail,'{0}') using utf8) as MContactEmail,t2.MNetWorkKey as MContactNetwork ", "JieNor-001");
					stringBuilder.AppendLine(" from T_BD_Contacts t2 ");
					stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts_l t3 ON t2.MOrgID = t3.MOrgID and  t2.MItemID=t3.MParentID AND t3.MLocaleID=@MLocaleID  and t3.MIsDelete=0 ");
					stringBuilder.AppendLine(" where t2.MIsDelete=0 AND t2.MItemID=@MContactID and t2.MOrgID = @MOrgID ");
				}
			}
			else if (param.SendType == EmailSendTypeEnum.Payslip)
			{
				stringBuilder.AppendLine("select t2.MID,t1.MNumber as MInvNumber,t2.MEmployeeID as MContactID,t2.MIsSent,t3.MEmail as MContactEmail");
				stringBuilder.AppendLine(" ,(case @MLocaleID when '0x0009' then concat(t4.MFirstName, ' ', t4.MLastName) else concat(t4.MLastName, t4.MFirstName) end) as MContactName");
				stringBuilder.AppendLine(" from T_PA_PayRun t1");
				stringBuilder.AppendLine(" INNER JOIN T_PA_SalaryPayment t2 ON t2.MOrgID = t1.MOrgID and  t1.MID=t2.MRunID  and t2.MIsDelete=0 ");
				stringBuilder.AppendLine(" INNER JOIN T_BD_Employees t3 ON t3.MOrgID = t1.MOrgID and  t2.MEmployeeID=t3.MItemID and t3.MIsDelete=0 ");
				stringBuilder.AppendLine(" INNER JOIN T_BD_Employees_l t4 ON t4.MOrgID = t1.MOrgID and  t3.MItemID=t4.MParentID AND t4.MLocaleID=@MLocaleID and t4.MIsDelete=0 ");
				stringBuilder.AppendLine(" where  t1.MIsDelete=0 AND t2.MID in (" + param.KeyIDSWithSingleQuote + ") and t1.MOrgID = @MOrgID");
				stringBuilder.AppendLine(" order by t2.MSeq, MContactName");
			}
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = param.ContactID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			List<IVInvoiceSendModel> list = ModelInfoManager.DataTableToList<IVInvoiceSendModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
			foreach (IVInvoiceSendModel item in list)
			{
				item.MContactPrimaryPerson = MConverter.ToUserName(ctx.MLCID, item.MFirstName, item.MLastName);
			}
			return list;
		}

		public static OperationResult SendInvoiceList(MContext ctx, IVInvoiceEmailSendModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<MailHelper> list = new List<MailHelper>();
			List<CommandInfo> list2 = new List<CommandInfo>();
			foreach (IVInvoiceSendModel sendEntry in model.SendEntryList)
			{
				MailHelper mailHelper = SendInvMessage(ctx, model, sendEntry);
				if (mailHelper != null)
				{
					list.Add(mailHelper);
				}
				if (model.MSendType == EmailSendTypeEnum.Invoice && sendEntry.MIsSent)
				{
					IVInvoiceModel dataEditModel = ModelInfoManager.GetDataEditModel<IVInvoiceModel>(ctx, sendEntry.MID, false, true);
					dataEditModel.MIsSent = true;
					list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVInvoiceModel>(ctx, dataEditModel, new List<string>
					{
						"MIsSent"
					}, true));
				}
				if (model.MSendType == EmailSendTypeEnum.Payslip && sendEntry.MIsSent)
				{
					PASalaryPaymentModel dataEditModel2 = ModelInfoManager.GetDataEditModel<PASalaryPaymentModel>(ctx, sendEntry.MID, false, true);
					dataEditModel2.MIsSent = true;
					list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PASalaryPaymentModel>(ctx, dataEditModel2, new List<string>
					{
						"MIsSent"
					}, true));
				}
				if (string.IsNullOrWhiteSpace(sendEntry.MOriginalContactEmail))
				{
					if (model.MSendType == EmailSendTypeEnum.Payslip)
					{
						BDEmployeesModel dataEditModel3 = ModelInfoManager.GetDataEditModel<BDEmployeesModel>(ctx, sendEntry.MContactID, false, true);
						dataEditModel3.MEmail = GetEmailToList(sendEntry.MContactEmail).FirstOrDefault();
						list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDEmployeesModel>(ctx, dataEditModel3, new List<string>
						{
							"MEmail"
						}, true));
					}
					else
					{
						BDContactsInfoModel dataEditModel4 = ModelInfoManager.GetDataEditModel<BDContactsInfoModel>(ctx, sendEntry.MContactID, false, true);
						dataEditModel4.MEmail = GetEmailToList(sendEntry.MContactEmail).FirstOrDefault();
						list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsInfoModel>(ctx, dataEditModel4, new List<string>
						{
							"MEmail"
						}, true));
					}
				}
			}
			SendMail.SendSMTPEMail(list);
			if (list2.Count == 0)
			{
				return operationResult;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list2);
			if (num < 0)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "SendInvoiceEmailError", "Send Invoice Email Error.")
				});
			}
			return operationResult;
		}

		public static List<IVInvoiceModel> GetModelList(MContext ctx, List<string> keyIds)
		{
			string text = "";
			foreach (string keyId in keyIds)
			{
				text += $"'{keyId}',";
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(0, text.Length - 1);
			}
			string text2 = $"select * from t_iv_invoice where MOrgID='{ctx.MOrgID}' AND MIsDelete=0 and MID in ({text})";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<IVInvoiceModel>(dynamicDbHelperMySQL.Query(text2.ToString()).Tables[0]);
		}

		public static List<CommandInfo> GetNewInvoiceCmdList(MContext ctx, List<IVInvoiceModel> modelList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string value = "INSERT INTO t_iv_invoice(MID,MOrgID,MType,MContactID,MNumber,MBizDate,MDueDate,MExpectedDate,MAttachCount,MTaxID,MCyID,MExchangeRate,MOToLRate,MTotalAmtFor,MLToORate,MTotalAmt,MTaxTotalAmtFor,MTaxTotalAmt,MVerificationAmt,MStatus,MReference,MDesc,MIsDelete,MCreatorID,MCreateDate,MModifierID,MModifyDate)";
			string format = " SELECT @MID{0},@MOrgID{0},@MType{0},@MContactID{0},@MNumber{0},@MBizDate{0},@MDueDate{0},@MExpectedDate{0},@MAttachCount{0},@MTaxID{0},@MCyID{0},@MExchangeRate{0},@MOToLRate{0},@MLToORate{0},@MTotalAmtFor{0},@MTotalAmt{0},@MTaxTotalAmtFor{0},@MTaxTotalAmt{0},@MVerificationAmt{0},@MStatus{0},@MReference{0},@MDesc{0},0,@MCreatorID{0},@MCreateDate{0},@MModifierID{0},@MModifyDate{0}";
			string value2 = "INSERT INTO t_iv_invoiceentry(MEntryID,MID,MSeq,MItemID,MAcctID,MTaxID,MTrackItem1,MTrackItem2,MTrackItem3,MTrackItem4,MTrackItem5,MQty,MPrice,MDiscount,MAmountFor,MAmount,MTaxAmountFor,MTaxAmount,MTaxAmtFor,MTaxAmt,MDesc,MOrgID,MIsDelete,MCreatorID,MCreateDate,MModifierID,MModifyDate)";
			string format2 = "SELECT @MEntryID{0},@MID{0},@MSeq{0},@MItemID{0},@MAcctID{0},@MTaxID{0},@MTrackItem1{0},@MTrackItem2{0},@MTrackItem3{0},@MTrackItem4{0},@MTrackItem5{0},@MQty{0},@MPrice{0},@MDiscount{0},@MAmountFor{0},@MAmount{0},@MTaxAmountFor{0},@MTaxAmount{0},@MTaxAmtFor{0},@MTaxAmt{0},@MDesc{0},@MOrgID{0},0,@MCreatorID{0},@MCreateDate{0},@MModifierID{0},@MModifyDate{0}";
			StringBuilder stringBuilder = new StringBuilder(1000);
			StringBuilder stringBuilder2 = new StringBuilder(1000);
			stringBuilder.Append(value);
			stringBuilder2.Append(value2);
			int num = 0;
			int num2 = 0;
			List<MySqlParameter> list2 = new List<MySqlParameter>();
			List<MySqlParameter> list3 = new List<MySqlParameter>();
			foreach (IVInvoiceModel model in modelList)
			{
				if (num > 0)
				{
					stringBuilder.Append(" UNION ALL ");
				}
				stringBuilder.AppendFormat(format, num);
				list2.AddRange(GetNewInvoiceParams(ctx, num, model));
				foreach (IVInvoiceEntryModel item in model.InvoiceEntry)
				{
					if (num2 > 0)
					{
						stringBuilder2.Append(" UNION ALL ");
					}
					stringBuilder2.AppendFormat(format2, num2);
					list3.AddRange(GetNewInvoiceEntryParams(ctx, num2, model, item));
					num2++;
				}
				num++;
			}
			List<CommandInfo> list4 = list;
			CommandInfo obj = new CommandInfo
			{
				CommandText = stringBuilder.ToString()
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			list4.Add(obj);
			List<CommandInfo> list5 = list;
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = stringBuilder2.ToString()
			};
			array = (obj2.Parameters = list3.ToArray());
			list5.Add(obj2);
			return list;
		}

		private static List<MySqlParameter> GetNewInvoiceParams(MContext ctx, int i, IVInvoiceModel model)
		{
			return new List<MySqlParameter>
			{
				new MySqlParameter($"@MID{i}", model.MID),
				new MySqlParameter($"@MOrgID{i}", ctx.MOrgID),
				new MySqlParameter($"@MType{i}", model.MType),
				new MySqlParameter($"@MContactID{i}", model.MContactID),
				new MySqlParameter($"@MNumber{i}", model.MNumber),
				new MySqlParameter($"@MBizDate{i}", model.MBizDate),
				new MySqlParameter($"@MDueDate{i}", model.MDueDate),
				new MySqlParameter($"@MExpectedDate{i}", model.MExpectedDate),
				new MySqlParameter($"@MOToLRate{i}", model.MOToLRate),
				new MySqlParameter($"@MLToORate{i}", model.MLToORate),
				new MySqlParameter($"@MAttachCount{i}", model.MAttachCount),
				new MySqlParameter($"@MTaxID{i}", model.MTaxID),
				new MySqlParameter($"@MCyID{i}", model.MCyID),
				new MySqlParameter($"@MExchangeRate{i}", model.MExchangeRate),
				new MySqlParameter($"@MTotalAmtFor{i}", model.MTotalAmtFor),
				new MySqlParameter($"@MTotalAmt{i}", model.MTotalAmt),
				new MySqlParameter($"@MTaxTotalAmtFor{i}", model.MTaxTotalAmtFor),
				new MySqlParameter($"@MTaxTotalAmt{i}", model.MTaxTotalAmt),
				new MySqlParameter($"@MVerificationAmt{i}", model.MVerificationAmt),
				new MySqlParameter($"@MStatus{i}", model.MStatus),
				new MySqlParameter($"@MReference{i}", model.MReference),
				new MySqlParameter($"@MDesc{i}", model.MDesc),
				new MySqlParameter($"@MIsDelete{i}", MySqlDbType.Decimal),
				new MySqlParameter($"@MCreatorID{i}", ctx.MUserID),
				new MySqlParameter($"@MCreateDate{i}", model.MCreateDate),
				new MySqlParameter($"@MModifierID{i}", ctx.MUserID),
				new MySqlParameter($"@MModifyDate{i}", ctx.DateNow)
			};
		}

		private static List<MySqlParameter> GetNewInvoiceEntryParams(MContext ctx, int i, IVInvoiceModel model, IVInvoiceEntryModel entry)
		{
			return new List<MySqlParameter>
			{
				new MySqlParameter($"@MEntryID{i}", entry.MEntryID),
				new MySqlParameter($"@MID{i}", model.MID),
				new MySqlParameter($"@MSeq{i}", entry.MSeq),
				new MySqlParameter($"@MItemID{i}", entry.MItemID),
				new MySqlParameter($"@MAcctID{i}", entry.MAcctID),
				new MySqlParameter($"@MTaxID{i}", entry.MTaxID),
				new MySqlParameter($"@MTrackItem1{i}", entry.MTrackItem1),
				new MySqlParameter($"@MTrackItem2{i}", entry.MTrackItem2),
				new MySqlParameter($"@MTrackItem3{i}", entry.MTrackItem3),
				new MySqlParameter($"@MTrackItem4{i}", entry.MTrackItem4),
				new MySqlParameter($"@MTrackItem5{i}", entry.MTrackItem5),
				new MySqlParameter($"@MQty{i}", entry.MQty),
				new MySqlParameter($"@MPrice{i}", entry.MPrice),
				new MySqlParameter($"@MDiscount{i}", entry.MDiscount),
				new MySqlParameter($"@MAmountFor{i}", entry.MAmountFor),
				new MySqlParameter($"@MAmount{i}", entry.MAmount),
				new MySqlParameter($"@MTaxAmountFor{i}", entry.MTaxAmountFor),
				new MySqlParameter($"@MTaxAmount{i}", entry.MTaxAmount),
				new MySqlParameter($"@MTaxAmtFor{i}", entry.MTaxAmtFor),
				new MySqlParameter($"@MTaxAmt{i}", entry.MTaxAmt),
				new MySqlParameter($"@MDesc{i}", entry.MDesc),
				new MySqlParameter($"@MOrgID{i}", ctx.MOrgID),
				new MySqlParameter($"@MIsDelete{i}", MySqlDbType.Decimal),
				new MySqlParameter($"@MCreatorID{i}", ctx.MUserID),
				new MySqlParameter($"@MCreateDate{i}", model.MCreateDate),
				new MySqlParameter($"@MModifierID{i}", ctx.MUserID),
				new MySqlParameter($"@MModifyDate{i}", ctx.DateNow)
			};
		}

		private static string GetEmailSubject(MContext ctx, string template, IVInvoiceSendModel entryModel)
		{
			switch (template)
			{
			case "1":
				return string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "FormatBasicMessageSubject", "Invoice {0} from {1} for {2}"), entryModel.MInvNumber, ctx.MOrgName, entryModel.MContactName);
			case "2":
				return string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "FormatOverdueMessageSubject", "Overdue invoice reminder from {0}"), ctx.MOrgName);
			case "3":
				return string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "FormatStatementMessageSubject", "Statement from {0} for {1}"), entryModel.MInvNumber, ctx.MOrgName, entryModel.MContactName);
			case "5":
				return string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "FormatPayslipMessageSubject", "Payslip for {0} ({1})"), entryModel.MContactName, entryModel.MInvNumber);
			default:
				return string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "FormatBasicMessageSubject", "Invoice {0} from {1} for {2}"), entryModel.MInvNumber, ctx.MOrgName, entryModel.MContactName);
			}
		}

		public static string PreviewEmailTmpl(MContext ctx, IVInvoiceEmailSendModel model, IVInvoiceSendModel entryModel, string tmpl = null, EmailPreviewTypeEnum previewType = EmailPreviewTypeEnum.None, bool isFromSendEmail = false)
		{
			string text = string.Empty;
			if (string.IsNullOrWhiteSpace(tmpl))
			{
				tmpl = ((previewType == EmailPreviewTypeEnum.Subject) ? model.MSubject : model.MContent);
			}
			IVInvoiceModel invoiceModel = GetInvoiceModel(ctx, entryModel.MID);
			if (invoiceModel != null)
			{
				decimal num = invoiceModel.MTaxTotalAmtFor;
				string newValue = num.ToString("f2") + " " + invoiceModel.MCyID;
				num = invoiceModel.MTaxTotalAmtFor - invoiceModel.MVerificationAmt;
				string newValue2 = num.ToString("f2") + " " + invoiceModel.MCyID;
				DateTime mDueDate = invoiceModel.MDueDate;
				string newValue3 = (invoiceModel.MDueDate == DateTime.MinValue) ? string.Empty : (" " + invoiceModel.MDueDate.ToString("yyyy-MM-dd"));
				text = tmpl.Replace("[Invoice Number]", invoiceModel.MNumber).Replace("[Invoice Total]", newValue).Replace("[Amount Due]", newValue2)
					.Replace("[Due Date]", newValue3);
			}
			text = ((!string.IsNullOrWhiteSpace(text)) ? text : tmpl);
			string text2 = ctx.MOrgName;
			if (!(previewType == EmailPreviewTypeEnum.Subject & isFromSendEmail))
			{
				entryModel.MContactName = MText.Encode(entryModel.MContactName);
				entryModel.MContactPrimaryPerson = MText.Encode(entryModel.MContactPrimaryPerson);
				text2 = MText.Encode(text2);
			}
			text = text.Replace("[Contact Name]", entryModel.MContactName).Replace("[Employee Name]", entryModel.MContactName).Replace("[Contact Primary Person]", entryModel.MContactPrimaryPerson)
				.Replace("[Organization Name]", text2)
				.Replace("[Salary Period]", model.MSalaryPeriod);
			if (model.MSendType == EmailSendTypeEnum.Statement)
			{
				string newValue4 = string.IsNullOrWhiteSpace(model.MBeginDate) ? (COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "asat", "as at") + model.MEndDate) : string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ForThePeriodFromTo", "for the period{0}to{1}"), model.MBeginDate, model.MEndDate);
				text = text.Replace("[Statement Date]", newValue4).Replace("[Statement Begin Date]", model.MBeginDate).Replace("[Statement End Date]", model.MEndDate)
					.Replace("[Statement Date]", newValue4);
			}
			return text;
		}

		private static string FormatBasicMessage(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Hi {0}, <br/><br/>");
			stringBuilder.AppendLine("Here's invoice {1} for {2}.<br/><br/>");
			stringBuilder.AppendLine("The amount outstanding of {3} is due on{4}.<br/><br/>");
			stringBuilder.AppendLine("From your online bill you can print a PDF, export a CSV, or create a free login and view your outstanding bills.<br/><br/>");
			stringBuilder.AppendLine("If you have any questions, please let us know.<br/><br/>");
			stringBuilder.AppendLine("Thanks,<br/>");
			stringBuilder.AppendLine("{5}");
			return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "FormatBasicMessageBody", stringBuilder.ToString());
		}

		private static string FormatOverdueMessage(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Hello<br/><br/>");
			stringBuilder.AppendLine("Invoice {0} to {1} now has {2} overdue.<br/><br/>");
			stringBuilder.AppendLine("The invoice was due payment by{3} and we would really appreciate your prompt attention to this.<br/>");
			stringBuilder.AppendLine("Please get in touch if you'd like to discuss payment options or to query the invoice.<br/><br/>");
			stringBuilder.AppendLine("To make things quicker for you, you can view and pay this invoice online: [Online Invoice Link]<br/><br/>");
			stringBuilder.AppendLine("Thanks and we look forward to hearing from you,<br/>");
			stringBuilder.AppendLine("{4}");
			return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "FormatOverdueMessageBody", stringBuilder.ToString());
		}

		private static string FormatStatementMessage(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Hi {0}, <br/><br/>");
			stringBuilder.AppendLine("Here's your statement {1}.<br/><br/>");
			stringBuilder.AppendLine("If you have any questions, please let us know.<br/><br/>");
			stringBuilder.AppendLine("Thanks,<br/>");
			stringBuilder.AppendLine("{2}");
			return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "FormatStatementMessageBody", stringBuilder.ToString());
		}

		private static MailHelper SendInvMessage(MContext ctx, IVInvoiceEmailSendModel model, IVInvoiceSendModel item)
		{
			List<string> emailToList = GetEmailToList(item.MContactEmail);
			string strSubject = PreviewEmailTmpl(ctx, model, item, model.MSubject, EmailPreviewTypeEnum.Subject, true);
			string strBody = PreviewEmailTmpl(ctx, model, item, model.MContent, EmailPreviewTypeEnum.Content, false);
			string filePath = model.MIncludePDF ? item.MFilePath : string.Empty;
			if (model.MSendMeACopy)
			{
				emailToList.Add(model.MReplyEmail);
			}
			return SendMail.GetSendMailHelper(emailToList, strSubject, strBody, model.MFromUserName, filePath);
		}

		private static List<string> GetEmailToList(string emailStr)
		{
			List<string> list = new List<string>();
			if (string.IsNullOrWhiteSpace(emailStr))
			{
				return list;
			}
			List<string> list2 = emailStr.Split(',').ToList();
			foreach (string item in list2)
			{
				list.AddRange(item.Split(';'));
			}
			return list;
		}

		public static List<CommandInfo> UpdateCurrentAccount(MContext ctx, string oldCode, string newCode)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@oldCode", MySqlDbType.VarChar, 36)
				{
					Value = oldCode
				},
				new MySqlParameter("@newCode", MySqlDbType.VarChar, 36)
				{
					Value = newCode
				}
			};
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_iv_invoice set MCurrentAccountCode=@newCode where MCurrentAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete=0";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}

		public static IVGoldenTaxInvoiceModel GetGoldenTaxInvoiceEditModel(MContext ctx, string pkID)
		{
			IVGoldenTaxInvoiceModel iVGoldenTaxInvoiceModel = ModelInfoManager.GetDataEditModel<IVGoldenTaxInvoiceModel>(ctx, pkID, false, true);
			if (iVGoldenTaxInvoiceModel == null)
			{
				iVGoldenTaxInvoiceModel = new IVGoldenTaxInvoiceModel();
				iVGoldenTaxInvoiceModel.IsNew = true;
			}
			return iVGoldenTaxInvoiceModel;
		}

		public static OperationResult UpdateGoldenTaxInvoice(MContext ctx, IVGoldenTaxInvoiceModel model)
		{
			OperationResult operationResult = ModelInfoManager.InsertOrUpdate<IVGoldenTaxInvoiceModel>(ctx, model, null);
			if (operationResult.Success)
			{
				IVGoldenTaxInvoiceLogRepository.AddGoldenTaxInvoiceEditLog(ctx, model);
			}
			return operationResult;
		}

		public static OperationResult UpdateGoldenTaxCourierInfo(MContext ctx, IVGoldenTaxInvoiceModel model)
		{
			OperationResult operationResult = ModelInfoManager.InsertOrUpdate<IVGoldenTaxInvoiceModel>(ctx, model, new List<string>
			{
				"MCourierCompany",
				"MCourierNumber",
				"MCourierDate"
			});
			if (operationResult.Success)
			{
				IVGoldenTaxInvoiceLogRepository.AddGoldenTaxInvoiceUpdateExpressInfoLog(ctx, model);
			}
			return operationResult;
		}

		public static OperationResult DeleteGoldenTaxInvoiceList(MContext ctx, ParamBase param)
		{
			List<string> pkIDS = param.KeyIDs.Split(',').ToList();
			OperationResult operationResult = ModelInfoManager.DeleteFlag<IVGoldenTaxInvoiceModel>(ctx, pkIDS);
			if (operationResult.Success)
			{
				IVGoldenTaxInvoiceLogRepository.AddGoldenTaxInvoiceDeleteLog(ctx, param);
			}
			return operationResult;
		}

		public static OperationResult ArchiveGoldenTaxInvoiceList(MContext ctx, ParamBase param)
		{
			List<string> list = param.KeyIDs.Split(',').ToList();
			List<CommandInfo> list2 = new List<CommandInfo>();
			foreach (string item in list)
			{
				list2.AddRange(ModelInfoManager.GetArchiveFlagCmd<IVGoldenTaxInvoiceModel>(ctx, item));
			}
			int num = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list2);
			return new OperationResult
			{
				Success = (num > 0)
			};
		}

		public static OperationResult UpdateGoldenTaxInvoicePrintStatusList(MContext ctx, ParamBase param, bool isPrint)
		{
			string sql = $"update T_IV_GoldenTaxInvoice set MIsPrint = @MIsPrint where MID in ({param.KeyIDSWithSingleQuote})";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MIsPrint", MySqlDbType.Bit)
				{
					Value = (object)isPrint
				}
			};
			int num = new DynamicDbHelperMySQL(ctx).ExecuteSql(sql, cmdParms);
			if (num > 0)
			{
				IVGoldenTaxInvoiceLogRepository.AddGoldenTaxInvoiceUpdatePrintStatusLog(ctx, param);
			}
			return new OperationResult
			{
				Success = (num > 0)
			};
		}

		public static OperationResult AddGoldenTaxInvoiceNoteLog(MContext ctx, IVGoldenTaxInvoiceModel model)
		{
			IVGoldenTaxInvoiceLogRepository.AddGoldenTaxInvoiceNoteLog(ctx, model);
			return new OperationResult
			{
				Success = true
			};
		}

		public static List<IVGoldenTaxInvoiceListModel> GetGoldenTaxInvoiceList(MContext ctx, IVGoldenTaxInvoiceListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" select a.*,group_concat(DISTINCT b.MAttachID) as MAttachIDs from T_IV_GoldenTaxInvoice a");
			stringBuilder.Append(" left join T_IV_GoldenTaxInvoiceAttachment b on a.MID=b.MParentID");
			stringBuilder.Append(" where a.MIsDelete = 0 and a.MIsActive = 1 and a.MOrgID=@MOrgID and a.MInvoiceSource=@MInvoiceSource and a.MInvoiceType=@MInvoiceType ");
			stringBuilder.Append(GetGoldenTaxInvoiceListFilterSql(ctx, filter));
			stringBuilder.Append(" group by a.MID ");
			if (string.IsNullOrEmpty(filter.Sort))
			{
				stringBuilder.Append(" order by a.MCreateDate desc");
			}
			else
			{
				stringBuilder.AppendFormat(" order by a.{0} {1}", filter.Sort, filter.Order);
			}
			stringBuilder.Append(filter.PageSqlString);
			MySqlParameter[] goldenTaxInvoiceListParameters = GetGoldenTaxInvoiceListParameters(ctx, filter);
			DataTable dt = new DynamicDbHelperMySQL(ctx).Query(stringBuilder.ToString(), goldenTaxInvoiceListParameters).Tables[0];
			return ModelInfoManager.DataTableToList<IVGoldenTaxInvoiceListModel>(dt);
		}

		public static int GetGoldenTaxInvoiceTotalCount(MContext ctx, IVGoldenTaxInvoiceListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" select count(1) from T_IV_GoldenTaxInvoice a");
			stringBuilder.Append(" where a.MIsDelete = 0 and a.MIsActive = 1 and a.MOrgID=@MOrgID and a.MInvoiceSource=@MInvoiceSource and a.MInvoiceType=@MInvoiceType ");
			stringBuilder.Append(GetGoldenTaxInvoiceListFilterSql(ctx, filter));
			MySqlParameter[] goldenTaxInvoiceListParameters = GetGoldenTaxInvoiceListParameters(ctx, filter);
			object single = new DynamicDbHelperMySQL(ctx).GetSingle(stringBuilder.ToString(), goldenTaxInvoiceListParameters);
			return Convert.ToInt32(single);
		}

		private static string GetGoldenTaxInvoiceListFilterSql(MContext ctx, IVGoldenTaxInvoiceListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				stringBuilder.Append(" and (a.MNumber like concat('%',@MKeyword,'%') or a.MReference like concat('%',@MKeyword,'%')) ");
			}
			DateTime value = new DateTime(1900, 1, 1);
			switch (filter.MSearchWithin)
			{
			case IVInvoiceSearchWithinEnum.AnyDate:
				if (filter.MStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" and a.MOpenDate >= @MStartDate ");
				}
				if (filter.MEndDate > (DateTime?)value)
				{
					stringBuilder.Append(" and a.MOpenDate < @MEndDate ");
				}
				break;
			case IVInvoiceSearchWithinEnum.OpenDate:
				if (filter.MStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" and a.MOpenDate >= @MStartDate ");
				}
				if (filter.MEndDate > (DateTime?)value)
				{
					stringBuilder.Append(" and a.MOpenDate < @MEndDate ");
				}
				break;
			}
			return stringBuilder.ToString();
		}

		private static MySqlParameter[] GetGoldenTaxInvoiceListParameters(MContext ctx, IVGoldenTaxInvoiceListFilterModel filter)
		{
			return new MySqlParameter[6]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@MInvoiceSource", MySqlDbType.Int32, 4)
				{
					Value = (object)filter.MInvoiceSource
				},
				new MySqlParameter("@MInvoiceType", MySqlDbType.Int32, 4)
				{
					Value = (object)filter.MInvoiceType
				},
				new MySqlParameter("@MKeyword", MySqlDbType.VarChar, 500)
				{
					Value = filter.Keyword
				},
				new MySqlParameter("@MStartDate", MySqlDbType.DateTime)
				{
					Value = (object)filter.MStartDate
				},
				new MySqlParameter("@MEndDate", MySqlDbType.DateTime)
				{
					Value = (object)filter.MEndDate
				}
			};
		}

		public static OperationResult UpdateRepeatInvoiceMessage(MContext ctx, IVInvoiceEmailSendModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (IVInvoiceSendModel sendEntry in model.SendEntryList)
			{
				if (!string.IsNullOrWhiteSpace(sendEntry.MID))
				{
					IVRepeatInvoiceModel iVRepeatInvoiceModel = new IVRepeatInvoiceModel();
					iVRepeatInvoiceModel.MID = sendEntry.MID;
					iVRepeatInvoiceModel.MStatus = Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment);
					iVRepeatInvoiceModel.MIsMarkAsSent = sendEntry.MIsSent;
					iVRepeatInvoiceModel.MIsIncludePDFAttachment = model.MIncludePDF;
					iVRepeatInvoiceModel.MIsSendMeACopy = model.MSendMeACopy;
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVRepeatInvoiceModel>(ctx, iVRepeatInvoiceModel, new List<string>
					{
						"MStatus",
						"MIsMarkAsSent",
						"MIsIncludePDFAttachment",
						"MIsSendMeACopy"
					}, true));
				}
				if (string.IsNullOrWhiteSpace(sendEntry.MOriginalContactEmail))
				{
					BDContactsInfoModel dataEditModel = ModelInfoManager.GetDataEditModel<BDContactsInfoModel>(ctx, sendEntry.MContactID, false, true);
					dataEditModel.MEmail = GetEmailToList(sendEntry.MContactEmail).FirstOrDefault();
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsInfoModel>(ctx, dataEditModel, new List<string>
					{
						"MEmail"
					}, true));
				}
			}
			if (list.Count == 0)
			{
				return operationResult;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num < 0)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "EditInvoiceMessageError", "Edit InvoiceMessage Error.")
				});
			}
			return operationResult;
		}

		public static IVRepeatInvoiceModel GetRepeatInvoiceEditModel(MContext ctx, string pkID)
		{
			IVRepeatInvoiceModel iVRepeatInvoiceModel = ModelInfoManager.GetDataEditModel<IVRepeatInvoiceModel>(ctx, pkID, false, true);
			if (iVRepeatInvoiceModel == null)
			{
				iVRepeatInvoiceModel = new IVRepeatInvoiceModel();
				iVRepeatInvoiceModel.IsNew = true;
			}
			return iVRepeatInvoiceModel;
		}

		public static IVRepeatInvoiceModel GetRepeatInvoiceCopyModel(MContext ctx, string pkID)
		{
			IVRepeatInvoiceModel iVRepeatInvoiceModel = ModelInfoManager.GetDataEditModel<IVRepeatInvoiceModel>(ctx, pkID, false, true);
			if (iVRepeatInvoiceModel == null)
			{
				iVRepeatInvoiceModel = new IVRepeatInvoiceModel();
			}
			iVRepeatInvoiceModel.MID = string.Empty;
			iVRepeatInvoiceModel.MStatus = Convert.ToInt32(IVInvoiceStatusEnum.Draft);
			if (iVRepeatInvoiceModel.RepeatInvoiceEntry != null && iVRepeatInvoiceModel.RepeatInvoiceEntry.Count > 0)
			{
				foreach (IVRepeatInvoiceEntryModel item in iVRepeatInvoiceModel.RepeatInvoiceEntry)
				{
					item.MEntryID = "";
					item.MID = "";
				}
			}
			return iVRepeatInvoiceModel;
		}

		public static OperationResult UpdateRepeatInvoice(MContext ctx, IVRepeatInvoiceModel model)
		{
			IVRepeatInvoiceModel dataEditModel = ModelInfoManager.GetDataEditModel<IVRepeatInvoiceModel>(ctx, model.MID, false, true);
			if (dataEditModel != null)
			{
				model.MIsMarkAsSent = dataEditModel.MIsMarkAsSent;
				model.MIsIncludePDFAttachment = dataEditModel.MIsIncludePDFAttachment;
				model.MIsSendMeACopy = dataEditModel.MIsSendMeACopy;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVRepeatInvoiceModel>(ctx, model, null, true));
			list.Add(IVRepeatInvoiceLogRepository.GetAddRepeatInvoiceEditLogCmd(ctx, model));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return new OperationResult
			{
				Success = (num > 0),
				ObjectID = model.MID
			};
		}

		public static void UpdateRepeatInvoiceStatus(MContext ctx, ParamBase param)
		{
			string sql = $"update T_IV_RepeatInvoice set MStatus = @MStatus where MID in ({param.KeyIDSWithSingleQuote})";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MStatus", MySqlDbType.Int32, 2)
			};
			array[0].Value = param.MOperationID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSql(sql, array);
			IVRepeatInvoiceLogRepository.AddRepeatInvoiceUpdateStatusLog(ctx, param);
		}

		public static OperationResult DeleteRepeatInvoiceList(MContext ctx, ParamBase param)
		{
			List<string> pkIDS = param.KeyIDs.Split(',').ToList();
			OperationResult result = ModelInfoManager.DeleteFlag<IVRepeatInvoiceModel>(ctx, pkIDS);
			IVRepeatInvoiceLogRepository.AddRepeatInvoiceDeleteLog(ctx, param);
			return result;
		}

		public static OperationResult AddRepeatInvoiceNoteLog(MContext ctx, IVRepeatInvoiceModel model)
		{
			IVRepeatInvoiceLogRepository.AddRepeatInvoiceNoteLog(ctx, model);
			return new OperationResult
			{
				Success = true
			};
		}

		public static void UpdateRepeatInvoiceExpectedInfo(MContext ctx, IVRepeatInvoiceModel model)
		{
			string sql = "update T_IV_RepeatInvoice set MExpectedDate=@MExpectedDate, MDesc=@MDesc where MID=@MID";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MExpectedDate", MySqlDbType.DateTime),
				new MySqlParameter("@MDesc", MySqlDbType.VarChar, 500),
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ((model.MExpectedDate < new DateTime(1900, 1, 1)) ? DateTime.Now.Date : model.MExpectedDate);
			array[1].Value = model.MDesc;
			array[2].Value = model.MID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSql(sql, array);
			IVRepeatInvoiceLogRepository.AddRepeatInvoiceExpectedInfoLog(ctx, model);
		}

		public static List<IVRepeatInvoiceListModel> GetRepeatInvoiceList(MContext ctx, IVInvoiceListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select a.*, c.MCurrencyID AS MOrgCyID, convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MContactName,group_concat(DISTINCT d.MAttachID) AS MAttachIDs from T_IV_RepeatInvoice a ", "JieNor-001");
			stringBuilder.Append(" LEFT JOIN T_BD_Contacts_l b ON a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete=0 ");
			stringBuilder.Append(" LEFT JOIN T_REG_Financial c ON a.MOrgID=c.MOrgID");
			stringBuilder.Append(" LEFT JOIN T_IV_InvoiceAttachment d ON a.MID=d.MParentID");
			stringBuilder.Append(" WHERE a.MIsDelete = 0 AND a.MIsActive = 1 AND a.MOrgID=@MOrgID AND LOCATE(@MType, a.MType)=1 ");
			stringBuilder.Append(GetRepeatInvoiceListFilterSql(ctx, filter));
			stringBuilder.Append(" GROUP BY a.MID");
			if (string.IsNullOrEmpty(filter.Sort))
			{
				stringBuilder.Append(" ORDER BY a.MBizDate");
			}
			else if (filter.Sort == "MContactName" || filter.Sort == "MNumber" || filter.Sort == "MDueDate" || filter.Sort == "MVerifyAmtFor" || filter.Sort == "MIsSent")
			{
				stringBuilder.AppendFormat(" ORDER BY b.MName {0}", filter.Order);
			}
			else
			{
				stringBuilder.AppendFormat(" ORDER BY a.{0} {1}", filter.Sort, filter.Order);
			}
			stringBuilder.Append(filter.PageSqlString);
			MySqlParameter[] invoiceListParameters = GetInvoiceListParameters(ctx, filter);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dt = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), invoiceListParameters).Tables[0];
			return ModelInfoManager.DataTableToList<IVRepeatInvoiceListModel>(dt);
		}

		public static int GetRepeatInvoiceTotalCount(MContext ctx, IVInvoiceListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select COUNT(*) from T_IV_RepeatInvoice a ");
			stringBuilder.Append(" LEFT JOIN T_BD_Contacts_l b ON a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID and a.MOrgID = b.MOrgID and b.MIsDelete = 0 and b.MIsActive = 1 ");
			stringBuilder.Append(" WHERE a.MIsDelete = 0 AND a.MIsActive = 1 AND a.MOrgID=@MOrgID AND LOCATE(@MType, a.MType)=1 ");
			stringBuilder.Append(GetRepeatInvoiceListFilterSql(ctx, filter));
			MySqlParameter[] invoiceListParameters = GetInvoiceListParameters(ctx, filter);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(stringBuilder.ToString(), invoiceListParameters);
			return Convert.ToInt32(single.ToString());
		}

		private static string GetRepeatInvoiceListFilterSql(MContext ctx, IVInvoiceListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (filter.MStatus > 0)
			{
				stringBuilder.Append(" AND a.MStatus = @MStatus ");
			}
			if (filter.MIsOnlyInitData)
			{
				stringBuilder.Append(" AND a.MBizDate < @MConversionDate ");
			}
			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				stringBuilder.Append(" AND (a.MReference LIKE concat('%',@MKeyword,'%') OR a.MTaxTotalAmtFor LIKE concat('%',@MKeyword,'%') OR b.MName LIKE concat('%',@MKeyword,'%')) ");
			}
			if (!string.IsNullOrEmpty(filter.MContactID))
			{
				stringBuilder.Append(" AND b.MParentID = @MContactID ");
			}
			DateTime value = new DateTime(1900, 1, 1);
			switch (filter.MSearchWithin)
			{
			case IVInvoiceSearchWithinEnum.AnyDate:
				if (filter.MStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND (a.MBizDate >= @MStartDate OR a.MEndDate >= @MStartDate OR a.MExpectedDate >= @MStartDate ) ");
				}
				if (filter.MEndDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND (a.MBizDate < @MEndDate OR a.MEndDate < @MEndDate OR a.MExpectedDate < @MStartDate) ");
				}
				break;
			case IVInvoiceSearchWithinEnum.NextInvoiceDate:
				if (filter.MStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND a.MBizDate >= @MStartDate ");
				}
				if (filter.MEndDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND a.MBizDate < @MEndDate ");
				}
				break;
			case IVInvoiceSearchWithinEnum.EndDate:
				if (filter.MStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND a.MEndDate >= @MStartDate ");
				}
				if (filter.MEndDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND a.MEndDate < @MEndDate ");
				}
				break;
			}
			return stringBuilder.ToString();
		}
	}
}
