using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JieNor.Megi.DataRepository.PA
{
	public class PAPayItemGroupRepository : DataServiceT<PAPayItemGroupModel>
	{
		public static List<PAPayItemGroupModel> GetSalaryItemGroupList(MContext ctx, PAPayItemGroupTypeEnum groupType = PAPayItemGroupTypeEnum.All)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select * from t_pa_payitemgroup t1\r\n                                inner join t_pa_payitemgroup_l t2 on t2.MOrgID=t1.MOrgID and t2.MParentID=t1.MItemID and t2.MLocaleID=@MLCID and t2.MIsDelete=0\r\n                                WHERE t1.MOrgID=@MOrgID and t1.MIsDelete=0 ");
			switch (groupType)
			{
			case PAPayItemGroupTypeEnum.User:
				stringBuilder.AppendFormat(" and t1.MItemType in ('{0}', '{1}')", 1023, 1067);
				break;
			case PAPayItemGroupTypeEnum.NoChildLevel:
				stringBuilder.AppendFormat(" and t1.MItemType < {0} and t1.MItemType not in ('{1}', '{2}')", 2000, 1035, 1055);
				break;
			}
			stringBuilder.Append(" Order by t1.MItemType, t1.MCreateDate ");
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLCID", ctx.MLCID)
			};
			List<PAPayItemGroupModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<PAPayItemGroupModel>(ctx, stringBuilder.ToString(), cmdParms);
			foreach (PAPayItemGroupModel item in dataModelBySql)
			{
				item.MItemTypeName = GetItemTypeName((PayrollItemEnum)item.MItemType, item.MItemID);
			}
			return dataModelBySql;
		}

		public static List<NameValueModel> GetMultLangGroupNameList(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select t1.MItemType as MName, t2.MName as MValue, t2.MLocaleID as MTag from t_pa_payitemgroup t1\r\n                                inner join t_pa_payitemgroup_l t2 on t2.MOrgID=t1.MOrgID and t2.MParentID=t1.MItemID and t2.MIsDelete=0\r\n                                WHERE t1.MOrgID=@MOrgID and t1.MIsDelete=0\r\n                                order by t1.MItemType, t1.MCreateDate");
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return ModelInfoManager.GetDataModelBySql<NameValueModel>(ctx, stringBuilder.ToString(), cmdParms);
		}

		public static List<PAPayItemGroupAmtModel> GetUserPayItemList(MContext ctx)
		{
			string sql = string.Format(" SELECT t1.MItemID as MPayItemID, t1.MItemType as ItemType, t1.MCoefficient, t2.MName, t1.MCreateDate, t1.MIsActive FROM T_PA_PayItemGroup t1\r\n                                                join T_PA_PayItemGroup_l t2 on t2.MOrgID=t1.MOrgID and t2.MParentID=t1.MItemID and t2.MIsDelete=0 and t2.MLocaleID=@MLocaleID\r\n                                              where t1.MOrgID=@MOrgID and t1.MIsDelete=0 and t1.MItemType in ('{0}')\r\n                                                order by t1.MItemType, t1.MCreateDate", string.Join("','", new int[2]
			{
				1023,
				1067
			}));
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID)
			};
			List<PAPayItemGroupAmtModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<PAPayItemGroupAmtModel>(ctx, sql, cmdParms);
			foreach (PAPayItemGroupAmtModel item in dataModelBySql)
			{
				item.ItemTypeName = GetItemTypeName((PayrollItemEnum)item.ItemType, item.MPayItemID);
			}
			return dataModelBySql;
		}

		public static string GetItemTypeName(PayrollItemEnum itemType, string payItemId)
		{
			bool flag = itemType == PayrollItemEnum.UserAddItem || itemType == PayrollItemEnum.UserSubtractItem;
			return string.Format("{0}{1}", itemType.ToString(), flag ? payItemId : "");
		}

		public List<PAPayItemGroupModel> GetSelectedSalaryItemGroupList(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT a.MItemID,b.MName FROM T_PA_PayItemGroup a ");
			stringBuilder.Append(" inner join T_PA_PayItemGroup_L b ");
			stringBuilder.Append(" on a.MItemID=b.MParentID AND b.MIsDelete=0");
			stringBuilder.Append(" where a.MIsActive=1 AND a.MIsDelete=0 AND LOCATE(concat(',',a.MItemID,','), (select concat(',',MPayItemIDs,',') from T_PA_PaySetting where MOrgID=@MOrgID))>0;");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			return ModelInfoManager.GetDataModelBySql<PAPayItemGroupModel>(ctx, stringBuilder.ToString(), array);
		}

		public OperationResult ForbiddenItem(MContext ctx, string id, bool value)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> archiveFlagCmd = ModelInfoManager.GetArchiveFlagCmd<PAPayItemGroupModel>(ctx, id, value);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(archiveFlagCmd);
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public DataSet GetMultilanguage(MContext ctx, string parentId)
		{
			PAPayItemGroupModel pAPayItemGroupModel = new PAPayItemGroupModel();
			string arg = pAPayItemGroupModel.TableName + "_l";
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

		public List<PAPayItemGroupModel> GetAllPayItemList(MContext ctx, bool isIncludeDisable = false)
		{
			string str = "select * from (\n                                select t1.MItemID, t1.MItemType, t2.MName , t1.MIsActive from t_pa_payitemgroup t1\n                                left join t_pa_payitemgroup_l t2 on t1.MItemID = t2.MParentID\n\t                                AND t1.MOrgID = t2.MOrgID\n\t                                AND t1.MIsDelete = t2.MIsDelete\n                                where t1.MOrgID = @MOrgID\n\t                                AND t2.MLocaleID = @MLocaleID\n\t                                AND t1.MIsDelete = 0 ";
			if (!isIncludeDisable)
			{
				str += " AND t1.MIsActive = 1 ";
			}
			str += "\n\t                                AND MItemID not in (select MGroupID from t_pa_payitem where MOrgID = @MOrgID)\n                                union all \n                                select t1.MItemID, t1.MItemType, concat(t3.MName, '-', t2.MName) as MName , t1.MIsActive from t_pa_payitem t1\n                                left join t_pa_payitem_l t2 on t1.MItemID = t2.MParentID\n\t                                AND t1.MOrgID = t2.MOrgID\n\t                                AND t1.MIsDelete = t2.MIsDelete\n                                left join t_pa_payitemgroup_l t3 on t1.MGroupID=t3.MParentID\n\t                                AND t1.MOrgID = t3.MOrgID\n\t                                AND t1.MIsDelete = t3.MIsDelete\n\t                                and t2.MLocaleID = t3.MLocaleID\n                                where t1.MOrgID = @MOrgID\n\t                                AND t2.MLocaleID = @MLocaleID\n\t                                AND t1.MIsDelete = 0 ";
			if (!isIncludeDisable)
			{
				str += " AND t1.MIsActive = 1 ";
			}
			str += "\n                                ) u\n                            ORDER BY MItemType";
			return ModelInfoManager.GetDataModelBySql<PAPayItemGroupModel>(ctx, str, ctx.GetParameters((MySqlParameter)null));
		}
	}
}
