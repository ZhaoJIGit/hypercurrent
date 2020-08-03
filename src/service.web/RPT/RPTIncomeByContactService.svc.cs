using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTIncomeByContactService : ServiceT<BaseModel>, IRPTIncomeByContact, IRPTBizReport<RPTIncomeByContactFilterModel>
	{
		private readonly IRPTIncomeByContactBusiness biz = new RPTIncomeByContactBusiness();

		public MActionResult<string> GetBizReportJson(RPTIncomeByContactFilterModel filter, string accessToken = null)
		{
			IRPTIncomeByContactBusiness iRPTIncomeByContactBusiness = biz;
			return base.RunFunc(iRPTIncomeByContactBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
