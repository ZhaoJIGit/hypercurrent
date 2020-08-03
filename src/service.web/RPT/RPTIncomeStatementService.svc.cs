using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTIncomeStatementService : ServiceT<BaseModel>, IRPTIncomeStatement, IRPTBizReport<RPTIncomeStatementFilterModel>
	{
		private readonly IRPTIncomeStatementBusiness biz = new RPTIncomeStatementBusiness();

		public MActionResult<string> GetBizReportJson(RPTIncomeStatementFilterModel filter, string accessToken = null)
		{
			IRPTIncomeStatementBusiness iRPTIncomeStatementBusiness = biz;
			return base.RunFunc(iRPTIncomeStatementBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
