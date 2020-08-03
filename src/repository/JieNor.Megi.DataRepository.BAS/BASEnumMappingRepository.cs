using JieNor.Megi.Core;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASEnumMappingRepository
	{
		private static Dictionary<string, List<BASEnumMappingModel>> _dict = null;

		private static object _locker = new object();

		private static Dictionary<string, List<BASEnumMappingModel>> GetEnumMappingCache(MContext ctx)
		{
			if (_dict == null || _dict.Count == 0)
			{
				lock (_locker)
				{
					if (_dict == null || _dict.Count == 0)
					{
						_dict = new Dictionary<string, List<BASEnumMappingModel>>();
						bool isSys = ctx.IsSys;
						ctx.IsSys = true;
						List<BASEnumMappingModel> dataModelList = ModelInfoManager.GetDataModelList<BASEnumMappingModel>(ctx, new SqlWhere(), false, false);
						ctx.IsSys = isSys;
						IEnumerable<IGrouping<string, BASEnumMappingModel>> enumerable = from f in dataModelList
						group f by f.MType;
						foreach (IGrouping<string, BASEnumMappingModel> item in enumerable)
						{
							_dict.Add(item.Key, item.ToList());
						}
					}
				}
			}
			return _dict;
		}

		public static string GetInternalValue(MContext ctx, string type, object publicValue, string apiFieldName = null, bool isRequired = true)
		{
			Dictionary<string, List<BASEnumMappingModel>> enumMappingCache = GetEnumMappingCache(ctx);
			if (!enumMappingCache.ContainsKey(type))
			{
				return string.Empty;
			}
			string inputValue = Convert.ToString(publicValue).Trim().Replace("\"", "");
			if (!isRequired && string.IsNullOrEmpty(inputValue) && !string.IsNullOrWhiteSpace(apiFieldName))
			{
				return string.Empty;
			}
			BASEnumMappingModel bASEnumMappingModel = (publicValue == null) ? null : enumMappingCache[type].FirstOrDefault((BASEnumMappingModel f) => f.MPublicValue.Equals(inputValue, StringComparison.CurrentCultureIgnoreCase));
			if (bASEnumMappingModel == null && !string.IsNullOrWhiteSpace(apiFieldName))
			{
				string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "EnumDataInvoid", "“{0}”不是有效的“{1}”值。"), publicValue, apiFieldName);
				throw new Exception(message);
			}
			if (type == 24.ToString())
			{
				return ctx.MAccountTableID + bASEnumMappingModel.MInternalValue;
			}
			return (bASEnumMappingModel == null) ? string.Empty : bASEnumMappingModel.MInternalValue;
		}

		public static object GetPublicValue(MContext ctx, string type, object internalValue)
		{
			Dictionary<string, List<BASEnumMappingModel>> enumMappingCache = GetEnumMappingCache(ctx);
			if (!enumMappingCache.ContainsKey(type))
			{
				return null;
			}
			if (type == 24.ToString())
			{
				internalValue = Regex.Replace(Convert.ToString(internalValue), "^" + ctx.MAccountTableID, "");
			}
			BASEnumMappingModel bASEnumMappingModel = enumMappingCache[type].FirstOrDefault((BASEnumMappingModel f) => f.MInternalValue.Equals(Convert.ToString(internalValue)));
			string[] array = internalValue as string[];
			if (array != null)
			{
				List<string> list = new List<string>();
				string[] array2 = array;
				foreach (string val in array2)
				{
					bASEnumMappingModel = enumMappingCache[type].FirstOrDefault((BASEnumMappingModel f) => f.MInternalValue.Equals(val));
					if (bASEnumMappingModel != null)
					{
						list.Add(bASEnumMappingModel.MPublicValue);
					}
				}
				return list.ToArray();
			}
			return (bASEnumMappingModel == null) ? internalValue : bASEnumMappingModel.MPublicValue;
		}
	}
}
