using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FA.Controllers
{
	public class FAInitController : GoControllerBase
	{
		private IBASOrgPrefixSetting orgPrefixSetting = null;

		public FAInitController(IBASOrgPrefixSetting _orgPrefixSetting)
		{
			orgPrefixSetting = _orgPrefixSetting;
		}

		public ActionResult Index(int type = 0)
		{
			ViewBag.FAType = type;
			return View();
		}

		public JsonResult SaveFAInit(BASOrgPrefixSettingModel model)
		{
			model.MPrefixModule = "FixAssets";
			MActionResult<OperationResult> data = orgPrefixSetting.UpdateOrgPrefixSettingModel(model, null);
			return base.Json(data);
		}

		public JsonResult GetFAInfo()
		{
			MActionResult<BASOrgPrefixSettingModel> orgPrefixSettingModel = orgPrefixSetting.GetOrgPrefixSettingModel("FixAssets", null);
			BASOrgPrefixSettingModel resultData = orgPrefixSettingModel.ResultData;
			DateTime mFABeginDate = resultData.MFABeginDate;
			resultData.MConversionYear = mFABeginDate.Year;
			resultData.MConversionMonth = mFABeginDate.Month;
			return base.Json(orgPrefixSettingModel);
		}
	}
}
