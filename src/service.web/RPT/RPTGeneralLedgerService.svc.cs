using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTGeneralLedgerService : ServiceT<BaseModel>, IRPTGeneralLedger, IRPTBizReport<RPTGeneralLedgerFilterModel>
	{
		private readonly IRPTGeneralLedgerBusiness biz = new RPTGeneralLedgerBusiness();

		public MActionResult<string> GetBizReportJson(RPTGeneralLedgerFilterModel filter, string accessToken = null)
		{
			IRPTGeneralLedgerBusiness iRPTGeneralLedgerBusiness = biz;
			return base.RunFunc(iRPTGeneralLedgerBusiness.GetBizReportJson, filter, accessToken);
		}

		public MActionResult<BizReportModel> GetBalanceReportModel(RPTGeneralLedgerFilterModel filter, string accessToken = null)
		{
			IRPTGeneralLedgerBusiness iRPTGeneralLedgerBusiness = biz;
			return base.RunFunc(iRPTGeneralLedgerBusiness.GetBalanceReportModel, filter, accessToken);
		}
	}
}
