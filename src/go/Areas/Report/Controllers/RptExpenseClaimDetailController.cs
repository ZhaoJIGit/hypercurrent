using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptExpenseClaimDetailController : GoControllerBase
	{
		private IRPTExpenseClaim ReportService = null;

		public RptExpenseClaimDetailController(IRPTExpenseClaim report)
		{
			ReportService = report;
		}

		[Permission("Report", "View", "")]
		public string GetReportData(RPTExpenseClaimDeatailFilterModel filter)
		{
			return ReportService.GetBizSubReportJson(filter, null).ResultData;
		}
	}
}
