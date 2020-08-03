using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDAccountRepository : DataServiceT<BDAccountModel>
	{
		private readonly string multLangFieldSql = "\n                ,t2.MName{0},\n                replace(t2.MFullName{0}, concat(t1.MNumber, ' '), '') as MFullName{0}";

		private readonly string commonSql = "SELECT\r\n                t1.MItemID,\n                t1.MItemID AS MAccountID,\n                t1.MNumber AS MApiCode,\n                t3.MNumber AS MParentNumber,\n                t1.MAccountGroupID,\n                t1.MAccountTypeID,\n                t1.MCode,\n                t1.MDC,\n                t1.MIsCheckForCurrency,\n                t1.MIsSys,\n                t1.MIsActive,\n                t1.MModifyDate,                       \n                t5.MType AS MAccountDimensions_MType,\n                t5.MName AS MAccountDimensions_MName,\n                t5.MInputType AS MAccountDimensions_MInputType,\n                (case  when (t5.MStatus = 1 and t5.MType is not null) then TRUE else null end) AS MAccountDimensions_MStatus,\r\n                t5.MSeq AS MAccountDimensions_MSeq,\r\n\r\n                t2.MName,\n                replace(t2.MFullName, concat(t1.MNumber, ' '), '') as MFullName\r\n                #_#lang_field0#_#\r\n            FROM\n                t_bd_account t1\n                    INNER JOIN\n                @_@t_bd_account_l@_@ t2 ON t1.MOrgID = t2.MOrgID\n                    AND t1.MIsDelete = t2.MIsDelete\n                    AND t1.MItemID = t2.MParentID\n                    LEFT JOIN\n                t_bd_account t3 ON t3.MItemID = t1.MParentID\n                    AND t3.MIsDelete = 0\n                    LEFT JOIN\n                (SELECT\r\n                    *\n                FROM\n                    (SELECT\r\n                    0 AS MSeq,\r\n                    MItemID,\n                        'MASTERDATA' AS MType,\n                        'Contact' AS MName,\n                        (CASE MContactID\n                            WHEN 0 THEN ''\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 2\n                            WHEN 3 THEN 1\n                            WHEN 4 THEN 2\n                            ELSE ''\n                        END) AS MInputType,\n                        (CASE MContactID\n                            WHEN 0 THEN 0\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 1\n                            ELSE 2\n                        END) AS MStatus\n                FROM\n                    t_gl_checkgroup\n                WHERE\n                    MIsDelete = 0 UNION ALL SELECT\r\n                    1 AS MSeq,\r\n                    MItemID,\n                        'MASTERDATA' AS MType,\n                        'Employee' AS MName,\n                        (CASE MEmployeeID\n                            WHEN 0 THEN ''\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 2\n                            WHEN 3 THEN 1\n                            WHEN 4 THEN 2\n                            ELSE ''\n                        END) AS MInputType,\n                        (CASE MEmployeeID\n                            WHEN 0 THEN 0\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 1\n                            ELSE 2\n                        END) AS MStatus\n                FROM\n                    t_gl_checkgroup\n                WHERE\n                    MIsDelete = 0 UNION ALL SELECT\r\n                    2 AS MSeq,\r\n                    MItemID,\n                        'MASTERDATA' AS MType,\n                        'Item' AS MName,\n                        (CASE MMerItemID\n                            WHEN 0 THEN ''\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 2\n                            WHEN 3 THEN 1\n                            WHEN 4 THEN 2\n                            ELSE ''\n                        END) AS MInputType,\n                        (CASE MMerItemID\n                           WHEN 0 THEN 0\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 1\n                            ELSE 2\n                        END) AS MStatus\n                FROM\n                    t_gl_checkgroup\n                WHERE\n                    MIsDelete = 0 UNION ALL SELECT\r\n                    3 AS MSeq,\r\n                    MItemID,\n                        'MASTERDATA' AS MType,\n                        'ExpenseItem' AS MName,\n                        (CASE MExpItemID\n                            WHEN 0 THEN ''\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 2\n                            WHEN 3 THEN 1\n                            WHEN 4 THEN 2\n                            ELSE ''\n                        END) AS MInputType,\n                        (CASE MExpItemID\n                            WHEN 0 THEN 0\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 1\n                            ELSE 2\n                        END) AS MStatus\n                FROM\n                    t_gl_checkgroup\n                WHERE\n                    MIsDelete = 0 UNION ALL SELECT\r\n                    4 AS MSeq,\r\n                    MItemID,\n                        'MASTERDATA' AS MType,\n                        'SalaryItem' AS MName,\n                        (CASE MPaItemID\n                            WHEN 0 THEN ''\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 2\n                            WHEN 3 THEN 1\n                            WHEN 4 THEN 2\n                            ELSE ''\n                        END) AS MInputType,\n                        (CASE MPaItemID\n                            WHEN 0 THEN 0\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 1\n                            ELSE 2\n                        END) AS MStatus\n                FROM\n                    t_gl_checkgroup\n                WHERE\n                    MIsDelete = 0 UNION ALL SELECT\r\n                    5 AS MSeq,\r\n                    MItemID,\n                        'TRACKING' AS MType,\n                        (SELECT\r\n                                t1.MItemID\n                            FROM\n                                T_BD_Track t1\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 order by t1.MCreateDate\n                            LIMIT 0 , 1) AS MName,\n                        (CASE MTrackItem1\n                            WHEN 0 THEN ''\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 2\n                            WHEN 3 THEN 1\n                            WHEN 4 THEN 2\n                            ELSE ''\n                        END) AS MInputType,\n                        (CASE MTrackItem1\n                            WHEN 0 THEN 0\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 1\n                            ELSE 2\n                        END) AS MStatus\n                FROM\n                    t_gl_checkgroup\n                WHERE\n                    MIsDelete = 0 UNION ALL SELECT\r\n                    6 AS MSeq,\r\n                    MItemID,\n                        'TRACKING' AS MType,\n                        (SELECT\r\n                                t1.MItemID\n                            FROM\n                                T_BD_Track t1\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 order by t1.MCreateDate\n                            LIMIT 1 , 1) AS MName,\n                        (CASE MTrackItem2\n                            WHEN 0 THEN ''\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 2\n                            WHEN 3 THEN 1\n                            WHEN 4 THEN 2\n                            ELSE ''\n                        END) AS MInputType,\n                        (CASE MTrackItem2\n                            WHEN 0 THEN 0\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 1\n                            ELSE 2\n                        END) AS MStatus\n                FROM\n                    t_gl_checkgroup\n                WHERE\n                    MIsDelete = 0 UNION ALL SELECT\r\n                    7 AS MSeq,\r\n                    MItemID,\n                        'TRACKING' AS MType,\n                        (SELECT\r\n                                t1.MItemID\n                            FROM\n                                T_BD_Track t1\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 order by t1.MCreateDate\n                            LIMIT 2 , 1) AS MName,\n                        (CASE MTrackItem3\n                            WHEN 0 THEN ''\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 2\n                            WHEN 3 THEN 1\n                            WHEN 4 THEN 2\n                            ELSE ''\n                        END) AS MInputType,\n                        (CASE MTrackItem3\n                            WHEN 0 THEN 0\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 1\n                            ELSE 2\n                        END) AS MStatus\n                FROM\n                    t_gl_checkgroup\n                WHERE\n                    MIsDelete = 0 UNION ALL SELECT\r\n                    8 AS MSeq,\r\n                    MItemID,\n                        'TRACKING' AS MType,\n                        (SELECT\r\n                                t1.MItemID\n                            FROM\n                                T_BD_Track t1\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 order by t1.MCreateDate\n                            LIMIT 3 , 1) AS MName,\n                        (CASE MTrackItem4\n                            WHEN 0 THEN ''\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 2\n                            WHEN 3 THEN 1\n                            WHEN 4 THEN 2\n                            ELSE ''\n                        END) AS MInputType,\n                        (CASE MTrackItem4\n                            WHEN 0 THEN 0\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 1\n                            ELSE 2\n                        END) AS MStatus\n                FROM\n                    t_gl_checkgroup\n                WHERE\n                    MIsDelete = 0 UNION ALL SELECT\r\n                    9 AS MSeq,\r\n                    MItemID,\n                        'TRACKING' AS MType,\n                        (SELECT\r\n                                t1.MItemID\n                            FROM\n                                T_BD_Track t1\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 order by t1.MCreateDate\n                            LIMIT 4 , 1) AS MName,\n                        (CASE MTrackItem5\n                            WHEN 0 THEN ''\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 2\n                            WHEN 3 THEN 1\n                            WHEN 4 THEN 2\n                            ELSE ''\n                        END) AS MInputType,\n                        (CASE MTrackItem5\n                            WHEN 0 THEN 0\n                            WHEN 1 THEN 1\n                            WHEN 2 THEN 1\n                            ELSE 2\n                        END) AS MStatus\n                FROM\n                    t_gl_checkgroup\n                WHERE\n                    MIsDelete = 0) x) t5 ON t5.MItemID = t1.MCheckGroupID and t5.MStatus != 0\n            WHERE\n                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 ";

		public DataGridJson<BDAccountEditModel> Get(MContext ctx, GetParam param)
		{
			param.IncludeDetail = true;
			return new APIDataRepository().Get<BDAccountEditModel>(ctx, param, commonSql, multLangFieldSql, false, true, null);
		}

		public static BDAccountListModel GetAccountByCode(List<BDAccountListModel> accounts, string code)
		{
			return accounts.Find((BDAccountListModel x) => x.MCode != null && x.MCode.Equals(code));
		}

		public static BDAccountListModel GetAccountByID(List<BDAccountListModel> accounts, string id)
		{
			return accounts.Find((BDAccountListModel x) => x.MItemID != null && x.MItemID.Equals(id));
		}

		public List<BDAccountListModel> GetCurrentAccountInfo(MContext ctx, bool withParent = false)
		{
			List<string> values = new List<string>
			{
				"1122",
				"2202",
				"1123",
				"2203",
				"1221",
				"2241"
			};
			string text = "SELECT \r\n                    t1.MItemID,\r\n                    t1.MOrgID,\r\n                    t1.MDC,\r\n                    t1.MNumber,\r\n                    t2.MName,\r\n                    t2.MFullName,\r\n                    t1.MCode,\r\n                    t1.MParentID,\r\n                    sum(t3.MInitBalance) as MInitBalance,\r\n                    sum(t3.MInitBalanceFOR) as MInitBalanceFOR,\r\n                    sum(t3.MYtdDebit) as MYtdDebit,\r\n                    sum(t3.MYtdDebitFor) as MYtdDebitFor,\r\n                    sum(t3.MYtdCredit) as MYtdCredit,\r\n                    sum(t3.MYtdCreditFor) as MYtdCreditFor\r\n                FROM\r\n                    t_bd_account t1\r\n                        INNER JOIN\r\n                    t_bd_account_l t2 ON t1.MItemID = t2.MParentID and t2.MOrgId = t1.MOrgID \r\n                        AND t2.MLocaleID = @MLocaleID and t2.MIsDelete = 0\r\n                        LEFT JOIN\r\n                    t_gl_initbalance t3 ON t3.MAccountID = t1.MItemID and t3.MOrgID = t1.MOrgID\r\n                        AND t3.MContactID = '0' and t3.MIsDelete = 0\r\n                WHERE\r\n\t                t1.MOrgID = @MOrgID\r\n                    and SUBSTRING(t1.MCode, 1, 4) IN ('" + string.Join("','", values) + "')\r\n                    and t1.MIsActive = 1\r\n                    and t1.MIsDelete = 0\r\n                    group by t1.MItemID, t1.MOrgID, t2.MName, t2.MFullName, t1.MCode, t1.MParentID";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MLocaleID",
					Value = ctx.MLCID
				},
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			List<BDAccountListModel> list = ModelInfoManager.DataTableToList<BDAccountListModel>(new DynamicDbHelperMySQL(ctx).Query(text.ToString(), cmdParms).Tables[0]);
			if (!withParent)
			{
				List<BDAccountListModel> listWithNoParent = new List<BDAccountListModel>();
				list.ForEach(delegate(BDAccountListModel x)
				{
					if ((from y in list
					where y.MParentID == x.MItemID
					select y).Count() == 0)
					{
						listWithNoParent.Add(x);
					}
				});
				return listWithNoParent;
			}
			return list;
		}

		public List<BDAccountGroupEditModel> GetBDAccountGroupList(MContext ctx, string strWhere = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT  a.MItemID,a.MNumber,b.MName,b.MDesc ");
			stringBuilder.AppendLine(" FROM T_Bas_AccountGroup  a");
			stringBuilder.AppendLine(" Left Join T_Bas_AccountGroup_L b ON a.MItemID=b.MParentID  And b.MLocaleID=@MLocaleID and b.MIsDelete = 0 ");
			stringBuilder.AppendLine(" WHERE a.MIsDelete = 0  ");
			if (!string.IsNullOrWhiteSpace(strWhere))
			{
				stringBuilder.AppendLine(" AND " + strWhere);
			}
			stringBuilder.AppendLine(" ORDER BY MSeq ");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
			};
			array[0].Value = ctx.MLCID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDAccountGroupEditModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public List<BDAccountTypeListModel> GetBDAccountTypeList(MContext ctx, string strWhere)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT  a.MItemID,a.MAccountGroupID,a.MNumber,b.MName,b.MDesc ,c.MName as MAcctGroupName ");
			stringBuilder.AppendLine(" FROM T_Bas_AccountType  a");
			stringBuilder.AppendLine(" Left Join T_Bas_AccountType_L b ON a.MItemID=b.MParentID  And b.MLocaleID=@MLocaleID and b.MIsDelete = 0  ");
			stringBuilder.AppendLine(" Left Join T_Bas_AccountGroup_L c ON a.MAccountGroupID=c.MParentID And c.MLocaleID=@MLocaleID and c.MIsDelete = 0  ");
			stringBuilder.AppendLine(" WHERE a.MIsDelete = 0  and a.MAccountTableID=@AccountStandard");
			if (!string.IsNullOrWhiteSpace(strWhere))
			{
				stringBuilder.AppendLine(" AND " + strWhere);
			}
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@@AccountStandard", ctx.MAccountTableID)
			};
			array[0].Value = ctx.MLCID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDAccountTypeListModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public List<BDAccountListModel> GetBDAccountList(MContext ctx, string strWhere)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT  a.MItemID,a.MNumber,b.MName,b.MFullName, b.MDesc,a.MAccountGroupID, a.MIsSys,a.MParentID ,a.MDC,a.MIsCheckForCurrency,a.MCode,");
			stringBuilder.AppendLine("  c.MName as MAcctGroupName,a.MAccountTypeID,d.MName as MAcctTypeName,a.IsCanRelateContact ");
			stringBuilder.AppendLine(" FROM T_BD_Account  a");
			stringBuilder.AppendLine(" Inner Join T_BD_Account_l b ON a.MItemID=b.MParentID  AND b.MLocaleID=@MLocaleID And a.MOrgID=b.MOrgID and b.MIsDelete = 0 ");
			stringBuilder.AppendLine(" Left Join T_Bas_AccountGroup_l c ON a.MAccountGroupID=c.MParentID And c.MLocaleID=@MLocaleID and c.MIsDelete = 0  ");
			stringBuilder.AppendLine(" Left Join T_Bas_AccountType_l d ON a.MAccountTypeID=d.MParentID And d.MLocaleID=@MLocaleID and d.MIsDelete = 0  ");
			stringBuilder.AppendLine(" where a.MOrgID = @MOrgID and a.MIsDelete = 0 and a.MIsActive = 1 ");
			if (strWhere.Trim() != "")
			{
				stringBuilder.AppendLine(" AND " + strWhere);
			}
			stringBuilder.AppendLine(" order by MNumber ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDAccountListModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public List<BDAccountListModel> GetBDAccountList(MContext ctx, SqlWhere filter, string keyword = null, bool isAll = true, bool ignoreLocale = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT  a.MItemID,a.MNumber,b.MName,b.MFullName,b.MDesc,a.MAccountGroupID, a.MIsSys,a.MParentID ,a.MDC,a.MIsCheckForCurrency,a.IsCanRelateContact,a.MCheckGroupID,");
			stringBuilder.AppendLine("  cl.MName as MAcctGroupName,a.MAccountTypeID,d.MName as MAcctTypeName ,a.MCode ,a.MIsActive, b.MLocaleID");
			stringBuilder.AppendLine(" FROM T_BD_Account  a");
			stringBuilder.AppendFormat(" Inner Join T_BD_Account_l b ON a.MItemID=b.MParentID and a.MIsDelete=0 {0} And a.MOrgID=b.MOrgID And a.MOrgID=@MOrgID  and b.MIsDelete = 0 ", ignoreLocale ? "" : " AND b.MLocaleID=@MLocaleID");
			bool flag = filter != null && !string.IsNullOrEmpty(filter.WhereSqlString);
			bool flag2 = flag && filter.WhereSqlString.IndexOf("MIsActive", StringComparison.OrdinalIgnoreCase) > -1;
			if (isAll)
			{
				stringBuilder.AppendLine(" Left JOIN t_bas_accountgroup c on c.MItemID = a.MAccountGroupID ");
				stringBuilder.AppendLine(" Left Join T_Bas_AccountGroup_l cl ON c.MItemID=cl.MParentID And cl.MLocaleID=@MLocaleID ");
			}
			else
			{
				stringBuilder.AppendLine(" Left JOIN t_bas_accountgroup c on c.MItemID = a.MAccountGroupID or LOCATE(c.MItemID, a.MAccountGroupID) > 0");
				stringBuilder.AppendLine(" Left Join T_Bas_AccountGroup_l cl ON c.MItemID=cl.MParentID And cl.MLocaleID=@MLocaleID");
			}
			stringBuilder.AppendLine(" Left Join T_Bas_AccountType_l d ON a.MAccountTypeID=d.MParentID And d.MLocaleID=@MLocaleID");
			if (flag)
			{
				stringBuilder.AppendLine(filter.WhereSqlString);
				if (!flag2)
				{
					stringBuilder.AppendLine(" and a.MIsActive=1");
				}
				if (!string.IsNullOrEmpty(keyword))
				{
					stringBuilder.AppendFormat(" and (a.mnumber like '%{0}%' or b.MName like '%{0}%' or cl.MName like '%{0}%')", keyword);
				}
			}
			else if (!string.IsNullOrEmpty(keyword))
			{
				stringBuilder.AppendFormat(" where a.MOrgID = @MOrgID AND a.MIsActive = 1 and a.MIsDelete = 0 and a.mnumber like '%{0}%' or b.MName like '%{0}%' or cl.MName like '%{0}%' ", keyword);
			}
			stringBuilder.Append("  order by MNumber asc , MCode  asc ");
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6));
			arrayList.Add(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36));
			if (filter != null && filter.Parameters.Length != 0)
			{
				MySqlParameter[] parameters = filter.Parameters;
				foreach (MySqlParameter value in parameters)
				{
					arrayList.Add(value);
				}
			}
			MySqlParameter[] array = (MySqlParameter[])arrayList.ToArray(typeof(MySqlParameter));
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDAccountListModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public List<BDAccountModel> GetBaseBDAccountList(MContext ctx, SqlWhere filter, bool includeForbidden = false, List<string> bankIdList = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" SELECT * FROM  (SELECT a.*,b.MName,b.MFullName FROM t_bd_account a ");
			if (includeForbidden)
			{
				stringBuilder.AppendLine(" Inner JOIN t_bd_account_l b on a.MItemID = b.MParentID AND b.MLocaleID=@MLocaleID and a.MOrgID=@MOrgID and b.MOrgID = @MOrgID  and a.MIsDelete=0 and b.MIsDelete = 0 ");
			}
			else
			{
				stringBuilder.AppendLine(" Inner JOIN t_bd_account_l b on a.MItemID = b.MParentID AND b.MLocaleID=@MLocaleID and a.MOrgID=@MOrgID and b.MOrgID = @MOrgID and a.MIsDelete=0 and b.MIsDelete = 0 and a.MIsActive=1 ");
			}
			if (filter != null && !string.IsNullOrEmpty(filter.WhereSqlString))
			{
				stringBuilder.AppendLine(filter.WhereSqlString);
			}
			if (bankIdList?.Any() ?? false)
			{
				stringBuilder.AppendLine(" union all ");
				stringBuilder.AppendLine(" SELECT a.*,b.MName,b.MFullName FROM t_bd_account a");
				stringBuilder.AppendFormat(" Inner JOIN t_bd_account_l b on a.MItemID = b.MParentID AND b.MLocaleID=@MLocaleID and a.MOrgID=@MOrgID and b.MOrgID = @MOrgID and a.MItemID in ('{0}')", string.Join("','", bankIdList));
			}
			stringBuilder.AppendLine(") u");
			stringBuilder.AppendLine(" order by MNumber asc , MCode  asc ");
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6));
			arrayList.Add(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36));
			if (filter != null && filter.Parameters.Length != 0)
			{
				MySqlParameter[] parameters = filter.Parameters;
				foreach (MySqlParameter value in parameters)
				{
					arrayList.Add(value);
				}
			}
			MySqlParameter[] array = (MySqlParameter[])arrayList.ToArray(typeof(MySqlParameter));
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDAccountModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public List<BDAccountModel> GetInitBDAccountList(MContext ctx, SqlWhere filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" SELECT a.*,b.MName,b.MFullName FROM t_bd_account a ");
			stringBuilder.AppendLine(" Left JOIN t_bd_account_l b on a.MItemID = b.MParentID AND b.MLocaleID=@MLocaleID and a.MOrgID=@MOrgID and a.MIsActive=1 and a.MIsDelete=0 and b.MOrgID = @MOrgID and  b.MIsDelete = 0 ");
			if (filter != null && !string.IsNullOrEmpty(filter.WhereSqlString))
			{
				stringBuilder.AppendLine(filter.WhereSqlString);
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6));
			arrayList.Add(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36));
			if (filter != null && filter.Parameters.Length != 0)
			{
				MySqlParameter[] parameters = filter.Parameters;
				foreach (MySqlParameter value in parameters)
				{
					arrayList.Add(value);
				}
			}
			MySqlParameter[] array = (MySqlParameter[])arrayList.ToArray(typeof(MySqlParameter));
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDAccountModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public OperationResult UpdateAccount(MContext ctx, BDAccountEditModel model)
		{
			List<CommandInfo> updateAccountCmds = GetUpdateAccountCmds(ctx, model);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(updateAccountCmds);
			if (num > 0)
			{
				return new OperationResult
				{
					Success = true
				};
			}
			return new OperationResult
			{
				Success = false,
				Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ErrorSaveFailed", "Error, save failed!")
			};
		}

		public List<CommandInfo> GetUpdateAccountCmds(MContext ctx, BDAccountEditModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			model.MOrgID = ctx.MOrgID;
			if (string.IsNullOrWhiteSpace(model.MItemID))
			{
				list.AddRange(GetAccountInsertSql(ctx, model));
			}
			else
			{
				list.AddRange(GetAccountUpdateSql(ctx, model));
			}
			return list;
		}

		public bool IsAccountEditCodeExists(MContext ctx, BDAccountEditModel model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT 1 FROM T_BD_Account where MNumber=@MNumber ");
			stringBuilder.AppendLine(" and MOrgID=@MOrgID and MIsDelete = 0 and MIsActive = 1 ");
			if (!string.IsNullOrEmpty(model.MItemID))
			{
				stringBuilder.Append("and MItemID<>@MItemID ");
			}
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MNumber", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = model.MNumber;
			array[1].Value = ctx.MOrgID;
			array[2].Value = model.MItemID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(stringBuilder.ToString(), array);
		}

		private List<CommandInfo> GetAccountUpdateSql(MContext ctx, BDAccountEditModel model)
		{
			model.MOrgID = ctx.MOrgID;
			if (model.MBankAccountType == 1)
			{
				List<CommandInfo> list = new List<CommandInfo>();
				BDBankAccountEditModel bDBankAccountEditModel = CopyBDBankAccountModel(model);
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDBankAccountEditModel>(ctx, bDBankAccountEditModel, null, true));
				model.MultiLanguage = bDBankAccountEditModel.MultiLanguage;
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountEditModel>(ctx, model, null, true));
				return list;
			}
			return ModelInfoManager.GetInsertOrUpdateCmd<BDAccountEditModel>(ctx, model, null, true);
		}

		private List<CommandInfo> GetAccountInsertSql(MContext ctx, BDAccountEditModel model)
		{
			model.MOrgID = ctx.MOrgID;
			model.MIsActive = true;
			if (model.MBankAccountType == 1 || model.MBankAccountType == 3)
			{
				List<CommandInfo> list = new List<CommandInfo>();
				List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BDAccountEditModel>(ctx, model, null, true);
				BDBankAccountEditModel bDBankAccountEditModel = CopyBDBankAccountModel(model);
				bDBankAccountEditModel.MItemID = model.MItemID;
				bDBankAccountEditModel.IsNew = true;
				List<CommandInfo> insertOrUpdateCmd2 = ModelInfoManager.GetInsertOrUpdateCmd<BDBankAccountEditModel>(ctx, bDBankAccountEditModel, null, true);
				string filedName = "@MPKID";
				for (int i = 1; i < insertOrUpdateCmd2.Count && i < insertOrUpdateCmd.Count; i++)
				{
					object value = (from x in insertOrUpdateCmd[i].Parameters
					where x.ParameterName.ToUpper().Equals(filedName)
					select x).First().Value;
					(from x in insertOrUpdateCmd2[i].Parameters
					where x.ParameterName.ToUpper().Equals(filedName)
					select x).First().Value = value;
				}
				list.AddRange(insertOrUpdateCmd);
				list.AddRange(insertOrUpdateCmd2);
				return list;
			}
			return ModelInfoManager.GetInsertOrUpdateCmd<BDAccountEditModel>(ctx, model, null, true);
		}

		private BDBankAccountEditModel CopyBDBankAccountModel(BDAccountEditModel model)
		{
			BDBankAccountEditModel bDBankAccountEditModel = new BDBankAccountEditModel();
			bDBankAccountEditModel.MItemID = model.MItemID;
			bDBankAccountEditModel.MOrgID = model.MOrgID;
			bDBankAccountEditModel.MIsActive = model.MIsActive;
			bDBankAccountEditModel.MNumber = model.MNumber;
			bDBankAccountEditModel.MultiLanguage = model.MultiLanguage;
			return bDBankAccountEditModel;
		}

		public OperationResult UpdateAccountType(MContext ctx, BDAccountTypeEditModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (string.IsNullOrWhiteSpace(model.MItemID))
			{
				list.AddRange(GetAccountTypeInsertSql(ctx, model));
			}
			else
			{
				list.AddRange(GetAccountTypeUpdateSql(ctx, model));
			}
			string text = DynamicPubConstant.ConnectionString(ctx.MOrgID);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			OperationResult operationResult = new OperationResult();
			if (num > 0)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Success", "Success");
				operationResult.SuccessModelID = new List<string>
				{
					model.MItemID
				};
			}
			else
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Fail", "Fail");
				operationResult.FailModelID = new List<string>
				{
					model.MItemID
				};
			}
			return operationResult;
		}

		private List<CommandInfo> GetAccountTypeUpdateSql(MContext ctx, BDAccountTypeEditModel model)
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BDAccountTypeEditModel>(ctx, model, null, true);
		}

		private List<CommandInfo> GetAccountTypeInsertSql(MContext ctx, BDAccountTypeEditModel model)
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BDAccountTypeEditModel>(ctx, model, null, true);
		}

		public BDAccountEditModel GetBDAccountEditModel(MContext ctx, string pkID)
		{
			BDAccountEditModel bDAccountEditModel = ModelInfoManager.GetDataEditModel<BDAccountEditModel>(ctx, pkID, false, true);
			if (bDAccountEditModel == null)
			{
				bDAccountEditModel = new BDAccountEditModel();
			}
			return bDAccountEditModel;
		}

		public List<BDAccountEditModel> GetAccountEditModelList(MContext ctx, SqlWhere sqlwhere, bool isPage = false)
		{
			return ModelInfoManager.GetDataModelList<BDAccountEditModel>(ctx, sqlwhere, isPage, false);
		}

		public BDAccountModel GetBDAccountModel(MContext ctx, string code)
		{
			return base.GetDataModelByFilter(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).Equal("MCode", code));
		}

		public BDBankAccountEditModel GetBDBankAccountEditModel(MContext ctx, string pkID)
		{
			BDBankAccountEditModel bDBankAccountEditModel = ModelInfoManager.GetDataEditModel<BDBankAccountEditModel>(ctx, pkID, false, true);
			if (bDBankAccountEditModel == null)
			{
				bDBankAccountEditModel = new BDBankAccountEditModel();
			}
			return bDBankAccountEditModel;
		}

		public JieNor.Megi.DataModel.BD.BDAccountViewModel GetBDAccountViewModel(MContext ctx, string pkID)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT a.*,b.MName,b.MFullName, ,c.MCyID, convert(AES_DECRYPT(c.MBankNo,'{0}') using utf8) AS MBankNo FROM T_BD_Account a ", "JieNor-001");
			stringBuilder.AppendLine(" LEFT JOIN T_BD_Account_l b ON a.MItemID=b.MParentID AND b.MLocaleID=@MLocaleID and b.MOrgId = @MOrgID and b.MIsDelete = 0 \r\n                            inner join t_bd_bankaccount c on c.MAccountID = a.MItemID and c.MOrgID = a.MOrgID and c.MIsDelete = 0 \r\n                            WHERE a.MItemID = @MItemID a.MOrgID = @MOrgID and a.MIsDelete = 0 and a.MIsActive = 1 ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
			};
			array[0].Value = pkID;
			array[1].Value = ctx.MLCID;
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			JieNor.Megi.DataModel.BD.BDAccountViewModel result = new JieNor.Megi.DataModel.BD.BDAccountViewModel();
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return result;
			}
			List<JieNor.Megi.DataModel.BD.BDAccountViewModel> list = ModelInfoManager.DataTableToList<JieNor.Megi.DataModel.BD.BDAccountViewModel>(dataSet.Tables[0]);
			if (list != null && list.Count > 0)
			{
				result = list[0];
			}
			return result;
		}

		public BDAccountTypeEditModel GetBDAccountTypeEditModel(MContext ctx, string pkID)
		{
			return ModelInfoManager.GetDataEditModel<BDAccountTypeEditModel>(ctx, pkID, false, true);
		}

		public OperationResult DeleteAccount(MContext ctx, ParamBase param)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> pkIDS = param.KeyIDs.Split(',').ToList();
			list.AddRange(ModelInfoManager.GetDeleteCmd<BDBankAccountEditModel>(ctx, pkIDS));
			list.AddRange(ModelInfoManager.GetDeleteCmd<BDAccountEditModel>(ctx, pkIDS));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num > 0)
			{
				return new OperationResult
				{
					Success = true
				};
			}
			return new OperationResult
			{
				Success = false
			};
		}

		public List<CommandInfo> GetDeleteAccountCmds(MContext ctx, ParamBase param)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> pkIDS = param.KeyIDs.Split(',').ToList();
			list.AddRange(ModelInfoManager.GetDeleteCmd<BDBankAccountEditModel>(ctx, pkIDS));
			list.AddRange(ModelInfoManager.GetDeleteCmd<BDAccountEditModel>(ctx, pkIDS));
			return list;
		}

		public OperationResult DeleteAccount(MContext ctx, string pkID)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(ModelInfoManager.GetDeleteCmd<BDBankAccountEditModel>(ctx, pkID));
			list.AddRange(ModelInfoManager.GetDeleteCmd<BDAccountEditModel>(ctx, pkID));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num > 0)
			{
				return new OperationResult
				{
					Success = true
				};
			}
			return new OperationResult
			{
				Success = false
			};
		}

		public OperationResult ArchiveAccount(MContext ctx, string pkID)
		{
			return ModelInfoManager.ArchiveFlag<BDAccountEditModel>(ctx, pkID);
		}

		public OperationResult ArchiveAccount(MContext ctx, ParamBase param)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> pkIDS = param.KeyIDs.Split(',').ToList();
			list.AddRange(ModelInfoManager.GetArchiveFlagCmd<BDAccountEditModel>(ctx, pkIDS));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num > 0)
			{
				return new OperationResult
				{
					Success = true
				};
			}
			return new OperationResult
			{
				Success = false
			};
		}

		public OperationResult UnArchiveAccount(MContext ctx, ParamBase param)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> list2 = param.KeyIDs.Split(',').ToList();
			foreach (string item in list2)
			{
				CommandInfo commandInfo = new CommandInfo();
				commandInfo.CommandText = "update t_bd_account a, t_bd_account_l b  set a.MIsActive=1  where a.MOrgID=@MOrgID and a.MItemID=@MItemID and a.MIsDelete = 0 and a.MIsActive = 0 and b.MOrgID=@MOrgID and b.MParentid=@MItemID and b.MIsDelete = 0 ";
				DbParameter[] array = commandInfo.Parameters = new MySqlParameter[2]
				{
					new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
					new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
				};
				commandInfo.Parameters[0].Value = ctx.MOrgID;
				commandInfo.Parameters[1].Value = item;
				list.Add(commandInfo);
				CommandInfo commandInfo2 = new CommandInfo();
				commandInfo2.CommandText = "update t_bd_bankaccount a , t_bd_bankaccount_l b  set a.MIsActive=1   where a.MOrgID=@MOrgID and a.MAccountID = @MItemID and a.MIsActive=0 and b.MOrgID=@MOrgID and b.MParentID = @MItemID ";
				array = (commandInfo2.Parameters = new MySqlParameter[2]
				{
					new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
					new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
				});
				commandInfo2.Parameters[0].Value = ctx.MOrgID;
				commandInfo2.Parameters[1].Value = item;
				list.Add(commandInfo2);
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num > 0)
			{
				return new OperationResult
				{
					Success = true
				};
			}
			return new OperationResult
			{
				Success = false
			};
		}

		public bool IsCodeExists(string id, MContext ctx, BDAccountTypeEditModel model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT * FROM T_BD_Account where MNumber=@MNumber ");
			stringBuilder.AppendLine(" and MOrgID=@MOrgID and (IFNULL(@MItemID,'')='' OR MItemID<>@MItemID) and MIsDelete = 0 and MIsActive = 1 ");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MNumber", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = model.MNumber;
			array[1].Value = ctx.MOrgID;
			array[2].Value = id;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(stringBuilder.ToString(), array);
		}

		public static List<GLBalanceModel> GetAccountBalance(MContext ctx, string itemid)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" SELECT MBankID AS MItemID,SUM(ifnull(MTaxTotalAmtFor,0)) AS MSystemBalance FROM ");
			stringBuilder.AppendLine(" (SELECT MBankID, ");
			stringBuilder.AppendLine(" CASE  MType ");
			stringBuilder.AppendLine(" WHEN @Receive_Sale OR @Receive_Other OR @Receive_Prepare THEN ifnull(MTaxTotalAmtFor,0) ");
			stringBuilder.AppendLine(" WHEN @Receive_SaleReturn OR @Receive_OtherReturn THEN -1 * ifnull(MTaxTotalAmtFor,0) ");
			stringBuilder.AppendLine(" ELSE ifnull(MTaxTotalAmtFor,0) END MTaxTotalAmtFor ");
			stringBuilder.AppendLine(" FROM T_IV_Receive WHERE MIsDelete=0 AND MOrgID=@MOrgID  ");
			stringBuilder.AppendLine(" UNION ALL ");
			stringBuilder.AppendLine(" SELECT MBankID, ");
			stringBuilder.AppendLine(" CASE  MType ");
			stringBuilder.AppendLine(" WHEN @Pay_Purchase OR @Pay_Other OR @Pay_Prepare THEN ifnull(MTaxTotalAmtFor,0) ");
			stringBuilder.AppendLine(" WHEN @Pay_PurReturn OR @Pay_OtherReturn THEN -1 * ifnull(MTaxTotalAmtFor,0) ");
			stringBuilder.AppendLine(" ELSE ifnull(MTaxTotalAmtFor,0) END MTaxTotalAmtFor ");
			stringBuilder.AppendLine(" FROM T_IV_Payment WHERE MIsDelete=0 AND MOrgID=@MOrgID  ");
			stringBuilder.AppendLine(" )T  ");
			if (itemid.Trim() != "")
			{
				stringBuilder.AppendLine(" WHERE MBankID=@itemid ");
			}
			stringBuilder.AppendLine(" GROUP BY MBankID ");
			MySqlParameter[] array = new MySqlParameter[12]
			{
				new MySqlParameter("@itemid", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Receive_Sale", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Receive_Other", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Receive_Prepare", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Receive_SaleReturn", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Receive_OtherReturn", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Pay_Purchase", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Pay_Other", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Pay_Prepare", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Pay_PurReturn", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Pay_OtherReturn", MySqlDbType.VarChar, 36)
			};
			array[0].Value = itemid;
			array[1].Value = ctx.MOrgID;
			array[2].Value = "Receive_Sale";
			array[3].Value = "Receive_Other";
			array[4].Value = "Receive_Prepare";
			array[5].Value = "Receive_SaleReturn";
			array[6].Value = "Receive_OtherReturn";
			array[7].Value = "Pay_Purchase";
			array[8].Value = "Pay_Other";
			array[9].Value = "Pay_Prepare";
			array[10].Value = "Pay_PurReturn";
			array[11].Value = "Pay_OtherReturn";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			return ModelInfoManager.DataTableToList<GLBalanceModel>(dataSet.Tables[0]);
		}

		public static List<GLInitBalanceModel> GetInitBalanceList(MContext ctx, SqlWhere filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select a.*,bl.MName, bl.MFullName , b.MDC , b.MParentID,convert(AES_DECRYPT(cl.MName ,'{0}') using utf8) as MContactName  from T_GL_INITBALANCE a", "JieNor-001");
			stringBuilder.AppendLine(" inner join t_bd_account b on  a.MAccountID = b.MItemID and a.MOrgID=@MOrgID and b.MOrgID=@MOrgID and b.MIsDelete = 0 and b.MIsActive = 1 ");
			stringBuilder.AppendLine(" inner join t_bd_account_l bl on  b.MItemID = bl.MParentID and bl.MLocaleID=@MLocaleID and bl.MOrgID = @MOrgID  and bl.MIsDelete = 0  ");
			stringBuilder.AppendLine(" Left join t_bd_contacts_l cl on a.MContactID=cl.MParentID and cl.MLocaleID=@MLocaleID and cl.MOrgID = @MOrgID  and cl.MIsDelete = 0  ");
			stringBuilder.AppendLine(" where a.MIsDelete = 0 ");
			if (filter != null && !string.IsNullOrEmpty(filter.FilterString))
			{
				stringBuilder.Append(" and " + filter.FilterString);
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36));
			arrayList.Add(new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6));
			if (filter != null && filter.Parameters.Length != 0)
			{
				MySqlParameter[] parameters = filter.Parameters;
				foreach (MySqlParameter value in parameters)
				{
					arrayList.Add(value);
				}
			}
			MySqlParameter[] array = (MySqlParameter[])arrayList.ToArray(typeof(MySqlParameter));
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<GLInitBalanceModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public static List<GLInitBalanceModel> GetDetailInitBalanceList(MContext ctx, SqlWhere filter)
		{
			return new List<GLInitBalanceModel>();
		}

		public static DataGridJson<GLInitBalanceModel> GetBanlInitBalanceListByPage(MContext ctx, GLBalanceListFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			StringBuilder listSqlBuilder = GetListSqlBuilder(ctx, filter);
			MySqlParameter[] listParameters = GetListParameters(ctx, filter.searchFilter);
			sqlQuery.SqlWhere = filter;
			if (!string.IsNullOrEmpty(filter.Sort))
			{
				sqlQuery.SqlWhere.OrderBy($" v1.{filter.Sort} {filter.Order}");
			}
			sqlQuery.SelectString = listSqlBuilder.ToString();
			MySqlParameter[] array = listParameters;
			foreach (MySqlParameter para in array)
			{
				sqlQuery.AddParameter(para);
			}
			return ModelInfoManager.GetPageDataModelListBySql<GLInitBalanceModel>(ctx, sqlQuery);
		}

		private static StringBuilder GetListSqlBuilder(MContext ctx, GLBalanceListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select * from (");
			stringBuilder.AppendFormat(" select t1.MItemID as MAccountID, t1.MCyID,t2.MName,t3.MInitBalanceFor,  convert(AES_DECRYPT(t1.MBankNo ,'{0}') using utf8)  as MBankNo ,t4.MName as MBankTypeName,t1.MCreateDate ", "JieNor-001");
			stringBuilder.AppendLine(" from T_BD_BankAccount t1");
			stringBuilder.AppendLine(" join T_BD_BankAccount_L t2 on t1.MItemID=t2.MParentID and t1.MOrgID=2.MOrgID and t2.MLocaleID=@MLocaleID  and t2.MIsDelete = 0  ");
			stringBuilder.AppendLine(" left join T_GL_INITBALANCE t3 on t1.MItemID=t3.MACCOUNTID and t3.MOrgID=t1.MOrgID  and t3.MIsDelete = 0  ");
			stringBuilder.AppendLine(" left join T_BD_BankType_L t4 on t4.MParentID=t1.MBankTypeID and t4.MLocaleID=@MLocaleID ");
			stringBuilder.AppendLine(" where t1.MOrgID = @MOrgID and t1.MIsDelete = 0 ");
			stringBuilder.AppendLine(" ) v1 ");
			return stringBuilder;
		}

		private static MySqlParameter[] GetListParameters(MContext ctx, string searchFilter)
		{
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@searchFilter", MySqlDbType.VarChar, 6)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = searchFilter;
			return array;
		}

		private static string GetFilterWhere(string searchFilter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(searchFilter) && !searchFilter.Equals("0") && !searchFilter.Equals("ALL"))
			{
				if (searchFilter.Equals("123"))
				{
					stringBuilder.AppendLine(" AND (t2.MFirstName REGEXP '^[0-9]' or t2.MLastName REGEXP '^[0-9]') ");
				}
				else
				{
					stringBuilder.AppendLine(" AND (t2.MFirstName like concat('%', @searchFilter, '%') or t2.MLastName like concat('%', @searchFilter, '%')) ");
				}
			}
			return stringBuilder.ToString();
		}

		public static List<CommandInfo> GetUpdateInitBalanceCmds(MContext ctx, GLInitBalanceModel model, List<BDAccountModel> accountList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
			if (!string.IsNullOrEmpty(model.MContactID) && model.MContactID != "0")
			{
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MContactID", model.MContactID);
				sqlWhere.Equal("MAccountID", model.MAccountID);
				sqlWhere.Equal("MCurrencyID", model.MCurrencyID);
				GLInitBalanceModel gLInitBalanceModel = gLInitBalanceRepository.GetDataModelByFilter(ctx, sqlWhere);
				GLInitBalanceModel gLInitBalanceModel2 = null;
				if (gLInitBalanceModel == null)
				{
					gLInitBalanceModel = new GLInitBalanceModel();
					gLInitBalanceModel.MContactID = model.MContactID;
					gLInitBalanceModel.MAccountID = model.MAccountID;
					gLInitBalanceModel.MCurrencyID = model.MCurrencyID;
					gLInitBalanceModel.MOrgID = ctx.MOrgID;
					gLInitBalanceModel.MInitBalance = Math.Round(model.MInitBalance, 2);
					gLInitBalanceModel.MInitBalanceFor = Math.Round(model.MInitBalanceFor, 2);
					gLInitBalanceModel.MYtdDebit = Math.Round(model.MYtdDebit, 2);
					gLInitBalanceModel.MYtdCredit = Math.Round(model.MYtdCredit, 2);
					gLInitBalanceModel.MYtdDebitFor = Math.Round(model.MYtdDebitFor, 2);
					gLInitBalanceModel.MYtdCreditFor = Math.Round(model.MYtdCreditFor, 2);
				}
				else
				{
					gLInitBalanceModel2 = gLInitBalanceRepository.GetDataModelByFilter(ctx, sqlWhere);
					gLInitBalanceModel.MOrgID = ctx.MOrgID;
					gLInitBalanceModel.MInitBalance = model.MInitBalance;
					gLInitBalanceModel.MInitBalanceFor = model.MInitBalanceFor;
					gLInitBalanceModel.MYtdDebit = model.MYtdDebit;
					gLInitBalanceModel.MYtdCredit = model.MYtdCredit;
					gLInitBalanceModel.MYtdDebitFor = model.MYtdDebitFor;
					gLInitBalanceModel.MYtdCreditFor = model.MYtdCreditFor;
				}
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLInitBalanceModel>(ctx, gLInitBalanceModel, null, true));
				SqlWhere sqlWhere2 = new SqlWhere();
				sqlWhere2.Equal("MAccountID", model.MAccountID);
				sqlWhere2.Equal("MCurrencyID", model.MCurrencyID);
				sqlWhere2.Equal("MContactID", "0");
				model = gLInitBalanceRepository.GetInitBalanceModel(ctx, sqlWhere2);
				if (model == null)
				{
					//model = model;
					model.MContactID = "0";
				}
				else
				{
					if (gLInitBalanceModel2 != null)
					{
						GLInitBalanceModel gLInitBalanceModel3 = model;
						gLInitBalanceModel3.MInitBalanceFor -= gLInitBalanceModel2.MInitBalanceFor;
						GLInitBalanceModel gLInitBalanceModel4 = model;
						gLInitBalanceModel4.MInitBalance -= gLInitBalanceModel2.MInitBalance;
						GLInitBalanceModel gLInitBalanceModel5 = model;
						gLInitBalanceModel5.MYtdDebit -= gLInitBalanceModel2.MYtdDebit;
						GLInitBalanceModel gLInitBalanceModel6 = model;
						gLInitBalanceModel6.MYtdCredit -= gLInitBalanceModel2.MYtdCredit;
						GLInitBalanceModel gLInitBalanceModel7 = model;
						gLInitBalanceModel7.MYtdDebitFor -= gLInitBalanceModel2.MYtdDebitFor;
						GLInitBalanceModel gLInitBalanceModel8 = model;
						gLInitBalanceModel8.MYtdCreditFor -= gLInitBalanceModel2.MYtdCreditFor;
					}
					GLInitBalanceModel gLInitBalanceModel9 = model;
					gLInitBalanceModel9.MInitBalance += gLInitBalanceModel.MInitBalance;
					GLInitBalanceModel gLInitBalanceModel10 = model;
					gLInitBalanceModel10.MInitBalanceFor += gLInitBalanceModel.MInitBalanceFor;
					GLInitBalanceModel gLInitBalanceModel11 = model;
					gLInitBalanceModel11.MYtdDebit += gLInitBalanceModel.MYtdDebit;
					GLInitBalanceModel gLInitBalanceModel12 = model;
					gLInitBalanceModel12.MYtdCredit += gLInitBalanceModel.MYtdCredit;
					GLInitBalanceModel gLInitBalanceModel13 = model;
					gLInitBalanceModel13.MYtdDebitFor += gLInitBalanceModel.MYtdDebitFor;
					GLInitBalanceModel gLInitBalanceModel14 = model;
					gLInitBalanceModel14.MYtdCreditFor += gLInitBalanceModel.MYtdCreditFor;
				}
			}
			List<GLInitBalanceModel> list2 = new List<GLInitBalanceModel>();
			BDAccountModel bDAccountModel = (from x in accountList
			where x.MItemID == model.MAccountID
			select x).First();
			List<string> accountParentIdByRecursion = GetAccountParentIdByRecursion(ctx, model.MAccountID, accountList);
			foreach (string item in accountParentIdByRecursion)
			{
				SqlWhere sqlWhere3 = new SqlWhere();
				sqlWhere3.Equal("MAccountID", item);
				sqlWhere3.Equal("MCurrencyID", model.MCurrencyID);
				sqlWhere3.Equal("MContactID", "0");
				GLInitBalanceModel gLInitBalanceModel15 = gLInitBalanceRepository.GetInitBalanceModel(ctx, sqlWhere3);
				BDAccountModel bDAccountModel2 = (from x in accountList
				where x.MItemID == item
				select x).First();
				if (gLInitBalanceModel15 == null)
				{
					gLInitBalanceModel15 = new GLInitBalanceModel();
					gLInitBalanceModel15.MAccountID = item;
					gLInitBalanceModel15.MCurrencyID = model.MCurrencyID;
					gLInitBalanceModel15.MOrgID = ctx.MOrgID;
				}
				GLInitBalanceModel initBalanceModel = gLInitBalanceRepository.GetInitBalanceModel(ctx, new SqlWhere().Equal("a.MItemID", model.MItemID));
				if (initBalanceModel != null)
				{
					model.MDC = ((model.MDC == 0) ? initBalanceModel.MDC : model.MDC);
				}
				int value = (bDAccountModel.MDC == bDAccountModel2.MDC) ? 1 : (-1);
				if (initBalanceModel != null)
				{
					GLInitBalanceModel gLInitBalanceModel16 = gLInitBalanceModel15;
					gLInitBalanceModel16.MInitBalance -= initBalanceModel.MInitBalance * (decimal)value;
					GLInitBalanceModel gLInitBalanceModel17 = gLInitBalanceModel15;
					gLInitBalanceModel17.MInitBalanceFor -= initBalanceModel.MInitBalanceFor * (decimal)value;
					GLInitBalanceModel gLInitBalanceModel18 = gLInitBalanceModel15;
					gLInitBalanceModel18.MYtdDebit -= initBalanceModel.MYtdDebit;
					GLInitBalanceModel gLInitBalanceModel19 = gLInitBalanceModel15;
					gLInitBalanceModel19.MYtdCredit -= initBalanceModel.MYtdCredit;
					GLInitBalanceModel gLInitBalanceModel20 = gLInitBalanceModel15;
					gLInitBalanceModel20.MYtdDebitFor -= initBalanceModel.MYtdDebitFor;
					GLInitBalanceModel gLInitBalanceModel21 = gLInitBalanceModel15;
					gLInitBalanceModel21.MYtdCreditFor -= initBalanceModel.MYtdCreditFor;
				}
				GLInitBalanceModel gLInitBalanceModel22 = gLInitBalanceModel15;
				gLInitBalanceModel22.MInitBalance += model.MInitBalance * (decimal)value;
				GLInitBalanceModel gLInitBalanceModel23 = gLInitBalanceModel15;
				gLInitBalanceModel23.MInitBalanceFor += model.MInitBalanceFor * (decimal)value;
				GLInitBalanceModel gLInitBalanceModel24 = gLInitBalanceModel15;
				gLInitBalanceModel24.MYtdDebit += model.MYtdDebit;
				GLInitBalanceModel gLInitBalanceModel25 = gLInitBalanceModel15;
				gLInitBalanceModel25.MYtdCredit += model.MYtdCredit;
				GLInitBalanceModel gLInitBalanceModel26 = gLInitBalanceModel15;
				gLInitBalanceModel26.MYtdDebitFor += model.MYtdDebitFor;
				GLInitBalanceModel gLInitBalanceModel27 = gLInitBalanceModel15;
				gLInitBalanceModel27.MYtdCreditFor += model.MYtdCreditFor;
				gLInitBalanceModel15.MContactID = "0";
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLInitBalanceModel>(ctx, gLInitBalanceModel15, null, true));
			}
			model.MOrgID = ctx.MOrgID;
			model.MContactID = (string.IsNullOrEmpty(model.MContactID) ? "0" : model.MContactID);
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLInitBalanceModel>(ctx, model, null, true));
			return list;
		}

		public static List<string> GetAccountParentIdByRecursion(MContext ctx, string childrenId, List<BDAccountModel> accountList)
		{
			List<string> list = new List<string>();
			BDAccountModel child = (from x in accountList
			where x.MItemID == childrenId
			select x).FirstOrDefault();
			BDAccountModel bDAccountModel = (from x in accountList
			where x.MItemID == child.MParentID
			select x).FirstOrDefault();
			if (bDAccountModel == null)
			{
				return list;
			}
			list.Add(bDAccountModel.MItemID);
			list.AddRange(GetAccountParentIdByRecursion(ctx, bDAccountModel.MItemID, accountList));
			return list;
		}

		public List<BDAccountModel> GetChildrenAccountByCode(MContext ctx, List<string> codes)
		{
			List<BDAccountModel> baseBDAccountList = GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
			if (codes == null)
			{
				throw new ArgumentNullException("codes can not be Null");
			}
			List<BDAccountModel> list = new List<BDAccountModel>();
			foreach (string code in codes)
			{
				if (baseBDAccountList != null)
				{
					BDAccountModel bDAccountModel = (from x in baseBDAccountList
					where x.MCode == code && x.MAccountTableID == ctx.MAccountTableID
					select x).FirstOrDefault();
					if (bDAccountModel != null)
					{
						list.AddRange(GetChildrenByRecursion(baseBDAccountList, bDAccountModel));
					}
				}
			}
			return list;
		}

		public List<BDAccountModel> GetChildrenByRecursion(List<BDAccountModel> accountList, BDAccountModel parentModel)
		{
			List<BDAccountModel> list = new List<BDAccountModel>();
			List<BDAccountModel> list2 = (from x in accountList
			where x.MParentID == parentModel.MItemID
			select x).ToList();
			if (list2 == null || list2.Count() == 0)
			{
				list.Add(parentModel);
				return list;
			}
			foreach (BDAccountModel item in list2)
			{
				list.AddRange(GetChildrenByRecursion(accountList, item));
			}
			return list;
		}

		public string GetAccountIncreasingNumber(MContext ctx, string parentId)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("a.MOrgID", ctx.MOrgID);
			sqlWhere.Equal("a.MParentID", parentId);
			List<BDAccountModel> baseBDAccountList = GetBaseBDAccountList(ctx, sqlWhere, true, null);
			if (baseBDAccountList == null || baseBDAccountList.Count() == 0)
			{
				return "01";
			}
			int num = 1;
			foreach (BDAccountModel item in baseBDAccountList)
			{
				string[] array = item.MNumber.Split('.');
				string s = array[array.Length - 1];
				int num2 = 0;
				if (int.TryParse(s, out num2) && num2 > num)
				{
					num = num2;
				}
			}
			num++;
			string text = Convert.ToString(num);
			if (text.Length == 1)
			{
				text = "0" + text;
			}
			return text;
		}

		public static List<string> GetItemListByType(List<BDAccountListModel> accounts, List<string> types, bool isParent = true, bool isLeaf = false)
		{
			List<string> list = new List<string>();
			accounts.ForEach(delegate(BDAccountListModel x)
			{
				if (types.Contains(x.MAccountTypeID))
				{
					bool flag = true;
					if (isParent)
					{
						flag = (flag && x.MParentID == "0");
					}
					if (isLeaf)
					{
						flag = (flag && (from y in accounts
						where y.MParentID == x.MItemID
						select y).Count() == 0);
					}
					if (flag)
					{
						list.Add(x.MItemID);
					}
				}
			});
			return list;
		}

		public static List<string> GetItemListByType(List<BDAccountModel> accounts, List<string> types, bool isParent = true, bool isLeaf = false)
		{
			List<string> list = new List<string>();
			accounts.ForEach(delegate(BDAccountModel x)
			{
				if (types.Contains(x.MAccountTypeID))
				{
					bool flag = true;
					if (isParent)
					{
						flag = (flag && x.MParentID == "0");
					}
					if (isLeaf)
					{
						flag = (flag && (from y in accounts
						where y.MParentID == x.MItemID
						select y).Count() == 0);
					}
					if (flag)
					{
						list.Add(x.MItemID);
					}
				}
			});
			return list;
		}

		public BDAccountListModel GetLeafAccountByCode(List<BDAccountListModel> accounts, string code)
		{
			BDAccountListModel current = (from x in accounts
			where x.MCode == code
			select x).FirstOrDefault();
			List<BDAccountListModel> list = (from x in accounts
			where x.MParentID == current.MItemID
			select x).ToList();
			if (list.Count > 0)
			{
				return GetLeafAccountByCode(accounts, list[0].MCode);
			}
			return current;
		}

		public void GetLeafAccountListByCode<T>(List<T> accounts, string code, List<T> result)
		{
			if (result == null)
			{
				result = new List<T>();
			}
			T current = (T)(from x in accounts
			where ModelHelper.GetModelValue<T>(x, "MCode") == code
			select x).FirstOrDefault();
			if (current != null)
			{
				List<T> list = (from x in accounts
				where ModelHelper.GetModelValue<T>(x, "MParentID") == ModelHelper.GetModelValue<T>((T)current, "MItemID")
				select x).ToList();
				if (list.Count > 0)
				{
					foreach (T item in list)
					{
						GetLeafAccountListByCode(accounts, ModelHelper.GetModelValue(item, "MCode"), result);
					}
				}
				else
				{
					result.Add((T)current);
				}
			}
		}

		public static bool IsAccountContainsSub(List<BDAccountListModel> accounts, List<string> root, string sub)
		{
			int i;
			for (i = 0; i < root.Count; i++)
			{
				if (root[i] == sub || IsAccountSubOf((from x in accounts
				where x.MParentID == root[i]
				select x).ToList(), sub))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsAccountSubOf(List<BDAccountListModel> accounts, string sub)
		{
			if (accounts != null && accounts.Count != 0)
			{
				for (int i = 0; i < accounts.Count; i++)
				{
					if (accounts[i].MItemID == sub || IsAccountContainsSub(accounts, new List<string>
					{
						accounts[i].MItemID
					}, sub))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void GetAllSubCodes(List<BDAccountListModel> accounts, List<string> codes, ref List<string> result)
		{
			for (int i = 0; i < codes.Count; i++)
			{
				result.Add(codes[i]);
				BDAccountListModel currentAccount = GetAccountByCode(accounts, codes[i]);
				List<BDAccountListModel> source = accounts.FindAll((BDAccountListModel x) => x.MParentID == currentAccount.MItemID).ToList();
				GetAllSubCodes(accounts, (from x in source
				select x.MCode).ToList(), ref result);
			}
		}

		public static List<string> GetAllSubCodes(List<BDAccountListModel> accounts, List<string> codes)
		{
			List<string> result = new List<string>();
			GetAllSubCodes(accounts, codes, ref result);
			return result;
		}

		public BDAccountModel GetLeafAccountByCode(MContext ctx, List<BDAccountModel> accountList, string code)
		{
			if (accountList == null)
			{
				accountList = GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
			}
			BDAccountModel account = (from x in accountList
			where x.MCode == code
			select x).FirstOrDefault();
			if (account == null)
			{
				throw new Exception("can find account by code:" + code);
			}
			List<BDAccountModel> list = (from x in accountList
			where x.MParentID == account.MItemID
			select x).ToList();
			if (list == null && list.Count() == 0)
			{
				return account;
			}
			BDAccountModel bDAccountModel = (from x in list
			orderby decimal.Parse(x.MCode)
			select x).FirstOrDefault();
			if (accountList.Exists((BDAccountModel x) => x.MParentID == account.MItemID))
			{
				account = GetLeafAccountByCode(ctx, accountList, bDAccountModel.MCode);
			}
			return account;
		}

		public string GetAccountCodeIncreaseNumber(MContext ctx, string parentId, string parentCode)
		{
			string sql = $"SELECT MAX(CONCAT(substring('000000000000000000000000000000' ,LENGTH(mcode), 30-LENGTH(mcode)),mcode))  FROM t_bd_account \r\n                                             WHERE MOrgID = '{ctx.MOrgID}' and MParentID = '{parentId}' and MIsDelete = 0  ";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql);
			if (single == null)
			{
				return "01";
			}
			decimal value = Convert.ToDecimal(single);
			string text = Convert.ToString(value);
			text = text.Substring(parentCode.Length);
			decimal d = Convert.ToDecimal(text);
			d += decimal.One;
			if (d < 10m)
			{
				return "0" + Convert.ToString(d);
			}
			return Convert.ToString(d);
		}

		public BDAccountModel GetMapAccountId(MContext ctx, List<BDAccountModel> accountList, string settingCode, string defaultCode)
		{
			BDAccountModel bDAccountModel = new BDAccountModel();
			if (string.IsNullOrWhiteSpace(settingCode))
			{
				return string.IsNullOrWhiteSpace(defaultCode) ? bDAccountModel : GetLeafAccountByCode(ctx, accountList, defaultCode);
			}
			return GetLeafAccountByCode(ctx, accountList, settingCode);
		}

		public static bool IsAccountFullNameUpdated(MContext ctx, string morgID, string localeID)
		{
			string sql = "select t1.MItemID, t2.MFullName from t_bd_account t1 inner join t_bd_account_l t2 on t1.MItemID = t2.MParentID\r\n                        AND t1.MOrgID = t2.MOrgID\r\n                        AND t2.MLocaleID = @MLCID and ( t2.MFullName is null or length(t2.MFullName) = 0 )\r\n                         and t2.MIsDelete = 0\r\n                        where t1.MOrgID = @MOrgID and t1.MIsDelete = 0 limit 1";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = morgID
				},
				new MySqlParameter
				{
					ParameterName = "@MLCID",
					Value = localeID
				}
			};
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, cmdParms);
			return dataSet == null || dataSet.Tables.Count <= 0 || dataSet.Tables[0].Rows.Count <= 0;
		}

		public static List<CommandInfo> GetUpdateFullNameCmds(MContext ctx, Dictionary<string, string> updateNameList, BDAccountModel account)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (updateNameList == null || updateNameList.Count() == 0)
			{
				return list;
			}
			foreach (string key in updateNameList.Keys)
			{
				List<CommandInfo> list2 = list;
				CommandInfo commandInfo = new CommandInfo
				{
					CommandText = "update t_bd_account_l set MFullName = @MFullName where MParentID = @MParentID and MLocaleID = @MLCID and MOrgID = @MOrgID and MIsDelete = 0  "
				};
				DbParameter[] array = commandInfo.Parameters = new MySqlParameter[4]
				{
					new MySqlParameter
					{
						ParameterName = "@MFullName",
						Value = updateNameList[key]
					},
					new MySqlParameter
					{
						ParameterName = "@MParentID",
						Value = account.MItemID
					},
					new MySqlParameter
					{
						ParameterName = "@MLCID",
						Value = key
					},
					new MySqlParameter
					{
						ParameterName = "@MOrgID",
						Value = ctx.MOrgID
					}
				};
				list2.Add(commandInfo);
			}
			return list;
		}

		private static List<BDAccountModel> GetAccountModelByLCID(MContext ctx, string LCID)
		{
			string sql = "SELECT \r\n                t1.MItemID, t1.MNumber, t1.MParentID, t2.MName, (LENGTH(t1.MNumber) - LENGTH(REPLACE(t1.MNumber,'.', ''))) as MRank\r\n                        FROM\r\n                t_bd_account t1\r\n                    INNER JOIN\r\n                t_bd_account_l t2 ON t1.MItemID = t2.MParentID\r\n                    AND t1.MOrgID = t2.MOrgID\r\n                    AND t2.MLocaleID = @MLCID\r\n                    and t2.MIsDelete = 0  \r\n                where t1.MOrgID = @MOrgID and t1.MIsDelete  = 0 and t1.MIsActive = 1 \r\n            ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MLCID",
					Value = LCID
				}
			};
			return ModelInfoManager.GetDataModelBySql<BDAccountModel>(ctx, sql, cmdParms);
		}

		public static List<CommandInfo> GetClearAccountCmds(MContext ctx)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_bd_account_l set MIsDelete = 1 where MParentID in (select MItemID from t_bd_account where MOrgID=@MOrgID and MAccountTableID=@AccountStandart and MIsDelete = 0 )";
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@AccountStandart",
					Value = ctx.MAccountTableID
				}
			};
			list.Add(commandInfo);
			CommandInfo commandInfo2 = new CommandInfo();
			commandInfo2.CommandText = "update t_bd_account set MIsDelete = 1 where MOrgID=@MOrgID and MAccountTableID=@AccountStandart and MIsDelete = 0 ";
			array = (commandInfo2.Parameters = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@AccountStandart",
					Value = ctx.MAccountTableID
				}
			});
			list.Add(commandInfo2);
			CommandInfo commandInfo3 = new CommandInfo();
			commandInfo3.CommandText = "update t_bd_bankaccount set MAccountID=null where MOrgID=@MOrgID and MIsDelete = 0 ";
			array = (commandInfo3.Parameters = new MySqlParameter[1]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			});
			list.Add(commandInfo3);
			return list;
		}

		public List<CommandInfo> GetUpdateBankAccountIdCmd(MContext ctx, string newMItemId, string oldMItemId)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@newId",
					Value = newMItemId
				},
				new MySqlParameter
				{
					ParameterName = "@oldId",
					Value = oldMItemId
				}
			};
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_bd_account set MItemID=@newId where MItemID=@oldId and MOrgID=@MOrgID and MIsDelete = 0 ";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			CommandInfo commandInfo2 = new CommandInfo();
			commandInfo2.CommandText = "update t_bd_account_l set MParentID = @newId where MParentID=@oldId and MOrgID = @MOrgID and MIsDelete = 0 ";
			array = (commandInfo2.Parameters = parameters);
			list.Add(commandInfo2);
			return list;
		}

		public bool IsExistAccountName(MContext ctx, BDAccountModel model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT count(*) from t_bd_account a");
			stringBuilder.AppendLine("inner JOIN t_bd_account_l b on a.MOrgID=b.MOrgID AND b.MLocaleID=@MLocaleID and b.MName=@MName and a.mitemid=b.mparentid and a.mparentid=@ParentID and b.MIsDelete = 0  where a.MOrgID = @MOrgID and a.MIsDelete = 0  ");
			if (!string.IsNullOrWhiteSpace(model.MItemID))
			{
				stringBuilder.Append(" and a.mitemid<>@MAccountID");
			}
			MySqlParameter[] cmdParms = new MySqlParameter[5]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MLocaleID",
					Value = ctx.MLCID
				},
				new MySqlParameter
				{
					ParameterName = "@MName",
					Value = model.MName
				},
				new MySqlParameter
				{
					ParameterName = "@MAccountID",
					Value = model.MItemID
				},
				new MySqlParameter
				{
					ParameterName = "@ParentID",
					Value = (string.IsNullOrWhiteSpace(model.MParentID) ? "0" : model.MParentID)
				}
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(stringBuilder.ToString(), cmdParms);
		}

		public List<CommandInfo> GetRestoreBankAccountCmds(MContext ctx, List<string> idList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string empty = string.Empty;
			MySqlParameter[] parameters = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			empty = base.GetWhereInSql(idList, ref parameters, null);
			List<CommandInfo> list2 = list;
			CommandInfo obj = new CommandInfo
			{
				CommandText = $"update t_bd_account set MIsDelete = 0, MIsActive=1 where MOrgID=@MOrgID and MItemID in ({empty})"
			};
			DbParameter[] array = obj.Parameters = parameters;
			list2.Add(obj);
			List<CommandInfo> list3 = list;
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = $"update t_bd_account_l set MIsDelete = 0, MIsActive=1 where MOrgID=@MOrgID and MParentID in ({empty})"
			};
			array = (obj2.Parameters = parameters);
			list3.Add(obj2);
			return list;
		}

		public static List<string> GetChildAcctNumberList(MContext ctx, List<string> accoutIds, string fieldName = "MNumber")
		{
			string sql = string.Format(" select distinct {0} as MValue from t_bd_account where MParentID in ('" + string.Join("','", accoutIds) + "') and MOrgID = @MOrgID and MIsDelete = 0  ", fieldName);
			List<NameValueModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<NameValueModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
			return (from f in dataModelBySql
			select f.MValue).ToList();
		}

		public List<BDAccountListModel> GetAccountListIncludeCheckType(MContext ctx, BDAccountListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = string.Empty;
			if (filter?.IncludeAllLangName ?? false)
			{
				IEnumerable<BASLangModel> enumerable = from f in BASLangRepository.GetOrgLangList(ctx)
				where f.LangID != ctx.MLCID
				select f;
				int num = 1;
				List<string> list = new List<string>();
				string[] megiLangTypes = ServerHelper.MegiLangTypes;
				List<string> list2 = new List<string>();
				foreach (BASLangModel item in enumerable)
				{
					list2.Add(item.LangID);
					list.Add($"b{num}.MName as MName{item.LangID}");
					stringBuilder.AppendFormat(" INNER JOIN t_bd_account_l b{0} on a.MItemID = b{0}.MParentID  and b{0}.MLocaleID='{1}' and a.MOrgID = b{0}.MOrgID and a.MOrgID=@MOrgID", num, item.LangID);
					num++;
				}
				string[] array = megiLangTypes;
				foreach (string text in array)
				{
					if (!list2.Contains(text))
					{
						list.Add($"'' as MName{text}");
					}
				}
				value = string.Join(",", list) + ", ";
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendLine("select DISTINCT * from (");
			stringBuilder2.AppendLine("SELECT a.* , b.MName, b.MFullName , c.MName  as MAcctGroupName , f.MName as MAcctTypeName ,");
			if (filter?.IncludeAllLangName ?? false)
			{
				stringBuilder2.Append(value);
			}
			stringBuilder2.AppendLine(" IFNULL(e.MContactID,0) as MContactID ,IFNULL(e.MEmployeeID,0) as  MEmployeeID ,IFNULL(e.MMerItemID,0) as  MMerItemID , IFNULL(e.MExpItemID,0) as  MExpItemID,\r\n                                           IFNULL(e.MPaItemID,0) as MPaItemID,IFNULL(e.MTrackItem1,0) as MTrackItem1,IFNULL(e.MTrackItem2,0) as MTrackItem2,");
			stringBuilder2.AppendLine(" IFNULL(e.MTrackItem3,0) MTrackItem3,IFNULL(e.MTrackItem4,0) as MTrackItem4 ,IFNULL(e.MTrackItem5,0) as MTrackItem5 ");
			stringBuilder2.AppendLine("from t_bd_account a ");
			stringBuilder2.AppendLine(" INNER JOIN t_bd_account_l b on a.MItemID = b.MParentID  and b.MLocaleID=@MLocaleID and a.MOrgID = b.MOrgID and a.MOrgID=@MOrgID");
			stringBuilder2.AppendLine(" LEFT JOIN t_bas_accountgroup_l c on c.MParentID = a.MAccountGroupID and c.MLocaleID=@MLocaleID and a.MOrgID = b.MOrgID ");
			stringBuilder2.AppendLine(" LEFT JOIN t_bas_accounttype_l f on f.MParentID =a.MAccountTypeId and f.MLocaleID=@MLocaleID and a.MOrgID = b.MOrgID ");
			stringBuilder2.AppendLine(" LEFT JOIN t_gl_checkgroup e on e.MItemID = a.MCheckGroupID ");
			if (filter?.IncludeAllLangName ?? false)
			{
				stringBuilder2.Append(stringBuilder);
			}
			stringBuilder2.AppendLine(" where a.MIsDelete=0 ) t where MOrgID=@MOrgID ");
			MySqlParameter[] array2 = ctx.GetParameters((MySqlParameter)null);
			if (filter != null)
			{
				List<MySqlParameter> list3 = new List<MySqlParameter>();
				if (!string.IsNullOrWhiteSpace(filter.Keyword))
				{
					stringBuilder2.AppendFormat(" and ( MName like concat('%',@Keyword,'%') or MNumber like concat('%',@Keyword,'%') ) ");
					list3.Add(new MySqlParameter
					{
						ParameterName = "@Keyword",
						Value = filter.Keyword
					});
				}
				if (!string.IsNullOrWhiteSpace(filter.Group))
				{
					stringBuilder2.AppendLine(" and MAcctGroupName=@GroupName ");
					list3.Add(new MySqlParameter
					{
						ParameterName = "@GroupName",
						Value = filter.Group
					});
				}
				if (!string.IsNullOrWhiteSpace(filter.GroupID))
				{
					stringBuilder2.AppendLine(" and MAccountGroupID=@GroupID ");
					list3.Add(new MySqlParameter
					{
						ParameterName = "@GroupID",
						Value = filter.GroupID
					});
				}
				if (!string.IsNullOrWhiteSpace(filter.NotParentCodes))
				{
					stringBuilder2.AppendLine(" and MCode not in (@MExcludeCode)");
					list3.Add(new MySqlParameter
					{
						ParameterName = "@MExcludeCode",
						Value = "'" + string.Join("','", filter.NotParentCodes.Split(',')) + "'"
					});
				}
				if (!filter.IsActive && !filter.IsAll)
				{
					stringBuilder2.AppendLine(" and MIsActive=0");
				}
				else if (!filter.IsAll)
				{
					stringBuilder2.AppendLine(" and MIsActive=1");
				}
				if (filter.StartYearPeriod > 0 && filter.EndYearPeriod > 0)
				{
					list3.Add(new MySqlParameter
					{
						ParameterName = "@StartYearPeriod",
						Value = (object)filter.StartYearPeriod
					});
					list3.Add(new MySqlParameter
					{
						ParameterName = "@EndYearPeriod",
						Value = (object)filter.EndYearPeriod
					});
				}
				if (list3.Count() > 0)
				{
					array2 = array2.Concat(list3.ToArray()).ToArray();
				}
			}
			else
			{
				stringBuilder2.Append(" and MIsActive=1 ");
			}
			stringBuilder2.AppendLine(" order by MNumber");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder2.ToString(), array2);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return new List<BDAccountListModel>();
			}
			List<BDAccountListModel> list4 = new List<BDAccountListModel>();
			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				BDAccountListModel bDAccountListModel = new BDAccountListModel
				{
					MItemID = Convert.ToString(row["MItemID"]),
					MNumber = Convert.ToString(row["MNumber"]),
					MName = Convert.ToString(row["MName"]),
					MCode = Convert.ToString(row["MCode"]),
					MDC = Convert.ToInt32(row["MDC"]),
					MAcctTypeName = Convert.ToString(row["MAcctTypeName"]),
					MParentID = Convert.ToString(row["MParentID"]),
					MFullName = Convert.ToString(row["MFullName"]),
					MAccountGroupID = Convert.ToString(row["MAccountGroupID"]),
					MIsSys = Convert.ToBoolean(row["MIsSys"]),
					MCheckGroupID = Convert.ToString(row["MCheckGroupID"]),
					MIsCheckForCurrency = Convert.ToBoolean(row["MIsCheckForCurrency"]),
					MCreateInitBill = Convert.ToBoolean(row["MCreateInitBill"])
				};
				if (filter?.IncludeAllLangName ?? false)
				{
					bDAccountListModel.MName0x0009 = Convert.ToString(row["MName0x0009"]);
					bDAccountListModel.MName0x7804 = Convert.ToString(row["MName0x7804"]);
					bDAccountListModel.MName0x7C04 = Convert.ToString(row["MName0x7C04"]);
				}
				bDAccountListModel.MCheckGroupModel = new GLCheckGroupModel
				{
					MAccountID = Convert.ToString(row["MItemID"]),
					MItemID = Convert.ToString(row["MCheckGroupID"]),
					MContactID = Convert.ToInt32(row["MContactID"]),
					MEmployeeID = Convert.ToInt32(row["MEmployeeID"]),
					MMerItemID = Convert.ToInt32(row["MMerItemID"]),
					MExpItemID = Convert.ToInt32(row["MExpItemID"]),
					MPaItemID = Convert.ToInt32(row["MPaItemID"]),
					MTrackItem1 = Convert.ToInt32(row["MTrackItem1"]),
					MTrackItem2 = Convert.ToInt32(row["MTrackItem2"]),
					MTrackItem3 = Convert.ToInt32(row["MTrackItem3"]),
					MTrackItem4 = Convert.ToInt32(row["MTrackItem4"]),
					MTrackItem5 = Convert.ToInt32(row["MTrackItem5"])
				};
				list4.Add(bDAccountListModel);
			}
			return list4;
		}

		public List<MultiLanguageFieldList> GetAccountListMultiLanguageList(MContext ctx)
		{
			List<MultiLanguageFieldList> list = new List<MultiLanguageFieldList>();
			string sql = "SELECT * from t_bd_account_l where MOrgID=@MorgID and MIsDelete=0  order by MParentID";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, cmdParms);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return list;
			}
			DataTable dataTable = dataSet.Tables[0];
			List<MultiLanguageField> list2 = new List<MultiLanguageField>();
			DataRowCollection rows = dataSet.Tables[0].Rows;
			string b = null;
			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				string parentId = Convert.ToString(row["MParentID"]);
				if (parentId != b)
				{
					MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
					multiLanguageFieldList.MParentID = row.Field<string>("MParentID");
					multiLanguageFieldList.MMultiLanguageField = new List<MultiLanguageField>();
					multiLanguageFieldList.MFieldName = "MName";
					list.Add(multiLanguageFieldList);
					MultiLanguageFieldList multiLanguageFieldList2 = new MultiLanguageFieldList();
					multiLanguageFieldList2.MParentID = row.Field<string>("MParentID");
					multiLanguageFieldList2.MMultiLanguageField = new List<MultiLanguageField>();
					multiLanguageFieldList2.MFieldName = "MFullName";
					list.Add(multiLanguageFieldList2);
					b = parentId;
				}
				MultiLanguageField multiLanguageField = new MultiLanguageField();
				multiLanguageField.MPKID = row.Field<string>("MPKID");
				multiLanguageField.MOrgID = row.Field<string>("MOrgID");
				multiLanguageField.MValue = row.Field<string>("MName");
				multiLanguageField.MLocaleID = row.Field<string>("MLocaleID");
				MultiLanguageField multiLanguageField2 = new MultiLanguageField();
				multiLanguageField2.MPKID = row.Field<string>("MPKID");
				multiLanguageField2.MOrgID = row.Field<string>("MOrgID");
				multiLanguageField2.MValue = row.Field<string>("MFullName");
				multiLanguageField2.MLocaleID = row.Field<string>("MLocaleID");
				MultiLanguageFieldList multiLanguageFieldList3 = (from x in list
				where x.MParentID == parentId && x.MFieldName == "MName"
				select x).First();
				MultiLanguageFieldList multiLanguageFieldList4 = (from x in list
				where x.MParentID == parentId && x.MFieldName == "MFullName"
				select x).First();
				multiLanguageFieldList3.MMultiLanguageField.Add(multiLanguageField);
				multiLanguageFieldList4.MMultiLanguageField.Add(multiLanguageField2);
			}
			return list;
		}

		public List<BDAccountModel> GetAccountListWithCheckType(MContext ctx, string itemID = null, bool includeParent = false, bool includeDisable = false)
		{
			string empty = string.Empty;
			if (includeParent || !string.IsNullOrWhiteSpace(itemID))
			{
				empty = "\r\n                SELECT \r\n                t1.MItemID,\r\n                t1.MOrgID,\r\n                t1.MNumber,\r\n                t1.MCode,\r\n                t1.MDC,\r\n                t1.MCreateInitBill,\r\n                t1.MAccountTypeId,\r\n                t1.MAccountTableID,\r\n                t1.MIsCheckForCurrency,\r\n                t1.MAccountGroupID,\r\n                t1.MParentID,\r\n                t1.MCheckGroupID,\r\n                t1.MIsActive,\r\n                t2.MName,\r\n                t2.MFullName,\r\n                t3.MContactID,\r\n                t3.MEmployeeID,\r\n                t3.MMerItemID,\r\n                t3.MExpItemID,\r\n                t3.MPaItemID,\r\n                t3.MTrackItem1,\r\n                t3.MTrackItem2,\r\n                t3.MTrackItem3,\r\n                t3.MTrackItem4,\r\n                t3.MTrackItem5,\r\n                t4.MCyID as MCurrencyID,\r\n                (case when length(ifnull(t4.MItemID,'')) > 0 then '1' else '0' end) as MIsBankAccount\r\n            FROM\r\n                t_bd_account t1\r\n                inner join t_bd_account_l t2\r\n                on t1.MOrgID = t2.MOrgID\r\n                and t1.MIsDelete = t2.MIsDelete \r\n                and t1.MItemID = t2.MParentID\r\n                inner join\r\n                t_gl_checkgroup t3\r\n                on t1.MCheckGroupID = t3.MItemID\r\n                and t3.MIsDelete = t1.MIsDelete \r\n\r\n                LEFT JOIN \r\n                t_bd_bankAccount t4 \r\n                ON t4.MItemID = t1.MItemID\r\n                AND t4.MOrgID = t1.MOrgID\r\n                AND t4.MIsDelete = t1.MIsDelete\r\n            WHERE\r\n                t1.morgid = @MOrgID\r\n                and t1.MIsDelete = 0 \r\n                and t2.MLocaleID = @MLocaleID\r\n                ";
				if (!includeDisable)
				{
					empty += " and t1.MIsActive = 1";
				}
				if (!string.IsNullOrWhiteSpace(itemID))
				{
					empty += " and t1.MItemID = @MItemID";
				}
			}
			else
			{
				empty = "select t10.* from (\r\n                    SELECT \r\n                        t1.MItemID,\r\n                        t1.MOrgID,\r\n                        t1.MNumber,\r\n                        t1.MCode,\r\n                        t1.MDC,\r\n                        t1.MCreateInitBill,\r\n                        t1.MAccountTypeID,\r\n                        t1.MAccountTableID,\r\n                        t1.MIsCheckForCurrency,\r\n                        t1.MAccountGroupID,\r\n                        t1.MParentID,\r\n                        t1.MCheckGroupID,\r\n                        t1.MIsActive,\r\n                        t2.MName,\r\n                        t2.MFullName,\r\n                        t3.MContactID,\r\n                        t3.MEmployeeID,\r\n                        t3.MMerItemID,\r\n                        t3.MExpItemID,\r\n                        t3.MPaItemID,\r\n                        t3.MTrackItem1,\r\n                        t3.MTrackItem2,\r\n                        t3.MTrackItem3,\r\n                        t3.MTrackItem4,\r\n                        t3.MTrackItem5,\r\n                        t4.MCyID as MCurrencyID,\r\n                        (case when length(ifnull(t4.MItemID,'')) > 0 then '1' else '0' end) as MIsBankAccount\r\n                    FROM\r\n                        t_bd_account t1\r\n                        inner join t_bd_account_l t2\r\n                        on t1.MOrgID = t2.MOrgID\r\n                        and t1.MIsDelete = t2.MIsDelete \r\n                        and t1.MItemID = t2.MParentID\r\n                        inner join\r\n                        t_gl_checkgroup t3\r\n                        on t1.MCheckGroupID = t3.MItemID\r\n                        LEFT JOIN \r\n                        t_bd_bankAccount t4 \r\n                        ON t4.MItemID = t1.MItemID\r\n                        AND t4.MOrgID = t1.MOrgID\r\n                        AND t4.MIsDelete = t1.MIsDelete\r\n                    WHERE\r\n                        t1.morgid = @MOrgID\r\n                        and t1.MIsDelete = 0 ";
				if (!includeDisable)
				{
					empty += "  and t1.MIsActive = 1";
				}
				empty += "       \r\n                        and t2.MLocaleID = @MLocaleID\r\n                        and t3.MIsDelete = 0 \r\n                        order by MNumber asc\r\n                        ) t10 \r\n                        where not exists \r\n                        (\r\n                    SELECT \r\n                        1\r\n                    FROM\r\n                        t_bd_account t1\r\n                        inner join t_bd_account_l t2\r\n                        on t1.MOrgID = t2.MOrgID\r\n                        and t1.MIsDelete = t2.MIsDelete\r\n                        and t1.MItemID = t2.MParentID\r\n                        inner join\r\n                        t_gl_checkgroup t3\r\n                        on t1.MCheckGroupID = t3.MItemID\r\n                    WHERE\r\n                        t1.morgid = @MOrgID\r\n                        and t1.MIsDelete = 0 ";
				if (!includeDisable)
				{
					empty += "  and t1.MIsActive = 1";
				}
				empty += "  \r\n                        and t2.MLocaleID = @MLocaleID\r\n                        and t3.MIsDelete = 0 \r\n\t                    and t1.MParentId = t10.MItemID\r\n                        )";
			}
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(empty, ctx.GetParameters("@MItemID", itemID));
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			return (from x in dataSet.Tables[0].AsEnumerable()
			select new BDAccountModel
			{
				MItemID = x.MField<string>("MItemID"),
				MOrgID = x.MField<string>("MOrgID"),
				MNumber = x.MField<string>("MNumber"),
				MCode = x.MField<string>("MCode"),
				MDC = x.MField<int>("MDC"),
				MCreateInitBill = x.MField<bool>("MCreateInitBill"),
				MAccountTypeID = x.MField<string>("MAccountTypeID"),
				MAccountTableID = x.MField<string>("MAccountTableID"),
				MIsCheckForCurrency = x.MField<bool>("MIsCheckForCurrency"),
				MAccountGroupID = x.MField<string>("MAccountGroupID"),
				MParentID = x.MField<string>("MParentID"),
				MCheckGroupID = x.MField<string>("MCheckGroupID"),
				MIsActive = x.MField<bool>("MIsActive"),
				MName = x.MField<string>("MName"),
				MFullName = x.MField<string>("MFullName"),
				MCurrencyID = x.MField<string>("MCurrencyID"),
				MIsBankAccount = x.MField<bool>("MIsBankAccount"),
				MCheckGroupModel = new GLCheckGroupModel
				{
					MAccountID = x.MField<string>("MItemID"),
					MItemID = x.MField<string>("MCheckGroupID"),
					MContactID = x.MField<int>("MContactID"),
					MEmployeeID = x.MField<int>("MEmployeeID"),
					MMerItemID = x.MField<int>("MMerItemID"),
					MExpItemID = x.MField<int>("MExpItemID"),
					MPaItemID = x.MField<int>("MPaItemID"),
					MTrackItem1 = x.MField<int>("MTrackItem1"),
					MTrackItem2 = x.MField<int>("MTrackItem2"),
					MTrackItem3 = x.MField<int>("MTrackItem3"),
					MTrackItem4 = x.MField<int>("MTrackItem4"),
					MTrackItem5 = x.MField<int>("MTrackItem5")
				},
				MCurrencyDataModel = new GLCurrencyDataModel
				{
					MCurrencyID = x.Field<string>("MCurrencyID")
				},
				MCheckGroupValueModel = new GLCheckGroupValueModel()
			}).ToList();
		}

		public List<BDAccountModel> GetCurrentAccountBaseData(MContext ctx, bool top = true)
		{
			List<string> values = new List<string>
			{
				"1122",
				"2202",
				"1123",
				"2203",
				"1221",
				"2241"
			};
			string sql = "SELECT \r\n                                t10.*\r\n                            FROM\r\n                                (SELECT \r\n                                    t1.MItemID,\r\n                                        t1.MOrgID,\r\n                                        t1.MNumber,\r\n                                        t1.MCode,\r\n                                        t1.MDC,\r\n                                        t1.MCreateInitBill,\r\n                                        t1.MAccountTypeID,\r\n                                        t1.MAccountTableID,\r\n                                        t1.MIsCheckForCurrency,\r\n                                        t1.MAccountGroupID,\r\n                                        t1.MParentID,\r\n                                        t1.MCheckGroupID,\r\n                                        t2.MName,\r\n                                        t2.MFullName,\r\n                                        t3.MContactID,\r\n                                        t3.MEmployeeID,\r\n                                        t3.MMerItemID,\r\n                                        t3.MExpItemID,\r\n                                        t3.MPaItemID,\r\n                                        t3.MTrackItem1,\r\n                                        t3.MTrackItem2,\r\n                                        t3.MTrackItem3,\r\n                                        t3.MTrackItem4,\r\n                                        t3.MTrackItem5,\r\n                                        t5.MCheckGroupValueID,\r\n                                        t5.MInitBalance,\r\n                                        t5.MInitBalanceFor,\r\n                                        t5.MYtdDebit,\r\n                                        t5.MYtdDebitFor,\r\n                                        t5.MYtdCredit,\r\n                                        t5.MYtdCreditFor,\r\n                                        t5.MCurrencyID\r\n                                FROM\r\n                                    t_bd_account t1\r\n                                INNER JOIN t_bd_account_l t2 ON t1.MOrgID = t2.MOrgID\r\n                                    AND t1.MIsDelete = t2.MIsDelete\r\n                                    AND t1.MIsActive = t2.MIsActive\r\n                                    AND t1.MItemID = t2.MParentID\r\n                                INNER JOIN t_gl_checkgroup t3 ON t1.MCheckGroupID = t3.MItemID\r\n                                    AND t3.MIsDelete = t1.MIsDelete\r\n                                LEFT JOIN t_bd_bankAccount t4 ON t4.MItemID = t1.MItemID\r\n                                    AND t4.MOrgID = t1.MOrgID\r\n                                    AND t4.MIsDelete = t1.MIsDelete\r\n                                LEFT JOIN t_gl_initbalance t5 ON t5.MAccountID = t1.MItemID\r\n                                    AND t5.MOrgID = t1.MOrgID\r\n                                    AND t5.MCheckGroupValueID  " + (top ? " = '0' " : " != '0' ") + "\r\n                                    AND t5.MIsDelete = t1.MIsDelete\r\n                                WHERE\r\n                                    t1.morgid = @MOrgID\r\n                                        AND t1.MIsDelete = 0\r\n                                        AND t1.MIsActive = 1\r\n                                        AND t2.MLocaleID = @MLocaleID\r\n                                        AND SUBSTRING(t1.MCode, 1, 4) IN ('" + string.Join("','", values) + "')\r\n                                ORDER BY MNumber ASC) t10\r\n                            WHERE\r\n                                NOT EXISTS( SELECT \r\n                                        1\r\n                                    FROM\r\n                                        t_bd_account t1\r\n                                    WHERE\r\n                                        t1.morgid = t10.MOrgID\r\n                                            AND t1.MIsDelete = 0\r\n                                            AND t1.MIsActive = 1\r\n                                            AND t1.MParentId = t10.MItemID)";
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, parameters);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			List<BDAccountModel> list = (from x in dataSet.Tables[0].AsEnumerable()
			select new BDAccountModel
			{
				MItemID = x.MField<string>("MItemID"),
				MOrgID = x.MField<string>("MOrgID"),
				MNumber = x.MField<string>("MNumber"),
				MCode = x.MField<string>("MCode"),
				MDC = x.MField<int>("MDC"),
				MCreateInitBill = x.MField<bool>("MCreateInitBill"),
				MAccountTypeID = x.MField<string>("MAccountTypeID"),
				MAccountTableID = x.MField<string>("MAccountTableID"),
				MIsCheckForCurrency = (x.MField<ulong>("MIsCheckForCurrency") == 1),
				MAccountGroupID = x.MField<string>("MAccountGroupID"),
				MParentID = x.MField<string>("MParentID"),
				MCheckGroupID = x.MField<string>("MCheckGroupID"),
				MFullName = x.MField<string>("MFullName"),
				MName = x.MField<string>("MName"),
				MInitBalanceModel = new GLInitBalanceModel
				{
					MAccountID = x.MField<string>("MItemID"),
					MCheckGroupValueID = x.MField<string>("MCheckGroupValueID"),
					MCurrencyID = x.MField<string>("MCurrencyID"),
					MInitBalance = x.MField<decimal>("MInitBalance"),
					MInitBalanceFor = x.MField<decimal>("MInitBalanceFor"),
					MYtdDebit = x.MField<decimal>("MYtdDebit"),
					MYtdDebitFor = x.MField<decimal>("MYtdDebitFor"),
					MYtdCredit = x.MField<decimal>("MYtdCredit"),
					MYtdCreditFor = x.MField<decimal>("MYtdCreditFor")
				},
				MCheckGroupModel = new GLCheckGroupModel
				{
					MItemID = x.MField<string>("MCheckGroupID"),
					MContactID = x.MField<int>("MContactID"),
					MEmployeeID = x.MField<int>("MEmployeeID"),
					MMerItemID = x.MField<int>("MMerItemID"),
					MExpItemID = x.MField<int>("MExpItemID"),
					MPaItemID = x.MField<int>("MPaItemID"),
					MTrackItem1 = x.MField<int>("MTrackItem1"),
					MTrackItem2 = x.MField<int>("MTrackItem2"),
					MTrackItem3 = x.MField<int>("MTrackItem3"),
					MTrackItem4 = x.MField<int>("MTrackItem4"),
					MTrackItem5 = x.MField<int>("MTrackItem5")
				},
				MCurrencyDataModel = new GLCurrencyDataModel
				{
					MCurrencyID = x.Field<string>("MCurrencyID")
				},
				MCheckGroupValueModel = new GLCheckGroupValueModel()
			}).ToList();
			if (top)
			{
				List<string> accountIds = (from x in list
				select x.MItemID).Distinct().ToList();
				List<BDAccountModel> list2 = new List<BDAccountModel>();
				int i;
				for (i = 0; i < accountIds.Count; i++)
				{
					List<BDAccountModel> list3 = (from x in list
					where x.MItemID == accountIds[i]
					select x).ToList();
					list3[0].MInitBalanceModels = (from x in list3
					select x.MInitBalanceModel).ToList();
					list2.Add(list3[0]);
				}
				return list2;
			}
			return list;
		}

		public DataSet CheckTypeIsUsed(MContext ctx, string accountId, int checkTypeEnum)
		{
			DataSet result = new DataSet();
			if (string.IsNullOrWhiteSpace(accountId))
			{
				return result;
			}
			GLUtility gLUtility = new GLUtility();
			string checkTypeColumnName = gLUtility.GetCheckTypeColumnName(checkTypeEnum);
			if (string.IsNullOrWhiteSpace(checkTypeColumnName))
			{
				return result;
			}
			string arg = $"(ifnull(b.{checkTypeColumnName},'') <>'') ";
			string sql = string.Format("SELECT DISTINCT 1 from t_gl_initbalance a\r\n                                  INNER JOIN t_gl_checkgroupvalue b on a.MOrgID = b.MOrgID \r\n                                  and a.MCheckGroupValueID=b.MItemID \r\n                                  and a.MIsDelete=b.MIsDelete\r\n                               where a.MOrgID=@MOrgID and a.MAccountID=@MAccountID and a.MIsDelete=0 and {0}  and (ABS(a.MInitBalanceFor) + ABS(a.MInitBalance)+ABS(a.MYtdDebit)+ABS(a.MYtdCredit))<>0\r\n                               UNION\r\n                               SELECT DISTINCT 2 from t_gl_balance a\r\n                                  INNER JOIN t_gl_checkgroupvalue b on a.MOrgID = b.MOrgID \r\n                                  and a.MCheckGroupValueID=b.MItemID \r\n                                  and a.MIsDelete=b.MIsDelete\r\n                               where a.MOrgID=@MOrgID and a.MAccountID=@MAccountID and a.MIsDelete=0 and {0}  and (ABS(a.MBeginBalance) + ABS(a.MDebit)+ABS(a.MCredit)+ABS(a.MYtdDebit)+ABS(a.MYtdCredit)+ABS(a.MEndBalance))<>0\r\n                               UNION\r\n                               SELECT DISTINCT 3 from t_gl_voucher a \r\n                                  INNER JOIN t_gl_voucherentry c on a.MItemID=c.MID and ifnull(a.MNumber,'')<>'' and a.MOrgID=c.MOrgID and a.MIsDelete=c.MIsDelete\r\n                                  INNER JOIN t_gl_checkgroupvalue b on b.MOrgID = c.MOrgID \r\n                                  and c.MCheckGroupValueID=b.MItemID \r\n                                  and b.MIsDelete=c.MIsDelete\r\n                               where a.MOrgID=@MOrgID and c.MAccountID=@MAccountID and a.MIsDelete=0 and {0} \r\n                               UNION \r\n                               SELECT DISTINCT 4 from t_fc_vouchermoduleentry a \r\n                                  INNER JOIN t_gl_checkgroupvalue b on a.MOrgID = b.MOrgID \r\n                                  and a.MCheckGroupValueID=b.MItemID \r\n                                  and a.MIsDelete=b.MIsDelete\r\n                               where a.MOrgID=@MOrgID and a.MAccountID=@MAccountID and  a.MIsDelete=0 and  {0} ", arg);
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MAccountID", accountId),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return new DynamicDbHelperMySQL(ctx).Query(sql, cmdParms);
		}
	}
}
