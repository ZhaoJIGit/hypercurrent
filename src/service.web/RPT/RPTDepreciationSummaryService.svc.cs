using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTDepreciationSummaryService : ServiceT<BaseModel>, IRPTDepreciationSummary, IRPTBizReport<RPTDepreciationSummaryFilterModel>
	{
		private readonly IRPTDepreciationSummaryBussiness biz = new RPTDepreciationSummaryBussiness();

		public MActionResult<string> GetBizReportJson(RPTDepreciationSummaryFilterModel filter, string accessToken = null)
		{
			IRPTDepreciationSummaryBussiness iRPTDepreciationSummaryBussiness = biz;
			return base.RunFunc(iRPTDepreciationSummaryBussiness.GetBizReportJson, filter, accessToken);
		}
	}
}
