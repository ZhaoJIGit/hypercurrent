using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
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
using System.Reflection;
using System.Text;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDContactsRepository : DataServiceT<BDContactsModel>
	{
		private readonly string multLangFieldSql = "\r\n                ,t2.MName{0}\r\n                ,t2.MFirstName{0}\r\n                ,t2.MLastName{0}\r\n                ,t2.MPCityID{0}\r\n                ,t2.MRealCityID{0}\r\n                ,t2.MRealAttention{0}\r\n                ,t2.MRealStreet{0}\r\n                ,t2.MRealRegion{0}\r\n                ,t2.MPAttention{0}\r\n                ,t2.MPStreet{0}\r\n                ,t2.MPRegion{0}\r\n                ,t2.MBankAccName{0}\r\n                ,t2.MBankName{0}\r\n                ,t8.MName{0} as MReceivableTaxRate_MName{0} ";

		private string GetCommonSelect(MContext ctx, bool includeDetail)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = ctx.MOrgVersionID == 0;
			stringBuilder.AppendFormat("SELECT \r\n                t1.MItemID,\r\n                t1.MItemID AS MContactID,\r\n                t1.MIsCustomer,\r\n                t1.MIsSupplier,\r\n                t1.MIsOther,\r\n                t2.MRealCityID,\r\n                t1.MRealPostalNo,\r\n                t1.MRealCountryID,\r\n                CONVERT(AES_DECRYPT(t1.MEmail, '{0}') USING UTF8) AS MEmail,\r\n                t1.MPPostalNo,\r\n                t1.MPCountryID,\r\n                t1.MTaxNo,\r\n                t2.MPCityID,\r\n                CONVERT(AES_DECRYPT(t1.MPhone, '{0}') USING UTF8) AS MPhone,\r\n                CONVERT(AES_DECRYPT(t1.MFax, '{0}') USING UTF8) AS MFax,\r\n                CONVERT(AES_DECRYPT(t1.MMobile, '{0}') USING UTF8) AS MMobile,\r\n                CONVERT(AES_DECRYPT(t1.MDirectPhone, '{0}') USING UTF8) AS MDirectPhone,\r\n                CONVERT(AES_DECRYPT(t1.MSkypeName, '{0}') USING UTF8) AS MSkypeName,\r\n                t1.MDefaultCyID,\r\n                CONVERT(AES_DECRYPT(t1.MBankAcctNo, '{0}') USING UTF8) AS MBankAcctNo,\r\n                t1.MModifyDate,\r\n                CONVERT(AES_DECRYPT(t1.MWebsite, '{0}') USING UTF8) AS MWebsite,\r\n                t11.MNumber as MCCurrentAccountCode,\r\n                t1.MDiscount,\r\n                t1.MIsActive,\r\n\r\n                t2.MName,\r\n                t2.MFirstName,\r\n                t2.MLastName,\r\n                t2.MRealAttention,\r\n                t2.MRealStreet,\r\n                t2.MRealRegion,\r\n                t2.MPAttention,\r\n                t2.MPStreet,\r\n                t2.MPRegion,\r\n                t2.MBankAccName,\r\n                t2.MBankName,\r\n\r\n                t8.MName as MReceivableTaxRate_MName,\r\n               \r\n                t1.MSalTaxTypeID as MReceivableTaxRate_MTaxRateID,\r\n                t7.MEffectiveTaxRate as MReceivableTaxRate_MEffectiveTaxRate,\r\n                t1.MPurTaxTypeID as MPayableTaxRate_MTaxRateID,\r\n                t9.MEffectiveTaxRate as MPayableTaxRate_MEffectiveTaxRate,\r\n\r\n                t10.MName as MPayableTaxRate_MName,\r\n                t3.MTypeID AS MContactGroupID  ", "JieNor-001");
			stringBuilder.Append(", t1.MPurDueDate as MPaymentTerms_MBills_MDay,\r\n                    t1.MSalDueDate as MPaymentTerms_MSales_MDay,\r\n                    t1.MPurDueCondition as MPaymentTerms_MBills_MDayType,\r\n                    t1.MSalDueCondition as MPaymentTerms_MSales_MDayType ");
			if (flag && includeDetail)
			{
				stringBuilder.AppendLine(",\r\n                        t4.MSaleDueAmt as MBalances_MAccountsReceivable_MOutstanding,\r\n                        t4.MSaleOverDueAmt as MBalances_MAccountsReceivable_MOverdue,\r\n                        t4.MBillDueAmt as MBalances_MAccountsPayable_MOutstanding,\r\n                        t4.MBillOverDueAmt as MBalances_MAccountsPayable_MOverdue ");
			}
			stringBuilder.AppendLine("\r\n                #_#lang_field0#_#\r\n                FROM\r\n                    t_bd_contacts t1\r\n                        INNER JOIN\r\n                    @_@t_bd_contacts_l@_@ t2 ON t1.MItemID = t2.MParentID\r\n                        AND t1.MIsDelete = t2.MIsDelete\r\n                        AND t1.MOrgID = t2.MOrgID\r\n                        LEFT JOIN\r\n                    (SELECT \n                        x.MItemID,\n                            (select \n                                    group_concat(MTypeID)\n                                from\n                                    t_bd_contactstypelink\n                                where\n                                    MOrgID = x.MOrgID \n                                        and MIsDelete = 0\n                                        and MContactID = x.MContactID) as MTypeID,\n                            x.MContactID,\n                            x.MorgID,\n                            x.MIsDelete\n                    FROM\n                        t_bd_contactstypelink x\n                    WHERE\n                        x.MIsDelete = 0) t3 ON t3.MOrgID = t1.MOrgID\r\n                        AND t3.MContactID = t1.MItemID\r\n                        AND t3.MIsDelete = 0\r\n                        LEFT JOIN\r\n                    t_reg_taxrate t7 ON t7.MOrgID = t1.MOrgID\r\n                        AND t7.MItemID = t1.MSalTaxTypeID\r\n                        AND t7.MIsDelete = 0\r\n                        LEFT JOIN\r\n                    @_@t_reg_taxrate_l@_@ t8 ON t8.MOrgID = t1.MOrgID\r\n                        AND t8.MParentID = t7.MItemID\r\n                        AND t8.MIsDelete = 0\r\n                        LEFT JOIN\r\n                    t_reg_taxrate t9 ON t9.MOrgID = t1.MOrgID\r\n                        AND t9.MItemID = t1.MPurTaxTypeID\r\n                        AND t9.MIsDelete = 0\r\n                        LEFT JOIN\r\n                    @_@t_reg_taxrate_l@_@ t10 ON t10.MOrgID = t1.MOrgID\r\n                        AND t10.MParentID = t9.MItemID\r\n                        AND t10.MIsDelete = 0 \r\n                        ");
			if (flag & includeDetail)
			{
				stringBuilder.AppendLine("LEFT JOIN\r\n                    (SELECT \r\n                        t5.MOrgID,\r\n                            t6.MContactID,\r\n                            FORMAT(SUM(CASE\r\n                                WHEN\r\n                                    (t6.MType = 'Invoice_Sale'\r\n                                        OR t6.MType = 'Invoice_Sale_Red')\r\n                                        AND t6.MTaxTotalAmtFor > t6.MVerifyAmtFor\r\n                                THEN\r\n                                    IFNULL(t6.MTaxTotalAmt, 0) - IFNULL(t6.MVerifyAmt, 0)\r\n                                ELSE 0\r\n                            END), 2) AS MSaleDueAmt,\r\n                            FORMAT(SUM(CASE\r\n                                WHEN\r\n                                    (t6.MType = 'Invoice_Sale'\r\n                                        OR t6.MType = 'Invoice_Sale_Red')\r\n                                        AND t6.MDueDate < @DateNow\r\n                                        AND t6.MTaxTotalAmtFor > t6.MVerifyAmtFor\r\n                                THEN\r\n                                    IFNULL(t6.MTaxTotalAmt, 0) - IFNULL(t6.MVerifyAmt, 0)\r\n                                ELSE 0\r\n                            END), 2) AS MSaleOverDueAmt,\r\n                            FORMAT(SUM(CASE\r\n                                WHEN\r\n                                    (t6.MType = 'Invoice_Purchase'\r\n                                        OR t6.MType = 'Invoice_Purchase_Red')\r\n                                        AND t6.MTaxTotalAmtFor > t6.MVerifyAmtFor\r\n                                THEN\r\n                                    IFNULL(t6.MTaxTotalAmt, 0) - IFNULL(t6.MVerifyAmt, 0)\r\n                                ELSE 0\r\n                            END), 2) AS MBillDueAmt,\r\n                            FORMAT(SUM(CASE\r\n                                WHEN\r\n                                    (t6.MType = 'Invoice_Purchase'\r\n                                        OR t6.MType = 'Invoice_Purchase_Red')\r\n                                        AND t6.MDueDate < @DateNow\r\n                                        AND t6.MTaxTotalAmtFor > t6.MVerifyAmtFor\r\n                                THEN\r\n                                    IFNULL(t6.MTaxTotalAmt, 0) - IFNULL(t6.MVerifyAmt, 0)\r\n                                ELSE 0\r\n                            END), 2) AS MBillOverDueAmt\r\n                    from\r\n                        t_bd_contacts t5\r\n                    INNER JOIN T_IV_Invoice t6 ON t6.MOrgID = t5.MOrgID\r\n                        and t6.MContactID = t5.MItemID\r\n                        and t6.MStatus = 3\r\n                        AND t6.MIsDelete = 0\r\n                    WHERE\r\n                        t5.MOrgID = @MOrgID\r\n                            and t5.MIsDelete = 0\r\n                    GROUP BY t5.MItemID) t4 ON t4.MOrgID = t1.MOrgID\r\n                        AND t4.MContactID = t1.MItemID ");
			}
			stringBuilder.AppendLine(" LEFT JOIN\r\n                t_bd_account t11 ON t11.MOrgID = t1.MOrgID\r\n                    AND t11.MCode = t1.MCCurrentAccountCode\r\n                    AND t11.MIsDelete = 0\r\n                WHERE\r\n                t1.MOrgID = @MOrgID\r\n                    AND t1.MIsDelete = 0 ");
			return stringBuilder.ToString();
		}

		public DataGridJson<BDContactsInfoModel> Get(MContext ctx, GetParam param)
		{
			string commonSelect = GetCommonSelect(ctx, Convert.ToBoolean(param.IncludeDetail));
			return new APIDataRepository().Get<BDContactsInfoModel>(ctx, param, commonSelect, multLangFieldSql, false, true, null);
		}

		public BDContactsEditModel GetEditModel(MContext ctx, BDContactsEditModel model)
		{
			if (string.IsNullOrWhiteSpace(model.MItemID))
			{
				return ModelInfoManager.GetEmptyDataEditModel<BDContactsEditModel>(ctx);
			}
			return ModelInfoManager.GetDataEditModel<BDContactsEditModel>(ctx, model.MItemID, false, true);
		}

		public List<BDContactsTypeLModel> GetTypeListByWhere(MContext ctx, bool isAll = true)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" SELECT t2.* , 9 PMOrderBy");
			stringBuilder.AppendLine(" FROM T_BD_ContactsType t1");
			stringBuilder.AppendLine(" JOIN T_BD_ContactsType_L t2 ON t1.MItemID=t2.MParentID and t1.MOrgID = t2.MOrgID  And t2.MLocaleID=@MLocaleID and t2.MIsDelete = 0 ");
			stringBuilder.AppendLine(" WHERE t1.MIsDelete = 0  And t1.MIsSys=0 And t1.MOrgID=@MOrgID  ");
			if (isAll)
			{
				stringBuilder.Append(" UNION ALL ");
				stringBuilder.Append(" SELECT t2.* , ");
				stringBuilder.Append(" CASE MParentID WHEN 1 THEN 1 WHEN 2 THEN 2 WHEN 4 THEN 4 WHEN 3 THEN 99 else 9 end PMOrderBy ");
				stringBuilder.AppendLine(" FROM T_BD_ContactsType t1");
				stringBuilder.AppendLine(" JOIN T_BD_ContactsType_L t2 ON t1.MItemID=t2.MParentID And t2.MLocaleID=@MLocaleID  and t2.MIsDelete = 0 ");
				stringBuilder.AppendLine(" WHERE t1.MIsDelete = 0  And t1.MIsSys=1 ");
			}
			stringBuilder.AppendLine(" ORDER BY PMOrderBy asc , MName asc");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDContactsTypeLModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), ctx.GetParameters((MySqlParameter)null)).Tables[0]);
		}

		public List<BDContactsTypeLModel> GetGroupTypeList(MContext ctx)
		{
			string sql = "  SELECT t2.* FROM  T_BD_ContactsType t1 INNER JOIN T_BD_ContactsType_L t2   ON t1.MItemID=t2.MParentID AND t1.MOrgID = t2.MOrgID  AND t2.MIsDelete = 0    WHERE t1.MIsDelete = 0 AND t1.MIsSys=0 AND t1.MOrgID=@MOrgID  ";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDContactsTypeLModel>(dynamicDbHelperMySQL.Query(sql, ctx.GetParameters((MySqlParameter)null)).Tables[0]);
		}

		public List<BDContactsInfoModel> GetContactsInfo(MContext ctx, string typeId, string searchFilter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select t1.MItemID, convert(AES_DECRYPT(t2.MName,'{0}') using utf8) AS MName,convert(AES_DECRYPT(t1.MEmail,'{0}') using utf8) AS MEmail,\r\n                                     convert(AES_DECRYPT(t1.MPhone,'{0}') using utf8) AS MPhone,convert(AES_DECRYPT(t1.MBankAcctNo,'{0}') using utf8) AS MBankAcctNo, MIsCustomer, MIsSupplier,MIsOther,MCCurrentAccountCode,t3.MItemID AS MCCurrentAccountID ", "JieNor-001");
			stringBuilder.AppendLine(" from T_BD_Contacts t1 ");
			stringBuilder.AppendLine(" INNER JOIN T_BD_Contacts_l t2 ON t1.MOrgID = t2.MOrgID and  t1.MItemID=t2.MParentID AND t2.MLocaleID=@MLocaleID and t2.MIsDelete = 0 ");
			stringBuilder.AppendLine(" LEFT JOIN (select MItemID,MCode from t_bd_account where morgid=@MOrgID and MIsDelete = 0 ) t3 ON t1.MCCurrentAccountCode=t3.MCode");
			stringBuilder.AppendLine(" WHERE t1.MIsDelete = 0 AND t1.MOrgID=@MOrgID ");
			stringBuilder.AppendLine(GetFilterWhere(typeId, searchFilter, null, false));
			stringBuilder.AppendLine(" ORDER BY MName");
			MySqlParameter[] listParameters = GetListParameters(ctx, typeId, searchFilter, null, default(DateTime));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDContactsInfoModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), listParameters).Tables[0]);
		}

		public DataGridJson<BDContactsInfoModel> GetContactPageList(MContext ctx, BDContactsInfoFilterModel filter)
		{
			filter.keyword = filter.keyword;
			SqlQuery sqlQuery = new SqlQuery();
			StringBuilder stringBuilder = null;
			bool flag = filter.IncludeArchived;
			bool isFromExport = filter.IsFromExport;
			string typeId = filter.typeId;
			if (!flag && isFromExport && (typeId == "0" || string.IsNullOrEmpty(typeId)))
			{
				flag = true;
			}
			stringBuilder = ((ctx.MOrgVersionID == 1) ? GetListSqlVersion(filter.typeId, filter.searchFilter, filter.keyword, filter.IsFromExport) : GetListSqlBuilder(filter.typeId, filter.searchFilter, filter.keyword, flag, filter.showInvoice));
			MySqlParameter[] listParameters = GetListParameters(ctx, filter.typeId, filter.searchFilter, filter.keyword, default(DateTime));
			if (!string.IsNullOrEmpty(filter.Sort))
			{
				filter.OrderBy($" CONVERT({filter.Sort} USING gbk) {filter.Order}");
			}
			else
			{
				filter.AddOrderBy("CONVERT(MName USING gbk)", SqlOrderDir.Asc);
			}
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = stringBuilder.ToString();
			MySqlParameter[] array = listParameters;
			foreach (MySqlParameter para in array)
			{
				sqlQuery.AddParameter(para);
			}
			return ModelInfoManager.GetPageDataModelListBySql<BDContactsInfoModel>(ctx, sqlQuery);
		}

		private StringBuilder GetListSqlBuilder(string typeId, string searchFilter, string keyword, bool includeArchived = false, bool showInvoice = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select MItemID, MName, MEmail, MPhone, MBankAcctNo, MContactName, MTypeID,MIsActive");
			if (showInvoice)
			{
				stringBuilder.AppendLine(", format(sum(CASE WHEN (MType = @Invoice_Sale or MType = @Invoice_Sale_Red ) AND MTaxTotalAmtFor>MVerifyAmtFor \r\n                                                THEN ifnull(MTaxTotalAmt,0)-ifnull(MVerifyAmt,0) \r\n                                                ELSE 0 END),2) AS MSaleDueAmt,");
				stringBuilder.AppendLine(" format(sum(CASE WHEN (MType = @Invoice_Sale or MType = @Invoice_Sale_Red) and MDueDate<@DateNow  AND MTaxTotalAmtFor>MVerifyAmtFor  \r\n                                                 THEN ifnull(MTaxTotalAmt,0)-ifnull(MVerifyAmt,0) \r\n                                                 ELSE 0 END),2) AS MSaleOverDueAmt,");
				stringBuilder.AppendLine(" format(sum(CASE WHEN (MType = @Invoice_Purchase or MType = @Invoice_Purchase_Red) AND MTaxTotalAmtFor>MVerifyAmtFor \r\n                                                THEN ifnull(MTaxTotalAmt,0)-ifnull(MVerifyAmt,0) \r\n                                                ELSE 0 END),2) AS MBillDueAmt,");
				stringBuilder.AppendLine(" format(sum(CASE WHEN (MType = @Invoice_Purchase or MType = @Invoice_Purchase_Red) and MDueDate<@DateNow  AND MTaxTotalAmtFor>MVerifyAmtFor  \r\n                                                THEN ifnull(MTaxTotalAmt,0)-ifnull(MVerifyAmt,0) \r\n                                                ELSE 0 END),2) AS MBillOverDueAmt");
			}
			stringBuilder.AppendLine(" from (");
			stringBuilder.AppendFormat(" select t1.MItemID,t1.MIsActive, convert(AES_DECRYPT(t2.MName,'{0}') using utf8) AS MName,convert(AES_DECRYPT(t1.MEmail,'{0}') using utf8) AS MEmail,convert(AES_DECRYPT(t1.MPhone,'{0}') using utf8) AS MPhone,convert(AES_DECRYPT(t1.MBankAcctNo,'{0}') using utf8) AS MBankAcctNo, ", "JieNor-001");
			stringBuilder.AppendLine(" (case @MLocaleID when '0x0009' then concat(t2.MFirstName,' ',t2.MLastName) else concat(t2.MLastName,' ',t2.MFirstName) end) as MContactName,t4.MTypeID ");
			if (showInvoice)
			{
				stringBuilder.AppendLine(",t3.MType,t3.MDueDate, t3.MTaxTotalAmt,t3.MTaxTotalAmtFor,t3.MVerifyAmtFor,t3.MVerifyAmt ");
			}
			stringBuilder.AppendLine(" from T_BD_Contacts t1 ");
			stringBuilder.AppendFormat(" INNER JOIN T_BD_Contacts_l t2 ON t1.MItemID=t2.MParentID AND t2.MLocaleID=@MLocaleID and t2.MOrgID = t1.MOrgID  and t2.MIsDelete = 0 and IFNULL(convert(AES_DECRYPT(t2.MName,'{0}') using utf8),'') <>'' ", "JieNor-001");
			if (showInvoice)
			{
				stringBuilder.AppendLine(" LEFT JOIN T_IV_Invoice t3 ON t1.MItemID=t3.MContactID AND t3.MIsDelete=0 and t1.MOrgID = t3.MOrgID  ");
				stringBuilder.AppendLine(" AND t3.MStatus=3 ");
			}
			stringBuilder.AppendLine(" LEFT JOIN t_bd_contactstypelink t4 on t1.MItemID=t4.MContactID and t4.MIsDelete = 0 ");
			stringBuilder.AppendLine(" WHERE t1.MIsDelete = 0  AND t1.MOrgID=@MOrgID");
			stringBuilder.AppendLine(GetFilterWhere(typeId, searchFilter, keyword, includeArchived));
			stringBuilder.AppendLine(" ) t");
			stringBuilder.AppendLine(" GROUP BY MItemID, MName, MEmail, MPhone,MBankAcctNo");
			return stringBuilder;
		}

		private StringBuilder GetListSqlVersion(string typeId, string searchFilter, string keyword, bool includeArchived = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" Select  MItemID, MName, MEmail, MPhone,MBankAcctNo,MContactName,MTypeID,MIsActive ");
			stringBuilder.AppendLine(" from (");
			stringBuilder.AppendFormat(" select t1.MItemID,t1.MIsActive, convert(AES_DECRYPT(t2.MName,'{0}') using utf8) AS MName,convert(AES_DECRYPT(t1.MEmail,'{0}') using utf8) AS MEmail,convert(AES_DECRYPT(t1.MPhone,'{0}') using utf8) AS MPhone,convert(AES_DECRYPT(t1.MBankAcctNo,'{0}') using utf8) AS MBankAcctNo,", "JieNor-001");
			stringBuilder.AppendLine(" (case @MLocaleID when '0x0009' then concat(t2.MFirstName,' ',t2.MLastName) else concat(t2.MLastName,' ',t2.MFirstName) end) as MContactName,t4.MTypeID");
			stringBuilder.AppendLine(" from T_BD_Contacts t1  ");
			stringBuilder.AppendFormat(" INNER JOIN T_BD_Contacts_l t2 ON t1.MItemID=t2.MParentID AND t2.MLocaleID=@MLocaleID and t2.MOrgID = t1.MOrgID  and t2.MIsDelete = 0 and IFNULL(convert(AES_DECRYPT(t2.MName,'{0}') using utf8),'') <>''", "JieNor-001");
			stringBuilder.AppendLine(" LEFT JOIN t_bd_contactstypelink t4 on t1.MItemID=t4.MContactID and t4.MIsDelete = 0 ");
			stringBuilder.AppendLine(" WHERE t1.MIsDelete = 0   AND t1.MOrgID=@MOrgID");
			stringBuilder.AppendLine(GetFilterWhere(typeId, searchFilter, keyword, includeArchived));
			stringBuilder.AppendLine(" ) t");
			stringBuilder.AppendLine(" GROUP BY MItemID, MName, MEmail, MPhone,MBankAcctNo");
			return stringBuilder;
		}

		private MySqlParameter[] GetListParameters(MContext ctx, string typeId, string searchFilter, string keyword = null, DateTime modifyDate = default(DateTime))
		{
			MySqlParameter[] array = new MySqlParameter[12]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@DateNow", MySqlDbType.DateTime),
				new MySqlParameter("@MTypeID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@SearchFilter", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Archive", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Keyword", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Sale_Red", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Purchase", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Invoice_Purchase_Red", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MModifyDate", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = ctx.DateNow;
			array[3].Value = typeId;
			array[4].Value = searchFilter;
			array[5].Value = "Archived";
			array[6].Value = (string.IsNullOrWhiteSpace(keyword) ? keyword : keyword.Replace("\\", "\\\\"));
			array[7].Value = "Invoice_Sale";
			array[8].Value = "Invoice_Sale_Red";
			array[9].Value = "Invoice_Purchase";
			array[10].Value = "Invoice_Purchase_Red";
			array[11].Value = modifyDate;
			return array;
		}

		private static string GetFilterWhere(string typeId, string searchFilter, string keyword = null, bool includeArchived = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!includeArchived)
			{
				if (typeId == "3")
				{
					stringBuilder.Append(" AND t1.MIsActive=0 ");
				}
				else
				{
					stringBuilder.Append(" AND t1.MIsActive=1 ");
				}
			}
			if (!string.IsNullOrWhiteSpace(typeId) && !typeId.Equals("0"))
			{
				if (typeId == "1")
				{
					stringBuilder.Append(" and t1.MIsCustomer=1 ");
				}
				else if (typeId == "2")
				{
					stringBuilder.Append(" and t1.MIsSupplier=1 ");
				}
				else if (typeId == "4")
				{
					stringBuilder.Append(" and t1.MIsOther =1");
				}
				else if (typeId != "3")
				{
					stringBuilder.AppendLine(" AND t1.MItemID IN (");
					stringBuilder.AppendLine(" SELECT MContactID FROM T_BD_ContactsTypeLink WHERE MTypeID=@MTypeID and MIsDelete = 0  ");
					stringBuilder.AppendLine(" )");
				}
			}
			if (!string.IsNullOrWhiteSpace(searchFilter) && !searchFilter.Equals("0") && !searchFilter.Equals("ALL"))
			{
				if (searchFilter.Equals("123"))
				{
					stringBuilder.AppendFormat(" AND convert(AES_DECRYPT(t2.MName,'{0}') using utf8) REGEXP '^[0-9]' ", "JieNor-001");
				}
				else
				{
					stringBuilder.AppendFormat(" AND  ((t2.MNameFirstLetter  like concat(@SearchFilter, '%')) or (convert(AES_DECRYPT(t2.MName,'{0}') using utf8) like concat(@SearchFilter, '%'))) ", "JieNor-001");
				}
			}
			if (!string.IsNullOrWhiteSpace(keyword))
			{
				stringBuilder.AppendFormat(" AND (convert(AES_DECRYPT(t2.MName,'{0}') using utf8) like concat('%', @Keyword, '%') \r\n                  or convert(AES_DECRYPT(t1.MEmail,'{0}') using utf8) like concat('%', @Keyword, '%') \r\n                  or t2.MLastName like concat('%', @Keyword, '%') or t2.MFirstName like concat('%', @Keyword, '%') \r\n                  or concat(t2.MLastName,' ',t2.MFirstName) like concat('%', @Keyword, '%') \r\n                  or concat(t2.MFirstName,' ',t2.MLastName) like concat('%', @Keyword, '%') \r\n                  or t2.MNameFirstLetter  like concat(@Keyword, '%')\r\n                  or convert(AES_DECRYPT(t1.MPhone,'{0}') using utf8) like concat('%', @Keyword, '%') \r\n                  )", "JieNor-001");
			}
			return stringBuilder.ToString();
		}

		public OperationResult ContactsUpdate(MContext ctx, BDContactsInfoModel model, List<CommandInfo> cmdList, List<string> fields = null)
		{
			OperationResult operationResult = new OperationResult();
			OptLogTemplate optLogTemplate = OptLogTemplate.None;
			if (model.IsDelete)
			{
				model.MIsDelete = true;
			}
			optLogTemplate = ((!string.IsNullOrWhiteSpace(model.MItemID)) ? OptLogTemplate.Contact_Edited : OptLogTemplate.Contact_Created);
			if (model.MIsValidateName && ModelInfoManager.IsLangColumnValueExists<BDContactsInfoModel>(ctx, "MName", model.MultiLanguage, model.MItemID, "", "", true))
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "ContactExist", "The name already exists!")
				});
				return operationResult;
			}
			BDContactsInfoModel bDContactsInfoModel = MultiLanguageAdd(model);
			cmdList = (cmdList ?? new List<CommandInfo>());
			cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsInfoModel>(ctx, bDContactsInfoModel, fields, true));
			List<CommandInfo> saveContactTrackCommandList = GetSaveContactTrackCommandList(ctx, model);
			if (saveContactTrackCommandList != null)
			{
				cmdList.AddRange(saveContactTrackCommandList);
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(cmdList);
			if (num > 0)
			{
				OptLog.AddLog(optLogTemplate, ctx, model.MItemID, model.MName);
			}
			operationResult.Success = (num > 0);
			if (operationResult.Success)
			{
				operationResult.ObjectID = bDContactsInfoModel.MContactID;
			}
			return operationResult;
		}

		public BDContactsInfoModel MultiLanguageAdd(BDContactsInfoModel model)
		{
			MultiLanguageFieldList multiLanguageFieldList = model.MultiLanguage.FirstOrDefault((MultiLanguageFieldList t) => t.MFieldName == "MName");
			if (multiLanguageFieldList == null)
			{
				return model;
			}
			foreach (MultiLanguageField item in multiLanguageFieldList.MMultiLanguageField)
			{
				string mLocaleID = item.MLocaleID;
				string text2 = item.MValue = item.MValue.Trim();
				string chineseSpell = AchieveLetter.GetChineseSpell(text2);
				string filedValue = AchieveLetter.StrConvertToPinyin(text2);
				MultiLangHelper.UpdateValue(mLocaleID, "MNameLetter", filedValue, model.MultiLanguage);
				MultiLangHelper.UpdateValue(mLocaleID, "MNameFirstLetter", chineseSpell, model.MultiLanguage);
			}
			return model;
		}

		public OperationResult ContactsGroupUpdate(MContext ctx, BDContactsGroupModel model)
		{
			if (model.IsDelete)
			{
				model.MIsDelete = true;
			}
			return ModelInfoManager.InsertOrUpdate<BDContactsGroupModel>(ctx, model, null);
		}

		public OperationResult ContactToGroup(MContext ctx, string selIds, string typeId, MultiLanguageFieldList newGroupLangModel, bool isExist, string moveFromTypeId)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> contactToGroupCmdList = GetContactToGroupCmdList(ctx, selIds, typeId, newGroupLangModel, isExist, moveFromTypeId);
			if (contactToGroupCmdList.Count > 0)
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(contactToGroupCmdList) > 0);
			}
			return operationResult;
		}

		public List<CommandInfo> GetContactToGroupCmdList(MContext ctx, string selIds, string typeId, MultiLanguageFieldList newGroupLangModel = null, bool isExist = true, string moveFromTypeId = null)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (!isExist)
			{
				BDContactsGroupModel emptyDataEditModel = ModelInfoManager.GetEmptyDataEditModel<BDContactsGroupModel>(ctx);
				emptyDataEditModel.MOrgID = ctx.MOrgID;
				foreach (MultiLanguageFieldList item in emptyDataEditModel.MultiLanguage)
				{
					item.MParentID = emptyDataEditModel.MItemID;
					item.MMultiLanguageField = newGroupLangModel.MMultiLanguageField;
				}
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsGroupModel>(ctx, emptyDataEditModel, null, true));
				typeId = emptyDataEditModel.MItemID;
			}
			List<string> list2 = base.ConvertGridKeyIDs(selIds);
			foreach (string item2 in list2)
			{
				List<BDContactsTypeLinkModel> contactTypeLinkList = GetContactTypeLinkList(ctx, typeId, new List<string>
				{
					item2
				}, false);
				if (contactTypeLinkList == null || contactTypeLinkList.Count == 0)
				{
					BDContactsTypeLinkModel bDContactsTypeLinkModel = new BDContactsTypeLinkModel();
					bDContactsTypeLinkModel.MItemID = UUIDHelper.GetGuid();
					bDContactsTypeLinkModel.MTypeID = typeId;
					bDContactsTypeLinkModel.MContactID = item2;
					list.Add(GetContactTypeLinkInsertSql(ctx, bDContactsTypeLinkModel));
				}
			}
			if (!string.IsNullOrEmpty(moveFromTypeId) && typeId != moveFromTypeId)
			{
				List<BDContactsTypeLinkModel> contactTypeLinkList2 = GetContactTypeLinkList(ctx, moveFromTypeId, list2, false);
				List<CommandInfo> deleteContactTypeLinkCmdList = GetDeleteContactTypeLinkCmdList(ctx, (from t in contactTypeLinkList2
				select t.MItemID).ToList());
				if (deleteContactTypeLinkCmdList != null && deleteContactTypeLinkCmdList.Count() > 0)
				{
					list.InsertRange(0, deleteContactTypeLinkCmdList);
				}
			}
			return list;
		}

		public OperationResult ContactToArchivedGroup(MContext ctx, string selIds, string typeName = "Archived")
		{
			BDContactsTypeLModel typeLModelByName = GetTypeLModelByName(typeName, ctx);
			return ContactToGroup(ctx, selIds, typeLModelByName.MParentID, null, true, null);
		}

		public OperationResult ArchiveContact(MContext ctx, List<string> contactIds, bool isActive)
		{
			OperationResult operationResult = new OperationResult();
			if (isActive)
			{
				string ids = string.Join(",", contactIds);
				List<string> source = base.ConvertGridKeyIDs(ids);
				if ((from id in source
				select GetContactModelByKey(id, ctx) into model
				select ModelInfoManager.IsLangColumnValueExists<BDContactsInfoModel>(ctx, "MName", model.MultiLanguage, model.MItemID, "", "", true)).Any((bool existName) => existName))
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "RestoreContactFailure", "联系人已存在，恢复操作失败！");
					return operationResult;
				}
			}
			List<CommandInfo> list = new List<CommandInfo>();
			List<MySqlParameter> list2 = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MIsActive", MySqlDbType.Bit)
			};
			list2[0].Value = ctx.MOrgID;
			list2[1].Value = isActive;
			string inFilterQuery = GLUtility.GetInFilterQuery(contactIds, ref list2, "M_ID");
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update T_BD_Contacts_l set MIsActive=@MIsActive where MOrgID=@MOrgID and MParentID " + inFilterQuery;
			DbParameter[] array = commandInfo.Parameters = list2.ToArray();
			list.Add(commandInfo);
			List<CommandInfo> archiveFlagCmd = ModelInfoManager.GetArchiveFlagCmd<BDContactsInfoModel>(ctx, contactIds, isActive);
			list.AddRange(archiveFlagCmd);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0);
			return operationResult;
		}

		public void ContactMoveOutGroup(MContext ctx, string selIds, string moveFromTypeId)
		{
			List<string> contactIdList = base.ConvertGridKeyIDs(selIds);
			List<BDContactsTypeLinkModel> contactTypeLinkList = GetContactTypeLinkList(ctx, moveFromTypeId, contactIdList, false);
			bool flag = ContactTypeLinkDelete(ctx, (from t in contactTypeLinkList
			select t.MItemID).ToList());
		}

		public OperationResult DelGroupAndLink(MContext ctx, string typeId)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = false;
			List<CommandInfo> list = new List<CommandInfo>();
			List<BDContactsTypeLinkModel> contactTypeLinkList = GetContactTypeLinkList(ctx, typeId, null, false);
			list.AddRange(GetDeleteContactTypeLinkCmdList(ctx, (from t in contactTypeLinkList
			select t.MItemID).ToList()));
			list.AddRange(GetContactGroupDeleteSql(ctx, typeId));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public bool IsSysContactType(MContext ctx, string typeId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT 1 from t_bd_contactstype where MItemID=@TypeID and MIsSys=1");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@TypeID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = typeId;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(stringBuilder.ToString(), array);
		}

		public BDContactsInfoModel GetContactEditInfo(MContext ctx, BDContactsInfoModel info)
		{
			BDContactsInfoModel dataEditModel = ModelInfoManager.GetDataEditModel<BDContactsInfoModel>(ctx, info.MItemID, false, true);
			if (dataEditModel == null)
			{
				return new BDContactsInfoModel();
			}
			return dataEditModel;
		}

		public List<CommandInfo> GetContactGroupDeleteSql(MContext ctx, string keyId)
		{
			return ModelInfoManager.GetDeleteFlagCmd<BDContactsTypeModel>(ctx, keyId);
		}

		public CommandInfo GetContactTypeLinkInsertSql(MContext ctx, BDContactsTypeLinkModel model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			//CommandInfo commandInfo = null;
			stringBuilder.Append(" UPDATE T_BD_ContactsTypeLink SET MIsActive = 0 WHERE MContactID = @MContactID AND MOrgID=@MOrgID ; ");
			stringBuilder.Append("insert into T_BD_ContactsTypeLink(");
			stringBuilder.Append("MItemID,MTypeID,MContactID,MOrgID");
			stringBuilder.Append(") values (");
			stringBuilder.Append("@MItemID,@MTypeID,@MContactID,@MOrgID");
			stringBuilder.Append(") ");
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTypeID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = model.MItemID;
			array[1].Value = model.MTypeID;
			array[2].Value = model.MContactID;
			array[3].Value = ctx.MOrgID;
			return new CommandInfo(stringBuilder.ToString(), array);
		}

		public bool ContactTypeLinkDelete(MContext ctx, List<string> idList)
		{
			if (idList == null || idList.Count == 0)
			{
				return true;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("update T_BD_ContactsTypeLink ");
			stringBuilder.Append("set MIsDelete = 1 ");
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			string whereInSql = base.GetWhereInSql(idList, ref parameters, null);
			stringBuilder.AppendFormat(" where MItemID in ({0}) and  MOrgID = @MOrgID and MIsDelete = 0 ", whereInSql);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.ExecuteSql(stringBuilder.ToString(), parameters) > 0;
		}

		public List<CommandInfo> GetDeleteContactTypeLinkCmdList(MContext ctx, List<string> idList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (idList == null || idList.Count() == 0)
			{
				return list;
			}
			CommandInfo commandInfo = new CommandInfo();
			string str = "update T_BD_ContactsTypeLink set MIsDelete = 1 ";
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			string whereInSql = base.GetWhereInSql(idList, ref parameters, null);
			str = (commandInfo.CommandText = str + $" where MItemID in ({whereInSql}) and  MOrgID = @MOrgID and MIsDelete = 0 ");
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}

		public bool ContactTypeLinkRestore(MContext ctx, List<string> idList)
		{
			if (idList == null || idList.Count == 0)
			{
				return true;
			}
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			string whereInSql = base.GetWhereInSql(idList, ref parameters, null);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("update T_BD_ContactsTypeLink ");
			stringBuilder.Append("set MIsDelete = 0 ");
			stringBuilder.AppendFormat(" where MItemID in ( {0}) and MIsDelete = 1  and MOrgID = @MOrgID ", whereInSql);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSql(stringBuilder.ToString(), parameters);
			if (num > 0)
			{
				return true;
			}
			return false;
		}

		public List<CommandInfo> GetRestoreContactTypeLinkCmdList(MContext ctx, List<string> idList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (idList == null || idList.Count == 0)
			{
				return list;
			}
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			string whereInSql = base.GetWhereInSql(idList, ref parameters, null);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("update T_BD_ContactsTypeLink ");
			stringBuilder.Append("set MIsDelete = 0 ");
			stringBuilder.AppendFormat(" where MItemID in ( {0}) and MIsDelete = 1  and MOrgID = @MOrgID ", whereInSql);
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = stringBuilder.ToString();
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}

		public BDContactsTypeLModel GetTypeLModelByName(string name, MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select MPKID, MParentID, MLocaleID, MName, MDesc  ");
			stringBuilder.Append("  from T_BD_ContactsType_L ");
			stringBuilder.Append(" where MName=@MName ");
			BDContactsTypeLModel bDContactsTypeLModel = new BDContactsTypeLModel();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MName", name)
			};
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), cmdParms);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				return ModelInfoManager.DataTableToList<BDContactsTypeLModel>(dataSet.Tables[0])[0];
			}
			return null;
		}

		public List<BDContactsTypeLinkModel> GetContactTypeLinkList(MContext ctx, string typeId, List<string> contactIdList, bool isDelete = false)
		{
			if (contactIdList == null)
			{
				contactIdList = new List<string>();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT * ");
			stringBuilder.Append(" FROM T_BD_ContactsTypeLink a inner join t_bd_contacts b on a.MOrgID = b.MOrgID and a.MContactID=b.MItemID  and b.MIsDelete = 0 ");
			stringBuilder.Append("WHERE a.MOrgID = @MOrgID ");
			if (isDelete)
			{
				stringBuilder.Append(" and a.MIsDelete = 1  ");
			}
			else
			{
				stringBuilder.Append(" and a.MIsDelete = 0 ");
			}
			if (!string.IsNullOrEmpty(typeId))
			{
				stringBuilder.Append(" AND  a.MTypeID=@MTypeID ");
			}
			MySqlParameter[] array = new MySqlParameter[contactIdList.Count + 1];
			array[0] = new MySqlParameter("MTypeID", typeId);
			if (contactIdList.Count > 0)
			{
				stringBuilder.Append(" AND a.MContactID IN(");
				int num = 1;
				foreach (string contactId in contactIdList)
				{
					string text = $"@MContactID{num}";
					stringBuilder.AppendFormat("{0},", text);
					array[num] = new MySqlParameter(text, contactId);
					num++;
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.Append(")");
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), ctx.GetParameters(array));
			return ModelInfoManager.DataTableToList<BDContactsTypeLinkModel>(dataSet.Tables[0]);
		}

		public List<BDContactsInfoModel> GetDisableContactList(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" SELECT TRIM(CONVERT(AES_DECRYPT(bl.MName,'{0}') USING utf8)) as MName");
			stringBuilder.Append(" FROM t_bd_contacts b ");
			stringBuilder.Append(" inner join t_bd_contacts_l bl on  b.MItemID=bl.MParentID  and bl.MIsDelete = 0 and b.MOrgID=bl.MOrgID");
			stringBuilder.Append(" WHERE b.MOrgID = @MOrgID and bl.MLocaleID=@MLocaleID and b.MIsActive=0");
			string sql = string.Format(stringBuilder.ToString(), "JieNor-001");
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<BDContactsInfoModel>(dataSet.Tables[0]);
		}

		public BDContactsInfoModel GetContactModelByKey(string MItemID, MContext ctx)
		{
			return ModelInfoManager.GetDataEditModel<BDContactsInfoModel>(ctx, MItemID, false, true);
		}

		public void ReWriteTrack(BDContactsTrackLinkRepository _trc, List<CommandInfo> comman, MContext ctx, string TrackID, string ContactID, string SalTrackId, string PurTrackId)
		{
			if (!string.IsNullOrWhiteSpace(TrackID))
			{
				SqlWhere filter = new SqlWhere().AddFilter("MTrackID", SqlOperators.Equal, TrackID).AddFilter("MContactID", SqlOperators.Equal, ContactID);
				List<BDContactsTrackLinkModel> modelList = _trc.GetModelList(ctx, filter, false);
				if (modelList == null || modelList.Count == 0)
				{
					BDContactsTrackLinkModel bDContactsTrackLinkModel = new BDContactsTrackLinkModel();
					bDContactsTrackLinkModel.MTrackID = TrackID;
					bDContactsTrackLinkModel.MContactID = ContactID;
					bDContactsTrackLinkModel.MSalTrackId = SalTrackId;
					bDContactsTrackLinkModel.MPurTrackId = PurTrackId;
					comman.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsTrackLinkModel>(ctx, bDContactsTrackLinkModel, null, true));
				}
				else
				{
					BDContactsTrackLinkModel bDContactsTrackLinkModel = modelList[0];
					bDContactsTrackLinkModel.MSalTrackId = SalTrackId;
					bDContactsTrackLinkModel.MPurTrackId = PurTrackId;
					comman.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsTrackLinkModel>(ctx, bDContactsTrackLinkModel, null, true));
				}
			}
		}

		public List<NameValueModel> GetContactNameInfoList(MContext ctx)
		{
			string sql = string.Format("SELECT a.MItemID AS MValue, convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MName,t3.MItemID AS MTag,\r\n                    CONCAT_WS(',', CASE WHEN a.MIsSupplier=1 THEN '{1}' ELSE '' END, CASE WHEN a.MIsCustomer=1 THEN '{2}' ELSE '' END, CASE WHEN a.MIsOther=1 THEN '{3}' ELSE '' END) as MValue1\r\n                FROM t_bd_contacts a \r\n                    INNER JOIN t_bd_contacts_l b ON a.MItemID=b.MParentID  and b.MOrgID = a.MOrgID and b.MIsDelete = 0 \r\n                    LEFT JOIN (select MItemID,MCode from t_bd_account where morgid=@MOrgID and MIsDelete = 0 ) t3 ON a.MCCurrentAccountCode=t3.MCode\r\n                WHERE a.MOrgID=@MOrgID  AND a.MIsDelete=0 AND a.MIsActive=1", "JieNor-001", Convert.ToInt32(BDContactType.Supplier), Convert.ToInt32(BDContactType.Customer), Convert.ToInt32(BDContactType.Other));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, ctx.GetParameters((MySqlParameter)null));
			return ModelInfoManager.DataTableToList<NameValueModel>(ds);
		}

		public BDContactsInfoModel GetStatementContData(MContext ctx, string contactID)
		{
			BDContactsInfoModel result = new BDContactsInfoModel();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select t1.MItemID,convert(AES_DECRYPT(t1.MEmail,'{0}') using utf8) AS MEmail,convert(AES_DECRYPT(t2.MName,'{0}') using utf8) AS MName,t1.MPCountryID,t2.MPCityID,t1.MPPostalNo,t1.MIsCustomer,t1.MIsSupplier,t1.MIsOther,t2.MPAttention,t2.MPStreet,t2.MPRegion,t1.MIsActive ", "JieNor-001");
			stringBuilder.AppendLine(" from T_BD_Contacts t1 ");
			stringBuilder.AppendLine(" join T_BD_Contacts_l t2 on t1.MItemID=t2.MParentID and t2.MLocaleID=@MLocaleID and t2.MIsDelete = 0  ");
			stringBuilder.AppendLine(" where t1.MIsDelete=0 and t1.MItemID=@MItemID and t1.MOrgID = @MOrgID");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), ctx.GetParameters("@MItemID", contactID));
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				result = ModelInfoManager.DataTableToList<BDContactsInfoModel>(dataSet.Tables[0])[0];
			}
			return result;
		}

		public BDContactsInfoModel GetContactViewData(MContext ctx, string contactID)
		{
			BDContactsInfoModel result = new BDContactsInfoModel();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select t9.MFullName as AccountFullName,t1.MItemID,convert(AES_DECRYPT(t1.MEmail,'{0}') using utf8) AS MEmail,convert(AES_DECRYPT(t2.MName,'{0}') using utf8) AS MName,t2.MFirstName,t2.MLastName,convert(AES_DECRYPT(t1.MPhone,'{0}') using utf8) AS MPhone,convert(AES_DECRYPT(t1.MFax,'{0}') using utf8) AS MFax,convert(AES_DECRYPT(t1.MMobile,'{0}') using utf8) AS MMobile,convert(AES_DECRYPT(t1.MDirectPhone,'{0}') using utf8) AS MDirectPhone,convert(AES_DECRYPT(t1.MSkypeName,'{0}') using utf8) AS MSkypeName,convert(AES_DECRYPT(t1.MWebsite,'{0}') using utf8) AS MWebsite,convert(AES_DECRYPT(t1.MBankAcctNo,'{0}') using utf8) AS MBankAcctNo , ", "JieNor-001");
			stringBuilder.AppendLine(" (case @MLocaleID when '0x0009' then concat(t2.MFirstName, ' ', t2.MLastName) else concat(t2.MLastName, t2.MFirstName) end) as MFullName,");
			stringBuilder.AppendLine(" t18.MName as MPCountryName,t2.MPCityID,t1.MPPostalNo,t2.MPAttention,t2.MPStreet,t2.MPRegion,");
			stringBuilder.AppendLine(" t19.MName as MRealCountryName,t2.MRealCityID,t1.MRealPostalNo,t2.MRealAttention,t2.MRealStreet,t2.MRealRegion,");
			stringBuilder.AppendLine(" t1.MTaxNo,t3.MParentID AS MDefaultSaleTaxID, t3.MName as MSalTaxTypeID,t4.MParentID AS MDefaultPurchaseTaxID, t4.MName as MPurTaxTypeID,t5.MName as MRecAcctID,t6.MName as MPayAcctID,");
			stringBuilder.AppendLine(" t1.MDiscount,t7.MParentID as MDefaultCurrencyID,t7.MName as MDefaultCyID,t2.MBankName,t2.MBankAccName , t1.MPurDueDate,t1.MPurDueCondition,t1.MSalDueDate,t1.MSalDueCondition,t1.MIsCustomer,t1.MIsOther,t1.MIsSupplier,");
			stringBuilder.AppendLine(" t31.MTaxRate as MSalTaxRate , t41.MTaxRate as MPurTaxRate ");
			stringBuilder.AppendLine(" from T_BD_Contacts t1 ");
			stringBuilder.AppendLine(" join T_BD_Contacts_l t2 on  t1.MItemID=t2.MParentID and t2.MLocaleID=@MLocaleID and t2.MOrgID = t1.MOrgID and t2.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_REG_TaxRate_L t3 on  t1.MSalTaxTypeID=t3.MParentID and t3.MLocaleID=@MLocaleID  and t3.MOrgID = t1.MOrgID and t3.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_REG_TaxRate t31 on t1.MSalTaxTypeID = t31.MItemID and t31.MOrgID = t1.MOrgID and t31.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_REG_TaxRate_L t4 on  t1.MPurTaxTypeID=t4.MParentID and t4.MLocaleID=@MLocaleID  and t4.MOrgID = t1.MOrgID and t4.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_REG_TaxRate t41 on t1.MPurTaxTypeID = t41.MItemID and t41.MOrgID = t1.MOrgID and t41.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_BD_AcctType_l t5 on  t1.MRecAcctID=t5.MParentID and t5.MLocaleID=@MLocaleID and t5.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_BD_AcctType_l t6 on  t1.MPayAcctID=t6.MParentID and t6.MLocaleID=@MLocaleID and t6.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_Bas_Currency_L t7 on  t1.MDefaultCyID=t7.MParentID and t7.MLocaleID=@MLocaleID and t7.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join t_bas_country_l t18 on  t1.MPCountryID=t18.MParentID and t18.MLocaleID=@MLocaleID and t18.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join t_bas_country_l t19 on  t1.MRealCountryID=t19.MParentID and t19.MLocaleID=@MLocaleID and t19.MIsDelete = 0 ");
			stringBuilder.AppendLine(" LEFT JOIN (SELECT a.*,b.MCode  FROM t_bd_account_l a INNER JOIN  t_bd_account b ON a.MParentID=b.MItemID and a.MOrgID = b.MOrgID and b.MIsDelete = 0   and  b.MOrgID=@MOrgID WHERE a.MLocaleID=@MLocaleID and a.MOrgID = @MOrgID and a.MIsDelete = 0 ) t9 ON t9.MCode=t1.MCCurrentAccountCode ");
			stringBuilder.AppendLine(" where t1.MIsDelete=0 and t1.MOrgID = @MOrgID   and t1.MItemID=@MItemID ");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 100)
			};
			array[0].Value = contactID;
			array[1].Value = ctx.MLCID;
			array[2].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				result = ModelInfoManager.DataTableToList<BDContactsInfoModel>(dataSet.Tables[0])[0];
			}
			return result;
		}

		public List<BDContactsInfoModel> GetContactViewDataList(MContext ctx, string contactIDs)
		{
			List<BDContactsInfoModel> list = new List<BDContactsInfoModel>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select t9.MFullName as AccountFullName,t1.MItemID,convert(AES_DECRYPT(t1.MEmail,'{0}') using utf8) AS MEmail,convert(AES_DECRYPT(t2.MName,'{0}') using utf8) AS MName,t2.MFirstName,t2.MLastName,convert(AES_DECRYPT(t1.MPhone,'{0}') using utf8) AS MPhone,convert(AES_DECRYPT(t1.MFax,'{0}') using utf8) AS MFax,convert(AES_DECRYPT(t1.MMobile,'{0}') using utf8) AS MMobile,convert(AES_DECRYPT(t1.MDirectPhone,'{0}') using utf8) AS MDirectPhone,convert(AES_DECRYPT(t1.MSkypeName,'{0}') using utf8) AS MSkypeName,convert(AES_DECRYPT(t1.MWebsite,'{0}') using utf8) AS MWebsite,convert(AES_DECRYPT(t1.MBankAcctNo,'{0}') using utf8) AS MBankAcctNo , ", "JieNor-001");
			stringBuilder.AppendLine(" (case @MLocaleID when '0x0009' then concat(t2.MFirstName, ' ', t2.MLastName) else concat(t2.MLastName, t2.MFirstName) end) as MFullName,");
			stringBuilder.AppendLine(" t18.MName as MPCountryName,t2.MPCityID,t1.MPPostalNo,t2.MPAttention,t2.MPStreet,t2.MPRegion,");
			stringBuilder.AppendLine(" t19.MName as MRealCountryName,t2.MRealCityID,t1.MRealPostalNo,t2.MRealAttention,t2.MRealStreet,t2.MRealRegion,");
			stringBuilder.AppendLine(" t1.MTaxNo,t3.MParentID AS MDefaultSaleTaxID, t3.MName as MSalTaxTypeID,t4.MParentID AS MDefaultPurchaseTaxID, t4.MName as MPurTaxTypeID,t5.MName as MRecAcctID,t6.MName as MPayAcctID,");
			stringBuilder.AppendLine(" t1.MDiscount,t7.MParentID as MDefaultCurrencyID,t7.MName as MDefaultCyID,t2.MBankName,t2.MBankAccName , t1.MPurDueDate,t1.MPurDueCondition,t1.MSalDueDate,t1.MSalDueCondition ");
			stringBuilder.AppendLine(" from T_BD_Contacts t1 ");
			stringBuilder.AppendLine(" join T_BD_Contacts_l t2 on  t1.MItemID=t2.MParentID and t2.MLocaleID=@MLocaleID and t2.MIsDelete=0");
			stringBuilder.AppendLine(" left join T_REG_TaxRate_L t3 on  t1.MSalTaxTypeID=t3.MParentID and t3.MLocaleID=@MLocaleID  and t3.MOrgID = t1.MOrgID and t3.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_REG_TaxRate_L t4 on  t1.MPurTaxTypeID=t4.MParentID and t4.MLocaleID=@MLocaleID  and t4.MOrgID = t1.MOrgID and t4.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_BD_AcctType_l t5 on  t1.MRecAcctID=t5.MParentID and t5.MLocaleID=@MLocaleID");
			stringBuilder.AppendLine(" left join T_BD_AcctType_l t6 on  t1.MPayAcctID=t6.MParentID and t6.MLocaleID=@MLocaleID");
			stringBuilder.AppendLine(" left join T_Bas_Currency_L t7 on  t1.MDefaultCyID=t7.MParentID and t7.MLocaleID=@MLocaleID");
			stringBuilder.AppendLine(" left join T_Bas_Country_L t18 on  t1.MPCountryID=t18.MParentID and t18.MLocaleID=@MLocaleID and t18.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_Bas_Country_L t19 on  t1.MRealCountryID=t19.MParentID and t19.MLocaleID=@MLocaleID and t19.MIsDelete = 0 ");
			stringBuilder.AppendLine(" LEFT join t_bd_account_l t9 ON t9.MParentID = (\r\n                    SELECT MItemID from t_bd_account\r\n                    where mcode = t1.MCCurrentAccountCode and MOrgID=@MOrgID\r\n                    and MOrgID = @MOrgID and MIsDelete = 0 )  and t9.MLocaleID=@MLocaleID ");
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			stringBuilder.AppendFormat(" where t1.MOrgID = @MOrgID  and  t1.MIsDelete=0 and t1.MItemID in ({0}) ", base.GetWhereInSql(contactIDs, ref parameters, null));
			return ModelInfoManager.GetDataModelBySql<BDContactsInfoModel>(ctx, stringBuilder.ToString(), parameters);
		}

		public OperationResult IsImportContactNamesExist(MContext ctx, List<BDContactsInfoModel> list)
		{
			OperationResult operationResult = new OperationResult();
			List<string> list2 = new List<string>();
			foreach (BDContactsInfoModel item in list)
			{
				MultiLanguageFieldList multiLanguageFieldList = item.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
				list2.AddRange((from f in multiLanguageFieldList.MMultiLanguageField
				select f.MValue).ToList());
			}
			List<BDContactsModel> contactListByNameOrId = GetContactListByNameOrId(ctx, list2, false, true);
			if (contactListByNameOrId.Any())
			{
				IEnumerable<string> values = (from f in contactListByNameOrId
				select f.MName).Distinct();
				operationResult.Tag = "Exist";
				string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "ImportContactExist", "The name:{0} already exists, are you sure to continue?"), string.Join(",", values));
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = message
				});
			}
			return operationResult;
		}

		public OperationResult IsImportContactHaveSameName(MContext ctx, List<BDContactsInfoModel> list)
		{
			OperationResult operationResult = new OperationResult();
			List<string> list2 = new List<string>();
			if ((from m in list
			group m by m.MName).Count() != list.Count)
			{
				operationResult.Success = false;
				string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "ImportContactExistSameName", "导入的文件中存在相同的联系人"));
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = message
				});
				return operationResult;
			}
			List<BDContactsModel> contactList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).ContactList;
			foreach (BDContactsInfoModel item in list)
			{
				IEnumerable<BDContactsModel> source = from m in contactList
				where m.MName == item.MName
				select m;
				if (source.Count() > 1)
				{
					list2.Add(item.MName);
				}
			}
			if (list2.Any())
			{
				operationResult.Success = false;
				string message2 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "ImportContactExistMore", "联系人:{0}存在多个."), string.Join(",", list2));
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = message2
				});
			}
			return operationResult;
		}

		public OperationResult ImportContactList(MContext ctx, List<BDContactsInfoModel> list)
		{
			List<CommandInfo> insertContactCommandList = GetInsertContactCommandList(ctx, list);
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertContactCommandList) > 0)
			};
		}

		private List<CommandInfo> GetInsertContactCommandList(MContext ctx, List<BDContactsInfoModel> list)
		{
			List<CommandInfo> list2 = new List<CommandInfo>();
			List<BDTrackModel> list3 = new BDTrackRepository().GetList(string.Empty, ctx, null, null, false, true, "");
			List<string> list4 = (list3 != null) ? (from t in list3
			select t.MItemID).Distinct().ToList() : new List<string>();
			List<BDContactsTypeModel> list5 = new List<BDContactsTypeModel>();
			List<BDContactsTypeLinkModel> list6 = new List<BDContactsTypeLinkModel>();
			BDContactsTypeModel contactTypeModel = null;
			List<BDContactsTypeModel> contactTypeList = GetContactTypeList(ctx);
			List<BDContactsModel> contactList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).ContactList;
			List<BDContactsTypeLinkModel> modelList = new BDContactsTypeLinkRepository().GetModelList(ctx, new SqlWhere(), false);
			foreach (BDContactsInfoModel item in list)
			{
				BDContactsModel bDContactsModel = contactList.FirstOrDefault((BDContactsModel m) => m.MName == item.MName);
				if (bDContactsModel != null)
				{
					item.MItemID = bDContactsModel.MItemID;
					item.MIsCustomer = bDContactsModel.MIsCustomer;
					item.MIsSupplier = bDContactsModel.MIsSupplier;
					item.MIsOther = bDContactsModel.MIsOther;
				}
				else
				{
					item.IsNew = true;
					item.MItemID = UUIDHelper.GetGuid();
				}
				List<MultiLanguageFieldList> list7 = (from f in item.MultiLanguage
				where f.MFieldName == "MGroupName"
				select f).ToList();
				MultiLanguageField firstLangField = list7[0].MMultiLanguageField.FirstOrDefault((MultiLanguageField f) => !string.IsNullOrWhiteSpace(f.MValue));
				if (firstLangField != null)
				{
					contactTypeModel = contactTypeList.FirstOrDefault((BDContactsTypeModel f) => !string.IsNullOrWhiteSpace(f.MName) && f.MName.Trim() == firstLangField.MValue.Trim());
					if (contactTypeModel == null)
					{
						contactTypeModel = FindContactTypeFromNewList(list5, firstLangField.MValue);
						if (contactTypeModel == null)
						{
							contactTypeModel = new BDContactsTypeModel
							{
								MultiLanguage = list7,
								MOrgID = ctx.MOrgID,
								IsNew = true,
								MItemID = UUIDHelper.GetGuid()
							};
							list7[0].MFieldName = "MName";
							list7[0].MParentID = contactTypeModel.MItemID;
							list5.Add(contactTypeModel);
						}
					}
					BDContactsTypeLinkModel bDContactsTypeLinkModel = modelList.FirstOrDefault((BDContactsTypeLinkModel m) => m.MContactID == item.MItemID && m.MTypeID == contactTypeModel.MItemID);
					if (bDContactsTypeLinkModel != null)
					{
						list6.Add(new BDContactsTypeLinkModel
						{
							MItemID = bDContactsTypeLinkModel.MItemID,
							MContactID = item.MItemID,
							MTypeID = contactTypeModel.MItemID,
							MOrgID = ctx.MOrgID
						});
					}
					else
					{
						list6.Add(new BDContactsTypeLinkModel
						{
							MContactID = item.MItemID,
							MTypeID = contactTypeModel.MItemID,
							MOrgID = ctx.MOrgID
						});
					}
				}
				if (bDContactsModel != null)
				{
					list2.Add(GetDeleteContactslCmd(ctx, bDContactsModel.MItemID));
					List<BDContactslModel> list8 = GetContactslModels(ctx, item.MItemID).ToList();
					if (list8.Count > 0)
					{
						MultiLanguageFieldList multiLanguageFieldList = item.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
						if (multiLanguageFieldList != null)
						{
							List<MultiLanguageField> mMultiLanguageField = multiLanguageFieldList.MMultiLanguageField;
							foreach (MultiLanguageField item2 in mMultiLanguageField)
							{
								BDContactslModel bDContactslModel = list8.FirstOrDefault((BDContactslModel m) => m.MLocaleID == item2.MLocaleID);
								if (bDContactslModel != null)
								{
									item2.MValue = bDContactslModel.MName;
								}
							}
						}
					}
				}
				item.MultiLanguage = (from f in item.MultiLanguage
				where f.MFieldName != "MGroupName"
				select f).ToList();
				MultiLanguageAdd(item);
				int num = 1;
				if (list4?.Any() ?? false)
				{
					foreach (string item3 in list4)
					{
						PropertyInfo property = item.GetType().GetProperty("MTrackHead" + num);
						property.SetValue(item, item3);
						num++;
					}
				}
				list2.AddRange(GetSaveContactTrackCommandList(ctx, item));
				list2.Add(OptLog.GetAddLogCommand(item.IsNew ? OptLogTemplate.Contact_Created : OptLogTemplate.Contact_Edited, ctx, item.MItemID, item.MName));
			}
			list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list, null, true));
			if (list5.Any())
			{
				list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list5, null, true));
			}
			if (list6.Any())
			{
				list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list6, null, true));
			}
			return list2;
		}

		private CommandInfo GetDeleteContactslCmd(MContext ctx, string parentId)
		{
			string commandText = " UPDATE T_BD_Contacts_l  SET MIsDelete = 1 WHERE MParentID = @MParentID AND MOrgID =@MOrgID AND MIsDelete = 0 ";
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = commandText;
			MySqlParameter[] parameters = new MySqlParameter[2]
			{
				new MySqlParameter("@MParentID", parentId),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			DbParameter[] array = commandInfo.Parameters = parameters;
			return commandInfo;
		}

		private List<BDContactslModel> GetContactslModels(MContext ctx, string parentId)
		{
			string sql = string.Format(" SELECT convert(AES_DECRYPT(MName,'{0}') using utf8) AS MName,MLocaleID FROM t_bd_contacts_l WHERE MParentID=@MParentID AND MIsDelete=0 AND MOrgId = @MOrgId ", "JieNor-001");
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MParentID", parentId),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return ModelInfoManager.GetDataModelBySql<BDContactslModel>(ctx, sql, cmdParms);
		}

		private BDContactsTypeModel FindContactTypeFromNewList(List<BDContactsTypeModel> contactTypeList, string name)
		{
			BDContactsTypeModel result = null;
			if (contactTypeList == null || !contactTypeList.Any())
			{
				return result;
			}
			foreach (BDContactsTypeModel contactType in contactTypeList)
			{
				MultiLanguageFieldList multiLanguageFieldList = contactType.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
				IEnumerable<string> source = from f in multiLanguageFieldList.MMultiLanguageField
				select f.MValue;
				if (source.Contains(name))
				{
					result = contactType;
					break;
				}
			}
			return result;
		}

		private List<BDContactsTypeModel> GetContactTypeList(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select a.*,b.MName from T_BD_ContactsType a ");
			stringBuilder.AppendLine(" left join T_BD_ContactsType_L b ");
			stringBuilder.AppendLine(" on a.MItemID=b.MParentID  and b.MIsDelete=0");
			stringBuilder.AppendLine(" where (a.MOrgID=@MOrgID or a.MOrgID='0')  and a.MIsDelete=0");
			return ModelInfoManager.GetDataModelBySql<BDContactsTypeModel>(ctx, stringBuilder.ToString(), ctx.GetParameters((MySqlParameter)null));
		}

		private void SaveContactTrack(MContext ctx, BDContactsInfoModel model, OptLogTemplate logTemplate)
		{
			List<CommandInfo> saveContactTrackCommandList = GetSaveContactTrackCommandList(ctx, model);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTran(saveContactTrackCommandList);
			OptLog.AddLog(logTemplate, ctx, model.MItemID, model.MName);
		}

		public List<CommandInfo> GetSaveContactTrackCommandList(MContext ctx, BDContactsInfoModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			BDContactsTrackLinkRepository trc = new BDContactsTrackLinkRepository();
			ReWriteTrack(trc, list, ctx, model.MTrackHead1, model.MItemID, model.MSalTrackEntry1, model.MPurTrackEntry1);
			ReWriteTrack(trc, list, ctx, model.MTrackHead2, model.MItemID, model.MSalTrackEntry2, model.MPurTrackEntry2);
			ReWriteTrack(trc, list, ctx, model.MTrackHead3, model.MItemID, model.MSalTrackEntry3, model.MPurTrackEntry3);
			ReWriteTrack(trc, list, ctx, model.MTrackHead4, model.MItemID, model.MSalTrackEntry4, model.MPurTrackEntry4);
			ReWriteTrack(trc, list, ctx, model.MTrackHead5, model.MItemID, model.MSalTrackEntry5, model.MPurTrackEntry5);
			return list;
		}

		public List<BDContactsInfoModel> GetContactByIDs(MContext ctx, List<string> ids = null, bool includeDisable = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select distinct t1.*,t3.MItemID as MCCurrentAccountID, convert(AES_DECRYPT(t2.MName,'{0}') using utf8) as MContactName ,t4.MTypeID from T_BD_Contacts t1 ", "JieNor-001");
			stringBuilder.Append(" left join T_BD_Contacts_l t2 on t1.MOrgID = t2.MOrgID and t1.MItemID=t2.MParentID and t2.MLocaleID=@MLocaleID and t2.MIsDelete = 0  ");
			stringBuilder.Append(" left join T_BD_Account t3 on t1.MOrgID = t3.MOrgID and  t3.MCode=t1.MCCurrentAccountCode and t3.MIsDelete = 0 ");
			stringBuilder.Append(" left join T_BD_ContactsTypeLink t4 on t1.MItemID = t4.MContactID and t1.MOrgID=t4.MOrgID and t4.MIsDelete=0 ");
			stringBuilder.Append(" where t1.MIsDelete = 0  and t1.MOrgID=@MOrgID ");
			if (!includeDisable)
			{
				stringBuilder.Append(" AND t1.MIsActive=1 ");
			}
			if (ids != null && ids.Count > 0)
			{
				stringBuilder.Append(" and t1.MItemID in ('" + string.Join("','", ids) + "')");
			}
			stringBuilder.Append("order by convert(MContactName using gbk)");
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
				{
					Value = ctx.MLCID
				}
			};
			DataTable dt = new DynamicDbHelperMySQL(ctx).Query(stringBuilder.ToString(), cmdParms).Tables[0];
			return ModelInfoManager.DataTableToList<BDContactsInfoModel>(dt);
		}

		public List<BDContactsInfoModel> GetContactsListByContactType(MContext ctx, int contactType = 0, string keyWord = null, int top = 0, bool includeDisable = false, bool includeAccount = true)
		{
			string sbSql = GetSbSql(contactType, keyWord, includeDisable, includeAccount);
			sbSql += " order by convert(MContactName using gbk) ";
			if (top > 0)
			{
				sbSql += $" limit 0,{top}";
			}
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
				{
					Value = ctx.MLCID
				},
				new MySqlParameter("@KeyWord", MySqlDbType.VarChar, 36)
				{
					Value = (string.IsNullOrWhiteSpace(keyWord) ? keyWord : keyWord.Replace("\\", "\\\\"))
				}
			};
			DataTable dt = new DynamicDbHelperMySQL(ctx).Query(sbSql.ToString(), cmdParms).Tables[0];
			return ModelInfoManager.DataTableToList<BDContactsInfoModel>(dt);
		}

		public DataGridJson<BDContactsInfoModel> GetContactsListByPage(MContext ctx, BDContactsInfoFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@Keyword", filter.keyword),
				new MySqlParameter("@MModifyDate", filter.MModifyDate),
				new MySqlParameter("@MItemID", filter.ItemID)
			};
			string text = GetContactsSql(filter.keyword, filter.IncludeArchived, true, filter.ItemID);
			if (filter.MModifyDate > DateTime.MinValue)
			{
				text += " AND t1.MModifyDate >@MModifyDate ";
			}
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = text.ToString();
			sqlQuery.OrderBy(" t1.MCreateDate ");
			MySqlParameter[] array2 = array;
			foreach (MySqlParameter para in array2)
			{
				sqlQuery.AddParameter(para);
			}
			return ModelInfoManager.GetPageDataModelListBySql<BDContactsInfoModel>(ctx, sqlQuery);
		}

		private string GetContactsSql(string keyWord, bool includeDisable, bool includeAccount = true, string id = "")
		{
			string text = string.Format("\r\n                    SELECT DISTINCT\r\n                        t1.MItemID,t1.MOrgID,t1.MIsActive,t1.MIsDelete,t1.MCreatorID,t1.MCreateDate,t1.MModifierID,t1.MModifyDate,\r\n                        t1.MAccountID,TRIM(convert(AES_DECRYPT(t1.MEmail,'{0}') using utf8)) AS MEmail ,t1.MPCountryID,t2.MPCityID,t1.MPPostalNo,t1.MRealCountryID,t2.MRealCityID,t1.MRealPostalNo,\r\n                        TRIM(convert(AES_DECRYPT(t1.MPhone,'{0}') using utf8)) AS MPhone,TRIM(convert(AES_DECRYPT(t1.MFax,'{0}') using utf8)) AS MFax,\r\n                        TRIM(convert(AES_DECRYPT(t1.MMobile,'{0}') using utf8)) AS MMobile,TRIM(convert(AES_DECRYPT(t1.MDirectPhone,'{0}') using utf8)) AS MDirectPhone,\r\n                        t1.MQQNo,t1.MWeChatNo,TRIM(convert(AES_DECRYPT(t1.MSkypeName,'{0}') using utf8)) AS MSkypeName,TRIM(convert(AES_DECRYPT(t1.MWebsite,'{0}') using utf8)) AS MWebsite,\r\n                        t1.MOurEmail,TRIM(convert(AES_DECRYPT(t1.MBankAcctNo,'{0}') using utf8)) AS MBankAcctNo,\r\n                        t1.MDefaultCyID,t1.MTaxNo,t1.MSalTaxTypeID,t1.MPurTaxTypeID,t1.MBaseSalary,t1.MDiscount,t1.MPurDueDate,t1.MSalDueDate,\r\n                        t1.MPurDueCondition,t1.MSalDueCondition,t1.MRecAcctID,t1.MPayAcctID,t1.MBorrowAcctID,t1.MNetWorkKey,t1.MIsCustomer,\r\n                        t1.MIsSupplier,t1.MIsOther,t1.MCCurrentAccountCode,t1.MSCurrentAccountCode,t1.MSaleIncomeAccountCode,t2.MBankName,t2.MBankAccName,\r\n                        " + (includeAccount ? " t3.MItemID AS MCCurrentAccountID," : "") + "\r\n                        CONVERT( AES_DECRYPT(t2.MName, '{0}') USING UTF8) AS MName,\r\n                        t2.MFirstName,t2.MLastName,t2.MPStreet,t2.MPRegion,t2.MRealStreet,t2.MRealRegion,t2.MPAttention,t2.MRealAttention,t4.MTypeID\r\n                    FROM\r\n                        T_BD_Contacts t1\r\n                            LEFT JOIN\r\n                        T_BD_Contacts_l t2 ON t1.MItemID = t2.MParentID\r\n                            AND t2.MLocaleID = @MLocaleID\r\n                            AND t2.MOrgID = t1.MOrgID\r\n                            AND t2.MIsDelete = 0 " + (includeAccount ? " \r\n                            LEFT JOIN\r\n                            T_BD_Account t3 ON t3.MCode = t1.MCCurrentAccountCode\r\n                                AND t3.MOrgID = t1.MOrgID\r\n                                AND t3.MIsDelete = 0" : "") + "\r\n                            LEFT JOIN\r\n                        T_BD_ContactsTypeLink t4 ON t1.MItemID = t4.MContactID\r\n                            AND t1.MOrgID = t4.MOrgID\r\n                            AND t4.MIsDelete = 0\r\n                    WHERE\r\n                        t1.MIsDelete = 0 AND t1.MOrgID = @MOrgID ", "JieNor-001");
			if (!includeDisable)
			{
				text += " AND t1.MIsActive=1 ";
			}
			if (!string.IsNullOrWhiteSpace(keyWord))
			{
				text += string.Format(" and( CONVERT(AES_DECRYPT(t2.MName,'{0}') USING utf8) like concat('%',@KeyWord, '%')) ", "JieNor-001");
			}
			if (!string.IsNullOrWhiteSpace(id))
			{
				text += " AND t1.MItemID=@MItemID ";
			}
			return text;
		}

		private string GetSbSql(int contactType, string keyWord, bool includeDisable, bool includeAccount = true)
		{
			string str = "\r\n                    SELECT DISTINCT\r\n                        t1.*," + (includeAccount ? " t3.MItemID AS MCCurrentAccountID," : "") + "\r\n                        CONVERT( AES_DECRYPT(t2.MName, '{0}') USING UTF8) AS MContactName,\r\n                        t4.MTypeID\r\n                    FROM\r\n                        (SELECT\r\n\t\t\t                *\r\n\t\t                 FROM\r\n\t\t\t                T_BD_Contacts\r\n\t\t                 WHERE\r\n\t\t\t                 MOrgID = @MOrgID AND MIsDelete = 0 ";
			switch (contactType)
			{
			case 1:
				str += " and MIsCustomer=1 ";
				break;
			case 2:
				str += " and MIsSupplier=1 ";
				break;
			case 3:
				str += " and MIsCustomer=1 and MIsSupplier=1 ";
				break;
			case 4:
				str += " and MIsOther=1 ";
				break;
			}
			str = str + ") t1\r\n                            LEFT JOIN\r\n                        T_BD_Contacts_l t2 ON t1.MItemID = t2.MParentID\r\n                            AND t2.MLocaleID = @MLocaleID\r\n                            AND t2.MOrgID = t1.MOrgID\r\n                            AND t2.MIsDelete = 0 " + (includeAccount ? " \r\n                            LEFT JOIN\r\n                            T_BD_Account t3 ON t3.MCode = t1.MCCurrentAccountCode\r\n                                AND t3.MOrgID = t1.MOrgID\r\n                                AND t3.MIsDelete = 0" : "") + "\r\n                            LEFT JOIN\r\n                        T_BD_ContactsTypeLink t4 ON t1.MItemID = t4.MContactID\r\n                            AND t1.MOrgID = t4.MOrgID\r\n                            AND t4.MIsDelete = 0\r\n                    WHERE\r\n                        t1.MIsDelete = 0 AND t1.MOrgID = @MOrgID ";
			if (!includeDisable)
			{
				str += " AND t1.MIsActive=1";
			}
			if (!string.IsNullOrWhiteSpace(keyWord))
			{
				str += " and( CONVERT(AES_DECRYPT(t2.MName,'{0}') USING utf8) like concat('%',@KeyWord, '%')) ";
			}
			return string.Format(str, "JieNor-001");
		}

		public List<CommandInfo> UpdateContactMapAccount(MContext ctx, string oldCode, string newCode)
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
			commandInfo.CommandText = "update t_bd_contacts set MSCurrentAccountCode=@newCode where MSCurrentAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete = 0 ";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			CommandInfo commandInfo2 = new CommandInfo();
			commandInfo2.CommandText = "update t_bd_contacts set MCCurrentAccountCode=@newCode where MCCurrentAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete = 0 ";
			array = (commandInfo2.Parameters = parameters);
			list.Add(commandInfo2);
			CommandInfo commandInfo3 = new CommandInfo();
			commandInfo3.CommandText = "update t_bd_contacts set MSaleIncomeAccountCode=@newCode where MSaleIncomeAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete = 0 ";
			array = (commandInfo3.Parameters = parameters);
			list.Add(commandInfo3);
			return list;
		}

		public List<BDContactsModel> GetContactListByNameOrId(MContext ctx, List<string> nameOrIdList, bool includeAll = true, bool includeDisable = false)
		{
			if (!includeAll && !nameOrIdList.Any())
			{
				return new List<BDContactsModel>();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select TRIM(convert(AES_DECRYPT(MName,'{0}') using utf8)) as MName,a.* from t_bd_contacts a \r\n                join t_bd_contacts_l b on a.MItemID=b.MParentID and a.MOrgID = b.MOrgID and b.MIsDelete=0 \r\n                where a.MOrgID=@MOrgID and a.MIsDelete=0", "JieNor-001");
			if (!includeDisable)
			{
				stringBuilder.AppendLine(" AND a.MIsActive=1 ");
			}
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			if (nameOrIdList.Any())
			{
				string whereInSql = base.GetWhereInSql(nameOrIdList, ref parameters, null);
				stringBuilder.AppendFormat(" and (a.MItemID in ({1}) or TRIM(convert(AES_DECRYPT(MName,'{0}') using utf8)) in ({1}))", "JieNor-001", whereInSql);
			}
			stringBuilder.AppendLine(" group by a.MItemID");
			return ModelInfoManager.GetDataModelBySql<BDContactsModel>(ctx, stringBuilder.ToString(), parameters);
		}

		public List<BDContactsModel> GetContactListByName(MContext ctx, List<string> nameList, bool isLike = false)
		{
			if (!nameList.Any())
			{
				return new List<BDContactsModel>();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select a.*,TRIM(convert(AES_DECRYPT(MName,'{0}') using utf8)) as MName from t_bd_contacts a ", "JieNor-001");
			stringBuilder.AppendLine(" join t_bd_contacts_l b on a.MItemID=b.MParentID and a.MOrgID = b.MOrgID and a.MIsDelete = 0 and b.MIsDelete=0 ");
			stringBuilder.AppendLine(" where a.MOrgID=@MOrgID  and a.MIsDelete=0");
			MySqlParameter[] array = ctx.GetParameters((MySqlParameter)null);
			if (isLike && nameList.Count() == 1)
			{
				stringBuilder.AppendFormat(" and convert(AES_DECRYPT(MName,'{0}') using utf8) like concat('%', @MName, '%')");
				array = array.Concat(new MySqlParameter[1]
				{
					new MySqlParameter("@MName", nameList[0])
				}).ToArray();
			}
			else
			{
				string whereInSql = base.GetWhereInSql(nameList, ref array, null);
				stringBuilder.AppendFormat(" and TRIM(convert(AES_DECRYPT(MName,'{0}') using utf8)) in ({1})", "JieNor-001", whereInSql);
			}
			return ModelInfoManager.GetDataModelBySql<BDContactsModel>(ctx, stringBuilder.ToString(), array);
		}

		public static OperationResult AddContactNoteLog(MContext ctx, BDContactsModel model)
		{
			BDContactLogRepository.AddContactNoteLog(ctx, model);
			return new OperationResult
			{
				Success = true
			};
		}

		public BDContactsModel GetContactModelByTaxNo(MContext ctx, string taxNo)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select a.*,TRIM(convert(AES_DECRYPT(MName,'{0}') using utf8)) as MName from t_bd_contacts a ", "JieNor-001");
			stringBuilder.AppendLine(" join t_bd_contacts_l b on a.MItemID=b.MParentID and a.MOrgID = b.MOrgID and a.MIsDelete = 0 and b.MIsDelete=0 ");
			stringBuilder.AppendLine(" where a.MOrgID=@MOrgID  and a.MIsDelete=0 AND MTaxNo=@MTaxNo ");
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MTaxNo", taxNo)
			};
			return ModelInfoManager.GetDataModel<BDContactsModel>(ctx, stringBuilder.ToString(), cmdParms);
		}
	}
}
