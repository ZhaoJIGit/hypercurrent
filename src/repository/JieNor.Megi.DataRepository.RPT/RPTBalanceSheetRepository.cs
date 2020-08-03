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
	public class RPTBalanceSheetRepository
	{
		public static List<RPTGLAccountModel> GetBeginBalanceList(RPTBalanceSheetFilterModel filter, MContext ctx)
		{
			DateTime dateTime = ctx.DateNow;
			int year = dateTime.Year;
			string sql = " SELECT MAccountID, b.MCode AS MAccountCode,b.MDC  ,\r\n                            CASE WHEN b.MDC = '1' THEN SUM(MInitBalance + MYTDCredit -  MYTDDebit) ELSE 0 END AS MDebitAmt, \r\n                            CASE WHEN b.MDC = '-1' THEN SUM(MInitBalance + MYTDDebit -  MYTDCredit) ELSE 0 END AS MCreditAmt \r\n\t\t                    FROM T_GL_InitBalance P\r\n                            INNER JOIN (SELECT * FROM T_BD_ACCOUNT WHERE MOrgID=@MOrgID  AND MIsActive=1 AND MIsDelete=0 ) b ON P.MAccountID=b.MItemID\r\n\t\t                    WHERE P.MOrgID=@MOrgID AND  MCheckGroupValueID='0' AND p.MIsDelete=0 \r\n\t                        GROUP BY MAccountID\r\n                            ORDER BY b.MCode \r\n                         ";
			MySqlParameter[] obj = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				null,
				null
			};
			dateTime = ctx.MGLBeginDate;
			obj[1] = new MySqlParameter("@Year", dateTime.Year);
			dateTime = ctx.MGLBeginDate;
			obj[2] = new MySqlParameter("@Period", dateTime.Month);
			MySqlParameter[] cmdParms = obj;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<RPTGLAccountModel>(ds);
		}

		public static List<RPTGLAccountModel> GetInitBalanceList(RPTBalanceSheetFilterModel filter, MContext ctx)
		{
			DateTime dateTime = ctx.DateNow;
			int year = dateTime.Year;
			string sql = " SELECT MAccountID, b.MCode AS MAccountCode,b.MDC  , SUM(IFNULL(MInitBalance,0))  AS MBeginBalAmt\r\n\t\t                    FROM T_GL_InitBalance P\r\n                            INNER JOIN (SELECT * FROM T_BD_ACCOUNT WHERE MOrgID=@MOrgID  AND MIsActive=1 AND MIsDelete=0 ) b ON P.MAccountID=b.MItemID\r\n\t\t                    WHERE P.MOrgID=@MOrgID AND  MCheckGroupValueID='0' AND p.MIsDelete=0 \r\n\t                        GROUP BY  b.MItemID\r\n                            ORDER BY b.MCode \r\n                         ";
			MySqlParameter[] obj = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				null,
				null
			};
			dateTime = ctx.MGLBeginDate;
			obj[1] = new MySqlParameter("@Year", dateTime.Year);
			dateTime = ctx.MGLBeginDate;
			obj[2] = new MySqlParameter("@Period", dateTime.Month);
			MySqlParameter[] cmdParms = obj;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<RPTGLAccountModel>(ds);
		}

		public static List<RPTGLAccountModel> GetBeginBalanceListForOtherYear(RPTBalanceSheetFilterModel filter, MContext ctx)
		{
			DateTime dateTime = ctx.DateNow;
			int year = dateTime.Year;
			string sql = " SELECT b.MItemID AS MAccountID, b.MCode AS MAccountCode,b.MDC  ,\r\n                            CASE WHEN b.MDC = '1' THEN SUM(IFNULL(MBeginBalance,0)) ELSE 0 END AS MDebitAmt, \r\n                            CASE WHEN b.MDC = '-1' THEN SUM(IFNULL(MBeginBalance,0)) ELSE 0 END AS MCreditAmt \r\n\t\t                    FROM (SELECT * FROM T_BD_ACCOUNT WHERE MOrgID=@MOrgID  AND MIsActive=1 AND MIsDelete=0 ) b\r\n                            LEFT JOIN T_GL_Balance P ON P.MAccountID=b.MItemID\r\n\t\t                    AND P.MOrgID=@MOrgID AND  P.MCheckGroupValueID='0' AND P.MYear=@Year AND P.MPeriod=1 AND p.MIsDelete=0 \r\n\t                        GROUP BY b.MItemID\r\n                            ORDER BY b.MCode \r\n                         ";
			MySqlParameter[] obj = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				null,
				null
			};
			dateTime = filter.MDate;
			obj[1] = new MySqlParameter("@Year", dateTime.Year);
			dateTime = ctx.MGLBeginDate;
			obj[2] = new MySqlParameter("@Period", dateTime.Month);
			MySqlParameter[] cmdParms = obj;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			List<RPTGLAccountModel> list = ModelInfoManager.DataTableToList<RPTGLAccountModel>(ds);
			if (list == null || list.Count == 0)
			{
				return list;
			}
			ResetAmt(list, "1122", "2203");
			ResetAmt(list, "1123", "2202");
			ResetAmt(list, "1221", "2241");
			return list;
		}

		public static List<RPTGLAccountModel> GetEndBalanceList(RPTBalanceSheetFilterModel filter, MContext ctx)
		{
			DateTime dateTime = ctx.DateNow;
			int year = dateTime.Year;
			string sql = " select MAccountID, a.MCode AS MAccountCode, a.MDC,SUM(IFNULL(MCredit,0)) AS MCreditAmt, SUM(IFNULL(MDebit,0)) AS MDebitAmt,\n                            SUM(IFNULL(MBeginBalance,0)) AS MBeginBalAmt, \n                            SUM(IFNULL(MEndBalance,0)) AS MEndBalAmt from \r\n                            (SELECT * FROM T_BD_ACCOUNT WHERE MOrgID=@MOrgID  AND MIsActive=1 AND MIsDelete=0 ) a\r\n                            LEFT JOIN T_GL_Balance b ON b.MAccountID=a.MItemID AND b.MIsDelete=0 \r\n                            and b.MOrgID=@MOrgID AND b.MYear=@Year AND b.MPeriod=@Period AND b.MCheckGroupValueID='0'\r\n                            GROUP BY MAccountID,a.MCode\r\n                            ORDER BY a.MCode ";
			MySqlParameter[] obj = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				null,
				null
			};
			dateTime = filter.MDate;
			obj[1] = new MySqlParameter("@Year", dateTime.Year);
			dateTime = filter.MDate;
			obj[2] = new MySqlParameter("@Period", dateTime.Month);
			MySqlParameter[] cmdParms = obj;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			List<RPTGLAccountModel> list = ModelInfoManager.DataTableToList<RPTGLAccountModel>(ds);
			if (list == null || list.Count == 0)
			{
				return list;
			}
			ResetAmt(list, "1122", "2203");
			ResetAmt(list, "1123", "2202");
			ResetAmt(list, "1221", "2241");
			return list;
		}

		private static void ResetAmt(List<RPTGLAccountModel> list, string debitAmtCode, string creditAmtCode)
		{
			RPTGLAccountModel rPTGLAccountModel = (from t in list
			where t.MAccountCode == debitAmtCode
			select t).FirstOrDefault();
			RPTGLAccountModel rPTGLAccountModel2 = (from t in list
			where t.MAccountCode == creditAmtCode
			select t).FirstOrDefault();
			decimal num = rPTGLAccountModel.MBeginBalAmt + rPTGLAccountModel.MDebitAmt - rPTGLAccountModel.MCreditAmt;
			decimal num2 = rPTGLAccountModel2.MBeginBalAmt + rPTGLAccountModel2.MCreditAmt - rPTGLAccountModel2.MDebitAmt;
			int num3 = 0;
			foreach (RPTGLAccountModel item in list)
			{
				if (num3 == 2)
				{
					break;
				}
				if (item.MAccountCode == debitAmtCode)
				{
					num3++;
					if (num < decimal.Zero)
					{
						item.MBeginBalAmt = decimal.Zero;
						item.MDebitAmt = decimal.Zero;
						item.MCreditAmt = decimal.Zero;
					}
					if (num2 < decimal.Zero)
					{
						item.MDebitAmt -= num2;
					}
				}
				else if (item.MAccountCode == creditAmtCode)
				{
					num3++;
					if (num2 < decimal.Zero)
					{
						item.MBeginBalAmt = decimal.Zero;
						item.MDebitAmt = decimal.Zero;
						item.MCreditAmt = decimal.Zero;
					}
					if (num < decimal.Zero)
					{
						item.MCreditAmt -= num;
					}
				}
			}
		}

		private static RPTGLAccountModel GetGLAccountModel(List<RPTGLAccountModel> list, string code)
		{
			return (from t in list
			where t.MAccountCode == code
			select t).FirstOrDefault();
		}
	}
}
