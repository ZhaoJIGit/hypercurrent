using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RPTDepreciationSummaryController : GoControllerBase
	{
		private IRPTDepreciationSummary _rpt = null;

		public RPTDepreciationSummaryController(IRPTDepreciationSummary rpt)
		{
			_rpt = rpt;
		}

		public string GetReportData(RPTDepreciationSummaryFilterModel filter)
		{
			return _rpt.GetBizReportJson(filter, null).ResultData;
		}
	}
}
