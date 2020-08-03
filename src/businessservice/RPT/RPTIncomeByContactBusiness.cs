using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTIncomeByContactBusiness : IRPTIncomeByContactBusiness, IRPTBizReportBusiness<RPTIncomeByContactFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTIncomeByContactFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => new RPTIncomeByContactRepository().GetIncomeByContactList(filter, ctx));
		}
	}
}
