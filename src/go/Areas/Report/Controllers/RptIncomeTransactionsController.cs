using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptIncomeTransactionsController : GoControllerBase
	{
		private IRPTIncomeTrans _rpt = null;

		public RptIncomeTransactionsController(IRPTIncomeTrans rpt)
		{
			_rpt = rpt;
		}

		[Permission("Report", "View", "")]
		public string GetReportData(RPTIncomeTransFilterModel filter)
		{
			return _rpt.GetBizReportJson(filter, null).ResultData;
		}
	}
}
