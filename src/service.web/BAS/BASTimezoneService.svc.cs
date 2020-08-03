using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BAS
{
	public class BASTimezoneService : ServiceT<BASTimezoneModel>, IBASTimezone
	{
		private readonly IBASTimezoneBusiness biz = new BASTimezoneBusiness();

		public MActionResult<List<BASTimezoneModel>> GetList(string accessToken)
		{
			IBASTimezoneBusiness iBASTimezoneBusiness = biz;
			return base.RunFunc(iBASTimezoneBusiness.GetList, accessToken);
		}
	}
}
