using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTAgedPayableService : ServiceT<BaseModel>, IRPTAgedPayable, IRPTBizReport<RPTAgedPayableFilterModel>
	{
		private readonly IRPTAgedPayableBusiness biz = new RPTAgedPayableBusiness();

		public MActionResult<string> GetBizReportJson(RPTAgedPayableFilterModel filter, string accessToken = null)
		{
			IRPTAgedPayableBusiness iRPTAgedPayableBusiness = biz;
			return base.RunFunc(iRPTAgedPayableBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
