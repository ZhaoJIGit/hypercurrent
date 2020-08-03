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
	public class RPTIncomeStatementRepository
	{
		public static List<RPTGLAccountModel> GetYearIncomeStatementList(RPTIncomeStatementFilterModel filter, MContext ctx)
		{
			string sql = " SELECT MAccountID,MAccountCode,MDC,MCreditAmt,MDebitAmt FROM (\r\n                            select MAccountID, b.MCode AS MAccountCode, b.MDC,SUM(MYtdCredit) AS MCreditAmt, SUM(MYtdDebit) AS MDebitAmt from t_gl_balance a\r\n                            inner join (SELECT * FROM T_BD_ACCOUNT WHERE MOrgID=@MOrgID AND MIsActive=1 AND MIsDelete=0 ) b ON a.MAccountID=b.MItemID\r\n                            WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0  AND MYear=@Year AND MPeriod=@MPeriod\r\n                            AND a.MCheckGroupValueID='0'  GROUP BY b.MCode) P                         \r\n                            GROUP BY MAccountCode ";
			return GetYearIncomeStatementList(filter, ctx, sql);
		}

		public static List<RPTGLAccountModel> GetYearIncomeStatementListByConversionYear(RPTIncomeStatementFilterModel filter, MContext ctx)
		{
			string sql = " SELECT MAccountID,MAccountCode,MDC,SUM(MCreditAmt) AS MCreditAmt,SUM(MDebitAmt) AS MDebitAmt FROM (\r\n                            select MAccountID, b.MCode AS MAccountCode, b.MDC,SUM((CASE WHEN b.MDC=-1 THEN MYtdCredit ELSE 0 END)) AS MCreditAmt, SUM((CASE WHEN b.MDC=1 THEN MYtdDebit ELSE 0 END)) AS MDebitAmt \r\n                            from t_gl_initbalance a\r\n                            inner join (SELECT * FROM T_BD_ACCOUNT WHERE MOrgID=@MOrgID AND MIsActive=1 AND MIsDelete=0 ) b ON a.MAccountID=b.MItemID\r\n                            WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0\r\n                            AND a.MCheckGroupValueID='0'  GROUP BY b.MCode\r\n                            UNION ALL\r\n                            select MAccountID, b.MCode AS MAccountCode, b.MDC,SUM((CASE WHEN b.MDC=-1 THEN MYtdCredit-MYtdDebit ELSE 0 END)) AS MCreditAmt, SUM((CASE WHEN b.MDC=1 THEN MYtdDebit-MYtdCredit ELSE 0 END)) AS MDebitAmt from t_gl_balance a\r\n                            inner join (SELECT * FROM T_BD_ACCOUNT WHERE MOrgID=@MOrgID AND MIsActive=1 AND MIsDelete=0 ) b ON a.MAccountID=b.MItemID\r\n                            WHERE a.MOrgID=@MOrgID  AND a.MIsDelete=0  AND MYear=@Year AND MPeriod=@MPeriod\r\n                            AND a.MCheckGroupValueID='0'  GROUP BY b.MCode\r\n                            ) P                         \r\n                            GROUP BY MAccountCode ";
			return GetYearIncomeStatementList(filter, ctx, sql);
		}

		private static List<RPTGLAccountModel> GetYearIncomeStatementList(RPTIncomeStatementFilterModel filter, MContext ctx, string sql)
		{
			MySqlParameter[] obj = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				null,
				null
			};
			DateTime mToDate = filter.MToDate;
			obj[1] = new MySqlParameter("@Year", mToDate.Year);
			mToDate = filter.MToDate;
			obj[2] = new MySqlParameter("MPeriod", mToDate.Month);
			MySqlParameter[] cmdParms = obj;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<RPTGLAccountModel>(ds);
		}

		public static List<RPTGLAccountModel> GetIncomeStatementList(RPTIncomeStatementFilterModel filter, MContext ctx)
		{
			DateTime dateTime = ctx.DateNow;
			int year = dateTime.Year;
			string sql = " select MAccountID, b.MCode AS MAccountCode,b.MDC, SUM(MCredit) AS MCreditAmt, SUM(MDebit) AS MDebitAmt from t_gl_balance a\r\n                            inner join (SELECT * FROM T_BD_ACCOUNT WHERE MOrgID=@MOrgID AND MIsActive=1 AND MIsDelete=0 ) b ON a.MAccountID=b.MItemID\r\n                            WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0  AND MYearPeriod>=@FromYearPeriod AND MYearPeriod<=@ToYearPeriod\r\n                            and a.MCheckGroupValueID='0'\r\n                            GROUP BY MAccountID,B.MCode ";
			MySqlParameter[] obj = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				null,
				null
			};
			dateTime = filter.MFromDate;
			obj[1] = new MySqlParameter("@FromYearPeriod", dateTime.ToString("yyyyMM"));
			dateTime = filter.MToDate;
			obj[2] = new MySqlParameter("@ToYearPeriod", dateTime.ToString("yyyyMM"));
			MySqlParameter[] cmdParms = obj;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<RPTGLAccountModel>(ds);
		}
	}
}
