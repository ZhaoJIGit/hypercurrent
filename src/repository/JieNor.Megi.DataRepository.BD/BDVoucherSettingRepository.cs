using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDVoucherSettingRepository : DataServiceT<BDVoucherSettingModel>
	{
		public List<BDVoucherSettingCategoryModel> GetVoucherSettingCategoryList(MContext ctx)
		{
			List<BDVoucherSettingCategoryModel> list = new List<BDVoucherSettingCategoryModel>();
			string text = "SELECT \n                                t1.MItemID,\n                                t1.MColumnID,\n                                t1.MIndex,\n                                t1.MModuleID,\n                                t1.MDC,\n                                t1.MTypeID,\n                                t1.MIsCheckBox,\n                                t1.MEnable,\n                                t1.MControlStatus,\n                                t2.MOrgID,\n                                t2.MItemID AS MSettingID,\n                                t2.MStatus\n                            FROM\n                                t_bd_vouchersettingcategory t1\n                                    INNER JOIN\n                                t_bd_vouchersetting t2 ON t1.MItemID = t2.MID\n                                    AND t1.MIsDelete = t2.MIsDelete\n                            WHERE\n                                t1.MIsDelete = 0 \n                                    AND t1.MEnable = 1\n                                    AND t2.MOrgID = @MOrgID";
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			text = ((ctx.MOrgVersionID != 1) ? (text + " and t1.MModuleID in( " + 0 + "," + 1 + "," + 2 + "," + 6 + "," + 7 + "," + 8 + ")") : (text + " and t1.MModuleID in( " + 6 + "," + 7 + "," + 5 + "," + 4 + "," + 2 + ")"));
			text += " order by t1.MItemID asc, t1.MIndex asc";
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(text, ctx.GetParameters((MySqlParameter)null));
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				list = BindDataTable2List(ctx, dataSet.Tables[0].Rows);
			}
			if (list == null || !list.Any())
			{
				InitVoucherSetting(ctx);
				return GetVoucherSettingCategoryList(ctx);
			}
			return list;
		}

		private List<BDVoucherSettingCategoryModel> BindDataTable2List(MContext ctx, DataRowCollection drs)
		{
			List<BDVoucherSettingCategoryModel> list = new List<BDVoucherSettingCategoryModel>();
			BDVoucherSettingCategoryModel bDVoucherSettingCategoryModel = null;
			for (int i = 0; i < drs.Count; i++)
			{
				DataRow row = drs[i];
				int num = row.MField<int>("MModuleID");
				if (bDVoucherSettingCategoryModel == null || bDVoucherSettingCategoryModel.MModuleID != num)
				{
					bDVoucherSettingCategoryModel = new BDVoucherSettingCategoryModel
					{
						MItemID = row.MField("MItemID"),
						MModuleID = row.MField<int>("MModuleID"),
						MColumnID = row.MField<int>("MColumnID"),
						MIndex = row.MField<int>("MIndex"),
						MDC = row.MField<int>("MDC"),
						MTypeID = row.MField<int>("MTypeID"),
						MIsCheckBox = row.MField<bool>("MIsCheckBox"),
						MEnable = row.MField<bool>("MEnable"),
						MControlStatus = row.MField<int>("MControlStatus"),
						MSettingList = new List<BDVoucherSettingModel>()
					};
					list.Add(bDVoucherSettingCategoryModel);
				}
				bDVoucherSettingCategoryModel.MSettingList.Add(new BDVoucherSettingModel
				{
					MItemID = row.MField("MSettingID"),
					MOrgID = row.MField("MOrgID"),
					MID = row.MField("MItemID"),
					MColumnID = row.MField<int>("MColumnID"),
					MIndex = row.MField<int>("MIndex"),
					MDC = row.MField<int>("MDC"),
					MTypeID = row.MField<int>("MTypeID"),
					MIsCheckBox = row.MField<bool>("MIsCheckBox"),
					MEnable = row.MField<bool>("MEnable"),
					MControlStatus = row.MField<int>("MControlStatus"),
					MModuleID = num,
					MStatus = row.MField<bool>("MStatus")
				});
			}
			return list;
		}

		public OperationResult InitVoucherSetting(MContext ctx)
		{
			CommandInfo obj = new CommandInfo
			{
				CommandText = " update t_bd_vouchersetting set MIsDelete = 1 where MOrgID = @MOrgID and MIsDelete = 0 "
			};
			DbParameter[] array = obj.Parameters = ctx.GetParameters((MySqlParameter)null);
			CommandInfo item = obj;
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = "INSERT INTO t_bd_vouchersetting (MItemID, MOrgID, MID, MStatus, MIsDelete, MCreatorID , MCreateDate,MModifierID,MModifyDate) \r\n                           SELECT REPLACE(UUID(), '-', ''), @MOrgID, MItemID, MDefaultStatus, MIsDelete ,@MUserID, @MCreateDate, @MUserID, @MCreateDate from T_BD_VoucherSettingCategory "
			};
			array = (obj2.Parameters = ctx.GetParameters("@MCreateDate", ctx.DateNow).ToArray());
			CommandInfo item2 = obj2;
			List<CommandInfo> cmdList = new List<CommandInfo>
			{
				item,
				item2
			};
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(cmdList) > 0)
			};
		}
	}
}
