using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class BDVoucherSettingController : GoControllerBase
	{
		private IBDVoucherSetting setting;

		public BDVoucherSettingController(IBDVoucherSetting _setting)
		{
			setting = _setting;
		}

		[Permission("General_Ledger", "Change", "")]
		public ActionResult VoucherSetting()
		{
			return base.View();
		}

		[Permission("General_Ledger", "Change", "")]
		public ActionResult SaveVoucherSetting(List<BDVoucherSettingModel> list)
		{
			return base.Json(setting.SaveVoucherSetting(list, null));
		}

		[Permission("General_Ledger", "Change", "")]
		public ActionResult GetVoucherSetting()
		{
			return base.Json(setting.GetVoucherSettingCategoryList(null));
		}
	}
}
