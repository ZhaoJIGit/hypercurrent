using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace JieNor.Megi.DataRepository.SYS
{
	public class SYSCouponRepository : DataServiceT<SYSCouponModel>
	{
		public SYSCouponModel GetCouponModel(string code)
		{
			if (string.IsNullOrEmpty(code))
			{
				return null;
			}
			return base.GetDataModelByFilter(new MContext
			{
				IsSys = true
			}, new SqlWhere().Equal("MCode", code));
		}

		public bool ApplyCouponForTrial(MContext ctx, string couponId, string couponCode, int day)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.Add(GetUpdateOrgCmdSql(ctx, day));
			list.Add(GetApplyCouponCmdSql(ctx, couponCode));
			list.Add(GetUpdateCouponUserCmdSql(ctx, couponId));
			int num = DbHelperMySQL.ExecuteSqlTran(ctx, list);
			return num > 0;
		}

		private CommandInfo GetUpdateOrgCmdSql(MContext ctx, int day)
		{
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = $"UPDATE T_Bas_Organisation SET MExpiredDate=date_add(MExpiredDate, INTERVAL {day} day) WHERE MItemID=@MOrgID";
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return commandInfo;
		}

		private CommandInfo GetApplyCouponCmdSql(MContext ctx, string code)
		{
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "UPDATE T_Sys_Coupon SET  MApplyOrgID=@MOrgID,MApplyUserID=@MUserID,MApplyDate=@MDate WHERE MCode=@MCode";
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[5]
			{
				new MySqlParameter("@MStatuID", Convert.ToInt32(SYSCouponStatu.Used)),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MUserID", ctx.MUserID),
				new MySqlParameter("@MDate", ctx.DateNow),
				new MySqlParameter("@MCode", code)
			};
			return commandInfo;
		}

		private CommandInfo GetUpdateCouponUserCmdSql(MContext ctx, string couponId)
		{
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "INSERT INTO T_Sys_CouponUser(MItemID,MCouponID,MUserID) VALUES(@MItemID,@MCouponID,@MUserID)";
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MCouponID", couponId),
				new MySqlParameter("@MUserID", ctx.MUserID),
				new MySqlParameter("@MItemID", UUIDHelper.GetGuid())
			};
			return commandInfo;
		}
	}
}
