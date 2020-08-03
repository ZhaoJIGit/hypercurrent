using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTSalesTaxReportService : ServiceT<BaseModel>, IRPTSalesTaxReport, IRPTBizReport<RPTSalesTaxReportFilterModel>
	{
		private readonly IRPTSalesTaxReportBusiness biz = new RPTSalesTaxReportBusiness();

		public MActionResult<string> GetBizReportJson(RPTSalesTaxReportFilterModel filter, string accessToken = null)
		{
			IRPTSalesTaxReportBusiness iRPTSalesTaxReportBusiness = biz;
			return base.RunFunc(iRPTSalesTaxReportBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
