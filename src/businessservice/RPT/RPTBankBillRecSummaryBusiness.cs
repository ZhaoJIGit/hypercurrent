using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTBankBillRecSummaryBusiness : IRPTBankBillRecSummaryBusiness, IRPTBizReportBusiness<RPTBankBillRecSummaryFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTBankBillRecSummaryFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, delegate
			{
				BizReportModel report = RPTBankBillRecSummaryRepository.GetReport(ctx, filter);
				RPTBankBillRecSummaryRepository.UpdateReport(ctx, filter);
				return report;
			});
		}

		public string AddReport(MContext ctx)
		{
			return RPTBankBillRecSummaryRepository.AddReport(ctx);
		}
	}
}
