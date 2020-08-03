using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTIncomeTransService : ServiceT<BaseModel>, IRPTIncomeTrans, IRPTBizReport<RPTIncomeTransFilterModel>
	{
		private readonly IRPTIncomeTransBusiness biz = new RPTIncomeTransBusiness();

		public MActionResult<string> GetBizReportJson(RPTIncomeTransFilterModel filter, string accessToken = null)
		{
			IRPTIncomeTransBusiness iRPTIncomeTransBusiness = biz;
			return base.RunFunc(iRPTIncomeTransBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
