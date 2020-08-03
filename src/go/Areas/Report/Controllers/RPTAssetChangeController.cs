using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RPTAssetChangeController : GoControllerBase
	{
		private IRPTAssetChange _rpt = null;

		public RPTAssetChangeController(IRPTAssetChange rpt)
		{
			_rpt = rpt;
		}

		public string GetReportData(RPTAssetChangeFilterModel filter)
		{
			return _rpt.GetBizReportJson(filter, null).ResultData;
		}
	}
}
