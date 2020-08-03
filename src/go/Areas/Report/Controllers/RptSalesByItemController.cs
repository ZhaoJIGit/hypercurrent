using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptSalesByItemController : GoControllerBase
	{
		private IRPTSalseByItem _rpt = null;

		public RptSalesByItemController(IRPTSalseByItem rpt)
		{
			_rpt = rpt;
		}

		[Permission("Report", "View", "")]
		public string GetReportData(RPTSalseByItemFilterModel filter)
		{
			filter.MSortBy = (filter.MSortBy ?? "1");
			return _rpt.GetBizReportJson(filter, null).ResultData;
		}
	}
}
