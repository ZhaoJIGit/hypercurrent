using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.RPT.GL;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace JieNor.Megi.DataRepository.RPT.Biz
{
	public class RPTFilterSchemeRepository : DataServiceT<RPTFilterSchemeModel>
	{
		public OperationResult InsertOrUpdate(MContext ctx, RPTFilterSchemeModel filterScheme)
		{
			return base.InsertOrUpdate(ctx, filterScheme, null);
		}

		public List<RPTFilterSchemeModel> GetFilterSchemeList(MContext ctx, RPTFilterSchemeFilterModel filter)
		{
			string sql = "select a.MItemID , b.MName from t_rpt_scheme a\r\n                           inner join t_rpt_scheme_l b on a.mitemid = b.MParentID  and b.MLocaleId = @MLCID \r\n                      where a.MOrgID=@MOrgID and a.MReportType=@RptType and a.MIsDelete=0";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@RptType", filter.MReportType),
				new MySqlParameter("@MLCID", ctx.MLCID)
			};
			DataTable dt = new DynamicDbHelperMySQL(ctx).Query(sql, cmdParms).Tables[0];
			return ModelInfoManager.DataTableToList<RPTFilterSchemeModel>(dt);
		}

		public OperationResult DeleteFilterScheme(MContext ctx, string id)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "Update t_rpt_scheme set MIsDelete=1 where MItemID=@Id and MOrgID=@MOrgID";
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@Id", id)
			};
			CommandInfo item = commandInfo;
			list.Add(item);
			commandInfo = new CommandInfo();
			commandInfo.CommandText = "Update t_rpt_scheme_l set MIsDelete=1 where MParentID=@Id and MOrgID=@MOrgID";
			array = (commandInfo.Parameters = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@Id", id)
			});
			CommandInfo item2 = commandInfo;
			list.Add(item2);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public bool IsExistFilterSchemeName(MContext ctx, string schemeId, List<string> nameList)
		{
			if (nameList == null || nameList.Count() == 0)
			{
				throw new NullReferenceException("nameList is null");
			}
			bool flag = !string.IsNullOrWhiteSpace(schemeId);
			string str = "select MPKID from t_rpt_scheme_l where MOrgID=@MOrgId  and MIsDelete=0  and MParentID not in(select MItemID from t_rpt_scheme where MOrgID=@MOrgId and MIsDelete=1 )";
			if (flag)
			{
				str += " and MParentID<>@SchemeId ";
			}
			str += " and (";
			int num = flag ? (nameList.Count() + 2) : (nameList.Count() + 1);
			MySqlParameter[] array = new MySqlParameter[num];
			int num2 = 0;
			foreach (string name in nameList)
			{
				str += $" MName = @MName{num2} Or";
				array[num2] = new MySqlParameter("@MName" + num2, name);
				num2++;
			}
			if (flag)
			{
				array[num - 2] = new MySqlParameter("@SchemeId", schemeId);
			}
			array[num - 1] = new MySqlParameter("@MOrgId", ctx.MOrgID);
			str = str.Substring(0, str.Length - 2);
			str += " ) ";
			//bool flag2 = true;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(str, array);
			return dataSet != null && dataSet.Tables[0].Rows.Count > 0;
		}
	}
}
