using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTBalanceSheetService : ServiceT<BaseModel>, IRPTBalanceSheet, IRPTBizReport<RPTBalanceSheetFilterModel>
	{
		private readonly IRPTBalanceSheetBusiness biz = new RPTBalanceSheetBusiness();

		public MActionResult<string> GetBizReportJson(RPTBalanceSheetFilterModel filter, string accessToken = null)
		{
			IRPTBalanceSheetBusiness iRPTBalanceSheetBusiness = biz;
			return base.RunFunc(iRPTBalanceSheetBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
