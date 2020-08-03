using JieNor.Megi.Core;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.API;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using Microsoft.CSharp;
using MySql.Data.MySqlClient;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JieNor.Megi.DataRepository.API
{
	public class APIDataHelper
	{
		private static Hashtable ModelTypeCache = Hashtable.Synchronized(new Hashtable());

		private const string INVOLID_ENUM_VALUE = "-999999";

		private static string LEFT = "♀";

		private static string RIGHT = "♂";

		private static string CENTER = "♁";

		private static string PREFIX = "卐";

		private static string SPACE = "✰";

		private static string AND = "「";

		private static string OR = "」";

		private static string DOT = "✯";

		private static string EQ = "✭";

		private static string QUOTES = "§";

		private static string ESCAPE_QUOTES = "£";

		private static string LAMBDA_PREFIX = "megi_api";

		public static KeyValuePair<string, List<MySqlParameter>> ParseWhere<T>(MContext ctx, GetParam param, List<APIFieldModel> fieldsInfo)
		{
			Type typeFromHandle = typeof(T);
			KeyValuePair<string, List<MySqlParameter>> result = new KeyValuePair<string, List<MySqlParameter>>((string)null, new List<MySqlParameter>());
			param.Where = "";
			if (!string.IsNullOrWhiteSpace(param.WhereString))
			{
				param.WhereString = HandleUpdateDateUTC(param.WhereString);
			}
			ParseQueryString(typeFromHandle, param, fieldsInfo);
			string exp = " " + param.Where + " ";
			if (!string.IsNullOrWhiteSpace(param.Where.Trim()))
			{
				ValidateExp(ctx, typeFromHandle, exp, fieldsInfo);
				exp = RepaceWithSpecialChar(exp);
				exp = AddSpace(exp);
				exp = ReplaceDateTime(exp);
				exp = MarkMethodParameter(exp);
				exp = FindParameter(exp);
				exp = ReplaceLambda(exp);
				exp = HandleOperator(ctx, exp, fieldsInfo);
				exp = RepalceLogicOperator(exp);
				return GetFinalSqlParameter(ctx, typeFromHandle, exp, fieldsInfo);
			}
			return result;
		}

		public static List<APIFieldModel> GetTypeFieldList(Type type, APIFieldModel parentFeild = null, bool allFields = false)
		{
			List<APIFieldModel> list = new List<APIFieldModel>();
			string key = type.Name + (allFields ? "true" : "false");
			if (ModelTypeCache.ContainsKey(key) && parentFeild == null)
			{
				return ModelTypeCache[key] as List<APIFieldModel>;
			}
			List<PropertyInfo> list2 = type.GetProperties().ToList();
			for (int i = 0; i < list2.Count; i++)
			{
				PropertyInfo propertyInfo = list2[i];
				object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(ApiMemberAttribute), false);
				if (customAttributes.GetLength(0) != 0 || allFields)
				{
					APIFieldModel aPIFieldModel = new APIFieldModel();
					aPIFieldModel.Prop = propertyInfo;
					object[] customAttributes2 = propertyInfo.GetCustomAttributes(typeof(ApiEnumAttribute), false);
					if (customAttributes2.GetLength(0) == 1)
					{
						ApiEnumAttribute apiEnumAttribute = (ApiEnumAttribute)customAttributes2[0];
						aPIFieldModel.EnumMappingType = apiEnumAttribute.Type.ToString();
					}
					object[] customAttributes3 = propertyInfo.GetCustomAttributes(typeof(ApiDetailAttribute), false);
					aPIFieldModel.IsDetail = (customAttributes3.GetLength(0) != 0);
					object[] customAttributes4 = propertyInfo.GetCustomAttributes(typeof(ColumnEncryptAttribute), false);
					aPIFieldModel.IsEncrypt = (customAttributes4.GetLength(0) != 0);
					object[] customAttributes5 = propertyInfo.GetCustomAttributes(typeof(BaseDataAttribute), false);
					aPIFieldModel.BaseDataAttr = ((customAttributes5.GetLength(0) > 0) ? (customAttributes5[0] as BaseDataAttribute) : null);
					ApiMemberAttribute apiMemberAttribute = (customAttributes.GetLength(0) == 0) ? null : ((ApiMemberAttribute)customAttributes[0]);
					if (apiMemberAttribute != null)
					{
						aPIFieldModel.ApiAttr = apiMemberAttribute;
						aPIFieldModel.IsLang = (apiMemberAttribute != null && apiMemberAttribute.MemberType == ApiMemberType.MultiLang);
						string obj = (parentFeild != null) ? (parentFeild.ApiName + "_" + apiMemberAttribute.Name) : apiMemberAttribute.Name;
						string text2 = aPIFieldModel.ApiName = obj;
					}
					string fieldName = (parentFeild != null) ? (parentFeild.Name + "_" + propertyInfo.Name) : propertyInfo.Name;
					aPIFieldModel.Name = fieldName;
					if (IsListProperty(propertyInfo))
					{
						string listPropertyGenericType = GetListPropertyGenericType(propertyInfo);
						Type type2 = Type.GetType(listPropertyGenericType);
						list.AddRange(GetTypeFieldList(type2, aPIFieldModel, false));
					}
					if (!IsPrimitive(propertyInfo.PropertyType))
					{
						list.AddRange(GetTypeFieldList(propertyInfo.PropertyType, aPIFieldModel, false));
					}
					if (!list.Exists((APIFieldModel x) => x.Name == fieldName))
					{
						list.Add(aPIFieldModel);
					}
				}
			}
			if (parentFeild == null)
			{
				lock (ModelTypeCache.SyncRoot)
				{
					if (!ModelTypeCache.ContainsKey(key))
					{
						ModelTypeCache[key] = list;
					}
				}
			}
			return list;
		}

		private static void ParseQueryString(Type type, GetParam param, List<APIFieldModel> fieldsInfo)
		{
			param.Where = (param.WhereString ?? "");
			bool flag = !string.IsNullOrWhiteSpace(param.ElementID);
			if (flag)
			{
				object obj = Activator.CreateInstance(type);
				APIFieldModel aPIFieldModel = fieldsInfo.FirstOrDefault((APIFieldModel x) => x.ApiAttr != null && x.ApiAttr.IsPKField && x.Level == 1);
				if (aPIFieldModel != null)
				{
					param.QueryString = AddNameValueModel(param.QueryString, aPIFieldModel.ApiName, param.ElementID, aPIFieldModel.ApiAttr.RecordFilterField);
				}
			}
			DateTime modifiedSince = param.ModifiedSince;
			if (modifiedSince.Year > 1)
			{
				APIFieldModel aPIFieldModel2 = fieldsInfo.FirstOrDefault((APIFieldModel x) => x.Name == "MModifyDate");
				if (aPIFieldModel2 != null)
				{
					object[] obj2 = new object[14]
					{
						aPIFieldModel2.ApiName,
						" >= DateTime(",
						null,
						null,
						null,
						null,
						null,
						null,
						null,
						null,
						null,
						null,
						null,
						null
					};
					modifiedSince = param.ModifiedSince;
					obj2[2] = modifiedSince.Year;
					obj2[3] = ",";
					modifiedSince = param.ModifiedSince;
					obj2[4] = modifiedSince.Month;
					obj2[5] = ",";
					modifiedSince = param.ModifiedSince;
					obj2[6] = modifiedSince.Day;
					obj2[7] = ",";
					modifiedSince = param.ModifiedSince;
					obj2[8] = modifiedSince.Hour;
					obj2[9] = ",";
					modifiedSince = param.ModifiedSince;
					obj2[10] = modifiedSince.Minute;
					obj2[11] = ",";
					modifiedSince = param.ModifiedSince;
					obj2[12] = modifiedSince.Second;
					obj2[13] = ")";
					string text = string.Concat(obj2);
					param.Where = (string.IsNullOrWhiteSpace(param.Where) ? text : ("(" + param.Where + ") &&" + text));
				}
			}
			if (param.QueryString != null && param.QueryString.Count != 0)
			{
				List<string> list = new List<string>
				{
					"orderBy",
					"where",
					"Type",
					"IncludeDisabled",
					"MethodType",
					"pageIndex",
					"pageSize"
				};
				List<string> list2 = new List<string>();
				string[] names = param.QueryString.AllKeys;
				int i;
				for (i = 0; i < names.Count(); i++)
				{
					if (names[i] != null && !list.Exists((string x) => x.ToLower() == names[i].ToLower()))
					{
						APIFieldModel aPIFieldModel3 = fieldsInfo.FirstOrDefault((APIFieldModel x) => !string.IsNullOrWhiteSpace(x.ApiName) && x.ApiName.ToLower() == names[i].ToLower());
						if (aPIFieldModel3 != null)
						{
							list2.Add(names[i] + "==" + GetParamValue(aPIFieldModel3, param.QueryString[names[i]]));
						}
					}
				}
				if (flag && list2.Count > 1)
				{
					string item = "(" + string.Join("||", list2) + ")";
					list2.Clear();
					list2.Add(item);
				}
				param.Where = ((list2 == null || list2.Count == 0) ? param.Where : ((!string.IsNullOrWhiteSpace(param.Where)) ? ("(" + param.Where + ")&&" + string.Join("&&", list2)) : string.Join("&&", list2)));
				param.Where = ((!string.IsNullOrWhiteSpace(param.Where)) ? (" (" + param.Where + ") ") : param.Where);
			}
		}

		private static NameValueCollection AddNameValueModel(NameValueCollection src, string key, string value, string recordFilterField = null)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (src != null && src.Count != 0)
			{
				string[] allKeys = src.AllKeys;
				foreach (string name in allKeys)
				{
					nameValueCollection.Add(name, src[name]);
				}
			}
			if (!nameValueCollection.AllKeys.Contains(key))
			{
				nameValueCollection.Add(key, value);
			}
			if (!string.IsNullOrWhiteSpace(recordFilterField) && !nameValueCollection.AllKeys.Contains(recordFilterField))
			{
				nameValueCollection.Add(recordFilterField, value);
			}
			return nameValueCollection;
		}

		private static string GetParamValue(APIFieldModel filed, string value)
		{
			if (filed.Prop.PropertyType == typeof(string))
			{
				return "\"" + value.TrimStart('"').TrimEnd('"') + "\"";
			}
			return value;
		}

		private static string RepalceApiMember(MContext ctx, string exp, List<APIFieldModel> fieldsInfo)
		{
			APIFieldModel entryField = fieldsInfo.FirstOrDefault((APIFieldModel x) => x.IsDetail);
			List<APIFieldModel> list = (from x in fieldsInfo
			where entryField == null || !x.Name.StartsWith(entryField.Name)
			select x).ToList();
			List<APIFieldModel> list2 = (from x in fieldsInfo
			where entryField != null && x.Name.StartsWith(entryField.Name)
			select x).ToList();
			for (int i = 0; i < list.Count; i++)
			{
				Regex regex = new Regex("\\s" + list[i].ApiName.Replace("_", ".") + "(\\.|\\s)");
				exp = regex.Replace(exp, " " + list[i].Name.Replace("_", ".") + "$1 ");
				if (list[i].EnumMappingType != null)
				{
					regex = new Regex("(?<=\\s" + list[i].Name.Replace("_", ".") + "\\s*(==|!=|<=|\\>=|<|>){1}\\s*)(\"\\S*\")");
					Match match = regex.Match(exp);
					while (match.Success)
					{
						string publicValue = match.Groups[2].ToString();
						string internalValue = BASEnumMappingRepository.GetInternalValue(ctx, list[i].EnumMappingType, publicValue, null, true);
						internalValue = ((list[i].Prop.PropertyType == typeof(bool)) ? internalValue.ToLower() : (string.IsNullOrWhiteSpace(internalValue) ? "-999999" : internalValue));
						exp = regex.Replace(exp, (list[i].Prop.PropertyType == typeof(string)) ? ("\"" + internalValue + "\"") : internalValue);
						match = match.NextMatch();
					}
				}
			}
			if (entryField != null)
			{
				Regex regex2 = new Regex(" " + entryField.ApiName + "(\\.| )");
				exp = regex2.Replace(exp, " " + entryField.Name + "$1");
				for (int j = 0; j < list2.Count; j++)
				{
					regex2 = new Regex("(?<=\\s*" + entryField.Name + "\\s*\\.Exists\\s*\\(.*\\.)" + list2[j].ApiAttr.Name + "(?=(\\.|\\s))");
					exp = regex2.Replace(exp, list2[j].Prop.Name);
					if (list2[j].EnumMappingType != null)
					{
						regex2 = new Regex("(?<=\\s" + entryField.Name + "\\s*\\.Exists\\s*\\(.*\\." + new Regex(entryField.Name + "_").Replace(list2[j].Name, "") + "\\s*(==|!=|<=|\\>=|<|>){1}\\s*)(\"\\S*\")");
						Match match2 = regex2.Match(exp);
						while (match2.Success)
						{
							string publicValue2 = match2.Groups[2].ToString();
							string internalValue2 = BASEnumMappingRepository.GetInternalValue(ctx, list2[j].EnumMappingType, publicValue2, null, true);
							internalValue2 = ((list2[j].Prop.PropertyType == typeof(bool)) ? internalValue2.ToLower() : internalValue2);
							exp = regex2.Replace(exp, (list2[j].Prop.PropertyType == typeof(string)) ? ("\"" + internalValue2 + "\"") : (string.IsNullOrWhiteSpace(internalValue2) ? "-999999" : internalValue2));
							match2 = match2.NextMatch();
						}
					}
				}
			}
			return exp;
		}

		private static string EscapeSpecialChar(string exp)
		{
			string exp2 = RepaceWithSpecialChar(exp);
			exp2 = AddSpace(exp2);
			exp2 = MarkMethodParameter(exp2);
			exp2 = FindParameter(exp2);
			exp2 = exp2.Replace(CENTER + "\"", CENTER + QUOTES).Replace("\"" + CENTER, QUOTES + CENTER);
			if (exp2.Contains("\""))
			{
				exp = exp2.Replace("\\\"", ESCAPE_QUOTES).Replace("\"", "\\\"").Replace(ESCAPE_QUOTES, "\\\"");
				exp = exp.Replace(CENTER + QUOTES, "\"").Replace(QUOTES + CENTER, "\"");
				exp = exp.Replace(CENTER + "DateTime" + CENTER, "DateTime");
			}
			return exp;
		}

		public static void ValidateExp(MContext ctx, Type type, string exp, List<APIFieldModel> fieldsInfo = null)
		{
			fieldsInfo = (fieldsInfo ?? GetTypeFieldList(type, null, false));
			string exp2 = EscapeSpecialChar(exp);
			exp2 = " " + AddSpace(exp2) + " ";
			exp2 = HandleDateTime(exp2);
			exp2 = HandleSpecialMethod(exp2);
			exp2 = RepalceApiMember(ctx, exp2, fieldsInfo);
			exp2 = AddLambdaPrefix(type, exp2, fieldsInfo);
			CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
			IVInvoiceModel iVInvoiceModel = new IVInvoiceModel();
			MContext mContext = new MContext();
			ApiParam apiParam = new ApiParam();
			iVInvoiceModel = null;
			mContext = null;
			apiParam = null;
			ICodeCompiler codeCompiler = cSharpCodeProvider.CreateCompiler();
			IEnumerable<string> source = from a in AppDomain.CurrentDomain.GetAssemblies()
			where !a.IsDynamic
			select a.Location;
			CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.ReferencedAssemblies.AddRange(source.ToArray());
			compilerParameters.GenerateExecutable = false;
			compilerParameters.GenerateInMemory = true;
			string source2 = "\r\n                using System;\r\n                using System.Collections.Generic;\r\n                using JieNor.Megi.DataModel.IV;\r\n                using JieNor.Megi.DataModel.GL;\r\n                using JieNor.Megi.DataModel.BD;\r\n                using JieNor.Megi.DataModel.FA;\r\n                using JieNor.Megi.DataModel.FP;\r\n                using JieNor.Megi.DataModel.FC;\r\n                using JieNor.Megi.DataModel.IO;\r\n                using JieNor.Megi.DataModel.PA;\r\n                using JieNor.Megi.DataModel.RI;\r\n                using JieNor.Megi.DataModel.BAS;\r\n                using JieNor.Megi.DataModel.SEC;\r\n                using JieNor.Megi.DataModel.SYS;\r\n                using JieNor.Megi.DataModel.REG;\r\n                using JieNor.Megi.Core.DataModel;\r\n                using JieNor.Megi.EntityModel.Enum;\r\n                using JieNor.Megi.Core.Attribute;\r\n                using System.Linq;\r\n\r\n                namespace JieNor.Megi.Dynamic\r\n                {\r\n                    public class WhereExpressionTestClass{\r\n\r\n                        public IEnumerable<" + type.Name + "> WhereExpressionTest()\r\n                        {\r\n                            var objList = new List<" + type.Name + ">();\r\n\r\n                            return objList.Where(" + LAMBDA_PREFIX + " => " + exp2 + ");\r\n                        } \r\n                    }\r\n                }";
			CompilerResults compilerResults = codeCompiler.CompileAssemblyFromSource(compilerParameters, source2);
			if (!compilerResults.Errors.HasErrors || ValidateErrors(compilerResults.Errors))
			{
				return;
			}
			string text = string.Join(";", compilerResults.Errors);
			throw new ExpressionException(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "WhereExpressionError", "Where条件表达式错误"));
		}

		private static bool ValidateErrors(CompilerErrorCollection errors)
		{
			bool result = true;
			Regex regex = new Regex("运算符“(>|<|>=|<=|==|!=)”无法应用于“(string|decimal|double)”和“(string|double|decimal)”类型的操作数");
			Regex regex2 = new Regex("Operator '(>|<|>=|<=|==|!=)' cannot be applied to operands of type '(string|decimal|double)' and '(string|double|decimal)'");
			for (int i = 0; i < errors.Count; i++)
			{
				if (!regex.IsMatch(errors[i].ToString()) && !regex2.IsMatch(errors[i].ToString()))
				{
					result = false;
				}
			}
			return result;
		}

		public static string GetOrderBy(MContext ctx, string orderBy, List<APIFieldModel> fieldsInfo)
		{
			if (string.IsNullOrWhiteSpace(orderBy))
			{
				return null;
			}
			orderBy = " " + orderBy.Trim() + " ";
			APIFieldModel entryField = fieldsInfo.FirstOrDefault((APIFieldModel x) => x.IsDetail);
			List<APIFieldModel> list = (from x in fieldsInfo
			where entryField == null || !x.Name.StartsWith(entryField.Name)
			select x).ToList();
			bool flag = false;
			for (int i = 0; i < list.Count; i++)
			{
				Regex regex = new Regex("(?<=(\\s|,))" + list[i].ApiName.Replace("_", ".") + "(?=\\s+(desc|asc)*)");
				bool success = regex.Match(orderBy).Success;
				flag |= success;
				if (success)
				{
					orderBy = regex.Replace(orderBy, list[i].Name);
					regex = new Regex("(?<=(\\s|,))" + list[i].Name + "(\\s+(desc|asc|DESC|ASC)*)*\\s+$");
					flag = (flag && regex.IsMatch(orderBy));
				}
			}
			if (!flag)
			{
				throw new ExpressionException(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "OrderExpressionError", "Order表达式错误"));
			}
			return orderBy.TrimStart();
		}

		private static string AddSpace(string exp)
		{
			List<string> list = new List<string>();
			List<string> list2 = (from x in exp.ToCharArray()
			select x.ToString()).ToList();
			bool flag = false;
			string a = null;
			for (int i = 0; i < list2.Count; i++)
			{
				string text = list2[i];
				if (text == "\"")
				{
					flag = (!flag && true);
				}
				else if (text == "&")
				{
					if (!flag)
					{
						if (a != "&")
						{
							list.Add(" ");
						}
						else
						{
							text += " ";
						}
					}
				}
				else if (text == "|")
				{
					if (!flag)
					{
						if (a != "|")
						{
							list.Add(" ");
						}
						else
						{
							text += " ";
						}
					}
				}
				else if (text == "(")
				{
					if (!flag)
					{
						text = " " + text + " ";
					}
				}
				else if (text == ")")
				{
					if (!flag)
					{
						text = " " + text + " ";
					}
				}
				else if (text == ">")
				{
					if (!flag && i != list2.Count - 1 && list2[i + 1] != "=")
					{
						text += " ";
					}
					if (!flag && i != 0 && list2[i - 1] != "=")
					{
						text = " " + text;
					}
				}
				else if (text == "<")
				{
					if (!flag)
					{
						text = " " + text;
					}
					if (!flag && i != list2.Count - 1 && list2[i + 1] != "=")
					{
						text += " ";
					}
				}
				else if (text == "!")
				{
					if (!flag && i != list2.Count - 1 && list2[i + 1] == "=")
					{
						text = " " + text;
					}
				}
				else if (text == "=")
				{
					if (!flag && i != 0 && list2[i - 1] != "=" && list2[i - 1] != "<" && list2[i - 1] != ">" && list2[i - 1] != "!")
					{
						text = " " + text;
					}
					if (!flag && i != list2.Count - 1 && list2[i + 1] != "=" && list2[i + 1] != ">")
					{
						text += " ";
					}
				}
				else if ((text == "\n" || text == "\r") && !flag)
				{
					text = "";
				}
				list.Add(text);
				a = text;
			}
			return string.Join("", list.ToArray());
		}

		private static string ReplaceDateTime(string exp)
		{
			Match match = Regex.Match(exp, "( DateTime\\s*\\((.*?)\\))");
			while (match.Success)
			{
				if (match.Groups.Count == 3)
				{
					string oldValue = match.Groups[1].ToString();
					string value = match.Groups[2].ToString();
					exp = exp.Replace(oldValue, " " + ParseDatetime(value) + " ");
				}
				match = match.NextMatch();
			}
			return exp;
		}

		private static string HandleUpdateDateUTC(string exp)
		{
			Regex regex = new Regex("(?<=UpdatedDateUTC\\s*(==|!=|<=|>=|<|>){1}\\s*DateTime\\s*\\()(.+?)(?=\\))");
			Match match = regex.Match(exp);
			int num = 0;
			while (match.Success)
			{
				string replacement = ToDateTimeString(match.Groups[2].ToString().Trim(), true);
				exp = regex.Replace(exp, replacement, 1, num++);
				match = match.NextMatch();
			}
			return exp;
		}

		private static string FindParameter(string exp)
		{
			Regex regex = new Regex("(\\s(>=|<=|!=|==|>|<)\\s*(.*?)\\s)");
			Match match = regex.Match(exp);
			exp = regex.Replace(exp, " $2 " + CENTER + "$3" + CENTER + " ");
			return exp;
		}

		private static string TrimSpace(string exp)
		{
			Regex regex = new Regex("\\s{2,}");
			exp = regex.Replace(exp, " ");
			return exp;
		}

		private static string RepaceWithSpecialChar(string exp)
		{
			List<string> list = new List<string>();
			List<string> list2 = (from x in exp.ToCharArray()
			select x.ToString()).ToList();
			bool flag = false;
			for (int i = 0; i < list2.Count; i++)
			{
				string text = list2[i];
				if (text == "\"")
				{
					flag = (!flag && true);
				}
				else if (text == "(")
				{
					if (flag)
					{
						text = LEFT;
					}
				}
				else if (text == ")")
				{
					if (flag)
					{
						text = RIGHT;
					}
				}
				else if (text == " ")
				{
					if (flag)
					{
						text = SPACE;
					}
				}
				else if (text == "&")
				{
					if (flag)
					{
						text = AND;
					}
				}
				else if (text == "|")
				{
					if (flag)
					{
						text = OR;
					}
				}
				else if (text == ".")
				{
					if (flag)
					{
						text = DOT;
					}
				}
				else if (text == "=" && flag)
				{
					text = EQ;
				}
				list.Add(text);
			}
			return string.Join("", list.ToArray());
		}

		private static string MarkMethodParameter(string exp)
		{
			Regex regex = new Regex("(\\.\\s*Contains\\s*\\(\\s*(.*?)\\s*\\))");
			exp = regex.Replace(exp, ".Contains" + LEFT + CENTER + "$2" + CENTER + RIGHT + " ");
			regex = new Regex("(\\.\\s*StartsWith\\s*\\(\\s*(.*?)\\s*\\))");
			exp = regex.Replace(exp, ".StartsWith" + LEFT + CENTER + "$2" + CENTER + RIGHT + " ");
			regex = new Regex("(\\.\\s*EndsWith\\s*\\(\\s*(.*?)\\s*\\))");
			exp = regex.Replace(exp, ".EndsWith" + LEFT + CENTER + "$2" + CENTER + RIGHT + " ");
			regex = new Regex("(\\.\\s*NotContains\\s*\\(\\s*(.*?)\\s*\\))");
			exp = regex.Replace(exp, ".NotContains" + LEFT + CENTER + "$2" + CENTER + RIGHT + " ");
			regex = new Regex("(\\.\\s*NotStartsWith\\s*\\(\\s*(.*?)\\s*\\))");
			exp = regex.Replace(exp, ".NotStartsWith" + LEFT + CENTER + "$2" + CENTER + RIGHT + " ");
			regex = new Regex("(\\.\\s*NotEndsWith\\s*\\(\\s*(.*?)\\s*\\))");
			exp = regex.Replace(exp, ".NotEndsWith" + LEFT + CENTER + "$2" + CENTER + RIGHT + " ");
			return exp;
		}

		private static string GetMultiLangWhereExp(MContext ctx, string str)
		{
			List<string> list = new List<string>
			{
				ctx.MLCID
			};
			string str2 = "(";
			for (int i = 0; i < list.Count; i++)
			{
				str2 = str2 + string.Format(str, list[i]) + ((i == list.Count - 1) ? " " : " || ");
			}
			return str2 + " )";
		}

		private static string HandleOperator(MContext ctx, string exp, List<APIFieldModel> fieldsInfo)
		{
			List<string> list = new List<string>
			{
				ctx.MLCID
			};
			for (int i = 0; i < fieldsInfo.Count; i++)
			{
				APIFieldModel aPIFieldModel = fieldsInfo[i];
				bool flag = ctx.MultiLang && aPIFieldModel.ApiAttr.MemberType == ApiMemberType.MultiLang;
				if (flag)
				{
					Regex regex = new Regex("(\\s|\\.)(" + aPIFieldModel.ApiName.Replace("_", "\\.") + ")\\s*(==|!=|>|<|>=|<=)\\s*" + CENTER + "(.*?)" + CENTER + "\\s*");
					string multiLangWhereExp = GetMultiLangWhereExp(ctx, "$1$2_{0} $3 " + CENTER + "$4" + CENTER);
					exp = regex.Replace(exp, multiLangWhereExp);
				}
				Regex regex2 = new Regex("(\\s|\\.)(" + aPIFieldModel.ApiName.Replace("_", "\\.") + ")(\\.\\s*Contains\\s*" + LEFT + "\\s*" + CENTER + "(.*?)" + CENTER + "\\s*" + RIGHT + ")");
				if (regex2.Match(exp).Success)
				{
					if (flag)
					{
						string multiLangWhereExp2 = GetMultiLangWhereExp(ctx, "$1$2_{0} like concat" + LEFT + "'%'," + CENTER + "$4" + CENTER + ",'%'" + RIGHT);
						exp = regex2.Replace(exp, multiLangWhereExp2);
					}
					else
					{
						exp = regex2.Replace(exp, "$1$2 like concat" + LEFT + "'%'," + CENTER + "$4" + CENTER + ",'%'" + RIGHT + " ");
					}
				}
				regex2 = new Regex("(\\s|\\.)(" + aPIFieldModel.ApiName.Replace("_", "\\.") + ")(\\.\\s*StartsWith\\s*" + LEFT + "\\s*" + CENTER + "(.*?)" + CENTER + "\\s*" + RIGHT + ")");
				if (regex2.Match(exp).Success)
				{
					if (flag)
					{
						string multiLangWhereExp3 = GetMultiLangWhereExp(ctx, "$1$2_{0} like concat" + LEFT + "''," + CENTER + "$4" + CENTER + ",'%'" + RIGHT + " ");
						exp = regex2.Replace(exp, multiLangWhereExp3);
					}
					else
					{
						exp = regex2.Replace(exp, "$1$2 like concat" + LEFT + "''," + CENTER + "$4" + CENTER + ",'%'" + RIGHT + " ");
					}
				}
				regex2 = new Regex("(\\s|\\.)(" + aPIFieldModel.ApiName.Replace("_", "\\.") + ")(\\.\\s*EndsWith\\s*" + LEFT + "\\s*" + CENTER + "(.*?)" + CENTER + "\\s*" + RIGHT + ")");
				if (regex2.Match(exp).Success)
				{
					if (flag)
					{
						string multiLangWhereExp4 = GetMultiLangWhereExp(ctx, "$1$2_{0} like concat" + LEFT + "'%'," + CENTER + "$4" + CENTER + ",''" + RIGHT + " ");
						exp = regex2.Replace(exp, multiLangWhereExp4);
					}
					else
					{
						exp = regex2.Replace(exp, "$1$2 like concat" + LEFT + "'%'," + CENTER + "$4" + CENTER + ",''" + RIGHT + " ");
					}
				}
				regex2 = new Regex("(\\s|\\.)(" + aPIFieldModel.ApiName.Replace("_", "\\.") + ")(\\.\\s*NotContains\\s*" + LEFT + "\\s*" + CENTER + "(.*?)" + CENTER + "\\s*" + RIGHT + ")");
				if (regex2.Match(exp).Success)
				{
					if (flag)
					{
						string multiLangWhereExp5 = GetMultiLangWhereExp(ctx, "$1$2_{0} not like concat" + LEFT + "'%'," + CENTER + "$4" + CENTER + ",'%'" + RIGHT + " ");
						exp = regex2.Replace(exp, multiLangWhereExp5);
					}
					else
					{
						exp = regex2.Replace(exp, "$1$2 not like concat" + LEFT + "'%'," + CENTER + "$4" + CENTER + ",'%'" + RIGHT + " ");
					}
				}
				regex2 = new Regex("(\\s|\\.)(" + aPIFieldModel.ApiName.Replace("_", "\\.") + ")(\\.\\s*NotStartsWith\\s*" + LEFT + "\\s*" + CENTER + "(.*?)" + CENTER + "\\s*" + RIGHT + ")");
				if (regex2.Match(exp).Success)
				{
					if (flag)
					{
						string multiLangWhereExp6 = GetMultiLangWhereExp(ctx, "$1$2_{0} not like concat" + LEFT + "''," + CENTER + "$4" + CENTER + ",'%'" + RIGHT + " ");
						exp = regex2.Replace(exp, multiLangWhereExp6);
					}
					else
					{
						exp = regex2.Replace(exp, "$1$2 not like concat" + LEFT + "''," + CENTER + "$4" + CENTER + ",'%'" + RIGHT + " ");
					}
				}
				regex2 = new Regex("(\\s|\\.)(" + aPIFieldModel.ApiName.Replace("_", "\\.") + ")(\\.\\s*NotEndsWith\\s*" + LEFT + "\\s*" + CENTER + "(.*?)" + CENTER + "\\s*" + RIGHT + ")");
				if (regex2.Match(exp).Success)
				{
					if (flag)
					{
						string multiLangWhereExp7 = GetMultiLangWhereExp(ctx, "$1$2_{0} not like concat" + LEFT + "'%'," + CENTER + "$4" + CENTER + ",''" + RIGHT + " ");
						exp = regex2.Replace(exp, multiLangWhereExp7);
					}
					else
					{
						exp = regex2.Replace(exp, "$1$2 not like concat" + LEFT + "'%'," + CENTER + "$4" + CENTER + ",''" + RIGHT + " ");
					}
				}
			}
			return exp;
		}

		private static string GetLangString(MContext ctx)
		{
			if (ctx.MActiveLocaleIDS == null || ctx.MActiveLocaleIDS.Count == 0)
			{
				return "(0x0009|0x7804|0x7C04)";
			}
			return "(" + string.Join("|", ctx.MActiveLocaleIDS) + ")";
		}

		private static string ReplaceLambda(string exp)
		{
			Regex regex = new Regex("(?<=(\\S+)\\s*\\.\\s*(Exists|Where)\\s*\\(\\s*(\\S+)\\s*=>.*?)(\\3\\s*\\.)(?=\\s*(\\S+).*?\\))");
			while (regex.Match(exp).Success)
			{
				exp = regex.Replace(exp, "$1.");
			}
			Regex regex2 = new Regex("(\\S+?)((\\.|\\s)(Exists|Where)\\s*\\(\\s*\\S+\\s*=>(\\s*\\S+.+?)\\))");
			while (regex2.Match(exp).Success)
			{
				exp = regex2.Replace(exp, "$5");
			}
			return exp;
		}

		private static string ReplaceNull(string exp)
		{
			Regex regex = new Regex(" !=\\s*?null ");
			while (regex.Match(exp).Success)
			{
				exp = regex.Replace(exp, " is not null ", 1);
			}
			regex = new Regex(" =\\s*?null ");
			while (regex.Match(exp).Success)
			{
				exp = regex.Replace(exp, " is null ", 1);
			}
			return exp;
		}

		private static string RepalceLogicOperator(string exp)
		{
			exp = exp.Replace("&&", " and ").Replace("||", " or ");
			return exp;
		}

		private static KeyValuePair<string, List<MySqlParameter>> GetFinalSqlParameter(MContext ctx, Type type, string exp, List<APIFieldModel> fieldsInfo)
		{
			List<MySqlParameter> list = new List<MySqlParameter>();
			exp = " " + exp.Trim() + " ";
			string arg = "@API_";
			bool flag = true;
			List<string> list2 = new List<string>
			{
				ctx.MLCID
			};
			string langExp = "(";
			list2.ForEach(delegate(string x)
			{
				langExp = langExp + "_" + x + "|\\s";
			});
			langExp += ")";
			for (int i = 0; i < fieldsInfo.Count; i++)
			{
				string apiName = fieldsInfo[i].ApiName;
				string name = fieldsInfo[i].Name;
				PropertyInfo prop = fieldsInfo[i].Prop;
				string enumMappingType = fieldsInfo[i].EnumMappingType;
				Regex regex = new Regex(" " + apiName.Replace("_", ".") + langExp + "(?=.*?" + CENTER + ".*?" + CENTER + ")");
				exp = regex.Replace(exp, " " + name + "$1 ");
				regex = new Regex("(?<= " + name + langExp + ".*?)" + CENTER + ".*?" + CENTER);
				int count = 1;
				int num = 0;
				while (regex.Match(exp).Success)
				{
					string text = regex.Match(exp).Value.Replace(CENTER, "");
					if (text != "null")
					{
						string text2 = arg + name + num++;
						if (enumMappingType != null)
						{
							text = BASEnumMappingRepository.GetInternalValue(ctx, enumMappingType, text, null, true);
							flag = !string.IsNullOrWhiteSpace(text);
						}
						object value = (!flag && prop.PropertyType != typeof(string)) ? "-999999" : GetValue(prop, RecoverSpecialChar(text));
						exp = regex.Replace(exp, text2, count);
						list.Add(new MySqlParameter
						{
							ParameterName = text2,
							Value = value
						});
					}
					Regex regex2 = new Regex(" " + name + langExp + " ");
					exp = regex2.Replace(exp, PREFIX + name + "$1 ", count);
				}
			}
			exp = exp.Replace(PREFIX, " ");
			exp = TrimSpace(exp);
			exp = exp.Replace("==", "=");
			exp = RecoverSpecialChar(exp);
			exp = ReplaceNull(exp);
			return new KeyValuePair<string, List<MySqlParameter>>(exp, list);
		}

		private static string JoinField(string exp)
		{
			exp = exp.Replace(".", "_");
			return exp;
		}

		private static string RecoverSpecialChar(string exp)
		{
			exp = exp.Replace(LEFT, "(").Replace(RIGHT, ")").Replace(DOT, ".")
				.Replace(AND, "&")
				.Replace(OR, "|")
				.Replace(PREFIX, "")
				.Replace(SPACE, " ")
				.Replace(CENTER, "")
				.Replace(EQ, "=");
			return exp;
		}

		private static string ParseDatetime(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return "";
			}
			string[] array = value.Split(',');
			if (array.Count() == 3)
			{
				return "\"" + array[0].Trim() + "-" + array[1].Trim() + "-" + array[2].Trim() + "\"";
			}
			if (array.Count() == 6)
			{
				return "\"" + array[0].Trim() + "-" + array[1].Trim() + "-" + array[2].Trim() + SPACE + array[3].Trim() + ":" + array[4].Trim() + ":" + array[5].Trim() + "\"";
			}
			return null;
		}

		private static string ToDateTimeString(string value, bool isUtc = false)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return "1,1,1,00,00,00";
			}
			string[] array = value.Split(',');
			int num = isUtc ? 8 : 0;
			DateTime dateTime;
			if (array.Count() == 3)
			{
				dateTime = new DateTime(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]));
				dateTime = dateTime.AddHours((double)num);
				return dateTime.ToString("yyyy,MM,dd");
			}
			if (array.Count() == 6)
			{
				dateTime = new DateTime(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]), int.Parse(array[4]), int.Parse(array[5]));
				dateTime = dateTime.AddHours((double)num);
				return dateTime.ToString("yyyy,MM,dd,HH,mm,ss");
			}
			return "1,1,1,00,00,00";
		}

		private static string HandleDateTime(string exp)
		{
			Regex regex = new Regex("(\\s+DateTime\\s*\\()");
			exp = regex.Replace(exp, " new$1");
			return exp;
		}

		private static string HandleSpecialMethod(string exp)
		{
			Regex regex = new Regex("(?<=\\.\\s*)(NotContains)(?=\\s*\\(\\s*(.*?)\\s*\\))");
			exp = regex.Replace(exp, "Contains");
			regex = new Regex("(?<=\\.\\s*)(NotStartsWith)(?=\\s*\\(\\s*(.*?)\\s*\\))");
			exp = regex.Replace(exp, "StartsWith");
			regex = new Regex("(?<=\\.\\s*)(NotEndsWith)(?=\\s*\\(\\s*(.*?)\\s*\\))");
			exp = regex.Replace(exp, "EndsWith");
			return exp;
		}

		private static string AddLambdaPrefix(Type type, string exp, List<APIFieldModel> fieldsInfo)
		{
			for (int i = 0; i < fieldsInfo.Count; i++)
			{
				Regex regex = new Regex("(\\s" + fieldsInfo[i].Name + "[\\s|\\.]?)");
				exp = regex.Replace(exp, LAMBDA_PREFIX + ".$1");
				exp = exp.Replace(LAMBDA_PREFIX + ". ", LAMBDA_PREFIX + ".");
			}
			return exp;
		}

		private static string MarkValue(string value)
		{
			return CENTER + value + CENTER;
		}

		private static bool IsPrimitive(Type type)
		{
			if (type.IsPrimitive)
			{
				return true;
			}
			if (type == typeof(DateTime))
			{
				return true;
			}
			if (type == typeof(int))
			{
				return true;
			}
			if (type == typeof(string))
			{
				return true;
			}
			if (type == typeof(long))
			{
				return true;
			}
			if (type == typeof(short))
			{
				return true;
			}
			if (type == typeof(decimal))
			{
				return true;
			}
			if (type == typeof(bool))
			{
				return true;
			}
			return false;
		}

		public static object GetValue(PropertyInfo info, object value)
		{
			if (info.PropertyType == typeof(decimal))
			{
				return (value == null) ? 0.0m : Convert.ToDecimal(value);
			}
			if (info.PropertyType == typeof(short))
			{
				return (value != null) ? Convert.ToInt16(Convert.ToDecimal(value)) : 0;
			}
			if (info.PropertyType == typeof(int))
			{
				return (value != null) ? Convert.ToInt32(Convert.ToDecimal(value)) : 0;
			}
			if (info.PropertyType == typeof(long))
			{
				return (value == null) ? 0 : Convert.ToInt64(Convert.ToDecimal(value));
			}
			if (info.PropertyType == typeof(bool))
			{
				string text = Convert.ToString(value);
				if (value == null || value == DBNull.Value)
				{
					return false;
				}
				if (text.Equals("1"))
				{
					return true;
				}
				bool flag = false;
				bool.TryParse(text, out flag);
				return flag;
			}
			if (info.PropertyType == typeof(DateTime))
			{
				if (value == null)
				{
					return DateTime.MinValue;
				}
				value = value.ToString().TrimStart('"').TrimEnd('"');
				if (string.IsNullOrWhiteSpace(value.ToString()))
				{
					return DateTime.MinValue;
				}
				DateTime dateTime = Convert.ToDateTime(value);
				return dateTime;
			}
			if (info.PropertyType == typeof(string))
			{
				if (value == null)
				{
					return null;
				}
				value = value.ToString().TrimStart('"').TrimEnd('"');
				return value;
			}
			if (info.PropertyType == typeof(string[]))
			{
				if (value == null)
				{
					return null;
				}
				if (value is string)
				{
					return Convert.ToString(value).Split(',');
				}
				return value;
			}
			return value;
		}

		public static bool IsListProperty(PropertyInfo prop)
		{
			if (prop.PropertyType.FullName.Contains("System.Collections.Generic.List"))
			{
				return true;
			}
			return false;
		}

		public static string Format(string src, params object[] values)
		{
			if (string.IsNullOrWhiteSpace(src))
			{
				return src;
			}
			for (int i = 0; i < values.Count(); i++)
			{
				src = src.Replace("{" + i + "}", (values[i] == null) ? "" : values[i].ToString());
			}
			return src;
		}

		public static string GetListPropertyGenericType(PropertyInfo prop)
		{
			string fullName = prop.PropertyType.FullName;
			GroupCollection groups = Regex.Match(fullName, "(\\[\\[.+?\\]\\])").Groups;
			if (groups.Count == 2)
			{
				return groups[1].ToString().Replace("[[", "").Replace("]]", "");
			}
			return null;
		}
	}
}
