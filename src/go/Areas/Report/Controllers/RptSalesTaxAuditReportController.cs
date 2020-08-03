using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptSalesTaxAuditReportController : GoControllerBase
	{
		private IRPTSalesTaxAuditReport _rpt = null;

		public RptSalesTaxAuditReportController(IRPTSalesTaxAuditReport rpt)
		{
			_rpt = rpt;
		}

		[Permission("Report", "View", "")]
		public string GetReportData(RPTSalesTaxAuditReportFilterModel filter)
		{
			return _rpt.GetBizReportJson(filter, null).ResultData;
		}
	}
}
