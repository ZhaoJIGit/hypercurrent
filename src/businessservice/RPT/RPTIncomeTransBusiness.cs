using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTIncomeTransBusiness : IRPTIncomeTransBusiness, IRPTBizReportBusiness<RPTIncomeTransFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTIncomeTransFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => new RPTIncomeTransRepository().GetIncomeTransactionsList(filter, ctx));
		}
	}
}
