using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace JieNor.Megi.DataRepository.PA
{
	public class PAPayItemRepository : DataServiceT<PAPayItemModel>
	{
		private List<string> multLangFieldSql = new List<string>
		{
			",MName{0}\r\n              ,MParentSalaryItemModel_MName{0} ",
			",(case when x.MParentID is null then null else z.MName{0} end) AS MParentSalaryItemModel_MName{0}\r\n              ,y.MName{0}",
			",s.MName{0}\n              ,s.MName{0} AS MParentName{0}",
			",t.MName{0}\n              ,NULL AS MParentName{0}",
			",m.MName{0}\n              ,m.MName{0} AS MParentName{0}",
			",n.MName\n              ,NULL AS MParentName"
		};

		public string CommonSelect = "SELECT \n                MParentSalaryItemModel_MSalaryItemID,\n                MParentSalaryItemModel_MName,\n                MItemID,\n                MSalaryItemID,\n                MName,\n                MAccountCode,\n                MIsActive,\n                MModifyDate,\n                MOrgID\n                #_#lang_field0#_#  \n            FROM\n                (SELECT \n                    x.MParentID AS MParentSalaryItemModel_MSalaryItemID,\n                        (case when x.MParentID is null then null else z.MName end) AS MParentSalaryItemModel_MName,\n                        x.MItemID,\n                        x.MItemID AS MSalaryItemID,\n                        y.MName,\n                        a.MNumber AS MAccountCode,\n                        x.MIsActive,\n                        x.MModifyDate,\n                        x.MOrgID\n                        #_#lang_field1#_#  \n                FROM\n                    (SELECT \n                    NULL AS MParentID,\n                        m.MItemID,\n                        m.MItemID AS MSalaryItemID,\n                        m.MOrgID,\n                        m.MAccountCode,\n                        m.MIsActive,\n                        m.MIsDelete,\n                        m.MModifyDate\n                FROM\n                    t_pa_payitemgroup m UNION ALL SELECT \n                    n.MGroupID AS MParentID,\n                        n.MItemID,\n                        n.MItemID AS MSalaryItemID,\n                        n.MOrgID,\n                        n.MAccountCode,\n                        n.MIsActive,\n                        n.MIsDelete,\n                        n.MModifyDate\n                FROM\n                    t_pa_payitem n) x\n                INNER JOIN (SELECT \n                    s.MName,\n                        s.MName AS MParentName,\n                        s.MParentID,\n                        s.MOrgID,\n                        s.MIsDelete\n                    #_#lang_field2#_#  \n                FROM\n                    @_@t_pa_payitemgroup_l@_@ s UNION ALL SELECT \n                    t.MName,\n                        NULL AS MParentName,\n                        t.MParentID,\n                        t.MOrgID,\n                        t.MIsDelete\n                    #_#lang_field3#_# \n                FROM\n                    @_@t_pa_payitem_l@_@ t) y ON x.MOrgID = y.MOrgID\n                    AND x.MItemID = y.MParentID\n                    AND y.MIsDelete = x.MIsDelete\n                LEFT JOIN (SELECT \n                    m.MName,\n                        m.MName AS MParentName,\n                        m.MParentID,\n                        m.MOrgID,\n                        m.MIsDelete\n                    #_#lang_field4#_# \n                FROM\n                    @_@t_pa_payitemgroup_l@_@ m UNION ALL SELECT \n                    n.MName,\n                        NULL AS MParentName,\n                        n.MParentID,\n                        n.MOrgID,\n                        n.MIsDelete\n                    #_#lang_field5#_# \n                FROM\n                    @_@t_pa_payitem_l@_@ n) z ON x.MOrgID = z.MOrgID\n                    AND x.MParentID = z.MParentID\n                    AND z.MIsDelete = x.MIsDelete\n                LEFT JOIN t_bd_account a ON a.MCode = x.MAccountCode\n                    AND a.MOrgID = x.MOrgID\n                    AND a.MIsDelete = x.MIsDelete\n                WHERE\n                    x.MOrgID = @MOrgID AND x.MIsDelete = 0) t1 where t1.MOrgID = @MOrgID ";

		public DataGridJson<PAPayItemModel> Get(MContext ctx, GetParam param)
		{
			return new APIDataRepository().Get<PAPayItemModel>(ctx, param, CommonSelect, multLangFieldSql, false, true, null);
		}

		public List<PAPayItemModel> GetSalaryItemList(MContext ctx, string MGroupID = null, string orgId = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" select t.MItemID , t.MIsActive , t.MIsDelete , t.MItemType , tl.MName ,t1l.MParentID as MGroupID , t1l.MName as MGroupName , t.MAccountCode from t_pa_payitem t ");
			stringBuilder.Append(" inner join t_pa_payitem_l tl on t.MOrgID=@MOrgID and tl.MLocaleID=@MLCID and t.MItemID = tl.MParentID AND tl.MIsDelete=0 ");
			if (!string.IsNullOrEmpty(MGroupID))
			{
				stringBuilder.Append(" and t.MGroupID=@MGroupID ");
			}
			stringBuilder.Append(" inner join t_pa_payitemgroup_l t1l on t1l.MLocaleID = @MLCID and t1l.MParentID = t.MGroupID  AND t1l.MIsDelete=0 ");
			stringBuilder.Append(" WHERE  tl.MIsDelete=0 ");
			stringBuilder.Append(" order by  MGroupName , MName");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MGroupID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ((orgId == null) ? ctx.MOrgID : orgId);
			array[1].Value = ctx.MLCID;
			array[2].Value = MGroupID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<PAPayItemModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public PAPayItemModel GetSalaryItemById(MContext ctx, string id)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" select * , t1l.MParentID as MGroupID , t1l.MName as MGroupName from t_pa_payitem t ");
			stringBuilder.Append(" inner join t_pa_payitem_l tl on t.MOrgID=@MOrgID and tl.MLocaleID=@MLCID and t.MItemID = tl.MParentID and t.MItemID=@MItemID AND tl.MIsDelete=0 ");
			stringBuilder.Append(" inner join t_pa_payitemgroup_l t1l on t1l.MLocaleID = @MLCID and t1l.MParentID = t.MGroupID AND t1l.MIsDelete=0 ");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = id;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.GetFirstOrDefaultModel<PAPayItemModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array));
		}

		public static List<PAPayItemModel> GetCompanySalaryItemList(MContext ctx)
		{
			int[] values = new int[9]
			{
				2020,
				2025,
				2035,
				2040,
				2030,
				2045,
				2050,
				2005,
				2010
			};
			int[] values2 = new int[2]
			{
				2015,
				2000
			};
			string sql = string.Format("select t1.* from t_pa_payitem t1 where t1.MOrgID=@MOrgID and MIsDelete=0 and MItemType in ({0})\r\n                                            union all\r\n                                        select t1.* from t_pa_payitemgroup t1 where t1.MOrgID=@MOrgID and MIsDelete=0 and MItemType in ({1})", string.Join(",", values), string.Join(",", values2));
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return ModelInfoManager.GetDataModelBySql<PAPayItemModel>(ctx, sql, cmdParms);
		}

		public OperationResult ForbiddenItem(MContext ctx, string id, bool value)
		{
			OperationResult operationResult = new OperationResult();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			List<CommandInfo> archiveFlagCmd = ModelInfoManager.GetArchiveFlagCmd<PAPayItemModel>(ctx, id, value);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(archiveFlagCmd);
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public DataSet GetMultilanguage(MContext ctx, string parentId)
		{
			PAPayItemModel pAPayItemModel = new PAPayItemModel();
			string arg = pAPayItemModel.TableName + "_l";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select * from {0} where mparentid=@MParentId AND MIsDelete=0 ", arg);
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MParentId", MySqlDbType.VarChar, 36)
			};
			array[0].Value = parentId;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
		}

		public List<CommandInfo> UpdatePayItemMapAccount(MContext ctx, string oldCode, string newCode)
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
			commandInfo.CommandText = "update t_pa_payitemgroup set MAccountCode=@newCode where MAccountCode=@oldCode and MOrgID=@MOrgID  AND MIsActive=1 AND MIsDelete=0";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			CommandInfo commandInfo2 = new CommandInfo();
			commandInfo2.CommandText = "update t_pa_payitem set MAccountCode=@newCode where MAccountCode=@oldCode and MOrgID=@MOrgID  AND MIsActive=1 AND MIsDelete=0";
			array = (commandInfo2.Parameters = parameters);
			list.Add(commandInfo2);
			return list;
		}

		public static List<PAPayItemModel> GetPayItemList(MContext ctx, bool? isActive = default(bool?))
		{
			string sql = string.Format("select * from (select t1.MItemID,t1.MItemType,'' as MGroupID,t1.MIsActive,t2.MName from t_pa_payitemgroup t1\r\n                                            left join t_pa_payitemgroup_l t2 on t2.MOrgID=t1.MOrgID and t2.MParentID=t1.MItemID and t2.MIsDelete=0 and t2.MLocaleID=@MLocaleID\r\n                                            where t1.MOrgID=@MOrgID and t1.MIsDelete=0 {0} \r\n                                         union all select t1.MItemID,t1.MItemType,t1.MGroupID,t1.MIsActive,t2.MName from t_pa_payitem t1\r\n                                            left join t_pa_payitem_l t2 on t2.MOrgID=t1.MOrgID and t2.MParentID=t1.MItemID and t2.MIsDelete=0 and t2.MLocaleID=@MLocaleID\r\n                                            where t1.MOrgID=@MOrgID and t1.MIsDelete=0 {0}) u\r\n                                         order by MItemType", (!isActive.HasValue) ? "" : " and t1.MIsActive=@MIsActive");
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MIsActive", !isActive.HasValue || isActive.Value)
			};
			return ModelInfoManager.GetDataModelBySql<PAPayItemModel>(ctx, sql, cmdParms);
		}
	}
}
