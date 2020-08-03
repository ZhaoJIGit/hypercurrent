using JieNor.Megi.EntityModel.COM;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.Core.DBUtility
{
	public static class TableColumnHelper
	{
		public static List<MTableColumnModel> GetTableColumnModels(string tableName)
		{
			string sQLString = string.Format("SELECT distinct distinct lower(column_name) as column_name, lower(column_type) as column_type FROM information_schema.COLUMNS WHERE lower(table_name) = '{0}'", tableName);
			DataSet dataSet = DbHelperMySQL.Query(sQLString);
			if (dataSet != null && dataSet.Tables[0] != null)
			{
				return DataTableToColumnModelList(dataSet.Tables[0]);
			}
			return null;
		}

		private static List<MTableColumnModel> DataTableToColumnModelList(DataTable dt)
		{
			List<MTableColumnModel> list = new List<MTableColumnModel>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				MTableColumnModel mTableColumnModel = new MTableColumnModel();
				mTableColumnModel.Name = dt.Rows[i]["column_name"].ToString();
				string text = dt.Rows[i]["column_type"].ToString().Replace(")", "");
				string[] array = text.Split('(');
				mTableColumnModel.Type = array[0];
				if (array.Length == 2)
				{
					string[] array2 = array[1].Split(',');
					if (array2.Length == 2)
					{
						mTableColumnModel.MaxLength = int.Parse(array2[0]) + 1;
						mTableColumnModel.DecimalMaxLength = int.Parse(array2[0]) - int.Parse(array2[1]);
					}
					else
					{
						mTableColumnModel.MaxLength = int.Parse(array[1]);
					}
				}
				list.Add(mTableColumnModel);
			}
			return list;
		}
	}
}
