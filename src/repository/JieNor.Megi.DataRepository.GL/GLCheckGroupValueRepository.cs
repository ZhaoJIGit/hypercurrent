using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLCheckGroupValueRepository : DataServiceT<GLCheckGroupValueModel>
	{
		public List<GLCheckGroupValueModel> GetCheckGroupValueList(MContext ctx)
		{
			string format = "\r\n                SELECT \r\n                t5.MItemID,\r\n                t5.MOrgID,\r\n                t5.MContactID,\r\n                t5.MEmployeeID,\r\n                t5.MMerItemID,\r\n                t5.MExpItemID,\r\n                t5.MPaItemID,\r\n                t5.MTrackItem1,\r\n                t5.MTrackItem2,\r\n                t5.MTrackItem3,\r\n                t5.MTrackItem4,\r\n                t5.MTrackItem5,\r\n                CONVERT( AES_DECRYPT(t6.MName, '{0}') USING UTF8) AS MContactName,\r\n                F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeName,\r\n                t8.MDesc AS MMerItemName,\r\n                t9.MName AS MExpItemName,\r\n                t10.MName AS MPaItemName,\r\n                t10_0.MGroupID AS MPaItemGroupID,\r\n                t10_1.MName as MPaItemGroupName,\r\n                t11.MItemID AS MTrackItem1GroupID,\r\n                t12.MName AS MTrackItem1GroupName,\r\n                t13.MName AS MTrackItem1Name,\r\n                t14.MItemID AS MTrackItem2GroupID,\r\n                t15.MName AS MTrackItem2GroupName,\r\n                t16.MName AS MTrackItem2Name,\r\n                t17.MItemID AS MTrackItem3GroupID,\r\n                t18.MName AS MTrackItem3GroupName,\r\n                t19.MName AS MTrackItem3Name,\r\n                t20.MItemID AS MTrackItem4GroupID,\r\n                t21.MName AS MTrackItem4GroupName,\r\n                t22.MName AS MTrackItem4Name,\r\n                t23.MItemID AS MTrackItem5GroupID,\r\n                t24.MName AS MTrackItem5GroupName,\r\n                t25.MName AS MTrackItem5Name\r\n            FROM\r\n                t_gl_checkgroupvalue t5 \r\n                    LEFT JOIN\r\n                t_bd_contacts_l t6 ON t6.MParentID = t5.MContactID\r\n                    AND t6.MOrgID = t5.MOrgId\r\n                    AND t6.MIsDelete = t5.MIsDelete\r\n                    AND t6.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_employees_l t7 ON t7.MParentID = t5.MEmployeeID\r\n                    AND t7.MOrgID = t5.MOrgId\r\n                    AND t7.MIsDelete = t5.MIsDelete\r\n                    AND t7.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_item_l t8 ON t8.MParentID = t5.MMerItemID\r\n                    AND t8.MOrgID = t5.MOrgId\r\n                    AND t8.MIsDelete = t5.MIsDelete\r\n                    AND t8.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_expenseitem_l t9 ON t9.MParentID = t5.MExpItemID\r\n                    AND t9.MOrgID = t5.MOrgId\r\n                    AND t9.MIsDelete = t5.MIsDelete\r\n                    AND t9.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_pa_payitem_l t10 ON t10.MParentID = t5.MPaItemID\r\n                    AND t10.MOrgID = t5.MOrgId\r\n                    AND t10.MIsDelete = t5.MIsDelete\r\n                    AND t10.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_pa_payitem t10_0 ON t10_0.MItemID = t5.MPaItemID\r\n                    AND t10_0.MOrgID = t5.MOrgId\r\n                    AND t10_0.MIsDelete = t5.MIsDelete\r\n                    LEFT JOIN\r\n                t_pa_payitemgroup_l t10_1 ON t10_1.MParentID = t5.MPaItemID\r\n                    AND t10_1.MOrgID = t5.MOrgId\r\n                    AND t10_1.MIsDelete = t5.MIsDelete\r\n                    AND t10_1.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t11 ON t11.MEntryID = t5.MTrackItem1\r\n                    AND t11.MOrgID = t5.MOrgID\r\n                    AND t11.MIsDelete = t5.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t12 ON t12.MParentID = t11.MItemID\r\n                    AND t12.MOrgID = t5.MOrgID\r\n                    AND t12.MIsDelete = t5.MIsDelete\r\n                    AND t12.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t13 ON t13.MParentID = t5.MTrackItem1\r\n                    AND t13.MOrgID = t5.MOrgId\r\n                    AND t13.MIsDelete = t5.MIsDelete\r\n                    AND t13.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t14 ON t14.MEntryID = t5.MTrackItem2\r\n                    AND t14.MOrgID = t5.MOrgID\r\n                    AND t14.MIsDelete = t5.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t15 ON t15.MParentID = t14.MItemID\r\n                    AND t15.MOrgID = t5.MOrgID\r\n                    AND t15.MIsDelete = t5.MIsDelete\r\n                    AND t15.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t16 ON t16.MParentID = t5.MTrackItem2\r\n                    AND t16.MOrgID = t5.MOrgId\r\n                    AND t16.MIsDelete = t5.MIsDelete\r\n                    AND t16.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t17 ON t17.MEntryID = t5.MTrackItem3\r\n                    AND t17.MOrgID = t5.MOrgID\r\n                    AND t17.MIsDelete = t5.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t18 ON t18.MParentID = t17.MItemID\r\n                    AND t18.MOrgID = t5.MOrgID\r\n                    AND t18.MIsDelete = t5.MIsDelete\r\n                    AND t18.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t19 ON t19.MParentID = t5.MTrackItem3\r\n                    AND t19.MOrgID = t5.MOrgId\r\n                    AND t19.MIsDelete = t5.MIsDelete\r\n                    AND t19.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t20 ON t20.MEntryID = t5.MTrackItem4\r\n                    AND t20.MOrgID = t5.MOrgID\r\n                    AND t20.MIsDelete = t5.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t21 ON t21.MParentID = t20.MItemID\r\n                    AND t21.MOrgID = t5.MOrgID\r\n                    AND t21.MIsDelete = t5.MIsDelete\r\n                    AND t21.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t22 ON t22.MParentID = t5.MTrackItem4\r\n                    AND t22.MOrgID = t5.MOrgID\r\n                    AND t22.MIsDelete = t5.MIsDelete\r\n                    AND t22.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t23 ON t23.MEntryID = t5.MTrackItem5\r\n                    AND t23.MOrgID = t5.MOrgID\r\n                    AND t23.MIsDelete = t5.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t24 ON t24.MParentID = t23.MItemID\r\n                    AND t24.MOrgID = t5.MOrgID\r\n                    AND t24.MIsDelete = t5.MIsDelete\r\n                    AND t24.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t25 ON t25.MParentID = t5.MTrackItem5\r\n                    AND t25.MOrgID = t5.MOrgId\r\n                    AND t25.MIsDelete = t5.MIsDelete\r\n                    AND t25.MLocaleID = @MLocaleID\r\n                WHERE\r\n                    t5.MOrgID = @MOrgID\r\n                        AND t5.MIsDelete = 0\r\n\r\n            ";
			format = string.Format(format, "JieNor-001");
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(format, ctx.GetParameters((MySqlParameter)null));
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			return BindDataset2List(ctx, dataSet.Tables[0]);
		}

		public List<GLCheckGroupValueModel> GetBaseCheckGroupValueList(MContext ctx)
		{
			List<GLCheckGroupValueModel> result = new List<GLCheckGroupValueModel>();
			string sql = "SELECT \r\n                                    MItemID\r\n                                FROM\r\n                                    t_gl_checkgroupvalue\r\n                                WHERE\r\n                                    MOrgID = @MOrgID AND MIsDelete = 0 ";
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, parameters);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				result = ModelInfoManager.DataTableToList<GLCheckGroupValueModel>(dataSet);
			}
			return result;
		}

		public List<GLCheckGroupValueModel> BindDataset2List(MContext ctx, DataTable dt)
		{
			List<GLCheckGroupValueModel> list = new List<GLCheckGroupValueModel>();
			DataRow dataRow = null;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				dataRow = dt.Rows[i];
				list.Add(new GLCheckGroupValueModel
				{
					MOrgID = ConvertString(dataRow, "MOrgID", null),
					MItemID = ConvertString(dataRow, "MItemID", null),
					MContactID = ConvertString(dataRow, "MContactID", null),
					MContactName = ConvertString(dataRow, "MContactName", null),
					MEmployeeID = ConvertString(dataRow, "MEmployeeID", null),
					MEmployeeName = ConvertString(dataRow, "MEmployeeName", null),
					MMerItemID = ConvertString(dataRow, "MMerItemID", null),
					MMerItemName = ConvertString(dataRow, "MMerItemName", null),
					MExpItemID = ConvertString(dataRow, "MExpItemID", null),
					MExpItemName = ConvertString(dataRow, "MExpItemName", null),
					MPaItemID = ConvertString(dataRow, "MPaItemID", null),
					MPaItemGroupID = ConvertString(dataRow, "MPaItemGroupID", null),
					MPaItemGroupName = ConvertString(dataRow, "MPaItemGroupName", null),
					MPaItemName = ConvertString(dataRow, "MPaItemName", null),
					MTrackItem1 = ConvertString(dataRow, "MTrackItem1", null),
					MTrackItem2 = ConvertString(dataRow, "MTrackItem2", null),
					MTrackItem3 = ConvertString(dataRow, "MTrackItem3", null),
					MTrackItem4 = ConvertString(dataRow, "MTrackItem4", null),
					MTrackItem5 = ConvertString(dataRow, "MTrackItem5", null),
					MTrackItem1GroupID = ConvertString(dataRow, "MTrackItem1GroupID", null),
					MTrackItem2GroupID = ConvertString(dataRow, "MTrackItem2GroupID", null),
					MTrackItem3GroupID = ConvertString(dataRow, "MTrackItem3GroupID", null),
					MTrackItem4GroupID = ConvertString(dataRow, "MTrackItem4GroupID", null),
					MTrackItem5GroupID = ConvertString(dataRow, "MTrackItem5GroupID", null),
					MTrackItem1Name = ConvertString(dataRow, "MTrackItem1Name", null),
					MTrackItem2Name = ConvertString(dataRow, "MTrackItem2Name", null),
					MTrackItem3Name = ConvertString(dataRow, "MTrackItem3Name", null),
					MTrackItem4Name = ConvertString(dataRow, "MTrackItem4Name", null),
					MTrackItem5Name = ConvertString(dataRow, "MTrackItem5Name", null),
					MTrackItem1GroupName = ConvertString(dataRow, "MTrackItem1GroupName", null),
					MTrackItem2GroupName = ConvertString(dataRow, "MTrackItem2GroupName", null),
					MTrackItem3GroupName = ConvertString(dataRow, "MTrackItem3GroupName", null),
					MTrackItem4GroupName = ConvertString(dataRow, "MTrackItem4GroupName", null),
					MTrackItem5GroupName = ConvertString(dataRow, "MTrackItem5GroupName", null)
				});
			}
			return list;
		}

		private string ConvertString(DataRow row, string name, string defaultValue = null)
		{
			return row.IsNull(name) ? defaultValue : row.MField<string>(name);
		}
	}
}
