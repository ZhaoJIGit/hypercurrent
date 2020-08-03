using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTCashFlowStatementService : ServiceT<BaseModel>, IRPTCashFlowStatement, IRPTBizReport<RPTCashFlowStatementFilterModel>
	{
		private readonly IRPTCashFlowStatementBusiness biz = new RPTCashFlowStatementBusiness();

		public MActionResult<string> GetBizReportJson(RPTCashFlowStatementFilterModel filter, string accessToken = null)
		{
			IRPTCashFlowStatementBusiness iRPTCashFlowStatementBusiness = biz;
			return base.RunFunc(iRPTCashFlowStatementBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
