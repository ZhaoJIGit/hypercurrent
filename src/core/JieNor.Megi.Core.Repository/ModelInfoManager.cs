using Fasterflect;
using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace JieNor.Megi.Core.Repository
{
	public static class ModelInfoManager
	{
		private static object objLock = new object();

		private static ConcurrentDictionary<Type, MyPropertyInfo[]> modelInfoCache = new ConcurrentDictionary<Type, MyPropertyInfo[]>();

		private static ConcurrentDictionary<Type, ModelToMySqlInfor> modelSqlCache = new ConcurrentDictionary<Type, ModelToMySqlInfor>();

		private static Hashtable encryptCache = Hashtable.Synchronized(new Hashtable());

		public static OperationResult ArchiveFlag<T>(MContext ctx, string pkID) where T : BaseModel
		{
			return ArchiveFlag<T>(ctx, new List<string>
			{
				pkID
			});
		}

		public static OperationResult ArchiveFlag<T>(MContext ctx, List<string> pkID) where T : BaseModel
		{
			List<CommandInfo> archiveFlagCmd = GetArchiveFlagCmd<T>(ctx, pkID);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(archiveFlagCmd);
			OperationResult operationResult = new OperationResult();
			if (num > 0)
			{
				operationResult.Message = "Success";
				operationResult.SuccessModelID = pkID;
			}
			else
			{
				operationResult.Message = "Fail";
				operationResult.FailModelID = pkID;
			}
			return operationResult;
		}

		public static List<CommandInfo> GetArchiveFlagCmd<T>(MContext ctx, string pkID) where T : BaseModel
		{
			return GetArchiveFlagCmd<T>(ctx, pkID, false);
		}

		public static List<CommandInfo> GetArchiveFlagCmd<T>(MContext ctx, string pkID, bool isActive) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			List<CommandInfo> list = new List<CommandInfo>();
			Type typeFromHandle = typeof(T);
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[typeFromHandle];
			MySqlParameter[] para = new MySqlParameter[5]
			{
				new MySqlParameter("@PKID", pkID),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MIsActive", isActive),
				new MySqlParameter("@ModifyDate", ctx.DateNow),
				new MySqlParameter("@ModifierID", ctx.MUserID)
			};
			if (!string.IsNullOrWhiteSpace(modelToMySqlInfor.MainArchiveFlagSql))
			{
				list.Add(new CommandInfo(modelToMySqlInfor.MainArchiveFlagSql, para));
			}
			return list;
		}

		public static List<CommandInfo> GetArchiveFlagCmd<T>(MContext ctx, List<string> pkIDS) where T : BaseModel
		{
			return GetArchiveFlagCmd<T>(ctx, pkIDS, false);
		}

		public static List<CommandInfo> GetArchiveFlagCmd<T>(MContext ctx, List<string> pkIDS, bool isActive) where T : BaseModel
		{
			if (pkIDS == null || pkIDS.Count == 0)
			{
				throw new Exception("Error Paramenter");
			}
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (string pkID in pkIDS)
			{
				list.AddRange((IEnumerable<CommandInfo>)GetArchiveFlagCmd<T>(ctx, pkID, isActive));
			}
			return list;
		}

		public static string GetAutoNumber<T>(MContext ctx, string pkId, string preText, string oldNumber = "") where T : BaseModel
		{
			int minLength = 4;
			BaseModel baseModel = Activator.CreateInstance(typeof(T)) as BaseModel;
			if (!(baseModel is BDModel))
			{
				goto IL_0027;
			}
			goto IL_0027;
			IL_0027:
			string modelNumber = GetModelNumber(ctx, pkId, oldNumber, baseModel);
			if (!string.IsNullOrWhiteSpace(modelNumber))
			{
				return modelNumber;
			}
			return GetUserDefineNumber(ctx, preText, baseModel.TableName, minLength);
		}

		private static string GetUserDefineNumber(MContext ctx, string preText, string tableName, int minLength)
		{
			string arg = preText + "[0-9]{" + minLength + "}";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select MNumber AS MNo from {1} ", minLength, tableName);
			stringBuilder.AppendFormat("WHERE MOrgID=@MOrgID AND MNumber REGEXP '{0}' ", arg);
			stringBuilder.Append("ORDER BY MNumber DESC limit 1");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MType", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(stringBuilder.ToString(), array);
			if (single != null && single != DBNull.Value)
			{
				string value = single.ToString().Replace(preText, "").ToString();
				return string.Format("{0}{1}", preText, (Convert.ToInt32(value) + 1).ToString().PadLeft(minLength, '0'));
			}
			return string.Format("{0}{1}", preText, "1".PadLeft(minLength, '0'));
		}

		private static string GetModelNumber(MContext ctx, string pkId, string oldNumber, BaseModel model)
		{
			if (string.IsNullOrWhiteSpace(pkId))
			{
				return string.Empty;
			}
			if (string.IsNullOrWhiteSpace(oldNumber))
			{
				return string.Empty;
			}
			if (IsNumberExists(ctx, pkId, oldNumber, model))
			{
				return string.Empty;
			}
			return oldNumber;
		}

		private static bool IsNumberExists(MContext ctx, string pkId, string oldNumber, BaseModel model)
		{
			string text = string.Format("select COUNT(*) from {0} WHERE MOrgID=@MOrgID AND {1} <> @MID AND MNumber=@MNumber", model.TableName, model.PKFieldName);
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MNumber", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = pkId;
			array[2].Value = oldNumber;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(text.ToString(), array);
			return Convert.ToInt32(single) > 0 && true;
		}

		public static bool IsLangColumnValueExists<T>(MContext ctx, string columnName, List<MultiLanguageFieldList> langList, string keyId, string parentColumn, string parentValue, bool isEncypt = false) where T : BaseModel, new()
		{
			MultiLanguageFieldList multiLanguageFieldList = (from t in langList
			where t.MFieldName == columnName
			select t).FirstOrDefault();
			if (multiLanguageFieldList == null || multiLanguageFieldList.MMultiLanguageField == null || multiLanguageFieldList.MMultiLanguageField.Count == 0)
			{
				return false;
			}
			T val = new T();
			BuildEditModelInfo(ctx, typeof(T));
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select 1 from {0} a INNER JOIN {0}_L b ON a.{1}=b.MParentID WHERE 1=1 ", val.TableName, val.PKFieldName);
			if (ExistsColunm(val, "MOrgID"))
			{
				stringBuilder.Append(" AND a.MOrgID = @OrgID ");
			}
			if (ExistsColunm(val, "MIsDelete"))
			{
				stringBuilder.Append(" AND a.MIsDelete = 0 AND b.MIsDelete = 0");
			}
			if (!string.IsNullOrEmpty(keyId))
			{
				stringBuilder.AppendFormat(" AND {0} <> @KeyID ", val.PKFieldName);
			}
			if (!string.IsNullOrWhiteSpace(parentColumn))
			{
				stringBuilder.AppendFormat(" and {0} = @ParentValue ", parentColumn);
			}
			List<MySqlParameter> list = new List<MySqlParameter>();
			list.Add(new MySqlParameter("@OrgID", ctx.MOrgID));
			list.Add(new MySqlParameter("@KeyID", keyId));
			list.Add(new MySqlParameter("@ParentValue", parentValue));
			stringBuilder.Append(" AND (");
			int num = 1;
			foreach (MultiLanguageField item in multiLanguageFieldList.MMultiLanguageField)
			{
				if (!string.IsNullOrEmpty(item.MValue))
				{
					if (isEncypt)
					{
						stringBuilder.AppendFormat(" (TRIM(CONVERT(AES_DECRYPT(b.{0},'{1}') USING utf8))=@ColumnValue{2} and b.MLocaleID = @MLCID{2})   OR", columnName, "JieNor-001", num);
					}
					else
					{
						stringBuilder.AppendFormat(" (TRIM(b.{0})=@ColumnValue{1} and b.MLocaleID = @MLCID{1}) OR", columnName, num);
					}
					list.Add(new MySqlParameter("@MLCID" + num, item.MLocaleID));
					list.Add(new MySqlParameter(string.Format("@ColumnValue{0}", num), item.MValue.Trim()));
					num++;
				}
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
			stringBuilder.Append(")");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(stringBuilder.ToString(), list.ToArray());
		}

		private static void BuildModelInfo<T>(MContext ctx) where T : BaseModel
		{
			Type typeFromHandle = typeof(T);
			BuildModelInfo(ctx, typeFromHandle);
		}

		private static void BuildModelInfo(MContext ctx, Type model)
		{
			if (model.IsSubclassOf(typeof(BaseModel)))
			{
				BuildEditModelInfo(ctx, model);
			}
			else
			{
				BuildListModelInfo(model);
			}
		}

		private static void BuildListModelInfo(Type model)
		{
			if (!modelInfoCache.ContainsKey(model))
			{
				lock (objLock)
				{
					List<MyPropertyInfo> list = new List<MyPropertyInfo>();
					foreach (PropertyInfo item in model.GetProperties().ToList())
					{
						MyPropertyInfo myPropertyInfor = GetMyPropertyInfor(item);
						list.Add(myPropertyInfor);
					}
					if (!modelInfoCache.ContainsKey(model))
					{
						modelInfoCache.TryAdd(model, list.ToArray());
					}
				}
			}
		}

		private static MyPropertyInfo GetMyPropertyInfor(PropertyInfo pt)
		{
			MyPropertyInfo myPropertyInfo = new MyPropertyInfo(pt, pt.IsDefined(typeof(ColumnEncryptAttribute), true));
			GetBizVerificationRule(myPropertyInfo, pt);
			GetFieldCustomerRule(myPropertyInfo, pt);
			return myPropertyInfo;
		}

		private static void BuildEditModelInfo(MContext ctx, Type model)
		{
			BaseModel baseModel = Activator.CreateInstance(model) as BaseModel;
			if (!modelInfoCache.ContainsKey(model))
			{
				DataColumnCollection tableCol = GetTableCol(ctx, baseModel.TableName);
				lock (objLock)
				{
					List<MyPropertyInfo> list = new List<MyPropertyInfo>();
					foreach (PropertyInfo item in model.GetProperties().ToList())
					{
						MyPropertyInfo myPropertyInfor = GetMyPropertyInfor(item);
						foreach (DataColumn item2 in tableCol)
						{
							if (item2.ColumnName.EqualsIgnoreCase(item.Name))
							{
								myPropertyInfor.HaveBDField = true;
								break;
							}
						}
						list.Add(myPropertyInfor);
						if (item.PropertyType.IsGenericType)
						{
							Type[] genericArguments = item.PropertyType.GetGenericArguments();
							if (genericArguments[0].GetTypeInfo().IsSubclassOf(typeof(BaseModel)) && HasEntryAttribute(item))
							{
								BuildModelInfo(ctx, genericArguments[0]);
							}
						}
					}
					GetRuleAutoMakeNo(model, list);
					if (!modelInfoCache.ContainsKey(model))
					{
						modelInfoCache.TryAdd(model, list.ToArray());
					}
				}
			}
			lock (objLock)
			{
				BuildModelSql(ctx, baseModel);
			}
		}

		private static void BuildModelSql(MContext ctx, BaseModel modelData)
		{
			ModelToMySqlInfor modelToMySqlInfor = new ModelToMySqlInfor();
			Type type = modelData.GetType();
			if (modelSqlCache.ContainsKey(type))
			{
				modelToMySqlInfor = modelSqlCache[type];
			}
			else
			{
				modelSqlCache.TryAdd(type, modelToMySqlInfor);
			}
			if (string.IsNullOrWhiteSpace(modelToMySqlInfor.MainTableName) || string.IsNullOrWhiteSpace(modelToMySqlInfor.MainTableInsertSql) || string.IsNullOrWhiteSpace(modelToMySqlInfor.MainTableUpdateSql))
			{
				modelToMySqlInfor.MainTableName = modelData.TableName;
				modelToMySqlInfor.MainTablePKFieldName = modelData.PKFieldName;
				BuildMainTableSql(ctx, type, modelData, modelToMySqlInfor);
				BuildMultiLangTableSql(ctx, modelData, modelToMySqlInfor);
				modelSqlCache[type] = modelToMySqlInfor;
			}
		}

		private static void BuildMultiLangTableSql(MContext ctx, BaseModel modelData, ModelToMySqlInfor sql)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string text = modelData.TableName + "_L";
			sql.HaveMultiLangTable = IsExistsTable(ctx, text);
			if (sql.HaveMultiLangTable)
			{
				DataColumnCollection dataColumnCollection = sql.MultiCols = GetTableCol(ctx, text);
				if (modelData.MultiLanguage == null || modelData.MultiLanguage.Count == 0)
				{
					GetMultiFieldLst(ctx, modelData, dataColumnCollection);
				}
				StringBuilder stringBuilder = new StringBuilder(" insert into " + text + "(MPKID, MParentID, MLocaleID, MIsDelete, MModifierID, MModifyDate, MCreatorID, MCreateDate ");
				StringBuilder stringBuilder2 = new StringBuilder(" values (@MPKID,@MParentID,@MLocaleID,@MIsDelete, @MModifierID, @MModifyDate, @MCreatorID, @MCreateDate ");
				StringBuilder stringBuilder3 = new StringBuilder(" update " + text + " set");
				StringBuilder stringBuilder4 = new StringBuilder(" select MPKID, MParentID, MLocaleID, MIsDelete, MModifierID, MModifyDate, MCreatorID, MCreateDate");
				stringBuilder3.AppendLine(" MPKID = @MPKID,");
				stringBuilder3.AppendLine(" MParentID = @MParentID,");
				stringBuilder3.AppendLine(" MLocaleID =@MLocaleID,");
				stringBuilder3.AppendLine(" MIsDelete = @MIsDelete,");
				stringBuilder3.AppendLine(" MModifierID = @MModifierID,");
				stringBuilder3.AppendLine(" MModifyDate = @MModifyDate,");
				stringBuilder3.AppendLine(" MCreatorID = @MCreatorID,");
				stringBuilder3.AppendLine(" MCreateDate =@MCreateDate ");
				if (ExistsColunm(modelData, "MOrgID"))
				{
					stringBuilder.Append(", MOrgID");
					stringBuilder2.Append(",@MOrgID");
					stringBuilder3.Append(", MOrgID = @MOrgID");
					stringBuilder4.Append(", MOrgID");
				}
				foreach (MultiLanguageFieldList item in modelData.MultiLanguage)
				{
					stringBuilder.AppendFormat(",{0}", item.MFieldName);
					if (item.IsEncrypt)
					{
						stringBuilder4.AppendFormat(",CONVERT(AES_DECRYPT({0},'{1}') USING utf8) AS {0} ", item.MFieldName, "JieNor-001");
						stringBuilder2.AppendFormat(",AES_ENCRYPT(@{0},'{1}')", item.MFieldName, "JieNor-001");
						if (!item.MFieldName.EqualsIgnoreCase("MCreateDate") && !item.MFieldName.EqualsIgnoreCase("MCreatorID") && !item.MFieldName.EqualsIgnoreCase("MModifyDate") && !item.MFieldName.EqualsIgnoreCase("MModifierID") && !item.MFieldName.EqualsIgnoreCase("MOrgID") && !item.MFieldName.EqualsIgnoreCase("MIsDelete"))
						{
							stringBuilder3.AppendLine(string.Format(", {0} = AES_ENCRYPT(@{0},'{1}') ", item.MFieldName, "JieNor-001"));
						}
					}
					else
					{
						stringBuilder4.AppendFormat(",{0} ", item.MFieldName);
						stringBuilder2.AppendFormat(",@{0}", item.MFieldName);
						if (!item.MFieldName.EqualsIgnoreCase("MCreateDate") && !item.MFieldName.EqualsIgnoreCase("MCreatorID") && !item.MFieldName.EqualsIgnoreCase("MModifyDate") && !item.MFieldName.EqualsIgnoreCase("MModifierID") && !item.MFieldName.EqualsIgnoreCase("MOrgID") && !item.MFieldName.EqualsIgnoreCase("MIsDelete"))
						{
							stringBuilder3.AppendLine(string.Format(", {0} = @{0} ", item.MFieldName));
						}
					}
				}
				stringBuilder.Append(" )");
				stringBuilder2.Append(" )");
				stringBuilder3.Append(" where MPKID=@MPKID and MIsDelete = 0  ");
				if (ExistsColunm(modelData, "MOrgID"))
				{
					stringBuilder3.Append(" and MOrgID = @MOrgID ");
				}
				stringBuilder4.AppendFormat(" from {0} ", text);
				sql.MultiLangTableInsertSql = stringBuilder.ToString() + stringBuilder2.ToString();
				sql.MultiLangTableUpdateSql = stringBuilder3.ToString();
				sql.MultiLangTableSelectSql = stringBuilder4.ToString();
				foreach (DataColumn item2 in dataColumnCollection)
				{
					if (item2.ColumnName.Equals("MIsDelete", StringComparison.OrdinalIgnoreCase))
					{
						StringBuilder stringBuilder5 = new StringBuilder("Update " + text + " Set ");
						stringBuilder5.AppendLine(" MIsDelete = 1 ");
						stringBuilder5.AppendLine(" Where MParentID = @PKID and MIsDelete = 0 ");
						if (ExistsColunm(modelData, "MOrgID"))
						{
							stringBuilder5.Append(" and MOrgID = @MOrgID ");
						}
						sql.MultiLangDeleteFlagSql = stringBuilder5.ToString();
					}
				}
			}
		}

		private static void GetMultiFieldLst(MContext ctx, BaseModel modelData, DataColumnCollection fields)
		{
			modelData.MultiLanguage = new List<MultiLanguageFieldList>();
			foreach (DataColumn field in fields)
			{
				if (!field.ColumnName.Equals("MParentID", StringComparison.OrdinalIgnoreCase) && !field.ColumnName.Equals("MPKID", StringComparison.OrdinalIgnoreCase) && !field.ColumnName.Equals("MLocaleID", StringComparison.OrdinalIgnoreCase) && !field.ColumnName.Equals("MIsDelete", StringComparison.OrdinalIgnoreCase) && !field.ColumnName.Equals("MIsActive", StringComparison.OrdinalIgnoreCase) && !field.ColumnName.Equals("MModifierID", StringComparison.OrdinalIgnoreCase) && !field.ColumnName.Equals("MModifyDate", StringComparison.OrdinalIgnoreCase) && !field.ColumnName.Equals("MCreateDate", StringComparison.OrdinalIgnoreCase) && !field.ColumnName.Equals("MCreatorID", StringComparison.OrdinalIgnoreCase) && !field.ColumnName.Equals("MOrgID", StringComparison.OrdinalIgnoreCase))
				{
					MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
					multiLanguageFieldList.MFieldName = field.ColumnName;
					if (modelData.MMultiLangEncryptColumns != null && modelData.MMultiLangEncryptColumns.Contains(field.ColumnName))
					{
						multiLanguageFieldList.IsEncrypt = true;
					}
					foreach (string mActiveLocaleID in ctx.MActiveLocaleIDS)
					{
						MultiLanguageField multiLanguageField = new MultiLanguageField();
						multiLanguageField.MLocaleID = mActiveLocaleID;
						multiLanguageFieldList.MMultiLanguageField.Add(multiLanguageField);
					}
					modelData.MultiLanguage.Add(multiLanguageFieldList);
				}
			}
		}

		private static string BuildCustomizeSearchSql(MContext ctx, Type model, List<MyPropertyInfo> properties)
		{
			BaseModel baseModel = Activator.CreateInstance(model) as BaseModel;
			string empty = string.Empty;
			if (properties.Count == 0)
			{
				return empty;
			}
			List<string> list = new List<string>();
			foreach (MyPropertyInfo property in properties)
			{
				if (property.IsEncrypt)
				{
					list.Add(string.Format("CONVERT(AES_DECRYPT({0},'{1}') USING utf8) AS {0}", property.Property.Name, "JieNor-001"));
				}
				else
				{
					list.Add(string.Format("{0}", property.Property.Name));
				}
			}
			return string.Format(" Select {0} From {1} ", string.Join(",", list), baseModel.TableName);
		}

		private static void BuildMainTableSql(MContext ctx, Type model, BaseModel modelData, ModelToMySqlInfor sql)
		{
			MyPropertyInfo[] array = (from f in modelInfoCache[model]
			where f.HaveBDField
			select f).ToArray();
			if (array.Length == 0)
			{
				modelInfoCache.TryRemove(model, out array);
				BuildEditModelInfo(ctx, model);
				array = (from f in modelInfoCache[model]
				where f.HaveBDField
				select f).ToArray();
			}
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			List<string> list4 = new List<string>();
			MyPropertyInfo[] array2 = array;
			foreach (MyPropertyInfo myPropertyInfo in array2)
			{
				list.Add(myPropertyInfo.Property.Name);
				if (myPropertyInfo.IsEncrypt)
				{
					if (!string.IsNullOrWhiteSpace(myPropertyInfo.AppSourceString))
					{
						list2.Add(string.Format("( case when @AppSource in ({2}) then ( AES_ENCRYPT(@{0},'{1}')) else null end ) ", myPropertyInfo.Property.Name, "JieNor-001", myPropertyInfo.AppSourceString));
					}
					else
					{
						list2.Add(string.Format("AES_ENCRYPT(@{0},'{1}')", myPropertyInfo.Property.Name, "JieNor-001"));
					}
					list3.Add(string.Format("CONVERT(AES_DECRYPT({0},'{1}') USING utf8) AS {0}", myPropertyInfo.Property.Name, "JieNor-001"));
					if (!myPropertyInfo.InsertOnly)
					{
						if (!string.IsNullOrWhiteSpace(myPropertyInfo.AppSourceString))
						{
							list4.Add(string.Format("{0} = ( case when @AppSource in ({2}) then( AES_ENCRYPT(@{0}, '{1}')) else {0} end )", myPropertyInfo.Property.Name, "JieNor-001", myPropertyInfo.AppSourceString));
						}
						else
						{
							list4.Add(string.Format("{0}=AES_ENCRYPT(@{0},'{1}')", myPropertyInfo.Property.Name, "JieNor-001"));
						}
					}
				}
				else
				{
					if (!string.IsNullOrWhiteSpace(myPropertyInfo.AppSourceString))
					{
						list2.Add(string.Format("( case when @AppSource in ({1}) then @{0} else null end ) ", myPropertyInfo.Property.Name, myPropertyInfo.AppSourceString));
					}
					else
					{
						list2.Add(string.Format("@{0}", myPropertyInfo.Property.Name));
					}
					list3.Add(string.Format("{0}", myPropertyInfo.Property.Name));
					if (!myPropertyInfo.InsertOnly)
					{
						if (!string.IsNullOrWhiteSpace(myPropertyInfo.AppSourceString))
						{
							list4.Add(string.Format(" {0}= ( case when @AppSource in ({2}) then @{0}  else {0} end )", myPropertyInfo.Property.Name, "JieNor-001", myPropertyInfo.AppSourceString));
						}
						else
						{
							list4.Add(string.Format("{0}=@{0}", myPropertyInfo.Property.Name));
						}
					}
				}
			}
			if (list.Count != 0)
			{
				sql.MainTableSelectSql = string.Format(" Select {0} From {1} ", string.Join(",", list3), modelData.TableName);
				StringBuilder stringBuilder = new StringBuilder("Insert Into " + modelData.TableName + "(");
				stringBuilder.AppendLine(string.Join(",", list) + ")");
				stringBuilder.AppendLine("Values(" + string.Join(",", list2) + ")");
				sql.MainTableInsertSql = stringBuilder.ToString();
				StringBuilder stringBuilder2 = new StringBuilder("Update " + modelData.TableName + " Set ");
				stringBuilder2.AppendLine(string.Join(",", list4));
				stringBuilder2.AppendLine(" Where " + modelData.PKFieldName + " = @" + modelData.PKFieldName);
				sql.MainTableUpdateSql = stringBuilder2.ToString();
				if (array.Any((MyPropertyInfo f) => f.Property.Name.Equals("MIsDelete", StringComparison.OrdinalIgnoreCase)))
				{
					StringBuilder stringBuilder3 = new StringBuilder("Update " + modelData.TableName + " Set ");
					stringBuilder3.AppendLine(" MIsDelete = 1, MModifierID = @ModifierID, MModifyDate = @ModifyDate ");
					stringBuilder3.AppendLine(" Where " + modelData.PKFieldName + " = @PKID and MOrgID = @MOrgID and MIsDelete = 0 ");
					sql.MainDeleteFlagSql = stringBuilder3.ToString();
					if (!string.IsNullOrWhiteSpace(modelData.FKFieldName))
					{
						StringBuilder stringBuilder4 = new StringBuilder("Update " + modelData.TableName + " Set ");
						stringBuilder4.AppendLine(" MIsDelete = 1, MModifierID = @ModifierID, MModifyDate = @ModifyDate ");
						stringBuilder4.AppendLine(" Where " + modelData.FKFieldName + " = @PKID and MOrgID = @MOrgID and MIsDelete = 0 ");
						sql.MainDeleteFlagByFKSql = stringBuilder4.ToString();
					}
				}
				if (array.Any((MyPropertyInfo f) => f.Property.Name.Equals("MIsActive", StringComparison.OrdinalIgnoreCase)))
				{
					StringBuilder stringBuilder5 = new StringBuilder("Update " + modelData.TableName + " Set ");
					stringBuilder5.AppendLine(" MIsActive = @MIsActive, MModifierID =@ModifierID, MModifyDate= @ModifyDate ");
					stringBuilder5.AppendLine(" Where " + modelData.PKFieldName + " = @PKID and MOrgID = @MOrgID and MIsDelete = 0 and MIsActive != @MIsActive ");
					sql.MainArchiveFlagSql = stringBuilder5.ToString();
				}
			}
		}

		private static DataColumnCollection GetTableCol(MContext ctx, string tableName)
		{
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(string.Format("select * from {0} where 1 = 0 ", tableName));
			return (dataSet != null && dataSet.Tables.Count > 0) ? dataSet.Tables[0].Columns : null;
		}

		private static List<CommandInfo> GetMultiInsertOrUpdateCmd(MContext ctx, BaseModel modelData, ModelToMySqlInfor sqlInfor, List<string> fields = null)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (sqlInfor.HaveMultiLangTable)
			{
				DataColumnCollection multiCols = sqlInfor.MultiCols;
				List<MultiLanguageFieldList> multiLanguage = modelData.MultiLanguage;
				if (multiLanguage != null && multiLanguage.Count > 0)
				{
					if (modelData.IsNew)
					{
						foreach (MultiLanguageFieldList item in multiLanguage)
						{
							if (string.IsNullOrWhiteSpace(item.MParentID))
							{
								item.MParentID = modelData.PKFieldValue;
							}
						}
					}
					foreach (string mActiveLocaleID in ctx.MActiveLocaleIDS)
					{
						string empty = string.Empty;
						bool flag = IsNewMultiLangRow(multiLanguage, mActiveLocaleID, out empty);
						List<MySqlParameter> list2 = new List<MySqlParameter>();
						if (flag || modelData.IsNew)
						{
							IEnumerable<MultiLanguageField> enumerable = multiLanguage.SelectMany((MultiLanguageFieldList a) => from b in a.MMultiLanguageField
							where b.MLocaleID == mActiveLocaleID
							select b);
							if (enumerable.Any())
							{
								foreach (MultiLanguageField item2 in enumerable)
								{
									item2.MPKID = empty;
								}
								list2 = GetMultiInsertOrUpdatePara(ctx, modelData, multiCols, multiLanguage, mActiveLocaleID, empty);
								list.Add(new CommandInfo(sqlInfor.MultiLangTableInsertSql, list2.ToArray()));
							}
						}
						else if (fields == null || fields.Count == 0)
						{
							list2 = GetMultiInsertOrUpdatePara(ctx, modelData, multiCols, multiLanguage, mActiveLocaleID, empty);
							if (!list2.Any((MySqlParameter f) => f.ParameterName == "@MPKID"))
							{
								MySqlParameter mySqlParameter = new MySqlParameter("@MPKID", MySqlDbType.VarChar, 36);
								mySqlParameter.Value = empty;
								list2.Add(mySqlParameter);
							}
							list.Add(new CommandInfo(sqlInfor.MultiLangTableUpdateSql, list2.ToArray()));
						}
						else
						{
							string multiPartialUpdateSql = GetMultiPartialUpdateSql(sqlInfor, modelData, fields);
							if (!string.IsNullOrWhiteSpace(multiPartialUpdateSql))
							{
								list2 = GetMultiPartialUpdatePara(multiCols, multiLanguage, mActiveLocaleID, fields);
								MySqlParameter mySqlParameter2 = new MySqlParameter("@MPKID", MySqlDbType.VarChar, 36);
								mySqlParameter2.Value = empty;
								list2.Add(mySqlParameter2);
								list.Add(new CommandInfo(multiPartialUpdateSql, list2.ToArray()));
							}
						}
					}
				}
			}
			return list;
		}

		private static string GetMultiPartialUpdateSql(ModelToMySqlInfor sqlInfor, BaseModel modelData, List<string> fields)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder(" update " + sqlInfor.MainTableName + "_L Set ");
			foreach (MultiLanguageFieldList item in modelData.MultiLanguage)
			{
				if (fields.Any((string f) => f.EqualsIgnoreCase(item.MFieldName)))
				{
					if (item.IsEncrypt)
					{
						stringBuilder.AppendLine(string.Format(", {0}=AES_ENCRYPT(@{0},'{1}') ", item.MFieldName, "JieNor-001"));
						list.Add(string.Format(" {0}=AES_ENCRYPT(@{0},'{1}')", item.MFieldName, "JieNor-001"));
					}
					else
					{
						stringBuilder.AppendLine(string.Format(", {0} = @{0} ", item.MFieldName));
						list.Add(string.Format(" {0} = @{0} ", item.MFieldName));
					}
				}
			}
			if (list.Count > 0)
			{
				return string.Format(" update {0}_L Set {1} where MPKID=@MPKID ", sqlInfor.MainTableName, string.Join(",", list));
			}
			return "";
		}

		private static List<MySqlParameter> GetMultiInsertOrUpdatePara(MContext ctx, BaseModel modelData, DataColumnCollection multiFields, List<MultiLanguageFieldList> mltFldValues, string localID, string pkid)
		{
			List<MySqlParameter> list = new List<MySqlParameter>();
			MySqlParameter mySqlParameter = new MySqlParameter("@MPKID", MySqlDbType.VarChar, 36);
			mySqlParameter.Value = pkid;
			list.Add(mySqlParameter);
			MySqlParameter mySqlParameter2 = new MySqlParameter("@MParentID", MySqlDbType.VarChar, 36);
			mySqlParameter2.Value = modelData.PKFieldValue;
			list.Add(mySqlParameter2);
			MySqlParameter mySqlParameter3 = new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36);
			mySqlParameter3.Value = localID;
			list.Add(mySqlParameter3);
			MySqlParameter mySqlParameter4 = new MySqlParameter("@MIsDelete", MySqlDbType.Bit, 1);
			mySqlParameter4.Value = 0;
			list.Add(mySqlParameter4);
			if (ctx != null)
			{
				MySqlParameter mySqlParameter5 = new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36);
				mySqlParameter5.Value = ctx.MOrgID;
				list.Add(mySqlParameter5);
				MySqlParameter mySqlParameter6 = new MySqlParameter("@MModifierID", MySqlDbType.VarChar, 36);
				mySqlParameter6.Value = ctx.MUserID;
				list.Add(mySqlParameter6);
				MySqlParameter mySqlParameter7 = new MySqlParameter("@MModifyDate", MySqlDbType.DateTime);
				mySqlParameter7.Value = ctx.DateNow;
				list.Add(mySqlParameter7);
				MySqlParameter mySqlParameter8 = new MySqlParameter("@MCreatorID", MySqlDbType.VarChar, 36);
				mySqlParameter8.Value = ctx.MUserID;
				list.Add(mySqlParameter8);
				MySqlParameter mySqlParameter9 = new MySqlParameter("@MCreateDate", MySqlDbType.DateTime, 36);
				mySqlParameter9.Value = ctx.DateNow;
				list.Add(mySqlParameter9);
				MySqlParameter mySqlParameter10 = new MySqlParameter("@AppSource", MySqlDbType.DateTime, 36);
				mySqlParameter10.Value = (string.IsNullOrWhiteSpace(ctx.AppSource) ? "System" : ctx.AppSource);
				list.Add(mySqlParameter10);
			}
			if (mltFldValues != null)
			{
				mltFldValues = (from t in mltFldValues
				where t != null
				select t).ToList();
				foreach (DataColumn multiField in multiFields)
				{
					if (!multiField.ColumnName.EqualsIgnoreCase("MPKID") && !multiField.ColumnName.EqualsIgnoreCase("MParentID") && !multiField.ColumnName.EqualsIgnoreCase("MLocaleID") && !multiField.ColumnName.EqualsIgnoreCase("MOrgID") && !multiField.ColumnName.EqualsIgnoreCase("MModifierID") && !multiField.ColumnName.EqualsIgnoreCase("MModifyDate") && !multiField.ColumnName.EqualsIgnoreCase("MCreatorID") && !multiField.ColumnName.EqualsIgnoreCase("MCreateDate") && !multiField.ColumnName.EqualsIgnoreCase("MIsDelete"))
					{
						MySqlParameter mySqlParameter11 = new MySqlParameter("@" + multiField.ColumnName, MySqlDbType.VarChar, 2000);
						mySqlParameter11.Value = "";
						MultiLanguageFieldList multiLanguageFieldList = mltFldValues.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName.EqualsIgnoreCase(multiField.ColumnName));
						if (multiLanguageFieldList != null)
						{
							MultiLanguageField multiLanguageField = multiLanguageFieldList.MMultiLanguageField.FirstOrDefault((MultiLanguageField f) => f.MLocaleID == localID);
							if (multiLanguageField != null && multiLanguageField.MValue != null)
							{
								mySqlParameter11.Value = multiLanguageField.MValue;
							}
						}
						list.Add(mySqlParameter11);
					}
				}
			}
			return list;
		}

		private static List<MySqlParameter> GetMultiPartialUpdatePara(DataColumnCollection multiFields, List<MultiLanguageFieldList> mltFldValues, string localID, List<string> fields)
		{
			List<MySqlParameter> list = new List<MySqlParameter>();
			foreach (DataColumn multiField in multiFields)
			{
				if (!multiField.ColumnName.EqualsIgnoreCase("MPKID") && !multiField.ColumnName.EqualsIgnoreCase("MParentID") && !multiField.ColumnName.EqualsIgnoreCase("MLocaleID") && !multiField.ColumnName.EqualsIgnoreCase("MOrgID") && !multiField.ColumnName.EqualsIgnoreCase("MModifierID") && !multiField.ColumnName.EqualsIgnoreCase("MModifyDate") && !multiField.ColumnName.EqualsIgnoreCase("MCreatorID") && !multiField.ColumnName.EqualsIgnoreCase("MCreateDate") && fields.Any((string f) => f.EqualsIgnoreCase(multiField.ColumnName)))
				{
					MySqlParameter mySqlParameter = new MySqlParameter("@" + multiField.ColumnName, MySqlDbType.VarChar, 2000);
					mySqlParameter.Value = "";
					MultiLanguageFieldList multiLanguageFieldList = mltFldValues.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName.EqualsIgnoreCase(multiField.ColumnName));
					if (multiLanguageFieldList != null)
					{
						MultiLanguageField multiLanguageField = multiLanguageFieldList.MMultiLanguageField.FirstOrDefault((MultiLanguageField f) => f.MLocaleID == localID);
						if (multiLanguageField != null && multiLanguageField.MValue != null)
						{
							mySqlParameter.Value = multiLanguageField.MValue;
						}
					}
					list.Add(mySqlParameter);
				}
			}
			return list;
		}

		private static bool IsNewMultiLangRow(List<MultiLanguageFieldList> multiFieldValues, string localeID, out string pkid)
		{
			MultiLanguageFieldList multiLanguageFieldList = multiFieldValues.FirstOrDefault();
			if (multiLanguageFieldList != null)
			{
				MultiLanguageField multiLanguageField = multiLanguageFieldList.MMultiLanguageField.FirstOrDefault((MultiLanguageField f) => f.MLocaleID == localeID);
				if (multiLanguageField == null || string.IsNullOrWhiteSpace(multiLanguageField.MPKID))
				{
					pkid = UUIDHelper.GetGuid();
					return true;
				}
				pkid = multiLanguageField.MPKID;
			}
			else
			{
				pkid = string.Empty;
			}
			return false;
		}

		private static MySqlParameter GetSqlParameter(BaseModel modelData, MyPropertyInfo property, bool encode, string paramPrefix = null)
		{
			Type propertyType = property.Property.PropertyType;
			string name = property.Property.Name;
			object sqlParameterValue = GetSqlParameterValue(modelData, property, name);
			MySqlParameter mySqlParameter = new MySqlParameter();
			mySqlParameter.ParameterName = "@" + name;
			if (!string.IsNullOrWhiteSpace(paramPrefix))
			{
				mySqlParameter.ParameterName += paramPrefix;
			}
			if (propertyType == typeof(int))
			{
				mySqlParameter.MySqlDbType = MySqlDbType.Int32;
				mySqlParameter.Size = 30;
				mySqlParameter.Value = sqlParameterValue;
			}
			else if (propertyType == typeof(long))
			{
				mySqlParameter.MySqlDbType = MySqlDbType.Int64;
				mySqlParameter.Size = 30;
				mySqlParameter.Value = sqlParameterValue;
			}
			else if (propertyType == typeof(decimal) || propertyType == typeof(decimal?) || propertyType == typeof(float) || propertyType == typeof(double))
			{
				mySqlParameter.MySqlDbType = MySqlDbType.Decimal;
				mySqlParameter.Size = 30;
				mySqlParameter.Value = sqlParameterValue;
			}
			else if (propertyType == typeof(bool))
			{
				mySqlParameter.MySqlDbType = MySqlDbType.Bit;
				mySqlParameter.Value = sqlParameterValue;
			}
			else if (propertyType == typeof(DateTime))
			{
				mySqlParameter.MySqlDbType = MySqlDbType.DateTime;
				mySqlParameter.Value = sqlParameterValue;
			}
			else if (propertyType == typeof(string))
			{
				mySqlParameter.MySqlDbType = MySqlDbType.VarChar;
				mySqlParameter.Size = 2000;
				mySqlParameter.Value = sqlParameterValue;
			}
			return mySqlParameter;
		}

		private static object GetSqlParameterValue(BaseModel modelData, MyPropertyInfo property, string proertyName)
		{
			object propertyValue = modelData.GetPropertyValue(proertyName);
			if (propertyValue == null || !property.IsEncrypt)
			{
				return propertyValue;
			}
			return propertyValue;
		}

		private static void SetModelValue<T>(MyPropertyInfo[] plist, DataColumnCollection cols, DataRow dr, T model, Dictionary<string, MyPropertyInfo> dicPropertyList)
		{
			foreach (DataColumn col in cols)
			{
				MyPropertyInfo myPropertyInfo = dicPropertyList[col.ColumnName];
				if (myPropertyInfo != null && !Convert.IsDBNull(dr[myPropertyInfo.Property.Name]))
				{
					SetPropertyValue(dr, model, myPropertyInfo, true, 0);
				}
			}
		}

		private static void SetPropertyValue<T>(DataRow dr, T model, MyPropertyInfo info, bool decode, int rowIndex = 0)
		{
			Type propertyType = info.Property.PropertyType;
			string name = info.Property.Name;
			try
			{
				if (propertyType == typeof(bool))
				{
					model.SetPropertyValue(name, dr[name].ToString() == "1");
				}
				else if (propertyType == typeof(DateTime))
				{
					object obj = dr[name];
					DateTime dateTime = default(DateTime);
					if (obj != null && DateTime.TryParse(obj.ToString(), out dateTime))
					{
						model.SetPropertyValue(name, dateTime);
					}
					else
					{
						model.SetPropertyValue(name, DateTime.MinValue);
					}
				}
				else if (propertyType == typeof(string))
				{
					object obj2 = dr[name];
					if (object.Equals(obj2, DBNull.Value) || obj2 == null || string.IsNullOrWhiteSpace(obj2.ToString()))
					{
						model.SetPropertyValue(name, "");
					}
					else if (info.IsEncrypt)
					{
						string text = "";
						string text2 = obj2.ToString();
						if (encryptCache.ContainsKey(text2))
						{
							text = encryptCache[text2].ToString();
						}
						else
						{
							text = DESEncrypt.Decrypt(text2);
							lock (encryptCache.SyncRoot)
							{
								if (!encryptCache.ContainsKey(text2))
								{
									encryptCache.Add(text2, text);
								}
							}
						}
						if (decode && text != null)
						{
							model.SetPropertyValue(name, HttpUtility.HtmlDecode(text));
						}
						else
						{
							model.SetPropertyValue(name, text);
						}
					}
					else if (obj2 != null)
					{
						if (decode && HttpContext.Current != null)
						{
							model.SetPropertyValue(name, HttpUtility.HtmlDecode(obj2.ToString()));
						}
						else
						{
							model.SetPropertyValue(name, obj2.ToString());
						}
					}
				}
				else if (propertyType == typeof(decimal))
				{
					model.SetPropertyValue(name, Convert.ToDecimal(dr[name]));
				}
				else if (propertyType == typeof(int))
				{
					model.SetPropertyValue(name, Convert.ToInt32(dr[name]));
				}
				else
				{
					model.SetPropertyValue(name, dr[name]);
				}
			}
			catch (Exception ex)
			{
				MConvertException ex2 = new MConvertException(ex, name, dr[name], rowIndex);
				throw ex2;
			}
		}

		private static void FillModelMultiLanguageFieldValue<T>(MContext ctx, T model, DataColumnCollection cols, IEnumerable<DataRow> dataRows) where T : BaseModel
		{
			if (model != null && dataRows != null && dataRows.Count() != 0 && cols != null && cols.Count != 0)
			{
				foreach (DataColumn col in cols)
				{
					if (!col.ColumnName.Equals("MParentID") && !col.ColumnName.Equals("MPKID") && !col.ColumnName.Equals("MLocaleID") && !col.ColumnName.Equals("MOrgID") && !col.ColumnName.Equals("MModifierID") && !col.ColumnName.Equals("MModifyDate") && !col.ColumnName.Equals("MCreatorID") && !col.ColumnName.Equals("MCreateDate") && !col.ColumnName.Equals("MIsActive") && !col.ColumnName.Equals("MIsDelete"))
					{
						MultiLanguageFieldList multiLanguageFieldList = model.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName.EqualsIgnoreCase(col.ColumnName));
						if (multiLanguageFieldList != null)
						{
							multiLanguageFieldList.MFieldName = col.ColumnName;
							foreach (DataRow dataRow in dataRows)
							{
								multiLanguageFieldList.MParentID = dataRow["MParentID"].ToString();
								string localeId = dataRow["MLocaleID"].ToString();
								MultiLanguageField multiLanguageField = multiLanguageFieldList.MMultiLanguageField.FirstOrDefault((MultiLanguageField t) => t.MLocaleID.EqualsIgnoreCase(localeId));
								if (multiLanguageField != null)
								{
									multiLanguageField.MLocaleID = localeId;
									multiLanguageField.MPKID = dataRow["MPKID"].ToString();
									if (cols.Contains("MOrgID"))
									{
										multiLanguageField.MOrgID = dataRow["MOrgID"].ToString();
									}
									multiLanguageField.MIsDelete = (int.Parse(dataRow["MIsDelete"].ToString()) == 1);
									multiLanguageField.MModifyDate = ((!string.IsNullOrWhiteSpace(dataRow["MModifyDate"].ToString())) ? DateTime.Parse(dataRow["MModifyDate"].ToString()) : multiLanguageField.MModifyDate);
									multiLanguageField.MModifierID = dataRow["MModifierID"].ToString();
									multiLanguageField.MCreateDate = ((!string.IsNullOrWhiteSpace(dataRow["MCreateDate"].ToString())) ? DateTime.Parse(dataRow["MCreateDate"].ToString()) : multiLanguageField.MModifyDate);
									multiLanguageField.MCreatorID = dataRow["MCreatorID"].ToString();
									multiLanguageField.MValue = HttpUtility.HtmlDecode(dataRow[col.ColumnName].ToString());
									if (localeId.EqualsIgnoreCase(ctx.MLCID))
									{
										model.TrySetPropertyValue(col.ColumnName, multiLanguageField.MValue);
										multiLanguageFieldList.MMultiLanguageValue = HttpUtility.HtmlDecode(multiLanguageField.MValue);
									}
								}
							}
						}
					}
				}
			}
		}

		private static bool ExistsColunm(BaseModel model, string colName)
		{
			if (modelInfoCache.ContainsKey(model.GetType()))
			{
				return modelInfoCache[model.GetType()].Any((MyPropertyInfo f) => f.HaveBDField && f.Property.Name.EqualsIgnoreCase(colName));
			}
			return false;
		}

		private static bool ExistsColunm(Type modelType, string colName)
		{
			if (modelInfoCache.ContainsKey(modelType))
			{
				return modelInfoCache[modelType].Any((MyPropertyInfo f) => f.HaveBDField && f.Property.Name.EqualsIgnoreCase(colName));
			}
			return false;
		}

		private static MyPropertyInfo[] GetEntryProperty(Type modelType)
		{
			MyPropertyInfo[] source = modelInfoCache[modelType];
			return (from f in source
			where f.Property.PropertyType.IsGenericType && HasEntryAttribute(f.Property) && f.Property.PropertyType.GetGenericArguments()[0].IsSubclassOf(typeof(BaseModel))
			select f).ToArray();
		}

		public static bool HasEntryAttribute(PropertyInfo property)
		{
			object[] customAttributes = property.GetCustomAttributes(false);
			foreach (object obj in customAttributes)
			{
				if (obj.ToString().ToUpper() == "JieNor.Megi.Core.Attribute.ModelEntryAttribute".ToUpper())
				{
					return true;
				}
			}
			return false;
		}

		public static T GetFirstOrDefaultModel<T>(DataSet ds)
		{
			if (ds == null || ds.Tables == null || ds.Tables.Count == 0)
			{
				return default(T);
			}
			return GetFirstOrDefaultModel<T>(ds.Tables[0]);
		}

		public static T GetFirstOrDefaultModel<T>(DataTable dt)
		{
			List<T> list = DataTableToList<T>(dt, 1, false);
			if (list.Count == 0)
			{
				return default(T);
			}
			return list[0];
		}

		public static List<T> DataTableToList<T>(DataSet ds)
		{
			return DataTableToList<T>(ds, 0, false);
		}

		public static List<T> DataTableToList<T>(DataTable dt)
		{
			return DataTableToList<T>(dt, 0, false);
		}

		public static List<T> DataTableToList<T>(DataSet ds, int top = 0, bool decode = false)
		{
			if (ds == null || ds.Tables == null || ds.Tables.Count == 0)
			{
				return new List<T>();
			}
			return DataTableToList<T>(ds.Tables[0], 0, decode);
		}

		public static List<T> DataTableToList<T>(DataTable dt, int top = 0, bool decode = false)
		{
			BuildListModelInfo(typeof(T));
			List<T> list = new List<T>();
			if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
			{
				return list;
			}
			MyPropertyInfo[] source = modelInfoCache[typeof(T)];
			int num = dt.Rows.Count;
			if (top > 0 && top < dt.Rows.Count)
			{
				num = top;
			}
			for (int i = 0; i < num; i++)
			{
				T val = Activator.CreateInstance<T>();
				foreach (DataColumn column in dt.Columns)
				{
					MyPropertyInfo myPropertyInfo = source.FirstOrDefault((MyPropertyInfo f) => f.Property.Name.EqualsIgnoreCase(column.ColumnName));
					if (myPropertyInfo != null && !Convert.IsDBNull(dt.Rows[i][myPropertyInfo.Property.Name]))
					{
						SetPropertyValue(dt.Rows[i], val, myPropertyInfo, decode, i + 1);
					}
				}
				list.Add(val);
			}
			return list;
		}

		public static OperationResult Delete<T>(MContext ctx, string pkID) where T : BaseModel
		{
			return Delete<T>(ctx, pkID.Trim(',').Split(',').ToList());
		}

		public static OperationResult Delete<T>(MContext ctx, List<string> pkIDS) where T : BaseModel
		{
			return DeleteFlag<T>(ctx, pkIDS);
		}

		public static List<CommandInfo> GetDeleteCmd<T>(MContext ctx, string pkID) where T : BaseModel
		{
			return GetDeleteFlagCmd<T>(ctx, pkID);
		}

		public static List<CommandInfo> GetDeleteCmd<T>(MContext ctx, List<string> pkIDS) where T : BaseModel
		{
			return GetDeleteFlagCmd<T>(ctx, pkIDS);
		}

		public static OperationResult DeleteFlag<T>(MContext ctx, string pkID) where T : BaseModel
		{
			List<string> list = new List<string>();
			if (pkID.IndexOf(',') > -1)
			{
				list = pkID.Split(',').ToList();
			}
			else
			{
				list.Add(pkID);
			}
			return DeleteFlag<T>(ctx, list);
		}

		public static OperationResult DeleteFlag<T>(MContext ctx, List<string> pkIDS) where T : BaseModel
		{
			List<CommandInfo> deleteFlagCmd = GetDeleteFlagCmd<T>(ctx, pkIDS);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(deleteFlagCmd);
			OperationResult operationResult = new OperationResult();
			if (num > 0)
			{
				operationResult.Message = "Success";
				operationResult.SuccessModelID = pkIDS;
			}
			else
			{
				operationResult.Message = "Fail";
				operationResult.FailModelID = pkIDS;
			}
			return operationResult;
		}

		public static List<CommandInfo> GetDeleteFlagCmd<T>(MContext ctx, string pkID) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			List<CommandInfo> list = new List<CommandInfo>();
			Type typeFromHandle = typeof(T);
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[typeFromHandle];
			List<MySqlParameter> list2 = new List<MySqlParameter>();
			MySqlParameter[] para = new MySqlParameter[4]
			{
				new MySqlParameter("@PKID", pkID.Replace("'", "")),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@ModifyDate", ctx.DateNow),
				new MySqlParameter("@ModifierID", ctx.MUserID)
			};
			if (!string.IsNullOrWhiteSpace(modelToMySqlInfor.MainDeleteFlagSql))
			{
				list.Add(new CommandInfo(modelToMySqlInfor.MainDeleteFlagSql, para));
			}
			PropertyInfo propertyInfo = typeFromHandle.GetProperties().FirstOrDefault((PropertyInfo x) => HasEntryAttribute(x));
			if (propertyInfo != (PropertyInfo)null)
			{
				Type[] genericArguments = propertyInfo.PropertyType.GetGenericArguments();
				Type key = genericArguments[0];
				ModelToMySqlInfor modelToMySqlInfor2 = modelSqlCache[key];
				if (!string.IsNullOrWhiteSpace(modelToMySqlInfor2.MainDeleteFlagByFKSql))
				{
					list.Add(new CommandInfo(modelToMySqlInfor2.MainDeleteFlagByFKSql, para));
				}
			}
			if (!string.IsNullOrWhiteSpace(modelToMySqlInfor.MultiLangDeleteFlagSql))
			{
				list.Add(new CommandInfo(modelToMySqlInfor.MultiLangDeleteFlagSql, para));
			}
			return list;
		}

		public static List<CommandInfo> GetDeleteFlagCmd<T>(MContext ctx, List<string> pkIDS) where T : BaseModel
		{
			if (pkIDS == null || pkIDS.Count == 0)
			{
				throw new Exception("Error Paramenter");
			}
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (string pkID in pkIDS)
			{
				list.AddRange((IEnumerable<CommandInfo>)GetDeleteFlagCmd<T>(ctx, pkID));
			}
			return list;
		}

		public static bool IsExistsTable(MContext ctx, string tableName)
		{
			MySqlParameter mySqlParameter = new MySqlParameter("@TableName", MySqlDbType.VarChar, 36);
			mySqlParameter.Value = tableName;
			string strSql = " select count(*) from information_schema.tables  where table_name = @TableName";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(strSql, mySqlParameter);
		}

		public static bool ExistsByKey<T>(MContext ctx, string MPKID, bool includeDelete = false) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[typeof(T)];
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal(modelToMySqlInfor.MainTablePKFieldName, MPKID);
			if (!includeDelete && ExistsColunm(typeof(T), "MIsDelete"))
			{
				sqlWhere.AddDeleteFilter("MIsDelete", SqlOperators.Equal, false);
			}
			string text = string.Format("select count(1) from {0} {1} ", modelToMySqlInfor.MainTableName, sqlWhere.WhereSqlString);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(text.ToString(), sqlWhere.Parameters);
		}

		public static bool ExistsByKey<T>(MContext ctx, string connString, string MPKID, bool includeDelete = false) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[typeof(T)];
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal(modelToMySqlInfor.MainTablePKFieldName, MPKID);
			if (!includeDelete && ExistsColunm(typeof(T), "MIsDelete"))
			{
				sqlWhere.AddDeleteFilter("MIsDelete", SqlOperators.Equal, false);
			}
			string text = string.Format("select count(1) from {0} {1} ", modelToMySqlInfor.MainTableName, sqlWhere.WhereSqlString);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(connString, text.ToString(), sqlWhere.Parameters);
		}

		public static bool ExistsByFilter<T>(MContext ctx, SqlWhere filter) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			if (filter == null)
			{
				filter = new SqlWhere();
			}
			if (ExistsColunm(typeof(T), "MOrgID") && filter.FilterString.IndexOf("MOrgID") == -1)
			{
				filter.AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
			}
			if (ExistsColunm(typeof(T), "MIsDelete") && filter.FilterString.IndexOf("MIsDelete") == -1)
			{
				filter.AddFilter("MIsDelete", SqlOperators.Equal, 0);
			}
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[typeof(T)];
			string text = string.Format("select count(*) from {0} {1}  ", modelToMySqlInfor.MainTableName, filter.WhereSqlString);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(text.ToString(), filter.Parameters);
		}

		public static OperationResult InsertOrUpdate<T>(MContext ctx, BaseModel modelData, List<string> fields = null) where T : BaseModel
		{
			return ModelInfoManager.InsertOrUpdate<T>(ctx, new List<BaseModel>
			{
				modelData
			}, fields);
		}

		public static OperationResult InsertOrUpdate<T>(MContext ctx, List<T> modelData, List<string> fields = null) where T : BaseModel
		{
			List<BaseModel> list = new List<BaseModel>();
			foreach (T modelDatum in modelData)
			{
				list.Add((BaseModel)modelDatum);
			}
			return ModelInfoManager.InsertOrUpdate<T>(ctx, list, fields);
		}

		public static OperationResult InsertOrUpdate<T>(MContext ctx, List<BaseModel> modelData, List<string> fields = null) where T : BaseModel
		{
			OperationResult successVerifiationModelList = GetSuccessVerifiationModelList<T>(ctx, modelData, OperateTime.Save, null);
			List<BaseModel> successModel = successVerifiationModelList.SuccessModel;
			if (successModel != null && successModel.Count > 0)
			{
				try
				{
					MakeBizBillNumber(ctx, successModel);
					List<CommandInfo> insertOrUpdateCmd = GetInsertOrUpdateCmd<T>(ctx, successModel, fields);
					DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
					dynamicDbHelperMySQL.ExecuteSqlTran(insertOrUpdateCmd);
					successVerifiationModelList.SuccessModel = successModel;
					successVerifiationModelList.SuccessModelID = (from item in successModel
					select item.PKFieldValue).ToList();
					successVerifiationModelList.ObjectID = successModel[0].PKFieldValue;
					successVerifiationModelList.Message = "Success";
				}
				catch (Exception ex)
				{
					successVerifiationModelList.Message = "Fail";
					successVerifiationModelList.FailModel = modelData;
					successVerifiationModelList.FailModelID = (from item in modelData
					select item.PKFieldValue).ToList();
					successVerifiationModelList.ObjectID = successModel[0].PKFieldValue;
					successVerifiationModelList.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = ex.Message,
						CheckItem = "Save Data"
					});
					MLogger.Log("InsertOrUpdate<T>", ex, ctx);
				}
			}
			return successVerifiationModelList;
		}

		public static OperationResult GetSuccessVerifiationModelList<T>(MContext ctx, List<BaseModel> modelData, OperateTime opTime = OperateTime.Save, List<string> fields = null) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			OperationResult operationResult = new OperationResult();
			if (modelData == null || modelData.Count == 0)
			{
				return operationResult;
			}
			JugeInsertOrUpdate(ctx, modelData);
			SetModelDefaultValue(ctx, modelData);
			if (fields != null && fields.Count > 0)
			{
				return operationResult;
			}
			foreach (BaseModel modelDatum in modelData)
			{
				OperationResult operationResult2 = modelDatum.Verification(ctx, opTime);
				operationResult.VerificationInfor.AddRange((IEnumerable<BizVerificationInfor>)operationResult2.VerificationInfor);
				if (operationResult2.Success)
				{
					operationResult.SuccessModel.Add(modelDatum);
				}
				else
				{
					operationResult.FailModel.Add(modelDatum);
					operationResult.FailModelID.Add(modelDatum.PKFieldValue);
				}
			}
			return operationResult;
		}

		public static List<CommandInfo> GetAddEntryDeleteFlagCmd(MContext ctx, List<BaseModel> modelData)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<BaseModel> list2 = (from f in modelData
			where f.IsUpdate
			select f).ToList();
			if (list2 != null && list2.Count > 0)
			{
				MyPropertyInfo[] entryProperty = GetEntryProperty(list2[0].GetType());
				List<string> parentID = (from item in modelData
				select item.PKFieldValue).ToList();
				MyPropertyInfo[] array = entryProperty;
				foreach (MyPropertyInfo myPropertyInfo in array)
				{
					Type type = myPropertyInfo.Property.PropertyType.GetGenericArguments()[0];
					List<string> bizEntryDataPKID = GetBizEntryDataPKID(ctx, type, parentID);
					ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[type];
					foreach (BaseModel item in list2)
					{
						if (type.Name.ToLower().IndexOf("entry") >= 0)
						{
							IEnumerable enumerable = item.GetPropertyValue(myPropertyInfo.Property.Name) as IEnumerable;
							if (enumerable != null)
							{
								foreach (string item2 in bizEntryDataPKID)
								{
									bool flag = true;
									foreach (BaseModel item3 in enumerable)
									{
										if (item3.PKFieldValue.EqualsIgnoreCase(item2))
										{
											flag = false;
											break;
										}
									}
									if (flag)
									{
										MySqlParameter[] para = new MySqlParameter[4]
										{
											new MySqlParameter("@" + modelToMySqlInfor.MainTablePKFieldName, item2),
											new MySqlParameter("@MOrgID", ctx.MOrgID),
											new MySqlParameter("@MModifierID", ctx.MUserID),
											new MySqlParameter("@MModifyDate", ctx.DateNow)
										};
										string sqlText = string.Format("update {0} set MIsDelete = 1, MModifierID = @MModifierID, MModifyDate = @MModifyDate Where {1} = @{1} and MOrgID = @MOrgID and MIsDelete = 0", modelToMySqlInfor.MainTableName, modelToMySqlInfor.MainTablePKFieldName);
										list.Add(new CommandInfo(sqlText, para));
									}
								}
							}
						}
					}
				}
			}
			return list;
		}

		private static void SetModelDefaultValue(MContext ctx, List<BaseModel> modelData)
		{
			MyPropertyInfo[] entryProperty = GetEntryProperty(modelData[0].GetType());
			foreach (BaseModel modelDatum in modelData)
			{
				SetDefaultDate(ctx, modelDatum);
				if (entryProperty != null && entryProperty.Length != 0)
				{
					MyPropertyInfo[] array = entryProperty;
					foreach (MyPropertyInfo myPropertyInfo in array)
					{
						IEnumerable enumerable = modelDatum.GetPropertyValue(myPropertyInfo.Property.Name) as IEnumerable;
						if (enumerable != null)
						{
							foreach (BaseModel item in enumerable)
							{
								SetDefaultDate(ctx, item);
							}
						}
					}
				}
			}
		}

		private static void SetDefaultDate(MContext ctx, BaseModel data)
		{
			data.MModifyDate = ctx.DateNow;
			data.MModifierID = ctx.MUserID;
			if (data.IsNew)
			{
				data.MIsDelete = false;
				if (data.MCreateDate < new DateTime(1900, 1, 1))
				{
					data.MCreateDate = ctx.DateNow;
				}
				data.MCreatorID = ctx.MUserID;
			}
		}

		private static void JugeInsertOrUpdate(MContext ctx, List<BaseModel> modelData)
		{
			MyPropertyInfo[] entryProperty = GetEntryProperty(modelData[0].GetType());
			foreach (BaseModel modelDatum in modelData)
			{
				SetInsertOrUpdateStatus(modelDatum);
			}
			if (entryProperty != null && entryProperty.Length != 0)
			{
				JugeEntryInsertOrUpdate(modelData, entryProperty);
			}
		}

		private static void JugeEntryInsertOrUpdate(List<BaseModel> modelData, MyPropertyInfo[] myProperty)
		{
			if (myProperty != null && myProperty.Length != 0)
			{
				foreach (BaseModel modelDatum in modelData)
				{
					foreach (MyPropertyInfo myPropertyInfo in myProperty)
					{
						IEnumerable enumerable = modelDatum.GetPropertyValue(myPropertyInfo.Property.Name) as IEnumerable;
						if (enumerable != null)
						{
							foreach (BaseModel item in enumerable)
							{
								SetParentID(modelDatum, item);
								SetInsertOrUpdateStatus(item);
								if (item.IsNew && item.IsDelete)
								{
									item.IsNew = false;
									item.IsUpdate = false;
									item.IsDelete = true;
								}
							}
						}
					}
				}
			}
		}

		private static void SetInsertOrUpdateStatus(BaseModel model)
		{
			if (string.IsNullOrWhiteSpace(model.PKFieldValue))
			{
				model.IsNew = true;
				model.IsUpdate = false;
				model.SetPropertyValue(model.PKFieldName, UUIDHelper.GetGuid());
			}
			else if (!model.IsNew)
			{
				model.IsUpdate = true;
			}
		}

		private static void SetParentID(BaseModel parentData, BaseModel entryData)
		{
			if (entryData is BizEntryDataModel)
			{
				((BizEntryDataModel)entryData).MID = parentData.PKFieldValue;
			}
			else if (entryData is BDEntryModel)
			{
				((BDEntryModel)entryData).MItemID = parentData.PKFieldValue;
			}
			else if (entryData is BizDetailDataModel)
			{
				((BizDetailDataModel)entryData).MEntryID = parentData.PKFieldValue;
			}
			else if (entryData is BillAttachmentModel)
			{
				((BillAttachmentModel)entryData).MParentID = parentData.PKFieldValue;
			}
			else if (entryData is IOModel)
			{
				((IOModel)entryData).MParentID = parentData.PKFieldValue;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="successModel"></param>
		private static void MakeBizBillNumber(MContext ctx, List<BaseModel> successModel)
		{
			MyPropertyInfo[] source = modelInfoCache[successModel[0].GetType()];
			List<MyPropertyInfo> list = (from f in source
			where f.AutoMakeNo != null
			select f).ToList();
			if (list != null && list.Count != 0)
			{
				foreach (BaseModel item in successModel)
				{
					foreach (MyPropertyInfo item2 in list)
					{
						item2.AutoMakeNo.BizData = item;
						item2.AutoMakeNo.MContext = ctx;
						item2.AutoMakeNo.CurrentOperateContent = OperateTime.Save;
						item2.AutoMakeNo.Verification(ctx);
					}
				}
			}
		}

		public static List<CommandInfo> GetInsertOrUpdateCmd<T>(MContext ctx, BaseModel modelData, List<string> fields = null, bool updateEntry = true) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			OperationResult successVerifiationModelList = GetSuccessVerifiationModelList<T>(ctx, new List<BaseModel>
			{
				modelData
			}, OperateTime.Save, null);
			if (successVerifiationModelList.SuccessModel != null && successVerifiationModelList.SuccessModel.Count > 0)
			{
				List<CommandInfo> insertOrUpdateCmd = GetInsertOrUpdateCmd(ctx, successVerifiationModelList.SuccessModel[0], fields, updateEntry);
				if (updateEntry)
				{
					insertOrUpdateCmd.AddRange((IEnumerable<CommandInfo>)GetAddEntryDeleteFlagCmd(ctx, successVerifiationModelList.SuccessModel));
				}
				return insertOrUpdateCmd;
			}
			return new List<CommandInfo>();
		}

		public static List<CommandInfo> GetBatchInsertOrUpdateCmd<T>(MContext ctx, List<BaseModel> modelDataList, List<string> fields = null, string customizePrefix = null) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			OperationResult successVerifiationModelList = GetSuccessVerifiationModelList<T>(ctx, modelDataList, OperateTime.Save, null);
			if (successVerifiationModelList.SuccessModel != null && successVerifiationModelList.SuccessModel.Count > 0)
			{
				return GetBatchInsertOrUpdateCmd(ctx, successVerifiationModelList.SuccessModel, fields, customizePrefix);
			}
			return new List<CommandInfo>();
		}

		private static List<CommandInfo> GetInsertOrUpdateCmd(MContext ctx, BaseModel modelData, List<string> fields = null, bool updateEntry = true)
		{
			if (string.IsNullOrWhiteSpace(modelData.PKFieldValue))
			{
				JugeInsertOrUpdate(ctx, new List<BaseModel>
				{
					modelData
				});
			}
			else if (modelData.IsNew)
			{
				modelData.MIsDelete = false;
				modelData.MCreatorID = ctx.MUserID;
				if (modelData.MCreateDate <= new DateTime(1900, 1, 1))
				{
					modelData.MCreateDate = ctx.DateNow;
				}
			}
			List<CommandInfo> list = new List<CommandInfo>();
			Type type = modelData.GetType();
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[type];
			MyPropertyInfo[] array = modelInfoCache[type];
			List<MySqlParameter> list2 = new List<MySqlParameter>
			{
				new MySqlParameter("@AppSource", (!string.IsNullOrWhiteSpace(ctx.AppSource)) ? ctx.AppSource : "System")
			};
			if (ExistsColunm(modelData, "MOrgID"))
			{
				object propertyValue = modelData.GetPropertyValue("MOrgID");
				if (propertyValue == null || string.IsNullOrWhiteSpace(propertyValue.ToString()))
				{
					modelData.SetPropertyValue("MOrgID", ctx.MOrgID);
				}
			}
			if (modelData.IsNew && !modelData.IsDelete)
			{
				MyPropertyInfo[] array2 = array;
				foreach (MyPropertyInfo property in array2)
				{
					MySqlParameter sqlParameter = GetSqlParameter(modelData, property, true, null);
					list2.Add(sqlParameter);
				}
				list.Add(new CommandInfo(modelData.TableName, modelToMySqlInfor.MainTableInsertSql, list2.ToArray()));
			}
			else if (!modelData.IsDelete)
			{
				if (fields == null || fields.Count == 0)
				{
					MyPropertyInfo[] array3 = array;
					foreach (MyPropertyInfo property2 in array3)
					{
						MySqlParameter sqlParameter2 = GetSqlParameter(modelData, property2, true, null);
						list2.Add(sqlParameter2);
					}
					list.Add(new CommandInfo(modelData.TableName, modelToMySqlInfor.MainTableUpdateSql, list2.ToArray()));
				}
				else
				{
					string partialUpdateSql = GetPartialUpdateSql(type, fields);
					if (!string.IsNullOrWhiteSpace(partialUpdateSql))
					{
						MyPropertyInfo[] array4 = array;
						foreach (MyPropertyInfo item in array4)
						{
							if (fields.Any((string f) => f.EqualsIgnoreCase(item.Property.Name)))
							{
								MySqlParameter sqlParameter3 = GetSqlParameter(modelData, item, true, null);
								list2.Add(sqlParameter3);
							}
						}
						if (!list2.Any((MySqlParameter f) => f.ParameterName.EqualsIgnoreCase("@" + modelData.PKFieldName)))
						{
							MySqlParameter mySqlParameter = new MySqlParameter("@" + modelData.PKFieldName, MySqlDbType.VarChar, 36);
							mySqlParameter.Value = modelData.PKFieldValue;
							list2.Add(mySqlParameter);
						}
						list.Add(new CommandInfo(modelData.TableName, partialUpdateSql, list2.ToArray()));
					}
				}
			}
			list.AddRange(GetMultiInsertOrUpdateCmd(ctx, modelData, modelToMySqlInfor, fields));
			if (updateEntry)
			{
				list.AddRange(GetEntryInsertOrUpdateCmd(ctx, type, modelData));
			}
			return list;
		}

		private static List<CommandInfo> GetBatchInsertOrUpdateCmd(MContext ctx, List<BaseModel> modelDataList, List<string> fields = null, string customizePrefix = null)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			int num = 1;
			string text = "";
			string text2 = "";
			CommandInfo commandInfo = new CommandInfo();
			List<MySqlParameter> list2 = new List<MySqlParameter>
			{
				new MySqlParameter("@AppSource", (!string.IsNullOrWhiteSpace(ctx.AppSource)) ? ctx.AppSource : "System")
			};
			List<string> list3 = new List<string>();
			string text3 = "";
			CommandInfo commandInfo2 = new CommandInfo();
			List<MySqlParameter> list4 = new List<MySqlParameter>
			{
				new MySqlParameter("@AppSource", (!string.IsNullOrWhiteSpace(ctx.AppSource)) ? ctx.AppSource : "System")
			};
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string text4 = "";
			List<string> list5 = new List<string>();
			Dictionary<string, Regex> dictionary2 = new Dictionary<string, Regex>();
			Dictionary<string, Regex> dictionary3 = new Dictionary<string, Regex>();
			Regex regex = new Regex("values", RegexOptions.IgnoreCase);
			foreach (BaseModel modelData in modelDataList)
			{
				if (string.IsNullOrWhiteSpace(text3))
				{
					text3 = modelData.TableName;
				}
				if (string.IsNullOrWhiteSpace(text4))
				{
					text4 = modelData.PKFieldName;
				}
				if (string.IsNullOrWhiteSpace(modelData.PKFieldValue))
				{
					JugeInsertOrUpdate(ctx, new List<BaseModel>
					{
						modelData
					});
				}
				else if (modelData.IsNew)
				{
					modelData.MIsDelete = false;
					modelData.MCreatorID = ctx.MUserID;
					if (modelData.MCreateDate <= new DateTime(1900, 1, 1))
					{
						modelData.MCreateDate = ctx.DateNow;
					}
				}
				List<CommandInfo> list6 = new List<CommandInfo>();
				Type type = modelData.GetType();
				ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[type];
				MyPropertyInfo[] array = modelInfoCache[type];
				List<MySqlParameter> list7 = new List<MySqlParameter>();
				if (ExistsColunm(modelData, "MOrgID"))
				{
					object propertyValue = modelData.GetPropertyValue("MOrgID");
					if (propertyValue == null || string.IsNullOrWhiteSpace(propertyValue.ToString()))
					{
						modelData.SetPropertyValue("MOrgID", ctx.MOrgID);
					}
				}
				if (modelData.IsNew && !modelData.IsDelete)
				{
					string[] array2 = regex.Split(modelToMySqlInfor.MainTableInsertSql);
					string text5 = array2[0];
					string text6 = array2[1];
					if (string.IsNullOrWhiteSpace(text))
					{
						text = array2[0];
					}
					MyPropertyInfo[] array3 = array;
					foreach (MyPropertyInfo property in array3)
					{
						string text7 = num.ToString() + customizePrefix;
						MySqlParameter sqlParameter = GetSqlParameter(modelData, property, true, text7);
						list2.Add(sqlParameter);
						int length = sqlParameter.ParameterName.Length;
						int length2 = text7.Length;
						string str = sqlParameter.ParameterName.Substring(1, length - length2 - 1);
						string replacement = sqlParameter.ParameterName.Substring(1, length - 1);
						Regex regex2 = null;
						if (dictionary2.ContainsKey(str + "\\b"))
						{
							regex2 = dictionary2[str + "\\b"];
						}
						else
						{
							regex2 = new Regex(str + "\\b");
							dictionary2.Add(str + "\\b", regex2);
						}
						text6 = regex2.Replace(text6, replacement);
					}
					list3.Add(text6);
					if (string.IsNullOrWhiteSpace(text2))
					{
						text2 = modelData.TableName;
					}
				}
				else if (!modelData.IsDelete && (fields == null || fields.Count == 0))
				{
					MyPropertyInfo[] array4 = array;
					foreach (MyPropertyInfo property2 in array4)
					{
						string text8 = num.ToString();
						MySqlParameter sqlParameter2 = GetSqlParameter(modelData, property2, true, text8);
						list4.Add(sqlParameter2);
						int length3 = sqlParameter2.ParameterName.Length;
						int length4 = text8.Length;
						string text9 = sqlParameter2.ParameterName.Substring(1, length3 - length4 - 1);
						Regex regex3 = null;
						if (dictionary3.ContainsKey("\\b" + text9 + "\\b"))
						{
							regex3 = dictionary3["\\b" + text9 + "\\b"];
						}
						else
						{
							regex3 = new Regex("\\b" + text9 + "\\b");
							dictionary3.Add("\\b" + text9 + "\\b", regex3);
						}
						if (regex3.IsMatch(modelToMySqlInfor.MainTableUpdateSql))
						{
							string text10 = "@" + text4 + text8;
							string text11 = string.Format(" when {0} then {1}", text10, sqlParameter2.ParameterName);
							if (!list5.Contains(text10))
							{
								list5.Add(text10);
							}
							if (!dictionary.Keys.Contains(text9))
							{
								dictionary.Add(text9, text11);
							}
							else
							{
								string str2 = dictionary[text9];
								str2 = (dictionary[text9] = str2 + text11);
							}
						}
					}
				}
				num++;
				if (list3.Count() > 200)
				{
					CommandInfo batchInsertCommandInfo = GetBatchInsertCommandInfo(list3, text, text2, list2);
					if (batchInsertCommandInfo != null)
					{
						list.Add(batchInsertCommandInfo);
						list3.Clear();
						list2.Clear();
					}
				}
				if (list5.Count() > 200)
				{
					CommandInfo batchUpdateCommandInfo = GetBatchUpdateCommandInfo(dictionary, list5, text3, text4, list4);
					if (batchUpdateCommandInfo != null)
					{
						list.Add(batchUpdateCommandInfo);
						dictionary.Clear();
						list5.Clear();
						list4.Clear();
					}
				}
			}
			CommandInfo batchInsertCommandInfo2 = GetBatchInsertCommandInfo(list3, text, text2, list2);
			if (batchInsertCommandInfo2 != null)
			{
				list.Add(batchInsertCommandInfo2);
			}
			CommandInfo batchUpdateCommandInfo2 = GetBatchUpdateCommandInfo(dictionary, list5, text3, text4, list4);
			if (batchUpdateCommandInfo2 != null)
			{
				list.Add(batchUpdateCommandInfo2);
			}
			return list;
		}

		private static CommandInfo GetBatchUpdateCommandInfo(Dictionary<string, string> setFieldValueDic, List<string> pkIdList, string tableName, string pkFieldName, List<MySqlParameter> paramList)
		{
			CommandInfo result = null;
			string empty = string.Empty;
			if (setFieldValueDic != null && setFieldValueDic.Keys.Count() > 0)
			{
				string str = " set ";
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string key in setFieldValueDic.Keys)
				{
					stringBuilder.Append(string.Format(" {0} = case {1} {2} END,", key, pkFieldName, setFieldValueDic[key]));
				}
				str += stringBuilder.ToString();
				if (!string.IsNullOrWhiteSpace(str))
				{
					str = str.Substring(0, str.Length - 1);
					string text = string.Join(",", pkIdList);
					empty = string.Format(" update {0} {1} where {2} in({3})", tableName, str, pkFieldName, text);
					result = new CommandInfo(tableName, empty, paramList.ToArray());
				}
			}
			return result;
		}

		private static CommandInfo GetBatchInsertCommandInfo(List<string> valuesList, string insertSql, string tableName, List<MySqlParameter> paramList)
		{
			CommandInfo result = null;
			if (valuesList.Count() > 0)
			{
				string arg = string.Join(",", valuesList);
				insertSql = string.Format(" {0} values {1}", insertSql, arg);
				result = new CommandInfo(tableName, insertSql, paramList.ToArray());
			}
			return result;
		}

		private static string GetPartialUpdateSql(Type model, List<string> fields)
		{
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[model];
			MyPropertyInfo[] array = (from f in modelInfoCache[model]
			where f.HaveBDField
			select f).ToArray();
			if (array.Length == 0)
			{
				throw new Exception("Model Can't be Save");
			}
			List<string> list = new List<string>();
			MyPropertyInfo[] array2 = array;
			foreach (MyPropertyInfo item in array2)
			{
				if (!item.InsertOnly && fields.Any((string f) => f.EqualsIgnoreCase(item.Property.Name)))
				{
					if (item.IsEncrypt)
					{
						if (!string.IsNullOrWhiteSpace(item.AppSourceString))
						{
							list.Add(string.Format("{0} = ( case when @AppSource in ({2}) then AES_ENCRYPT(@{0},'{1}')  else {0} end )", item.Property.Name, "JieNor-001", item.AppSourceString));
						}
						else
						{
							list.Add(string.Format("{0} = AES_ENCRYPT(@{0},'{1}') ", item.Property.Name, "JieNor-001"));
						}
					}
					else if (!string.IsNullOrWhiteSpace(item.AppSourceString))
					{
						list.Add(string.Format("{0} = ( case when @AppSource in ({1}) then @{0}  else {0} end )", item.Property.Name, item.AppSourceString));
					}
					else
					{
						list.Add(string.Format("{0} = @{0}", item.Property.Name));
					}
				}
			}
			if (list.Count == 0)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder("Update " + modelToMySqlInfor.MainTableName + " Set ");
			stringBuilder.AppendLine(string.Join(",", list));
			stringBuilder.AppendLine(" Where " + modelToMySqlInfor.MainTablePKFieldName + " = @" + modelToMySqlInfor.MainTablePKFieldName);
			return stringBuilder.ToString();
		}

		private static List<CommandInfo> GetEntryInsertOrUpdateCmd(MContext ctx, Type parentModelType, BaseModel parentData)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MyPropertyInfo[] array = modelInfoCache[parentModelType];
			MyPropertyInfo[] array2 = array;
			foreach (MyPropertyInfo myPropertyInfo in array2)
			{
				if (myPropertyInfo.Property.PropertyType.IsGenericType)
				{
					Type type = myPropertyInfo.Property.PropertyType.GetGenericArguments()[0];
					if (type.IsSubclassOf(typeof(BaseModel)) && HasEntryAttribute(myPropertyInfo.Property))
					{
						IEnumerable enumerable = parentData.GetPropertyValue(myPropertyInfo.Property.Name) as IEnumerable;
						if (enumerable != null)
						{
							foreach (BaseModel item in enumerable)
							{
								string name = "";
								if (item is BizEntryDataModel)
								{
									name = "MID";
								}
								else if (item is BDEntryModel)
								{
									name = "MItemID";
								}
								else if (item is BizDetailDataModel)
								{
									name = "MEntryID";
								}
								else if (item is BillAttachmentModel)
								{
									name = "MParentID";
								}
								else if (item is IOModel)
								{
									name = "MParentID";
								}
								item.SetPropertyValue(name, parentData.PKFieldValue);
								if (ExistsColunm(item, "MOrgID"))
								{
									object propertyValue = parentData.GetPropertyValue("MOrgID");
									item.SetPropertyValue("MOrgID", (propertyValue != null && !string.IsNullOrWhiteSpace(propertyValue.ToString())) ? propertyValue.ToString() : ctx.MOrgID);
								}
								list.AddRange(GetInsertOrUpdateCmd(ctx, item, null, true));
							}
						}
					}
				}
			}
			return list;
		}

		public static List<CommandInfo> GetInsertOrUpdateCmd<T>(MContext ctx, List<BaseModel> modelData, List<string> fields = null) where T : BaseModel
		{
			if (modelData == null || modelData.Count == 0)
			{
				return new List<CommandInfo>();
			}
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (BaseModel modelDatum in modelData)
			{
				list.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetInsertOrUpdateCmd<T>(ctx, modelDatum, fields, true));
			}
			return list;
		}

		public static List<CommandInfo> GetInsertOrUpdateCmds<T>(MContext ctx, List<T> modelData, List<string> fields = null, bool updateEntry = true) where T : BaseModel
		{
			if (modelData == null || modelData.Count == 0)
			{
				return new List<CommandInfo>();
			}
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (T modelDatum in modelData)
			{
				list.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetInsertOrUpdateCmd<T>(ctx, (BaseModel)modelDatum, fields, updateEntry));
			}
			return list;
		}

		public static List<CommandInfo> GetBatchInsertOrUpdateCmds<T>(MContext ctx, List<T> modelData, List<string> fields = null, string customizePrefix = null) where T : BaseModel
		{
			if (modelData == null || modelData.Count == 0)
			{
				return new List<CommandInfo>();
			}
			List<BaseModel> list = new List<BaseModel>();
			foreach (T modelDatum in modelData)
			{
				list.Add((BaseModel)modelDatum);
			}
			return ModelInfoManager.GetBatchInsertOrUpdateCmd<T>(ctx, list, fields, customizePrefix);
		}

		private static List<string> GetBizEntryDataPKID(MContext ctx, Type entryType, List<string> parentID)
		{
			List<string> list = new List<string>();
			foreach (string item in parentID)
			{
				list.Add(string.Format("'{0}'", item.Replace("'", "")));
			}
			string text = "";
			if (entryType.IsSubclassOf(typeof(BizEntryDataModel)))
			{
				text = "MID";
			}
			else if (entryType.IsSubclassOf(typeof(BDEntryModel)))
			{
				text = "MItemID";
			}
			else if (entryType.IsSubclassOf(typeof(BDDetailModel)))
			{
				text = "MEntryID";
			}
			else if (entryType.IsSubclassOf(typeof(BizDetailDataModel)))
			{
				text = "MEntryID";
			}
			else if (entryType.IsSubclassOf(typeof(BillAttachmentModel)))
			{
				text = "MParentID";
			}
			else if (entryType.IsSubclassOf(typeof(IOModel)))
			{
				text = "MParentID";
			}
			if (text == "")
			{
				return list;
			}
			List<string> list2 = new List<string>();
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[entryType];
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			string text2 = string.Format("select {0} from {1} where {2} In ({3}) AND MIsDelete=0 ", modelToMySqlInfor.MainTablePKFieldName, modelToMySqlInfor.MainTableName, text, string.Join(",", list));
			DataSet dataSet = dynamicDbHelperMySQL.Query(text2.ToString());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					list2.Add(row[0].ToString());
				}
			}
			return list2;
		}

		public static List<T> GetDataModelBySql<T>(MContext ctx, string sql, params MySqlParameter[] cmdParms)
		{
			BaseModel baseModel = Activator.CreateInstance(typeof(T)) as BaseModel;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, cmdParms);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				return DataTableToList<T>(dataSet.Tables[0]);
			}
			return new List<T>();
		}

		public static DataGridJson<T> GetPageDataModelListBySql<T>(MContext ctx, SqlQuery sql)
		{
			if (sql == null)
			{
				sql = new SqlQuery();
			}
			if (string.IsNullOrWhiteSpace(sql.SqlWhere.OrderBySqlString))
			{
				sql.SqlWhere.OrderBy("MCreateDate Desc");
			}
			DataGridJson<T> dataGridJson = new DataGridJson<T>();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql.Sql, sql.Parameters);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				dataGridJson.rows = DataTableToList<T>(dataSet.Tables[0]);
			}
			else
			{
				dataGridJson.rows = new List<T>();
			}
			dataSet = dynamicDbHelperMySQL.Query(sql.CountSqlString, sql.Parameters);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				dataGridJson.total = Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
			}
			return dataGridJson;
		}

		public static DataGridJson<T> GetPageDataModelListBySql<T>(MContext ctx, string sql, MySqlParameter[] parameters, GetParam param)
		{
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = new SqlWhere
			{
				PageIndex = param.PageIndex,
				PageSize = param.PageSize
			};
			sqlQuery.SelectString = sql;
			if (!string.IsNullOrEmpty(param.OrderBy))
			{
				sqlQuery.OrderBy(param.OrderBy);
			}
			foreach (MySqlParameter para in parameters)
			{
				sqlQuery.AddParameter(para);
			}
			return GetPageDataModelListBySql<T>(ctx, sqlQuery);
		}

		public static DataGridJson<T> GetPageDataModelList<T>(MContext ctx, SqlWhere filter, bool includeDelete = false) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			if (filter == null)
			{
				filter = new SqlWhere();
			}
			if (!includeDelete)
			{
				filter.AddDeleteFilter("MIsDelete", SqlOperators.Equal, false);
			}
			DataGridJson<T> dataGridJson = new DataGridJson<T>();
			dataGridJson.rows = GetDataModelList<T>(ctx, filter, true, false);
			dataGridJson.total = GetDataModelListCount(ctx, typeof(T), filter);
			return dataGridJson;
		}

		public static List<T> GetDataModelList<T>(MContext ctx, SqlWhere filter, bool isPage = false, bool getEntry = false) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			List<T> list = new List<T>();
			List<BaseModel> dataModelList = GetDataModelList(ctx, typeof(T), filter, isPage, getEntry);
			foreach (BaseModel item in dataModelList)
			{
				list.Add(item as T);
			}
			return list;
		}

		public static List<T> GetDataModelList<T>(MContext ctx, SqlWhere filter, List<MyPropertyInfo> properties, bool isPage = false, bool getEntry = false) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			List<T> list = new List<T>();
			List<BaseModel> dataModelList = GetDataModelList(ctx, typeof(T), filter, properties, isPage, getEntry);
			foreach (BaseModel item in dataModelList)
			{
				list.Add(item as T);
			}
			return list;
		}

		public static List<T> GetDataModelList<T>(MContext ctx, List<string> keyIdList) where T : BaseModel
		{
			if (keyIdList == null || keyIdList.Count == 0)
			{
				return new List<T>();
			}
			keyIdList = (from t in keyIdList
			select t.Replace("'", "")).ToList();
			ModelInfoManager.BuildModelInfo<T>(ctx);
			List<T> list = new List<T>();
			Type typeFromHandle = typeof(T);
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[typeFromHandle];
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(modelToMySqlInfor.MainTableSelectSql);
			stringBuilder.AppendFormat("WHERE (");
			MySqlParameter[] array = new MySqlParameter[keyIdList.Count + 1];
			stringBuilder.AppendFormat("{0}=@{0}0 ", modelToMySqlInfor.MainTablePKFieldName);
			array[0] = new MySqlParameter(string.Format("@{0}0", modelToMySqlInfor.MainTablePKFieldName), keyIdList[0]);
			for (int i = 1; i < keyIdList.Count; i++)
			{
				string text = keyIdList[i];
				stringBuilder.AppendFormat(" OR {0}=@{0}{1} ", modelToMySqlInfor.MainTablePKFieldName, i);
				array[i] = new MySqlParameter(string.Format("@{0}{1}", modelToMySqlInfor.MainTablePKFieldName, i), keyIdList[i]);
			}
			stringBuilder.Append(" ) ");
			if (ExistsColunm(typeFromHandle, "MOrgID"))
			{
				stringBuilder.AppendFormat(" AND MOrgID =@MOrgID ");
			}
			array[keyIdList.Count] = new MySqlParameter("@MOrgID", ctx.MOrgID);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return new List<T>();
			}
			return DataTableToList<T>(dataSet);
		}

		public static List<T> GetDataModelList<T>(MContext ctx, string keyIds) where T : BaseModel
		{
			if (string.IsNullOrEmpty(keyIds))
			{
				return new List<T>();
			}
			List<string> keyIdList = keyIds.Split(',').ToList();
			return GetDataModelList<T>(ctx, keyIdList);
		}

		public static List<T> GetBizEntryDataEditModel<T>(MContext ctx, string pkID) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter("MID", SqlOperators.Equal, pkID);
			List<T> list = new List<T>();
			List<BaseModel> dataModelList = GetDataModelList(ctx, typeof(T), sqlWhere, false, false);
			foreach (BaseModel item in dataModelList)
			{
				list.Add(item as T);
			}
			return list;
		}

		public static List<T> GetBizAttachmentRelationModel<T>(MContext ctx, string pkID) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter("MParentID", SqlOperators.Equal, pkID);
			List<T> list = new List<T>();
			List<BaseModel> dataModelList = GetDataModelList(ctx, typeof(T), sqlWhere, false, false);
			foreach (BaseModel item in dataModelList)
			{
				list.Add(item as T);
			}
			return list;
		}

		public static int GetDataModelListCount(MContext ctx, Type modelType, SqlWhere filter)
		{
			if (filter == null)
			{
				filter = new SqlWhere();
			}
			if (ExistsColunm(modelType, "MOrgID") && filter.FilterString.IndexOf("MOrgID") == -1)
			{
				filter.AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
			}
			if (ExistsColunm(modelType, "MIsDelete") && filter.FilterString.IndexOf("MIsDelete") == -1)
			{
				filter.AddFilter("MIsDelete", SqlOperators.Equal, 0);
			}
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[modelType];
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			string text = string.Format("select count(1) As MRecordCount from {0} {1} ", modelToMySqlInfor.MainTableName, filter.WhereSqlString);
			DataSet dataSet = dynamicDbHelperMySQL.Query(text.ToString(), filter.Parameters);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return 0;
			}
			return Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
		}

		private static List<BaseModel> GetDataModelList(MContext ctx, Type modelType, SqlWhere filter, bool isPage = false, bool getEntry = false)
		{
			if (filter == null)
			{
				filter = new SqlWhere();
			}
			if (ExistsColunm(modelType, "MOrgID") && filter.FilterString.IndexOf("MOrgID") == -1)
			{
				filter.AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
			}
			if (ExistsColunm(modelType, "MIsDelete") && filter.FilterString.IndexOf("MIsDelete") == -1)
			{
				filter.AddFilter("MIsDelete", SqlOperators.Equal, 0);
			}
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[modelType];
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			string text = isPage ? string.Format("{0} {1} {2}", modelToMySqlInfor.MainTableSelectSql, filter.FilterAndOrderByString, filter.PageSqlString) : string.Format("{0} {1}", modelToMySqlInfor.MainTableSelectSql, filter.FilterAndOrderByString);
			DataSet dataSet = dynamicDbHelperMySQL.Query(text.ToString(), filter.Parameters);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return new List<BaseModel>();
			}
			DataSet dataSet2 = MultiLanguageFieldValue(ctx, modelToMySqlInfor, filter);
			List<BaseModel> emptyDataEditModel = GetEmptyDataEditModel(ctx, modelType, dataSet.Tables[0].Rows.Count);
			MyPropertyInfo[] plist = modelInfoCache[modelType];
			Dictionary<string, MyPropertyInfo> properInfoDic = GetProperInfoDic(plist, dataSet.Tables[0]);
			for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
			{
				BaseModel baseModel = emptyDataEditModel[i];
				SetModelValue(plist, dataSet.Tables[0].Columns, dataSet.Tables[0].Rows[i], baseModel, properInfoDic);
				if (dataSet2 != null && dataSet2.Tables.Count > 0 && dataSet2.Tables[0].Rows.Count > 0)
				{
					DataRow[] dataRows = dataSet2.Tables[0].Select("MParentID='" + baseModel.PKFieldValue + "'");
					FillModelMultiLanguageFieldValue(ctx, baseModel, dataSet2.Tables[0].Columns, dataRows);
				}
			}
			if (getEntry)
			{
				GetEntryModelData(ctx, modelType, emptyDataEditModel);
			}
			return emptyDataEditModel;
		}

		private static List<BaseModel> GetDataModelList(MContext ctx, Type modelType, SqlWhere filter, List<MyPropertyInfo> properties, bool isPage = false, bool getEntry = false)
		{
			if (filter == null)
			{
				filter = new SqlWhere();
			}
			if (ExistsColunm(modelType, "MOrgID") && filter.FilterString.IndexOf("MOrgID") == -1)
			{
				filter.AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
			}
			if (ExistsColunm(modelType, "MIsDelete") && filter.FilterString.IndexOf("MIsDelete") == -1)
			{
				filter.AddFilter("MIsDelete", SqlOperators.Equal, 0);
			}
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[modelType];
			string arg = modelToMySqlInfor.MainTableSelectSql;
			if (properties.Any())
			{
				arg = BuildCustomizeSearchSql(ctx, modelType, properties);
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			string text = isPage ? string.Format("{0} {1} {2}", arg, filter.FilterAndOrderByString, filter.PageSqlString) : string.Format("{0} {1}", arg, filter.FilterAndOrderByString);
			DataSet dataSet = dynamicDbHelperMySQL.Query(text.ToString(), filter.Parameters);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return new List<BaseModel>();
			}
			DataSet dataSet2 = MultiLanguageFieldValue(ctx, modelToMySqlInfor, filter);
			List<BaseModel> emptyDataEditModel = GetEmptyDataEditModel(ctx, modelType, dataSet.Tables[0].Rows.Count);
			MyPropertyInfo[] plist = modelInfoCache[modelType];
			Dictionary<string, MyPropertyInfo> properInfoDic = GetProperInfoDic(plist, dataSet.Tables[0]);
			for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
			{
				BaseModel baseModel = emptyDataEditModel[i];
				SetModelValue(plist, dataSet.Tables[0].Columns, dataSet.Tables[0].Rows[i], baseModel, properInfoDic);
				if (dataSet2 != null && dataSet2.Tables.Count > 0 && dataSet2.Tables[0].Rows.Count > 0)
				{
					DataRow[] dataRows = dataSet2.Tables[0].Select("MParentID='" + baseModel.PKFieldValue + "'");
					FillModelMultiLanguageFieldValue(ctx, baseModel, dataSet2.Tables[0].Columns, dataRows);
				}
			}
			if (getEntry)
			{
				GetEntryModelData(ctx, modelType, emptyDataEditModel);
			}
			return emptyDataEditModel;
		}

		private static void GetEntryModelData(MContext ctx, Type entryModelType, List<BaseModel> parentDatas)
		{
			MyPropertyInfo[] array = modelInfoCache[entryModelType];
			MyPropertyInfo[] array2 = array;
			foreach (MyPropertyInfo myPropertyInfo in array2)
			{
				if (myPropertyInfo.Property.PropertyType.IsGenericType)
				{
					Type type = myPropertyInfo.Property.PropertyType.GetGenericArguments()[0];
					if (type.IsSubclassOf(typeof(BaseModel)) && HasEntryAttribute(myPropertyInfo.Property))
					{
						List<BaseModel> entryDataMode = GetEntryDataMode(ctx, type, parentDatas);
						if (entryDataMode != null)
						{
							foreach (BaseModel parentData in parentDatas)
							{
								List<BaseModel> entryRows = GetEntryRows(type, entryDataMode, parentData);
								Type type2 = typeof(List<>).MakeGenericType(type);
								object obj = Activator.CreateInstance(type2);
								MethodInfo method = type2.GetMethod("Add");
								foreach (BaseModel item in entryRows)
								{
									method.Invoke(obj, new object[1]
									{
										item
									});
								}
								parentData.SetPropertyValue(myPropertyInfo.Property.Name, obj);
							}
						}
					}
				}
			}
		}

		private static List<BaseModel> GetEntryRows(Type entryModel, List<BaseModel> lstEntry, BaseModel parent)
		{
			List<BaseModel> list = new List<BaseModel>();
			foreach (BaseModel item in lstEntry)
			{
				if (entryModel.IsSubclassOf(typeof(BizEntryDataModel)) && ((BizEntryDataModel)item).MID == parent.PKFieldValue)
				{
					list.Add(item);
				}
				else if (entryModel.IsSubclassOf(typeof(BDEntryModel)) && ((BDEntryModel)item).MItemID == parent.PKFieldValue)
				{
					list.Add(item);
				}
				else if (entryModel.IsSubclassOf(typeof(BizDetailDataModel)) && ((BizDetailDataModel)item).MEntryID == parent.PKFieldValue)
				{
					list.Add(item);
				}
				else if (entryModel.IsSubclassOf(typeof(BillAttachmentModel)) && ((BillAttachmentModel)item).MParentID == parent.PKFieldValue)
				{
					list.Add(item);
				}
			}
			return list;
		}

		private static List<BaseModel> GetEntryDataMode(MContext ctx, Type entryModelType, List<BaseModel> parentDatas)
		{
			List<BaseModel> list = new List<BaseModel>();
			SqlWhere sqlWhere = new SqlWhere();
			string[] values = (from item in parentDatas
			select item.PKFieldValue).ToArray();
			if (entryModelType.IsSubclassOf(typeof(BizEntryDataModel)))
			{
				sqlWhere.AddFilter("MID", SqlOperators.In, values);
			}
			else if (entryModelType.IsSubclassOf(typeof(BDEntryModel)))
			{
				sqlWhere.AddFilter("MItemID", SqlOperators.In, values);
			}
			else if (entryModelType.IsSubclassOf(typeof(BizDetailDataModel)))
			{
				sqlWhere.AddFilter("MEntryID", SqlOperators.In, values);
			}
			else if (entryModelType.IsSubclassOf(typeof(BillAttachmentModel)))
			{
				sqlWhere.AddFilter("MParentID", SqlOperators.In, values);
			}
			if (ExistsColunm(entryModelType, "MSeq"))
			{
				sqlWhere.AddOrderBy("MSeq", SqlOrderDir.Asc);
			}
			return GetDataModelList(ctx, entryModelType, sqlWhere, false, false);
		}

		private static DataSet MultiLanguageFieldValue(MContext ctx, ModelToMySqlInfor sqlInfor, SqlWhere filter)
		{
			DataSet result = null;
			if (sqlInfor.HaveMultiLangTable)
			{
				string text = string.IsNullOrEmpty(filter.FilterAndOrderByString) ? " where " : string.Format(" {0} AND ", filter.FilterAndOrderByString);
				string text2 = string.Format(" {0} a\r\n                                          Where exists (select 1 from {1} b {2} a.MParentID=b.{3} ) ", sqlInfor.MultiLangTableSelectSql, sqlInfor.MainTableName, text, sqlInfor.MainTablePKFieldName);
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				result = dynamicDbHelperMySQL.Query(text2.ToString(), filter.Parameters);
			}
			return result;
		}

		private static Dictionary<string, MyPropertyInfo> GetProperInfoDic(MyPropertyInfo[] plist, DataTable dt)
		{
			Dictionary<string, MyPropertyInfo> dictionary = new Dictionary<string, MyPropertyInfo>();
			foreach (DataColumn column in dt.Columns)
			{
				MyPropertyInfo value = plist.FirstOrDefault((MyPropertyInfo f) => f.Property.Name.EqualsIgnoreCase(column.ColumnName));
				dictionary.Add(column.ColumnName, value);
			}
			return dictionary;
		}

		public static T GetEmptyDataEditModel<T>(MContext ctx) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			return GetEmptyDataEditModel(ctx, typeof(T)) as T;
		}

		public static List<T> GetEmptyDataEditModel<T>(MContext ctx, int count = 1) where T : BaseModel
		{
			if (count < 1)
			{
				throw new Exception("Error Paramenter");
			}
			List<T> list = new List<T>();
			List<BaseModel> emptyDataEditModel = GetEmptyDataEditModel(ctx, typeof(T), count);
			foreach (BaseModel item in emptyDataEditModel)
			{
				list.Add(item as T);
			}
			return list;
		}

		public static T GetDataModel<T>(MContext ctx, string sql, params MySqlParameter[] cmdParms) where T : new()
		{
			BaseModel baseModel = Activator.CreateInstance(typeof(T)) as BaseModel;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, cmdParms);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				List<T> list = DataTableToList<T>(dataSet.Tables[0]);
				if (list != null && list.Count > 0)
				{
					return list[0];
				}
			}
			return new T();
		}

		public static T GetDataEditModel<T>(MContext ctx, string pkID, bool includeDelete = false, bool isFilterByOrgId = true) where T : BaseModel
		{
			ModelInfoManager.BuildModelInfo<T>(ctx);
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[typeof(T)];
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal(modelToMySqlInfor.MainTablePKFieldName, pkID);
			if (isFilterByOrgId && ExistsColunm(typeof(T), "MOrgID"))
			{
				sqlWhere.AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
			}
			if (!includeDelete && ExistsColunm(typeof(T), "MIsDelete"))
			{
				sqlWhere.AddDeleteFilter("MIsDelete", SqlOperators.Equal, false);
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(modelToMySqlInfor.MainTableSelectSql + sqlWhere.WhereSqlString, sqlWhere.Parameters);
			T val = null;
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				List<T> list = DataTableToList<T>(dataSet.Tables[0], 1, false);
				if (list != null && list.Count > 0)
				{
					val = list[0];
					if (modelToMySqlInfor.HaveMultiLangTable)
					{
						GetMultiFieldLst(ctx, val, modelToMySqlInfor.MultiCols);
						GetDataEditModel_L(ctx, val, pkID, isFilterByOrgId);
					}
					GetEntryDataModel(ctx, pkID, val);
				}
			}
			return val;
		}

		private static void GetEntryDataModel<T>(MContext ctx, string pkID, T parentModel) where T : BaseModel
		{
			MyPropertyInfo[] array = modelInfoCache[typeof(T)];
			MyPropertyInfo[] array2 = array;
			foreach (MyPropertyInfo myPropertyInfo in array2)
			{
				if (myPropertyInfo.Property.PropertyType.IsGenericType)
				{
					Type type = myPropertyInfo.Property.PropertyType.GetGenericArguments()[0];
					if (type.IsSubclassOf(typeof(BaseModel)) && HasEntryAttribute(myPropertyInfo.Property))
					{
						List<BaseModel> entryDataMode = GetEntryDataMode(ctx, type, new List<BaseModel>
						{
							(BaseModel)parentModel
						});
						if (entryDataMode != null)
						{
							Type type2 = typeof(List<>).MakeGenericType(type);
							object obj = Activator.CreateInstance(type2);
							MethodInfo method = type2.GetMethod("Add");
							foreach (BaseModel item in entryDataMode)
							{
								method.Invoke(obj, new object[1]
								{
									item
								});
							}
							parentModel.SetPropertyValue(myPropertyInfo.Property.Name, obj);
						}
					}
				}
			}
		}

		private static List<BaseModel> GetEmptyDataEditModel(MContext ctx, Type modelType, int count = 1)
		{
			if (count < 1)
			{
				throw new Exception("Error Paramenter");
			}
			List<BaseModel> list = new List<BaseModel>();
			for (int i = 0; i < count; i++)
			{
				BaseModel emptyDataEditModel = GetEmptyDataEditModel(ctx, modelType);
				list.Add(emptyDataEditModel);
			}
			return list;
		}

		private static BaseModel GetEmptyDataEditModel(MContext ctx, Type modelType)
		{
			BaseModel baseModel = Activator.CreateInstance(modelType) as BaseModel;
			ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[modelType];
			if (!modelToMySqlInfor.HaveMultiLangTable || modelToMySqlInfor.MultiCols == null || modelToMySqlInfor.MultiCols.Count == 0)
			{
				return baseModel;
			}
			GetMultiFieldLst(ctx, baseModel, modelToMySqlInfor.MultiCols);
			return baseModel;
		}

		private static void GetDataEditModel_L<T>(MContext ctx, T model, string pkID, bool isFilterByOrgId = true) where T : BaseModel
		{
			string tableName = model.TableName + "_L";
			if (IsExistsTable(ctx, tableName))
			{
				Type typeFromHandle = typeof(T);
				Type type = model.GetType();
				ModelToMySqlInfor modelToMySqlInfor = modelSqlCache[typeFromHandle];
				if (!string.IsNullOrEmpty(modelToMySqlInfor.MultiLangTableSelectSql))
				{
					SqlWhere sqlWhere = new SqlWhere();
					sqlWhere.AddFilter("MParentID", SqlOperators.Equal, pkID).AddFilter("MIsDelete", SqlOperators.Equal, 0);
					if (isFilterByOrgId && ExistsColunm(type, "MOrgID"))
					{
						sqlWhere.AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
					}
					DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(modelToMySqlInfor.MultiLangTableSelectSql + sqlWhere.WhereSqlString, sqlWhere.Parameters);
					if (dataSet != null && dataSet.Tables.Count != 0 && dataSet.Tables[0].Rows.Count != 0)
					{
						FillModelMultiLanguageFieldValue(ctx, model, dataSet.Tables[0].Columns, dataSet.Tables[0].Select());
					}
				}
			}
		}

		public static MyPropertyInfo[] GetBizModelProperty(MContext ctx, BaseModel model)
		{
			if (!modelInfoCache.ContainsKey(model.GetType()))
			{
				BuildEditModelInfo(ctx, model.GetType());
			}
			return modelInfoCache[model.GetType()];
		}

		public static List<IBizVerificationRule> GetBizVerificationRule(MContext ctx, BaseModel model)
		{
			if (!modelInfoCache.ContainsKey(model.GetType()))
			{
				BuildEditModelInfo(ctx, model.GetType());
			}
			MyPropertyInfo[] array = modelInfoCache[model.GetType()];
			List<IBizVerificationRule> list = new List<IBizVerificationRule>();
			MyPropertyInfo[] array2 = array;
			foreach (MyPropertyInfo myPropertyInfo in array2)
			{
				if (myPropertyInfo.VerificationRules != null && myPropertyInfo.VerificationRules.Count != 0)
				{
					list.AddRange(myPropertyInfo.VerificationRules);
				}
			}
			return list;
		}

		private static void GetFieldCustomerRule(MyPropertyInfo myProperty, PropertyInfo pt)
		{
			InsertOnlyAttribute customAttribute = ((MemberInfo)pt).GetCustomAttribute<InsertOnlyAttribute>();
			if (customAttribute != null)
			{
				myProperty.InsertOnly = true;
			}
			AppSourceAttribute customAttribute2 = ((MemberInfo)pt).GetCustomAttribute<AppSourceAttribute>();
			if (customAttribute2 != null)
			{
				myProperty.AppSourceList = customAttribute2.AppSourceList;
			}
		}

		private static void GetBizVerificationRule(MyPropertyInfo myProperty, PropertyInfo pt)
		{
			GetRuleEmail(myProperty, pt);
			GetRuleHttp(myProperty, pt);
			GetRuleMustInput(myProperty, pt);
			GetRuleRangeDate(myProperty, pt);
			GetRuleRangeNumber(myProperty, pt);
			GetRuleRepeat(myProperty, pt);
			GetRuleCompare(myProperty, pt);
		}

		private static void GetRuleCompare(MyPropertyInfo myProperty, PropertyInfo pt)
		{
			CompareAttribute customAttribute = ((MemberInfo)pt).GetCustomAttribute<CompareAttribute>();
			if (customAttribute != null)
			{
				BizRuleCompare bizRuleCompare = new BizRuleCompare(customAttribute.Express, pt.Name, customAttribute.PropertyDesc);
				bizRuleCompare.OperateContent = customAttribute.OperateContext;
				myProperty.VerificationRules.Add(bizRuleCompare);
			}
		}

		private static void GetRuleRepeat(MyPropertyInfo myProperty, PropertyInfo pt)
		{
			RepeatCheckAttribute customAttribute = ((MemberInfo)pt).GetCustomAttribute<RepeatCheckAttribute>();
			if (customAttribute != null)
			{
				BizRuleRepeatCheck bizRuleRepeatCheck = new BizRuleRepeatCheck(pt.Name, customAttribute.Properties, customAttribute.PropertyDesc, true);
				bizRuleRepeatCheck.OperateContent = customAttribute.OperateContext;
				if (pt.PropertyType.IsGenericType && HasEntryAttribute(pt) && pt.PropertyType.GetGenericArguments()[0].GetTypeInfo().IsSubclassOf(typeof(BaseModel)))
				{
					bizRuleRepeatCheck.IsBillHead = false;
				}
				myProperty.VerificationRules.Add(bizRuleRepeatCheck);
			}
		}

		private static void GetRuleHttp(MyPropertyInfo myProperty, PropertyInfo pt)
		{
			HttpAttribute customAttribute = ((MemberInfo)pt).GetCustomAttribute<HttpAttribute>();
			if (customAttribute != null)
			{
				BizRuleHttp bizRuleHttp = new BizRuleHttp(pt.Name, customAttribute.PropertyDesc);
				bizRuleHttp.OperateContent = customAttribute.OperateContent;
				myProperty.VerificationRules.Add(bizRuleHttp);
			}
		}

		private static void GetRuleAutoMakeNo(Type model, List<MyPropertyInfo> properties)
		{
			IEnumerable<AutoBillNoAttribute> customAttributes = ((MemberInfo)model).GetCustomAttributes<AutoBillNoAttribute>(true);
			if (customAttributes != null && customAttributes.Count() != 0)
			{
				foreach (AutoBillNoAttribute item in customAttributes)
				{
					BizRuleAutoMakeNo bizRuleAutoMakeNo = new BizRuleAutoMakeNo(item.PropertyName, item.PropertyDesc);
					bizRuleAutoMakeNo.OperateContent = item.OperateContext;
					MyPropertyInfo myPropertyInfo = properties.FirstOrDefault((MyPropertyInfo f) => f.Property.Name.EqualsIgnoreCase(item.PropertyName));
					if (myPropertyInfo != null)
					{
						myPropertyInfo.AutoMakeNo = bizRuleAutoMakeNo;
					}
				}
			}
		}

		private static void GetRuleRangeDate(MyPropertyInfo myProperty, PropertyInfo pt)
		{
			DateRangeAttribute customAttribute = ((MemberInfo)pt).GetCustomAttribute<DateRangeAttribute>();
			if (customAttribute != null)
			{
				BizRuleRangeDate bizRuleRangeDate = new BizRuleRangeDate(pt.Name, customAttribute.PropertyDesc);
				bizRuleRangeDate.MaxValue = customAttribute.MaxValue;
				bizRuleRangeDate.MinValue = customAttribute.MinValue;
				bizRuleRangeDate.IncludeMaxValue = customAttribute.IncludeMaxValue;
				bizRuleRangeDate.IncludeMinValue = customAttribute.IncludeMinValue;
				bizRuleRangeDate.OperateContent = customAttribute.OperateContent;
				myProperty.VerificationRules.Add(bizRuleRangeDate);
			}
		}

		private static void GetRuleEmail(MyPropertyInfo myProperty, PropertyInfo pt)
		{
			EmailAttribute customAttribute = ((MemberInfo)pt).GetCustomAttribute<EmailAttribute>();
			if (customAttribute != null)
			{
				BizRuleEmail bizRuleEmail = new BizRuleEmail(pt.Name, customAttribute.PropertyDesc);
				bizRuleEmail.OperateContent = customAttribute.OperateContent;
				myProperty.VerificationRules.Add(bizRuleEmail);
			}
		}

		private static void GetRuleMustInput(MyPropertyInfo myProperty, PropertyInfo pt)
		{
			MustInputAttribute customAttribute = ((MemberInfo)pt).GetCustomAttribute<MustInputAttribute>();
			if (customAttribute != null)
			{
				if (pt.PropertyType.IsGenericType && pt.PropertyType.GetGenericArguments()[0].GetTypeInfo().IsSubclassOf(typeof(BaseModel)) && HasEntryAttribute(pt))
				{
					BizRuleMustInputEntry bizRuleMustInputEntry = new BizRuleMustInputEntry(pt.Name, customAttribute.PropertyDesc);
					bizRuleMustInputEntry.OperateContent = customAttribute.OperateContent;
					myProperty.VerificationRules.Add(bizRuleMustInputEntry);
				}
				else
				{
					BizRuleMustInput bizRuleMustInput = new BizRuleMustInput(pt.Name, customAttribute.PropertyDesc);
					bizRuleMustInput.OperateContent = customAttribute.OperateContent;
					myProperty.VerificationRules.Add(bizRuleMustInput);
				}
			}
		}

		private static void GetRuleRangeNumber(MyPropertyInfo myProperty, PropertyInfo pt)
		{
			NumberRangeAttribute customAttribute = ((MemberInfo)pt).GetCustomAttribute<NumberRangeAttribute>();
			if (customAttribute != null)
			{
				BizRuleRangeNumber bizRuleRangeNumber = new BizRuleRangeNumber(pt.Name, customAttribute.PropertyDesc);
				bizRuleRangeNumber.MaxValue = customAttribute.MaxValue;
				bizRuleRangeNumber.MinValue = customAttribute.MinValue;
				bizRuleRangeNumber.IncludeMaxValue = customAttribute.IncludeMaxValue;
				bizRuleRangeNumber.IncludeMinValue = customAttribute.IncludeMinValue;
				bizRuleRangeNumber.OperateContent = customAttribute.OperateContent;
				myProperty.VerificationRules.Add(bizRuleRangeNumber);
			}
		}
	}
}
