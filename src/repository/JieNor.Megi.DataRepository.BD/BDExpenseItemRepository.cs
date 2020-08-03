using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDExpenseItemRepository : DataServiceT<BDExpenseItemModel>
	{
		private string multLangFieldSql = "\r\n            ,t2.MName{0}\r\n            ,t2.MDesc{0}\r\n            ,t4.MName{0} as MParentExpenseItemModel_MName{0} ";

		public string CommonSelect = "SELECT \r\n                t1.MItemID,\r\n                t1.MItemID AS MExpenseItemID,\r\n                t5.MNumber as MAccountCode,\r\n                t1.MIsActive,\r\n                t1.MModifyDate,\r\n                t1.MCreateDate,\r\n                t2.MName,\r\n                t2.MDesc,\r\n                t3.MItemID as MParentExpenseItemModel_MExpenseItemID,\r\n                t4.MName as MParentExpenseItemModel_MName\r\n                #_#lang_field0#_#    \r\n            FROM\r\n                T_BD_ExpenseItem t1\r\n                    INNER JOIN\r\n                @_@t_bd_expenseitem_l@_@ t2 ON t1.MOrgID = t2.MOrgID\r\n                    AND t1.MItemID = t2.MParentID\r\n                    AND t2.MIsDelete = 0\r\n                    LEFT JOIN\r\n                (select \r\n                    *\r\n                from\r\n                    T_BD_ExpenseItem\r\n                where\r\n                    MOrgID = @MOrgID\r\n                        and MIsDelete = 0\r\n                        and MItemID in (select \r\n                            MParentItemID\r\n                        from\r\n                            T_BD_ExpenseItem\r\n                        where\r\n                            MOrgID = @MOrgID\r\n                                and MIsDelete = 0\r\n                                and MParentItemID <> '0')) t3 ON t3.MOrgID = t1.MOrgID\r\n                    and t3.MItemID = t1.MParentItemID\r\n                    and t3.MIsDelete = 0\r\n                    left join\r\n                @_@t_bd_expenseitem_l@_@ t4 ON t4.MOrgID = t1.MOrgID\r\n                    and t4.MParentID = t3.MItemID\r\n                    and t4.MIsDelete = 0 \r\n                    LEFT JOIN\r\n                t_bd_account t5 ON t5.MOrgID = t1.MOrgID\r\n                    AND t5.MCode = t1.MAccountCode\r\n                    AND t5.MIsDelete = 0\r\n            WHERE\r\n                t1.MOrgID = @MOrgID\r\n                    AND t1.MIsDelete = 0";

		public DataGridJson<BDExpenseItemModel> Get(MContext ctx, GetParam param)
		{
			return new APIDataRepository().Get<BDExpenseItemModel>(ctx, param, CommonSelect, multLangFieldSql, false, true, null);
		}

		public List<BDExpenseItemModel> GetParentExpenseItemList(MContext ctx, bool isOnlyActive = true, bool ignoreLocale = false, bool includeChildren = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" select a.* , b.MName ,b.MDesc from t_bd_expenseitem a ");
			stringBuilder.AppendFormat(" inner join t_bd_expenseitem_l b on a.MOrgID =b.MOrgID and  a.MItemID = b.MParentID {0} and b.MIsDelete = 0 ", ignoreLocale ? string.Empty : " and b.MLocaleID=@MLCID");
			stringBuilder.Append(" where  a.MOrgID = @MOrgID and a.MIsDelete = 0 AND a.MIsDelete=0");
			if (!includeChildren)
			{
				stringBuilder.Append(" and a.MParentItemID='0' ");
			}
			if (isOnlyActive)
			{
				stringBuilder.Append(" and a.MIsActive=1 ");
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLCID", ctx.MLCID)
			};
			return ModelInfoManager.DataTableToList<BDExpenseItemModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), cmdParms));
		}

		public List<BDExpenseItemModel> GetParentExistList(MContext ctx, BDExpenseItemModel model)
		{
			string sql = "select   b.MItemID,a.MName   from   T_BD_ExpenseItem  b \r\n                 left   join  t_bd_expenseitem_l   a  on   a.MParentID=b.MitemID AND  a.MLocaleID=@MlocaleID AND a.MIsDelete=0 \r\n                 where  b.MOrgID=@MOrgID  AND  b.MParentItemID=@MParentitemID AND b.MIsDelete=0 ";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MlocaleID", ctx.MLCID),
				new MySqlParameter("@MParentitemID", model.MParentItemID)
			};
			return ModelInfoManager.DataTableToList<BDExpenseItemModel>(dynamicDbHelperMySQL.Query(sql, cmdParms));
		}

		public List<BDExpenseItemModel> GetParentExistListMParentItemID(MContext ctx, BDExpenseItemModel model)
		{
			string sql = "select b.MItemID, a.MName   from  T_BD_ExpenseItem  b\r\n                              inner  join  t_bd_expenseitem_l   a  on   a.MParentID=b.MitemID  AND  a.MlocaleID=@MlocaleID\r\n                              where  b.MOrgID=@MOrgID   AND b.MIsDelete = 0  and  b.MParentitemID=@MParentitemID";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MlocaleID", ctx.MLCID),
				new MySqlParameter("@MParentitemID", model.MParentItemID)
			};
			return ModelInfoManager.DataTableToList<BDExpenseItemModel>(dynamicDbHelperMySQL.Query(sql, cmdParms));
		}

		public List<BDExpenseItemModel> GetExpenseItemList(MContext ctx, List<string> itemIdList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT a.*,b.MItemID AS MAccountId FROM T_BD_ExpenseItem a \r\n                             LEFT JOIN (SELECT * FROM T_BD_Account WHERE MOrgID=@MOrgID and MIsDelete = 0 ) b ON a.MAccountCode=b.MCode  AND  b.MIsDelete = 0 \r\n                             WHERE a.MOrgID=@MOrgID and a.MIsDelete = 0 and a.MIsActive = 1  AND a.MItemID IN (");
			foreach (string itemId in itemIdList)
			{
				stringBuilder.AppendFormat("'{0}',", itemId);
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append(")");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDExpenseItemModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), ctx.GetParameters((MySqlParameter)null)));
		}

		public List<BDExpenseItemModel> GetChildrenExpenseItemList(MContext ctx, bool ignoreLocale, bool includeDisabled)
		{
			string arg = ignoreLocale ? string.Empty : " and b.MLocaleID=@MLocaleID";
			string arg2 = includeDisabled ? string.Empty : " and a.MIsActive = 1";
			string sql = string.Format("select a.*,b.MName,b.MLocaleID from T_BD_ExpenseItem a \r\n                left join T_BD_ExpenseItem_l b on a.MItemID=b.MParentID and a.MOrgID=b.MOrgID and b.MIsDelete = 0 {0}  \r\n                where a.MOrgID=@MOrgID and a.MIsDelete = 0 {1} \r\n                    and (a.MParentItemID<>'0' \r\n                         or (a.MParentItemID='0' \r\n                             and a.MItemID not in(select MParentItemID from T_BD_ExpenseItem a where MOrgID=@MOrgID and MIsDelete = 0 {1} )))", arg, arg2);
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID)
			};
			return ModelInfoManager.GetDataModelBySql<BDExpenseItemModel>(ctx, sql, cmdParms);
		}

		public List<CommandInfo> UpdateExpenseItemMapAccount(MContext ctx, string oldCode, string newCode)
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
			commandInfo.CommandText = "update t_bd_expenseitem set MAccountCode=@newCode where MAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete = 0 ";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}

		public int ArchievExpenseItem(MContext ctx, string itemIds, bool isRestore)
		{
			if (string.IsNullOrWhiteSpace(itemIds))
			{
				throw new NullReferenceException("itemids is can be null");
			}
			List<string> list = itemIds.Split(',').ToList();
			List<CommandInfo> list2 = new List<CommandInfo>();
			foreach (string item in list)
			{
				list2.AddRange(GetArchiveExpenseItemCmdList(ctx, isRestore, item));
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.ExecuteSqlTran(list2);
		}

		public List<CommandInfo> GetArchiveExpenseItemCmdList(MContext ctx, bool isRestore, string itemId)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MItemID", itemId),
				new MySqlParameter("@MIsActive", isRestore ? 1 : 0)
			};
			List<CommandInfo> archiveFlagCmd = ModelInfoManager.GetArchiveFlagCmd<BDExpenseItemModel>(ctx, itemId, isRestore);
			list.AddRange(archiveFlagCmd);
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_bd_expenseitem_l set MIsActive=@MIsActive where MOrgID=@MOrgID and MParentID = @MItemID and MIsDelete = 0 and MIsActive != @MIsActive";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}
	}
}
