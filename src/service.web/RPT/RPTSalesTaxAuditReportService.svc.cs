using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTSalesTaxAuditReportService : ServiceT<BaseModel>, IRPTSalesTaxAuditReport, IRPTBizReport<RPTSalesTaxAuditReportFilterModel>
	{
		private readonly IRPTSalesTaxAuditReportBusiness biz = new RPTSalesTaxAuditReportBusiness();

		public MActionResult<string> GetBizReportJson(RPTSalesTaxAuditReportFilterModel filter, string accessToken = null)
		{
			IRPTSalesTaxAuditReportBusiness iRPTSalesTaxAuditReportBusiness = biz;
			return base.RunFunc(iRPTSalesTaxAuditReportBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
