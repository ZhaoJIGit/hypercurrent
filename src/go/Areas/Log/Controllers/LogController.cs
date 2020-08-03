using JieNor.Megi.Core;
using JieNor.Megi.Core.Log;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.FA;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Log.Controllers
{
	public class LogController : GoControllerBase
	{
		private IOptLogList _iLog = null;

		private IFAFixAssetsChange _fixAssetsChange = null;

		public LogController(IOptLogList iLog, IFAFixAssetsChange fixAssetsChange)
		{
			_iLog = iLog;
			_fixAssetsChange = fixAssetsChange;
		}

		public ActionResult BusLogList(string invoiceId, string billType)
		{
			base.ViewData["InvoiceId"] = invoiceId;
			base.ViewData["BillType"] = billType;
			return base.View();
		}

		public JsonResult GetBussinessLogList(OptLogListFilter filter)
		{
			filter.MContext = base.MContext;
			MActionResult<DataGridJson<OptLogListModel>> optLogList = _iLog.GetOptLogList(filter, null);
			return base.Json(optLogList);
		}

		public ActionResult FALogList(string itemId)
		{
			base.ViewData["ItemId"] = itemId;
			return base.View();
		}

		public JsonResult GetFALogList(FAFixAssetsChangeFilterModel filter)
		{
			MActionResult<List<FAFixAssetsChangeModel>> fixAssetsChangeLog = _fixAssetsChange.GetFixAssetsChangeLog(filter, null);
			return base.Json(fixAssetsChangeLog);
		}
	}
}
