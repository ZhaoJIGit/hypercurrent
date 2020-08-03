using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.PT;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PT.Controllers
{
	public class PTSalaryListController : PTBaseController
	{
		private IPTSalaryList _ptSalaryList = null;

		private IBDAttachment _attachment = null;

		public PTSalaryListController(IPTSalaryList ptSalaryList, IBDAttachment attachment)
			: base(attachment)
		{
			_ptSalaryList = ptSalaryList;
			_attachment = attachment;
		}

		public ActionResult PTSalaryEdit(string id)
		{
			ViewBag.ItemID = id;
			return View();
		}

		public ActionResult PTSalaryCopy(string id)
		{
			ViewBag.CopyID = id;
			return View();
		}

		public ActionResult PTSalaryLogoUpload(string id)
		{
			ViewBag.CopyID = id;
			return View();
		}

		public JsonResult SortPT(string ids)
		{
			return Json(_ptSalaryList.Sort(ids, null));
		}

		public JsonResult GetPT(string id)
		{
			return Json(_ptSalaryList.GetDataModel(id, false, null));
		}

		public JsonResult UpdatePT(PAPrintSettingModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			return Json(_ptSalaryList.InsertOrUpdate(model, null, null));
		}

		public JsonResult CopyPT(PAPrintSettingModel model, bool? isCopyTmpl)
		{
			return Json(_ptSalaryList.Copy(model, isCopyTmpl.HasValue && isCopyTmpl.Value, null));
		}

		public JsonResult RemovePTLogo(string id)
		{
			PAPrintSettingModel modelData = new PAPrintSettingModel
			{
				MItemID = id,
				MLogoID = string.Empty
			};
			return Json(_ptSalaryList.InsertOrUpdate(modelData, "MLogoID", null));
		}

		public JsonResult DeletePT(string id)
		{
			return Json(_ptSalaryList.DeleteModels(id.Split(',').ToList(), null));
		}

		[HttpPost]
		public JsonResult UploadPTLogo(string id)
		{
			string empty = string.Empty;
			string mLogoID = base.UploadLogoFile(id, empty);
			if (!string.IsNullOrWhiteSpace(empty))
			{
				return GetJsonResult(new
				{
					isSuccess = true,
					Message = empty
				});
			}
			PAPrintSettingModel modelData = new PAPrintSettingModel
			{
				MItemID = id,
				MLogoID = mLogoID,
				IsUpdate = true
			};
			OperationResult resultData = _ptSalaryList.InsertOrUpdate(modelData, "MLogoID", null).ResultData;
			return GetJsonResult(resultData);
		}
	}
}
