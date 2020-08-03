using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT.Biz;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTAssetDepreciationBussiness : IRPTAssetDepreciationBussiness, IRPTBizReportBusiness<RPTAssetDepreciationFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTAssetDepreciationFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => RPTAssetDepreciationRepository.GetAssetDepreciationData(ctx, filter));
		}
	}
}
