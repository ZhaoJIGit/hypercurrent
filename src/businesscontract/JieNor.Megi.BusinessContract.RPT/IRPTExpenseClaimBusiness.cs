using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.RPT
{
	public interface IRPTExpenseClaimBusiness : IRPTBizReportBusiness<RPTExpenseClaimFilterModel>
	{
		string GetBizSubReportJson(MContext ctx, RPTExpenseClaimDeatailFilterModel filter);
	}
}
