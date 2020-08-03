using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;

namespace JieNor.Megi.Service.Web.BAS
{
	public class BASSearchService : ServiceT<BASSearchModel>, IBASSearch
	{
		private readonly IBASSearchBusiness biz = new BASSearchBusiness();

		public MActionResult<DataGridJson<BASSearchModel>> GetSearchResult(BASSearchFilterModel filter, string accessToken = null)
		{
			IBASSearchBusiness iBASSearchBusiness = biz;
			return base.RunFunc(iBASSearchBusiness.GetSearchResult, filter, accessToken);
		}
	}
}
