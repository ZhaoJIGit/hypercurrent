using JieNor.Megi.DataModel.PT;
using JieNor.Megi.ServiceContract.PT;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PT.Controllers
{
	public class PTVoucherController : PTBaseController
	{
		private IPTVoucher _ptVoucher = null;

		public PTVoucherController(IPTVoucher ptVoucher)
		{
			_ptVoucher = ptVoucher;
		}

		public ActionResult PTVoucherEdit(string id)
		{
			ViewBag.ItemID = id;
			return View();
		}

		public JsonResult SortPT(string ids)
		{
			return Json(_ptVoucher.Sort(ids, null));
		}

		public JsonResult GetPT(string id)
		{
			return Json(_ptVoucher.GetDataModel(id, false, null));
		}

		public JsonResult UpdatePT(PTVoucherModel model)
		{
			model.MOrgID = MContext.MOrgID;
			return Json(_ptVoucher.InsertOrUpdate(model, null, null));
		}

		public JsonResult DeletePT(string id)
		{
			return Json(_ptVoucher.DeleteModels(id.Split(',').ToList(), null));
		}
	}
}
