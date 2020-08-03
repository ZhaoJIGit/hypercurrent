using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.DataRepository.RPT.Biz;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLCheckOtherRepository
	{
		public int CheckNewAccount(MContext ctx, DateTime beginTime, DateTime endTime)
		{
			string sql = " SELECT COUNT(*) RowCounts FROM t_bd_account WHERE MOrgID = @MOrgID \r\n                            AND MIsDelete=0 AND MCreateDate BETWEEN @BeginTime AND @EndTime ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@BeginTime", beginTime),
				new MySqlParameter("@EndTime", endTime)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, cmdParms);
			return (single != null) ? Convert.ToInt32(single) : 0;
		}

		public List<string> CheckVoucherNumber(MContext ctx, int year, int period)
		{
			string sql = " SELECT MNumber FROM t_gl_voucher \r\n                            WHERE MOrgID = @MOrgID AND MYear = @Year AND MPeriod = @Period \r\n                            AND MStatus>=0 AND LENGTH(ifnull(MNumber,'')) > 0 AND MIsDelete=0 ORDER BY convert(MNumber,signed) asc  ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@Year", year),
				new MySqlParameter("@Period", period)
			};
			return (from m in ModelInfoManager.GetDataModelBySql<GLVoucherModel>(ctx, sql, cmdParms)
			select m.MNumber).ToList();
		}

		public bool CheckFixAsset(MContext ctx, int year, int period)
		{
			RPTDepreciationDetailFilterModel filter = new RPTDepreciationDetailFilterModel();
			RPTDepreciationDetailFilterModel rPTDepreciationDetailFilterModel = filter;
			int num = year * 100 + period;
			rPTDepreciationDetailFilterModel.MStartPeroid = num.ToString();
			RPTDepreciationDetailFilterModel rPTDepreciationDetailFilterModel2 = filter;
			num = year * 100 + period;
			rPTDepreciationDetailFilterModel2.MEndPeroid = num.ToString();
			List<RPTDepreciationDetailModel> source = (from m in RPTDepreciationDetailRepository.GetDepreciationDetail(ctx, filter)
			orderby m.MFixAssetsNumber descending
			select m).ToList();
			List<FAFixAssetsChangeModel> fAChangeList = RPTFABaseRepository.GetFAChangeList(ctx);
			List<RPTDepreciationDetailModel> list = (from p in source
			group p by new
			{
				p.MitemId
			} into g
			select new RPTDepreciationDetailModel
			{
				MitemId = g.Key.MitemId
			}).ToList();
			List<RPTDepreciationDetailModel> list2 = new List<RPTDepreciationDetailModel>();
			DateTime startDate = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime dateTime = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture).AddMonths(1);
			foreach (RPTDepreciationDetailModel item in list)
			{
				RPTDepreciationDetailModel rPTDepreciationDetailModel = source.FirstOrDefault((RPTDepreciationDetailModel m) => m.MitemId == item.MitemId);
				if (rPTDepreciationDetailModel != null)
				{
					RPTDepreciationDetailModel rPTDepreciationDetailModel2 = new RPTDepreciationDetailModel();
					RPTDepreciationDetailRepository.GetValueChangeItems(fAChangeList, rPTDepreciationDetailModel, dateTime, startDate);
					decimal num2 = (rPTDepreciationDetailModel.MStatus != 0 && rPTDepreciationDetailModel.MHandledDate < dateTime) ? decimal.Zero : rPTDepreciationDetailModel.MOriginalAmount;
					decimal num3 = default(decimal);
					decimal num4 = default(decimal);
					decimal num5 = default(decimal);
					decimal num6 = default(decimal);
					DateTime dateTime2 = DateTime.ParseExact(filter.MStartPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
					DateTime t = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
					List<RPTDepreciationDetailModel> list3 = (from m in source
					orderby m.MYearPeriod descending
					where m.MitemId == item.MitemId && m.MYearPeriod <= Convert.ToInt32(filter.MEndPeroid) && m.MYearPeriod > 0 && !string.IsNullOrWhiteSpace(m.MVoucherID)
					select m).ToList();
					List<RPTDepreciationDetailModel> notDeplastItemModel = (from m in source
					orderby m.MYearPeriod descending
					where m.MitemId == item.MitemId && m.MYearPeriod <= Convert.ToInt32(filter.MEndPeroid) && m.MYearPeriod > 0
					select m).ToList();
					if (list3.Count <= 0)
					{
						if (ctx.MFABeginDate > t)
						{
							num3 = default(decimal);
							num5 = default(decimal);
							num4 = default(decimal);
						}
						else if (notDeplastItemModel.Count > 0)
						{
							List<RPTDepreciationDetailModel> source2 = (from m in notDeplastItemModel
							where m.MYearPeriod == notDeplastItemModel.FirstOrDefault().MYearPeriod
							select m).ToList();
							num3 = source2.Sum((RPTDepreciationDetailModel m) => m.MOrgDepreciatedAmount);
							num4 = source2.FirstOrDefault().MOrgDepreciatedAmountOfYear;
							num5 = rPTDepreciationDetailModel.MOrgPrepareForDecreaseAmount;
						}
						else
						{
							num3 = rPTDepreciationDetailModel.MOrgDepreciatedAmount;
							num5 = rPTDepreciationDetailModel.MOrgPrepareForDecreaseAmount;
							num4 = rPTDepreciationDetailModel.MOrgDepreciatedAmountOfYear;
						}
					}
					else
					{
						RPTDepreciationDetailModel rPTDepreciationDetailModel3 = list3.FirstOrDefault();
						int yearPeriod = rPTDepreciationDetailModel3.MYearPeriod;
						List<RPTDepreciationDetailModel> source3 = (from m in list3
						where m.MYearPeriod == yearPeriod
						select m).ToList();
						num3 = source3.Sum((RPTDepreciationDetailModel m) => m.MDepreciatedAmount);
						int year2 = t.Year;
						int num7 = yearPeriod / 100;
						num4 = ((year2 != num7) ? decimal.Zero : source3.Sum((RPTDepreciationDetailModel m) => m.MDepreciatedAmountOfYear));
						num5 = rPTDepreciationDetailModel.MOrgPrepareForDecreaseAmount;
					}
					if (rPTDepreciationDetailModel.MStatus != 0 && rPTDepreciationDetailModel.MHandledDate < dateTime)
					{
						num2 = default(decimal);
						num3 = default(decimal);
						num5 = default(decimal);
					}
					num6 = num2 - num3 - num5;
					rPTDepreciationDetailModel2.MOriginalAmount = num2;
					rPTDepreciationDetailModel2.MDepreciatedAmount = num3;
					rPTDepreciationDetailModel2.MDepreciatedAmountOfYear = num4;
					rPTDepreciationDetailModel2.MPrepareForDecreaseAmount = num5;
					rPTDepreciationDetailModel2.MNetAmount = num6;
					list2.Add(rPTDepreciationDetailModel2);
				}
			}
			decimal d = list2.Sum((RPTDepreciationDetailModel m) => m.MNetAmount);
			string sql = " SELECT t0.MCode AS MNumberID,t1.*\r\n                             FROM( SELECT MCode,MItemID FROM t_bd_account WHERE  MOrgID=@MOrgID AND MIsDelete = 0 \r\n                             AND MCode IN ('1601', '1602', '1603') ) t0\r\n                             INNER JOIN (\r\n                             SELECT * FROM t_gl_balance\r\n                             WHERE MIsDelete = 0  AND MCheckGroupValueID = '0' AND MYear = @MYear AND MPeriod = @MPeriod\r\n                             ) t1 ON t0.MItemID = t1.MAccountID ";
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MYear", year),
				new MySqlParameter("@MPeriod", period),
				new MySqlParameter("@MYearPeriod", year * 100 + period)
			};
			List<RPTAssetDepreciationModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<RPTAssetDepreciationModel>(ctx, sql, cmdParms);
			decimal d2 = default(decimal);
			decimal d3 = default(decimal);
			decimal d4 = default(decimal);
			decimal num8 = default(decimal);
			RPTAssetDepreciationModel rPTAssetDepreciationModel = dataModelBySql.FirstOrDefault((RPTAssetDepreciationModel m) => m.MNumberID == "1601");
			if (rPTAssetDepreciationModel != null)
			{
				d2 = rPTAssetDepreciationModel.MBeginBalance;
			}
			RPTAssetDepreciationModel rPTAssetDepreciationModel2 = dataModelBySql.FirstOrDefault((RPTAssetDepreciationModel m) => m.MNumberID == "1602");
			if (rPTAssetDepreciationModel2 != null)
			{
				d3 = rPTAssetDepreciationModel2.MBeginBalance;
			}
			RPTAssetDepreciationModel rPTAssetDepreciationModel3 = dataModelBySql.FirstOrDefault((RPTAssetDepreciationModel m) => m.MNumberID == "1603");
			if (rPTAssetDepreciationModel3 != null)
			{
				d4 = rPTAssetDepreciationModel3.MBeginBalance;
			}
			num8 = d2 - d3 - d4;
			string sql2 = " SELECT t1.*,t3.MCode AS MNumberID,t2.MExplanation AS MSummary,t2.MDebit,t2.MCredit \r\n                                FROM ( SELECT * FROM t_gl_voucher\r\n                                WHERE MOrgID = @MOrgID AND MIsDelete = 0  AND MStatus = 1\r\n                                AND MYear = @MYear AND MPeriod= @MPeriod ) t1\r\n                                INNER JOIN t_gl_voucherentry t2 ON t2.MID = t1.MitemID AND t2.MIsDelete = 0 \r\n                                INNER JOIN ( SELECT * FROM t_bd_account\r\n                                WHERE MOrgID = @MOrgID AND MIsDelete = 0  AND ( MCode LIKE '1601%' OR MCode LIKE '1602%' OR MCode LIKE '1603%' ) ) t3 \r\n                                ON t3.MItemID = t2.MAccountID  ";
			List<RPTAssetDepreciationModel> dataModelBySql2 = ModelInfoManager.GetDataModelBySql<RPTAssetDepreciationModel>(ctx, sql2, cmdParms);
			decimal d5 = default(decimal);
			decimal d6 = default(decimal);
			decimal d7 = default(decimal);
			decimal num9 = default(decimal);
			List<RPTAssetDepreciationModel> source4 = (from m in dataModelBySql2
			where m.MNumberID.StartsWith("1601")
			select m).ToList();
			if (source4.Any())
			{
				d5 = source4.Sum((RPTAssetDepreciationModel m) => m.MDebit) - source4.Sum((RPTAssetDepreciationModel m) => m.MCredit);
			}
			List<RPTAssetDepreciationModel> source5 = (from m in dataModelBySql2
			where m.MNumberID.StartsWith("1602")
			select m).ToList();
			if (source5.Any())
			{
				d6 = source5.Sum((RPTAssetDepreciationModel m) => m.MCredit) - source5.Sum((RPTAssetDepreciationModel m) => m.MDebit);
			}
			List<RPTAssetDepreciationModel> source6 = (from m in dataModelBySql2
			where m.MNumberID.StartsWith("1603")
			select m).ToList();
			if (source6.Any())
			{
				d7 = source6.Sum((RPTAssetDepreciationModel m) => m.MCredit) - source6.Sum((RPTAssetDepreciationModel m) => m.MDebit);
			}
			num9 = d5 - d6 - d7;
			decimal d8 = num8 + num9;
			if (d8 != d)
			{
				return false;
			}
			return true;
		}
	}
}
