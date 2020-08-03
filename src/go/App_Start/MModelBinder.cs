using DevExpress.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web
{
	public class MModelBinder : DevExpressEditorsBinder
	{
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (controllerContext.RequestContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
			{
				ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
				if (bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName) && value != null)
				{
					if (bindingContext.ModelMetadata.IsComplexType)
					{
						object obj = JsonConvert.DeserializeObject(value.AttemptedValue, bindingContext.ModelType, new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore,
							ObjectCreationHandling = ObjectCreationHandling.Replace,
							DateTimeZoneHandling = DateTimeZoneHandling.Local
						});
						if (obj == null)
						{
							obj = Assembly.GetAssembly(bindingContext.ModelType).CreateInstance(bindingContext.ModelType.FullName);
						}
						return obj;
					}
					return GetValue(bindingContext.ModelType, value.AttemptedValue);
				}
			}
			return base.BindModel(controllerContext, bindingContext);
		}

		protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
		}

		protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
		{
			return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
		}

		private static object GetDefaultValue(Type type)
		{
			if (type == typeof(ushort))
			{
				return (ushort)0;
			}
			if (type == typeof(uint))
			{
				return 0u;
			}
			if (type == typeof(ulong))
			{
				return 0uL;
			}
			if (type == typeof(short))
			{
				return (short)0;
			}
			if (type == typeof(int))
			{
				return 0;
			}
			if (type == typeof(long))
			{
				return 0L;
			}
			if (type == typeof(float))
			{
				return 0f;
			}
			if (type == typeof(bool))
			{
				return false;
			}
			if (type == typeof(byte))
			{
				return (byte)0;
			}
			if (type == typeof(sbyte))
			{
				return (sbyte)0;
			}
			if (type == typeof(char))
			{
				return '\0';
			}
			if (type == typeof(string))
			{
				return null;
			}
			if (type == typeof(decimal))
			{
				return decimal.Zero;
			}
			if (type == typeof(DateTime))
			{
				return default(DateTime);
			}
			if (type == typeof(Enum))
			{
				return null;
			}
			return null;
		}

		private static object GetValue(Type type, string value)
		{
			try
			{
				if (type == typeof(ushort) || Nullable.GetUnderlyingType(type) == typeof(ushort))
				{
					return Convert.ToUInt16(value);
				}
				if (type == typeof(uint) || Nullable.GetUnderlyingType(type) == typeof(uint))
				{
					return Convert.ToUInt32(value);
				}
				if (type == typeof(ulong) || Nullable.GetUnderlyingType(type) == typeof(ulong))
				{
					return Convert.ToUInt64(value);
				}
				if (type == typeof(short) || Nullable.GetUnderlyingType(type) == typeof(short))
				{
					return Convert.ToInt16(value);
				}
				if (type == typeof(int) || Nullable.GetUnderlyingType(type) == typeof(int))
				{
					return Convert.ToInt32(value);
				}
				if (type == typeof(long) || Nullable.GetUnderlyingType(type) == typeof(long))
				{
					return Convert.ToInt64(value);
				}
				if (type == typeof(float) || Nullable.GetUnderlyingType(type) == typeof(float))
				{
					return Convert.ToSingle(value);
				}
				if (type == typeof(bool) || Nullable.GetUnderlyingType(type) == typeof(bool))
				{
					return Convert.ToBoolean(value);
				}
				if (type == typeof(byte) || Nullable.GetUnderlyingType(type) == typeof(byte))
				{
					return Convert.ToByte(value);
				}
				if (type == typeof(sbyte) || Nullable.GetUnderlyingType(type) == typeof(sbyte))
				{
					return Convert.ToSByte(value);
				}
				if (type == typeof(char) || Nullable.GetUnderlyingType(type) == typeof(char))
				{
					return Convert.ToChar(value);
				}
				if (type == typeof(string) || Nullable.GetUnderlyingType(type) == typeof(string))
				{
					return Convert.ToString(value);
				}
				if (type == typeof(decimal) || Nullable.GetUnderlyingType(type) == typeof(decimal))
				{
					return Convert.ToDecimal(value);
				}
				if (type == typeof(DateTime) || Nullable.GetUnderlyingType(type) == typeof(DateTime))
				{
					return Convert.ToDateTime(value);
				}
				if (type.BaseType == typeof(Enum) || Nullable.GetUnderlyingType(type.BaseType) == typeof(Enum))
				{
					return Enum.Parse(type, value);
				}
			}
			catch
			{
				return GetDefaultValue(type);
			}
			return null;
		}
	}
}
