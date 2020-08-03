using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTBankStatementService : ServiceT<BaseModel>, IRPTBankStatement, IRPTBizReport<RPTBankStatementFilterModel>
	{
		private readonly IRPTBankStatementBusiness biz = new RPTBankStatementBusiness();

		public MActionResult<string> GetBizReportJson(RPTBankStatementFilterModel filter, string accessToken = null)
		{
			IRPTBankStatementBusiness iRPTBankStatementBusiness = biz;
			return base.RunFunc(iRPTBankStatementBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
