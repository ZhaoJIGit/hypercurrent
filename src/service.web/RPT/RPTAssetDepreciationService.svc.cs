using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTAssetDepreciationService : ServiceT<BaseModel>, IRPTAssetDepreciation, IRPTBizReport<RPTAssetDepreciationFilterModel>
	{
		private readonly IRPTAssetDepreciationBussiness biz = new RPTAssetDepreciationBussiness();

		public MActionResult<string> GetBizReportJson(RPTAssetDepreciationFilterModel filter, string accessToken = null)
		{
			IRPTAssetDepreciationBussiness iRPTAssetDepreciationBussiness = biz;
			return base.RunFunc(iRPTAssetDepreciationBussiness.GetBizReportJson, filter, accessToken);
		}
	}
}
