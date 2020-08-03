using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.RPT.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.RPT;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptAccountDimensionSummaryController : GoControllerBase
	{
		private IRPTAccountDimensionSummary _rpt = null;

		public RptAccountDimensionSummaryController(IRPTAccountDimensionSummary rpt)
		{
			_rpt = rpt;
		}

		[Permission("Other_Reports", "View", "")]
		public ActionResult GetReportData(RPTAccountDemensionSummaryFilterModel filter)
		{
			return base.Json(_rpt.GetBizReportJson(filter, null));
		}

		[Permission("Other_Reports", "View", "")]
		public ActionResult EditFilterScheme(string id)
		{
			base.ViewData["MItemID"] = id;
			base.ViewData["RptType"] = Convert.ToInt32(BizReportType.AccountDimensionSummary);
			if (!string.IsNullOrWhiteSpace(id))
			{
				RPTFilterSchemeModel resultData = _rpt.GetFilterScheme(id, null).ResultData;
				base.ViewData["FilterScheme"] = resultData;
			}
			else
			{
				base.ViewData["FilterScheme"] = null;
			}
			return base.View("~\\Areas\\Report\\Views\\RptUC\\GL\\AccountDemensionSummaryFilter.cshtml");
		}

		[Permission("Other_Reports", "View", "")]
		public ActionResult GetFilterScheme(string id)
		{
			MActionResult<RPTFilterSchemeModel> filterScheme = _rpt.GetFilterScheme(id, null);
			return base.Json(filterScheme);
		}

		[Permission("Other_Reports", "Change", "")]
		public ActionResult SaveFilterScheme(RPTFilterSchemeModel schemeModel)
		{
			MActionResult<OperationResult> data = _rpt.InsertOrUpdateFilterScheme(schemeModel, null);
			return base.Json(data);
		}

		[Permission("Other_Reports", "View", "")]
		public ActionResult GetFilterSchemeList(RPTFilterSchemeFilterModel filter)
		{
			MActionResult<List<RPTFilterSchemeModel>> filterSchemeList = _rpt.GetFilterSchemeList(filter, null);
			return base.Json(filterSchemeList);
		}

		[Permission("Other_Reports", "View", "")]
		public ActionResult DeleteFilterScheme(string id)
		{
			MActionResult<OperationResult> data = _rpt.DeleteFilterScheme(id, null);
			return base.Json(data);
		}
	}
}
