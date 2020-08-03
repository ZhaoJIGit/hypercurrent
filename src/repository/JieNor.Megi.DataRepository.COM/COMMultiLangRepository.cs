using JieNor.Megi.Common.Cache;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JieNor.Megi.DataRepository.COM
{
	public class COMMultiLangRepository
	{
		private static string stringCacheName = "stringCacheName";

		private static string isUpdateLang = ConfigurationManager.AppSettings["UpdateLang"];

		private static string intCacheName = "intCacheName";

		private static List<KeyValuePair<string, Hashtable>> StringKeyTableList
		{
			get
			{
				object obj = HttpRuntimeCaches.Get(stringCacheName);
				if (obj == null || !(obj is List<KeyValuePair<string, Hashtable>>) || (obj as List<KeyValuePair<string, Hashtable>>).Count == 0)
				{
					obj = GetStringKeyLangTableList(null);
					HttpRuntimeCaches.Set(stringCacheName, obj, 1440);
				}
				return obj as List<KeyValuePair<string, Hashtable>>;
			}
			set
			{
				HttpRuntimeCaches.Set(stringCacheName, value);
			}
		}

		private static List<KeyValuePair<string, Hashtable>> IntKeyTableList
		{
			get
			{
				object obj = HttpRuntimeCaches.Get(intCacheName);
				if (obj == null || !(obj is List<KeyValuePair<string, Hashtable>>) || (obj as List<KeyValuePair<string, Hashtable>>).Count == 0)
				{
					obj = GetIntKeyLangTableList(null);
					HttpRuntimeCaches.Set(intCacheName, obj, 1440);
				}
				return obj as List<KeyValuePair<string, Hashtable>>;
			}
			set
			{
				HttpRuntimeCaches.Set(intCacheName, value);
			}
		}

		public static string[] MegiLangTypes
		{
			get
			{
				string text = ConfigurationManager.AppSettings["MegiLangTypes"];
				if (string.IsNullOrEmpty(text))
				{
					return new string[3]
					{
						"0x0009",
						"0x7804",
						"0x7C04"
					};
				}
				return text.Split(',').ToArray();
			}
			set
			{
			}
		}

		public static void UpdateLang(LangModule module, string key, string defaultValue, int type)
		{
			int existsItemID = GetExistsItemID(module, key, type);
			if (existsItemID == 0)
			{
				existsItemID = GetNewItemID();
				AddLang(module, key, defaultValue, type);
			}
			else
			{
				UpdateLang(existsItemID, defaultValue);
			}
		}

		private static void UpdateLang(int itemId, string defaultValue)
		{
			if (!string.IsNullOrWhiteSpace(defaultValue))
			{
				string value = "0x0009";
				string pattern = "[一-龥]+";
				if (defaultValue != null && Regex.IsMatch(defaultValue, pattern))
				{
					value = "0x7804";
				}
				string sQLString = "UPDATE T_Bas_Lang_L SET MName=@MValue WHERE MParentID=@MParentID and MIsDelete = 0 and MIsActive = 1 AND MLocaleID= @MLocaleID";
				MySqlParameter[] cmdParms = new MySqlParameter[3]
				{
					new MySqlParameter("@MParentID", itemId),
					new MySqlParameter("@MValue", defaultValue),
					new MySqlParameter("@MLocaleID", value)
				};
				DbHelperMySQL.ExecuteSql(new MContext(), sQLString, cmdParms);
			}
		}

		private static int GetExistsItemID(LangModule module, string key, int type)
		{
			string sQLString = "SELECT distinct a.MItemID FROM T_Bas_Lang a inner join T_bas_lang_l b on a.MItemID = b.MParentID and b.MIsDelete = 0 and b.MIsActive = 1 WHERE a.MModuleID=@MModuleID and a.MIsDelete = 0 and a.MIsActive = 1 AND lower(a.MName)=@MName AND MType=@MType ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MModuleID", Convert.ToInt32(module)),
				new MySqlParameter("@MName", key.ToLower()),
				new MySqlParameter("@MType", type)
			};
			DataSet dataSet = DbHelperMySQL.Query(sQLString, cmdParms);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return 0;
			}
			return Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
		}

		private static int GetNewItemID()
		{
			string sQLString = "SELECT MAX(MItemID) FROM T_Bas_Lang a inner join t_bas_lang_l b on a.MItemID = b.MParentID and b.MIsDelete = 0 and b.MIsActive = 1 where a.MIsDelete = 0 and a.MIsActive = 1";
			DataSet dataSet = DbHelperMySQL.Query(sQLString);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return 1;
			}
			object obj = dataSet.Tables[0].Rows[0][0];
			if (obj == DBNull.Value)
			{
				return 1;
			}
			return Convert.ToInt32(obj) + 1;
		}

		public static List<KeyValuePair<string, Hashtable>> GetStringKeyLangTableList(string[] localeIds = null)
		{
			localeIds = ((localeIds == null || localeIds.Length == 0) ? MegiLangTypes : localeIds);
			List<KeyValuePair<string, Hashtable>> list = new List<KeyValuePair<string, Hashtable>>();
			string[] array = localeIds;
			foreach (string text in array)
			{
				list.Add(new KeyValuePair<string, Hashtable>(text, GetStringKeyLangTable(text)));
			}
			return list;
		}

		public static List<KeyValuePair<string, Hashtable>> GetIntKeyLangTableList(string[] localeIds = null)
		{
			localeIds = ((localeIds == null || localeIds.Length == 0) ? MegiLangTypes : localeIds);
			List<KeyValuePair<string, Hashtable>> list = new List<KeyValuePair<string, Hashtable>>();
			string[] array = localeIds;
			foreach (string text in array)
			{
				list.Add(new KeyValuePair<string, Hashtable>(text, GetIntKeyLangTable(text)));
			}
			return list;
		}

		private static Hashtable GetStringKeyLangTable(string localeId)
		{
			string text = "\r\n                select distinct concat(a.MModuleID , '.' ,lower(a.MName)) as keyText , b.MName as valueText from \r\n                 t_bas_lang  a\r\n                 inner join \r\n                 t_bas_lang_l b\r\n                 on \r\n                 b.MParentID=a.MItemID\r\n                 and b.MLocaleID=@MLocaleID and b.MIsDelete = 0 and b.MIsActive = 1\r\n                 where  a.MIsDelete = 0 and a.MIsActive = 1 and a.MType = 1 and a.MName is not null and LENGTH(trim(a.MName))>= 1";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MLocaleID", localeId)
			};
			DataSet dataSet = DbHelperMySQL.Query(text.ToString(), cmdParms);
			return DataTableToHashtable(dataSet.Tables[0]);
		}

		private static Hashtable GetIntKeyLangTable(string localeId)
		{
			string text = "select distinct MItemID as keyText ,b.MName as valueText from \r\n                 t_bas_lang  a\r\n                 inner join \r\n                 t_bas_lang_l b\r\n                 on \r\n                 b.MParentID=a.MItemID\r\n                 and b.MLocaleID=@MLocaleID and b.MIsDelete = 0 and b.MIsActive = 1\r\n                where a.MType=1 and a.MIsDelete = 0 and a.MIsActive = 1";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MLocaleID", localeId)
			};
			DataSet dataSet = DbHelperMySQL.Query(text.ToString(), cmdParms);
			return DataTableToHashtable(dataSet.Tables[0]);
		}

		private static Hashtable DataTableToHashtable(DataTable dt)
		{
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string text = dt.Rows[i]["keyText"].ToString().ToLower();
				if (!string.IsNullOrEmpty(text) && !hashtable.Contains(text))
				{
					hashtable.Add(text, dt.Rows[i]["valueText"].ToString());
				}
			}
			return hashtable;
		}

		private static string GetTextFromDB(LangModule module, string localeId, string key)
		{
			string sQLString = "select b.MName from T_Bas_Lang a\r\n                            inner join T_Bas_Lang_L b ON a.MItemID=b.MParentID and b.MIsActive = 1 and b.MIsDelete = 0  AND b.MLocaleID=@MLocaleID\r\n                            WHERE a.MModuleID = @MModuleID AND lower(a.MName) = @MName AND a.MType = 1 and a.MIsDelete = 0 and a.MIsActive = 1";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MModuleID", Convert.ToInt32(module)),
				new MySqlParameter("@MName", key.ToLower()),
				new MySqlParameter("@MLocaleID", localeId)
			};
			object single = DbHelperMySQL.GetSingle(sQLString, cmdParms);
			return (single == null) ? string.Empty : single.ToString();
		}

		public static string GetText(string localeId, LangModule module, string key, string defaultValue)
		{
			string text = GetText(module, localeId, key.ToLower());
			if (string.IsNullOrEmpty(text))
			{
				text = defaultValue;
				if (!string.IsNullOrEmpty(isUpdateLang) && isUpdateLang.ToLower() == "true")
				{
					UpdateLang(module, key, defaultValue, 1);
				}
			}
			return text;
		}

		public static string GetText(LangModule module, string localeId, string key)
		{
			Hashtable hashTable = GetHashTable(StringKeyTableList, localeId);
			if (hashTable == null || hashTable.Count == 0)
			{
				return string.Empty;
			}
			object obj = hashTable[Convert.ToInt32(module) + "." + key.ToLower()];
			if (obj == null || obj.ToString() == "")
			{
				obj = GetTextFromDB(module, localeId, key);
			}
			return (obj == null) ? string.Empty : obj.ToString();
		}

		public static Dictionary<string, string> GetAllText(string[] localeIds, COMLangInfoModel[] langInfoList)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in localeIds)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (COMLangInfoModel cOMLangInfoModel in langInfoList)
				{
					string value = GetText(cOMLangInfoModel.Module, text, cOMLangInfoModel.Key.ToLower());
					if (string.IsNullOrEmpty(value))
					{
						value = cOMLangInfoModel.DefaultValue;
					}
					stringBuilder.Append(value);
					if (!string.IsNullOrEmpty(cOMLangInfoModel.Comment))
					{
						stringBuilder.Append(cOMLangInfoModel.Comment);
					}
				}
				dictionary.Add(text, stringBuilder.ToString());
			}
			return dictionary;
		}

		public static string GetTextFormat(string localeId, LangModule module, string key, string defaultValue, params object[] args)
		{
			string text = GetText(localeId, module, key, defaultValue);
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			return string.Format(text, args);
		}

		public static string GetText(string langId, int key, string defaultValue)
		{
			for (int i = 0; i < MegiLangTypes.Length; i++)
			{
				if (MegiLangTypes[i].Equals(langId) || string.IsNullOrEmpty(langId))
				{
					object obj = IntKeyTableList.FirstOrDefault((KeyValuePair<string, Hashtable> x) => x.Key.Equals(langId)).Value[key.ToString()];
					if (obj != null)
					{
						return obj.ToString();
					}
				}
			}
			return defaultValue;
		}

		public static string GetText(string langId, LangKey key)
		{
			return GetText(langId, Convert.ToInt32(key), "");
		}

		private static Hashtable GetHashTable(List<KeyValuePair<string, Hashtable>> tables, string localeId)
		{
			return tables.FirstOrDefault((KeyValuePair<string, Hashtable> x) => x.Key.Equals(localeId)).Value;
		}

		private static void AddLang(LangModule module, string key, string defaultValue, int type)
		{
			int newItemID = GetNewItemID();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("insert into T_Bas_Lang(MItemID,MModuleID,MName,MType)values(@MItemID,@MModuleID,@MName,@MType);");
			stringBuilder.Append("insert into T_Bas_Lang_L(MPKID,MParentID,MLocaleID,MName)values(@MPKID1,@MItemID,'0x0009',@MValue);");
			stringBuilder.Append("insert into T_Bas_Lang_L(MPKID,MParentID,MLocaleID,MName)values(@MPKID2,@MItemID,'0x7804',@MValue);");
			stringBuilder.Append("insert into T_Bas_Lang_L(MPKID,MParentID,MLocaleID,MName)values(@MPKID3,@MItemID,'0x7C04',@MValue);");
			MySqlParameter[] cmdParms = new MySqlParameter[8]
			{
				new MySqlParameter("@MItemID", newItemID),
				new MySqlParameter("@MModuleID", Convert.ToInt32(module)),
				new MySqlParameter("@MType", type),
				new MySqlParameter("@MName", key),
				new MySqlParameter("@MPKID1", $"{newItemID}_1"),
				new MySqlParameter("@MPKID2", $"{newItemID}_2"),
				new MySqlParameter("@MPKID3", $"{newItemID}_3"),
				new MySqlParameter("@MValue", defaultValue)
			};
			DbHelperMySQL.ExecuteSql(new MContext(), stringBuilder.ToString(), cmdParms);
		}
	}
}
