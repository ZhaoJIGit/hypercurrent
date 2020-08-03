using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BAS
{
	public class BASCountryRegionService : ServiceT<BASCountryRegionModel>, IBASCountryRegion
	{
		private readonly IBASCountryRegionBusiness biz = new BASCountryRegionBusiness();

		public MActionResult<List<BASCountryRegionModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null)
		{
			IBASCountryRegionBusiness iBASCountryRegionBusiness = biz;
			return base.GetModelList(iBASCountryRegionBusiness.GetModelList, filter, includeDelete, accessToken);
		}
	}
}
