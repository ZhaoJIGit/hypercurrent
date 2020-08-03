using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTSalesTaxReportBusiness : IRPTSalesTaxReportBusiness, IRPTBizReportBusiness<RPTSalesTaxReportFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTSalesTaxReportFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => RPTSalesTaxReportRepository.GetSalesTax(filter, ctx));
		}
	}
}
