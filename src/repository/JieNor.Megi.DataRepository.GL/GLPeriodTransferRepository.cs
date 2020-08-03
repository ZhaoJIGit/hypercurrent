using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLPeriodTransferRepository : DataServiceT<GLPeriodTransferModel>
	{
		public static List<CommandInfo> GetDeleteCmdByVoucherID(MContext ctx, List<string> voucherIDs)
		{
			if (voucherIDs == null || !voucherIDs.Any())
			{
				return new List<CommandInfo>();
			}
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string commandText = "update t_gl_periodTransfer t set t.MIsDelete = 1 where t.MOrgID = @MOrgID  and t.MIsDelete = 0 and t.MVoucherID " + GLUtility.GetInFilterQuery(voucherIDs, ref list, "M_ID");
			List<CommandInfo> list2 = new List<CommandInfo>();
			CommandInfo obj = new CommandInfo
			{
				CommandText = commandText
			};
			DbParameter[] array = obj.Parameters = list.ToArray();
			list2.Add(obj);
			return list2;
		}

		public List<GLPeriodTransferModel> GetModelByFilter(MContext ctx, int? year, int? period, int? type, int? status)
		{
			string text = "select * from t_gl_periodtransfer t1 where t1.MOrgID = @MOrgID and t1.MIsDelete = 0  and \r\n             exists (select 1 from t_gl_voucher t2 where t2.MOrgID = t1.MOrgID and t2.MItemID = t1.MVoucherID and t2.MIsDelete = 0 and (t2.MStatus  = 0 or t2.MStatus = 1))";
			List<MySqlParameter> list = new List<MySqlParameter>();
			if (year.HasValue)
			{
				text += " and t1.MYear = @MYear";
				list.Add(new MySqlParameter
				{
					ParameterName = "@MYear",
					Value = (object)year
				});
			}
			if (period.HasValue)
			{
				text += " and t1.MPeriod = @MPeriod";
				list.Add(new MySqlParameter
				{
					ParameterName = "@MPeriod",
					Value = (object)period
				});
			}
			if (type.HasValue)
			{
				text += " and t1.MTransferTypeID = @MType";
				list.Add(new MySqlParameter
				{
					ParameterName = "@MType",
					Value = (object)type
				});
			}
			if (status.HasValue)
			{
				text += " and t1.MStatus = @MStatus";
				list.Add(new MySqlParameter
				{
					ParameterName = "@MStatus",
					Value = (object)status
				});
			}
			list.Add(new MySqlParameter
			{
				ParameterName = "@MOrgID",
				Value = ctx.MOrgID
			});
			return ModelInfoManager.GetDataModelBySql<GLPeriodTransferModel>(ctx, text, list.ToArray());
		}

		public List<GLPeriodTransferModel> GetModelByPeriodAndType(MContext ctx, int startYearPeriod, int endYearPeriod, int typeID, int? status)
		{
			string sql = "select t1.* from t_Gl_periodtransfer t1\r\n                            inner join t_gl_voucher t2 on\r\n                            t1.MVoucherID = t2.MItemID\r\n                            and t1.MOrgID = t2.MOrgID" + (status.HasValue ? " and t2.MStatus = @MStatus " : "") + "\r\n                            and t2.MIsDelete = 0 \r\n                            where \r\n                            t1.MTransferTypeID = @MTypeID\r\n                            and (t1.MYear * 12 + t1.MPeriod) >= @StartYearPeriod\r\n                            and (t1.MYear * 12 + t1.MPeriod) <= @EndYearPeriod\r\n                            and t1.MOrgID = @MOrgID\r\n                            and t1.MIsDelete = 0 ";
			MySqlParameter[] source = new MySqlParameter[5]
			{
				new MySqlParameter
				{
					ParameterName = "@MStatus",
					Value = (object)(status.HasValue ? status.Value : 0)
				},
				new MySqlParameter
				{
					ParameterName = "@StartYearPeriod",
					Value = (object)startYearPeriod
				},
				new MySqlParameter
				{
					ParameterName = "@EndYearPeriod",
					Value = (object)endYearPeriod
				},
				new MySqlParameter
				{
					ParameterName = "@MTypeID",
					Value = (object)typeID
				},
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			return ModelInfoManager.GetDataModelBySql<GLPeriodTransferModel>(ctx, sql, source.ToArray());
		}

		public string GetTransferTypeNameByID(MContext ctx, int type)
		{
			string result = string.Empty;
			switch (type)
			{
			case 0:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CarryForwardCostOfSale", "Carry forward the cost of sales");
				break;
			case 1:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccrualPayroll", "Accrual payroll");
				break;
			case 2:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccrualDepreciations", "Accrual depreciation");
				break;
			case 3:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AmortizationPrepaidExpense", "Amortize of deferred expenses");
				break;
			case 4:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccrualTax", "Accrual tax");
				break;
			case 5:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CarryForwardOutOfVAT", "Carry forward Unpaid VAT");
				break;
			case 9:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "FinalTransfer", "期末调汇");
				break;
			case 6:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccrualIncomeTax", "Accrual income tax");
				break;
			case 7:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CarryForwardProfitAndLoss", "Carry forward profit & loss");
				break;
			case 8:
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CarryForwardUndistributedProfits", "Carry\u00a0forward\u00a0undistributed\u00a0profits");
				break;
			}
			return result;
		}

		public int GetLastTranferPeriod(MContext ctx, int tranferType)
		{
			string sql = "SELECT max(MYear*100+MPeriod) as yearperiod FROM t_gl_periodtransfer where MTransferTypeID=@TranferType and MOrgID=@MOrgID and MIsDelete=0";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@TranferType",
					Value = (object)tranferType
				},
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, cmdParms);
			int result = 0;
			if (single != null)
			{
				int.TryParse(single.ToString(), out result);
			}
			return result;
		}
	}
}
