using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTSalseByItemBusiness : IRPTSalseByItemBusiness, IRPTBizReportBusiness<RPTSalseByItemFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTSalseByItemFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => RPTSalseByItemRepository.GetSalesByItem(filter, ctx));
		}
	}
}
