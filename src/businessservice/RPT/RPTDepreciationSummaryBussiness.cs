using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT.Biz;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTDepreciationSummaryBussiness : IRPTDepreciationSummaryBussiness, IRPTBizReportBusiness<RPTDepreciationSummaryFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTDepreciationSummaryFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => RPTDepreciationSummaryRepository.GetDepreciationSummaryData(ctx, filter));
		}
	}
}
