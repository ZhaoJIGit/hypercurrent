using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTCashSummaryBusiness : IRPTCashSummaryBusiness, IRPTBizReportBusiness<RPTCashSummaryFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTCashSummaryFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			filter.IsShowDetail = true;
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => RPTBankAndCashSummaryRepository.BankAndCashSummaryList(ctx, filter));
		}
	}
}
