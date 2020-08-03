using JieNor.Megi.Common.Logger;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace JieNor.Megi.DataRepository.SEC
{
	public class SECUserLoginLogRepository : DataServiceT<SECUserLoginLogModel>
	{
		private static string GetUserMultiSql()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select MParentID, MFristName , MLastName from ");
			stringBuilder.AppendLine("(select MParentID, MFristName , MLastName, case MLocaleID when @MLocaleID then 99 else locate(MLocaleID, '0x7C04,0x0009,0x7804') end rowNum ");
			stringBuilder.AppendLine("from T_Sec_User_L ");
			stringBuilder.AppendLine("where (ifnull(MFristName,'')<>'' || ifnull(MLastName,'')<>'') ");
			stringBuilder.AppendLine(") a where ( ");
			stringBuilder.AppendLine("select COUNT(1) FROM  ");
			stringBuilder.AppendLine("(select MParentID, case MLocaleID when @MLocaleID then 99 else locate(MLocaleID, '0x7C04,0x0009,0x7804') end rowNum ");
			stringBuilder.AppendLine("from T_Sec_User_L ");
			stringBuilder.AppendLine("where (ifnull(MFristName,'')<>'' || ifnull(MLastName,'')<>'') ");
			stringBuilder.AppendLine(") b where a.MParentID=b.MParentID AND a.rowNum<=b.rowNum )<=1 ");
			return stringBuilder.ToString();
		}

		public DataGridJson<SECUserLoginLogModel> GetUserLoginLogPageListByOrgId(MContext ctx, string orgId, SECUserLoginLogListFilter filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select t1.MItemID, t1.MOrgID , t1.MUserID, t1.MLoginDate as MLoginDate , t2l.MFristName AS MFirstName , t2l.MLastName as MLastName from T_User_LoginLog t1");
			stringBuilder.AppendLine("join( ");
			stringBuilder.AppendLine(GetUserMultiSql());
			stringBuilder.AppendLine(") t2l on t1.MUserID=t2l.MParentID ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
			};
			array[0].Value = (string.IsNullOrEmpty(orgId) ? ctx.MOrgID : orgId);
			array[1].Value = ctx.MLCID;
			filter.AddOrderBy("t1.MLoginDate", SqlOrderDir.Desc);
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = stringBuilder.ToString();
			MySqlParameter[] array2 = array;
			foreach (MySqlParameter para in array2)
			{
				sqlQuery.AddParameter(para);
			}
			DataGridJson<SECUserLoginLogModel> pageDataModelListBySql = ModelInfoManager.GetPageDataModelListBySql<SECUserLoginLogModel>(ctx, sqlQuery);
			if (pageDataModelListBySql != null && pageDataModelListBySql.rows != null && pageDataModelListBySql.rows.Count > 0)
			{
				for (int j = 0; j < pageDataModelListBySql.rows.Count; j++)
				{
					pageDataModelListBySql.rows[j].MFullName = GlobalFormat.GetUserName(pageDataModelListBySql.rows[j].MFirstName, pageDataModelListBySql.rows[j].MLastName, ctx);
				}
			}
			return pageDataModelListBySql;
		}

		public DataGridJson<SECUserLoginLogModel> GetUserLoginLogPageList(MContext ctx)
		{
			return null;
		}

		public OperationResult InsertLoginLog(MContext ctx, SECUserLoginLogModel model)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				List<CommandInfo> list = new List<CommandInfo>();
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECUserLoginLogModel>(ctx, model, null, true));
				bool flag = model != null && !string.IsNullOrWhiteSpace(model.MOrgID);
				MultiDBCommand[] array = flag ? new MultiDBCommand[2] : new MultiDBCommand[1];
				array[0] = new MultiDBCommand(ctx)
				{
					DBType = SysOrBas.Sys,
					CommandList = list
				};
				if (flag)
				{
					array[1] = new MultiDBCommand(ctx)
					{
						DBType = SysOrBas.Bas,
						CommandList = list
					};
				}
				operationResult.Success = DbHelperMySQL.ExecuteSqlTran(ctx, array);
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
			}
			return operationResult;
		}
	}
}
