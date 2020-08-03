using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTDepreciationDetailService : ServiceT<BaseModel>, IRPTDepreciationDetail, IRPTBizReport<RPTDepreciationDetailFilterModel>
	{
		private readonly IRPTDepreciationDetailBussiness biz = new RPTDepreciationDetailBussiness();

		public MActionResult<string> GetBizReportJson(RPTDepreciationDetailFilterModel filter, string accessToken = null)
		{
			IRPTDepreciationDetailBussiness iRPTDepreciationDetailBussiness = biz;
			return base.RunFunc(iRPTDepreciationDetailBussiness.GetBizReportJson, filter, accessToken);
		}
	}
}
