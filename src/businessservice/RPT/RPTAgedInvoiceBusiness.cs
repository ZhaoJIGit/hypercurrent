using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTAgedInvoiceBusiness : IRPTAgedInvoiceBusiness, IRPTBizReportBusiness<RPTAgeInvoiceFilterModel>
	{
		public string GetBizReportJson(MContext ctx, RPTAgeInvoiceFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, () => RPTAgedInvoiceRepository.InvoicesAgedList(ctx, filter));
		}
	}
}
