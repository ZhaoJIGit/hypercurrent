using Fasterflect;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JieNor.Megi.DataRepository.API
{
	public static class APIValidator
	{
		public static void ValidateDuplicateName<T>(MContext ctx, T model, string errorMsg, ref Dictionary<string, List<string>> validNameList, string fieldName = "MName")
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			ValidateDuplicateName(ctx, model, errorMsg, ref validNameList, ref dictionary, fieldName, "MItemID");
		}

		public static void ValidateDuplicateName<T>(MContext ctx, T model, string errorMsg, ref Dictionary<string, List<string>> validNameList, ref Dictionary<string, string> updNameList, string fieldName = "MName", string idField = "MItemID")
		{
			BaseModel baseModel = model as BaseModel;
			if (baseModel != null)
			{
				baseModel.ValidationErrors = (baseModel.ValidationErrors ?? new List<ValidationError>());
				if (!baseModel.ValidationErrors.Any())
				{
					MultiLanguageFieldList multiLanguageFieldList = baseModel.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == fieldName);
					string modelValue = ModelHelper.GetModelValue(model, idField);
					IEnumerable<string> enumerable = new string[1]
					{
						ctx.MLCID
					}.Concat(from f in ctx.MActiveLocaleIDS
					where f != ctx.MLCID
					select f);
					if (multiLanguageFieldList != null)
					{
						foreach (string item in enumerable)
						{
							string text = multiLanguageFieldList.MMultiLanguageField.FirstOrDefault((MultiLanguageField lang) => lang.MLocaleID == item)?.MValue.Trim();
							if (!string.IsNullOrWhiteSpace(text))
							{
								if (validNameList.ContainsKey(item))
								{
									if (validNameList[item].Contains(text, StringComparer.OrdinalIgnoreCase))
									{
										string key = text.ToLower();
										if (!updNameList.ContainsKey(key) || updNameList[key] != modelValue)
										{
											baseModel.ValidationErrors.Add(new ValidationError
											{
												Message = string.Format(errorMsg, text)
											});
										}
										break;
									}
									validNameList[item].Add(text);
								}
								else
								{
									validNameList.Add(item, new List<string>
									{
										text
									});
								}
							}
						}
					}
					else
					{
						string text2 = GetModelValue(model, fieldName).Trim();
						if (!string.IsNullOrWhiteSpace(text2))
						{
							if (validNameList.ContainsKey(fieldName))
							{
								if (validNameList[fieldName].Contains(text2, StringComparer.OrdinalIgnoreCase))
								{
									string key2 = text2.ToLower();
									if (!updNameList.ContainsKey(key2) || updNameList[key2] != modelValue)
									{
										baseModel.ValidationErrors.Add(new ValidationError
										{
											Message = string.Format(errorMsg, text2)
										});
									}
								}
								else
								{
									validNameList[fieldName].Add(text2);
								}
							}
							else
							{
								validNameList.Add(fieldName, new List<string>
								{
									text2
								});
							}
						}
					}
				}
			}
		}

		public static void ValidateExistName<T>(BaseModel model, List<T> sysList, string errorMsg, string fieldName = "MName")
		{
			List<MultiLanguageField> mMultiLanguageField = model.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == fieldName).MMultiLanguageField;
			model.ValidationErrors = (model.ValidationErrors ?? new List<ValidationError>());
			foreach (MultiLanguageField item in mMultiLanguageField)
			{
				if (!string.IsNullOrWhiteSpace(item.MValue))
				{
					bool flag = false;
					foreach (T sys in sysList)
					{
						List<MultiLanguageField> mMultiLanguageField2 = (sys as BaseModel).MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == fieldName).MMultiLanguageField;
						if (mMultiLanguageField2.Any((MultiLanguageField f) => f.MValue == item.MValue))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						model.ValidationErrors.Add(new ValidationError
						{
							Message = errorMsg
						});
						break;
					}
				}
			}
		}

		public static string GetModelValue<T>(T model, string propName)
		{
			List<object> list = new List<object>();
			string[] array = propName.Split(',');
			if (array.Length == 1)
			{
				return Convert.ToString(model.GetPropertyValue(propName));
			}
			string[] array2 = array;
			foreach (string name in array2)
			{
				list.Add(model.GetPropertyValue(name));
			}
			return string.Join("", list.OfType<string>());
		}

		public static T2 MatchByIdThenName<T1, T2>(MContext ctx, bool isPut, T1 model, List<T2> sysList, string existErrorMsg, string nameField = "MName", string idField = "MItemID", bool isMultiField = true, BasicDataReferenceTypeEnum referenceType = BasicDataReferenceTypeEnum.NotReference)
			where T2: new()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			return MatchByIdThenName(ctx, isPut, model, sysList, existErrorMsg, ref dictionary, nameField, idField, isMultiField, referenceType, null, false, null);
		}

		public static T2 MatchByIdThenName<T1, T2>(MContext ctx, bool isPut, T1 model, List<T2> sysList, string existErrorMsg, ref Dictionary<string, string> updNameList, string nameField = "MName", string idField = "MItemID", bool isMultiField = true, BasicDataReferenceTypeEnum referenceType = BasicDataReferenceTypeEnum.NotReference, string editDisabledMsg = null, bool isMainModel = false, List<T1> disabledModelList = null)
			where T2:new()
		{
			T2 val = new T2();
			T2 val2 = new T2();
			if (model == null)
			{
				return val;
			}
			BaseModel baseModel = model as BaseModel;
			string itemId = ModelHelper.GetModelValue(model, idField);
			bool flag = !string.IsNullOrWhiteSpace(itemId);
			string modelValue = GetModelValue(model, nameField);
			baseModel.ValidationErrors = (baseModel.ValidationErrors ?? new List<ValidationError>());
			sysList = (sysList ?? new List<T2>());
			if (isPut)
			{
				editDisabledMsg = string.Empty;
			}
			if (flag)
			{
				val = sysList.FirstOrDefault((T2 f) => ModelHelper.GetModelValue<T2>(f, idField) == itemId);
				if (val == null)
				{
					flag = false;
					ModelHelper.SetModelValue(model, idField, string.Empty, null);
				}
				else
				{
					ModelHelper.SetModelValue(model, "IsUpdate", true, null);
					ModelHelper.SetModelValue(model, "IsNew", false, null);
				}
			}
			if (flag && referenceType == BasicDataReferenceTypeEnum.ReferenceOnly)
			{
				goto IL_0137;
			}
			if (!isMultiField && string.IsNullOrWhiteSpace(modelValue))
			{
				goto IL_0137;
			}
			int num = (isMultiField && IsMultiLangEmpty(baseModel.MultiLanguage, nameField)) ? 1 : 0;
			goto IL_0138;
			IL_0138:
			if (num != 0)
			{
				CheckMatchItem(ctx, model, val, isPut, idField, modelValue, ref updNameList, editDisabledMsg, isMainModel, disabledModelList);
				return val;
			}
			List<string> matchedIdList = new List<string>();
			List<List<MultiLanguageFieldList>> matchedMultiLangList = new List<List<MultiLanguageFieldList>>();
			Dictionary<string, List<string>> matchedMutltiNameList = new Dictionary<string, List<string>>();
			List<T2> sysListExculeEditList = flag ? (from f in sysList
			where ModelHelper.GetModelValue<T2>(f, idField) != itemId
			select f).ToList() : sysList;
			MatchNameByLang(ctx, nameField, idField, isMultiField, ref val2, baseModel, modelValue, ref matchedIdList, ref matchedMultiLangList, matchedMutltiNameList, sysListExculeEditList);
			SetMatchResult(ctx, isPut, model, existErrorMsg, nameField, idField, isMultiField, referenceType, val2, baseModel, itemId, flag, modelValue, matchedIdList, matchedMultiLangList, matchedMutltiNameList);
			T2 val3 = (flag && val != null) ? val : val2;
			CheckMatchItem(ctx, model, val3, isPut, idField, modelValue, ref updNameList, editDisabledMsg, isMainModel, disabledModelList);
			return val3;
			IL_0137:
			num = 1;
			goto IL_0138;
		}

		private static void CheckMatchItem<T1, T2>(MContext ctx, T1 model, T2 matchItem, bool isPut, string idField, string name, ref Dictionary<string, string> updNameList, string editDisabledMsg, bool isMainModel = false, List<T1> disabledModelList = null)
		{
			string id = (matchItem == null) ? string.Empty : ModelHelper.GetModelValue(matchItem, idField);
			if (matchItem != null && !string.IsNullOrWhiteSpace(id))
			{
				if (!updNameList.ContainsKey(name.Trim().ToLower()))
				{
					updNameList.Add(name.Trim().ToLower(), id);
				}
				List<string> list = ModelHelper.GetModelValueO(model, "UpdateFieldList", false) as List<string>;
				if (list != null)
				{
					bool flag = Convert.ToBoolean(ModelHelper.GetModelValue(matchItem, "MIsActive"));
					if (disabledModelList != null)
					{
						T1 val = disabledModelList.FirstOrDefault((T1 f) => ModelHelper.GetModelValue<T1>(f, idField) == id);
						flag = ((val == null) ? flag : Convert.ToBoolean(ModelHelper.GetModelValue(val, "MIsActive")));
					}
					if (!flag && !string.IsNullOrWhiteSpace(editDisabledMsg))
					{
						(model as BaseModel).ValidationErrors.Add(new ValidationError
						{
							Message = string.Format(editDisabledMsg, name)
						});
					}
				}
			}
			else if (isMainModel && !isPut && string.IsNullOrWhiteSpace(id))
			{
				SetNewModelID(ctx, model, idField);
			}
		}

		private static void SetMatchResult<T1, T2>(MContext ctx, bool isPut, T1 model, string existErrorMsg, string nameField, string idField, bool isMultiField, BasicDataReferenceTypeEnum referenceType, T2 matchItem, BaseModel baseModel, string itemId, bool hasItemID, string name, List<string> matchedIdList, List<List<MultiLanguageFieldList>> matchedMultiLangList, Dictionary<string, List<string>> matchedMutltiNameList)
		{
			switch (matchedIdList.Distinct().Count())
			{
			case 0:
				break;
			case 1:
				if (((isPut | hasItemID) && referenceType == BasicDataReferenceTypeEnum.NotReference) || (hasItemID && referenceType == BasicDataReferenceTypeEnum.ReferenceAndUpdate))
				{
					AddExsitValidationError(ctx, existErrorMsg, baseModel, name, matchedMutltiNameList);
				}
				else
				{
					ModelHelper.SetModelValue(model, idField, matchedIdList[0], null);
					ModelHelper.SetModelValue(model, "IsUpdate", true, null);
					ModelHelper.SetModelValue(model, "IsNew", false, null);
					SetMatchMultiFieldInfo(baseModel.MultiLanguage, matchedIdList[0], matchedMultiLangList[0]);
				}
				break;
			default:
				if (isPut && referenceType == BasicDataReferenceTypeEnum.NotReference)
				{
					AddExsitValidationError(ctx, existErrorMsg, baseModel, name, matchedMutltiNameList);
				}
				else
				{
					string apiFieldName = GetApiFieldName<T1>(nameField);
					string textFormat = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Common, "MatchedMultiRecord", "多语言字段“{0}”的传值“{1}”匹配到了两个系统记录，请检查。", apiFieldName, name);
					AddExsitValidationError(ctx, textFormat, baseModel, name, matchedMutltiNameList);
				}
				break;
			}
		}

		public static string GetApiFieldName<T>(string fieldName)
		{
			string result = string.Empty;
			PropertyInfo property = typeof(T).GetProperty(fieldName);
			ApiMemberAttribute apiMemberAttribute = (property == (PropertyInfo)null) ? null : ((MemberInfo)property).GetCustomAttribute<ApiMemberAttribute>();
			if (apiMemberAttribute != null)
			{
				result = apiMemberAttribute.Name;
			}
			return result;
		}

		private static void AddExsitValidationError(MContext ctx, string existErrorMsg, BaseModel baseModel, string name, Dictionary<string, List<string>> matchedMutltiNameList)
		{
			if (!string.IsNullOrWhiteSpace(existErrorMsg))
			{
				string arg = (!matchedMutltiNameList.ContainsKey(ctx.MLCID)) ? matchedMutltiNameList.FirstOrDefault().Value[0] : matchedMutltiNameList[ctx.MLCID][0];
				baseModel.ValidationErrors.Add(new ValidationError
				{
					Message = string.Format(existErrorMsg, arg)
				});
			}
		}

		private static void MatchNameByLang<T2>(MContext ctx, string nameField, string idField, bool isMultiField, ref T2 matchItem, BaseModel baseModel, string name, ref List<string> matchedIdList, ref List<List<MultiLanguageFieldList>> matchedMultiLangList, Dictionary<string, List<string>> matchedMutltiNameList, List<T2> sysListExculeEditList)
		{
			if (isMultiField)
			{
				MultiLanguageFieldList multiLanguageFieldList = baseModel.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == nameField);
				foreach (string mActiveLocaleID in ctx.MActiveLocaleIDS)
				{
					string text = multiLanguageFieldList?.MMultiLanguageField.FirstOrDefault((MultiLanguageField lang) => lang.MLocaleID == mActiveLocaleID)?.MValue;
					if (!string.IsNullOrWhiteSpace(text))
					{
						foreach (T2 sysListExculeEdit in sysListExculeEditList)
						{
							BaseModel baseModel2 = sysListExculeEdit as BaseModel;
							List<MultiLanguageFieldList> list = baseModel2?.MultiLanguage;
							MultiLanguageFieldList multiLanguageFieldList2 = list?.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == nameField);
							string text2 = multiLanguageFieldList2?.MMultiLanguageField.FirstOrDefault((MultiLanguageField lang) => lang.MLocaleID == mActiveLocaleID)?.MValue;
							if (text2.EqualsIgnoreCase(text))
							{
								matchedIdList.Add(ModelHelper.GetModelValue(sysListExculeEdit, idField));
								matchedMultiLangList.Add(list);
								if (!matchedMutltiNameList.ContainsKey(mActiveLocaleID))
								{
									matchedMutltiNameList.Add(mActiveLocaleID, new List<string>
									{
										text2
									});
								}
								else
								{
									matchedMutltiNameList[mActiveLocaleID].Add(text2);
								}
							}
						}
					}
				}
				if (matchedIdList.Distinct().Count() == 1)
				{
					List<string> matchedIdTmpList = matchedIdList;
					matchItem = sysListExculeEditList.FirstOrDefault((T2 f) => ModelHelper.GetModelValue<T2>(f, idField) == Enumerable.FirstOrDefault<string>((IEnumerable<string>)matchedIdTmpList));
				}
			}
			else
			{
				int num = 0;
				foreach (T2 sysListExculeEdit2 in sysListExculeEditList)
				{
					string modelValue = GetModelValue(sysListExculeEdit2, nameField);
					if (modelValue.EqualsIgnoreCase(name))
					{
						BaseModel baseModel3 = sysListExculeEdit2 as BaseModel;
						List<MultiLanguageFieldList> item = baseModel3?.MultiLanguage;
						string modelValue2 = ModelHelper.GetModelValue(sysListExculeEdit2, idField);
						matchedIdList.Add(modelValue2);
						matchedMultiLangList.Add(item);
						num++;
						if (num == 1)
						{
							matchItem = sysListExculeEdit2;
						}
					}
				}
				if (num > 0)
				{
					matchedMutltiNameList.Add(ctx.MLCID, new List<string>
					{
						name
					});
				}
			}
		}

		public static bool IsMultiLangEmpty(List<MultiLanguageFieldList> multiLangList, string fieldName = "MName")
		{
			if (multiLangList == null)
			{
				return true;
			}
			MultiLanguageFieldList multiLanguageFieldList = multiLangList.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == fieldName);
			return multiLanguageFieldList?.MMultiLanguageField.All((MultiLanguageField f) => string.IsNullOrWhiteSpace(f.MValue)) ?? true;
		}

		private static void SetNewModelID<T1>(MContext ctx, T1 model, string idField)
		{
			string guid = UUIDHelper.GetGuid();
			ModelHelper.SetModelValue(model, idField, guid, null);
			ModelHelper.SetModelValue(model, "IsNew", true, null);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string mActiveLocaleID in ctx.MActiveLocaleIDS)
			{
				dictionary.Add(mActiveLocaleID, UUIDHelper.GetGuid());
			}
			List<MultiLanguageFieldList> multiLanguage = (model as BaseModel).MultiLanguage;
			foreach (MultiLanguageFieldList item2 in multiLanguage)
			{
				item2.MParentID = guid;
				foreach (MultiLanguageField item3 in item2.MMultiLanguageField)
				{
					item3.MPKID = dictionary.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == item3.MLocaleID).Value;
				}
			}
		}

		public static void SetMatchMultiFieldInfo(List<MultiLanguageFieldList> mutiFieldList, string parentId, List<MultiLanguageFieldList> matchedMultiLangList)
		{
			if (mutiFieldList != null && matchedMultiLangList != null)
			{
				foreach (MultiLanguageFieldList mutiField in mutiFieldList)
				{
					mutiField.MParentID = parentId;
					MultiLanguageFieldList multiLanguageFieldList = matchedMultiLangList.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == mutiField.MFieldName);
					if (multiLanguageFieldList != null)
					{
						foreach (MultiLanguageField item in mutiField.MMultiLanguageField)
						{
							MultiLanguageField multiLanguageField = multiLanguageFieldList.MMultiLanguageField.FirstOrDefault((MultiLanguageField f) => f.MLocaleID == item.MLocaleID);
							if (multiLanguageField != null)
							{
								item.MPKID = multiLanguageField.MPKID;
							}
						}
					}
				}
			}
		}

		public static void AddError<T>(T model, string errMsg, params string[] param)
		{
			(model as BaseModel).ValidationErrors.Add(new ValidationError
			{
				Message = ((param != null || param.Any()) ? string.Format(errMsg, param) : errMsg)
			});
		}

		public static void ValidateAccountNumber<T>(MContext ctx, T model, List<BDAccountEditModel> accountList, string fieldName, string dbFieldName = null, string[] currentAccountCodes = null, string unCurrentAcctErrorMsg = null)
		{
			string number = ModelHelper.GetModelValue(model, fieldName);
			if (!string.IsNullOrWhiteSpace(number))
			{
				BDAccountEditModel bDAccountEditModel = accountList.FirstOrDefault((BDAccountEditModel f) => f.MApiCode == number);
				if (bDAccountEditModel == null)
				{
					AddError(model, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "AccountCodeNotExist", "科目代码“{0}”不存在。"), number);
				}
				else
				{
					if (!bDAccountEditModel.MIsActive)
					{
						AddError(model, string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountDisabled", "科目（{0}）已禁用"), number));
					}
					if (accountList.Any((BDAccountEditModel f) => f.MApiCode.StartsWith(number) && f.MApiCode.Length > number.Length))
					{
						AddError(model, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "AccountExistSubAcct", "Account“{0}”不是末级科目。"), number);
					}
					if (currentAccountCodes != null && !currentAccountCodes.Any((string c) => number.FilterNonNumericChar().StartsWith(c)))
					{
						AddError(model, unCurrentAcctErrorMsg);
					}
					if (!(model as BaseModel).ValidationErrors.Any())
					{
						if (!string.IsNullOrWhiteSpace(dbFieldName))
						{
							fieldName = dbFieldName;
						}
						ModelHelper.SetModelValue(model, fieldName, bDAccountEditModel.MCode, null);
					}
				}
			}
		}

		public static void ValidateTaxRate<T1, T2>(MContext ctx, T1 model, T2 parentModel, bool isSale, string taxRateFieldName, List<REGTaxRateModel> taxRates, BasicDataReferenceTypeEnum referenceType = BasicDataReferenceTypeEnum.ReferenceOnly)
		{
			REGTaxRateSimpleModel rEGTaxRateSimpleModel = ModelHelper.GetModelValueO(parentModel, taxRateFieldName, false) as REGTaxRateSimpleModel;
			List<string> list = ModelHelper.GetModelValueO(parentModel, "UpdateFieldList", false) as List<string>;
			BaseModel baseModel = model as BaseModel;
			if (rEGTaxRateSimpleModel != null)
			{
				string mTaxRateID = rEGTaxRateSimpleModel.MTaxRateID;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				string editDisabledMsg = (referenceType == BasicDataReferenceTypeEnum.ReferenceOnly) ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TaxRateDisabled", "提供的税率已禁用。") : string.Empty;
				REGTaxRateModel rEGTaxRateModel = MatchByIdThenName(ctx, false, rEGTaxRateSimpleModel, taxRates, null, ref dictionary, "MName", "MTaxRateID", true, referenceType, editDisabledMsg, false, null);
				bool flag = rEGTaxRateSimpleModel.ValidationErrors.Any();
				baseModel.ValidationErrors.AddRange((IEnumerable<ValidationError>)rEGTaxRateSimpleModel.ValidationErrors);
				rEGTaxRateSimpleModel.ValidationErrors.Clear();
				if (!string.IsNullOrWhiteSpace(rEGTaxRateSimpleModel.MTaxRateID))
				{
					string text = isSale ? "MSalTaxTypeID" : "MPurTaxTypeID";
					ModelHelper.SetModelValue(model, text, rEGTaxRateSimpleModel.MTaxRateID, null);
					ModelHelper.SetModelValue(rEGTaxRateSimpleModel, "MEffectiveTaxRate", rEGTaxRateModel.MEffectiveTaxRate, null);
					ModelHelper.SetModelValue(rEGTaxRateSimpleModel, "ValidationErrors", null, null);
					baseModel.UpdateFieldList.Add(text);
				}
				else if (rEGTaxRateSimpleModel.UpdateFieldList.Any() && (!string.IsNullOrWhiteSpace(mTaxRateID) || !IsMultiLangEmpty(rEGTaxRateSimpleModel.MultiLanguage, "MName")))
				{
					if (!flag)
					{
						baseModel.Validate(ctx, true, "TaxRateNameNotExist", "无效的税率。请提供有效的TaxRateID或税率Name。", LangModule.Common);
					}
				}
				else if (rEGTaxRateSimpleModel.UpdateFieldList.Contains("MTaxRateID") || rEGTaxRateSimpleModel.UpdateFieldList.Contains("MName"))
				{
					string item = isSale ? "MSalTaxTypeID" : "MPurTaxTypeID";
					baseModel.UpdateFieldList.Add(item);
				}
			}
			else if (list.Contains(taxRateFieldName))
			{
				string item2 = isSale ? "MSalTaxTypeID" : "MPurTaxTypeID";
				baseModel.UpdateFieldList.Add(item2);
			}
		}

		public static bool Validate<T>(this T model, MContext ctx, bool invalid, string key, string defaultValue, LangModule module = LangModule.Common, params object[] param) where T : BaseModel
		{
			if (invalid)
			{
				model.ValidationErrors = (model.ValidationErrors ?? new List<ValidationError>());
				string text = COMMultiLangRepository.GetText(ctx.MLCID, module, key, defaultValue);
				if (param?.Any() ?? false)
				{
					text = string.Format(text, param);
				}
				model.ValidationErrors.Add(new ValidationError(text));
			}
			return !invalid;
		}

		public static bool ValidateBaseData<T1, T2>(this T1 model, MContext ctx, T2 data, params string[] formats) where T1 : BaseModel
		{
			bool result = true;
			string typeName = GetTypeName(ctx, typeof(T2));
			string text = string.Empty;
			if (data == null)
			{
				text = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataNotExist", "{0}不存在!"), typeName);
			}
			else
			{
				string modelValue = ModelHelper.GetModelValue(model, "MIsActive");
				if (!string.IsNullOrWhiteSpace(modelValue) && !Convert.ToBoolean(modelValue))
				{
					text = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataDisabled", "{0}已禁用!"), typeName);
				}
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				if (formats != null && formats.Count() > 0)
				{
					text = string.Format(text, formats);
				}
				model.ValidationErrors = (model.ValidationErrors ?? new List<ValidationError>());
				model.ValidationErrors.Add(new ValidationError(text));
				result = false;
			}
			return result;
		}

		public static bool Validate<T>(this T model, OperationResult result) where T : BaseModel
		{
			if (result != null && !result.Success && result.VerificationInfor != null && result.VerificationInfor.Any())
			{
				model.ValidationErrors = (model.ValidationErrors ?? new List<ValidationError>());
				model.ValidationErrors = (from x in result.VerificationInfor
				select new ValidationError
				{
					Message = x.Message
				}).ToList();
				return false;
			}
			return true;
		}

		public static bool Validate<T>(this T model, MContext ctx, string accountName, GLCheckGroupModel checkGroup, GLCheckGroupValueModel checkValue) where T : BaseModel
		{
			if (checkGroup == null)
			{
				return true;
			}
			List<KeyValuePair<string, string>> checkTypeList = new GLUtility().GetCheckTypeList(ctx);
			bool flag = true;
			for (int i = 0; i < checkTypeList.Count; i++)
			{
				KeyValuePair<string, string> keyValuePair = checkTypeList[i];
				int num = int.Parse(checkGroup.GetPropertyValue(keyValuePair.Key).ToString());
				keyValuePair = checkTypeList[i];
				object propertyValue = checkValue.GetPropertyValue(keyValuePair.Key);
				string value = string.Empty;
				if (!string.IsNullOrWhiteSpace(propertyValue?.ToString()))
				{
					value = propertyValue.ToString();
				}
				bool num2 = flag;
				bool invalid = num == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(value);
				object[] obj = new object[2]
				{
					accountName,
					null
				};
				keyValuePair = checkTypeList[i];
				obj[1] = keyValuePair.Value;
				flag = (num2 & model.Validate(ctx, invalid, "AccountingDimensioinMustProvide", "科目“{0}”上的核算维度“{1}”必录，必须指定。", LangModule.GL, obj));
				if (num != CheckTypeStatusEnum.Required && num != CheckTypeStatusEnum.Optional && !string.IsNullOrWhiteSpace(value))
				{
					keyValuePair = checkTypeList[i];
					checkValue.SetPropertyValue(keyValuePair.Key, null);
				}
			}
			return flag;
		}

		private static string GetTypeName(MContext ctx, Type type)
		{
			if (type == typeof(BDContactsModel))
			{
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Contact", "Contact");
			}
			if (type == typeof(BDEmployeesModel))
			{
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Employee", "Employee");
			}
			if (type == typeof(BDItemModel))
			{
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "MerItem", "商品项目");
			}
			if (type == typeof(PAPayItemModel))
			{
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PaSalaryItem", "Salary item");
			}
			if (type == typeof(BDExpenseItemModel))
			{
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItem", "费用项目");
			}
			if (type == typeof(REGCurrencyViewModel))
			{
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Currency", "Currency");
			}
			if (type == typeof(BDTrackEntryModel))
			{
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TrackIndex", "Track{0}");
			}
			return string.Empty;
		}
	}
}
