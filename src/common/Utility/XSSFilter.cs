using Fasterflect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Common.Utility
{
	public class XSSFilter
	{
		public static readonly string DefaultXssFilterReg = "document\\s*\\.|window\\s*\\.|javascript|<(\\s*|\\/)script\\s+?>|<(\\s*|\\/)img\\s+?>|<(\\s*|\\/)applet\\s+?>|<(\\s*|\\/)embed\\s+?>|<(\\s*|\\/)a\\s+?>|<(\\s*|\\/)meta\\s+?>|<(\\s*|\\/)xml\\s+?>|<(\\s*|\\/)html\\s+?>|<(\\s*|\\/)body\\s+?>|<(\\s*|\\/)head\\s+?>|<(\\s*|\\/)div\\s+?>|<(\\s*|\\/)ul\\s+?>|<(\\s*|\\/)li\\s+?>|<(\\s*|\\/)style\\s+?>|<(\\s*|\\/)base\\s+?>|<(\\s*|\\/)link\\s+?>|<(\\s*|\\/)iframe\\s+?>|<(\\s*|\\/)frameset\\s+?>|<(\\s*|\\/)bgsound\\s+?>|<(\\s*|\\/)object\\s+?>|\\s+style\\s*=|\\s+href\\s*=|\\s+rel\\s*=|\\s+type\\s*=|\\s+src\\s*=|\\s+backgroud\\s*=|\\s+alert\\s*\\([^()]*\\)|\\s+url\\s*\\([^()]*\\)|\\s+eval\\s*\\([^()]*\\)|\\s+escape\\s*\\([^()]*\\)|\\s+unescape\\s*\\([^()]*\\)|\\s+execscript\\s*\\([^()]*\\)|\\s+msgbox\\s*\\([^()]*\\)|\\s+confirm\\s*\\([^()]*\\)|\\s+prompt\\s*\\([^()]*\\)|\\s+data\\s*:";

		private static string DefaultXssInvalidCharReg = "\\t|\\n|\\r|\\f";

		private static string DefaultXssScriptCharReg = "\\\\|&|#|%|<|>|:|\\(|\\)";

		private static string _xssFilterReg;

		private static string _xssInvalidCharReg;

		private static string _xssScriptCharReg;

		private static Regex xssFilter = new Regex(XssFilterReg, RegexOptions.IgnoreCase);

		private static Regex xssScriptCharFilter = new Regex(XssScriptCharReg, RegexOptions.IgnoreCase);

		private static Regex jsUnicoderegFilter = new Regex("(?i)\\\\[uU]([0-9a-f]{4})");

		private static Regex xssInvalidCharFilter = new Regex(XssInvalidCharReg, RegexOptions.IgnoreCase);

		private static Regex asciiToString8Filter = new Regex("\\\\([0-7]{1,4})");

		private static Regex asciiToString16Filter = new Regex("\\\\[xX]([0-9a-f]{1,4})");

		private static string XssFilterReg
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_xssFilterReg))
				{
					string value = ConfigurationManager.AppSettings["XssFilterReg"];
					if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
					{
						_xssFilterReg = value.ToString();
					}
					else
					{
						_xssFilterReg = DefaultXssFilterReg;
					}
				}
				return _xssFilterReg;
			}
		}

		private static string XssInvalidCharReg
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_xssInvalidCharReg))
				{
					string value = ConfigurationManager.AppSettings["XssInvalidCharReg"];
					if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
					{
						_xssInvalidCharReg = value.ToString();
					}
					else
					{
						_xssInvalidCharReg = DefaultXssInvalidCharReg;
					}
				}
				return _xssInvalidCharReg;
			}
		}

		private static string XssScriptCharReg
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_xssScriptCharReg))
				{
					string value = ConfigurationManager.AppSettings["XssScriptCharReg"];
					if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
					{
						_xssScriptCharReg = value.ToString();
					}
					else
					{
						_xssScriptCharReg = DefaultXssScriptCharReg;
					}
				}
				return _xssScriptCharReg;
			}
		}

		public static string XssFilter(string s)
		{
			if (string.IsNullOrWhiteSpace(s))
			{
				return s;
			}
			string s2 = string.Copy(s);
			if (HasXssEscapeChar(s2))
			{
				s2 = ReplaceInvalidChar(s2);
				s2 = XSSDecode(s2);
			}
			if (HasXssScript(s2))
			{
				return XssFilter(FilterXssScript(s2));
			}
			return s;
		}

		public static bool HasXssEscapeChar(string s)
		{
			return xssScriptCharFilter.IsMatch(s);
		}

		private static string ReplaceInvalidChar(string s)
		{
			return xssInvalidCharFilter.Replace(s, "");
		}

		private static bool HasXssScript(string s)
		{
			return xssFilter.IsMatch(s);
		}

		private static string XSSDecode(string s)
		{
			return string.IsNullOrWhiteSpace(s) ? s : HttpUtility.HtmlDecode(UrlDecode(AsciiToString(JsUnicode(s))));
		}

		private static string FilterXssScript(string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				value = xssFilter.Replace(value, string.Empty);
			}
			return value;
		}

		private static string JsUnicode(string s)
		{
			return jsUnicoderegFilter.Replace(s, (Match m) => ((char)(ushort)Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
		}

		private static string AsciiToString(string s)
		{
			return AsciiToString8(AsciiToString16(s));
		}

		private static string AsciiToString8(string s)
		{
			MatchCollection group = asciiToString8Filter.Matches(s);
			s = asciiToString8Filter.Replace(s, (Match m) => ((char)(ushort)Convert.ToInt32(m.Groups[1].Value, 8)).ToString().Replace("\0", ""));
			return s;
		}

		private static string AsciiToString16(string s)
		{
			s = asciiToString16Filter.Replace(s, (Match m) => ((char)(ushort)Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
			return s;
		}

		private static string Base64Decode(string s)
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

		private static string UrlDecode(string s)
		{
			return HttpUtility.UrlDecode(s);
		}

		[Obsolete("请使用 mText.ToJson 方法")]
		public static string ToJson(object o)
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			string jsonString = serializer.Serialize(JsonEncode(o));
			o = JsonDecode(o);
			return jsonString;
		}

		[Obsolete("请使用 mText.ParseJson 方法")]
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
	}
}
