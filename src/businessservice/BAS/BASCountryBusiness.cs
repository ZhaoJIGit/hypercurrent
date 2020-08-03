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
	public class BASCountryBusiness : IBASCountryBusiness
	{
		private readonly BASCountryRepository dal = new BASCountryRepository();

		public List<BASCountryModel> GetCountryList(MContext ctx)
		{
			return dal.GetCountryList(ctx);
		}

		public List<BASProvinceModel> GetProvinceList(MContext ctx, string countryId)
		{
			return dal.GetProvinceList(ctx, countryId);
		}

		public void CheckCountryExist<T>(MContext ctx, T model, List<IOValidationResultModel> validationResult, List<BASCountryModel> countryList, string countryField, int rowIndex = -1)
		{
			string countryName = ModelHelper.GetModelValue(model, countryField).Trim();
			if (!string.IsNullOrWhiteSpace(countryName) && countryList != null)
			{
				BASCountryModel bASCountryModel = countryList.FirstOrDefault((BASCountryModel f) => f.MItemID == countryName);
				if (!string.IsNullOrWhiteSpace(countryName) && bASCountryModel != null)
				{
					ModelHelper.SetModelValue(model, countryName, bASCountryModel.MItemID, null);
				}
				else
				{
					int rowIndex2 = 0;
					if (rowIndex != -1)
					{
						rowIndex2 = rowIndex;
					}
					else
					{
						int.TryParse(ModelHelper.TryGetModelValue(model, "MRowIndex"), out rowIndex2);
					}
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Country,
						FieldValue = countryName,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "CountryNotFound", "The Country:{0} can't be found!"),
						RowIndex = rowIndex2
					});
				}
			}
		}
	}
}
