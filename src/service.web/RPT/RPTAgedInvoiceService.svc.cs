using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTAgedInvoiceService : ServiceT<BaseModel>, IRPTAgedInvoice, IRPTBizReport<RPTAgeInvoiceFilterModel>
	{
		private readonly IRPTAgedInvoiceBusiness biz = new RPTAgedInvoiceBusiness();

		public MActionResult<string> GetBizReportJson(RPTAgeInvoiceFilterModel filter, string accessToken = null)
		{
			IRPTAgedInvoiceBusiness iRPTAgedInvoiceBusiness = biz;
			return base.RunFunc(iRPTAgedInvoiceBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
