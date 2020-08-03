using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTBankAndCashSummaryBusiness : IRPTBankAndCashSummaryBusiness, IRPTBizReportBusiness<RPTBankAndCashSummaryFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTBankAndCashSummaryFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			filter.IsShowDetail = false;
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => RPTBankAndCashSummaryRepository.BankAndCashSummaryList(ctx, filter));
		}
	}
}
