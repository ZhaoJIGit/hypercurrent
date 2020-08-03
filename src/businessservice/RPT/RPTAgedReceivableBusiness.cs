using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTAgedReceivableBusiness : IRPTAgedReceivableBusiness, IRPTBizReportBusiness<RPTAgedReceivableFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTAgedReceivableFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, delegate
			{
				AgedRptFilterModel filter2 = filter;
				return RPTAgedReportRepository.AgedReceivablesList(ctx, filter2);
			});
		}
	}
}
