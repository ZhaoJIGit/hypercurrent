using JieNor.Megi.Core;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptControllerBase<T> : GoControllerBase where T : ReportFilterBase
	{
		private IRPTBizReport<T> _rpt = null;

		public RptControllerBase(IRPTBizReport<T> rpt)
		{
			_rpt = rpt;
		}

		[Permission("Report", "View", "")]
		public string GetReportData(T filter)
		{
			return _rpt.GetBizReportJson(filter, null).ResultData;
		}
	}
}
