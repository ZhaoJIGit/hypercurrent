using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.RPT
{
	public interface IRPTBankBillRecSummaryBusiness : IRPTBizReportBusiness<RPTBankBillRecSummaryFilterModel>
	{
		string AddReport(MContext ctx);
	}
}
