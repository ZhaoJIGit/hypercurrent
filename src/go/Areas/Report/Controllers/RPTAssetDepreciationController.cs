using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RPTAssetDepreciationController : GoControllerBase
	{
		private IRPTAssetDepreciation _rpt = null;

		public RPTAssetDepreciationController(IRPTAssetDepreciation rpt)
		{
			_rpt = rpt;
		}

		public string GetReportData(RPTAssetDepreciationFilterModel filter)
		{
			return _rpt.GetBizReportJson(filter, null).ResultData;
		}
	}
}
