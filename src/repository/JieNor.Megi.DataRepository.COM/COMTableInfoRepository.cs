using JieNor.Megi.Common.Cache;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.EntityModel.COM;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.DataRepository.COM
{
	public class COMTableInfoRepository
	{
		private static string TableColumnCacheName = "TableColumnCacheName";

		private static Hashtable TableColumnCache
		{
			get
			{
				object obj = HttpRuntimeCaches.Get(TableColumnCacheName);
				if (obj == null)
				{
					obj = new Hashtable();
					HttpRuntimeCaches.Set(TableColumnCacheName, new Hashtable());
				}
				return obj as Hashtable;
			}
			set
			{
				HttpRuntimeCaches.Set(TableColumnCacheName, value);
			}
		}

		public List<MTableColumnModel> GetTableColumnModel(string tableName, string connectionString = null)
		{
			List<MTableColumnModel> list = TableColumnCache[tableName] as List<MTableColumnModel>;
			if (list != null && TableColumnCache.ContainsKey(tableName.ToLower()))
			{
				return TableColumnCache[tableName.ToLower()] as List<MTableColumnModel>;
			}
			string sQLString = $"SELECT distinct distinct lower(column_name) as column_name, lower(column_type) as column_type FROM information_schema.COLUMNS WHERE lower(table_name) = '{tableName}'";
			DataSet dataSet = string.IsNullOrWhiteSpace(connectionString) ? DbHelperMySQL.Query(sQLString) : DbHelperMySQL.Query(connectionString, sQLString);
			if (dataSet != null && dataSet.Tables[0] != null)
			{
				return DataTableToColumnModelList(dataSet.Tables[0]);
			}
			return null;
		}

		private List<MTableColumnModel> DataTableToColumnModelList(DataTable dt)
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

		public List<List<MTableColumnModel>> GetTableColumnModels(string tableName, string connectionString = null)
		{
			List<List<MTableColumnModel>> result = new List<List<MTableColumnModel>>();
			string tableName2 = tableName + "_l";
			Hashtable tableColumnCache = TableColumnCache;
			List<MTableColumnModel> list = InitTableColumnModels(tableName, tableColumnCache, result, connectionString);
			if (list == null || list.Count == 0)
			{
				return result;
			}
			List<MTableColumnModel> list2 = InitTableColumnModels(tableName2, tableColumnCache, result, connectionString);
			return result;
		}

		private List<MTableColumnModel> InitTableColumnModels(string tableName, Hashtable tableColumnCache, List<List<MTableColumnModel>> result, string connectionString)
		{
			List<MTableColumnModel> list = null;
			if (tableColumnCache.ContainsKey(tableName.ToLower()))
			{
				list = (tableColumnCache[tableName.ToLower()] as List<MTableColumnModel>);
			}
			else
			{
				list = GetTableColumnModel(tableName.ToLower(), connectionString);
				tableColumnCache.Add(tableName.ToLower(), list);
				TableColumnCache = tableColumnCache;
			}
			if (list != null && list.Count > 0)
			{
				result.Add(list);
			}
			return list;
		}
	}
}
