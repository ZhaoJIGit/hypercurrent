using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTExpenseClaimService : ServiceT<BaseModel>, IRPTExpenseClaim, IRPTBizReport<RPTExpenseClaimFilterModel>
	{
		private readonly IRPTExpenseClaimBusiness biz = new RPTExpenseClaimBusiness();

		public MActionResult<string> GetBizReportJson(RPTExpenseClaimFilterModel filter, string accessToken = null)
		{
			IRPTExpenseClaimBusiness iRPTExpenseClaimBusiness = biz;
			return base.RunFunc(iRPTExpenseClaimBusiness.GetBizReportJson, filter, accessToken);
		}

		public MActionResult<string> GetBizSubReportJson(RPTExpenseClaimDeatailFilterModel filter, string accessToken = null)
		{
			IRPTExpenseClaimBusiness iRPTExpenseClaimBusiness = biz;
			return base.RunFunc(iRPTExpenseClaimBusiness.GetBizSubReportJson, filter, accessToken);
		}
	}
}
