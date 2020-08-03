using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BAS
{
	public class BASCountryService : ServiceT<BASCountryModel>, IBASCountry
	{
		private readonly IBASCountryBusiness biz = new BASCountryBusiness();

		public MActionResult<List<BASCountryModel>> GetCountryList(string accessToken = null)
		{
			IBASCountryBusiness iBASCountryBusiness = biz;
			return base.RunFunc(iBASCountryBusiness.GetCountryList, accessToken);
		}

		public MActionResult<List<BASProvinceModel>> GetProvinceList(string countryId, string accessToken = null)
		{
			IBASCountryBusiness iBASCountryBusiness = biz;
			return base.RunFunc(iBASCountryBusiness.GetProvinceList, countryId, accessToken);
		}
	}
}
