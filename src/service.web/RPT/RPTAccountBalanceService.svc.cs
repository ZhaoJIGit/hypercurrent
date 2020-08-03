using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTAccountBalanceService : ServiceT<BaseModel>, IRPTAccountBalance, IRPTBizReport<RPTAccountBalanceFilterModel>
	{
		private readonly IRPTAccountBalanceBusiness biz = new RPTAccountBalanceBusiness();

		public MActionResult<string> GetBizReportJson(RPTAccountBalanceFilterModel filter, string accessToken = null)
		{
			IRPTAccountBalanceBusiness iRPTAccountBalanceBusiness = biz;
			return base.RunFunc(iRPTAccountBalanceBusiness.GetBizReportJson, filter, accessToken);
		}

		public MActionResult<BizReportModel> GetReportModel(RPTAccountBalanceFilterModel filter, string accessToken = null)
		{
			IRPTAccountBalanceBusiness iRPTAccountBalanceBusiness = biz;
			return base.RunFunc(iRPTAccountBalanceBusiness.GetReportModel, filter, accessToken);
		}
	}
}
