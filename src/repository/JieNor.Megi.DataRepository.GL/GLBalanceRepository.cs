using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLBalanceRepository : DataServiceT<GLBalanceModel>
	{
		private GLVoucherEntryRepository VEEntryDal = new GLVoucherEntryRepository();

		private GLVoucherRepository VEDal = new GLVoucherRepository();

		private BDAccountRepository accountDal = new BDAccountRepository();

		public List<GLBalanceModel> GetBalanceListIncludeCheckGroupValue(MContext ctx, SqlWhere filter, bool includeCheckTypeValue = false)
		{
			string str = " select * from (\n                                SELECT \n                                    t1.MItemID,\n                                    t1.MYear,\n                                    t1.MPeriod,\n                                    t1.MYearPeriod,\n                                    t1.MAccountID,\n                                    t1.MCurrencyID,\n                                    t1.MCheckGroupValueID,\n                                    t1.MBeginBalance,\n                                    t1.MBeginBalanceFor,\n                                    t1.MDebit,\n                                    t1.MDebitFor,\n                                    t1.MCredit,\n                                    t1.MCreditFor,\n                                    t1.MYtdDebit,\n                                    t1.MYtdDebitFor,\n                                    t1.MYtdCredit,\n                                    t1.MYtdCreditFor,\n                                    t1.MEndBalance,\n                                    t1.MEndBalanceFor,\n                                    t2.MCheckGroupID,\n                                    t2.MDC,\n                                    t2.MCode,\n                                    t2.MNumber,\n                                    t3.MContactID as MContact,\n                                    t3.MEmployeeID as MEmployee,\n                                    t3.MMerItemID as MMerItem,\n                                    t3.MExpItemID as MExpItem,\n                                    t3.MPaItemID as MPaItem,\n                                    t3.MTrackItem1 as MTrack1,\n                                    t3.MTrackItem2 as MTrack2,\n                                    t3.MTrackItem3 as MTrack3,\n                                    t3.MTrackItem4 as MTrack4,\n                                    t3.MTrackItem5 as MTrack5,\n                                    t4.MContactID,\n                                    t4.MEmployeeID, \n                                    t4.MMerItemID,\n                                    t4.MExpItemID,\n                                    t4.MPaItemID,\n                                    t4.MTrackItem1,\n                                    t4.MTrackItem2,\n                                    t4.MTrackItem3,\n                                    t4.MTrackItem4,\n                                    t4.MTrackItem5 ";
			if (includeCheckTypeValue)
			{
				str += string.Format(" ,convert(AES_DECRYPT(t5.MName,'{0}') using utf8) as MContactName , F_GetUserName(t6.MFirstName,t6.MLastName) as MEmployeeName , \r\n                              t7.MName as MExpItemName , t8.MName as MMerItemName , t9.MName as MPaItemGroupName, t10.MName as MPaItemName,\r\n                              t11.MName as MTrackItem1Name , t12.MName as MTrackItem2Name ,t13.MName as MTrackItem3Name ,t14.MName as MTrackItem4Name ,t15.MName as MTrackItem5Name ", "JieNor-001");
			}
			str += "\n                                FROM\n                                    t_gl_balance t1\n                                        INNER JOIN\n                                    t_bd_account t2 ON t1.MOrgID = t2.MOrgID\n                                        AND t1.MAccountId = t2.MItemID\n                                        AND t1.MisDelete = t2.MIsDelete\n                                        inner JOIN\n                                    t_gl_checkgroup t3 ON t3.MItemID = t2.MCheckGroupID\n                                        AND t3.MIsDelete = t1.MIsDelete\n                                        left JOIN\n                                    t_gl_checkgroupvalue t4 ON t4.MOrgID = t1.MOrgID\n                                        AND t4.MItemID = t1.MCheckGroupValueID\n                                        AND t4.MIsDelete = t1.MIsDelete ";
			if (includeCheckTypeValue)
			{
				str += GetRelationCheckTypeValueSql();
			}
			str += "WHERE t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 ) t";
			MySqlParameter[] array = ctx.GetParameters((MySqlParameter)null);
			if (filter != null)
			{
				str += filter.WhereSqlString;
				str += filter.OrderBySqlString;
				array = array.Concat(filter.Parameters ?? new MySqlParameter[0]).ToArray();
			}
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(str, array);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				return BindDataset2InitBalance(ctx, dataSet.Tables[0], includeCheckTypeValue, true);
			}
			return new List<GLBalanceModel>();
		}

		private string GetRelationCheckTypeValueSql()
		{
			return " LEFT JOIN t_bd_contacts_l t5 on t5.MParentID = t4.MContactID and t5.MLocaleID=@MLocaleID and t5.MOrgID = t4.MOrgID and t5.MIsDelete=0\r\n                           LEFT JOIN t_bd_employees_l t6 on t6.MParentID = t4.MEmployeeID and t6.MLocaleID=@MLocaleID and t6.MOrgID = t4.MOrgID and t6.MIsDelete=0\r\n                           LEFT JOIN t_bd_expenseitem_l t7 on t7.MParentID = t4.MExpItemID and t7.MLocaleID=@MLocaleID and t7.MOrgID = t4.MOrgID and t7.MIsDelete=0 \r\n                           LEFT JOIN t_bd_item t16 on t16.mitemid = t4.MMerItemID and t16.MOrgID = t4.MOrgID and t16.MIsDelete=0 \r\n                           LEFT JOIN t_bd_item_l t8 on t8.MParentID = t4.MMerItemID and t8.MLocaleID=@MLocaleID and t8.MOrgID = t4.MOrgID and t8.MIsDelete=0 \r\n                           LEFT JOIN t_pa_payitemgroup_l t9 on t9.MParentID = t4.MPaItemID and t9.MLocaleID=@MLocaleID and t9.MOrgID = t4.MOrgID and t9.MIsDelete=0 \r\n                           LEFT JOIN t_pa_payitem_l t10 on t10.MParentID = t4.MPaItemID and t10.MLocaleID=@MLocaleID and t10.MOrgID = t4.MOrgID and t10.MIsDelete=0  \r\n                           LEFT JOIN t_bd_trackentry_l t11 on t11.MParentID = t4.MTrackItem1 and t11.MLocaleID=@MLocaleID and t11.MOrgID = t4.MOrgID and t11.MIsDelete=0 \r\n                           LEFT JOIN t_bd_trackentry_l t12 on t12.MParentID = t4.MTrackItem2 and t12.MLocaleID=@MLocaleID and t12.MOrgID = t4.MOrgID and t12.MIsDelete=0\r\n                           LEFT JOIN t_bd_trackentry_l t13 on t13.MParentID = t4.MTrackItem3 and t13.MLocaleID=@MLocaleID and t13.MOrgID = t4.MOrgID and t13.MIsDelete=0\r\n                           LEFT JOIN t_bd_trackentry_l t14 on t14.MParentID = t4.MTrackItem4 and t14.MLocaleID=@MLocaleID and t14.MOrgID = t4.MOrgID and t14.MIsDelete=0\r\n                           LEFT JOIN t_bd_trackentry_l t15 on t15.MParentID = t4.MTrackItem5 and t15.MLocaleID=@MLocaleID and t15.MOrgID = t4.MOrgID and t15.MIsDelete=0 ";
		}

		public List<GLBalanceModel> GetBalanceListToGLReport(MContext ctx, GLReportBaseFilterModel filter)
		{
			SqlWhere sqlWhereFromGLReportFilter = GetSqlWhereFromGLReportFilter(filter);
			string str = " select * from (\n                                SELECT \n                                    t1.MItemID,\n                                    t1.MYear,\n                                    t1.MPeriod,\n                                    t1.MYearPeriod,\n                                    t1.MAccountID,\n                                    t1.MCurrencyID,\n                                    t1.MCheckGroupValueID,\n                                    t1.MBeginBalance,\n                                    t1.MBeginBalanceFor,\n                                    t1.MDebit,\n                                    t1.MDebitFor,\n                                    t1.MCredit,\n                                    t1.MCreditFor,\n                                    t1.MYtdDebit,\n                                    t1.MYtdDebitFor,\n                                    t1.MYtdCredit,\n                                    t1.MYtdCreditFor,\n                                    t1.MEndBalance,\n                                    t1.MEndBalanceFor,\n                                    t2.MCheckGroupID,\n                                    t2.MDC,\n                                    t2.MCode,\n                                    t2.MNumber,\n                                    t2.MIsActive as MAccountIsActive,\n                                    t3.MContactID as MContact,\n                                    t3.MEmployeeID as MEmployee,\n                                    t3.MMerItemID as MMerItem,\n                                    t3.MExpItemID as MExpItem,\n                                    t3.MPaItemID as MPaItem,\n                                    t3.MTrackItem1 as MTrack1,\n                                    t3.MTrackItem2 as MTrack2,\n                                    t3.MTrackItem3 as MTrack3,\n                                    t3.MTrackItem4 as MTrack4,\n                                    t3.MTrackItem5 as MTrack5,\n                                    t4.MContactID,\n                                    t4.MEmployeeID, \n                                    t4.MMerItemID,\n                                    t4.MExpItemID,\n                                    t4.MPaItemID,\n                                    t4.MTrackItem1,\n                                    t4.MTrackItem2,\n                                    t4.MTrackItem3,\n                                    t4.MTrackItem4,\n                                    t4.MTrackItem5 ";
			if (filter.IncludeCheckType)
			{
				str += string.Format(" ,convert(AES_DECRYPT(t5.MName,'{0}') using utf8) as MContactName , F_GetUserName(t6.MFirstName,t6.MLastName) as MEmployeeName , \r\n                              t7.MName as MExpItemName , CONCAT(t16.MNumber ,':',t8.MDesc) as MMerItemName , t9.MName as MPaItemGroupName, t10.MName as MPaItemName,\r\n                              t11.MName as MTrackItem1Name , t12.MName as MTrackItem2Name ,t13.MName as MTrackItem3Name ,t14.MName as MTrackItem4Name ,t15.MName as MTrackItem5Name ", "JieNor-001");
			}
			str += "\n                                FROM\n                                    t_gl_balance t1\n                                        INNER JOIN\n                                    t_bd_account t2 ON t1.MOrgID = t2.MOrgID\n                                        AND t1.MAccountId = t2.MItemID\n                                        AND t1.MIsDelete = t2.MIsDelete\n                                        inner JOIN\n                                    t_gl_checkgroup t3 ON t3.MItemID = t2.MCheckGroupID\n                                        AND t3.MIsDelete = t1.MIsDelete \n                                        left JOIN\n                                    t_gl_checkgroupvalue t4 ON t4.MOrgID = t1.MOrgID\n                                        AND t4.MItemID = t1.MCheckGroupValueID\n                                        AND t4.MIsDelete = t1.MIsDelete ";
			if (filter.IncludeCheckType)
			{
				str += GetRelationCheckTypeValueSql();
			}
			str += "WHERE t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 ";
			if (!string.IsNullOrWhiteSpace(filter.CheckGroupValueId))
			{
				str += $" and t1.MCheckGroupValueID='{filter.CheckGroupValueId}' ";
			}
			if (filter.OnlyCheckType)
			{
				str += " and t1.MCheckGroupValueID<>'0'";
			}
			if (sqlWhereFromGLReportFilter != null)
			{
				str = str + " and " + sqlWhereFromGLReportFilter.FilterString;
			}
			str += ") t where 1=1  ";
			MySqlParameter[] paramsToGLReport = GetParamsToGLReport(ctx, filter, sqlWhereFromGLReportFilter);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(str, paramsToGLReport);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				return BindDataset2InitBalance(ctx, dataSet.Tables[0], filter.IncludeCheckType, true);
			}
			return new List<GLBalanceModel>();
		}

		private string GetIncludeNoAccurrenceAmountSqlToGLReport(GLReportBaseFilterModel filter)
		{
			string text = "";
			if (!filter.MDisplayNoAccurrenceAmount)
			{
				text = " and (MAccountID in \r\n                                 (SELECT ge.MAccountID from t_gl_voucher gv \r\n                                  INNER JOIN t_gl_voucherentry ge on gv.MItemID=ge.mid and ge.MOrgID=@MOrgID \r\n                                  and (gv.MYEAR *100 + gv.MPeriod)>=@StartYearPeriod and (gv.MYEAR *100 + gv.MPeriod)<=@EndYearPeriod ";
				text = ((!filter.IncludeUnapprovedVoucher) ? (text + " and gv.MStatus=@VourchApproveStatus)") : (text + ")"));
				text += " or  MDebit<>0 or MCredit<>0)";
			}
			if (!filter.MDisplayZeorEndBalance)
			{
				text += " and MEndBalance <> 0 ";
			}
			return text;
		}

		private string GetCheckTypeValueSqlToGLReport(GLReportBaseFilterModel filter, List<MySqlParameter> sqlParameter)
		{
			string text = "";
			if (filter.CheckTypeValueList == null || filter.CheckTypeValueList.Count() == 0)
			{
				return text;
			}
			IEnumerable<IGrouping<string, string>> enumerable = from x in filter.CheckTypeValueList
			group x.MValue by x.MName;
			foreach (IGrouping<string, string> item in enumerable)
			{
				string key = item.Key;
				List<string> values = item.ToList();
				int num = -1;
				if (int.TryParse(key, out num))
				{
					switch (num)
					{
					case 0:
					{
						string text2 = "'" + string.Join("','", values) + "'";
						text += $" and MContactID in  ({text2})";
						sqlParameter.Add(new MySqlParameter("@MContactID", text2));
						break;
					}
					case 1:
					{
						text += " and MEmployeeID in (@MEmployeeID)";
						string text2 = "'" + string.Join("','", values) + "'";
						sqlParameter.Add(new MySqlParameter("@MEmployeeID", text2));
						break;
					}
					case 3:
					{
						string text2 = "'" + string.Join("','", values) + "'";
						text += $" and MExpItemID in ({text2})";
						sqlParameter.Add(new MySqlParameter("@MExpItemID", text2));
						break;
					}
					case 2:
					{
						string text2 = "'" + string.Join("','", values) + "'";
						text += $" and MMerItemID in ({text2})";
						sqlParameter.Add(new MySqlParameter("@MMerItemID", text2));
						break;
					}
					case 4:
					{
						string text2 = "'" + string.Join("','", values) + "'";
						text += $" and MPaItemID in ({text2})";
						sqlParameter.Add(new MySqlParameter("@MPaItemID", text2));
						break;
					}
					case 5:
					{
						string text2 = "'" + string.Join("','", values) + "'";
						text += $" and MTrackItem1 in ({text2})";
						sqlParameter.Add(new MySqlParameter("@MTrackItem1", text2));
						break;
					}
					case 6:
					{
						string text2 = "'" + string.Join("','", values) + "'";
						text += $" and MTrackItem2 in ({text2})";
						sqlParameter.Add(new MySqlParameter("@MTrackItem2", text2));
						break;
					}
					case 7:
					{
						string text2 = "'" + string.Join("','", values) + "'";
						text += $" and MTrackItem3 in ({text2})";
						sqlParameter.Add(new MySqlParameter("@MTrackItem3", text2));
						break;
					}
					case 8:
					{
						string text2 = "'" + string.Join("','", values) + "'";
						text += $" and MTrackItem4 in ({text2})";
						sqlParameter.Add(new MySqlParameter("@MTrackItem4", text2));
						break;
					}
					case 9:
					{
						string text2 = "'" + string.Join("','", values) + "'";
						text += $" and MTrackItem5 in ({text2})";
						sqlParameter.Add(new MySqlParameter("@MTrackItem5", text2));
						break;
					}
					}
				}
			}
			return text;
		}

		private MySqlParameter[] GetParamsToGLReport(MContext ctx, GLReportBaseFilterModel filter, SqlWhere sqlWhere)
		{
			MySqlParameter[] first = ctx.GetParameters((MySqlParameter)null);
			if (sqlWhere != null)
			{
				first = first.Concat(sqlWhere.Parameters ?? new MySqlParameter[0]).ToArray();
			}
			MySqlParameter[] second = new MySqlParameter[3]
			{
				new MySqlParameter("@StartYearPeriod", filter.MStartPeroid),
				new MySqlParameter("@EndYearPeriod", filter.MEndPeroid),
				new MySqlParameter("VourchApproveStatus", 1)
			};
			return first.Concat(second).ToArray();
		}

		private SqlWhere GetSqlWhereFromGLReportFilter(GLReportBaseFilterModel filter)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.LessOrEqual("t1.MYearPeriod", filter.MEndPeroid);
			sqlWhere.GreaterOrEqual("t1.MYearPeriod", filter.MStartPeroid);
			if (!string.IsNullOrEmpty(filter.MCurrencyID) && filter.MCurrencyID != "0")
			{
				sqlWhere.Equal("t1.MCurrencyID", filter.MCurrencyID);
			}
			if (!string.IsNullOrEmpty(filter.MAccountID))
			{
				sqlWhere.In("t1.MAccountID", filter.MAccountID.Split(','));
			}
			else if (filter.AccountIdList != null && filter.AccountIdList.Count() > 0)
			{
				sqlWhere.In("t1.MAccountID", filter.AccountIdList);
			}
			if (!filter.IncludeCheckType)
			{
				sqlWhere.Equal("t1.MCheckGroupValueID", "0");
			}
			if (!filter.IncludeDisabledAccount)
			{
				sqlWhere.Equal("t2.MIsActive", 1);
			}
			return sqlWhere;
		}

		public List<GLBalanceModel> BindDataset2InitBalance(MContext ctx, DataTable dt, bool includeCheckTypeValue = false, bool includeCheckType = true)
		{
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow row = dt.Rows[i];
				GLBalanceModel gLBalanceModel = new GLBalanceModel
				{
					MOrgID = ctx.MOrgID,
					MItemID = row.MField<string>("MItemID"),
					MAccountID = row.MField<string>("MAccountID"),
					MNumber = row.MField<string>("MNumber"),
					MCurrencyID = row.MField<string>("MCurrencyID"),
					MDC = row.MField<int>("MDC"),
					MAccountCode = row.MField("MCode"),
					MYear = row.MField<int>("MYear"),
					MPeriod = row.MField<int>("MPeriod"),
					MYearPeriod = row.MField<int>("MYearPeriod"),
					MCheckGroupValueID = row.MField<string>("MCheckGroupValueID"),
					MBeginBalance = row.MField<decimal>("MBeginBalance"),
					MBeginBalanceFor = row.MField<decimal>("MBeginBalanceFor"),
					MDebit = row.MField<decimal>("MDebit"),
					MDebitFor = row.MField<decimal>("MDebitFor"),
					MCredit = row.MField<decimal>("MCredit"),
					MCreditFor = row.MField<decimal>("MCreditFor"),
					MYtdDebit = row.MField<decimal>("MYtdDebit"),
					MYtdDebitFor = row.MField<decimal>("MYtdDebitFor"),
					MYtdCredit = row.MField<decimal>("MYtdCredit"),
					MYtdCreditFor = row.MField<decimal>("MYtdCreditFor"),
					MEndBalance = row.MField<decimal>("MEndBalance"),
					MEndBalanceFor = row.MField<decimal>("MEndBalanceFor")
				};
				BDAccountModel bDAccountModel = new BDAccountModel
				{
					MItemID = gLBalanceModel.MAccountID,
					MCheckGroupID = row.MField<string>("MCheckGroupID"),
					MDC = row.MField<int>("MDC"),
					MCode = row.MField("MCode")
				};
				if (includeCheckType)
				{
					GLCheckGroupModel obj = new GLCheckGroupModel
					{
						MContactID = row.MField<int>("MContact"),
						MEmployeeID = row.MField<int>("MEmployee"),
						MMerItemID = row.MField<int>("MMerItem"),
						MExpItemID = row.MField<int>("MExpItem"),
						MPaItemID = row.MField<int>("MPaItem"),
						MTrackItem1 = row.MField<int>("MTrack1"),
						MTrackItem2 = row.MField<int>("MTrack2"),
						MTrackItem3 = row.MField<int>("MTrack3"),
						MTrackItem4 = row.MField<int>("MTrack4"),
						MTrackItem5 = row.MField<int>("MTrack5")
					};
					GLCheckGroupModel gLCheckGroupModel2 = bDAccountModel.MCheckGroupModel = obj;
				}
				GLCheckGroupValueModel gLCheckGroupValueModel = new GLCheckGroupValueModel
				{
					MItemID = row.MField<string>("MCheckGroupValueID"),
					MContactID = row.MField<string>("MContactID"),
					MEmployeeID = row.MField<string>("MEmployeeID"),
					MMerItemID = row.MField<string>("MMerItemID"),
					MExpItemID = row.MField<string>("MExpItemID"),
					MPaItemID = row.MField<string>("MPaItemID"),
					MTrackItem1 = row.MField<string>("MTrackItem1"),
					MTrackItem2 = row.MField<string>("MTrackItem2"),
					MTrackItem3 = row.MField<string>("MTrackItem3"),
					MTrackItem4 = row.MField<string>("MTrackItem4"),
					MTrackItem5 = row.MField<string>("MTrackItem5")
				};
				if (includeCheckTypeValue)
				{
					gLCheckGroupValueModel.MContactName = row.Field<string>("MContactName");
					gLCheckGroupValueModel.MEmployeeName = row.Field<string>("MEmployeeName");
					gLCheckGroupValueModel.MMerItemName = row.Field<string>("MMerItemName");
					gLCheckGroupValueModel.MExpItemName = row.Field<string>("MExpItemName");
					gLCheckGroupValueModel.MPaItemGroupName = row.Field<string>("MPaItemGroupName");
					gLCheckGroupValueModel.MPaItemName = row.Field<string>("MPaItemName");
					gLCheckGroupValueModel.MTrackItem1Name = row.Field<string>("MTrackItem1Name");
					gLCheckGroupValueModel.MTrackItem2Name = row.Field<string>("MTrackItem2Name");
					gLCheckGroupValueModel.MTrackItem3Name = row.Field<string>("MTrackItem3Name");
					gLCheckGroupValueModel.MTrackItem4Name = row.Field<string>("MTrackItem4Name");
					gLCheckGroupValueModel.MTrackItem5Name = row.Field<string>("MTrackItem5Name");
					if (dt.Columns.Contains("MTrackItem1GroupName"))
					{
						gLCheckGroupValueModel.MTrackItem1GroupName = row.MField<string>("MTrackItem1GroupName");
					}
					if (dt.Columns.Contains("MTrackItem2GroupName"))
					{
						gLCheckGroupValueModel.MTrackItem2GroupName = row.MField<string>("MTrackItem2GroupName");
					}
					if (dt.Columns.Contains("MTrackItem3GroupName"))
					{
						gLCheckGroupValueModel.MTrackItem3GroupName = row.MField<string>("MTrackItem3GroupName");
					}
					if (dt.Columns.Contains("MTrackItem4GroupName"))
					{
						gLCheckGroupValueModel.MTrackItem4GroupName = row.MField<string>("MTrackItem4GroupName");
					}
					if (dt.Columns.Contains("MTrackItem5GroupName"))
					{
						gLCheckGroupValueModel.MTrackItem5GroupName = row.MField<string>("MTrackItem5GroupName");
					}
					if (dt.Columns.Contains("MAccountName"))
					{
						gLBalanceModel.MAccountName = row.MField<string>("MAccountName");
					}
				}
				gLBalanceModel.MAccountModel = bDAccountModel;
				gLBalanceModel.MCheckGroupValueModel = gLCheckGroupValueModel;
				list.Add(gLBalanceModel);
			}
			return list;
		}

		public int ExecuteSqlTran(MContext ctx, List<CommandInfo> cmdInfos)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.ExecuteSqlTran(cmdInfos);
		}

		private GLBalanceModel GetUpdateModel(List<GLBalanceModel> updateList, List<GLBalanceModel> balanceList, string accountId, GLVoucherEntryModel voucherEntry, bool isContact)
		{
			GLBalanceModel gLBalanceModel = null;
			if (isContact)
			{
				gLBalanceModel = (from x in updateList
				where x.MAccountID == voucherEntry.MAccountID && x.MCurrencyID == voucherEntry.MCurrencyID && x.MContactID == voucherEntry.MContactID
				select x).FirstOrDefault();
				if (gLBalanceModel == null)
				{
					gLBalanceModel = (from x in balanceList
					where x.MAccountID == voucherEntry.MAccountID && x.MCurrencyID == voucherEntry.MCurrencyID && x.MContactID == voucherEntry.MContactID
					select x).FirstOrDefault();
				}
			}
			else
			{
				gLBalanceModel = (from x in updateList
				where x.MAccountID == accountId && x.MCurrencyID == voucherEntry.MCurrencyID && (string.IsNullOrWhiteSpace(x.MContactID) || x.MContactID == "0")
				select x).FirstOrDefault();
				if (gLBalanceModel == null)
				{
					gLBalanceModel = (from x in balanceList
					where x.MAccountID == accountId && x.MCurrencyID == voucherEntry.MCurrencyID && (string.IsNullOrEmpty(x.MContactID) || x.MContactID == "0")
					select x).FirstOrDefault();
				}
			}
			return gLBalanceModel;
		}

		private GLBalanceModel ToGLBalanceModel(GLVoucherEntryModel voucherEntryModel, GLBalanceModel model, int dir, int year, int period, BDAccountModel account, bool isContact = false)
		{
			if (model == null)
			{
				model = new GLBalanceModel();
				model.MAccountID = (string.IsNullOrEmpty(account.MItemID) ? voucherEntryModel.MAccountID : account.MItemID);
				model.MCurrencyID = voucherEntryModel.MCurrencyID;
				model.MContactID = ((!isContact) ? "0" : voucherEntryModel.MContactID);
			}
			model.MYear = year;
			model.MPeriod = period;
			model.MYearPeriod = Convert.ToInt32(new DateTime(year, period, 1).ToString("yyyyMM"));
			model.MOrgID = account.MOrgID;
			if (voucherEntryModel.MDebit == decimal.Zero)
			{
				GLBalanceModel gLBalanceModel = model;
				gLBalanceModel.MCredit += voucherEntryModel.MAmount * (decimal)dir;
				GLBalanceModel gLBalanceModel2 = model;
				gLBalanceModel2.MCreditFor += voucherEntryModel.MAmountFor * (decimal)dir;
				GLBalanceModel gLBalanceModel3 = model;
				gLBalanceModel3.MYtdCredit += voucherEntryModel.MAmount * (decimal)dir;
				GLBalanceModel gLBalanceModel4 = model;
				gLBalanceModel4.MYtdCreditFor += voucherEntryModel.MAmountFor * (decimal)dir;
			}
			else if (voucherEntryModel.MCredit == decimal.Zero)
			{
				GLBalanceModel gLBalanceModel5 = model;
				gLBalanceModel5.MDebit += voucherEntryModel.MAmount * (decimal)dir;
				GLBalanceModel gLBalanceModel6 = model;
				gLBalanceModel6.MDebitFor += voucherEntryModel.MAmountFor * (decimal)dir;
				GLBalanceModel gLBalanceModel7 = model;
				gLBalanceModel7.MYtdDebit += voucherEntryModel.MAmount * (decimal)dir;
				GLBalanceModel gLBalanceModel8 = model;
				gLBalanceModel8.MYtdDebitFor += voucherEntryModel.MAmountFor * (decimal)dir;
			}
			return model;
		}

		public List<string> GetAccountParentIdByRecursion(MContext ctx, string childrenId, List<BDAccountModel> accountList)
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

		public List<BDAccountModel> GetAccountParentByRecursion(MContext ctx, string childrenId, List<BDAccountModel> accountList)
		{
			List<BDAccountModel> list = new List<BDAccountModel>();
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
			list.Add(bDAccountModel);
			list.AddRange(GetAccountParentByRecursion(ctx, bDAccountModel.MItemID, accountList));
			return list;
		}

		public DataSet GetAccountingPeriod(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select MYear , MPeriod from t_gl_balance where MOrgID='{0}' and MIsDelete = 0 group by MYear,MPeriod order by MYear desc , MPeriod desc", ctx.MOrgID);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Query(stringBuilder.ToString());
		}

		public List<GLBalanceModel> GetGLBalancesList(MContext ctx, SqlWhere filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT a.* , c.MDC ,convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName FROM t_gl_balance a ", "JieNor-001");
			stringBuilder.AppendLine(" inner join t_bd_account c on c.MItemID=a.MAccountID and c.MOrgID=@MOrgID and c.MIsDelete = 0  and a.MIsDelete=0 and a.MOrgID=@MOrgID");
			stringBuilder.AppendLine(" Left JOIN t_bd_contacts_l b on b.MIsDelete = 0  and  a.MContactID=b.MParentID and b.MLocaleID=@MLocaleID and b.MOrgID=@MOrgID");
			if (!string.IsNullOrEmpty(filter.WhereSqlString))
			{
				stringBuilder.AppendLine(filter.WhereSqlString);
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6));
			arrayList.Add(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36));
			if (filter.Parameters.Length != 0)
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
			return ModelInfoManager.DataTableToList<GLBalanceModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public CommandInfo GetDeleteAllBalanceCmd(MContext ctx)
		{
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_gl_balance set MIsDelete = 1 where MOrgID=@MOrgID and MYear=@MYear and MPeriod=@MPeriod and MIsDelete = 0  ";
			CommandInfo commandInfo2 = commandInfo;
			MySqlParameter[] obj = new MySqlParameter[3]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				null,
				null
			};
			MySqlParameter obj2 = new MySqlParameter
			{
				ParameterName = "@MYear"
			};
			DateTime dateNow = ctx.DateNow;
			obj2.Value = dateNow.Year;
			obj[1] = obj2;
			MySqlParameter obj3 = new MySqlParameter
			{
				ParameterName = "@MPeriod"
			};
			dateNow = ctx.DateNow;
			obj3.Value = dateNow.Month;
			obj[2] = obj3;
			DbParameter[] array = commandInfo2.Parameters = obj;
			return commandInfo;
		}

		public List<GLBalanceModel> GetBalanceByAccountID(MContext ctx, string accountID, int? year, int? period, bool isSummary = true)
		{
			List<GLBalanceModel> balanceListByAccountIDs = GetBalanceListByAccountIDs(ctx, new List<string>
			{
				accountID
			}, year, period, isSummary);
			if (balanceListByAccountIDs == null || balanceListByAccountIDs.Count == 0)
			{
				return new List<GLBalanceModel>
				{
					new GLBalanceModel()
				};
			}
			return balanceListByAccountIDs;
		}

		public List<GLBalanceModel> GetBalanceListByAccountIDs(MContext ctx, List<string> accountIDs, int? year, int? period, bool isSummary = true)
		{
			SqlWhere sqlWhere = new SqlWhere().AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID).AddFilter("MAccountID", SqlOperators.In, accountIDs);
			if (isSummary)
			{
				sqlWhere.AddFilter("MCheckGroupValueID", SqlOperators.Equal, "0");
			}
			sqlWhere.Equal("MIsDelete", 0);
			if (year.HasValue)
			{
				sqlWhere.AddFilter("MYear", SqlOperators.Equal, year.Value);
			}
			if (period.HasValue)
			{
				sqlWhere.AddFilter("MPeriod", SqlOperators.Equal, period.Value);
			}
			return GetModelList(ctx, sqlWhere, true);
		}

		public List<GLBalanceModel> GetPeriodBalanceListByAccountIDs(MContext ctx, List<string> accountIDs, int startYearPeriod, int endYearPeriod)
		{
			string str = "'" + string.Join("','", accountIDs) + "'";
			string sql = "select t1.* from t_Gl_balance t1\r\n                            where MAccountID in (" + str + ")\r\n                            and (t1.MYear * 12 + t1.MPeriod) >= @StartYearPeriod\r\n                            and (t1.MYear * 12 + t1.MPeriod) <= @EndYearPeriod\r\n                            and MCheckGroupValueID = '0'\r\n                            and t1.MOrgID = @MOrgID\r\n                            and t1.MIsDelete = 0 ";
			MySqlParameter[] source = new MySqlParameter[3]
			{
				new MySqlParameter
				{
					ParameterName = "@StartYearPeriod",
					Value = (object)startYearPeriod
				},
				new MySqlParameter
				{
					ParameterName = "@EndYearPeriod",
					Value = (object)endYearPeriod
				},
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			return ModelInfoManager.GetDataModelBySql<GLBalanceModel>(ctx, sql, source.ToArray());
		}

		public List<CommandInfo> GetDeleteBalanceCmds(MContext ctx)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_gl_balance set MIsDelete = 1 where MOrgID=@MOrgID and MIsDelete = 0 ";
			DbParameter[] array = commandInfo.Parameters = ctx.GetParameters((MySqlParameter)null);
			list.Add(commandInfo);
			return list;
		}

		public List<CommandInfo> GetDeleteBalanceCmdsByPeriod(MContext ctx, int minYear, int minPeriod, int maxYear, int maxPeriod)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			int num = maxYear * 100 + maxPeriod;
			int num2 = minYear * 100 + minPeriod;
			MySqlParameter[] parameters = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@minYearPeriod",
					Value = (object)num2
				}
			};
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_gl_balance set MIsDelete=1 where MOrgID=@MOrgID and MYearPeriod>@minYearPeriod and MIsDelete=0";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}

		private decimal GetRealYTD(decimal calculateValue, decimal balanceValue)
		{
			return balanceValue - calculateValue + calculateValue;
		}

		public DataTable GetYearAggregateAmount(MContext ctx, int year, int period, string accountId, string contactId, string currency)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT SUM(MCredit) as MYtdCredit , SUM(MCreditFor) as MYtdCreditFor , SUM(MDebit) as MYtdDebit , SUM(MDebitFor) as MYtdDebitFor\r\n                                  FROM t_gl_balance where MIsDelete = 0 and  MYear=@Year and MPeriod >=1 and MPeriod<=@Period and MOrgID=@MOrgID and MAccountID=@AccountID and MContactID=@ContactID and MCurrencyID=@Currency");
			MySqlParameter[] array = new MySqlParameter[6]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@AccountID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Year", MySqlDbType.Int32, 36),
				new MySqlParameter("@Period", MySqlDbType.Int32, 36),
				new MySqlParameter("@ContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Currency", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = accountId;
			array[2].Value = year;
			array[3].Value = period;
			array[4].Value = contactId;
			array[5].Value = currency;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			return (dataSet == null || dataSet.Tables == null) ? null : dataSet.Tables[0];
		}

		public List<GLBalanceModel> GetBalanceByPeriods(MContext ctx, List<DateTime> periods = null, List<string> accountIDS = null)
		{
			string text = "select t1.*,t2.MDC from t_gl_balance t1 \r\n                           inner join t_bd_account t2 on t1.MAccountID = t2.MItemID and t1.MOrgID = t2.MOrgID and t2.MIsDelete = 0  \r\n                           where t1.MOrgID = @MOrgID and t1.MIsDelete = 0 ";
			if (periods != null && periods.Count > 0)
			{
				text += " and (t1.MYear * 12 +  t1.MPeriod) in (";
				for (int i = 0; i < periods.Count; i++)
				{
					string arg = text;
					DateTime dateTime = periods[i];
					int num = dateTime.Year * 12;
					dateTime = periods[i];
					text = arg + (num + dateTime.Month) + ",";
				}
				text = text.TrimEnd(',') + ")";
			}
			if (accountIDS != null && accountIDS.Count > 0)
			{
				text += " and t1.MAccountID in (";
				for (int j = 0; j < accountIDS.Count; j++)
				{
					text = text + "'" + accountIDS[j] + "' ,";
				}
				text = text.TrimEnd(',') + ")";
			}
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			return ModelInfoManager.GetDataModelBySql<GLBalanceModel>(ctx, text, cmdParms);
		}

		public List<CommandInfo> GetApproveVoucherBalanceCmds(MContext ctx, List<string> voucherIdList, int approveStatus)
		{
			GLVoucherRepository gLVoucherRepository = new GLVoucherRepository();
			List<GLVoucherModel> voucherModelList = gLVoucherRepository.GetVoucherModelList(ctx, voucherIdList, false, 0, 0);
			return GetApproveVoucherBalanceCmds(ctx, voucherModelList, approveStatus);
		}

		public List<CommandInfo> GetApproveVoucherBalanceCmds(MContext ctx, List<GLVoucherModel> voucherList, int approveStatus)
		{
			List<GLBalanceModel> updateBalanceList = GetUpdateBalanceList(ctx, voucherList, approveStatus);
			return GetUpdateBalanceCmds(ctx, updateBalanceList);
		}

		public List<GLBalanceModel> GetUpdateBalanceList(MContext ctx, List<GLVoucherModel> voucherList, int approveStatus)
		{
			GLVoucherRepository gLVoucherRepository = new GLVoucherRepository();
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			if (voucherList == null || voucherList.Count() == 0)
			{
				return list;
			}
			foreach (GLVoucherModel voucher in voucherList)
			{
				List<GLVoucherEntryModel> mVoucherEntrys = voucher.MVoucherEntrys;
				foreach (GLVoucherEntryModel item in mVoucherEntrys)
				{
					GLBalanceModel balanceByVoucherEntry = GetBalanceByVoucherEntry(item, approveStatus);
					balanceByVoucherEntry.MYear = voucher.MYear;
					balanceByVoucherEntry.MPeriod = voucher.MPeriod;
					balanceByVoucherEntry.MYearPeriod = balanceByVoucherEntry.MYear * 100 + balanceByVoucherEntry.MPeriod;
					list.Add(balanceByVoucherEntry);
				}
			}
			return list;
		}

		public List<GLBalanceModel> GetUpdateBalanceList(MContext ctx, List<GLBalanceModel> balanceList, List<GLBalanceModel> balanceInDB = null, bool isProcessParent = true)
		{
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountModel> accountWithParentList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountWithParentList;
			if (accountWithParentList == null)
			{
				return list;
			}
			IEnumerable<IGrouping<string, GLBalanceModel>> enumerable = from x in balanceList
			group x by x.MAccountID;
			int mYearPeriod = balanceList.First().MYearPeriod;
			List<string> list2 = (from x in balanceList
			select x.MAccountID).Distinct().ToList();
			List<string> list3 = new List<string>();
			foreach (string item in list2)
			{
				List<BDAccountModel> accountParentByRecursion = GetAccountParentByRecursion(ctx, item, accountWithParentList);
				if (accountParentByRecursion != null && accountParentByRecursion.Count() != 0)
				{
					List<string> second = (from x in accountParentByRecursion
					select x.MItemID).Distinct().ToList();
					list3 = list3.Concat(second).ToList();
				}
			}
			list2 = list2.Concat(list3).ToList();
			List<GLBalanceModel> list4 = balanceInDB;
			if (balanceInDB == null)
			{
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MYearPeriod", mYearPeriod);
				sqlWhere.In("MAccountID", list2);
				list4 = GetBalanceListIncludeCheckGroupValue(ctx, sqlWhere, false);
			}
			if (list4 == null)
			{
				list4 = new List<GLBalanceModel>();
			}
			foreach (IGrouping<string, GLBalanceModel> item2 in enumerable)
			{
				string accountId = item2.Key;
				BDAccountModel bDAccountModel = (from x in accountWithParentList
				where x.MItemID == accountId
				select x).FirstOrDefault();
				if (bDAccountModel != null)
				{
					List<string> planUpdateAccountIdList = new List<string>();
					planUpdateAccountIdList.Add(accountId);
					List<BDAccountModel> accountParentByRecursion2 = GetAccountParentByRecursion(ctx, bDAccountModel.MItemID, accountWithParentList);
					if (accountParentByRecursion2 != null && accountParentByRecursion2.Count() > 0)
					{
						planUpdateAccountIdList.AddRange((from x in accountParentByRecursion2
						select x.MItemID).ToList());
					}
					List<GLBalanceModel> list5 = (from x in list4
					where planUpdateAccountIdList.Contains(x.MAccountID)
					select x).ToList();
					list5 = ((list5 == null) ? new List<GLBalanceModel>() : list5);
					balanceList = item2.ToList();
					foreach (GLBalanceModel balance in balanceList)
					{
						if (string.IsNullOrWhiteSpace(balance.MCheckGroupValueID))
						{
							balance.MCheckGroupValueModel = ((balance.MCheckGroupValueModel == null) ? new GLCheckGroupValueModel() : balance.MCheckGroupValueModel);
							GLCheckGroupValueModel checkGroupValueModel = new GLUtility().GetCheckGroupValueModel(ctx, balance.MCheckGroupValueModel);
							balance.MCheckGroupValueID = checkGroupValueModel.MItemID;
						}
						ProcessUpdateBalanceList(ctx, balance, list5, list, bDAccountModel);
						if (isProcessParent)
						{
							foreach (BDAccountModel item3 in accountParentByRecursion2)
							{
								ProcessUpdateBalanceList(ctx, balance, list5, list, item3);
							}
						}
					}
				}
			}
			return list;
		}

		public List<CommandInfo> GetUpdateBalanceCmds(MContext ctx, List<GLBalanceModel> balanceList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<GLBalanceModel> updateBalanceList = GetUpdateBalanceList(ctx, balanceList, null, true);
			if (updateBalanceList.Count() > 0)
			{
				list.AddRange(ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, updateBalanceList, null, null));
			}
			return list;
		}

		public void ProcessUpdateBalanceList(MContext ctx, GLBalanceModel balance, List<GLBalanceModel> balanceListInDB, List<GLBalanceModel> planUpdateList, BDAccountModel accountModel)
		{
			string accountId = accountModel.MItemID;
			GLBalanceModel balanceInDB = (from x in planUpdateList
			where x.MAccountID == accountId && x.MCurrencyID == balance.MCurrencyID && x.MCheckGroupValueID == balance.MCheckGroupValueID
			select x).FirstOrDefault();
			if (balanceInDB == null)
			{
				balanceInDB = (from x in balanceListInDB
				where x.MAccountID == accountId && x.MCurrencyID == balance.MCurrencyID && x.MCheckGroupValueID == balance.MCheckGroupValueID
				select x).FirstOrDefault();
			}
			if (balanceInDB == null)
			{
				balanceInDB = new GLBalanceModel();
				balanceInDB.MCheckGroupValueID = balance.MCheckGroupValueID;
				balanceInDB.MCheckGroupValueModel = balance.MCheckGroupValueModel;
				balanceInDB.MItemID = UUIDHelper.GetGuid();
				balanceInDB.MAccountID = accountId;
				balanceInDB.MCurrencyID = balance.MCurrencyID;
				balanceInDB.IsNew = true;
			}
			GLBalanceModel gLBalanceModel = balanceInDB;
			gLBalanceModel.MDebit += balance.MDebit;
			GLBalanceModel gLBalanceModel2 = balanceInDB;
			gLBalanceModel2.MDebitFor += balance.MDebitFor;
			GLBalanceModel gLBalanceModel3 = balanceInDB;
			gLBalanceModel3.MCredit += balance.MCredit;
			GLBalanceModel gLBalanceModel4 = balanceInDB;
			gLBalanceModel4.MCreditFor += balance.MCreditFor;
			GLBalanceModel gLBalanceModel5 = balanceInDB;
			gLBalanceModel5.MYtdCredit += balance.MCredit;
			GLBalanceModel gLBalanceModel6 = balanceInDB;
			gLBalanceModel6.MYtdCreditFor += balance.MCreditFor;
			GLBalanceModel gLBalanceModel7 = balanceInDB;
			gLBalanceModel7.MYtdDebit += balance.MDebit;
			GLBalanceModel gLBalanceModel8 = balanceInDB;
			gLBalanceModel8.MYtdDebitFor += balance.MDebitFor;
			balanceInDB.MEndBalance = ((accountModel.MDC == 1) ? (balanceInDB.MBeginBalance + balanceInDB.MDebit - balanceInDB.MCredit) : (balanceInDB.MBeginBalance + balanceInDB.MCredit - balanceInDB.MDebit));
			balanceInDB.MEndBalanceFor = ((accountModel.MDC == 1) ? (balanceInDB.MBeginBalanceFor + balanceInDB.MDebitFor - balanceInDB.MCreditFor) : (balanceInDB.MBeginBalanceFor + balanceInDB.MCreditFor - balanceInDB.MDebitFor));
			balanceInDB.MYear = balance.MYear;
			balanceInDB.MPeriod = balance.MPeriod;
			balanceInDB.MYearPeriod = balance.MYearPeriod;
			balanceInDB.MDC = accountModel.MDC;
			GLBalanceModel summaryBalanceInDB = null;
			summaryBalanceInDB = (from x in planUpdateList
			where x.MAccountID == accountId && x.MCurrencyID == balance.MCurrencyID && x.MCheckGroupValueID == "0"
			select x).FirstOrDefault();
			if (summaryBalanceInDB == null)
			{
				summaryBalanceInDB = (from x in balanceListInDB
				where x.MAccountID == accountId && x.MCurrencyID == balance.MCurrencyID && x.MCheckGroupValueID == "0"
				select x).FirstOrDefault();
			}
			if (summaryBalanceInDB == null)
			{
				summaryBalanceInDB = new GLBalanceModel();
				summaryBalanceInDB.MCurrencyID = balance.MCurrencyID;
				summaryBalanceInDB.MItemID = UUIDHelper.GetGuid();
				summaryBalanceInDB.MAccountID = accountId;
				summaryBalanceInDB.MCheckGroupValueID = "0";
				summaryBalanceInDB.IsNew = true;
			}
			GLBalanceModel gLBalanceModel9 = summaryBalanceInDB;
			gLBalanceModel9.MDebit += balance.MDebit;
			GLBalanceModel gLBalanceModel10 = summaryBalanceInDB;
			gLBalanceModel10.MDebitFor += balance.MDebitFor;
			GLBalanceModel gLBalanceModel11 = summaryBalanceInDB;
			gLBalanceModel11.MCredit += balance.MCredit;
			GLBalanceModel gLBalanceModel12 = summaryBalanceInDB;
			gLBalanceModel12.MCreditFor += balance.MCreditFor;
			GLBalanceModel gLBalanceModel13 = summaryBalanceInDB;
			gLBalanceModel13.MYtdCredit += balance.MCredit;
			GLBalanceModel gLBalanceModel14 = summaryBalanceInDB;
			gLBalanceModel14.MYtdCreditFor += balance.MCreditFor;
			GLBalanceModel gLBalanceModel15 = summaryBalanceInDB;
			gLBalanceModel15.MYtdDebit += balance.MDebit;
			GLBalanceModel gLBalanceModel16 = summaryBalanceInDB;
			gLBalanceModel16.MYtdDebitFor += balance.MDebitFor;
			summaryBalanceInDB.MEndBalance = ((accountModel.MDC == 1) ? (summaryBalanceInDB.MBeginBalance + summaryBalanceInDB.MDebit - summaryBalanceInDB.MCredit) : (summaryBalanceInDB.MBeginBalance + summaryBalanceInDB.MCredit - summaryBalanceInDB.MDebit));
			summaryBalanceInDB.MEndBalanceFor = ((accountModel.MDC == 1) ? (summaryBalanceInDB.MBeginBalanceFor + summaryBalanceInDB.MDebitFor - summaryBalanceInDB.MCreditFor) : (summaryBalanceInDB.MBeginBalanceFor + summaryBalanceInDB.MCreditFor - summaryBalanceInDB.MDebitFor));
			summaryBalanceInDB.MYear = balance.MYear;
			summaryBalanceInDB.MPeriod = balance.MPeriod;
			summaryBalanceInDB.MYearPeriod = balance.MYearPeriod;
			summaryBalanceInDB.MDC = accountModel.MDC;
			if (!planUpdateList.Exists((GLBalanceModel x) => x.MItemID == balanceInDB.MItemID))
			{
				planUpdateList.Add(balanceInDB);
			}
			if (!planUpdateList.Exists((GLBalanceModel x) => x.MItemID == summaryBalanceInDB.MItemID))
			{
				planUpdateList.Add(summaryBalanceInDB);
			}
		}

		private GLBalanceModel GetBalanceByVoucherEntry(GLVoucherEntryModel voucherEntry, int approveStatus)
		{
			GLBalanceModel gLBalanceModel = new GLBalanceModel();
			gLBalanceModel.MDebit = voucherEntry.MDebit * (decimal)approveStatus;
			gLBalanceModel.MDebitFor = voucherEntry.MDebitFor * (decimal)approveStatus;
			gLBalanceModel.MCredit = voucherEntry.MCredit * (decimal)approveStatus;
			gLBalanceModel.MCreditFor = voucherEntry.MCreditFor * (decimal)approveStatus;
			gLBalanceModel.MCurrencyID = voucherEntry.MCurrencyID;
			gLBalanceModel.MAccountID = voucherEntry.MAccountID;
			gLBalanceModel.MCheckGroupValueID = voucherEntry.MCheckGroupValueID;
			gLBalanceModel.MCheckGroupValueModel = voucherEntry.MCheckGroupValueModel;
			return gLBalanceModel;
		}

		public List<CommandInfo> GetSettlementBalanceCmds(MContext ctx, List<DateTime> timeList, int settlementStatus)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			timeList.Add(timeList.Max().AddMonths(1));
			DateTime dateTime = timeList.Min();
			DateTime dateTime2 = timeList.Max();
			int year = dateTime.Year;
			int month = dateTime.Month;
			int year2 = dateTime2.Year;
			int month2 = dateTime2.Month;
			if (settlementStatus == 1)
			{
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.GreaterOrEqual("MYearPeriod", Convert.ToInt32(dateTime.ToString("yyyyMM")));
				sqlWhere.LessOrEqual("MYearPeriod", Convert.ToInt32(dateTime2.ToString("yyyyMM")));
				List<GLBalanceModel> balanceListIncludeCheckGroupValue = GetBalanceListIncludeCheckGroupValue(ctx, sqlWhere, false);
				if (balanceListIncludeCheckGroupValue == null)
				{
					throw new Exception("not balance");
				}
				List<GLBalanceModel> list2 = new List<GLBalanceModel>();
				for (int i = 0; i < timeList.Count(); i++)
				{
					DateTime checkoutPeriod = timeList[i];
					List<GLBalanceModel> checkoutPeriodBalanceList = GetCheckoutPeriodBalanceList(checkoutPeriod, balanceListIncludeCheckGroupValue, list2);
					foreach (GLBalanceModel item in checkoutPeriodBalanceList)
					{
						GLBalanceModel settlementBalance = GetSettlementBalance(ctx, item, list2);
						list2.Add(settlementBalance);
					}
				}
				if (list2 != null && list2.Count > 0)
				{
					list.AddRange(ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, list2, null, null));
				}
			}
			else
			{
				list.AddRange(GetDeleteBalanceCmdsByPeriod(ctx, year, month, year2, month2));
			}
			return list;
		}

		private List<GLBalanceModel> GetCheckoutPeriodBalanceList(DateTime checkoutPeriod, List<GLBalanceModel> balanceListInDB, List<GLBalanceModel> planUpdateBalanceList)
		{
			List<GLBalanceModel> list = (from x in balanceListInDB
			where x.MYear == checkoutPeriod.Year && x.MPeriod == checkoutPeriod.Month
			select x).ToList();
			if (list == null)
			{
				list = new List<GLBalanceModel>();
			}
			DateTime preTime = checkoutPeriod.AddMonths(-1);
			List<GLBalanceModel> list2 = (from x in planUpdateBalanceList
			where x.MYear == preTime.Year && x.MPeriod == preTime.Month
			select x).ToList();
			if (list2 == null || list2.Count() == 0)
			{
				return list;
			}
			foreach (GLBalanceModel item in list2)
			{
				if (!list.Exists((GLBalanceModel x) => x.MAccountID == item.MAccountID && x.MCurrencyID == item.MCurrencyID && x.MCheckGroupValueID == item.MCheckGroupValueID))
				{
					GLBalanceModel gLBalanceModel = new GLBalanceModel();
					gLBalanceModel.MAccountID = item.MAccountID;
					gLBalanceModel.MContactID = item.MContactID;
					gLBalanceModel.MCurrencyID = item.MCurrencyID;
					gLBalanceModel.MYear = checkoutPeriod.Year;
					gLBalanceModel.MPeriod = checkoutPeriod.Month;
					gLBalanceModel.MYearPeriod = Convert.ToInt32(checkoutPeriod.ToString("yyyyMM"));
					gLBalanceModel.MOrgID = item.MOrgID;
					gLBalanceModel.MCredit = decimal.Zero;
					gLBalanceModel.MCreditFor = decimal.Zero;
					gLBalanceModel.MDebit = decimal.Zero;
					gLBalanceModel.MDebitFor = decimal.Zero;
					gLBalanceModel.MYtdDebitFor = decimal.Zero;
					gLBalanceModel.MYtdDebit = decimal.Zero;
					gLBalanceModel.MYtdCredit = decimal.Zero;
					gLBalanceModel.MYtdCreditFor = decimal.Zero;
					gLBalanceModel.MCheckGroupValueID = item.MCheckGroupValueID;
					gLBalanceModel.MCheckGroupValueModel = item.MCheckGroupValueModel;
					list.Add(gLBalanceModel);
				}
			}
			return list;
		}

		private GLBalanceModel GetSettlementBalance(MContext ctx, GLBalanceModel balance, List<GLBalanceModel> updateList)
		{
			DateTime preTime = new DateTime(balance.MYear, balance.MPeriod, 1).AddMonths(-1);
			GLBalanceModel gLBalanceModel = (from x in updateList
			where x.MAccountID == balance.MAccountID && x.MCurrencyID == balance.MCurrencyID && x.MCheckGroupValueID == balance.MCheckGroupValueID && x.MYear == preTime.Year && x.MPeriod == preTime.Month
			select x).FirstOrDefault();
			balance.MBeginBalance = (gLBalanceModel?.MEndBalance ?? balance.MBeginBalance);
			balance.MBeginBalanceFor = (gLBalanceModel?.MEndBalanceFor ?? balance.MBeginBalanceFor);
			if (balance.MPeriod == 1)
			{
				balance.MYtdCredit = balance.MYtdCredit;
				balance.MYtdCreditFor = balance.MYtdCreditFor;
				balance.MYtdDebit = balance.MYtdDebit;
				balance.MYtdDebitFor = balance.MYtdDebitFor;
			}
			else
			{
				balance.MYtdCredit = ((gLBalanceModel == null) ? balance.MYtdCredit : (gLBalanceModel.MYtdCredit + balance.MCredit));
				balance.MYtdCreditFor = ((gLBalanceModel == null) ? balance.MYtdCreditFor : (gLBalanceModel.MYtdCreditFor + balance.MCreditFor));
				balance.MYtdDebit = ((gLBalanceModel == null) ? balance.MYtdDebit : (gLBalanceModel.MYtdDebit + balance.MDebit));
				balance.MYtdDebitFor = ((gLBalanceModel == null) ? balance.MYtdDebitFor : (gLBalanceModel.MYtdDebitFor + balance.MDebitFor));
			}
			if (balance.MDC == 1)
			{
				balance.MEndBalance = balance.MBeginBalance + balance.MDebit - balance.MCredit;
				balance.MEndBalanceFor = balance.MBeginBalanceFor + balance.MDebitFor - balance.MCreditFor;
			}
			else
			{
				balance.MEndBalance = balance.MBeginBalance + balance.MCredit - balance.MDebit;
				balance.MEndBalanceFor = balance.MBeginBalanceFor + balance.MCreditFor - balance.MDebitFor;
			}
			return balance;
		}

		public static GLBalanceModel GetBalanceFromPrePeriod(GLBalanceModel preBalance, int year, int period)
		{
			GLBalanceModel gLBalanceModel = new GLBalanceModel();
			gLBalanceModel.MItemID = UUIDHelper.GetGuid();
			gLBalanceModel.MYear = year;
			gLBalanceModel.MPeriod = period;
			gLBalanceModel.MYearPeriod = year * 100 + period;
			gLBalanceModel.MCheckGroupID = preBalance.MCheckGroupID;
			gLBalanceModel.MCheckGroupValueID = preBalance.MCheckGroupValueID;
			gLBalanceModel.MCheckGroupValueModel = preBalance.MCheckGroupValueModel;
			gLBalanceModel.MAccountID = preBalance.MAccountID;
			gLBalanceModel.MAccountModel = preBalance.MAccountModel;
			gLBalanceModel.MAccountName = preBalance.MAccountName;
			gLBalanceModel.MDC = preBalance.MDC;
			gLBalanceModel.MCurrencyID = preBalance.MCurrencyID;
			gLBalanceModel.MBeginBalance = (preBalance?.MEndBalance ?? gLBalanceModel.MBeginBalance);
			gLBalanceModel.MBeginBalanceFor = (preBalance?.MEndBalanceFor ?? gLBalanceModel.MBeginBalanceFor);
			if (gLBalanceModel.MPeriod == 1)
			{
				gLBalanceModel.MYtdCredit = gLBalanceModel.MYtdCredit;
				gLBalanceModel.MYtdCreditFor = gLBalanceModel.MYtdCreditFor;
				gLBalanceModel.MYtdDebit = gLBalanceModel.MYtdDebit;
				gLBalanceModel.MYtdDebitFor = gLBalanceModel.MYtdDebitFor;
			}
			else
			{
				gLBalanceModel.MYtdCredit = ((preBalance == null) ? gLBalanceModel.MYtdCredit : (preBalance.MYtdCredit + gLBalanceModel.MCredit));
				gLBalanceModel.MYtdCreditFor = ((preBalance == null) ? gLBalanceModel.MYtdCreditFor : (preBalance.MYtdCreditFor + gLBalanceModel.MCreditFor));
				gLBalanceModel.MYtdDebit = ((preBalance == null) ? gLBalanceModel.MYtdDebit : (preBalance.MYtdDebit + gLBalanceModel.MDebit));
				gLBalanceModel.MYtdDebitFor = ((preBalance == null) ? gLBalanceModel.MYtdDebitFor : (preBalance.MYtdDebitFor + gLBalanceModel.MDebitFor));
			}
			if (gLBalanceModel.MDC == 1)
			{
				gLBalanceModel.MEndBalance = gLBalanceModel.MBeginBalance + gLBalanceModel.MDebit - gLBalanceModel.MCredit;
				gLBalanceModel.MEndBalanceFor = gLBalanceModel.MBeginBalanceFor + gLBalanceModel.MDebitFor - gLBalanceModel.MCreditFor;
			}
			else
			{
				gLBalanceModel.MEndBalance = gLBalanceModel.MBeginBalance + gLBalanceModel.MCredit - gLBalanceModel.MDebit;
				gLBalanceModel.MEndBalanceFor = gLBalanceModel.MBeginBalanceFor + gLBalanceModel.MCreditFor - gLBalanceModel.MDebitFor;
			}
			return gLBalanceModel;
		}
	}
}
