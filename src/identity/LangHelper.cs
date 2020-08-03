using JieNor.Megi.Common.Cache;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.ServiceContract.COM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.Identity
{
	public class LangHelper
	{
		private static string stringCacheName = "stringCacheName";

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

		public static List<KeyValuePair<string, Hashtable>> GetStringKeyLangTableList(string[] localeIds = null)
		{
			ICOMMultiLang sysService = ServiceHostManager.GetSysService<ICOMMultiLang>();
			using (sysService as IDisposable)
			{
				return sysService.GetStringKeyLangTableList(localeIds, null).ResultData;
			}
		}

		public static List<KeyValuePair<string, Hashtable>> GetIntKeyLangTableList(string[] localeIds = null)
		{
			ICOMMultiLang sysService = ServiceHostManager.GetSysService<ICOMMultiLang>();
			using (sysService as IDisposable)
			{
				return sysService.GetIntKeyLangTableList(localeIds, null).ResultData;
			}
		}

		public static string GetText(string localeId, LangModule module, string key)
		{
			localeId = (string.IsNullOrEmpty(localeId) ? LangIndentity.CurrentLangID : localeId);
			Hashtable hashTable = GetHashTable(StringKeyTableList, localeId);
			object obj = hashTable?[Convert.ToInt32(module) + "." + key.ToLower()];
			return (obj == null) ? string.Empty : obj.ToString();
		}

		private static Hashtable GetHashTable(List<KeyValuePair<string, Hashtable>> tables, string localeId)
		{
			return tables.FirstOrDefault((KeyValuePair<string, Hashtable> x) => x.Key.Equals(localeId)).Value;
		}

		public static string GetText(string localeId, LangModule module, string key, string defaultValue)
		{
			string text = GetText(localeId, module, key);
			if (string.IsNullOrEmpty(text))
			{
				ICOMMultiLang sysService = ServiceHostManager.GetSysService<ICOMMultiLang>();
				using (sysService as IDisposable)
				{
					text = sysService.GetTextByStringKey(localeId, module, key, defaultValue, null).ResultData;
					StringKeyTableList = new List<KeyValuePair<string, Hashtable>>();
				}
			}
			return text;
		}

		public static string GetText(string localeId, LangModule langModule, int key, string defaultValue)
		{
			localeId = (string.IsNullOrEmpty(localeId) ? LangIndentity.CurrentLangID : localeId);
			Hashtable hashTable = GetHashTable(IntKeyTableList, localeId);
			object obj = hashTable?[key.ToString()];
			return (obj == null) ? string.Empty : obj.ToString();
		}

		public static string GetText(LangModule langModule, int key, string defaultValue)
		{
			string currentLangID = LangIndentity.CurrentLangID;
			Hashtable hashTable = GetHashTable(IntKeyTableList, currentLangID);
			object obj = hashTable?[key.ToString()];
			return (obj == null) ? string.Empty : obj.ToString();
		}

		public static string GetText(LangModule langModule, string key, string defaultValue)
		{
			string currentLangID = LangIndentity.CurrentLangID;
			return GetText(currentLangID, langModule, key, defaultValue);
		}

		public static string GetText(string langId, LangKey key)
		{
			return GetText(langId, LangModule.Common, Convert.ToInt32(key), "");
		}
	}
}
