using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.COM;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity;

namespace JieNor.Megi.Tools
{
	public class ModelValidateFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			try
			{
				ActionDescriptor actionDescriptor = filterContext.ActionDescriptor;
				List<KeyValuePair<string, object>> list = filterContext.ActionParameters.ToList<KeyValuePair<string, object>>();
				string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
				string actionName = filterContext.ActionDescriptor.ActionName;
				if (!this.HasNoAuthorizationAttribute(filterContext) && AjaxRequestExtensions.IsAjaxRequest(filterContext.HttpContext.Request))
				{
					List<string> list2 = new List<string>();
					for (int i = 0; i < list.Count; i++)
					{
						object value = list[i].Value;
						this.IsValueFieldValid(value, list2);
					}
					if (list2.Count > 0)
					{
						filterContext.HttpContext.Response.StatusCode = 250;
						filterContext.Result = new JsonResult
						{
							Data = new
							{
								lengthOutOfRange = 1,
								list = list2
							},
							JsonRequestBehavior = 0
						};
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public void IsValueFieldValid(object parameter, List<string> stringList)
		{
			if (parameter != null && IsExtendDataModel(parameter.GetType()))
			{
				List<MTableColumnModel> tableColumnModels = TableCacheHelper.GetTableColumnModels((parameter as BaseModel).TableName);
				PropertyInfo[] properties = parameter.GetType().GetProperties();
				List<string> languageFieldNameList = GetLanguageFieldNameList(parameter, properties, "MultiLanguage");
				foreach (PropertyInfo propertyInfo in properties)
				{
					object value = propertyInfo.GetValue(parameter);
					if (value != null)
					{
						if (propertyInfo.GetCustomAttributes(typeof(ModelEntryAttribute), false).GetLength(0) > 0)
						{
							foreach (object item in (IList)value)
							{
								IsValueFieldValid(item, stringList);
							}
						}
						else
						{
							ValidateProperty(propertyInfo, value, tableColumnModels, languageFieldNameList, stringList);
						}
					}
				}
			}
			else if (parameter != null && !IsExtendDataModel(parameter.GetType()))
			{
				PropertyInfo[] properties2 = parameter.GetType().GetProperties();
				List<string> languageFieldNameList2 = GetLanguageFieldNameList(parameter, properties2, "MultiLanguage");
				foreach (PropertyInfo propertyInfo2 in properties2)
				{
					DBFieldValidateAttribute customAttribute = ((MemberInfo)propertyInfo2).GetCustomAttribute<DBFieldValidateAttribute>();
					if (customAttribute != null)
					{
						object value2 = propertyInfo2.GetValue(parameter);
						if (value2 != null)
						{
							List<MTableColumnModel> tableColumnModels2 = TableCacheHelper.GetTableColumnModels(customAttribute.TableName);
							ValidateProperty(propertyInfo2, value2, tableColumnModels2, languageFieldNameList2, stringList);
						}
					}
				}
			}
		}

		private void ValidateProperty(PropertyInfo propertyInfo, object value, List<MTableColumnModel> tableInfo, List<string> languageFieldNameList, List<string> stringList)
		{
			MTableColumnModel column = (from x in tableInfo
										where x.Name.ToLower().Equals(propertyInfo.Name.ToLower())
										select x).FirstOrDefault<MTableColumnModel>();
			if ((propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string)) && !languageFieldNameList.Exists((string x) => x == propertyInfo.Name))
			{
				this.IsValueTypeFiledValid(value, column, stringList);
				return;
			}
			if (propertyInfo.PropertyType == typeof(List<MultiLanguageFieldList>))
			{
				this.IsLangTypeFieldValid(value, tableInfo, stringList);
				return;
			}
			this.IsValueFieldValid(value, stringList);
		}

		private List<string> GetLanguageFieldNameList(object parameter, PropertyInfo[] propertyInfos, string mulitLanugageAttrName = "MultiLanguage")
		{
			List<string> result = new List<string>();
			if (propertyInfos == null || propertyInfos.Count<PropertyInfo>() == 0)
			{
				return result;
			}
			PropertyInfo propertyInfo = propertyInfos.ToList<PropertyInfo>().FirstOrDefault((PropertyInfo x) => x.Name == mulitLanugageAttrName);
			if (propertyInfo == null)
			{
				return result;
			}
			object value = propertyInfo.GetValue(parameter);
			if (value == null)
			{
				return result;
			}
			return (from x in value as List<MultiLanguageFieldList>
					select x.MFieldName).ToList<string>();
		}

		public void IsLangTypeFieldValid(object value, List<MTableColumnModel> columns, List<string> stringList)
		{
			List<MultiLanguageFieldList> list = value as List<MultiLanguageFieldList>;
			for (int i = 0; i < list.Count; i++)
			{
				MultiLanguageFieldList multiLanguageFieldList = list[i];
				string filedName = multiLanguageFieldList.MFieldName;
				MTableColumnModel mtableColumnModel = (from x in columns
													   where x.Name.Equals(filedName.ToLower())
													   select x).FirstOrDefault<MTableColumnModel>();
				for (int j = 0; j < multiLanguageFieldList.MMultiLanguageField.Count; j++)
				{
					string mvalue = multiLanguageFieldList.MMultiLanguageField[j].MValue;
					if (mtableColumnModel.MaxLength > 0 && !string.IsNullOrEmpty(mvalue) && mvalue.Length > mtableColumnModel.MaxLength)
					{
						stringList.Add(this.AssembleOutOfRange(filedName, mvalue, mtableColumnModel.MaxLength));
					}
				}
			}
		}

		private string AssembleOutOfRange(string filedName, string value, int maxLength)
		{
			value = ((value.Length < 6) ? value : (value.Substring(0, 6) + "..."));
			return string.Format(LangHelper.GetText(LangModule.Common, "InputValueTooLong", "输入的值:{0}超过了最大长度{1},请减少字符！"), value, maxLength);
		}

		public void IsValueTypeFiledValid(object value, MTableColumnModel column, List<string> stringList)
		{
			if (column != null && column.MaxLength > 0 && value.GetType() != typeof(bool))
			{
				string text = Convert.ToString(value);
				if (value.GetType() == typeof(string))
				{
					text = XSSFilter.XssFilter(text);
				}
				int num = column.MaxLength;
				if (column.Type.EqualsIgnoreCase("decimal"))
				{
					text = text.Split(new char[]
					{
						'.'
					})[0].TrimStart(new char[]
					{
						'-'
					});
					num = column.DecimalMaxLength;
				}
				if (text.Length > num)
				{
					stringList.Add(this.AssembleOutOfRange(column.Name, value.ToString(), num));
				}
			}
		}

		private bool IsExtendDataModel(Type type)
		{
			return type == typeof(BaseModel) || (!(type == typeof(object)) && this.IsExtendDataModel(type.BaseType));
		}

		private bool HasNoAuthorizationAttribute(ActionExecutingContext filterContext)
		{
			object[] customAttributes = filterContext.ActionDescriptor.GetCustomAttributes(false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				if (customAttributes[i].ToString().ToUpper() == "JieNor.Megi.Tools.NoModelValidateAttribute".ToUpper())
				{
					return true;
				}
			}
			return false;
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{
		}
	}
}
