using Fasterflect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Common.Utility
{
	public class MText
	{
		public static string Decode(string s)
		{
			if (!string.IsNullOrWhiteSpace(s))
			{
				s = s.Replace("&#39;", "'").Replace("&quot;", "\"").Replace("\\n", "\n")
					.Replace("&gt;", ">")
					.Replace("&lt;", "<")
					.Replace("&#91;", "[")
					.Replace("&#92;", "\\")
					.Replace("&#93;", "]")
					.Replace("&#123;", "{")
					.Replace("&#125;", "}");
			}
			return s;
		}

		public static string Encode(string s)
		{
			if (!string.IsNullOrWhiteSpace(s))
			{
				s = s.Replace("'", "&#39;").Replace("\"", "&quot;").Replace("\n", "\\n")
					.Replace(">", "&gt;")
					.Replace("<", "&lt;")
					.Replace("[", "&#91;")
					.Replace("\\", "&#92;")
					.Replace("]", "&#93;")
					.Replace("{", "&#123;")
					.Replace("}", "&#125;");
			}
			return s;
		}

		public static string ToJson(object o)
		{
			if (o != null)
			{
				if (o.GetType() == typeof(string))
				{
					return MText.JsonEncode<string>(o);
				}
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				string jsonString = serializer.Serialize(JsonEncode(o));
				o = JsonDecode(o);
				return jsonString;
			}
			return null;
		}

		public static T ParseJson<T>(string o)
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			return (T)JsonDecode(serializer.Deserialize<T>(o));
		}

		public static object JsonEncode(object o)
		{
			if (o == null || o.GetType() == typeof(DateTime))
			{
				return o;
			}
			if (o.GetType() == typeof(string))
			{
				return JsonTextEncode(o.ToString());
			}
			if (o.GetType().BaseType == typeof(Array))
			{
				Array array = (Array)o;
				for (int i = 0; i < array.Length; i++)
				{
					array.SetElement(i, JsonEncode(array.GetElement(i)));
				}
			}
			else if (o.GetType() == typeof(ArrayList))
			{
				ArrayList array2 = (ArrayList)o;
				for (int k = 0; k < array2.Count; k++)
				{
					array2.SetElement(k, JsonEncode(array2.GetElement(k)));
				}
			}
			else if (Enumerable.Contains(o.GetType().GetInterfaces(), typeof(IList)))
			{
				IList list = (IList)o;
				for (int l = 0; l < list.Count; l++)
				{
					list[l] = JsonEncode(list[l]);
				}
			}
			else if (Enumerable.Contains(o.GetType().GetInterfaces(), typeof(IDictionary)))
			{
				IDictionary dic = (IDictionary)o;
				ArrayList keys = new ArrayList(dic.Keys);
				foreach (object item in keys)
				{
					dic[item] = JsonEncode(dic[item]);
				}
			}
			else if (o.GetType().Name == typeof(KeyValuePair<, >).Name)
			{
				if (o.GetType() == typeof(KeyValuePair<string, string>))
				{
					KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)o;
					o = new KeyValuePair<string, string>(kvp.Key, JsonEncode(kvp.Value) as string);
				}
			}
			else if (!o.GetType().IsPrimitive)
			{
				PropertyInfo[] properties = o.GetType().GetProperties();
				for (int j = 0; j < properties.Length; j++)
				{
					MethodInfo setMethod = properties[j].GetSetMethod();
					if (setMethod != (MethodInfo)null)
					{
						object proValue = o.GetPropertyValue(properties[j].Name);
						if (proValue != null && ((proValue.GetType().IsPrimitive && proValue.GetType() == typeof(string)) || !proValue.GetType().IsPrimitive))
						{
							o.SetPropertyValue(properties[j].Name, JsonEncode(proValue));
						}
					}
				}
			}
			return o;
		}

		public static T JsonEncode<T>(object o)
		{
			if (o == null)
			{
				return default(T);
			}
			return (T)JsonEncode(o);
		}

		public static T JsonDecode<T>(object o)
		{
			if (o == null)
			{
				return default(T);
			}
			return (T)JsonDecode(o);
		}

		public static object JsonDecode(object o)
		{
			if (o == null || o.GetType() == typeof(DateTime))
			{
				return o;
			}
			if (o.GetType() == typeof(string))
			{
				return JsonTextDecode(o.ToString());
			}
			if (o.GetType().BaseType == typeof(Array))
			{
				Array array = (Array)o;
				for (int i = 0; i < array.Length; i++)
				{
					array.SetElement(i, JsonDecode(array.GetElement(i)));
				}
			}
			else if (o.GetType() == typeof(ArrayList))
			{
				ArrayList array2 = (ArrayList)o;
				for (int k = 0; k < array2.Count; k++)
				{
					array2.SetElement(k, JsonDecode(array2.GetElement(k)));
				}
			}
			else if (Enumerable.Contains(o.GetType().GetInterfaces(), typeof(IList)))
			{
				IList list = (IList)o;
				for (int l = 0; l < list.Count; l++)
				{
					list[l] = JsonDecode(list[l]);
				}
			}
			else if (Enumerable.Contains(o.GetType().GetInterfaces(), typeof(IDictionary)))
			{
				IDictionary dic = (IDictionary)o;
				ArrayList keys = new ArrayList(dic.Keys);
				foreach (object item in keys)
				{
					dic[item] = JsonDecode(dic[item]);
				}
			}
			else if (o.GetType().Name == typeof(KeyValuePair<, >).Name)
			{
				if (o.GetType() == typeof(KeyValuePair<string, string>))
				{
					KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)o;
					o = new KeyValuePair<string, string>(kvp.Key, JsonDecode(kvp.Value) as string);
				}
			}
			else if (!o.GetType().IsPrimitive)
			{
				PropertyInfo[] properties = o.GetType().GetProperties();
				for (int j = 0; j < properties.Length; j++)
				{
					MethodInfo setMethod = properties[j].GetSetMethod();
					if (setMethod != (MethodInfo)null)
					{
						object proValue = o.GetPropertyValue(properties[j].Name);
						if (proValue != null && ((proValue.GetType().IsPrimitive && proValue.GetType() == typeof(string)) || !proValue.GetType().IsPrimitive))
						{
							o.SetPropertyValue(properties[j].Name, JsonDecode(proValue));
						}
					}
				}
			}
			return o;
		}

		private static string JsonTextEncode(string s)
		{
			if (!string.IsNullOrWhiteSpace(s))
			{
				s = s.Replace("'", "&#39;").Replace("\"", "&quot;").Replace("\n", "\\n")
					.Replace(">", "&gt;")
					.Replace("<", "&lt;")
					.Replace("[", "&#91;")
					.Replace("\\", "&#92;")
					.Replace("]", "&#93;")
					.Replace("{", "&#123;")
					.Replace("}", "&#125;");
			}
			return s;
		}

		private static string JsonTextDecode(string s)
		{
			if (!string.IsNullOrWhiteSpace(s))
			{
				s = s.Replace("&#39;", "'").Replace("&quot;", "\"").Replace("\\n", "\n")
					.Replace("&gt;", ">")
					.Replace("&lt;", "<")
					.Replace("&#91;", "[")
					.Replace("&#92;", "\\")
					.Replace("&#93;", "]")
					.Replace("&#123;", "{")
					.Replace("&#125;", "}");
			}
			return s;
		}

		public static string Base64Decode(string s)
		{
			try
			{
				return Encoding.Default.GetString(Convert.FromBase64String(s));
			}
			catch (Exception)
			{
				return s;
			}
		}

		public static string Base64Encode(string s)
		{
			try
			{
				return Convert.ToBase64String(Encoding.Default.GetBytes(s));
			}
			catch (Exception)
			{
				return s;
			}
		}
	}
}
