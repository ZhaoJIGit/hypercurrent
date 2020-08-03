using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.MI
{
	public class MigrateConfigRepository
	{
		public List<MigrateConfigModel> GetList(MContext ctx, string id = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select a.*,b.MName as MOrgName,F_GETUSERNAME(c.MFristName, c.MLastName) as MModifierName,d.MSystemLanguage as MActiveLocaleIDS\r\n                            ,F_GetUserName(e.MFristName,e.MLastName) as MMegiUserName,b.MInitBalanceOver,b.MVersionID\r\n                            from t_mi_config a\r\n                            inner join t_bas_organisation b on a.MOrgID=b.MItemID and b.MIsDelete=0\n                            left join t_sec_user_l c ON a.MModifierID = c.MParentID and c.MIsDelete = a.MIsDelete and c.MLocaleID = @MLocaleID\r\n                            left join t_reg_globalization d on a.MOrgID=d.MOrgID and d.MIsDelete=0\r\n                            left join t_sec_user_l e on a.MCreatorID=e.MParentID and e.MIsDelete=0 and e.MLocaleID = @MLocaleID\r\n                         where a.MIsDelete=0 and a.MCreatorID=@MCreatorID");
			bool flag = !string.IsNullOrWhiteSpace(id);
			if (flag)
			{
				stringBuilder.Append(" and a.MItemID=@MItemID");
			}
			stringBuilder.Append(" order by a.MCreateDate desc");
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MItemID", id),
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MCreatorID", ctx.MUserID)
			};
			ctx.IsSys = true;
			List<MigrateConfigModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<MigrateConfigModel>(ctx, stringBuilder.ToString(), cmdParms);
			if (flag && dataModelBySql.Count == 1)
			{
				ctx.IsSys = false;
				ctx.MOrgID = dataModelBySql[0].MOrgID;
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				MySqlParameter[] cmdParms2 = new MySqlParameter[1]
				{
					new MySqlParameter("@MOrgID", dataModelBySql[0].MOrgID)
				};
				string sql = "select group_concat(distinct MType) from t_mi_log where MOrgID=@MOrgID and MIsDelete=0";
				dataModelBySql[0].MigratedTypes = Convert.ToString(dynamicDbHelperMySQL.GetSingle(sql, cmdParms2));
			}
			return dataModelBySql;
		}

		public int GetlistCount(MContext ctx)
		{
			string sql = "SELECT  count(*)  from  t_mi_config  where  MOrgID=@MOrgID  AND  MIsDelete=0";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return ModelInfoManager.GetDataModelBySql<MigrateConfigModel>(ctx, sql, cmdParms).Count;
		}

		public OperationResult Save(MContext ctx, List<MigrateConfigModel> list, List<string> updateFields = null)
		{
			ctx.IsSys = true;
			return ModelInfoManager.InsertOrUpdate(ctx, list, updateFields);
		}

		public static List<CommandInfo> GetUpdateProgressCmdList(MContext ctx, MigrateConfigModel model, string updateFields)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (string.IsNullOrWhiteSpace(model.MItemID))
			{
				return list;
			}
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<MigrateConfigModel>(ctx, model, updateFields.Split(',').ToList(), true));
			return list;
		}

		public static List<ExistDataModel> GetExistDataList(MContext ctx, List<string> orgIdList)
		{
			List<MySqlParameter> list = new List<MySqlParameter>();
			int num = 1;
			List<string> list2 = new List<string>();
			foreach (string orgId in orgIdList)
			{
				string text = "@MOrgID" + num;
				list2.Add(text);
				list.Add(new MySqlParameter(text, orgId));
				num++;
			}
			string sql = string.Format("select t1.MItemID as MOrgID,(select count(1) from t_bd_contacts where MOrgID=t1.MItemID and MIsActive=1 and MIsDelete=0) as Contact,\r\n                                (select count(1) from t_bd_employees where MOrgID=t1.MItemID and MIsActive=1 and MIsDelete=0) as Employee,\r\n                                (select count(1) from t_bd_Item where MOrgID=t1.MItemID and MIsActive=1 and MIsDelete=0) as Item,\r\n                                (select count(1) from t_bd_Track where MOrgID=t1.MItemID and MIsActive=1 and MIsDelete=0) as Track,\r\n                                (select count(1) from t_reg_currency where MOrgID=t1.MItemID and MIsDelete=0) as Currency,\r\n                                (select count(1) from t_gl_initbalance where MOrgID=t1.MItemID and MIsDelete=0) as InitBalance,\r\n                                (select count(1) from t_gl_voucher where MOrgID=t1.MItemID and MIsDelete=0) as Voucher\r\n                                from t_bas_organisation t1 where t1.MItemID in ({0}) and t1.MIsDelete=0", string.Join(",", list2));
			ctx.MOrgID = orgIdList.FirstOrDefault();
			return ModelInfoManager.GetDataModelBySql<ExistDataModel>(ctx, sql, list.ToArray());
		}
	}
}
