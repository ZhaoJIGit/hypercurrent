using JieNor.Megi.BusinessContract.FA;
using JieNor.Megi.BusinessService.FA;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.FA;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.FA
{
	public class FAFixAssetsChangeService : ServiceT<FAFixAssetsChangeModel>, IFAFixAssetsChange
	{
		private readonly IFAFixAssetsChangeBusiness biz = new FAFixAssetsChangeBusiness();

		public MActionResult<List<FAFixAssetsChangeModel>> GetFixAssetsChangeLog(FAFixAssetsChangeFilterModel filter = null, string accessToken = null)
		{
			IFAFixAssetsChangeBusiness iFAFixAssetsChangeBusiness = biz;
			return base.RunFunc(iFAFixAssetsChangeBusiness.GetFixAssetsChangeLog, filter, accessToken);
		}
	}
}
