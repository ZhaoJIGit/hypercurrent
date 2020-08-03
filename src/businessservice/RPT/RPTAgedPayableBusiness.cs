using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTAgedPayableBusiness : IRPTAgedPayableBusiness, IRPTBizReportBusiness<RPTAgedPayableFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTAgedPayableFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, delegate
			{
				AgedRptFilterModel filter2 = filter;
				return RPTAgedReportRepository.AgedPayablesList(ctx, filter2);
			});
		}
	}
}
