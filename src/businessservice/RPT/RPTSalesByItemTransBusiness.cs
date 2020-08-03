using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTSalesByItemTransBusiness : IRPTSalesByItemTransBusiness, IRPTBizReportBusiness<RPTSalesByItemTransFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTSalesByItemTransFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => RPTSalesByItemTransRepository.GetSalesByItemTransactions(filter, ctx));
		}
	}
}
