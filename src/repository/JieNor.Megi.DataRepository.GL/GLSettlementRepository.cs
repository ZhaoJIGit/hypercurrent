using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLSettlementRepository : DataServiceT<GLSettlementModel>
	{
		public GLSettlementModel GetSettlementModel(MContext ctx, GLSettlementModel model)
		{
			SqlWhere filter = new SqlWhere().AddFilter(new SqlFilter("MYear", SqlOperators.Equal, model.MYear)).AddFilter(new SqlFilter("MPeriod", SqlOperators.Equal, model.MPeriod));
			GLSettlementModel gLSettlementModel = GetDataModelByFilter(ctx, filter) ?? model;
			gLSettlementModel.IsNumberBroken = new GLVoucherRepository().CheckPeriodHasBrokenNumber(ctx, model.MYear, model.MPeriod);
			gLSettlementModel.UnsettledPeriod = GetUnsettledPeriod(ctx, model);
			DateTime currentPeriod = GetCurrentPeriod(ctx);
			gLSettlementModel.CurrentPeriod = currentPeriod.Year * 100 + currentPeriod.Month;
			return gLSettlementModel;
		}

		public bool IsPeirodSettled(MContext ctx, int year, int period)
		{
			GLSettlementModel settlementModel = GetSettlementModel(ctx, new GLSettlementModel
			{
				MYear = year,
				MPeriod = period,
				MOrgID = ctx.MOrgID
			});
			return settlementModel != null && settlementModel.MStatus == 1;
		}

		public bool IsPeriodsExistsSettled(MContext ctx, List<DateTime> dates)
		{
			List<string> intDates = new List<string>();
			dates.ForEach(delegate(DateTime x)
			{
				intDates.Add((x.Year * 12 + x.Month).ToString());
			});
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter("(MYear * 12 + MPeriod)", SqlOperators.In, intDates);
			sqlWhere.AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
			sqlWhere.AddFilter("MStatus", SqlOperators.Equal, 1);
			return ExistsByFilter(ctx, sqlWhere);
		}

		public List<GLSettlementModel> GetSettlementModel(MContext ctx, int? year, int? period, int setted = -1)
		{
			SqlWhere sqlWhere = new SqlWhere().AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
			if (year.HasValue && period.HasValue)
			{
				sqlWhere.AddFilter("MYear", SqlOperators.Equal, year.Value).AddFilter("MPeriod", SqlOperators.Equal, period.Value);
			}
			switch (setted)
			{
			case 1:
				sqlWhere.AddFilter("MStatus", SqlOperators.Equal, 1);
				break;
			case 0:
				sqlWhere.AddFilter("MStatus", SqlOperators.Equal, 0);
				break;
			}
			return GetModelList(ctx, sqlWhere, false) ?? new List<GLSettlementModel>();
		}

		public OperationResult Settle(MContext ctx, GLSettlementModel model)
		{
			List<GLSettlementModel> settlementModel = GetSettlementModel(ctx, model.MYear, model.MPeriod, -1);
			model.MItemID = ((settlementModel.Count > 0) ? settlementModel[0].MItemID : null);
			model.MOrgID = ctx.MOrgID;
			List<CommandInfo> deleteSettlementCmds = GetDeleteSettlementCmds(ctx, model.MYear, model.MPeriod);
			deleteSettlementCmds.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLSettlementModel>(ctx, model, null, true));
			int settlementStatus = (model.MStatus == 1) ? 1 : (-1);
			List<DateTime> list = GetNextSettledPeriod(ctx, model) ?? new List<DateTime>();
			list.Insert(0, new DateTime(model.MYear, model.MPeriod, 1));
			deleteSettlementCmds.AddRange(new GLBalanceRepository().GetSettlementBalanceCmds(ctx, list, settlementStatus));
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(deleteSettlementCmds) > 0)
			};
		}

		public List<DateTime> GetSettledPeriodFromBeginDate(MContext ctx, bool includeCurrentPeriod = false, bool includeBeginPeriod = true)
		{
			DateTime dateTime = ctx.MGLBeginDate;
			DateTime dateTime2 = dateTime.AddMonths(-1);
			List<DateTime> nextSettledPeriod = GetNextSettledPeriod(ctx, new GLSettlementModel
			{
				MOrgID = ctx.MOrgID,
				MYear = dateTime2.Year,
				MPeriod = dateTime2.Month
			});
			if (nextSettledPeriod == null || nextSettledPeriod.Count == 0)
			{
				return includeBeginPeriod ? new List<DateTime>
				{
					ctx.MGLBeginDate
				} : new List<DateTime>();
			}
			if (includeCurrentPeriod)
			{
				List<DateTime> list = nextSettledPeriod;
				dateTime = nextSettledPeriod[nextSettledPeriod.Count - 1];
				list.Add(dateTime.AddMonths(1));
			}
			return nextSettledPeriod;
		}

		public List<DateTime> GetNextSettledPeriod(MContext ctx, GLSettlementModel model)
		{
			List<DateTime> list = new List<DateTime>();
			string sql = "select * from t_gl_settlement a\r\n                    where a.MOrgID = @MOrgID \r\n                        and (a.MYear * 12 + a.MPeriod) >= (@MYear * 12 + @MPeriod) \r\n                        and a.MStatus = 1 \r\n                        and a.MIsDelete = 0 \r\n                    order by a.MYear asc,a.MPeriod asc";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MYear",
					Value = (object)model.MYear
				},
				new MySqlParameter
				{
					ParameterName = "@MPeriod",
					Value = (object)model.MPeriod
				}
			};
			List<GLSettlementModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<GLSettlementModel>(ctx, sql, cmdParms);
			if (dataModelBySql != null && dataModelBySql.Count > 0)
			{
				for (int i = 0; i < dataModelBySql.Count && dataModelBySql[i].MYear * 12 + dataModelBySql[i].MPeriod - (model.MYear * 12 + model.MPeriod) == i + 1; i++)
				{
					list.Add(new DateTime(dataModelBySql[i].MYear, dataModelBySql[i].MPeriod, 1));
				}
			}
			return list;
		}

		public DateTime GetCurrentPeriod(MContext ctx)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MStatus", 1);
			List<GLSettlementModel> modelList = GetModelList(ctx, sqlWhere, false);
			if (modelList == null || !modelList.Any())
			{
				return ctx.MGLBeginDate;
			}
			return (from x in modelList
			select new DateTime(x.MYear, x.MPeriod, 1)).Max().AddMonths(1);
		}

		public int GetLastSettementPeriod(MContext ctx)
		{
			string sql = "SELECT\r\n\t                            max(MYear * 100 + MPeriod) AS MYearPeriod\r\n                            FROM\r\n\t                            t_gl_settlement a\r\n                            WHERE\r\n\t                            a.MOrgID = @MOrgID\r\n                                AND a.MIsDelete = 0\r\n                                AND a.MStatus = 1";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, cmdParms);
			int result = 0;
			int.TryParse(Convert.ToString(single), out result);
			return result;
		}

		public List<GLUnsettlementModel> GetUnsettledPeriod(MContext ctx, int year, int period)
		{
			return GetUnsettledPeriod(ctx, new GLSettlementModel
			{
				MYear = year,
				MPeriod = period,
				MStatus = 0
			});
		}

		public List<GLUnsettlementModel> GetUnsettledPeriod(MContext ctx, GLSettlementModel model)
		{
			string sql = "select distinct a.MYear, a.MPeriod, a.MStatus from t_gl_settlement a\r\n                    where a.MOrgID = @MOrgID \r\n                      and a.MIsDelete = 0 \r\n                    order by a.MYear desc, a.MPeriod desc";
			MySqlParameter[] obj = new MySqlParameter[4]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MYear",
					Value = (object)model.MYear
				},
				new MySqlParameter
				{
					ParameterName = "@MPeriod",
					Value = (object)model.MPeriod
				},
				null
			};
			MySqlParameter obj2 = new MySqlParameter
			{
				ParameterName = "@MBeginPeriod"
			};
			DateTime mGLBeginDate = ctx.MGLBeginDate;
			int num = mGLBeginDate.Year * 12;
			mGLBeginDate = ctx.MGLBeginDate;
			obj2.Value = num + mGLBeginDate.Month;
			obj[3] = obj2;
			MySqlParameter[] cmdParms = obj;
			List<GLUnsettlementModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<GLUnsettlementModel>(ctx, sql, cmdParms);
			List<GLUnsettlementModel> list = new List<GLUnsettlementModel>();
			mGLBeginDate = ctx.MGLBeginDate;
			int num2 = mGLBeginDate.Year * 12;
			mGLBeginDate = ctx.MGLBeginDate;
			for (int i = num2 + mGLBeginDate.Month; i < model.MYear * 12 + model.MPeriod; i++)
			{
				int year = (i - 1) / 12;
				int period = (i - 1) % 12 + 1;
				if ((from x in dataModelBySql
				where x.MYear == year && x.MPeriod == period && x.MStatus == 1
				select x).Count() <= 0)
				{
					list.Add(new GLUnsettlementModel
					{
						MYear = year,
						MPeriod = period,
						MStatus = 0
					});
				}
			}
			return list;
		}

		public DateTime GetAvaliableVoucherDate(MContext ctx)
		{
			DateTime result = DateTime.Today;
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MStatus", 1);
			sqlWhere.AddOrderBy("MYear", SqlOrderDir.Asc);
			sqlWhere.AddOrderBy("MPeriod", SqlOrderDir.Asc);
			List<GLSettlementModel> modelList = new GLSettlementRepository().GetModelList(ctx, sqlWhere, false);
			if (modelList != null && modelList.Count > 0)
			{
				DateTime dateTime = new DateTime(modelList.Last().MYear, modelList.Last().MPeriod, 1);
				result = dateTime.AddMonths(1);
				int year = result.Year;
				dateTime = ctx.DateNow;
				int num;
				if (year == dateTime.Year)
				{
					int month = result.Month;
					dateTime = ctx.DateNow;
					num = ((month == dateTime.Month) ? 1 : 0);
				}
				else
				{
					num = 0;
				}
				if (num != 0)
				{
					result = ctx.DateNow;
				}
				else
				{
					int num2 = result.Year * 12 + result.Month;
					dateTime = ctx.DateNow;
					int num3 = dateTime.Year * 12;
					dateTime = ctx.DateNow;
					if (num2 < num3 + dateTime.Month)
					{
						dateTime = result.AddMonths(1);
						result = dateTime.AddDays(-1.0);
					}
				}
			}
			else
			{
				result = ((ctx.DateNow < ctx.MGLBeginDate) ? ctx.MGLBeginDate : ctx.DateNow);
			}
			return result;
		}

		public string GetLastFinishedPeriod(MContext ctx)
		{
			DateTime mGLBeginDate = ctx.MGLBeginDate;
			int year = mGLBeginDate.Year;
			mGLBeginDate = ctx.MGLBeginDate;
			DateTime dateTime = new DateTime(year, mGLBeginDate.Month, 1);
			DateTime dateTime2 = GetLastestVoucherDate(ctx);
			DateTime dateTime3 = DateTime.MinValue;
			dateTime2 = ((ctx.DateNow < dateTime2) ? dateTime2 : ctx.DateNow);
			dateTime3 = dateTime2;
			return dateTime.ToString("yyyy-MM-dd") + "," + dateTime2.ToString("yyyy-MM-dd") + "," + dateTime3.ToString("yyyy-MM-dd");
		}

		public DateTime GetLastestVoucherDate(MContext ctx)
		{
			string sql = " select max(MDate) MaxDate from t_gl_voucher where MOrgID = @MOrgID and MIsDelete = 0 ";
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, ctx.GetParameters((MySqlParameter)null));
			if (dataSet == null || (dataSet.Tables.Count == 0 && dataSet.Tables[0].Rows.Count == 0))
			{
				return DateTime.MinValue;
			}
			return dataSet.Tables[0].Rows[0].MField<DateTime>("MaxDate");
		}

		public CommandInfo UpdateSettlementByStatus(MContext ctx, string status, string oldStatus)
		{
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "UPDATE t_gl_settlement a set a.MStatus=@MStatus where a.MOrgID=@MOrgID and a.MStatus=@OldStatus";
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[3]
			{
				new MySqlParameter
				{
					ParameterName = "@MStatus",
					Value = status
				},
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@OldStatus",
					Value = oldStatus
				}
			};
			return commandInfo;
		}

		public List<DateTime> GetSettledPeriodList(MContext ctx)
		{
			List<DateTime> list = new List<DateTime>();
			if (ctx.MRegProgress == 15)
			{
				List<GLSettlementModel> modelList = base.GetModelList(ctx, new SqlWhere().AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID).AddFilter("MStatus", SqlOperators.Equal, 1).AddOrderBy("MYear", SqlOrderDir.Asc)
					.AddOrderBy("MPeriod", SqlOrderDir.Asc), false);
				int num = 0;
				while (modelList != null && num < modelList.Count)
				{
					list.Add(new DateTime(modelList[num].MYear, modelList[num].MPeriod, 1));
					num++;
				}
			}
			return list;
		}

		public List<GLSettlementModel> GetClosedPeriodList(MContext ctx)
		{
			string sql = " select * from t_gl_settlement where MOrgID = @MOrgID and MStatus = 1 and MIsDelete = 0 ";
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			return ModelInfoManager.GetDataModelBySql<GLSettlementModel>(ctx, sql, parameters.ToArray());
		}

		public void CheckPeriodHasSettlement(MContext ctx, int year, int period)
		{
			GLSettlementModel gLSettlementModel = GetSettlementModel(ctx, year, period, -1).FirstOrDefault();
			if (gLSettlementModel == null || string.IsNullOrWhiteSpace(gLSettlementModel.MItemID))
			{
				List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<GLSettlementModel>(ctx, new GLSettlementModel
				{
					MOrgID = ctx.MOrgID,
					MYear = year,
					MPeriod = period
				}, null, true);
				new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertOrUpdateCmd);
			}
		}

		public List<CommandInfo> GetDeleteSettlementCmds(MContext ctx, int year, int period)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = list;
			CommandInfo commandInfo = new CommandInfo
			{
				CommandText = "update t_gl_settlement t set t.MIsDelete = 1 where t.MOrgID = @MOrgID and (t.MYear * 12 + t.MPeriod >= @MYearPeriod)  and t.MIsDelete = 0 "
			};
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MYearPeriod",
					Value = (object)(year * 12 + period)
				}
			};
			list2.Add(commandInfo);
			List<CommandInfo> list3 = list;
			commandInfo = new CommandInfo
			{
				CommandText = "update t_gl_voucher t set t.MStatus = 0 where t.MOrgID = @MOrgID and (t.MYear * 12 + t.MPeriod > @MYearPeriod) and t.MIsDelete = 0 and MStatus = 1 "
			};
			array = (commandInfo.Parameters = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MYearPeriod",
					Value = (object)(year * 12 + period)
				}
			});
			list3.Add(commandInfo);
			return list;
		}

		public DataSet GetFullPeriod(MContext ctx)
		{
			string sql = "SELECT\r\n\t                            MYear,\r\n\t                            MPeriod\r\n                            FROM\r\n\t                            t_gl_voucher\r\n                            WHERE\r\n\t                            MOrgID = @MOrgID\r\n                                AND MIsDelete = 0\r\n                                AND IFNULL(MNumber, '') <> ''\r\n                                AND MYear * 100 + MPeriod >= @GLBeingPeriod\r\n                            GROUP BY\r\n\t                            MYear * 100 + MPeriod";
			MySqlParameter[] obj = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				null
			};
			MySqlParameter obj2 = new MySqlParameter
			{
				ParameterName = "@GLBeingPeriod"
			};
			DateTime mGLBeginDate = ctx.MGLBeginDate;
			int num = mGLBeginDate.Year * 100;
			mGLBeginDate = ctx.MGLBeginDate;
			obj2.Value = num + mGLBeginDate.Month;
			obj[1] = obj2;
			MySqlParameter[] cmdParms = obj;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Query(sql, cmdParms);
		}

		public DataSet GetMaxHadVoucherPeriod(MContext ctx)
		{
			string sql = "SELECT\r\n\t                            max(MYear * 100 + MPeriod)\r\n                            FROM\r\n\t                            t_gl_voucher\r\n                            WHERE\r\n\t                            MOrgID = @MOrgID\r\n                                AND MIsDelete = 0\r\n                                AND IFNULL(MNumber, '') <> ''\r\n                               ";
			MySqlParameter[] obj = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				null
			};
			MySqlParameter obj2 = new MySqlParameter
			{
				ParameterName = "@GLBeingPeriod"
			};
			DateTime mGLBeginDate = ctx.MGLBeginDate;
			int num = mGLBeginDate.Year * 100;
			mGLBeginDate = ctx.MGLBeginDate;
			obj2.Value = num + mGLBeginDate.Month;
			obj[1] = obj2;
			MySqlParameter[] cmdParms = obj;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Query(sql, cmdParms);
		}

		public GLSettlementModel GetCurrentSettlement(MContext ctx)
		{
			string sql = "SELECT * FROM t_gl_settlement \n                            WHERE MOrgID=@MOrgID AND MStatus=1 AND MIsActive=1 AND MIsDelete=0\n                            ORDER BY MYEAR*100 + MPeriod DESC \r\n                            LIMIT 1";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			return ModelInfoManager.GetDataModelBySql<GLSettlementModel>(ctx, sql, cmdParms).FirstOrDefault();
		}
	}
}
