using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTAgedReceivableService : ServiceT<BaseModel>, IRPTAgedReceivable, IRPTBizReport<RPTAgedReceivableFilterModel>
	{
		private readonly IRPTAgedReceivableBusiness biz = new RPTAgedReceivableBusiness();

		public MActionResult<string> GetBizReportJson(RPTAgedReceivableFilterModel filter, string accessToken = null)
		{
			IRPTAgedReceivableBusiness iRPTAgedReceivableBusiness = biz;
			return base.RunFunc(iRPTAgedReceivableBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
