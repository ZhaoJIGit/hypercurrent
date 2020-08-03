using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLExcelRepository
	{
		public GLVoucherRepository voucher = new GLVoucherRepository();

		public List<GLVoucherModel> GetVoucherListByFilter(MContext ctx, GLBalanceListFilterModel filter)
		{
			List<GLVoucherModel> result = new List<GLVoucherModel>();
			GLVoucherListFilterModel filter2 = new GLVoucherListFilterModel
			{
				MNumber = filter.MNumber,
				IncludeCheckType = filter.IncludeCheckType,
				CheckTypeValueList = filter.CheckTypeValueList,
				Status = filter.Status,
				From = filter.From
			};
			string voucherSelectSql = voucher.GetVoucherSelectSql(filter2, false, false);
			voucherSelectSql += " WHERE  t1.MIsDelete=0 ";
			if (filter.StartPeriod > 0)
			{
				voucherSelectSql += "  AND (t1.MYear * 100 + t1.MPeriod) >= @StartPeriod ";
			}
			if (filter.EndPeriod > 0)
			{
				voucherSelectSql += " AND (t1.MYear * 100 + t1.MPeriod) <= @EndPeriod ";
			}
			if (filter.AccountIDS != null && filter.AccountIDS.Count > 0)
			{
				List<string> newList = new List<string>();
				filter.AccountIDS.ForEach(delegate(string x)
				{
					newList.Add("'" + x + "'");
				});
				string str = string.Join(",", newList);
				voucherSelectSql = voucherSelectSql + "AND t4.MItemID IN( " + str + " )";
			}
			if (filter != null && filter.IncludeCheckType && filter.CheckTypeValueList != null && filter.CheckTypeValueList.Count() > 0)
			{
				voucherSelectSql += GetCheckTypeValueFilterSql(filter.CheckTypeValueList, "t5");
			}
			voucherSelectSql += " ORDER BY t1.MItemID,t1.MYear,t1.MPeriod,t1.MNumber,t2.Mentryseq ";
			MySqlParameter[] parameter = GetParameter(ctx, filter);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(voucherSelectSql, parameter.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				result = voucher.DataTable2VoucherList(ctx, dataSet.Tables[0]);
			}
			return result;
		}

		public List<GLBalanceModel> GetBalanceListByCheckGroupFilter(MContext ctx, GLBalanceListFilterModel filter)
		{
			List<GLBalanceModel> result = new List<GLBalanceModel>();
			string balanceListQueryCheckGroupSql = GetBalanceListQueryCheckGroupSql(filter);
			MySqlParameter[] parameter = GetParameter(ctx, filter);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(balanceListQueryCheckGroupSql, parameter);
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				result = ((!filter.IncludeCheckType) ? ModelInfoManager.DataTableToList<GLBalanceModel>(dataSet.Tables[0]) : new GLBalanceRepository().BindDataset2InitBalance(ctx, dataSet.Tables[0], true, false));
			}
			return result;
		}

		private string GetBalanceListQueryCheckGroupSql(GLBalanceListFilterModel filter)
		{
			string empty = string.Empty;
			empty += " SELECT t1.*,t3.MNumber,t3.MDC , t3.MCheckGroupID,t4.MName AS MAccountName ,t4.MFullName as MAccountFullName , t3.MCode ";
			if (filter.IncludeCheckType)
			{
				empty += string.Format("\r\n                        ,t2.MContactID,\r\n                        t2.MEmployeeID,\r\n                        t2.MMerItemID,\r\n                        t2.MExpItemID,\r\n                        t2.MPaItemID,\r\n                        t2.MTrackItem1,\r\n                        t2.MTrackItem2,\r\n                        t2.MTrackItem3,\r\n                        t2.MTrackItem4,\r\n                        t2.MTrackItem5,\r\n                        convert(AES_DECRYPT(t6.MName,'{0}') using utf8) as MContactName,\r\n                        F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeName,\r\n                        concat(t8_0.MNumber,':',t8.MDesc) AS MMerItemName,\r\n                        t9.MName AS MExpItemName,\r\n                        t10.MName AS MPaItemName,\r\n                        t10_0.MGroupID AS MPaItemGroupID,t10_1.MName as MPaItemGroupName,\r\n                        t11.MName AS MTrackItem1Name,\r\n                        t11_2.MName AS MTrackItem1GroupName,\r\n                        t12.MName AS MTrackItem2Name,\r\n                        t12_2.MName AS MTrackItem2GroupName,\r\n                        t13.MName AS MTrackItem3Name,\r\n                        t13_2.MName AS MTrackItem3GroupName,\r\n                        t14.MName AS MTrackItem4Name,\r\n                        t14_2.MName AS MTrackItem4GroupName,\r\n                        t15.MName AS MTrackItem5Name,\r\n                        t15_2.MName AS MTrackItem5GroupName ", "JieNor-001");
			}
			empty += " FROM ( ";
			empty += " SELECT MItemID, MOrgID, MAccountID, MCheckGroupValueID, MYEAR, MPeriod, MCurrencyID,MBeginBalanceFor, MBeginBalance, \r\n                            MDebitFor, MDebit, MCreditFor, MCredit, MYtdDebitFor, MYtdDebit, MYtdCreditFor, MYtdCredit, MEndBalanceFor, MEndBalance, MYearPeriod,MIsActive,\r\n                            MIsDelete, MCreatorID, MCreateDate, MModifierID, MModifyDate FROM t_gl_balance ";
			empty += " WHERE MIsDelete = 0 AND MOrgID = @MOrgID ";
			filter.StartYearPeriod = filter.StartYear * 100 + filter.StartPeriod;
			filter.EndYearPeriod = filter.EndYear * 100 + filter.EndPeriod;
			if (filter.StartYearPeriod > 0)
			{
				empty += " AND MYearPeriod >= @StartYearPeriod  ";
			}
			if (filter.EndYearPeriod > 0)
			{
				empty += " AND MYearPeriod <= @EndYearPeriod ";
			}
			if (filter.AccountIDS != null && filter.AccountIDS.Count > 0)
			{
				empty = empty + "  AND MAccountID in ('" + string.Join("','", filter.AccountIDS.ToArray()) + "')  ";
			}
			if (!string.IsNullOrWhiteSpace(filter.MCurrencyID) && filter.MCurrencyID != "0")
			{
				empty += " AND MCurrencyID=@MCurrencyID ";
			}
			empty += " ) t1";
			empty += " INNER JOIN t_bd_account t3 ON t3.MItemID=t1.MAccountID ";
			empty += " INNER JOIN t_bd_account_l t4 ON t1.MAccountID = t4.MParentID  AND t4.MLocaleID=@MLCID ";
			if (filter.IncludeCheckType)
			{
				empty += " INNER JOIN  t_gl_checkgroupvalue t2 ON  t2.MOrgID=t1.MOrgID AND t2.MItemID = t1.MCheckGroupValueID  AND t2.MIsDelete=0 ";
				empty += GetCheckTypeValueFilterSql(filter.CheckTypeValueList, "t2");
				return empty + "LEFT JOIN \r\n                t_bd_contacts_l t6 \r\n                ON t6.MParentID = t2.MContactID AND t6.MLocaleID =@MLCID AND t6.MOrgID = t1.MOrgID  AND t6.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_employees_l t7 \r\n                ON t7.MParentID = t2.MEmployeeID AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID =@MLCID\r\n                LEFT JOIN\r\n                t_bd_item t8_0 ON t8_0.MItemID = t2.MMerItemID\r\n                    AND t8_0.MOrgID = t1.MOrgId\r\n                    AND t8_0.MIsDelete = t1.MIsDelete\r\n                LEFT JOIN \r\n                t_bd_item_l t8 \r\n                ON t8.MParentID = t2.MMerItemID AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID=@MLCID \r\n                LEFT JOIN \r\n                t_bd_expenseitem_l t9 \r\n                ON t9.MParentID = t2.MExpItemID AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID =@MLCID\r\n                LEFT JOIN \r\n                t_pa_payitem_l t10 \r\n                ON t10.MParentID = t2.MPaItemID AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID =@MLCID\r\n                LEFT JOIN\r\n                t_pa_payitem t10_0\r\n                ON t10_0.MItemID = t2.MPaItemID AND t10_0.MOrgID = t1.MOrgId AND t10_0.MIsDelete = t1.MIsDelete \r\n                LEFT JOIN \r\n                t_pa_payitemgroup_l t10_1 \r\n                ON t10_1.MParentID = t2.MPaItemID AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID =@MLCID\r\n\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t11 \r\n                ON t11.MParentID = t2.MTrackItem1 AND t11.MLocaleID =@MLCID AND t11.MOrgID = t1.MOrgID AND t11.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t11_1 \r\n                ON t11_1.MEntryID = t2.MTrackItem1 AND t11_1.MOrgID = t1.MOrgID AND t11_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t11_2 \r\n                ON t11_2.MParentID = t11_1.MItemID AND t11_2.MLocaleID =@MLCID AND t11_2.MOrgID = t1.MOrgID AND t11_2.MIsDelete = 0\r\n\r\n                 LEFT JOIN \r\n                t_bd_trackentry_l t12 \r\n                ON t12.MParentID = t2.MTrackItem2 AND t12.MLocaleID =@MLCID AND t12.MOrgID = t1.MOrgID AND t12.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t12_1 \r\n                ON t12_1.MEntryID = t2.MTrackItem2 AND t12_1.MOrgID = t1.MOrgID AND t12_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t12_2 \r\n                ON t12_2.MParentID = t12_1.MItemID AND t12_2.MLocaleID =@MLCID AND t12_2.MOrgID = t1.MOrgID AND t12_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t13 \r\n                ON t13.MParentID = t2.MTrackItem3 AND t13.MLocaleID =@MLCID AND t13.MOrgID = t1.MOrgID AND t13.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t13_1 \r\n                ON t13_1.MEntryID = t2.MTrackItem3 AND t13_1.MOrgID = t1.MOrgID AND t13_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t13_2 \r\n                ON t13_2.MParentID = t13_1.MItemID AND t13_2.MLocaleID = @MLCID AND t13_2.MOrgID = t1.MOrgID AND t13_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t14 \r\n                ON t14.MParentID = t2.MTrackItem4 AND t14.MLocaleID = @MLCID AND t14.MOrgID = t1.MOrgID AND t14.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t14_1 \r\n                ON t14_1.MEntryID = t2.MTrackItem4 AND t14_1.MOrgID = t1.MOrgID AND t14_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t14_2 \r\n                ON t14_2.MParentID = t14_1.MItemID AND t14_2.MLocaleID = @MLCID AND t14_2.MOrgID = t1.MOrgID AND t14_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t15 \r\n                ON t15.MParentID = t2.MTrackItem5 AND t15.MLocaleID = @MLCID AND t15.MOrgID = t1.MOrgID AND t15.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t15_1 \r\n                ON t15_1.MEntryID = t2.MTrackItem5 AND t15_1.MOrgID = t1.MOrgID AND t15_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t15_2 \r\n                ON t15_2.MParentID = t15_1.MItemID AND t15_2.MLocaleID = @MLCID AND t15_2.MOrgID = t1.MOrgID AND t15_2.MIsDelete = 0";
			}
			return empty + " WHERE t1.MCheckGroupValueID='0' OR t1.MCheckGroupValueID='' ";
		}

		private string GetCheckTypeValueFilterSql(List<NameValueModel> checkTypeValueList, string tableOtherName)
		{
			string text = "";
			if (checkTypeValueList == null || checkTypeValueList.Count() == 0)
			{
				return text;
			}
			IEnumerable<IGrouping<string, NameValueModel>> enumerable = from x in checkTypeValueList
			group x by x.MName;
			foreach (IGrouping<string, NameValueModel> item in enumerable)
			{
				int num = -1;
				if (int.TryParse(item.Key, out num))
				{
					string name = Enum.GetName(typeof(CheckTypeEnum), num);
					text += GetCheckTypeValueFilterString(tableOtherName, name, item.ToList());
				}
			}
			return text;
		}

		private string GetCheckTypeValueFilterString(string tablePrefix, string dbFieldName, List<NameValueModel> checkTypeValueList)
		{
			List<string> list = new List<string>();
			int num = 1;
			foreach (NameValueModel checkTypeValue in checkTypeValueList)
			{
				list.Add(string.Format(" {0}.{1}=@{1}{2} ", tablePrefix, dbFieldName, Convert.ToString(num)));
				num++;
			}
			return string.Format(" AND ( {0} ) ", string.Join(" OR ", list));
		}

		public List<GLBalanceModel> GetBalanceListByFilter(MContext ctx, GLBalanceListFilterModel filter)
		{
			List<GLBalanceModel> result = new List<GLBalanceModel>();
			string balanceListQuerySql = GetBalanceListQuerySql(filter);
			MySqlParameter[] parameter = GetParameter(ctx, filter);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(balanceListQuerySql, parameter);
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				result = ModelInfoManager.DataTableToList<GLBalanceModel>(dataSet.Tables[0]);
			}
			return result;
		}

		private string GetBalanceListQuerySql(GLBalanceListFilterModel filter)
		{
			string str = " SELECT t3.MYear,t3.MPeriod,t3.MYearPeriod,t1.MItemID AS MAccountID,\r\n\t                                t2.MName AS MAccountName,t1.MNumber,t1.MDC,\r\n\t                                concat(t1.MNumber, ' ', t2.MName) AS MAccountFullName,\r\n\t                                t1.MAccountTypeID, t4.MName AS MAccountTypeName,\r\n\t                                concat(t3.MCurrencyID, ' ',t6.MName) AS MCurrencyName,\r\n\t                                t3.MBeginBalance,t3.MBeginBalanceFor,t3.MEndBalance,\r\n\t                                t3.MEndBalanceFor,t3.MDebit,t3.MDebitFor,t3.MCredit,\r\n\t                                t3.MCreditFor,t3.MYtdDebit,t3.MYtdDebitFor,t3.MYtdCredit,\r\n\t                                t3.MYtdCreditFor,F_GETUSERNAME (t5.MFirstName, t5.MLastName) AS MContactName\r\n                            FROM  ";
			str += " ( SELECT MItemID,MNumber,MDC,MAccountTypeID,MOrgID,MIsDelete\r\n                             FROM t_bd_account \r\n                             WHERE MOrgID = @MOrgID AND MIsDelete = 0 ";
			if (filter.AccountIDS != null && filter.AccountIDS.Count > 0)
			{
				str = str + "  AND MItemID in ('" + string.Join("','", filter.AccountIDS.ToArray()) + "')  ";
			}
			if (filter.AccountTypes != null && filter.AccountTypes.Count > 0)
			{
				str = str + " AND  MAccountTypeID in ('" + string.Join("','", filter.AccountTypes.ToArray()) + "')  ";
			}
			str += " )t1 ";
			str += " INNER JOIN t_bd_account_l t2 ON t2.MParentID = t1.MItemID \r\n                           AND t2.MLocaleID = @MLCID AND t2.MOrgID = t1.MOrgID\r\n                           AND t2.MIsDelete = 0 \r\n                           INNER JOIN t_gl_balance t3 ON t3.MAccountID = t1.MItemID\r\n                           AND t3.MOrgID = t1.MOrgID AND t3.MCheckGroupValueID = '0'\r\n                           AND t3.MIsDelete = 0 \r\n                           INNER JOIN t_bas_accounttype_l t4 ON t4.MParentID = t1.MAccountTypeID\r\n                           AND t4.MLocaleID = t2.MLocaleID  AND t4.MIsDelete = 0 \r\n                           LEFT JOIN t_bd_contacts_L t5 ON t3.MContactID = t5.MParentID\r\n                           AND t5.MLocaleID = t2.MLocaleID AND t5.MOrgID = t1.MOrgID\r\n                           AND t5.MIsDelete = 0 \r\n                           LEFT JOIN t_bas_currency_l t6 ON t6.MParentID = t3.MCurrencyID\r\n                           AND t6.MLocaleID = t2.MLocaleID  AND t6.MIsDelete = 0 ";
			str += " WHERE  t1.MIsDelete = 0  ";
			if (filter.StartYearPeriod > 0)
			{
				str += " AND t3.MYearPeriod >= @StartYearPeriod  ";
			}
			if (filter.EndYearPeriod > 0)
			{
				str += " AND t3.MYearPeriod <= @EndYearPeriod ";
			}
			if (!string.IsNullOrWhiteSpace(filter.Order))
			{
				return str + " ORDER BY " + filter.Order;
			}
			return str + " ORDER BY t3.Myear asc , t3.MPeriod asc, t1.MNumber asc";
		}

		private MySqlParameter[] GetParameter(MContext ctx, GLBalanceListFilterModel filter)
		{
			List<MySqlParameter> list = new List<MySqlParameter>();
			list.Add(new MySqlParameter
			{
				ParameterName = "@MLCID",
				Value = ctx.MLCID,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MOrgID",
				Value = ctx.MOrgID,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@StartYearPeriod",
				Value = (object)filter.StartYearPeriod,
				MySqlDbType = MySqlDbType.Int32
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@StartPeriod",
				Value = (object)((filter.StartYearPeriod > 0) ? filter.StartYearPeriod : filter.StartPeriod),
				MySqlDbType = MySqlDbType.Int32
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@EndPeriod",
				Value = (object)((filter.EndYearPeriod > 0) ? filter.EndYearPeriod : filter.EndPeriod),
				MySqlDbType = MySqlDbType.Int32
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@EndYearPeriod",
				Value = (object)filter.EndYearPeriod,
				MySqlDbType = MySqlDbType.Int32
			});
			if (filter.CheckTypeValueList != null && filter.CheckTypeValueList.Count() > 0)
			{
				IEnumerable<IGrouping<string, NameValueModel>> enumerable = from x in filter.CheckTypeValueList
				group x by x.MName;
				foreach (IGrouping<string, NameValueModel> item2 in enumerable)
				{
					int num = -1;
					if (int.TryParse(item2.Key, out num))
					{
						List<NameValueModel> list2 = item2.ToList();
						int num2 = 1;
						string name = Enum.GetName(typeof(CheckTypeEnum), num);
						foreach (NameValueModel item3 in list2)
						{
							string parameterName = $"@{name}{Convert.ToString(num2)}";
							MySqlParameter item = new MySqlParameter
							{
								ParameterName = parameterName,
								Value = item3.MValue,
								MySqlDbType = MySqlDbType.VarChar
							};
							list.Add(item);
							num2++;
						}
					}
				}
			}
			list.Add(new MySqlParameter
			{
				ParameterName = "@MStatus",
				Value = filter.Status,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MCurrencyID",
				Value = filter.MCurrencyID,
				MySqlDbType = MySqlDbType.VarChar
			});
			return list.ToArray();
		}
	}
}
