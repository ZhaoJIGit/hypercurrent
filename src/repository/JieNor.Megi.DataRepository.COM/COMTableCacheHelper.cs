using JieNor.Megi.Common.Cache;
using JieNor.Megi.EntityModel.COM;
using JieNor.Megi.EntityModel.Context;
using System.Collections;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.COM
{
	public class COMTableCacheHelper
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

		private static List<List<MTableColumnModel>> GetTableColumnModelsFromService(MContext ctx, string tableName)
		{
			return new COMTableInfoRepository().GetTableColumnModels(tableName, null);
		}

		public static List<MTableColumnModel> GetTableColumnModels(MContext ctx, string tableName)
		{
			List<MTableColumnModel> list = new List<MTableColumnModel>();
			List<MTableColumnModel> list2 = new List<MTableColumnModel>();
			List<MTableColumnModel> list3 = new List<MTableColumnModel>();
			tableName = tableName.ToLower();
			string text = (tableName + "_l").ToLower();
			Hashtable tableColumnCache = TableColumnCache;
			if (tableColumnCache != null && tableColumnCache.ContainsKey(tableName) && tableColumnCache[tableName] != null)
			{
				list = (tableColumnCache[tableName] as List<MTableColumnModel>);
				list2 = (tableColumnCache.ContainsKey(text) ? (tableColumnCache[text] as List<MTableColumnModel>) : list2);
			}
			else
			{
				List<List<MTableColumnModel>> tableColumnModelsFromService = GetTableColumnModelsFromService(ctx, tableName);
				if (tableColumnModelsFromService != null && tableColumnModelsFromService.Count > 0)
				{
					list = tableColumnModelsFromService[0];
					if (!tableColumnCache.ContainsKey(tableName))
					{
						tableColumnCache.Add(tableName, list);
					}
					if (tableColumnModelsFromService.Count == 2)
					{
						list2 = tableColumnModelsFromService[1];
						tableColumnCache.Remove(text);
						tableColumnCache.Add(text.ToLower(), list2);
					}
				}
				TableColumnCache = tableColumnCache;
			}
			list3.AddRange(list);
			list3.AddRange(list2);
			return list3;
		}
	}
}
