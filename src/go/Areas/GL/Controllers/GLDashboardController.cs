using JieNor.Megi.Common.Context;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.GL;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.GL.Controllers
{
	public class GLDashboardController : GoControllerBase
	{
		public IGLSettlement settle = null;

		public GLDashboardController(IGLSettlement _settle)
		{
			settle = _settle;
		}

		public ActionResult Index()
		{
			return base.View();
		}

		public ActionResult IsPeirodSettled(DateTime date)
		{
			MActionResult<bool> data = settle.IsPeriodValid(date, null);
			return base.Json(data);
		}

		public ActionResult GetSettledPeriod()
		{
			MActionResult<List<string>> settledPeriod = settle.GetSettledPeriod(null);
			string nextOpenDate = base.GetNextOpenDate(settledPeriod.ResultData);
			string beginDate = ContextHelper.MContext.MBeginDate.ToString("yyyy-MM-dd");
			return base.Json(new
			{
				Data = settledPeriod.ResultData,
				BeginDate = beginDate,
				OpenDate = nextOpenDate,
				Success = settledPeriod.Success
			});
		}
	}
}
