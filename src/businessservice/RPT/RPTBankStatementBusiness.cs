using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTBankStatementBusiness : IRPTBankStatementBusiness, IRPTBizReportBusiness<RPTBankStatementFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTBankStatementFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, delegate
			{
				BizReportModel report = RPTBankStatementRepository.GetReport(ctx, filter);
				RPTBankBillRecSummaryRepository.UpdateReport(ctx, filter);
				return report;
			});
		}
	}
}
