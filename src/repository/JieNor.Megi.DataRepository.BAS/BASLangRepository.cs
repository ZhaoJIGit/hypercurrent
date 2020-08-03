using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASLangRepository
	{
		private static List<BASLangModel> _sysLang = new List<BASLangModel>();

		private static Dictionary<string, List<BASLangModel>> _orgLang = new Dictionary<string, List<BASLangModel>>();

		private static object _lock = new object();

		public static List<BASLangModel> GetOrgLangList(MContext context)
		{
			if (string.IsNullOrEmpty(context.MOrgID))
			{
				return GetSysLangList(context);
			}
			if (_orgLang.Count > 0 && _orgLang.Keys.Contains(context.MOrgID))
			{
				return _orgLang[context.MOrgID];
			}
			lock (_lock)
			{
				REGGlobalizationRepository rEGGlobalizationRepository = new REGGlobalizationRepository();
				REGGlobalizationModel orgGlobalizationDetail = rEGGlobalizationRepository.GetOrgGlobalizationDetail(context, context.MOrgID);
				List<BASLangModel> list = new List<BASLangModel>();
				if (orgGlobalizationDetail == null || string.IsNullOrEmpty(orgGlobalizationDetail.MSystemLanguage))
				{
					list = GetSysLangList(context);
				}
				else
				{
					string[] langIdArray = orgGlobalizationDetail.MSystemLanguage.Split(',');
					List<BASLangModel> sysLangList = GetSysLangList(context);
					if (sysLangList != null)
					{
						list = (from t in sysLangList
						where langIdArray.Contains(t.LangID)
						select t).ToList();
					}
				}
				if (list != null && (_orgLang.Count == 0 || !_orgLang.Keys.Contains(context.MOrgID)))
				{
					_orgLang.Add(context.MOrgID, list);
				}
				return list;
			}
		}

		public static List<BASLangModel> GetSysLangList(MContext context)
		{
			if (_sysLang != null && _sysLang.Count > 0)
			{
				return _sysLang;
			}
			BASNationalLanguageRepository bASNationalLanguageRepository = new BASNationalLanguageRepository();
			List<BASNationalLanguageModel> modelList = bASNationalLanguageRepository.GetModelList(context, new SqlWhere().Equal("MIsActive", 1), false);
			if (modelList == null)
			{
				_sysLang = new List<BASLangModel>();
			}
			else
			{
				_sysLang = (from t in modelList
				select new BASLangModel
				{
					LangID = t.MItemID,
					LangName = t.MLocalName,
					StandardName = t.MStandardName
				}).ToList();
			}
			return _sysLang;
		}

		public static void ClearCache(MContext context)
		{
			lock (_lock)
			{
				if (_orgLang.Keys != null && !string.IsNullOrEmpty(context.MOrgID) && _orgLang.Keys.Contains(context.MOrgID))
				{
					_orgLang.Remove(context.MOrgID);
				}
			}
		}

		public static Dictionary<LangModule, Dictionary<string, string>> GetStringKeyLangList(MContext ctx, string langId)
		{
			List<BASLangModel> langDataList = GetLangDataList(langId);
			if (langDataList == null || langDataList.Count == 0)
			{
				return null;
			}
			Dictionary<LangModule, Dictionary<string, string>> dictionary = new Dictionary<LangModule, Dictionary<string, string>>();
			List<int> list = (from t in langDataList
			select t.MModuleID).Distinct().ToList();
			foreach (int item in list)
			{
				List<BASLangModel> list2 = (from t in langDataList
				where t.MModuleID == item
				select t).ToList();
				if (list2 != null && list2.Count != 0)
				{
					Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
					foreach (BASLangModel item2 in list2)
					{
						string key = item2.LangKey.ToString();
						if (!dictionary2.ContainsKey(key))
						{
							dictionary2.Add(key, item2.MName);
						}
					}
					LangModule key2 = (LangModule)item;
					dictionary.Add(key2, dictionary2);
				}
			}
			return dictionary;
		}

		public static Dictionary<LangModule, Dictionary<int, string>> GetIntKeyLangList(MContext ctx, string langId)
		{
			List<BASLangModel> langDataList = GetLangDataList(langId);
			if (langDataList == null || langDataList.Count == 0)
			{
				return null;
			}
			Dictionary<LangModule, Dictionary<int, string>> dictionary = new Dictionary<LangModule, Dictionary<int, string>>();
			List<int> list = (from t in langDataList
			select t.MModuleID).Distinct().ToList();
			foreach (int item in list)
			{
				List<BASLangModel> list2 = (from t in langDataList
				where t.MModuleID == item
				select t).ToList();
				if (list2 != null && list2.Count != 0)
				{
					Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
					foreach (BASLangModel item2 in list2)
					{
						int mItemID = item2.MItemID;
						dictionary2.Add(mItemID, item2.MName);
					}
					LangModule key = (LangModule)item;
					dictionary.Add(key, dictionary2);
				}
			}
			return dictionary;
		}

		private static List<BASLangModel> GetLangDataList(string langId)
		{
			string text = "select a.MItemID,a.MName AS LangKey,a.MModuleID,b.MName from T_Bas_Lang a\r\n                            INNER JOIN T_Bas_Lang_L b ON b.MParentID=a.MItemID and b.MIsDelete = 0  and b.MLocaleID=@MLocaleID \r\n                            WHERE a.MType=1 and a.MIsDelete = 0  ";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
			};
			array[0].Value = langId;
			DataSet dataSet = DbHelperMySQL.Query(text.ToString(), array);
			if (dataSet == null || dataSet.Tables.Count == 0)
			{
				return null;
			}
			return ModelInfoManager.DataTableToList<BASLangModel>(dataSet.Tables[0]);
		}

		public static void UpdateLang(MContext ctx, LangModule module, string key, string defaultValue, int type)
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

		public static object GetText(MContext ctx, LangModule module, string key)
		{
			ctx = (ctx ?? new MContext());
			string sQLString = "select b.MName from T_Bas_Lang a\r\n                            inner join T_Bas_Lang_L b ON a.MItemID=b.MParentID  and b.MIsDelete = 0 and b.MIsActive = 1\r\n                            WHERE a.MModuleID=@MModuleID AND a.MName=@MName AND a.MType=1 AND b.MLocaleID=@MLocaleID and a.MIsDelete = 0 and a.MIsActive = 1";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MModuleID", Convert.ToInt32(module)),
				new MySqlParameter("@MName", key),
				new MySqlParameter("@MLocaleID", ctx.MLCID)
			};
			return DbHelperMySQL.GetSingle(sQLString, cmdParms);
		}

		private static void AddLang(LangModule module, string key, string defaultValue, int type)
		{
			int newItemID = GetNewItemID();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("insert into T_Bas_Lang(MItemID,MModuleID,MName,MType)values(@MItemID,@MModuleID,@MName,@MType);");
			stringBuilder.Append("insert into T_Bas_Lang_L(MPKID,MParentID,MLocaleID,MName)values(@MPKID1,@MItemID,'0x0009',@MValue);");
			stringBuilder.Append("insert into T_Bas_Lang_L(MPKID,MParentID,MLocaleID,MName)values(@MPKID2,@MItemID,'0x7804','');");
			stringBuilder.Append("insert into T_Bas_Lang_L(MPKID,MParentID,MLocaleID,MName)values(@MPKID3,@MItemID,'0x7C04','');");
			MySqlParameter[] array = new MySqlParameter[8]
			{
				new MySqlParameter("@MItemID", MySqlDbType.Int32),
				new MySqlParameter("@MModuleID", MySqlDbType.Int32),
				new MySqlParameter("@MType", MySqlDbType.Int32),
				new MySqlParameter("@MName", MySqlDbType.VarChar, 500),
				new MySqlParameter("@MPKID1", MySqlDbType.VarChar, 50),
				new MySqlParameter("@MPKID2", MySqlDbType.VarChar, 50),
				new MySqlParameter("@MPKID3", MySqlDbType.VarChar, 50),
				new MySqlParameter("@MValue", MySqlDbType.VarChar, 500)
			};
			array[0].Value = newItemID;
			array[1].Value = Convert.ToInt32(module);
			array[2].Value = type;
			array[3].Value = key;
			array[4].Value = $"{newItemID}_1";
			array[5].Value = $"{newItemID}_2";
			array[6].Value = $"{newItemID}_3";
			array[7].Value = defaultValue;
			DbHelperMySQL.ExecuteSql(new MContext(), stringBuilder.ToString(), array);
		}

		private static void UpdateLang(int itemId, string defaultValue)
		{
			string sQLString = "UPDATE T_Bas_Lang_L SET MName=@MValue WHERE MParentID=@MParentID AND MLocaleID='0x0009' and MIsDelete = 0 and MIsActive = 1";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MParentID", MySqlDbType.Int32),
				new MySqlParameter("@MValue", MySqlDbType.VarChar, 500)
			};
			array[0].Value = itemId;
			array[1].Value = defaultValue;
			DbHelperMySQL.ExecuteSql(new MContext(), sQLString, array);
		}

		private static int GetExistsItemID(LangModule module, string key, int type)
		{
			string sQLString = "SELECT MItemID FROM T_Bas_Lang WHERE MModuleID=@MModuleID AND MName=@MName AND MType=@MType and MIsDelete = 0 and MIsActive = 1 ";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MModuleID", MySqlDbType.Int32),
				new MySqlParameter("@MName", MySqlDbType.VarChar, 500),
				new MySqlParameter("@MType", MySqlDbType.Int32)
			};
			array[0].Value = Convert.ToInt32(module);
			array[1].Value = key;
			array[2].Value = type;
			DataSet dataSet = DbHelperMySQL.Query(sQLString, array);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return 0;
			}
			return Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
		}

		private static int GetNewItemID()
		{
			string sQLString = "SELECT MAX(MItemID) FROM T_Bas_Lang where MIsDelete = 0 and MIsActive = 1";
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
	}
}
