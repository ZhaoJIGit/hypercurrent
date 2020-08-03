using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.DataRepository.RI
{
	public class RICategoryRepository : DataServiceT<RICategoryModel>
	{
		public List<RICategoryModel> GetCategoryList(MContext ctx, string localeID)
		{
			string str = "\n                        SELECT \n                            t1.MItemID,\n                            t1.MName,\n                            t1.MPermission,\n                            t1.MParentID,\n                            t1.MLinkUrl,\n                            t1.MIndex,\n                            t1.MFuncName,\n                            t1.MPassText,\n                            t1.MFailText,\n                            t1.MEnable,\n                            t3.MItemID as MSettingID,\n                            t3.MOrgID,\n                            t3.MRequirePass,\n                            t3.MEnable as MSettingEnable,\n                            t4.MItemID as MSettingParamID,\n                            t4.MParamName,\n                            t4.MParamvalue,\n                            t4.MCompareType,\n                            t4.MCompareValue,\n                            t4.MOperator\n                        FROM\n                            t_ri_category t1\n                                INNER JOIN\n                            t_ri_categorysetting t3 ON t1.MItemID = t3.MID\n                                AND t1.MIsDelete = t3.MIsDelete \n                                LEFT JOIN\n                            t_ri_categorysettingparam t4 ON t4.MID = t3.MID\n                                AND t4.MOrgID = t3.MOrgID\n                        WHERE\n                            t1.MEnable = 1 AND t1.MIsDelete = 0 \n                                AND t3.MOrgID = @MOrgID ";
			string userPermission = GetUserPermission(ctx);
			str = str + " AND locate('" + userPermission + "', t1.MPermission ) > 0 ";
			str += " order by t1.MParentID asc, t1.MIndex asc";
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(str, ctx.GetParameters((MySqlParameter)null));
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				return BindDataTable2List(ctx, dataSet.Tables[0].Rows, localeID);
			}
			return new List<RICategoryModel>();
		}

		private string GetUserPermission(MContext ctx)
		{
			string str = "";
			str += ((ctx.MOrgVersionID == 1) ? "1" : "0");
			return str + ((ctx.MFABeginDate >= ctx.MGLBeginDate) ? "1" : "0");
		}

		private List<RICategoryModel> BindDataTable2List(MContext ctx, DataRowCollection drs, string localeID)
		{
			List<RICategoryModel> list = new List<RICategoryModel>();
			for (int i = 0; i < drs.Count; i++)
			{
				DataRow row = drs[i];
				list.Add(new RICategoryModel
				{
					MItemID = row.MField("MItemID"),
					MPermission = row.MField("MPermission"),
					MParentID = row.MField("MParentID"),
					MLinkUrl = row.MField("MLinkUrl"),
					MIndex = row.MField<int>("MIndex"),
					MLocaleID = localeID,
					MPassText = row.MField("MPassText"),
					MFailText = row.MField("MFailText"),
					MFuncName = row.MField("MFuncName"),
					MPassTextString = COMMultiLangRepository.GetText(LangModule.GL, localeID, row.MField("MPassText")),
					MFailTextString = COMMultiLangRepository.GetText(LangModule.GL, localeID, row.MField("MFailText")),
					MEnable = row.MField<bool>("MEnable"),
					MName = COMMultiLangRepository.GetText(LangModule.GL, localeID, row.MField("MName")),
					MSetting = new RICategorySettingModel
					{
						MItemID = row.MField("MSettingID"),
						MOrgID = row.MField("MOrgID"),
						MID = row.MField("MItemID"),
						MEnable = row.MField<bool>("MSettingEnable"),
						MRequirePass = row.MField<bool>("MRequirePass"),
						MSettingParam = new RICategorySettingParamModel
						{
							MItemID = row.MField("MSettingParamID"),
							MParamName = row.MField("MParamName"),
							MID = row.MField("MSettingID"),
							MParamValue = row.MField("MParamValue"),
							MCompareType = row.MField("MCompareType"),
							MCompareValue = row.MField("MCompareValue"),
							MOperator = row.MField<int>("MOperator")
						}
					}
				});
			}
			return list;
		}
	}
}
