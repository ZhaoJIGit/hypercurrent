using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.DataRepository.RPT
{
	public class RPTReportConfigRepository
	{
		public static List<RPTReportConfigModel> GetConfigList(MContext ctx, string acctTableID, RPTGLReportType type)
		{
			string sql = "SELECT a.MItemID,a.MParentID,a.MAcctTableID,a.MReportType, CASE WHEN IFNULL(b.MName,'')='' THEN a.MName ELSE b.MName END AS MName,a.MNumber,a.MRowType,a.MFormula,a.MRemark,a.MSequence,a.MTag  FROM T_GL_ReportConfig a\r\n                          LEFT JOIN T_GL_ReportConfig_L b ON a.MItemID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete=0 \r\n                          WHERE a.MAcctTableID=@MAcctTableID AND  a.MReportType=@MReportType AND a.MIsDelete=0  Order by MSequence ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MAcctTableID", acctTableID),
				new MySqlParameter("@MReportType", Convert.ToInt32(type))
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<RPTReportConfigModel>(ds);
		}

		public static List<RPTReportConfigModel> GetAmountList(MContext ctx, string sql)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql);
			return ModelInfoManager.DataTableToList<RPTReportConfigModel>(ds);
		}
	}
}
