using JieNor.Megi.Core;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.RPT
{
	public interface IRPTAccountBalanceBusiness : IRPTBizReportBusiness<RPTAccountBalanceFilterModel>
	{
		BizReportModel GetReportModel(MContext ctx, RPTAccountBalanceFilterModel filter);
	}
}
