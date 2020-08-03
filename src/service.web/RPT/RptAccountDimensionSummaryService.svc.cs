using JieNor.Megi.BusinessContract;
using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.BusinessService.RPT.GL;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RptAccountDimensionSummaryService : ServiceT<BaseModel>, IRPTAccountDimensionSummary, IRPTBizReport<RPTAccountDemensionSummaryFilterModel>
	{
		private readonly IRPTAccountDimensionSummaryBusiness biz = new AccountingDimensionSummaryBusiness();

		private readonly IRPTFilterScheme filterBiz = new RPTFilterSchemeBussiness();

		public MActionResult<string> GetBizReportJson(RPTAccountDemensionSummaryFilterModel filter, string accessToken = null)
		{
			IRPTAccountDimensionSummaryBusiness iRPTAccountDimensionSummaryBusiness = biz;
			return base.RunFunc(iRPTAccountDimensionSummaryBusiness.GetBizReportJson, filter, accessToken);
		}

		public MActionResult<OperationResult> InsertOrUpdateFilterScheme(RPTFilterSchemeModel scheme, string accessToken = null)
		{
			IRPTFilterScheme iRPTFilterScheme = filterBiz;
			return base.RunFunc(iRPTFilterScheme.InsertOrUpateFilterScheme, scheme, accessToken);
		}

		public MActionResult<List<RPTFilterSchemeModel>> GetFilterSchemeList(RPTFilterSchemeFilterModel filter, string accessToken = null)
		{
			IRPTFilterScheme iRPTFilterScheme = filterBiz;
			return base.RunFunc(iRPTFilterScheme.GetFilterSchemeList, filter, accessToken);
		}

		public MActionResult<RPTFilterSchemeModel> GetFilterScheme(string id, string accessToken = null)
		{
			IRPTFilterScheme iRPTFilterScheme = filterBiz;
			return base.RunFunc(iRPTFilterScheme.GetFilterScheme, id, accessToken);
		}

		public MActionResult<OperationResult> DeleteFilterScheme(string id, string accessToken = null)
		{
			IRPTFilterScheme iRPTFilterScheme = filterBiz;
			return base.RunFunc(iRPTFilterScheme.DeleteFilterScheme, id, accessToken);
		}
	}
}
