using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.EntityModel.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JieNor.Megi.DataRepository.COM
{
	public class COMModelValidateHelper
	{
		public static List<string> ValidateModel(MContext ctx, object model)
		{
			List<string> list = new List<string>();
			IsValueFieldValid(ctx, model, list);
			return list;
		}

		public static bool ValidateModel<T>(MContext ctx, T model, OperationResult result) where T : BaseModel,new()
		{
			if (string.IsNullOrWhiteSpace(model.TableName))
			{
				model.TableName = new T().TableName;
			}
			List<string> list = ValidateModel(ctx, model);
			foreach (string item in list)
			{
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = item
				});
			}
			return !list.Any();
		}

		public static bool ValidateModel<T>(MContext ctx, T model, List<IOValidationResultModel> validationResult, IOValidationTypeEnum fieldType) where T : BaseModel,new()
		{
			if (string.IsNullOrWhiteSpace(model.TableName))
			{
				model.TableName = new T().TableName;
			}
			List<string> list = ValidateModel(ctx, model);
			if (list.Any())
			{
				validationResult.Add(new IOValidationResultModel
				{
					FieldType = fieldType,
					Message = string.Join("\r\n", list)
				});
			}
			return !list.Any();
		}

		public static void IsValueFieldValid(MContext ctx, object parameter, List<string> stringList)
		{
			if (parameter != null && IsExtendDataModel(parameter.GetType()))
			{
				Type type = parameter.GetType();
				BaseModel baseModel = parameter as BaseModel;
				List<MTableColumnModel> tableColumnModels = COMTableCacheHelper.GetTableColumnModels(ctx, baseModel.TableName);
				PropertyInfo[] properties = parameter.GetType().GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					object value = propertyInfo.GetValue(parameter);
					if (value != null)
					{
						MTableColumnModel column = (from x in tableColumnModels
						where x.Name.ToLower().Equals(propertyInfo.Name.ToLower())
						select x).FirstOrDefault();
						DBFieldValidateAttribute customAttribute = ((MemberInfo)propertyInfo).GetCustomAttribute<DBFieldValidateAttribute>();
						if (!(customAttribute?.IgnoreLengthValidate ?? false))
						{
							if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string))
							{
								IsValueTypeFiledValid(ctx, value, column, stringList);
							}
							else if (propertyInfo.PropertyType == typeof(List<MultiLanguageFieldList>))
							{
								IsLangTypeFieldValid(ctx, value, tableColumnModels, stringList);
							}
							else
							{
								IsValueFieldValid(ctx, value, stringList);
							}
						}
					}
				}
			}
		}

		public static void IsLangTypeFieldValid(MContext ctx, object value, List<MTableColumnModel> columns, List<string> stringList)
		{
			List<MultiLanguageFieldList> list = value as List<MultiLanguageFieldList>;
			for (int i = 0; i < list.Count; i++)
			{
				MultiLanguageFieldList multiLanguageFieldList = list[i];
				string filedName = multiLanguageFieldList.MFieldName;
				MTableColumnModel mTableColumnModel = columns.FirstOrDefault((MTableColumnModel x) => x.Name.Equals(filedName.ToLower()));
				for (int j = 0; j < multiLanguageFieldList.MMultiLanguageField.Count; j++)
				{
					string mValue = multiLanguageFieldList.MMultiLanguageField[j].MValue;
					if (mTableColumnModel != null && mTableColumnModel.MaxLength > 0 && !string.IsNullOrEmpty(mValue) && mValue.Length > mTableColumnModel.MaxLength)
					{
						stringList.Add(AssembleOutOfRange(ctx, filedName, mValue, mTableColumnModel.MaxLength));
					}
				}
			}
		}

		private static string AssembleOutOfRange(MContext ctx, string filedName, string value, int maxLength)
		{
			value = ((value.Length < 6) ? value : (value.Substring(0, 6) + "..."));
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "InputValueTooLong", "输入的值:{0}超过了最大长度{1},请减少字符！");
			return string.Format(text, value, maxLength);
		}

		public static void IsValueTypeFiledValid(MContext ctx, object value, MTableColumnModel column, List<string> stringList)
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
					text = text.Split('.')[0].TrimStart('-');
					num = column.DecimalMaxLength;
				}
				if (text.Length > num)
				{
					stringList.Add(AssembleOutOfRange(ctx, column.Name, value.ToString(), num));
				}
			}
		}

		private static bool IsExtendDataModel(Type type)
		{
			if (type == typeof(BaseModel))
			{
				return true;
			}
			if (type == typeof(object))
			{
				return false;
			}
			return IsExtendDataModel(type.BaseType);
		}
	}
}
