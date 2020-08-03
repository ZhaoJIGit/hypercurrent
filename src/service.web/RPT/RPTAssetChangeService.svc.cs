using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTAssetChangeService : ServiceT<BaseModel>, IRPTAssetChange, IRPTBizReport<RPTAssetChangeFilterModel>
	{
		private readonly IRPTAssetChangeBussiness biz = new RPTAssetChangeBussiness();

		public MActionResult<string> GetBizReportJson(RPTAssetChangeFilterModel filter, string accessToken = null)
		{
			IRPTAssetChangeBussiness iRPTAssetChangeBussiness = biz;
			return base.RunFunc(iRPTAssetChangeBussiness.GetBizReportJson, filter, accessToken);
		}
	}
}
