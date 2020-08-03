using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptInvoiceController : GoControllerBase
	{
		private IRPTAgedInvoice _rpt = null;

		public RptInvoiceController(IRPTAgedInvoice rpt)
		{
			_rpt = rpt;
		}

		[Permission("Report", "View", "")]
		public string GetReportData(RPTAgeInvoiceFilterModel filter)
		{
			return _rpt.GetBizReportJson(filter, null).ResultData;
		}
	}
}
