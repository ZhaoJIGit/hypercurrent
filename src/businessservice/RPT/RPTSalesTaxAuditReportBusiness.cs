using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTSalesTaxAuditReportBusiness : IRPTSalesTaxAuditReportBusiness, IRPTBizReportBusiness<RPTSalesTaxAuditReportFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTSalesTaxAuditReportFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => RPTSalesTaxAuditReportRepository.GetSalesTax(filter, ctx));
		}
	}
}
