using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.PT;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PT.Controllers
{
	public class PTBizController : PTBaseController
	{
		private IPTBiz _ptBiz = null;

		private IBDAttachment _attachment = null;

		public PTBizController(IPTBiz ptBiz, IBDAttachment attachment)
			: base(attachment)
		{
			_ptBiz = ptBiz;
			_attachment = attachment;
		}

		public ActionResult PTBizEdit(string id)
		{
			ViewBag.ItemID = id;
			return View();
		}

		public ActionResult PTBizCopy(string id)
		{
			ViewBag.CopyID = id;
			return View();
		}

		public ActionResult PTBizLogoUpload(string id)
		{
			ViewBag.CopyID = id;
			return View();
		}

		public JsonResult SortPT(string ids)
		{
			return Json(_ptBiz.Sort(ids, null));
		}

		public JsonResult GetPT(string id)
		{
			return Json(_ptBiz.GetDataModel(id, false, null));
		}

		public JsonResult UpdatePT(BDPrintSettingModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			return Json(_ptBiz.InsertOrUpdate(model, null, null));
		}

		public JsonResult CopyPT(BDPrintSettingModel model, bool? isCopyTmpl)
		{
			return Json(_ptBiz.Copy(model, isCopyTmpl.HasValue && isCopyTmpl.Value, null));
		}

		public JsonResult RemovePTLogo(string id)
		{
			BDPrintSettingModel modelData = new BDPrintSettingModel
			{
				MItemID = id,
				MLogoID = string.Empty
			};
			return Json(_ptBiz.InsertOrUpdate(modelData, "MLogoID", null));
		}

		public JsonResult DeletePT(string id)
		{
			return Json(_ptBiz.DeleteModels(id.Split(',').ToList(), null));
		}

		public JsonResult GetPTList(string bizObject)
		{
			Dictionary<string, string> resultData = _ptBiz.GetKeyValueList(bizObject, null).ResultData;
			return Json(resultData.ToList());
		}

		[HttpPost]
		public JsonResult UploadPTLogo(string id)
		{
			string empty = string.Empty;
			string mLogoID = base.UploadLogoFile(id, empty);
			if (!string.IsNullOrWhiteSpace(empty))
			{
				return base.GetJsonResult(new
				{
					isSuccess = true,
					Message = empty
				});
			}
			BDPrintSettingModel modelData = new BDPrintSettingModel
			{
				MItemID = id,
				MLogoID = mLogoID,
				IsUpdate = true
			};
			OperationResult resultData = _ptBiz.InsertOrUpdate(modelData, "MLogoID", null).ResultData;
			return base.GetJsonResult(resultData);
		}
	}
}
