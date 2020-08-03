// JieNor.Megi.DataRepository.API.APIDataRepository
using Fasterflect;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.API;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.COM;
using JieNor.Megi.EntityModel.Context;
using Microsoft.CSharp.RuntimeBinder;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public class APIDataRepository
{
	private readonly string LangFieldPlaceholder = "#_#lang_field{0}#_#";

	private static readonly List<Type> BaseDataWithLangType = new List<Type>
	{
		typeof(BDAccountEditModel),
		typeof(BDBankAccountEditModel),
		typeof(BDBankTypeModel),
		typeof(BDContactsInfoModel),
		typeof(BDContactsTypeModel),
		typeof(BDEmployeesModel),
		typeof(BDItemModel),
		typeof(BDTrackModel),
		typeof(REGTaxRateModel),
		typeof(REGCurrencyViewModel)
	};

	private KeyValuePair<string, List<MySqlParameter>> WhereFilter;

	private string CommonSelect
	{
		get;
		set;
	}

	private string IDSelectWithNoLimit => " select * from ( SELECT DISTINCT\n                            {0}\n                        FROM\n                            ( " + CommonSelect + " {5} ) t\n                        {1}) x  ";

	private string CountQuerySql => " select count(*) DataCount from ( SELECT $distinct\n                            {0}\n                        FROM\n                            ( " + CommonSelect + " {2} ) t\n                        {1}) x ";

	private string QuerySqlWithNoLimit => CommonSelect + " and t1.{0} in (" + IDSelectWithNoLimit + ") {5} order by {2}";

	private string QuerySqlWithLimit => CommonSelect + " and t1.{0} in ( select * from ( select * from ( SELECT DISTINCT\n                            {0}\n                        FROM\n                            (" + CommonSelect + "{5} ) t\n                        {1} order by {2}) x limit {3},{4} )y ){5} order by {2} ";

	public DataGridJson<T> Get<T>(MContext ctx, GetParam param, string commonSelect, List<string> mutliSelectList, bool fillBaseData = false, bool countDistinct = true, string defaultOrderBy = null) where T : BaseModel
	{
		string commonSQL = GetCommonSQL(ctx, commonSelect, mutliSelectList);
		commonSQL = (CommonSelect = HandleMultiLangQuery(ctx, commonSQL));
		int count = GetCount<T>(ctx, param, commonSQL, countDistinct);
		List<T> rows = (count == 0) ? new List<T>() : GetData<T>(ctx, param, commonSelect, fillBaseData, defaultOrderBy);
		return new DataGridJson<T>
		{
			rows = rows,
			total = count
		};
	}

	public DataGridJson<T> Get<T>(MContext ctx, GetParam param, string commonSelect, bool fillBaseData = false, bool countDistinct = true, string defaultOrderBy = null) where T : BaseModel
	{
		return Get<T>(ctx, param, commonSelect, new List<string>(), fillBaseData, countDistinct, defaultOrderBy);
	}

	public DataGridJson<T> Get<T>(MContext ctx, GetParam param, string commonSelect, string multiSelect, bool fillBaseData = false, bool countDistinct = true, string defaultOrderBy = null) where T : BaseModel
	{
		return Get<T>(ctx, param, commonSelect, new List<string>
		{
			multiSelect
		}, fillBaseData, countDistinct, defaultOrderBy);
	}

	public static List<CommandInfo> FillBaseData<T>(MContext ctx, List<T> list, bool create = false, List<APIFieldModel> fieldsInfo = null) where T : BaseModel
	{
		List<CommandInfo> list2 = new List<CommandInfo>();
		List<string> idList = new List<string>();
		fieldsInfo = (fieldsInfo ?? APIDataHelper.GetTypeFieldList(typeof(T), null, false));
		APIFieldModel entryField = fieldsInfo.FirstOrDefault((APIFieldModel x) => x.IsDetail);
		List<APIFieldModel> mainFields = (from x in fieldsInfo
										  where (entryField == null || !x.Name.StartsWith(entryField.Name)) && x.BaseDataAttr != null
										  select x).ToList();
		List<APIFieldModel> list3 = (from x in fieldsInfo
									 where entryField != null && x.Name.StartsWith(entryField.Name) && x.BaseDataAttr != null
									 select x).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			FillBaseData(ctx, list[i], fieldsInfo, mainFields, create, list2, idList, null);
			if (entryField != null && list3 != null && list3.Count > 0)
			{
				object propertyValue = list[i].GetPropertyValue(entryField.Name);
				if (propertyValue != null)
				{
					IEnumerable<object> enumerable = propertyValue as IEnumerable<object>;
					foreach (object item in enumerable)
					{
						FillBaseData(ctx, (BaseModel)item, fieldsInfo, list3, create, list2, idList, entryField.Name);
					}
				}
			}
		}
		return list2;
	}

	public static List<CommandInfo> FillBaseData<T>(MContext ctx, T model, bool create = false) where T : BaseModel
	{
		return FillBaseData(ctx, new List<T>
		{
			model
		}, false, null);
	}

	public static T MatchBaseData<T>(MContext ctx, T model, List<string> matchFields = null, bool create = false, List<string> idList = null, List<CommandInfo> cmdList = null) where T : BaseModel
	{
		string pKFieldName = model.PKFieldName;
		Type typeFromHandle = typeof(T);
		APIDataPool instance = APIDataPool.GetInstance(ctx);
		dynamic baseDataByModel = instance.GetBaseDataByModel(ctx, typeFromHandle);
		string pKFieldValue = model.PKFieldValue;
        dynamic obj = null;

		if (!string.IsNullOrWhiteSpace(pKFieldValue))
		{
			int num = 0;
			while (true)
			{
				if (num < baseDataByModel.Count)
				{
					dynamic obj2 = baseDataByModel[num];
					if (obj2 != null && obj2.PKFieldValue.ToString() == pKFieldValue)
					{
						obj = obj2;
						break;
					}

					num++;
					continue;
				}
				break;
			}
		}
		else if (string.IsNullOrWhiteSpace(pKFieldValue) && matchFields != null && matchFields.Count > 0)
		{
			for (int i = 0; i < matchFields.Count; i++)
			{
				if (obj != null)
				{
					break;
				}
				string text = matchFields[i];
				object propertyValue = model.GetPropertyValue(text);
				if (propertyValue != null && !string.IsNullOrWhiteSpace(propertyValue.ToString()))
				{
					int num2 = 0;
					while (true)
					{
						if (num2 < baseDataByModel.Count && obj == null)
						{
                            dynamic obj4 = baseDataByModel[num2]; 
							object arg6 = obj4.GetType().GetProperty(text).GetValue(obj4, null);
                            if(arg6 != null && !string.IsNullOrWhiteSpace(arg6.ToString()))
                            {
								if (ctx.MultiLang)
								{
                                    dynamic arg8 = JsonConvert.DeserializeObject(arg6.ToString());
                                    if(arg8 != null && arg8.Count > 0)
									{
										int num5 = 0;
										while (true)
										{
											if (num5 < arg8.Count)
											{
                                                dynamic arg11 = arg8[num5];
												if (arg11.LCID == ctx.MLCID && arg11.Value == propertyValue.ToString())
												{
													obj = obj4;
													break;
												}
												num5++;
												continue;
											}
											break;
										}
									}
								}
								else
								{
									if (arg6.ToString() == propertyValue.ToString())
									{
										obj = obj4;
									}
								}
							}
							i++;
							continue;
						}
						break;
					}
				}
			}
		}
		
        if (obj == null & create)
		{
			List<CommandInfo> insertOrUpdateCmd = GetInsertOrUpdateCmd(ctx, model);
			string item = model.GetPropertyValue(model.PKFieldName).ToString();
			if (idList == null)
			{
				idList = new List<string>();
			}
			if (!idList.Contains(item))
			{
				cmdList.AddRange((IEnumerable<CommandInfo>)insertOrUpdateCmd);
			}
		}
		return (obj == null) ? null : (obj as T);
	}

	private List<T> GetData<T>(MContext ctx, GetParam param, string commonSelect, bool fillBaseData = false, string defaultOrderBy = null) where T : BaseModel
	{
		string empty = string.Empty;
		List<APIFieldModel> typeFieldList = APIDataHelper.GetTypeFieldList(typeof(T), null, false);
		typeFieldList = (from x in typeFieldList
						 orderby x.Level descending
						 select x).ToList();
		List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
		string querySql = GetQuerySql<T>(ctx, param, list, typeFieldList, empty, defaultOrderBy);
		DataTable dt = new DynamicDbHelperMySQL(ctx).Query(querySql, list.ToArray()).Tables[0];
		List<T> list2 = DataTable2List<T>(ctx, dt, typeFieldList, Convert.ToBoolean(param.IncludeDetail));
		if (fillBaseData)
		{
			FillBaseData(ctx, list2, false, null);
		}
		return list2;
	}

	private int GetCount<T>(MContext ctx, GetParam param, string commonSelect, bool distinct = true) where T : BaseModel
	{
		string empty = string.Empty;
		List<APIFieldModel> typeFieldList = APIDataHelper.GetTypeFieldList(typeof(T), null, false);
		typeFieldList = (from x in typeFieldList
						 orderby x.Level descending
						 select x).ToList();
		List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
		string countQuerySql = GetCountQuerySql<T>(ctx, param, list, typeFieldList, empty, distinct);
		object single = new DynamicDbHelperMySQL(ctx).GetSingle(countQuerySql, list.ToArray());
		return int.Parse(single.ToString());
	}

	private string GetCommonSQL(MContext ctx, string sql, List<string> multSqls)
	{
		for (int i = 0; i < multSqls.Count; i++)
		{
			string text = multSqls[i];
			string oldValue = string.Format(LangFieldPlaceholder, i);
			if (!ctx.MultiLang || string.IsNullOrWhiteSpace(text))
			{
				sql = sql.Replace(oldValue, "");
			}
			else
			{
				string text2 = string.Empty;
				for (int j = 0; j < ctx.MActiveLocaleIDS.Count; j++)
				{
					text2 += string.Format(text, "_" + ctx.MActiveLocaleIDS[j]);
				}
				sql = sql.Replace(oldValue, text2);
			}
		}
		return sql;
	}

	private static string HandleMultiLangQuery(MContext ctx, string commonSelect)
	{
		Regex regex = new Regex("(?<=@_@)(\\S+)(?=@_@\\s+(\\S+?)\\s|,)");
		Match match;
		while ((match = regex.Match(commonSelect)).Success)
		{
			string key = match.Groups[1].ToString().ToLower();
			string text = match.Groups[2].ToString();
			string langSQL = APILangTableHelper.GetLangSQL(ctx, key);
			commonSelect = regex.Replace(commonSelect, langSQL, 1);
		}
		commonSelect = commonSelect.Replace("@_@", "");
		return commonSelect;
	}

	public string GetQuerySql<T>(MContext ctx, GetParam param, List<MySqlParameter> parameter, List<APIFieldModel> fieldsInfo, string groupString = null, string defaultOrderBy = null) where T : BaseModel
	{
		T val = Activator.CreateInstance(typeof(T)) as T;
		string pKFieldName = val.PKFieldName;
		KeyValuePair<string, List<MySqlParameter>> whereFilter = GetWhereFilter<T>(ctx, param, fieldsInfo);
		string text = string.IsNullOrWhiteSpace(defaultOrderBy) ? pKFieldName : defaultOrderBy;
		if (!string.IsNullOrWhiteSpace(param.OrderBy))
		{
			string orderBy = APIDataHelper.GetOrderBy(ctx, param.OrderBy, fieldsInfo);
			if (orderBy != null)
			{
				text = orderBy + "," + text;
			}
		}
		string result = APIDataHelper.Format((param.PageSize > 0) ? QuerySqlWithLimit : QuerySqlWithNoLimit, pKFieldName, string.IsNullOrWhiteSpace(whereFilter.Key) ? "" : (" where " + whereFilter.Key + " "), text, (param.PageIndex - 1) * param.PageSize, param.PageSize, groupString);
		parameter.AddRange((IEnumerable<MySqlParameter>)whereFilter.Value);
		return result;
	}

	private string GetCountQuerySql<T>(MContext ctx, GetParam param, List<MySqlParameter> parameter, List<APIFieldModel> fieldsInfo, string groupString = null, bool distinct = true) where T : BaseModel
	{
		T val = Activator.CreateInstance(typeof(T)) as T;
		string pKFieldName = val.PKFieldName;
		KeyValuePair<string, List<MySqlParameter>> whereFilter = GetWhereFilter<T>(ctx, param, fieldsInfo);
		string src = CountQuerySql.Replace("$distinct", distinct ? "distinct" : "");
		string result = APIDataHelper.Format(src, pKFieldName, string.IsNullOrWhiteSpace(whereFilter.Key) ? "" : (" where " + whereFilter.Key + " "), groupString);
		parameter.AddRange((IEnumerable<MySqlParameter>)whereFilter.Value);
		return result;
	}

	private KeyValuePair<string, List<MySqlParameter>> GetWhereFilter<T>(MContext ctx, GetParam param, List<APIFieldModel> fieldsInfo)
	{
		KeyValuePair<string, List<MySqlParameter>> keyValuePair = default(KeyValuePair<string, List<MySqlParameter>>);
		if (!keyValuePair.Equals(WhereFilter))
		{
			return WhereFilter;
		}
		KeyValuePair<string, List<MySqlParameter>> filter = APIDataHelper.ParseWhere<T>(ctx, param, fieldsInfo);
		return WhereFilter = GetIncludeDisabledFilter(filter, param, fieldsInfo);
	}

	private static KeyValuePair<string, List<MySqlParameter>> GetIncludeDisabledFilter(KeyValuePair<string, List<MySqlParameter>> filter, GetParam param, List<APIFieldModel> fieldsInfo)
	{
		APIFieldModel aPIFieldModel = fieldsInfo.FirstOrDefault((APIFieldModel x) => x.Name == "MIsActive");
		if (aPIFieldModel != null && !param.IncludeDisabled && (string.IsNullOrWhiteSpace(filter.Key) || !filter.Key.Contains(aPIFieldModel.Prop.Name)))
		{
			string text = $" {aPIFieldModel.Prop.Name} = @API_MIsActive_999";
			string key = string.IsNullOrWhiteSpace(filter.Key) ? text : (filter.Key + " and " + text);
			List<MySqlParameter> value = filter.Value;
			value.Add(new MySqlParameter
			{
				ParameterName = "@API_MIsActive_999",
				Value = (object)true
			});
			filter = new KeyValuePair<string, List<MySqlParameter>>(key, value);
		}
		return filter;
	}

	public static List<T> DataTable2List<T>(MContext ctx, DataTable dt, List<APIFieldModel> fieldsInfo, bool includeDetail) where T : BaseModel
	{
		List<T> list = new List<T>();
		if (dt == null || dt.Rows.Count == 0)
		{
			return list;
		}
		APIFieldModel aPIFieldModel = fieldsInfo.FirstOrDefault((APIFieldModel x) => x.IsDetail);
		T val = Activator.CreateInstance(typeof(T)) as T;
		string pKFieldName = val.PKFieldName;
		val = null;
		List<string> columns = GetColumns(dt.Columns);
		if (aPIFieldModel != null)
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow row = dt.Rows[i];
				string b = row.MField(pKFieldName);
				if (val == null || val.PKFieldValue != b)
				{
					val = (Activator.CreateInstance(typeof(T)) as T);
					list.Add(val);
					val = list.Last();
				}
				FillModel(ctx, row, columns, fieldsInfo, includeDetail, val);
			}
		}
		else
		{
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				DataRow row2 = dt.Rows[j];
				T val2 = Activator.CreateInstance(typeof(T)) as T;
				FillModel(ctx, row2, columns, fieldsInfo, includeDetail, val2);
				list.Add(val2);
			}
		}
		return list;
	}

	private static List<string> GetColumns(DataColumnCollection columns)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < columns.Count; i++)
		{
			list.Add(columns[i].ToString());
		}
		return list;
	}

	private static void FillModel<T>(MContext ctx, DataRow row, List<string> columns, List<APIFieldModel> fieldsInfo, bool includeDetail, T model = null) where T : BaseModel
	{
		model = (model ?? (Activator.CreateInstance(typeof(T)) as T));
		APIFieldModel entryField = fieldsInfo.FirstOrDefault((APIFieldModel x) => x.IsDetail && APIDataHelper.IsListProperty(x.Prop));
		List<string> list = (entryField == null) ? new List<string>() : (from x in columns
																		 where x.StartsWith(entryField.Name)
																		 select x).ToList();
		List<string> list2 = (entryField == null) ? columns : (from x in columns
															   where !x.StartsWith(entryField.Name)
															   select x).ToList();
		for (int i = 0; i < list2.Count; i++)
		{
			string value = row.IsNull(list2[i]) ? null : ((row[list2[i]] == null) ? "" : row[list2[i]].ToString());
			if (!string.IsNullOrWhiteSpace(value))
			{
				SetFieldValue(ctx, model, row.IsNull(list2[i]) ? null : row[list2[i]], list2[i], fieldsInfo, "");
			}
		}
		if (list.Count > 0 & includeDetail)
		{
			object propertyValue = model.GetPropertyValue(entryField.Name);
			if (propertyValue == null)
			{
				propertyValue = Activator.CreateInstance(entryField.Prop.PropertyType);
				model.SetPropertyValue(entryField.Name, propertyValue);
				propertyValue = model.GetPropertyValue(entryField.Name);
			}
			string listPropertyGenericType = APIDataHelper.GetListPropertyGenericType(entryField.Prop);
			bool flag = false;
			object obj = Activator.CreateInstance(Type.GetType(listPropertyGenericType));
			string prefix = entryField.Name + "_";
			for (int j = 0; j < list.Count; j++)
			{
				flag |= !row.IsNull(list[j]);
				string value2 = row.IsNull(list[j]) ? null : ((row[list[j]] == null) ? "" : row[list[j]].ToString());
				if (!string.IsNullOrWhiteSpace(value2))
				{
					SetFieldValue(ctx, obj, value2, list[j], fieldsInfo, prefix);
				}
			}
			if (flag)
			{
				object[] parameters = new object[1]
				{
					obj
				};
				entryField.Prop.PropertyType.GetMethod("Add").Invoke(propertyValue, parameters);
			}
		}
	}

	private static void FillBaseData<T>(MContext ctx, T model, List<APIFieldModel> fieldsInfo, List<APIFieldModel> mainFields, bool create = false, List<CommandInfo> cmdList = null, List<string> idList = null, string parentName = null)
	{
		if (model != null && mainFields != null && mainFields.Count != 0)
		{
			for (int i = 0; i < mainFields.Count; i++)
			{
				object fieldOwner = GetFieldOwner(model, mainFields[i], parentName);
				if (fieldOwner != null)
				{
					FillBaseData(ctx, fieldOwner, mainFields[i], create, cmdList, idList);
				}
			}
		}
	}

	private static object GetFieldOwner(object model, APIFieldModel field, string parentName = null)
	{
		if (field == null || model == null)
		{
			return null;
		}
		object obj = model;
		List<string> list = (from x in field.Name.Split('_')
							 where x != parentName
							 select x).ToList();
		int num = 0;
		object result;
		while (true)
		{
			if (num < list.Count && num != list.Count - 1)
			{
				obj = obj.GetPropertyValue(list[num]);
				if (obj == null)
				{
					result = null;
					break;
				}
				num++;
				continue;
			}
			return obj;
		}
		return result;
	}

	private static void FillBaseData(MContext ctx, object owner, APIFieldModel fieldInfo, bool create = false, List<CommandInfo> cmdsList = null, List<string> idList = null)
	{
		string iDField = fieldInfo.BaseDataAttr.IDField;
		List<string> matchFields = fieldInfo.BaseDataAttr.MatchFields;
		BaseModel baseModel = owner.GetPropertyValue(fieldInfo.Prop.Name) as BaseModel;
		dynamic baseDataByModel;
        object obj2;
		int num2;

		if (baseModel != null)
		{
			owner.SetPropertyValue(fieldInfo.Prop.Name, baseModel);
			baseModel = (owner.GetPropertyValue(fieldInfo.Prop.Name) as BaseModel);
			Type propertyType = fieldInfo.Prop.PropertyType;
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			baseDataByModel = instance.GetBaseDataByModel(ctx, propertyType);
			object obj = baseModel.GetPropertyValue(iDField);
			obj2 = null;
			if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
			{
				int num = 0;
				while (true)
				{
					if (num < baseDataByModel.Count && obj2 == null)
					{
                        dynamic obj4 = baseDataByModel[num];
						if (obj4 != null && obj4.PKFieldValue.ToString() == obj.ToString())
						{
							obj2 = obj4;
						}
						num++;
						continue;
					}
					break;
				}
				if (obj2 == null)
				{
					baseModel.SetPropertyValue(baseModel.PKFieldName, string.Empty);
					obj = null;
				}
			}
			if ((obj == null || string.IsNullOrEmpty(obj.ToString())) && matchFields != null)
			{
				num2 = ((matchFields.Count > 0) ? 1 : 0);
				goto IL_054b;
			}
			num2 = 0;
			goto IL_054b;
		}
		return;
	IL_054b:
		if (num2 != 0)
		{
			for (int i = 0; i < matchFields.Count; i++)
			{
				string text = matchFields[i];
				object propertyValue = baseModel.GetPropertyValue(text);
				if (propertyValue != null && !string.IsNullOrWhiteSpace(propertyValue.ToString()))
				{
					int num3 = 0;
					while (true)
					{
						if (num3 < baseDataByModel.Count && obj2 == null)
						{
                            dynamic obj5 = baseDataByModel[num3];
							object arg7 = obj5.GetType().GetProperty(text).GetValue(obj5, null);
                            if (arg7 != null && !string.IsNullOrWhiteSpace(arg7.ToString()))
							{
								if (ctx.MultiLang)
								{
                                    dynamic arg9 = JsonConvert.DeserializeObject(arg7.ToString());
                                    if(arg9 != null && arg9.Count > 0)
									{
										int num6 = 0;
										while (true)
										{
											if (num6 < arg9.Count)
											{
                                                dynamic arg12 = arg9[num6]; 
												if (arg12.LCID == ctx.MLCID && arg12.Value == propertyValue.ToString())
												{
													obj2 = obj5;
													break;
												}
												num6++;
												continue;
											}
											break;
										}
									}
								}
								else
								{
									if (arg7.ToString() == propertyValue.ToString())
									{
										obj2 = obj5;
									}
								}
							}
							num3++;
							continue;
						}
						break;
					}
				}
			}
		}
		if (obj2 != null)
		{
			if (obj2.GetType().GetProperty("MultiLanguage") != (PropertyInfo)null)
			{
				obj2.SetPropertyValue("MultiLanguage", baseModel?.MultiLanguage);
			}
			owner.SetPropertyValue(fieldInfo.Prop.Name, obj2);
		}
		else if (create)
		{
			List<CommandInfo> insertOrUpdateCmd = GetInsertOrUpdateCmd(ctx, baseModel);
			string text2 = Convert.ToString(baseModel.GetPropertyValue(baseModel.PKFieldName));
			if (idList == null)
			{
				idList = new List<string>();
			}
			if (!idList.Contains(text2) && !string.IsNullOrEmpty(text2))
			{
				cmdsList.AddRange(insertOrUpdateCmd);
			}
		}
	}

	private static List<CommandInfo> GetInsertOrUpdateCmd(MContext ctx, BaseModel data)
	{
		if (data == null)
		{
			return new List<CommandInfo>();
		}
		Type type = data.GetType();
		if (type == typeof(BDAccountEditModel))
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BDAccountEditModel>(ctx, data, null, true);
		}
		if (type == typeof(BDOrganisationModel))
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BDOrganisationModel>(ctx, data, null, true);
		}
		if (type == typeof(BDBankAccountEditModel))
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BDBankAccountEditModel>(ctx, data, null, true);
		}
		if (type == typeof(BDBankTypeModel))
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BDBankTypeModel>(ctx, data, null, true);
		}
		if (type == typeof(BDContactsInfoModel))
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BDContactsInfoModel>(ctx, data, null, true);
		}
		if (type == typeof(BDContactsTypeModel))
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BDContactsTypeModel>(ctx, data, null, true);
		}
		if (type == typeof(BDEmployeesModel))
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BDEmployeesModel>(ctx, data, null, true);
		}
		if (type == typeof(BDItemModel))
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BDItemModel>(ctx, data, null, true);
		}
		if (type == typeof(BDTrackModel))
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BDTrackModel>(ctx, data, null, true);
		}
		if (type == typeof(REGCurrencyModel))
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<REGCurrencyModel>(ctx, data, null, true);
		}
		return new List<CommandInfo>();
	}

	private static void SetFieldValue(MContext ctx, object model, object value, string column, List<APIFieldModel> fieldsInfo, string prefix = "")
	{
		APIFieldModel aPIFieldModel = fieldsInfo.FirstOrDefault((APIFieldModel x) => x.Name.EqualsIgnoreCase(column));
		if (aPIFieldModel != null)
		{
			List<string> list = ((!string.IsNullOrWhiteSpace(prefix)) ? aPIFieldModel.Name.Replace(prefix, "") : aPIFieldModel.Name).Split('_').ToList();
			string currentPath = "";
			object obj = model;
			for (int i = 0; i < list.Count; i++)
			{
				if (i == list.Count - 1)
				{
					obj.SetPropertyValue(list[i], APIDataHelper.GetValue(aPIFieldModel.Prop, value));
				}
				else
				{
					currentPath = ((i == 0) ? list[0] : (currentPath + "_" + list[i]));
					APIFieldModel info = fieldsInfo.FirstOrDefault((APIFieldModel x) => x.Name == prefix + currentPath);
					object fieldValue = GetFieldValue(obj, list[i], info);
					if (fieldsInfo.Exists((APIFieldModel x) => x.Name.IndexOf(prefix + currentPath + "_") >= 0))
					{
						obj.SetPropertyValue(list[i], fieldValue);
						obj = fieldValue;
					}
				}
			}
		}
	}

	private static object GetFieldValue(object obj, string fieldPath, APIFieldModel info)
	{
		if (fieldPath == "")
		{
			return obj;
		}
		object propertyValue = obj.GetPropertyValue(fieldPath);
		propertyValue = (propertyValue ?? Activator.CreateInstance(info.Prop.PropertyType));
		if (APIDataHelper.IsListProperty(info.Prop))
		{
			if (int.Parse(propertyValue.GetPropertyValue("Count").ToString()) == 0)
			{
				string listPropertyGenericType = APIDataHelper.GetListPropertyGenericType(info.Prop);
				object obj2 = Activator.CreateInstance(Type.GetType(listPropertyGenericType));
				object[] parameters = new object[1]
				{
					obj2
				};
				info.Prop.PropertyType.GetMethod("Add").Invoke(propertyValue, parameters);
			}
			return null;
		}
		return propertyValue;
	}

	public static List<T> GetBaseDataInDatabase<T>(MContext ctx, string commonSql = null) where T : BaseModel
	{
		List<T> list = new List<T>();
		Type typeFromHandle = typeof(T);
		List<APIFieldModel> typeFieldList = APIDataHelper.GetTypeFieldList(typeof(T), null, true);
		COMTableInfoRepository cOMTableInfoRepository = new COMTableInfoRepository();
		BaseModel baseModel = Activator.CreateInstance(typeFromHandle) as BaseModel;
		string tableName = baseModel.TableName;
		string langTableName = tableName + "_l";
		string pKFieldName = baseModel.PKFieldName;
		List<List<MTableColumnModel>> tableColumnModels = cOMTableInfoRepository.GetTableColumnModels(tableName, null);
		string baseDataCommonSql = GetBaseDataCommonSql<T>(ctx, tableName, langTableName, pKFieldName, tableColumnModels[0], (tableColumnModels.Count == 1) ? new List<MTableColumnModel>() : tableColumnModels[1], typeFieldList, null);
		DataTable dt = new DynamicDbHelperMySQL(ctx).Query(baseDataCommonSql, ctx.GetParameters((MySqlParameter)null)).Tables[0];
		return DataTable2BaseDataModel<T>(ctx, dt, tableName, langTableName, pKFieldName, tableColumnModels[0], (tableColumnModels.Count == 1) ? new List<MTableColumnModel>() : tableColumnModels[1], typeFieldList, null);
	}

	public static string GetBaseDataCommonSql<T>(MContext ctx, string mainTableName, string langTableName, string mainTablePKField, List<MTableColumnModel> mainTableColumnModels, List<MTableColumnModel> langTableColumnModels, List<APIFieldModel> fieldsInfo = null, string connectionString = null) where T : BaseModel
	{
		string empty = string.Empty;
		Type typeFromHandle = typeof(T);
		List<APIFieldModel> list = (from x in fieldsInfo
									where x.ApiAttr == null || x.ApiAttr.MemberType != ApiMemberType.MultiLang
									select x).ToList();
		List<APIFieldModel> list2 = (from x in fieldsInfo
									 where x.ApiAttr != null && x.ApiAttr.MemberType == ApiMemberType.MultiLang
									 select x).ToList();
		bool flag = typeof(T) != typeof(BASCurrencyModel);
		List<string> list3 = new List<string>();
		empty = ((!(typeof(T) == typeof(BDBankTypeModel))) ? ((!(typeof(T) == typeof(GLCheckGroupModel))) ? ((!(typeof(T) == typeof(BDContactsTypeModel))) ? ((!(typeof(T) == typeof(BDTrackModel))) ? ((!(typeof(T) == typeof(BDContactsInfoModel))) ? ((langTableColumnModels.Count == 0) ? (" select {0} from {1} t1  where " + (flag ? " t1.MOrgID = @MOrgID and " : "") + " t1.MIsDelete = 0 {4} order by t1.{3} ") : (" select {0} from {1} t1 inner join {2} t2 on t1.{3} = t2.MParentID and " + (flag ? " t1.MOrgID = t2.MOrgID and " : "") + " t1.MIsDelete = t2.MIsDelete  where " + (flag ? " t1.MOrgID = @MOrgID and " : "") + " t1.MIsDelete = 0 {4} order by t1.{3}, t2.MLocaleID ")) : ((langTableColumnModels.Count == 0) ? " select {0} from {1} t1  where  t1.MOrgID = @MOrgID and t1.MIsDelete = 0 {4} order by t1.{3} " : " select {0} from {1} t1\r\n                            inner join {2} t2 on t1.{3} = t2.MParentID and t1.MOrgID = t2.MOrgID and  t1.MIsDelete = t2.MIsDelete \r\n                        where t1.MOrgID = @MOrgID and t1.MIsDelete = 0 {4} order by t1.{3}, t2.MLocaleID ")) : ((langTableColumnModels.Count == 0) ? " select {0} from {1} t1  where t1.MOrgID = @MOrgID and t1.MIsDelete = 0 {4} order by t1.{3} " : " select {0} from {1} t1 inner join {2} t2 on t1.{3} = t2.MParentID and t1.MOrgID = t2.MOrgID and t1.MIsDelete = t2.MIsDelete where t1.MOrgID = @MOrgID and t1.MIsDelete = 0 {4} order by t1.MCreateDate, t2.MLocaleID ")) : ((langTableColumnModels.Count == 0) ? " select {0} from {1} t1  where (t1.MOrgID = @MOrgID OR t1.MOrgID = '0') and t1.MIsDelete = 0 {4} order by t1.{3} " : " select {0} from {1} t1 inner join {2} t2 on t1.{3} = t2.MParentID and ( ifnull(t1.MOrgID,'') = ifnull(t2.MOrgID,'')) and t1.MIsDelete = t2.MIsDelete  where (t1.MOrgID = @MOrgID OR t1.MOrgID = '0' ) and t1.MIsDelete = 0 {4} order by t1.{3}, t2.MLocaleID ")) : " select {0} from {1} t1  where t1.MIsDelete = 0 {4} order by t1.{3} ") : ((langTableColumnModels.Count == 0) ? " select {0} from {1} t1  where (t1.MOrgID = @MOrgID OR ifnull(t1.MOrgID,'') = '') and t1.MIsDelete = 0 {4} order by t1.{3} " : " select {0} from {1} t1 inner join {2} t2 on t1.{3} = t2.MParentID and ( ifnull(t1.MOrgID,'') = ifnull(t2.MOrgID,'')) and t1.MIsDelete = t2.MIsDelete  where (t1.MOrgID = @MOrgID OR ifnull(t1.MOrgID,'') = '') and t1.MIsDelete = 0 {4} order by t1.{3}, t2.MLocaleID "));
		if (typeof(T) == typeof(BDTrackEntryModel))
		{
			list3.Add("t1.MItemID as MTrackingCategoryID");
		}
		for (int i = 0; i < list.Count; i++)
		{
			APIFieldModel field = list[i];
			if (mainTableColumnModels.Exists((Predicate<MTableColumnModel>)((MTableColumnModel x) => x.Name.ToLower() == field.Name.ToLower())))
			{
				string item = field.IsEncrypt ? APIDataHelper.Format("convert(AES_DECRYPT(t1.{0},'{1}') using utf8) as " + field.Prop.Name, field.Prop.Name, "JieNor-001") : ("t1." + field.Prop.Name);
				list3.Add(item);
			}
		}
		if (!list3.Exists((Predicate<string>)((string x) => x.ToLower() == mainTablePKField.ToLower())))
		{
			list3.Insert(0, "t1." + mainTablePKField);
		}
		if (langTableColumnModels.Count > 0)
		{
			list3.Add("t2.MLocaleID");
			for (int j = 0; j < list2.Count; j++)
			{
				APIFieldModel field2 = list2[j];
				if (langTableColumnModels.Exists((Predicate<MTableColumnModel>)((MTableColumnModel x) => x.Name.ToLower() == field2.Name.ToLower())))
				{
					string item2 = field2.IsEncrypt ? APIDataHelper.Format("convert(AES_DECRYPT(t2.{0},'{1}') using utf8) as " + field2.Prop.Name, field2.Prop.Name, "JieNor-001") : ("t2." + field2.Prop.Name);
					list3.Add(item2);
				}
			}
		}
		return APIDataHelper.Format(empty, string.Join(",", list3), mainTableName, langTableName, mainTablePKField, (list2.Count == 0 || langTableColumnModels.Count == 0) ? "" : (ctx.MultiLang ? GetLocaleIDString(ctx) : " and t2.MLocaleID = @MLCID "));
	}

	private static string GetLocaleIDString(MContext ctx)
	{
		if (ctx.MActiveLocaleIDS == null || !ctx.MActiveLocaleIDS.Any())
		{
			return "";
		}
		string text = " in (";
		for (int i = 0; i < ctx.MActiveLocaleIDS.Count; i++)
		{
			text = text + "'" + ctx.MActiveLocaleIDS[i] + "',";
		}
		text = text.TrimEnd(',') + ")";
		return " and t2.MLocaleID " + text;
	}

	public static List<T> DataTable2BaseDataModel<T>(MContext ctx, DataTable dt, string mainTableName, string langTableName, string mainTablePkField, List<MTableColumnModel> mainTableColumnModels, List<MTableColumnModel> langTableColumnModels, List<APIFieldModel> fieldsInfo = null, string connectionString = null) where T : BaseModel
	{
		List<T> list = new List<T>();
		List<string> columns = GetColumns(dt.Columns);
		List<KeyValuePair<string, List<APILangModel>>> list2 = new List<KeyValuePair<string, List<APILangModel>>>();
		T val = null;
		bool flag = columns.Any((string item) => item.EqualsIgnoreCase("MLocaleID"));
		APIFieldModel field;
		for (int i = 0; i < dt.Rows.Count; i++)
		{
			DataRow dataRow = dt.Rows[i];
			string text = dataRow[mainTablePkField].ToString();
			if (val == null || val.TryGetPropertyValue(mainTablePkField).ToString() != text)
			{
				if (val != null && val.TryGetPropertyValue(mainTablePkField).ToString() != text)
				{
					foreach (KeyValuePair<string, List<APILangModel>> item3 in list2)
					{
						IsoDateTimeConverter isoDateTimeConverter = new IsoDateTimeConverter
						{
							DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
						};
						string value = JsonConvert.SerializeObject(item3.Value, Formatting.None, isoDateTimeConverter);
						val.SetPropertyValue(item3.Key, value);
					}
					list2 = new List<KeyValuePair<string, List<APILangModel>>>();
				}
				val = (Activator.CreateInstance(typeof(T)) as T);
				val.SetPropertyValue(mainTablePkField, text);
				list.Add(val);
			}
			string lCID = flag ? dataRow["MLocaleID"].ToString() : null;
			foreach (string item4 in columns)
			{
				if (!item4.EqualsIgnoreCase("MLocaleID"))
				{
					field = fieldsInfo?.FirstOrDefault((APIFieldModel x) => x.Name.EqualsIgnoreCase(item4));
					if (field != null)
					{
						if (field.ApiAttr != null && field.ApiAttr.MemberType == ApiMemberType.MultiLang && ctx.MultiLang)
						{
							KeyValuePair<string, List<APILangModel>> item2;
							if (list2.Exists((Predicate<KeyValuePair<string, List<APILangModel>>>)((KeyValuePair<string, List<APILangModel>> x) => x.Key.EqualsIgnoreCase(field.Name))))
							{
								item2 = list2.FirstOrDefault((KeyValuePair<string, List<APILangModel>> x) => x.Key.EqualsIgnoreCase(field.Name));
							}
							else
							{
								item2 = new KeyValuePair<string, List<APILangModel>>(field.Name, new List<APILangModel>());
								list2.Add(item2);
							}
							item2.Value.Add(new APILangModel
							{
								LCID = lCID,
								Value = ((dataRow[item4] != null) ? dataRow[item4].ToString() : null)
							});
						}
						else
						{
							SetFieldValue(ctx, val, dataRow[item4], item4, fieldsInfo, "");
						}
					}
				}
			}
		}
		if (val == null || !list2.Any())
		{
			return list;
		}
		foreach (KeyValuePair<string, List<APILangModel>> item5 in list2)
		{
			IsoDateTimeConverter isoDateTimeConverter2 = new IsoDateTimeConverter
			{
				DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
			};
			string value2 = JsonConvert.SerializeObject(item5.Value, Formatting.None, isoDateTimeConverter2);
			val.SetPropertyValue(item5.Key, value2);
		}
		return list;
	}

	public static List<APIAccountDimensionModel> ToApiDimension(MContext ctx, GLCheckGroupValueModel value, List<BDContactsInfoModel> contacts, List<BDEmployeesModel> employees, List<BDExpenseItemModel> expenseItems, List<BDItemModel> items, List<PAPayItemModel> payItems, List<PAPayItemGroupModel> payItemGroups, List<BDTrackModel> trackList)
	{
		List<APIAccountDimensionModel> list = new List<APIAccountDimensionModel>();
		if (value == null)
		{
			return list;
		}
		if (!string.IsNullOrWhiteSpace(value.MContactID))
		{
			BDContactsInfoModel bDContactsInfoModel = contacts.FirstOrDefault((BDContactsInfoModel x) => x.MItemID == value.MContactID);
			list.Add(new APIAccountDimensionModel(DimensionType.MasterData, DimensionName.Contact, bDContactsInfoModel?.MItemID, bDContactsInfoModel?.MName, bDContactsInfoModel != null, bDContactsInfoModel != null && !bDContactsInfoModel.MIsActive));
		}
		if (!string.IsNullOrWhiteSpace(value.MEmployeeID))
		{
			BDEmployeesModel bDEmployeesModel = employees.FirstOrDefault((BDEmployeesModel x) => x.MItemID == value.MEmployeeID);
			string fullName = GetFullName(bDEmployeesModel?.MFirstName, bDEmployeesModel?.MLastName, ctx);
			list.Add(new APIAccountDimensionModel(DimensionType.MasterData, DimensionName.Employee, bDEmployeesModel?.MItemID, fullName, bDEmployeesModel != null, bDEmployeesModel != null && !bDEmployeesModel.MIsActive));
		}
		if (!string.IsNullOrWhiteSpace(value.MMerItemID))
		{
			BDItemModel bDItemModel = items.FirstOrDefault((BDItemModel x) => x.MItemID == value.MMerItemID);
			list.Add(new APIAccountDimensionModel(DimensionType.MasterData, DimensionName.Item, bDItemModel?.MItemID, bDItemModel?.MNumber, bDItemModel != null, bDItemModel != null && !bDItemModel.MIsActive));
		}
		if (!string.IsNullOrWhiteSpace(value.MExpItemID))
		{
			BDExpenseItemModel bDExpenseItemModel = expenseItems.FirstOrDefault((BDExpenseItemModel x) => x.MItemID == value.MExpItemID);
			list.Add(new APIAccountDimensionModel(DimensionType.MasterData, DimensionName.ExpenseItems, bDExpenseItemModel?.MItemID, bDExpenseItemModel?.MName, bDExpenseItemModel != null, bDExpenseItemModel != null && !bDExpenseItemModel.MIsActive));
		}
		if (!string.IsNullOrWhiteSpace(value.MPaItemID))
		{
			PAPayItemGroupModel pAPayItemGroupModel = payItemGroups.FirstOrDefault((PAPayItemGroupModel x) => x.MItemID == value.MPaItemID);
			if (pAPayItemGroupModel == null)
			{
				PAPayItemModel pAPayItemModel = payItems.FirstOrDefault((PAPayItemModel a) => a.MItemID == value.MPaItemID);
				list.Add(new APIAccountDimensionModel(DimensionType.MasterData, DimensionName.PayrollItems, pAPayItemModel?.MItemID, pAPayItemModel?.MName, pAPayItemModel != null, pAPayItemModel != null && !pAPayItemModel.MIsActive));
			}
			else
			{
				list.Add(new APIAccountDimensionModel(DimensionType.MasterData, DimensionName.PayrollItems, pAPayItemGroupModel.MItemID, pAPayItemGroupModel?.MName, true, !pAPayItemGroupModel.MIsActive));
			}
		}
		if (!string.IsNullOrWhiteSpace(value.MTrackItem1) && trackList != null && trackList.Count > 0)
		{
			BDTrackEntryModel bDTrackEntryModel = (trackList.Count < 1) ? null : trackList[0]?.MEntryList?.FirstOrDefault((BDTrackEntryModel x) => x.MEntryID == value.MTrackItem1);
			list.Add(new APIAccountDimensionModel(DimensionType.Tracking, trackList[0]?.MItemID, bDTrackEntryModel?.MEntryID, bDTrackEntryModel?.MName, bDTrackEntryModel != null, bDTrackEntryModel != null && !bDTrackEntryModel.MIsActive));
		}
		if (!string.IsNullOrWhiteSpace(value.MTrackItem2) && trackList != null && trackList.Count > 1)
		{
			BDTrackEntryModel bDTrackEntryModel2 = (trackList.Count < 2) ? null : trackList[1]?.MEntryList?.FirstOrDefault((BDTrackEntryModel x) => x.MEntryID == value.MTrackItem2);
			list.Add(new APIAccountDimensionModel(DimensionType.Tracking, trackList[1]?.MItemID, bDTrackEntryModel2?.MEntryID, bDTrackEntryModel2?.MName, bDTrackEntryModel2 != null, bDTrackEntryModel2 != null && !bDTrackEntryModel2.MIsActive));
		}
		if (!string.IsNullOrWhiteSpace(value.MTrackItem3) && trackList != null && trackList.Count > 2)
		{
			BDTrackEntryModel bDTrackEntryModel3 = (trackList.Count < 3) ? null : trackList[2]?.MEntryList?.FirstOrDefault((BDTrackEntryModel x) => x.MEntryID == value.MTrackItem3);
			list.Add(new APIAccountDimensionModel(DimensionType.Tracking, trackList[2]?.MItemID, bDTrackEntryModel3?.MEntryID, bDTrackEntryModel3?.MName, bDTrackEntryModel3 != null, bDTrackEntryModel3 != null && !bDTrackEntryModel3.MIsActive));
		}
		if (!string.IsNullOrWhiteSpace(value.MTrackItem4) && trackList != null && trackList.Count > 3)
		{
			BDTrackEntryModel bDTrackEntryModel4 = (trackList.Count < 4) ? null : trackList[3]?.MEntryList?.FirstOrDefault((BDTrackEntryModel x) => x.MEntryID == value.MTrackItem4);
			list.Add(new APIAccountDimensionModel(DimensionType.Tracking, trackList[3]?.MItemID, bDTrackEntryModel4?.MEntryID, bDTrackEntryModel4?.MName, bDTrackEntryModel4 != null, bDTrackEntryModel4 != null && !bDTrackEntryModel4.MIsActive));
		}
		if (!string.IsNullOrWhiteSpace(value.MTrackItem5) && trackList != null && trackList.Count > 4)
		{
			BDTrackEntryModel bDTrackEntryModel5 = (trackList.Count < 5) ? null : trackList[4]?.MEntryList?.FirstOrDefault((BDTrackEntryModel x) => x.MEntryID == value.MTrackItem5);
			list.Add(new APIAccountDimensionModel(DimensionType.Tracking, trackList[4]?.MItemID, bDTrackEntryModel5?.MEntryID, bDTrackEntryModel5?.MName, bDTrackEntryModel5 != null, bDTrackEntryModel5 != null && !bDTrackEntryModel5.MIsActive));
		}
		return list;
	}

	public static GLCheckGroupValueModel ToLocalDimension(MContext ctx, List<APIAccountDimensionModel> values, List<BDContactsInfoModel> contactDataPool, List<BDExpenseItemModel> expenseItemDataPool, List<BDItemModel> itemDataPool, List<BDEmployeesModel> employeeDataPool, List<PAPayItemModel> payItemDataPool, List<PAPayItemGroupModel> payItemGroupsDataPool, List<BDTrackModel> trackDataPool)
	{
		GLCheckGroupValueModel gLCheckGroupValueModel = new GLCheckGroupValueModel();
		gLCheckGroupValueModel.MOrgID = ctx.MOrgID;
		if (values == null)
		{
			return gLCheckGroupValueModel;
		}
		for (int i = 0; i < values.Count; i++)
		{
			APIAccountDimensionModel v = values[i];
			if (!string.IsNullOrEmpty(v.MType) && !string.IsNullOrEmpty(v.MName) && !string.IsNullOrEmpty(v.MOptionID))
			{
				if (v.MType.EqualsIgnoreCase("MASTERDATA"))
				{
					switch (v.MName.ToLower())
					{
						case "contact":
							{
								BDContactsInfoModel bDContactsInfoModel = contactDataPool.FirstOrDefault((BDContactsInfoModel a) => a.MItemID == v.MOptionID);
								if (bDContactsInfoModel?.MIsActive ?? false)
								{
									gLCheckGroupValueModel.MContactID = (string.IsNullOrEmpty(gLCheckGroupValueModel.MContactID) ? bDContactsInfoModel.MItemID : gLCheckGroupValueModel.MContactID);
								}
								break;
							}
						case "employee":
							{
								BDEmployeesModel bDEmployeesModel = employeeDataPool.FirstOrDefault((BDEmployeesModel a) => a.MItemID == v.MOptionID);
								if (bDEmployeesModel?.MIsActive ?? false)
								{
									gLCheckGroupValueModel.MEmployeeID = (string.IsNullOrEmpty(gLCheckGroupValueModel.MEmployeeID) ? bDEmployeesModel.MItemID : gLCheckGroupValueModel.MEmployeeID);
								}
								break;
							}
						case "item":
							{
								BDItemModel bDItemModel = itemDataPool.FirstOrDefault((BDItemModel a) => a.MItemID == v.MOptionID);
								if (bDItemModel?.MIsActive ?? false)
								{
									gLCheckGroupValueModel.MMerItemID = (string.IsNullOrEmpty(gLCheckGroupValueModel.MMerItemID) ? bDItemModel?.MItemID : gLCheckGroupValueModel.MMerItemID);
								}
								break;
							}
						case "expenseitems":
							{
								BDExpenseItemModel bDExpenseItemModel = expenseItemDataPool.FirstOrDefault((BDExpenseItemModel a) => a.MItemID == v.MOptionID);
								if (bDExpenseItemModel != null && bDExpenseItemModel.MIsActive && !expenseItemDataPool.Exists((BDExpenseItemModel a) => a.MParentItemID == v.MOptionID))
								{
									gLCheckGroupValueModel.MExpItemID = (string.IsNullOrEmpty(gLCheckGroupValueModel.MExpItemID) ? bDExpenseItemModel?.MItemID : gLCheckGroupValueModel.MExpItemID);
								}
								break;
							}
						case "payrollitems":
							{
								PAPayItemGroupModel pAPayItemGroupModel = payItemGroupsDataPool.FirstOrDefault((PAPayItemGroupModel x) => x.MItemID == v.MOptionID);
								if (pAPayItemGroupModel != null && pAPayItemGroupModel.MIsActive && !payItemDataPool.Exists((PAPayItemModel a) => a.MGroupID == v.MOptionID))
								{
									gLCheckGroupValueModel.MPaItemID = (string.IsNullOrEmpty(gLCheckGroupValueModel.MPaItemID) ? pAPayItemGroupModel.MItemID : gLCheckGroupValueModel.MPaItemID);
								}
								else
								{
									PAPayItemModel pAPayItemModel = payItemDataPool.FirstOrDefault((PAPayItemModel a) => a.MItemID == v.MOptionID);
									if (pAPayItemModel?.MIsActive ?? false)
									{
										gLCheckGroupValueModel.MPaItemID = (string.IsNullOrEmpty(gLCheckGroupValueModel.MPaItemID) ? pAPayItemModel?.MItemID : gLCheckGroupValueModel.MPaItemID);
									}
								}
								break;
							}
					}
				}
				else if (v.MType.EqualsIgnoreCase("TRACKING"))
				{
					for (int j = 0; j < trackDataPool.Count; j++)
					{
						BDTrackModel bDTrackModel = trackDataPool[j];
						if (bDTrackModel.MItemID == v.MName && bDTrackModel.MEntryList != null && bDTrackModel.MEntryList.Any() && bDTrackModel.MEntryList.Exists((BDTrackEntryModel a) => a.MEntryID == v.MOptionID && a.MIsActive))
						{
							gLCheckGroupValueModel.SetPropertyValue("MTrackItem" + (j + 1), v.MOptionID);
						}
					}
				}
			}
		}
		return gLCheckGroupValueModel;
	}

	public static string GetFullName(string firstName, string lastName, MContext ctx)
	{
		if (ctx != null && ctx.MultiLang)
		{
			List<APILangModel> firstNameList = JsonConvert.DeserializeObject<List<APILangModel>>(firstName);
			List<APILangModel> lastNameList = JsonConvert.DeserializeObject<List<APILangModel>>(lastName);
			List<APILangModel> value = (from lcid in ctx.MActiveLocaleIDS
										let currentFirstName = firstNameList.FirstOrDefault((APILangModel a) => a.LCID == lcid)?.Value
										let currentLastName = lastNameList.FirstOrDefault((APILangModel a) => a.LCID == lcid)?.Value
										let currentFullName = GlobalFormat.GetUserName(currentFirstName, currentLastName, ctx)
										select new APILangModel
										{
											LCID = lcid,
											Value = currentFullName
										}).ToList();
			return JsonConvert.SerializeObject(value);
		}
		return GlobalFormat.GetUserName(firstName, lastName, ctx);
	}
}
