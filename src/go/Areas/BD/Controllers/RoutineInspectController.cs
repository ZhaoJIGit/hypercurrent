using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD.RI;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.RI;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class RoutineInspectController : GoControllerBase
	{
		private IRIInspect InspectServices;

		public RoutineInspectController(IRIInspect inspectServices)
		{
			InspectServices = inspectServices;
		}

		[Permission("General_Ledger", "Change", "")]
		public ActionResult RoutineInspectSetting()
		{
			InspectServices.InitInspectItem(null);
			return base.View();
		}

		[Permission("General_Ledger", "Change", "")]
		public ActionResult SaveRoutineInspectSetting(List<RICategorySettingModel> list)
		{
			MActionResult<OperationResult> data = InspectServices.SaveInspectSetting(list, null);
			return base.Json(data);
		}

		[Permission("General_Ledger", "View", "")]
		public ActionResult GetInspectItemTreeList(BDInspectItemFilterModel filter)
		{
			List<BDInspectItemTreeModel> list = InspectServices.GetInspectItemTreeList(filter, null).ResultData;
			if (list == null)
			{
				list = new List<BDInspectItemTreeModel>();
			}
			return base.Json(list);
		}
	}
}
