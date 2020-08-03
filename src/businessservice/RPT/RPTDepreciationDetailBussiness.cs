using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT.Biz;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTDepreciationDetailBussiness : IRPTDepreciationDetailBussiness, IRPTBizReportBusiness<RPTDepreciationDetailFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTDepreciationDetailFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => RPTDepreciationDetailRepository.GetDepreciationDetailData(ctx, filter));
		}
	}
}
