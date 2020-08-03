using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JieNor.Megi.Core.DataModel
{
	public abstract class DataServiceT<T> : IDataContract<T> where T : BaseModel
	{
		public virtual bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return ModelInfoManager.ExistsByKey<T>(ctx, pkID, includeDelete);
		}

		public virtual bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return ModelInfoManager.ExistsByFilter<T>(ctx, filter);
		}

		public virtual OperationResult InsertOrUpdate(MContext ctx, T modelData, string fields = null)
		{
			List<string> updateFields = GetUpdateFields(fields);
			return ModelInfoManager.InsertOrUpdate<T>(ctx, (BaseModel)modelData, updateFields);
		}

		public virtual OperationResult InsertOrUpdateModels(MContext ctx, List<T> modelData, string fields = "")
		{
			if (modelData == null || modelData.Count == 0)
			{
				throw new Exception("Error Paramenter");
			}
			List<string> updateFields = GetUpdateFields(fields);
			return ModelInfoManager.InsertOrUpdate<T>(ctx, modelData, updateFields);
		}

		private static List<string> GetUpdateFields(string fields)
		{
			List<string> list = null;
			if (!string.IsNullOrWhiteSpace(fields))
			{
				list = new List<string>();
				string[] array = fields.Split(',');
				string[] array2 = array;
				foreach (string text in array2)
				{
					list.Add(text.Trim());
				}
			}
			return list;
		}

		public virtual OperationResult Delete(MContext ctx, string pkID)
		{
			return ModelInfoManager.Delete<T>(ctx, pkID);
		}

		public virtual OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return ModelInfoManager.Delete<T>(ctx, pkID);
		}

		public virtual T GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return ModelInfoManager.GetDataEditModel<T>(ctx, pkID, includeDelete, true);
		}

		public virtual T GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			filter.Top = 1;
			List<T> dataModelList = ModelInfoManager.GetDataModelList<T>(ctx, filter, false, false);
			if (dataModelList.Count > 0)
			{
				return dataModelList[0];
			}
			return null;
		}

		public virtual List<T> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			if (filter == null)
			{
				filter = new SqlWhere();
			}
			if (!includeDelete)
			{
				filter.AddDeleteFilter("MIsDelete", SqlOperators.Equal, false);
			}
			return ModelInfoManager.GetDataModelList<T>(ctx, filter, false, false);
		}

		public virtual List<T> GetModelList(MContext ctx, SqlWhere filter, List<string> fields, bool includeDelete = false)
		{
			List<PropertyInfo> source = new List<PropertyInfo>(typeof(T).GetProperties());
			source = Enumerable.ToList<PropertyInfo>(Enumerable.Where<PropertyInfo>((IEnumerable<PropertyInfo>)source, (Func<PropertyInfo, bool>)((PropertyInfo a) => fields.Contains(a.Name))));
			List<MyPropertyInfo> list = new List<MyPropertyInfo>();
			foreach (PropertyInfo item2 in source)
			{
				bool isEncrypt = item2.IsDefined(typeof(ColumnEncryptAttribute), true);
				MyPropertyInfo item = new MyPropertyInfo(item2, isEncrypt);
				list.Add(item);
			}
			if (filter == null)
			{
				filter = new SqlWhere();
			}
			if (!includeDelete)
			{
				filter.AddDeleteFilter("MIsDelete", SqlOperators.Equal, false);
			}
			return ModelInfoManager.GetDataModelList<T>(ctx, filter, list, false, false);
		}

		public virtual DataGridJson<T> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return ModelInfoManager.GetPageDataModelList<T>(ctx, filter, includeDelete);
		}

		protected List<string> ConvertGridKeyIDs(string ids)
		{
			List<string> list = new List<string>();
			if (string.IsNullOrEmpty(ids) || ids.Trim().Length == 0)
			{
				return list;
			}
			string[] array = ids.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				list.Add(text.Replace("'", "").Trim());
			}
			return list;
		}

		protected string GetWhereInSql(string ids, ref MySqlParameter[] parameters, string parameterName = null)
		{
			ids = ids.Replace("'", "");
			List<string> list = ConvertGridKeyIDs(ids);
			if (list == null || list.Count == 0)
			{
				return string.Empty;
			}
			return GetWhereInSql(list, ref parameters, parameterName);
		}

		protected string GetWhereInSql(List<string> idList, ref MySqlParameter[] parameters, string parameterName = null)
		{
			MySqlParameter[] array = null;
			int num = 0;
			if (parameters == null)
			{
				array = new MySqlParameter[idList.Count];
			}
			else
			{
				array = new MySqlParameter[idList.Count + parameters.Length];
				MySqlParameter[] array2 = parameters;
				for (int i = 0; i < array2.Length; i++)
				{
					MySqlParameter mySqlParameter = array[num] = array2[i];
					num++;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			parameterName = (string.IsNullOrWhiteSpace(parameterName) ? "@MItemID" : parameterName);
			foreach (string id in idList)
			{
				string text = string.Format("{0}{1}", parameterName, num);
				stringBuilder.AppendFormat("{0},", text);
				array[num] = new MySqlParameter(text, id);
				num++;
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			parameters = array;
			return stringBuilder.ToString();
		}
	}
}
