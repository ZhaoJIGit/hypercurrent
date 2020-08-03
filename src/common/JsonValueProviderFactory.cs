using JieNor.Megi.Common.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Common
{
	public sealed class JsonValueProviderFactory : ValueProviderFactory
	{
		private class EntryLimitedDictionary
		{
			private static int _maximumDepth = GetMaximumDepth();

			private readonly IDictionary<string, object> _innerDictionary;

			private int _itemCount;

			public EntryLimitedDictionary(IDictionary<string, object> innerDictionary)
			{
				_innerDictionary = innerDictionary;
			}

			public void Add(string key, object value)
			{
				if (++_itemCount > _maximumDepth)
				{
					throw new InvalidOperationException("JsonValueProviderFactory_RequestTooLarge");
				}
				_innerDictionary.Add(key, value);
			}

			private static int GetMaximumDepth()
			{
				NameValueCollection appSettings = ConfigurationManager.AppSettings;
				if (appSettings != null)
				{
					string[] values = appSettings.GetValues("aspnet:MaxJsonDeserializerMembers");
					int result = default(int);
					if (values != null && values.Length != 0 && int.TryParse(values[0], out result))
					{
						return result;
					}
				}
				return 1000;
			}
		}

		public override IValueProvider GetValueProvider(ControllerContext controllerContext)
		{
			if (controllerContext == null)
			{
				throw new ArgumentNullException("controllerContext");
			}
			object deserializedObject = GetDeserializedObject(controllerContext);
			if (deserializedObject == null)
			{
				return null;
			}
			deserializedObject = HanleZipData(deserializedObject);
			Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			AddToBackingStore(new EntryLimitedDictionary(dictionary), string.Empty, deserializedObject);
			return new DictionaryValueProvider<object>(dictionary, CultureInfo.CurrentCulture);
		}

		private object HanleZipData(object deserializedObject)
		{
			Dictionary<string, object> dics = deserializedObject as Dictionary<string, object>;
			if (dics != null && dics.ContainsKey("zip") && dics.ContainsKey("data") && Convert.ToBoolean(dics["zip"]))
			{
				string zipData = dics["data"]?.ToString();
				string unzippedData = ZipHelper.Decompress(zipData);
				JavaScriptSerializer serializer = new JavaScriptSerializer
				{
					MaxJsonLength = 2147483647
				};
				deserializedObject = serializer.DeserializeObject(unzippedData);
			}
			return deserializedObject;
		}

		private static void AddToBackingStore(EntryLimitedDictionary backingStore, string prefix, object value)
		{
			IDictionary<string, object> dictionary = value as IDictionary<string, object>;
			JavaScriptSerializer serializer = new JavaScriptSerializer
			{
				MaxJsonLength = 2147483647
			};
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, object> item in dictionary)
				{
					object finalValue = item.Value;
					IDictionary<string, object> dic = item.Value as IDictionary<string, object>;
					if (dic != null)
					{
						finalValue = ((dic.Values.Count > 0) ? serializer.Serialize(item.Value) : null);
					}
					else
					{
						IList list2 = item.Value as IList;
						if (list2 != null)
						{
							finalValue = ((list2.Count > 0) ? serializer.Serialize(item.Value) : null);
						}
					}
					backingStore.Add(item.Key, finalValue);
				}
			}
			else
			{
				IList list = value as IList;
				if (list != null)
				{
					for (int index = 0; index < list.Count; index++)
					{
						AddToBackingStore(backingStore, MakeArrayKey(prefix, index), list[index]);
					}
				}
				else
				{
					backingStore.Add(prefix, value);
				}
			}
		}

		private static object GetDeserializedObject(ControllerContext controllerContext)
		{
			if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			string end = new StreamReader(controllerContext.HttpContext.Request.InputStream).ReadToEnd();
			if (string.IsNullOrEmpty(end))
			{
				return null;
			}
			JavaScriptSerializer serializer = new JavaScriptSerializer
			{
				MaxJsonLength = 2147483647
			};
			return serializer.DeserializeObject(end);
		}

		private static string MakeArrayKey(string prefix, int index)
		{
			return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
		}

		private static string MakePropertyKey(string prefix, string propertyName)
		{
			return (!string.IsNullOrEmpty(prefix)) ? (prefix + "." + propertyName) : propertyName;
		}
	}
}
