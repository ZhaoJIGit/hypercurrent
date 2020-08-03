using JieNor.Megi.Common.Context;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.PA;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PA.Controllers
{
	public class PayItemController : GoControllerBase
	{
		private IPAPayrollBasic Service = null;

		public PayItemController(IPAPayrollBasic service)
		{
			Service = service;
		}

		public ActionResult Index()
		{
			return base.View();
		}

		public ActionResult GetSalaryItemList()
		{
			return base.Json(Service.GetSalaryItemList(null));
		}

		public ActionResult PayItemEdit(string id, string type)
		{
			ViewBag.ItemID = id;
			ViewBag.EditType = type;
			ViewBag.IsEnableGL = ContextHelper.MContext.MRegProgress >= 12 && true;
			return View();
		}

		public ActionResult GetEditInfo(string id, string type)
		{
			if (type == "1")
			{
				return base.Json(Service.GetSalaryItemById(id, null));
			}
			return base.Json(Service.GetSalaryGroupItemById(id, null));
		}

		public ActionResult SalaryGroupItemUpdate(PAPayItemGroupModel model)
		{
			return base.Json(Service.UpdateSalaryGroupItem(model, null));
		}

		public ActionResult SalaryItemUpdate(PAPayItemModel model)
		{
			return base.Json(Service.UpdateSalaryItem(model, null));
		}

		public ActionResult GetSalaryGroupItemList()
		{
			return base.Json(Service.GetSalaryItemGroupList(null));
		}

		public ActionResult GetSalaryItemTreeList(bool removeInActive = false)
		{
			return base.Json(Service.GetSalaryItemTreeList(removeInActive, null));
		}

		public ActionResult ForbiddenItem(string ids)
		{
			return base.Json(Service.ForbiddenSalaryItem(ids, null));
		}

		public JsonResult IsCanDeleteOrInactive(ParamBase param)
		{
			MActionResult<BDIsCanDeleteModel> data = Service.IsCanDeleteOrInactive(param, null);
			return base.Json(data);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult Delete(ParamBase param)
		{
			return base.Json(Service.Delete(param, null));
		}

		public ActionResult GetDisableItemList()
		{
			return base.Json(Service.GetDisableItemList(null));
		}
	}
}
