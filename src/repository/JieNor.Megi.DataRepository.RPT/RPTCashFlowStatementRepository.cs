using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JieNor.Megi.DataRepository.RPT
{
	public class RPTCashFlowStatementRepository
	{
		public static List<RPTGLAccountModel> GetCashFlowStatementList(RPTCashFlowStatementFilterModel filter, MContext ctx)
		{
			string sql = " select MAccountID, b.MCode AS MAccountCode,b.MDC, SUM(MCredit) AS MCreditAmt, SUM(MDebit) AS MDebitAmt,\n                            SUM( CASE WHEN MYearPeriod=@FromYearPeriod THEN MBeginBalance ELSE 0 END) AS MBeginBalAmt, \n                            SUM(CASE WHEN MYearPeriod=@ToYearPeriod THEN MEndBalance ELSE 0 END) AS MEndBalAmt  \n                            from t_gl_balance a\n                            inner join (SELECT * FROM T_BD_ACCOUNT WHERE MOrgID=@MOrgID  AND MIsActive=1 AND MIsDelete=0) b ON a.MAccountID=b.MItemID\r\n                            WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0 AND MYearPeriod>=@FromYearPeriod AND MYearPeriod<=@ToYearPeriod\r\n                            and a.MCheckGroupValueID='0'\n                            GROUP BY MAccountID,B.MCode,b.b.MDC\r\n                            ORDER BY B.MCode";
			MySqlParameter[] obj = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				null,
				null
			};
			DateTime dateTime = filter.MFromDate;
			obj[1] = new MySqlParameter("@FromYearPeriod", dateTime.ToString("yyyyMM"));
			dateTime = filter.MToDate;
			obj[2] = new MySqlParameter("@ToYearPeriod", dateTime.ToString("yyyyMM"));
			MySqlParameter[] cmdParms = obj;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			List<RPTGLAccountModel> list = ModelInfoManager.DataTableToList<RPTGLAccountModel>(ds);
			dateTime = filter.MFromDate;
			int year = dateTime.Year;
			dateTime = filter.MToDate;
			int num;
			if (year == dateTime.Year)
			{
				dateTime = filter.MFromDate;
				int month = dateTime.Month;
				dateTime = filter.MToDate;
				num = ((month != dateTime.Month) ? 1 : 0);
			}
			else
			{
				num = 1;
			}
			if (num != 0)
			{
				List<RPTGLAccountModel> cashBalance = GetCashBalance(ctx, filter.MFromDate);
				if (cashBalance != null && cashBalance.Count > 0)
				{
					RPTGLAccountModel rPTGLAccountModel = (from t in list
					where t.MAccountCode == "1001"
					select t).FirstOrDefault();
					RPTGLAccountModel rPTGLAccountModel2 = (from t in list
					where t.MAccountCode == "1002"
					select t).FirstOrDefault();
					RPTGLAccountModel rPTGLAccountModel3 = (from t in list
					where t.MAccountCode == "1012"
					select t).FirstOrDefault();
					RPTGLAccountModel rPTGLAccountModel4 = (from t in cashBalance
					where t.MAccountCode == "1001"
					select t).FirstOrDefault();
					RPTGLAccountModel rPTGLAccountModel5 = (from t in cashBalance
					where t.MAccountCode == "1002"
					select t).FirstOrDefault();
					RPTGLAccountModel rPTGLAccountModel6 = (from t in cashBalance
					where t.MAccountCode == "1012"
					select t).FirstOrDefault();
					if (rPTGLAccountModel != null && rPTGLAccountModel4 != null)
					{
						rPTGLAccountModel.MBeginBalAmt = rPTGLAccountModel4.MBeginBalAmt;
					}
					if (rPTGLAccountModel2 != null && rPTGLAccountModel5 != null)
					{
						rPTGLAccountModel2.MBeginBalAmt = rPTGLAccountModel5.MBeginBalAmt;
					}
					if (rPTGLAccountModel3 != null && rPTGLAccountModel6 != null)
					{
						rPTGLAccountModel3.MBeginBalAmt = rPTGLAccountModel6.MBeginBalAmt;
					}
				}
			}
			return list;
		}

		private static List<RPTGLAccountModel> GetCashBalance(MContext ctx, DateTime date)
		{
			string sql = " select MAccountID, b.MCode AS MAccountCode,b.MDC, SUM(MCredit) AS MCreditAmt, SUM(MDebit) AS MDebitAmt,\n                            SUM(MBeginBalance) AS MBeginBalAmt, \n                            SUM(MEndBalance) AS MEndBalAmt  \n                            from t_gl_balance a\n                            inner join (SELECT * FROM T_BD_ACCOUNT WHERE MOrgID=@MOrgID  AND MIsActive=1 AND MIsDelete=0 AND MCode IN('1001','1002','1012')) b ON a.MAccountID=b.MItemID\r\n                            WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0 AND MYearPeriod=@YearPeriod \r\n                            and a.MCheckGroupValueID='0'\n                            GROUP BY MAccountID,B.MCode,b.b.MDC\r\n                            ORDER BY B.MCode";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@YearPeriod", date.ToString("yyyyMM"))
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<RPTGLAccountModel>(ds);
		}
	}
}
