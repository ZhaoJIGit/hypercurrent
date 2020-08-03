using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLVoucherEntryRepository : DataServiceT<GLVoucherEntryModel>
	{
		private readonly GLVoucherRepository voucher = new GLVoucherRepository();

		private readonly GLUtility utility = new GLUtility();

		public static CommandInfo GetUpdateEntryAccountIDCmd(MContext ctx, string newAccountID, string oldAccountID)
		{
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update T_GL_VoucherEntry t set t.MAccountID=@newAccountID where t.MAccountID=@oldAccountID and t.MOrgID = @MOrgID and t.MIsDelete = 0 ";
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@newAccountID", newAccountID),
				new MySqlParameter("@oldAccountID", oldAccountID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			DbParameter[] array = commandInfo.Parameters = parameters;
			return commandInfo;
		}

		public static List<CommandInfo> GetDeleteVoucherEntryCmd(MContext ctx, List<string> pkIDS)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
			List<CommandInfo> list3 = list;
			CommandInfo commandInfo = new CommandInfo
			{
				CommandText = "update t_gl_voucherentry t set t.MIsDelete = 1 where t.MOrgID = @MOrgID and MID " + GLUtility.GetInFilterQuery(pkIDS, ref list2, "M_ID") + "  and t.MIsDelete = 0 and exists ( select 1 from t_gl_voucher a where a.MOrgID = @MOrgID and a.MItemID " + GLUtility.GetInFilterQuery(pkIDS, ref list2, "M_ID") + " and a.MIsDelete = 1 ) "
			};
			DbParameter[] array = commandInfo.Parameters = list2.ToArray();
			list3.Add(commandInfo);
			return list;
		}

		public List<GLVoucherEntryModel> MergeListByAccount(List<GLVoucherEntryModel> list)
		{
			if (list == null || list.Count == 0)
			{
				return list;
			}
			List<GLVoucherEntryModel> list2 = new List<GLVoucherEntryModel>();
			list = (from x in list
			orderby x.MAccountID
			select x).ToList();
			GLVoucherEntryModel gLVoucherEntryModel = null;
			for (int i = 0; i < list.Count; i++)
			{
				GLVoucherEntryModel gLVoucherEntryModel2 = list[i];
				if (gLVoucherEntryModel != null && (gLVoucherEntryModel2.MEntryID != gLVoucherEntryModel.MEntryID || gLVoucherEntryModel2.MDC != gLVoucherEntryModel.MDC))
				{
					gLVoucherEntryModel.MEntryID.TrimEnd(',');
					list2.Add(gLVoucherEntryModel);
					gLVoucherEntryModel = null;
				}
				if (gLVoucherEntryModel == null)
				{
					gLVoucherEntryModel = gLVoucherEntryModel2;
				}
				else
				{
					GLVoucherEntryModel gLVoucherEntryModel3 = gLVoucherEntryModel;
					gLVoucherEntryModel3.MDebit += gLVoucherEntryModel2.MDebit;
					GLVoucherEntryModel gLVoucherEntryModel4 = gLVoucherEntryModel;
					gLVoucherEntryModel4.MCredit += gLVoucherEntryModel2.MCredit;
					GLVoucherEntryModel gLVoucherEntryModel5 = gLVoucherEntryModel;
					gLVoucherEntryModel5.MAmount += gLVoucherEntryModel2.MAmount;
					GLVoucherEntryModel gLVoucherEntryModel6 = gLVoucherEntryModel;
					gLVoucherEntryModel6.MAmountFor += gLVoucherEntryModel2.MAmountFor;
					GLVoucherEntryModel gLVoucherEntryModel7 = gLVoucherEntryModel;
					gLVoucherEntryModel7.MEntryID = gLVoucherEntryModel7.MEntryID + gLVoucherEntryModel2.MEntryID + ",";
				}
			}
			return list;
		}

		public List<GLVoucherEntryModel> GetListByPeriodAccountCodeOrType(MContext ctx, int year, int period, List<string> codes, List<string> types, bool merge = false)
		{
			string str = "select t2.*  from\n                    t_gl_voucher t1\n                    inner join \n                    t_gl_voucherentry t2\n                    on t2.MID = t1.MItemID\n                    and t1.MOrgID = t2.MOrgID\n                    and t2.MIsDelete = 0 \n                    inner join \n                    t_bd_account t3\n                    on t3.MItemID = t2.MAccountID\n                    and t3.MOrgID = t1.MOrgID\n                    and t3.MIsDelete = 0 \n                    where t1.MYear = @MYear\n                    and t1.MPERIOD = @MPeriod\n                    and t1.MIsDelete = 0 \n                    and t1.MOrgID = @MOrgID\r\n                    and length(ifnull(t1.MNumber,'')) > 0\r\n                    and t1.MStatus = 1 ";
			if (codes != null && codes.Count > 0)
			{
				str += " and t3.MCode in( '{0}' )";
			}
			if (types != null && types.Count > 0)
			{
				str += " and t3.MAccountTypeID in( '{1}' )";
			}
			str += " order by t2.MAccountID ";
			MySqlParameter[] source = new MySqlParameter[3]
			{
				new MySqlParameter
				{
					ParameterName = "@MYear",
					Value = (object)year
				},
				new MySqlParameter
				{
					ParameterName = "@MPeriod",
					Value = (object)period
				},
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			str = string.Format(str, (codes == null) ? "" : string.Join("','", codes), (types == null) ? "" : string.Join("','", types));
			List<GLVoucherEntryModel> list = ModelInfoManager.DataTableToList<GLVoucherEntryModel>(new DynamicDbHelperMySQL(ctx).Query(str, source.ToArray()));
			if (merge)
			{
				return MergeListByAccount(list);
			}
			return list;
		}

		public GLVoucherEntryModel GetSumEntryByAccountCodeOrType(MContext ctx, int year, int period, List<string> codes = null, List<string> types = null)
		{
			GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel
			{
				MCredit = decimal.Zero,
				MDebit = decimal.Zero
			};
			List<GLVoucherEntryModel> listByPeriodAccountCodeOrType = GetListByPeriodAccountCodeOrType(ctx, year, period, codes, types, false);
			if (listByPeriodAccountCodeOrType != null && listByPeriodAccountCodeOrType.Count > 0)
			{
				gLVoucherEntryModel.MDebit = listByPeriodAccountCodeOrType.Sum((GLVoucherEntryModel x) => x.MDebit);
				gLVoucherEntryModel.MCredit = listByPeriodAccountCodeOrType.Sum((GLVoucherEntryModel x) => x.MCredit);
			}
			return gLVoucherEntryModel;
		}

		public OperationResult UpdateVoucherEntrys(MContext ctx, GLVoucherModel voucher)
		{
			List<GLVoucherEntryModel> source = (from x in voucher.MVoucherEntrys
			where !string.IsNullOrWhiteSpace(x.MAccountID)
			select x).ToList();
			(from x in source
			where !string.IsNullOrWhiteSpace(x.MAccountID)
			select x).ToList().ForEach(delegate(GLVoucherEntryModel x)
			{
				x.MCheckGroupValueID = utility.GetCheckGroupValueModel(ctx, x.MCheckGroupValueModel).MItemID;
			});
			List<CommandInfo> insertOrUpdateCmds = ModelInfoManager.GetInsertOrUpdateCmds(ctx, voucher.MVoucherEntrys, null, true);
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertOrUpdateCmds) > 0)
			};
		}
	}
}
