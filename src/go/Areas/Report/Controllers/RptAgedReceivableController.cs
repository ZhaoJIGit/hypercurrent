using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptAgedReceivableController : GoControllerBase
	{
		private IRPTAgedReceivable _rpt = null;

		public RptAgedReceivableController(IRPTAgedReceivable rpt)
		{
			_rpt = rpt;
		}

		[Permission("Report", "View", "")]
		public string GetReportData(RPTAgedReceivableFilterModel filter)
		{
			return _rpt.GetBizReportJson(filter, null).ResultData;
		}
	}
}
