using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.FA
{
	public class FAFixAssetsChangeRepository : DataServiceT<FAFixAssetsChangeModel>
	{
		public List<FAFixAssetsChangeModel> GetFixAssetsChangeListForDepreciate(MContext ctx, FAFixAssetsFilterModel filter)
		{
			string sql = "SELECT \n                                *\n                            FROM\n                                t_fa_fixassetschange t\n                            WHERE\n                                MOrgID = @MOrgID\n                                    AND (MIndex = 0\n                                    OR (MType >= 4 && MType < 16))\n                                    AND MIsDelete = 0 " + ((!string.IsNullOrWhiteSpace(filter.MItemID)) ? " and MID = @MItemID " : "") + "\n                            ORDER BY MIndex DESC ";
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			list.AddRange(new List<MySqlParameter>
			{
				new MySqlParameter("@MItemID", filter.MItemID)
			});
			return ModelInfoManager.GetDataModelBySql<FAFixAssetsChangeModel>(ctx, sql, list.ToArray());
		}

		public List<FAFixAssetsChangeModel> GetFixAssetsChangeList(MContext ctx, List<string> idList)
		{
			string sql = "SELECT \n                                *\n                            FROM\n                                t_fa_fixassetschange t\n                            WHERE\n                                MOrgID = @MOrgID \n                                AND MID in ('" + string.Join("','", idList) + "')\n                                    AND MIsDelete = 0\n                            ORDER BY MID, MIndex DESC ";
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			return ModelInfoManager.GetDataModelBySql<FAFixAssetsChangeModel>(ctx, sql, list.ToArray());
		}

		public DataGridJson<FAFixAssetsChangeModel> GetFixAssetsChangeLog(MContext ctx, FAFixAssetsChangeFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = GetFixAssetsChangeLogSql();
			sqlQuery.AddParameter(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			{
				Value = ctx.MOrgID
			});
			sqlQuery.AddParameter(new MySqlParameter("@MLCID", MySqlDbType.VarChar, 6)
			{
				Value = ctx.MLCID
			});
			sqlQuery.AddParameter(new MySqlParameter("@MID", MySqlDbType.VarChar, 36)
			{
				Value = filter.MID
			});
			sqlQuery.SqlWhere.OrderBy(" MIndex DESC");
			return ModelInfoManager.GetPageDataModelListBySql<FAFixAssetsChangeModel>(ctx, sqlQuery);
		}

		public List<FAFixAssetsChangeModel> GetFixAssetsChangeLogNotPage(MContext ctx, FAFixAssetsChangeFilterModel filter)
		{
			string fixAssetsChangeLogSql = GetFixAssetsChangeLogSql();
			fixAssetsChangeLogSql += " ORDER BY MIndex DESC ";
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			list.AddRange(new List<MySqlParameter>
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36)
				{
					Value = filter.MID
				}
			});
			return ModelInfoManager.GetDataModelBySql<FAFixAssetsChangeModel>(ctx, fixAssetsChangeLogSql, list.ToArray());
		}

		private string GetFixAssetsChangeLogSql()
		{
			return " SELECT t1.*,t1_0.MName ,F_GETUSERNAME (t2.MFristName, t2.MLastName) AS MUserName, t3.MName AS MFATypeIDName, \r\n                                t4.MName AS MDepreciationTypeName ,t5_0.MFullName AS MDepAccountName,t6_0.MFullName AS MFixAccountName,t7_0.MFullName AS MExpAccountName\r\n                           FROM\r\n                            ( SELECT * FROM t_fa_fixassetschange \r\n                          WHERE\r\n                            MOrgID = @MOrgID AND MID = @MID  AND MIsDelete = 0 ) t1\r\n                          LEFT JOIN t_fa_fixassetschange_l t1_0 ON t1_0.MParentID = t1.MitemId \r\n                            AND t1_0.MOrgID = t1.MOrgID AND t1_0.MLocaleID = @MLCID AND t1_0.MIsDelete = 0 \r\n                          LEFT JOIN t_sec_user_l t2 ON t1.MCreatorID = t2.MParentID\r\n                            AND t2.MLocaleID = @MLCID AND t2.MIsDelete = 0\r\n                          LEFT JOIN t_fa_fixassetstype_l t3 ON t3.MParentID = t1.MFATypeID \r\n                            AND t3.MOrgID = t1.MOrgID AND t3.MLocaleID = @MLCID AND t3.MIsDelete = 0\r\n                          LEFT JOIN t_fa_depreciationtype_l t4 ON t4.MParentID = t1.MDepreciationTypeID\r\n                            AND t4.MLocaleID = @MLCID AND t4.MIsDelete = 0 \r\n                          LEFT JOIN t_bd_account t5 ON t5.MCode = t1.MDepAccountCode \r\n                            AND t5.MOrgID = t1.MOrgID AND t5.MIsDelete = 0\r\n                          LEFT JOIN t_bd_account_l t5_0 ON t5_0.MParentID = t5.MItemID\r\n                            AND t5_0.MOrgID = t1.MOrgID AND t5_0.MLocaleID = @MLCID AND t5_0.MIsDelete = 0\r\n                          LEFT JOIN t_bd_account t6 ON t6.MCode = t1.MFixAccountCode \r\n                            AND t6.MOrgID = t1.MOrgID AND t6.MIsDelete = 0\r\n                          LEFT JOIN t_bd_account_l t6_0 ON t6_0.MParentID = t6.MItemID\r\n                            AND t6_0.MOrgID = t1.MOrgID AND t6_0.MLocaleID = @MLCID AND t6_0.MIsDelete = 0\r\n                          LEFT JOIN t_bd_account t7 ON t7.MCode = t1.MExpAccountCode \r\n                            AND t7.MOrgID = t1.MOrgID AND t7.MIsDelete = 0\r\n                          LEFT JOIN t_bd_account_l t7_0 ON t7_0.MParentID = t7.MItemID\r\n                            AND t7_0.MOrgID = t1.MOrgID AND t7_0.MLocaleID = @MLCID AND t7_0.MIsDelete = 0 ";
		}
	}
}
