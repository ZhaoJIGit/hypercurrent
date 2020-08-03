using Fasterflect;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.MultiLanguage;
using System;
using System.Linq;
using System.Reflection;
using System.Web;

namespace JieNor.Megi.Core.Helper
{
	public class ModelHelper
	{
		public static void CopyModelValue<T1, T2>(T1 srcModel, T2 tgtModel)
		{
			bool isParseDecimal0ToNull = true;
			CopyModelValue(srcModel, tgtModel, isParseDecimal0ToNull);
		}

		public static void CopyModelValue<T1, T2>(T1 srcModel, T2 tgtModel, bool isParseDecimal0ToNull)
		{
			Type type = srcModel.GetType();
			Type type2 = tgtModel.GetType();
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanRead)
				{
					Type propertyType = propertyInfo.PropertyType;
					PropertyInfo property = type2.GetProperty(propertyInfo.Name);
					if (property != (PropertyInfo)null && property.CanWrite)
					{
						Type propertyType2 = property.PropertyType;
						object value = propertyInfo.GetValue(srcModel, null);
						DateTime dateTime2;
						if (propertyType2 == typeof(string))
						{
							if (propertyType == typeof(decimal))
							{
								decimal num = default(decimal);
								if (decimal.TryParse(Convert.ToString(value), out num))
								{
									property.SetValue(tgtModel, num.ToString("n4"), null);
								}
							}
							else if (propertyType == typeof(DateTime))
							{
								DateTime dateTime;
								if (DateTime.TryParse(Convert.ToString(value), out dateTime) && dateTime != DateTime.MinValue && dateTime.Year != 1)
								{
									property.SetValue(tgtModel, dateTime.ToOrgZoneDateString(null), null);
								}
							}
							else if (propertyType == typeof(bool))
							{
								property.SetValue(tgtModel, Convert.ToBoolean(value) ? "1" : "0", null);
							}
							else
							{
								property.SetValue(tgtModel, HttpUtility.HtmlDecode(Convert.ToString(value)), null);
							}
						}
						else if (propertyType2 == typeof(int))
						{
							property.SetValue(tgtModel, Convert.ToInt32(value), null);
						}
						else if (propertyType2 == typeof(decimal) || propertyType2 == typeof(decimal?))
						{
							decimal num2 = default(decimal);
							if (decimal.TryParse(Convert.ToString(value), out num2))
							{
								property.SetValue(tgtModel, isParseDecimal0ToNull ? num2.ZeroToNull() : new decimal?(num2), null);
							}
						}
						else if (propertyType2 == typeof(bool))
						{
							property.SetValue(tgtModel, Convert.ToBoolean(value), null);
						}
						else if (propertyType2 == typeof(DateTime) && DateTime.TryParse(Convert.ToString(value), out dateTime2) && dateTime2 != DateTime.MinValue && dateTime2.Year != 1)
						{
							property.SetValue(tgtModel, dateTime2, null);
						}
					}
				}
			}
		}

		public static void SetModelValue<T>(T model, string propName, object value, PropertyInfo[] propList = null)
		{
			if (propList == null)
			{
				propList = typeof(T).GetProperties();
			}
			PropertyInfo propertyInfo = propList.SingleOrDefault((PropertyInfo f) => f.Name.EqualsIgnoreCase(propName));
			if (!(propertyInfo == (PropertyInfo)null))
			{
				if (propertyInfo.PropertyType == typeof(decimal))
				{
					decimal num = default(decimal);
					if (decimal.TryParse(Convert.ToString(value), out num))
					{
						propertyInfo.SetValue(model, num);
					}
				}
				else if (propertyInfo.PropertyType == typeof(int))
				{
					propertyInfo.SetValue(model, Convert.ToInt32(value));
				}
				else if (propertyInfo.PropertyType == typeof(bool))
				{
					propertyInfo.SetValue(model, Convert.ToBoolean(value));
				}
				else
				{
					propertyInfo.SetValue(model, value);
				}
			}
		}

		public static string TryGetModelValue<T>(T model, string propName)
		{
			return Convert.ToString(GetModelValueO(model, propName, true));
		}

		public static string GetModelValue<T>(T model, string propName)
		{
			return Convert.ToString(GetModelValueO(model, propName, false));
		}

		public static object GetModelValueO<T>(T model, string propName, bool isTryGet = false)
		{
			if (isTryGet)
			{
				return model.TryGetPropertyValue(propName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
			}
			return model.GetPropertyValue(propName);
		}

		public static decimal GetModelValueD<T>(T model, string propName)
		{
			return Convert.ToDecimal(GetModelValueO(model, propName, false));
		}
	}
}
