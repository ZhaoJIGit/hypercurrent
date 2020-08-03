using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.COM
{
	public class COMValidateHelper
	{
		public static bool CheckBasicData<T>(MContext ctx, T model, List<ValidationError> validationErrors, bool checkExist = true)
		{
			bool result = true;
			string typeName = GetTypeName(ctx, typeof(T));
			if (model == null & checkExist)
			{
				string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataNotExist", "{0}不存在!"), typeName);
				validationErrors.Add(new ValidationError(message));
				result = false;
			}
			else
			{
				string modelValue = ModelHelper.GetModelValue(model, "MIsActive");
				if (!string.IsNullOrWhiteSpace(modelValue) && !Convert.ToBoolean(modelValue))
				{
					string message2 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataDisabled", "{0}已禁用!"), typeName);
					validationErrors.Add(new ValidationError(message2));
					result = false;
				}
			}
			return result;
		}

		public static void AddRecordNotExistError<T>(MContext ctx, ref T model, params string[] param)
		{
			AddRecordNotExistError(ctx, ref model, null, null, LangModule.Common.ToString(), param.ToString());
		}

		public static void AddRecordNotExistError<T>(MContext ctx, ref T model, string key = null, string defaultValue = null, LangModule module = LangModule.BD, params string[] param)
            where T : new()
		{
			if (model == null)
			{
				model = new T();
			}
			BaseModel baseModel = model as BaseModel;
			baseModel.ValidationErrors = (baseModel.ValidationErrors ?? new List<ValidationError>());
			string empty = string.Empty;
			empty = ((!string.IsNullOrWhiteSpace(key)) ? COMMultiLangRepository.GetText(ctx.MLCID, module, key, defaultValue) : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "APIKeyFieldNameNotExists", "{0}“{1}”不存在"));
			if (param != null && param.Count() > 0)
			{
				empty = string.Format(empty, param);
			}
			baseModel.ValidationErrors.Add(new ValidationError
			{
				Message = empty
			});
		}

		public static void CheckTrackValue<T>(MContext ctx, T model, List<BDTrackModel> trackList, List<ValidationError> errors)
		{
			int count = trackList.Count;
			for (int i = 0; i < 5; i++)
			{
				int num = i + 1;
				string trackItemVal = ModelHelper.GetModelValue(model, "MTrackItem" + num);
				if (!string.IsNullOrWhiteSpace(trackItemVal))
				{
					BDTrackModel bDTrackModel = (i < count) ? trackList[i] : new BDTrackModel
					{
						MEntryList = new List<BDTrackEntryModel>()
					};
					BDTrackEntryModel bDTrackEntryModel = bDTrackModel.MEntryList.FirstOrDefault((BDTrackEntryModel f) => f.MEntryID == trackItemVal);
					if (bDTrackEntryModel == null)
					{
						errors.Add(new ValidationError(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NoTrackItemExist", "跟踪项{0}不存在！"), num)));
					}
					else if (!bDTrackEntryModel.MIsActive)
					{
						errors.Add(new ValidationError(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TrackItemDisabled", "跟踪项{0}已禁用！"), num)));
					}
				}
			}
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
			return string.Empty;
		}
	}
}
