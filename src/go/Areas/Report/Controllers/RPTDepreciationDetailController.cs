using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RPTDepreciationDetailController : GoControllerBase
	{
		private IRPTDepreciationDetail _rpt = null;

		public RPTDepreciationDetailController(IRPTDepreciationDetail rpt)
		{
			_rpt = rpt;
		}

		public string GetReportData(RPTDepreciationDetailFilterModel filter)
		{
			return _rpt.GetBizReportJson(filter, null).ResultData;
		}
	}
}
