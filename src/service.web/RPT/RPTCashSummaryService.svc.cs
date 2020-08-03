using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTCashSummaryService : ServiceT<BaseModel>, IRPTCashSummary, IRPTBizReport<RPTCashSummaryFilterModel>
	{
		private readonly IRPTCashSummaryBusiness biz = new RPTCashSummaryBusiness();

		public MActionResult<string> GetBizReportJson(RPTCashSummaryFilterModel filter, string accessToken = null)
		{
			IRPTCashSummaryBusiness iRPTCashSummaryBusiness = biz;
			return base.RunFunc(iRPTCashSummaryBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
