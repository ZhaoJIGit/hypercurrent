using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT.Biz;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTAssetChangeBussiness : IRPTAssetChangeBussiness, IRPTBizReportBusiness<RPTAssetChangeFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTAssetChangeFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => RPTAssetChangeRepository.GetAssetChangeData(ctx, filter));
		}
	}
}
