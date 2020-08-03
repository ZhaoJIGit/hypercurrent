using JieNor.Megi.BusinessContract.REG;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.REG;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.REG
{
	public class REGTaxRateService : ServiceT<REGTaxRateModel>, IREGTaxRate
	{
		private readonly IREGTaxRateBusiness biz = new REGTaxRateBusiness();

		public MActionResult<DataGridJson<REGTaxRateModel>> GetTaxRateListByPage(REGTaxTateListFilterModel filter, bool includeDelete = false, string accessToken = null)
		{
			IREGTaxRateBusiness iREGTaxRateBusiness = biz;
			return base.RunFunc(iREGTaxRateBusiness.GetTaxTateListByPage, filter, accessToken);
		}

		public MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null)
		{
			IREGTaxRateBusiness iREGTaxRateBusiness = biz;
			return base.RunFunc(iREGTaxRateBusiness.IsCanDeleteOrInactive, param, accessToken);
		}

		public MActionResult<OperationResult> ArchiveTaxRate(string keyIDs, bool isActive, string accessToken = null)
		{
			IREGTaxRateBusiness iREGTaxRateBusiness = biz;
			return base.RunFunc(iREGTaxRateBusiness.ArchiveTaxRate, keyIDs, isActive, accessToken);
		}

		public MActionResult<List<REGTaxRateModel>> GetTaxRateList(bool ignoreLocale = false, string accessToken = null)
		{
			IREGTaxRateBusiness iREGTaxRateBusiness = biz;
			return base.RunFunc(iREGTaxRateBusiness.GetTaxRateList, ignoreLocale, accessToken);
		}

		public MActionResult<OperationResult> GetUpdateTaxInfo(int changeTaxType, string accessToken = null)
		{
			IREGTaxRateBusiness iREGTaxRateBusiness = biz;
			return base.RunFunc(iREGTaxRateBusiness.GetUpdateTaxInfo, changeTaxType, accessToken);
		}

		public MActionResult<REGTaxRateModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IREGTaxRateBusiness iREGTaxRateBusiness = biz;
			return base.GetDataModel(iREGTaxRateBusiness.GetDataModel, pkID, includeDelete, accessToken);
		}

		public MActionResult<OperationResult> Delete(string pkID, string accessToken = null)
		{
			IREGTaxRateBusiness iREGTaxRateBusiness = biz;
			return base.Delete(iREGTaxRateBusiness.Delete, pkID, accessToken);
		}

		public MActionResult<OperationResult> InsertOrUpdate(REGTaxRateModel modelData, string fields = null, string accessToken = null)
		{
			IREGTaxRateBusiness iREGTaxRateBusiness = biz;
			return base.InsertOrUpdate(iREGTaxRateBusiness.InsertOrUpdate, modelData, fields, accessToken);
		}
	}
}
