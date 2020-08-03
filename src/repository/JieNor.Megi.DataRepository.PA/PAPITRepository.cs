using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.PA
{
	public class PAPITRepository
	{
		public static List<PAPITThresholdModel> GetPITThresholdList(MContext ctx, PAPITThresholdFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (filter.IsDefault)
			{
				stringBuilder.AppendLine("select '' as MItemID, t1.MEffectiveDate, t1.MAmount, t1.MAmount as MDefaultAmount, '' as MEmployeeID \r\n                                 from t_pa_pitthreshold t1\r\n                                 where t1.MOrgID = '0' and t1.MIsActive = 1 and t1.MIsDelete = 0 ");
			}
			else
			{
				stringBuilder.AppendLine("select t3.MItemID, t2.MEffectiveDate, ifnull(t3.MAmount, t2.MAmount) as MAmount, t2.MAmount as MDefaultAmount, t1.MItemID as MEmployeeID \r\n                                 from t_bd_employees t1\r\n                                    join t_pa_pitthreshold t2 on t2.MOrgID = '0' and t2.MIsActive = 1 and t2.MIsDelete = 0\r\n                                    left join t_pa_pitthreshold t3 on t3.MOrgID = t1.MOrgID and t3.MEmployeeID = t1.MItemID and t3.MEffectiveDate = t2.MEffectiveDate\r\n                                        and t3.MIsActive = 1 and t3.MIsDelete = 0\n                                 where t1.MOrgID = @MOrgID and t1.MIsDelete = 0 ");
			}
			if (!string.IsNullOrWhiteSpace(filter.EmployeeID))
			{
				stringBuilder.AppendLine(" and t1.MItemID = @MEmployeeID");
			}
			if (filter.SalaryDate != DateTime.MinValue)
			{
				stringBuilder.AppendLine(" and t2.MEffectiveDate <= @SalaryDate");
			}
			stringBuilder.Append(" order by MEmployeeID, MEffectiveDate desc");
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MEmployeeID", filter.EmployeeID),
				new MySqlParameter("@SalaryDate", filter.SalaryDate)
			};
			return ModelInfoManager.GetDataModelBySql<PAPITThresholdModel>(ctx, stringBuilder.ToString(), cmdParms);
		}

		public static List<PAPITTaxRateModel> GetPITTaxRateList(MContext ctx, DateTime salaryDate)
		{
			CheckSalaryPeriod(ref salaryDate);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT \n                                t1.*\n                            FROM\n                                t_pa_pittaxrate t1\n                                    JOIN\n                                (SELECT DISTINCT\n                                    MEffectiveDate\n                                FROM\n                                    t_pa_pitthreshold\n                                WHERE                                    \n\t\t\t\t\t\t\t\t\tMOrgID = '0'\n\t\t\t\t\t\t\t\t\t\tAND MEffectiveDate <= @salaryDate\n                                        AND MIsActive = 1\n                                        AND MIsDelete = 0\n                                ORDER BY MEffectiveDate DESC\n                                LIMIT 1) t2 ON t2.MEffectiveDate = t1.MEffectiveDate\n                            WHERE\n                                t1.MIsActive = 1 AND t1.MIsDelete = 0");
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@salaryDate", salaryDate)
			};
			return ModelInfoManager.GetDataModelBySql<PAPITTaxRateModel>(ctx, stringBuilder.ToString(), cmdParms);
		}

		public static PAPITThresholdModel GetPITThresholdModel(MContext ctx, DateTime period, string employeeId)
		{
			CheckSalaryPeriod(ref period);
			return GetPITThresholdList(ctx, new PAPITThresholdFilterModel
			{
				EmployeeID = employeeId,
				SalaryDate = period
			}).FirstOrDefault();
		}

		public static void CheckSalaryPeriod(ref DateTime period)
		{
			DateTime dateTime = new DateTime(2011, 9, 1);
			if (period < dateTime)
			{
				period = dateTime;
			}
		}
	}
}
