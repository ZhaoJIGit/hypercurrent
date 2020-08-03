using System;
using System.Linq;
using System.Reflection;

namespace JieNor.Megi.Common.Utility
{
	public class ModelPropertyHelper
	{
		public static void SetPropertyValue<T>(T model, string propertyName, string value)
		{
			PropertyInfo prop = model.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
			if (prop != (PropertyInfo)null && prop.CanWrite)
			{
				if (prop.PropertyType == typeof(bool))
				{
					prop.SetValue(model, value == "1");
				}
				else if (prop.PropertyType == typeof(DateTime))
				{
					if (!string.IsNullOrWhiteSpace(value))
					{
						prop.SetValue(model, Convert.ToDateTime(value.ToDateString()));
					}
				}
				else if (prop.PropertyType == typeof(decimal))
				{
					decimal tmpValue = default(decimal);
					if (decimal.TryParse(value, out tmpValue))
					{
						prop.SetValue(model, tmpValue);
					}
				}
				else if (prop.PropertyType == typeof(decimal?))
				{
					decimal d = default(decimal);
					if (decimal.TryParse(Convert.ToString(value), out d))
					{
						prop.SetValue(model, d.ZeroToNull(), null);
					}
				}
				else if (prop.PropertyType == typeof(int))
				{
					prop.SetValue(model, Convert.ToInt32(value));
				}
				else
				{
					prop.SetValue(model, value);
				}
			}
		}

		public static void CopyModelValue<T1, T2>(T1 srcModel, T2 tgtModel)
		{
			if (srcModel != null)
			{
				PropertyInfo[] srcPropList = srcModel.GetType().GetProperties();
				PropertyInfo[] properties = tgtModel.GetType().GetProperties();
				foreach (PropertyInfo tgtProp in properties)
				{
					PropertyInfo srcProp = srcPropList.SingleOrDefault((PropertyInfo f) => f.Name == tgtProp.Name && f.PropertyType == tgtProp.PropertyType);
					if (tgtProp != (PropertyInfo)null && tgtProp.CanWrite && srcProp != (PropertyInfo)null)
					{
						object srcValue = srcProp.GetValue(srcModel, null);
						Type tgtPropType = tgtProp.PropertyType;
						if (tgtPropType == typeof(decimal))
						{
							decimal d = default(decimal);
							decimal.TryParse(Convert.ToString(srcValue), out d);
							if (d > decimal.Zero)
							{
								tgtProp.SetValue(tgtModel, d, null);
							}
						}
						else if (tgtPropType == typeof(DateTime))
						{
							if (DateTime.TryParse(Convert.ToString(srcValue), out DateTime dt) && dt != DateTime.MinValue)
							{
								tgtProp.SetValue(tgtModel, Convert.ToDateTime(dt), null);
							}
						}
						else if (tgtPropType == typeof(int))
						{
							tgtProp.SetValue(tgtModel, Convert.ToInt32(srcValue), null);
						}
						else if (tgtPropType == typeof(bool))
						{
							tgtProp.SetValue(tgtModel, Convert.ToBoolean(srcValue), null);
						}
						else if (tgtPropType == typeof(string))
						{
							tgtProp.SetValue(tgtModel, Convert.ToString(srcValue), null);
						}
					}
				}
			}
		}
	}
}
