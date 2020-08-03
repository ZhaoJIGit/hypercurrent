using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Core.Log
{
	internal class OptLogRepository
	{
		internal static void AddLog(OptLogTemplate template, MContext ctx, string pkId, bool createDateAdd, params object[] formatValues)
		{
			CommandInfo addLogCommand = GetAddLogCommand(template, ctx, pkId, createDateAdd, formatValues);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSql(addLogCommand.CommandText, (MySqlParameter[])addLogCommand.Parameters);
		}

		internal static CommandInfo GetAddLogCommand(OptLogTemplate template, MContext ctx, string pkId, bool createDateAdd, params object[] formatValues)
		{
			string sqlText = "INSERT INTO T_Log_Operation(MID, MTemplateID,MOrgID,MBizObject,MBizType,MAction,MPKID,MValue1,MValue2,MValue3,MValue4,MValue5,MValue6,MValue7,MValue8,MValue9,MValue10,MCreatorID,MCreateDate,MSource,MCreateBy)\r\n                            SELECT @MID,@MTemplateID,@MOrgID,MBizObject,MBizType,MAction,@MPKID,@MValue1,@MValue2,@MValue3,@MValue4,@MValue5,@MValue6,@MValue7,@MValue8,@MValue9,@MValue10,@MCreatorID,@MCreateDate,@MSource,@MCreateBy\r\n                            FROM T_Log_OperationTemplate\r\n                            WHERE MItemID=@MTemplateID";
			MySqlParameter[] array = new MySqlParameter[18]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTemplateID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MPKID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCreatorID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCreateDate", MySqlDbType.DateTime),
				new MySqlParameter("@MSource", MySqlDbType.VarChar, 100),
				new MySqlParameter("@MCreateBy", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MValue1", MySqlDbType.VarChar, 200),
				new MySqlParameter("@MValue2", MySqlDbType.VarChar, 200),
				new MySqlParameter("@MValue3", MySqlDbType.VarChar, 200),
				new MySqlParameter("@MValue4", MySqlDbType.VarChar, 200),
				new MySqlParameter("@MValue5", MySqlDbType.VarChar, 200),
				new MySqlParameter("@MValue6", MySqlDbType.VarChar, 200),
				new MySqlParameter("@MValue7", MySqlDbType.VarChar, 200),
				new MySqlParameter("@MValue8", MySqlDbType.VarChar, 200),
				new MySqlParameter("@MValue9", MySqlDbType.VarChar, 200),
				new MySqlParameter("@MValue10", MySqlDbType.VarChar, 200)
			};
			array[0].Value = UUIDHelper.GetGuid();
			array[1].Value = Convert.ToInt32(template);
			array[2].Value = ctx.MOrgID;
			array[3].Value = pkId;
			array[4].Value = ctx.MUserID;
			array[5].Value = (createDateAdd ? ctx.DateNow.AddSeconds(1.0) : ctx.DateNow);
			array[6].Value = "";
			array[7].Value = ctx.MConsumerKey;
			if (formatValues != null && formatValues.Length != 0)
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				int num = 8;
				foreach (object obj in formatValues)
				{
					if (obj == null)
					{
						array[num].Value = " ";
					}
					else
					{
						Type type = obj.GetType();
						if (type == typeof(DateTime))
						{
							array[num].Value = javaScriptSerializer.Serialize(obj);
						}
						else if (type == typeof(decimal))
						{
							array[num].Value = Convert.ToDecimal(obj).ToOrgDigitalFormat(ctx);
						}
						else
						{
							array[num].Value = obj;
						}
					}
					num++;
				}
			}
			return new CommandInfo(sqlText, array);
		}

		internal static CommandInfo GetAddLogCommand(OptLogTemplate template, MContext ctx, List<string> pkIdList, bool createDateAdd, Dictionary<string, List<object>> formatValues = null)
		{
			string arg = "INSERT INTO T_Log_Operation(MID, MTemplateID,MOrgID,MBizObject,MBizType,MAction,MPKID,MValue1,MValue2,MValue3,MValue4,MValue5,MValue6,MValue7,MValue8,MValue9,MValue10,MCreatorID,MCreateDate,MSource,MCreateBy)";
			List<string> list = new List<string>();
			int num = 1;
			List<MySqlParameter> list2 = new List<MySqlParameter>();
			foreach (string pkId in pkIdList)
			{
				string text = num.ToString();
				string item = string.Format("SELECT @MID{0},@MTemplateID{0},@MOrgID{0},MBizObject,MBizType,MAction,@MPKID{0},@MValue1{0},@MValue2{0},@MValue3{0},\r\n                                                          @MValue4{0},@MValue5{0},@MValue6{0},@MValue7{0},@MValue8{0},@MValue9{0},@MValue10{0},@MCreatorID{0},@MCreateDate{0},@MSource{0},@MCreateBy{0}\r\n                            FROM T_Log_OperationTemplate\r\n                            WHERE MItemID=@MTemplateID{0}", text);
				list.Add(item);
				MySqlParameter[] array = new MySqlParameter[18]
				{
					new MySqlParameter("@MID" + text, MySqlDbType.VarChar, 36),
					new MySqlParameter("@MTemplateID" + text, MySqlDbType.VarChar, 36),
					new MySqlParameter("@MOrgID" + text, MySqlDbType.VarChar, 36),
					new MySqlParameter("@MPKID" + text, MySqlDbType.VarChar, 36),
					new MySqlParameter("@MCreatorID" + text, MySqlDbType.VarChar, 36),
					new MySqlParameter("@MCreateDate" + text, MySqlDbType.DateTime),
					new MySqlParameter("@MSource" + text, MySqlDbType.VarChar, 100),
					new MySqlParameter("@MCreateBy" + text, MySqlDbType.VarChar, 36),
					new MySqlParameter("@MValue1" + text, MySqlDbType.VarChar, 200),
					new MySqlParameter("@MValue2" + text, MySqlDbType.VarChar, 200),
					new MySqlParameter("@MValue3" + text, MySqlDbType.VarChar, 200),
					new MySqlParameter("@MValue4" + text, MySqlDbType.VarChar, 200),
					new MySqlParameter("@MValue5" + text, MySqlDbType.VarChar, 200),
					new MySqlParameter("@MValue6" + text, MySqlDbType.VarChar, 200),
					new MySqlParameter("@MValue7" + text, MySqlDbType.VarChar, 200),
					new MySqlParameter("@MValue8" + text, MySqlDbType.VarChar, 200),
					new MySqlParameter("@MValue9" + text, MySqlDbType.VarChar, 200),
					new MySqlParameter("@MValue10" + text, MySqlDbType.VarChar, 200)
				};
				array[0].Value = UUIDHelper.GetGuid();
				array[1].Value = Convert.ToInt32(template);
				array[2].Value = ctx.MOrgID;
				array[3].Value = pkId;
				array[4].Value = ctx.MUserID;
				array[5].Value = (createDateAdd ? ctx.DateNow.AddSeconds(1.0) : ctx.DateNow);
				array[6].Value = "";
				array[7].Value = ctx.MConsumerKey;
				if (formatValues != null && formatValues.Count() > 0 && formatValues.Keys.Contains(pkId))
				{
					List<object> list3 = formatValues[pkId];
					JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
					int num2 = 8;
					foreach (object item2 in list3)
					{
						if (item2 == null)
						{
							array[num2].Value = " ";
						}
						else
						{
							Type type = item2.GetType();
							if (type == typeof(DateTime))
							{
								array[num2].Value = javaScriptSerializer.Serialize(item2);
							}
							else if (type == typeof(decimal))
							{
								array[num2].Value = Convert.ToDecimal(item2).ToOrgDigitalFormat(ctx);
							}
							else
							{
								array[num2].Value = item2;
							}
						}
						num2++;
					}
				}
				list2.AddRange(array.ToList());
				num++;
			}
			if (list.Count() == 0)
			{
				return null;
			}
			string arg2 = string.Join(" UNION ALL ", list);
			arg = string.Format("{0} {1}", arg, arg2);
			return new CommandInfo(arg, list2.ToArray());
		}

		internal static CommandInfo GetAddLogCommand(OptLogTemplate template, MContext ctx, string pkId, params object[] formatValues)
		{
			return GetAddLogCommand(template, ctx, pkId, false, formatValues);
		}

		public static DataGridJson<OptLogListModel> GetLogListForPrint(OptLogListFilter filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = filter;
			string logSqlWhere = GetLogSqlWhere(filter, sqlQuery);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT DISTINCT opt.MID, opt.MPKID,opt.MTemplateID,pro.MFristName AS MFirstName,pro.MLastName, opt.MCreateDate FROM \r\n                                    (SELECT * FROM  T_Log_Operation opt where MIsDelete=0 {0}) opt\r\n                                    LEFT JOIN ({1}) pro ON opt.MCreatorID=pro.MParentID\r\n                                order by opt.MCreateDate DESC", logSqlWhere, GetUserMultiSql());
			sqlQuery.SelectString = stringBuilder.ToString();
			List<OptLogListModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<OptLogListModel>(filter.MContext, sqlQuery.SelectString, sqlQuery.Parameters);
			return new DataGridJson<OptLogListModel>
			{
				rows = dataModelBySql,
				total = dataModelBySql.Count
			};
		}

		public static DataGridJson<OptLogListModel> GetLogList(OptLogListFilter filter)
		{
			if (filter.IsFromPrint)
			{
				return GetLogListForPrint(filter);
			}
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = filter;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT  DISTINCT opt.MID,opt.MTemplateID, dictItem.MName AS MBizType, dict.MName AS MAction, opt.MPKID, otl.MNote,pro.MFristName AS MFirstName,pro.MLastName, opt.MCreateDate, \r\n\t\t                            opt.MValue1,opt.MValue2,opt.MValue3,opt.MValue4,opt.MValue5,opt.MValue6,opt.MValue7,opt.MValue8,opt.MValue9,opt.MValue10,opt.MCreateBy ");
			stringBuilder.AppendLine(" FROM ");
			string logSqlWhere = GetLogSqlWhere(filter, sqlQuery);
			stringBuilder.AppendFormat(" (SELECT * FROM  T_Log_Operation opt where MIsDelete=0 {0}) opt", logSqlWhere.ToString());
			stringBuilder.AppendLine(" INNER JOIN T_Log_OperationTemplate_L otl ON opt.MTemplateID=otl.MParentID AND MLocaleID=@MLocaleID   AND  otl.MIsDelete=0\r\n                            INNER JOIN (SELECT b.MName, MValue FROM T_Bas_DictEntry a \r\n\t\t                             INNER JOIN T_Bas_DictEntry_L b ON a.MEntryID=b.MParentID AND b.MLocaleID=@MLocaleID AND   b.MIsDelete=0\r\n\t\t                             WHERE a.MDictName='LogActionType' AND  a.MIsDelete=0  ) dict ON opt.MAction=dict.MValue\r\n                            INNER JOIN (SELECT b.MName, MValue FROM T_Bas_DictEntry a \r\n\t\t                             INNER JOIN T_Bas_DictEntry_L b ON a.MEntryID=b.MParentID AND b.MLocaleID=@MLocaleID  AND  b.MIsDelete=0\r\n\t\t                             WHERE a.MDictName='LogItemType'  AND  a.MIsDelete=0) dictItem ON opt.MBizType=dictItem.MValue\r\n                            LEFT JOIN (" + GetUserMultiSql() + ") pro ON opt.MCreatorID=pro.MParentID");
			sqlQuery.SelectString = stringBuilder.ToString();
			sqlQuery.AddParameter(new MySqlParameter("@MLocaleID", filter.MContext.MLCID));
			if (string.IsNullOrEmpty(filter.Sort))
			{
				filter.AddOrderBy("opt.MCreateDate", SqlOrderDir.Desc);
			}
			else if (filter.Sort == "MUserName")
			{
				if (filter.Order.Trim().EqualsIgnoreCase("Desc"))
				{
					filter.AddOrderBy("pro.MFristName", SqlOrderDir.Desc);
				}
				else
				{
					filter.AddOrderBy("pro.MFristName", SqlOrderDir.Asc);
				}
			}
			else if (filter.Order.Trim().EqualsIgnoreCase("Desc"))
			{
				filter.AddOrderBy("pro." + filter.Sort, SqlOrderDir.Desc);
			}
			else
			{
				filter.AddOrderBy("pro." + filter.Sort, SqlOrderDir.Asc);
			}
			sqlQuery.CountSqlString = "SELECT count(1) from T_Log_Operation opt \r\n                                   INNER JOIN T_Log_OperationTemplate_L otl ON opt.MTemplateID=otl.MParentID AND otl.MLocaleID=@MLocaleID AND  otl.MIsDelete=0 \r\n                                   WHERE opt.MIsDelete=0 " + logSqlWhere;
			sqlQuery.OrderBy("opt.MCreateDate DESC");
			return ModelInfoManager.GetPageDataModelListBySql<OptLogListModel>(filter.MContext, sqlQuery);
		}

		private static string GetLogSqlWhere(OptLogListFilter filter, SqlQuery query)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" and MOrgID= @MOrgID");
			query.AddParameter(new MySqlParameter("@MOrgID", filter.MContext.MOrgID));
			if (!string.IsNullOrEmpty(filter.MBizObject))
			{
				stringBuilder.Append(" and opt.MBizObject=@MBizObject");
				query.AddParameter(new MySqlParameter("@MBizObject", filter.MBizObject));
			}
			if (!string.IsNullOrEmpty(filter.MPKID))
			{
				string[] array = filter.MPKID.Trim(',').Split(',');
				List<string> list = new List<string>();
				for (int i = 0; i < array.Length; i++)
				{
					list.Add("@MPKID" + i);
					query.AddParameter(new MySqlParameter("@MPKID" + i, array[i]));
				}
				stringBuilder.AppendFormat(" and opt.MPKID in ({0})", string.Join(",", list));
			}
			return stringBuilder.ToString();
		}

		public static int GetLogCount(OptLogListFilter filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT COUNT(1)\r\n                            FROM T_Log_Operation opt\r\n                            INNER JOIN T_Log_OperationTemplate_L otl ON opt.MTemplateID=otl.MParentID AND otl.MLocaleID=@MLocaleID AND  otl.MIsDelete=0\r\n                            INNER JOIN (SELECT b.MName, MValue FROM T_Bas_DictEntry a \r\n\t\t                             INNER JOIN T_Bas_DictEntry_L b ON a.MEntryID=b.MParentID AND b.MLocaleID=@MLocaleID  AND  b.MIsDelete=0\r\n\t\t                             WHERE a.MDictName='LogActionType') dict ON opt.MAction=dict.MValue\r\n                            LEFT JOIN T_Sec_User_L pro ON opt.MCreatorID=pro.MParentID AND  pro.MIsDelete=0");
			stringBuilder.Append(filter.SqlString);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(filter.MContext);
			object single = dynamicDbHelperMySQL.GetSingle(stringBuilder.ToString(), filter.Parameters);
			return single.ToMInt32();
		}

		public static List<NameValueModel> GetOrgContactList(OptLogListFilter filter)
		{
			string sql = string.Format("SELECT a.MItemID, convert(AES_DECRYPT(MName,'{0}') using utf8) AS MName FROM T_BD_Contacts a \r\n                        INNER JOIN T_BD_Contacts_l b ON a.MItemID=b.MParentID AND b.MLocaleID=@MLocaleID  AND  b.MIsDelete=0  \r\n                        WHERE a.MOrgID=@MOrgID AND  a.MIsDelete=0", "JieNor-001");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = filter.MContext.MOrgID;
			array[1].Value = filter.MContext.MLCID;
			List<NameValueModel> list = new List<NameValueModel>();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(filter.MContext);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, array);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return list;
			}
			DataTable dataTable = dataSet.Tables[0];
			foreach (DataRow row in dataTable.Rows)
			{
				NameValueModel nameValueModel = new NameValueModel();
				nameValueModel.MName = row["MName"].ToMString();
				nameValueModel.MValue = row["MItemID"].ToMString();
				list.Add(nameValueModel);
			}
			return list;
		}

		public static List<NameValueModel> GetOrgEmployeesList(OptLogListFilter filter)
		{
			string sql = "SELECT a.MItemID,b.MFirstName,b.MLastName FROM T_BD_Employees a \r\n                            INNER JOIN T_BD_Employees_l b ON a.MItemID=b.MParentID  AND  b.MLocaleID=@MLocaleID   AND  b.MIsDelete=0 \r\n                             WHERE a.MOrgID=@MOrgID  AND  a.MIsDelete=0 ";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = filter.MContext.MOrgID;
			array[1].Value = filter.MContext.MLCID;
			List<NameValueModel> list = new List<NameValueModel>();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(filter.MContext);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, array);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return list;
			}
			DataTable dataTable = dataSet.Tables[0];
			foreach (DataRow row in dataTable.Rows)
			{
				NameValueModel nameValueModel = new NameValueModel();
				string firstName = (row["MFirstName"] == null || row["MFirstName"] == DBNull.Value) ? "" : row["MFirstName"].ToString();
				string lastName = (row["MLastName"] == null || row["MLastName"] == DBNull.Value) ? "" : row["MLastName"].ToString();
				nameValueModel.MName = GlobalFormat.GetUserName(firstName, lastName, null);
				nameValueModel.MValue = row["MItemID"].ToMString();
				list.Add(nameValueModel);
			}
			return list;
		}

		private static string GetUserMultiSql()
		{
			return "select MParentID, MFristName,MLastName from T_Sec_User_L \r\n                    where IFNULL(MFristName,'') <>'' and IFNULL(MLastName,'')<>'' and MIsDelete=0 \r\n                    group by MParentID";
		}
	}
}
