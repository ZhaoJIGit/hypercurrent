using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDExchangeRateRepository : DataServiceT<BDExchangeRateModel>
	{
		public List<BDExchangeRateModel> GetExchangeRateList(BDExchangeRateModel model)
		{
			return new List<BDExchangeRateModel>();
		}

		public List<BDExchangeRateModel> GetMonthlyExchangeRateList(MContext ctx, DateTime date)
		{
			string sql = "select distinct\r\n               @MSourceCurrencyID as MSourceCurrencyID,a.MCurrencyID as MTargetCurrencyID ,T.MUserRate,T.MRate, T.MRateDate\r\n                from T_REG_Currency a left join T_BD_ExchangeRate T on a.MCurrencyID=T.MTargetCurrencyID and a.MOrgID=T.MOrgID and T.MIsDelete = 0 \r\n                where a.MOrgID = @MOrgID and a.MIsDelete = 0 \r\n                AND EXISTS( SELECT 1 FROM t_gl_balance t1 WHERE\r\n                t1.morgid = @MOrgID AND t1.myear = @MYear AND t1.mperiod = @MPeriod\r\n                and t1.MIsDelete = 0 AND t1.mcurrencyid = a.MCurrencyID)";
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MSourceCurrencyID", ctx.MBasCurrencyID),
				new MySqlParameter("@MYear", date.Year),
				new MySqlParameter("@MPeriod", date.Month)
			};
			List<BDExchangeRateModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BDExchangeRateModel>(ctx, sql, cmdParms);
			List<BDExchangeRateModel> list = new List<BDExchangeRateModel>();
			if (dataModelBySql != null && dataModelBySql.Count > 0)
			{
				List<string> targetCurrencyIDs = (from x in dataModelBySql
				select x.MTargetCurrencyID).Distinct().ToList();
				int i;
				for (i = 0; i < targetCurrencyIDs.Count; i++)
				{
					DateTime dateTime = new DateTime(date.Year, date.Month, 1);
					dateTime = dateTime.AddMonths(1);
					DateTime endDateOfMonth = dateTime.AddDays(-1.0);
					BDExchangeRateModel bDExchangeRateModel = dataModelBySql.FirstOrDefault(delegate(BDExchangeRateModel x)
					{
						DateTime mRateDate = x.MRateDate;
						int result;
						if (mRateDate.Year == date.Year)
						{
							mRateDate = x.MRateDate;
							if (mRateDate.Month == date.Month)
							{
								mRateDate = x.MRateDate;
								if (mRateDate.Day == endDateOfMonth.Day)
								{
									result = ((x.MTargetCurrencyID == targetCurrencyIDs[i]) ? 1 : 0);
									goto IL_009e;
								}
							}
						}
						result = 0;
						goto IL_009e;
						IL_009e:
						return (byte)result != 0;
					});
					if (bDExchangeRateModel == null)
					{
						list.Add(new BDExchangeRateModel
						{
							MSourceCurrencyID = ctx.MBasCurrencyID,
							MTargetCurrencyID = targetCurrencyIDs[i],
							MRateDate = endDateOfMonth
						});
					}
					else
					{
						list.Add(bDExchangeRateModel);
					}
				}
			}
			return list;
		}

		public DataGridJson<BDExchangeRateViewModel> GetExchangeRateViewList(BDExchangeRateFilterModel filter, MContext context)
		{
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SelectString = string.Format("\r\n                select \r\n                T.MItemID, T.MOrgID, T.MSourceCurrencyID, T.MModifyDate,\r\n                T.MTargetCurrencyID ,T.MUserRate,T.MRate, T.MRateDate,\r\n                L.MName as MSourceCurrencyName,\r\n                M.MName as MTargetCurrencyName\r\n                from T_BD_ExchangeRate T \r\n                join T_Bas_Currency_L L on L.MParentID =  @MSourceCurrencyID and L.MLocaleID = @MLocaleID and L.MIsDelete = 0 \r\n                join T_Bas_Currency_L M on M.MParentID =  @MTargetCurrencyID and M.MLocaleID = @MLocaleID and M.MIsDelete = 0\r\n                where T.MOrgID = @MOrgID \r\n                and T.MSourceCurrencyID = @MSourceCurrencyID\r\n                and T.MTargetCurrencyID = @MTargetCurrencyID\r\n                and T.MIsDelete = 0 \r\n                and T.MIsDelete = 0 ", context.MOrgID, filter.MSourceCurrencyID, filter.MTargetCurrencyID, context.MLCID);
			filter.AddParameter(new MySqlParameter("@MOrgID", context.MOrgID));
			filter.AddParameter(new MySqlParameter("@MLocaleID", context.MLCID));
			filter.AddParameter(new MySqlParameter("@MSourceCurrencyID", filter.MSourceCurrencyID));
			filter.AddParameter(new MySqlParameter("@MTargetCurrencyID", filter.MTargetCurrencyID));
			sqlQuery.SqlWhere = filter;
			if (!string.IsNullOrEmpty(filter.Sort))
			{
				sqlQuery.SqlWhere.OrderBy($" {filter.Sort} {filter.Order} ");
			}
			else
			{
				filter.AddOrderBy("T.MRateDate", SqlOrderDir.Desc);
				filter.AddOrderBy("T.MModifyDate", SqlOrderDir.Desc);
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(context);
			return ModelInfoManager.GetPageDataModelListBySql<BDExchangeRateViewModel>(context, sqlQuery);
		}

		public static BDExchangeRateModel GetEnableExchangeRate(MContext context, DateTime endDate, string cyId)
		{
			BDExchangeRateModel bDExchangeRateModel = new BDExchangeRateModel();
			REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
			endDate = endDate.Date;
			List<REGCurrencyViewModel> currencyList = GLDataPool.GetInstance(context, false, 0, 0, 0).CurrencyList;
			REGCurrencyViewModel rEGCurrencyViewModel = (from t in currencyList
			where t.MCurrencyID == cyId && !string.IsNullOrEmpty(t.MUserRate) && t.MRateDate.CompareTo(endDate) <= 0
			select t into a
			orderby a.MRateDate descending
			select a).FirstOrDefault();
			if (rEGCurrencyViewModel == null)
			{
				return null;
			}
			bDExchangeRateModel.MUserRate = Convert.ToDecimal(rEGCurrencyViewModel.MUserRate);
			bDExchangeRateModel.MRate = Convert.ToDecimal(rEGCurrencyViewModel.MRate);
			return bDExchangeRateModel;
		}

		public static decimal GetExchangeRate(MContext ctx, string from, DateTime date, string to = null)
		{
			to = (to ?? ctx.MBasCurrencyID);
			List<REGCurrencyViewModel> viewList = new REGCurrencyRepository().GetViewList(ctx, date, null, false, null);
			string text = string.Empty;
			if (from == ctx.MBasCurrencyID)
			{
				REGCurrencyViewModel rEGCurrencyViewModel = viewList.FirstOrDefault((REGCurrencyViewModel x) => x.MCurrencyID == to);
				if (rEGCurrencyViewModel != null)
				{
					text = rEGCurrencyViewModel.MRate;
				}
				goto IL_0147;
			}
			if (to == ctx.MBasCurrencyID)
			{
				REGCurrencyViewModel rEGCurrencyViewModel2 = viewList.FirstOrDefault((REGCurrencyViewModel x) => x.MCurrencyID == from);
				if (rEGCurrencyViewModel2 != null)
				{
					text = rEGCurrencyViewModel2.MRate;
				}
				goto IL_0147;
			}
			string mUserRate = viewList.FirstOrDefault((REGCurrencyViewModel x) => x.MCurrencyID == from).MUserRate;
			string mUserRate2 = viewList.FirstOrDefault((REGCurrencyViewModel x) => x.MCurrencyID == to).MUserRate;
			if (string.IsNullOrWhiteSpace(mUserRate) || string.IsNullOrWhiteSpace(mUserRate2))
			{
				return decimal.Zero;
			}
			decimal d = decimal.Parse(mUserRate) / decimal.Parse(mUserRate2);
			return Math.Round(d, 6, MidpointRounding.AwayFromZero);
			IL_0147:
			return (!string.IsNullOrWhiteSpace(text)) ? Math.Round(decimal.Parse(text), 6, MidpointRounding.AwayFromZero) : decimal.Zero;
		}

		public OperationResult DeleteCurrency(MContext ctx, BDExchangeRateModel model)
		{
			string sql = "Update T_BD_ExchangeRate set MIsDelete = 1\r\n                            where MItemID = @MItemID and MOrgID = @MOrgID and MIsDelete = 0 ";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return new OperationResult
			{
				Success = (dynamicDbHelperMySQL.ExecuteSql(sql, ctx.GetParameters("@MItemID", model.MItemID)) >= 1)
			};
		}

		public bool UpdateExchangeRate(MContext ctx, BDExchangeRateModel model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("update T_BD_ExchangeRate set ");
			stringBuilder.Append(" MUserRate = IFNULL(@MUserRate,MUserRate) , ");
			stringBuilder.Append(" MRate = IFNULL(@MRate,MRate),");
			stringBuilder.Append(" MRateDate = IFNULL(@MRateDate,MRateDate) ");
			stringBuilder.Append(" where MItemID = @MItemID and MOrgID = @MOrgID and MIsDelete = 0 ");
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MUserRate", MySqlDbType.Float),
				new MySqlParameter("@MRate", MySqlDbType.Float),
				new MySqlParameter("@MRateDate", MySqlDbType.DateTime),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = model.MItemID;
			array[1].Value = model.MUserRate;
			array[2].Value = model.MRate;
			array[3].Value = model.MRateDate;
			array[4].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.ExecuteSql(stringBuilder.ToString(), array) > 0;
		}
	}
}
