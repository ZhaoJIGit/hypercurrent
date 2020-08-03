using JieNor.Megi.Core;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.RPT
{
	public interface IRPTGeneralLedgerBusiness : IRPTBizReportBusiness<RPTGeneralLedgerFilterModel>
	{
		BizReportModel GetBalanceReportModel(MContext ctx, RPTGeneralLedgerFilterModel filter);
	}
}
