using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTBankBillRecSummaryService : ServiceT<BaseModel>, IRPTBankBillRecSummary, IRPTBizReport<RPTBankBillRecSummaryFilterModel>
	{
		private readonly IRPTBankBillRecSummaryBusiness biz = new RPTBankBillRecSummaryBusiness();

		public MActionResult<string> GetBizReportJson(RPTBankBillRecSummaryFilterModel filter, string accessToken = null)
		{
			IRPTBankBillRecSummaryBusiness iRPTBankBillRecSummaryBusiness = biz;
			return base.RunFunc(iRPTBankBillRecSummaryBusiness.GetBizReportJson, filter, accessToken);
		}

		public MActionResult<string> AddReport(string accessToken)
		{
			IRPTBankBillRecSummaryBusiness iRPTBankBillRecSummaryBusiness = biz;
			return base.RunFunc(iRPTBankBillRecSummaryBusiness.AddReport, accessToken);
		}
	}
}
