using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.BAS
{
	public class BASDataDictionaryBusiness : IBASDataDictionaryBusiness
	{
		public List<BASDataDictionaryModel> GetDictListByValues(MContext context, string dictType, List<string> filterValues)
		{
			return BASDataDictionary.GetDictList(dictType, context.MLCID, filterValues, false);
		}

		public List<BASDataDictionaryModel> GetDictList(MContext context, string dictType)
		{
			List<BASDataDictionaryModel> list = new List<BASDataDictionaryModel>();
			List<BASDataDictionaryModel> dictList = BASDataDictionary.GetDictList(dictType, context.MLCID);
			foreach (BASDataDictionaryModel item in dictList)
			{
				IEnumerable<BASDataDictionaryModel> enumerable = from f in dictList
				where f.ParentDictCode == item.DictCode
				select f;
				if (enumerable.Any())
				{
					foreach (BASDataDictionaryModel item2 in enumerable)
					{
						item2.ParentDictCode = item.DictName;
					}
					list.AddRange(enumerable);
				}
				else
				{
					list.Add(item);
				}
			}
			return list;
		}

		public void CheckTaxTypeExist<T>(MContext ctx, List<T> modelList, List<IOValidationResultModel> validationResult, string taxTypeField = null)
		{
			List<BASDataDictionaryModel> dictList = BASDataDictionary.GetDictList("TaxType", ctx.MLCID, null, true);
			if (string.IsNullOrWhiteSpace(taxTypeField))
			{
				taxTypeField = "MTaxID";
			}
			foreach (T model in modelList)
			{
				string taxTypeName = ModelHelper.GetModelValue(model, taxTypeField);
				if (!string.IsNullOrWhiteSpace(taxTypeName))
				{
					BASDataDictionaryModel bASDataDictionaryModel = dictList.FirstOrDefault((BASDataDictionaryModel f) => f.DictValue == taxTypeName || f.DictName.Trim().ToUpper() == taxTypeName.Trim().ToUpper());
					if (bASDataDictionaryModel != null)
					{
						ModelHelper.SetModelValue(model, taxTypeField, bASDataDictionaryModel.DictValue, null);
					}
					else
					{
						int rowIndex = 0;
						int.TryParse(ModelHelper.TryGetModelValue(model, "MRowIndex"), out rowIndex);
						validationResult.Add(new IOValidationResultModel
						{
							FieldType = IOValidationTypeEnum.TaxType,
							FieldValue = taxTypeName,
							Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TaxTypeNotFound", "The tax type:{0} can't be found!"), taxTypeName),
							RowIndex = rowIndex
						});
					}
				}
			}
		}
	}
}
